using Myphones.Authentication;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Xml;
using WebReferences;

namespace Myphones.Buddies.WebReferences
{
    /// <summary>Myphones.Buddies.WSInterface</summary>
    /// <author>sgregory</author>
    /// <date>12 April 2008</date>
    /// <remarks>Registration filter</remarks>
    public enum regApplianceFilter
    {
        All = 0,
        Account,
        Company,
        FirstName,
        SurName,
        Number,
        Device,
        MACAddress,
        IP,
        ConnectionPlan,
        DebugLevel,
        Product,
        Proxy,
        Protocol
    }


    // Forwarding mail target
    public enum ForwardTarget
    {
        ToFolder = 1,
        ToMailbox = 2,
        ToEmailAddress = 3
    }

    public class WSInterface
    {
        /// <summary>Myphones.Buddies.WSInterface.IsAuthenticated</summary>
        /// <author>sgregory</author>
        /// <date>12 January 2004</date>
        /// <remarks>Returns an indication of whether authentication has run</remarks>
        /// <returns type="bool">true if we have some form of token, else false</returns>
        static public bool IsAuthenticated() { return (m_strSessToken.Length > 0); }

        /// <summary>Myphones.Buddies.WSInterface.IsAPISSLd</summary>
        /// <author>sgregory</author>
        /// <date>12 January 2004</date>
        /// <remarks>Returns an indication of whether we are talking to the API using SSL or not</remarks>
        /// <returns type="bool">true if we are using SSL</returns>
        static public bool IsAPISSLd() { return m_choseSSL; }

        /// <summary>Myphones.Buddies.WSInterface.GetSiteMoniker</summary>
        /// <author>sgregory</author>
        /// <date>12 January 2004</date>
        /// <remarks>Returns C = Cogent L = LHR etc.</remarks>
        /// <returns type="string">As above</returns>
        //static public string GetSiteMoniker() { return m_siteMoniker; }


        // Public accessors
        static public string SessionContext
        {
            get
            {
                return m_strSessToken;
            }
            set
            {
                m_strSessToken = value;
            }
        }

        /// <summary>Myphones.Buddies.WSInterface.IsInternetAvailable</summary>
        /// <author>sgregory</author>
        /// <date>22 March 2007</date>
        /// <remarks>Checks the netowrk availability</remarks>
        /// <returns type="bool">true if network available</returns>
        /// <preconditions>Service accessible</preconditions>
        static public bool IsInternetAvailable()
        {
            // Check we have a connection enabled (false positives if there is VMWare / virtual)...
            bool isNetworkOnline = NetworkInterface.GetIsNetworkAvailable();

            // Only recognizes changes related to Internet adapters
            if (isNetworkOnline)
            {
                // However, this will include all adapters -- filter by opstatus and activity
                isNetworkOnline = false;
                NetworkInterface[] interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface face in interfaces)
                {
                    if (face.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                        face.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                        face.OperationalStatus == OperationalStatus.Up)
                    {
                        IPv4InterfaceStatistics stats = face.GetIPv4Statistics();
                        if (stats.BytesReceived > 0 && stats.BytesSent > 0)
                            isNetworkOnline = true;
                    }
                }
            }

            // Log if we find it down
            if (!isNetworkOnline)
                ConsoleDriver.LogSummaryMsgToConsole("[Myphones.Buddies.Comms.WSInterface]   IsInternetAvailable - Network appears DOWN!");

            return isNetworkOnline;
        }


        /// <summary>Myphones.Buddies.WSInterface.Authenticate</summary>
        /// <author>sgregory</author>
        /// <date>12 January 2004</date>
        /// <remarks>Drives the authenticate synchronously</remarks>
        /// <param name="userName" type="string">Unique User Name</param>
        /// <param name="passwordClear" type="string">Password in clear text</param>
        /// <preconditions>Authentication Web Service is accessible</preconditions>
        /// <postconditions>Session Token data memeber is set up or exception is thrown to the caller</postconditions>
        static async public Task<bool> AuthenticateAsync(
            string userName,
            string passwordClear)
        {

            return await Task.Run(() =>
            {
                bool loggedIn = false;

                MyPhonesAuthenticationSoapClient objAuthMgmt = null;

                ConsoleDriver.LogSummaryMsgToConsole($"[Myphones.Buddies.Comms.WSInterface]   Beginning process of authenticating user against DC [{ConfigHelper.LocalSiteName}]...");

                try
                {
                   // Create a SHA-256 password hash
                   string passwordHash = ConfigHelper.SHA256Hash(passwordClear);

                   // Create a proxy to the service
                   //objAuthMgmt = new MyPhonesAuthentication();

                   // Dont delay the send of the data by asking the server to send 100 when ready
                   ServicePointManager.Expect100Continue = false;

                   // We only support TLS 1.2 nowadays
                   //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;
                   ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                   // Set up proxy requirements
                   //SetupProxy(objAuthMgmt);

                   // Try once for https, then again for http if the https fails
                   ConsoleDriver.LogSummaryMsgToConsole($"[Myphones.Buddies.Comms.WSInterface]   Checking for valid WS API protocol - DC appears to be [{ConfigHelper.LocalSiteName}]...");
                    for (int idx = 0; idx < 2; idx++)
                    {
                       // Point at the right endpoint
                       ConsoleDriver.LogSummaryMsgToConsole(String.Format("[Myphones.Buddies.Comms.WSInterface]   Checking for valid protocol - now trying [{0}]...", (protocolOrderSSLOptions[idx]) ? "https" : "http"));

                        objAuthMgmt = ConfigHelper.GetWebServiceProxyForAuthentication("Authentication", "Authentication", protocolOrderSSLOptions[idx]);

                       // Set site moniker one time
                       //m_siteMoniker = ConfigHelper.LocalSiteName; // ConfigHelper.GetLocalSiteMonikerFromURL(objAuthMgmt.Url);

                       // Make the call synchronously to get the Session Token - this will be valid for a certain number of minutes e.g. 60
                       // Authenticate - with a context
                       // This includes:
                       //		Client Dns Name		0
                       //		Client IP Address	1
                       //		Browser				2
                       //		Browser Platform	3
                       //		User Identity		4
                       //		User Is Auth'd flag	5
                       //      PortalID            6

                       DateTime localTime = DateTime.Now;
                        bool localTimeIsDSTime = localTime.IsDaylightSavingTime();
                        DateTime utcTimeFromLocal = TimeZoneInfo.ConvertTimeToUtc(localTime);
                        TimeSpan tsDiff = localTime.Subtract(utcTimeFromLocal);
                        TimeZoneInfo tz = TimeZoneInfo.Local;
                        string timeInfo = tz.DisplayName + "#" + tsDiff.Hours;
                        string[] context = new string[7];
                        context[0] = "";
                        context[1] = "";
                        context[2] = "MPBuddies " + ConfigHelper.Buddies_VERSION; //Assembly.GetExecutingAssembly().GetName().Version.ToString();;
                       context[3] = Environment.OSVersion.Version.Major.ToString() + "." + Environment.OSVersion.Version.Minor.ToString(); //DeriveClientPlatform()
                       context[4] = timeInfo;
                        context[5] = "";
                        context[6] = ConfigHelper.GetWebServiceProxyEndPoint("Authentication", "Authentication", protocolOrderSSLOptions[idx]).ToString();

                        RemoteClientInfoHeader remoteClientInfoHeader = new RemoteClientInfoHeader();

                        remoteClientInfoHeader.ipAddress = null;
                        remoteClientInfoHeader.userAgent = "MPBuddies " + ConfigHelper.Buddies_VERSION;

                        try
                        {
                           // Make the call
                           m_strSessToken = (objAuthMgmt.AuthenticateAsync(remoteClientInfoHeader,
                                context,
                                userName,
                                passwordHash).Result.AuthenticateResult);

                           // If we havent bailed then store the SSL use status
                           m_choseSSL = (context[6].ToLower().IndexOf("localhost") != -1) ? false : protocolOrderSSLOptions[idx];
                            ConsoleDriver.LogSummaryMsgToConsole(String.Format("[Myphones.Buddies.Comms.WSInterface]   -> Deduced protocol to access WS API on site [{0}] is [{1}]", ConfigHelper.LocalSiteName, (m_choseSSL) ? "https" : "http"));     //m_siteMoniker

                           // Remember hash too
                           m_strPasswordHash = passwordHash;

                           // And we are good to book
                           loggedIn = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                           //ConsoleDriver.LogSummaryMsgToConsole(String.Format("[Myphones.Buddies.Comms.WSInterface]   -> Exception caught in nth ({0}) attempt to authenticate on site [{1}] using [{2}] - {3}", idx, m_siteMoniker, objAuthMgmt.Url, ex.Message));
                           bool isErrorWeShouldKnowAbout;
                            string typeOfErrMsg;
                            string errMsg = ConfigHelper.ResolveExceptionToErrorMessage(ex.Message, out typeOfErrMsg, out isErrorWeShouldKnowAbout);
                            if (idx == 1 || !isErrorWeShouldKnowAbout)       // 2nd attempt has failed, or first attempt yeielded a non-comms error e.g. password match failure
                               throw new ApplicationException(ex.ToString());

                            loggedIn = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                   // Tell our caller
                   //throw new ApplicationException("Exception caught while authenticating against PS: " + ex.ToString());
                   loggedIn = false;
                }
                finally
                {
                    if (objAuthMgmt != null)
                        _ = objAuthMgmt.CloseAsync();
                }

                return loggedIn;

            });
        }


        /// <summary>Myphones.Buddies.WSInterface.GetUserDetailsForLogonID</summary>
        /// <author>Simon Gregory</author>
        /// <date>22 August 2005</date>
        /// <remarks>Given a logon id, attempts to get the user details</remarks>
        /// <param name="logonId" type="string">Logon ID</param>
        /// <returns type="Myphones.Buddies.Myphones.UserManagement.SoftswitchUser[]">Array fo users</returns>
        static public async System.Threading.Tasks.Task<Myphones.UserManagement.SoftswitchUser[]> GetUserDetailsForLogonIDAsync(
            string logonId)
        {
            Myphones.UserManagement.SoftswitchUser[] users = null;

            Myphones.UserManagement.MyPhonesUsersSoapClient objUserMgmt = null;

            try
            {
                Myphones.UserManagement.SoftswitchSearchRequest searchReq = new Myphones.UserManagement.SoftswitchSearchRequest();
                searchReq.entityType = "Users";
                searchReq.includeCompanyInfo = true;
                searchReq.includeContacts = true;
                searchReq.includeDefaults = true;
                searchReq.criteria = new Myphones.UserManagement.SoftswitchSearchCriteria[1];
                searchReq.criteria[0] = new Myphones.UserManagement.SoftswitchSearchCriteria();
                searchReq.criteria[0].criteriaKey = "LogonID";
                searchReq.criteria[0].criteriaOperator = "=";
                searchReq.criteria[0].criteriaValue = logonId;
                objUserMgmt = ConfigHelper.GetWebServiceProxyForProvisioningUsers("Configure", "Users", m_choseSSL);

                Myphones.UserManagement.SoftswitchSearchResult res = null;

                Myphones.UserManagement.RemoteClientInfoHeader remoteClientInfoHeader = new Myphones.UserManagement.RemoteClientInfoHeader();

                remoteClientInfoHeader.ipAddress = null;
                remoteClientInfoHeader.userAgent = "MPBuddies " + ConfigHelper.Buddies_VERSION;

                // Make the call - make it in a loop
                for (int idx = 0; idx <= 1; idx++)
                {
                    try
                    {
                        res = (await objUserMgmt.GetEntityDetailsForSearchAsync(remoteClientInfoHeader, m_strSessToken, searchReq)).GetEntityDetailsForSearchResult;
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.IndexOf(SESS_EXPIRY_ERROR) < 0)
                            throw;
                        else
                            Reauthenticate(m_strPasswordHash);
                    }
                }

                // Anything?
                if (null != res && null != res.users)
                {
                    // Extract User Full Name if we can
                    users = res.users;
                }
            }
            catch (Exception ex)
            {
                // Tell our caller
                throw new ApplicationException("Exception caught while getting User details for [" + logonId + "]: " + ex.ToString());
            }
            finally
            {
                if (objUserMgmt != null)
                    _ = objUserMgmt.CloseAsync();
            }

            return users;
        }


        /// <summary>Myphones.Buddies.WSInterface.Reauthenticate</summary>
        /// <author>sgregory</author>
        /// <date>08 November 2004</date>
        /// <remarks>Drives the reauthentication synchronously</remarks>
        /// <param name="passwordHash" type="string">Password hash</param>
        /// <preconditions>Authentication Web Service is accessible</preconditions>
        /// <postconditions>Session Token data memeber is set up or exception is thrown to the caller</postconditions>
        static public void Reauthenticate(
            string passwordHash)
        {
            MyPhonesAuthenticationSoapClient objAuthMgmt = null;

            try
            {
                // Create a proxy to the service
                objAuthMgmt = ConfigHelper.GetWebServiceProxyForAuthentication("Authentication", "Authentication", m_choseSSL);

                RemoteClientInfoHeader remoteClientInfoHeader = new RemoteClientInfoHeader();

                remoteClientInfoHeader.ipAddress = null;
                remoteClientInfoHeader.userAgent = "MPBuddies " + ConfigHelper.Buddies_VERSION;

                // Make the call synchronously to get the Session Token - this will be valid for a certain number of minutes e.g. 60
                m_strSessToken = (objAuthMgmt.ReAuthenticateAsync(
                    remoteClientInfoHeader,
                    null,
                    m_strSessToken,
                    passwordHash).Result.ReAuthenticateResult);
            }
            catch (Exception ex)
            {
                // Tell our caller
                throw new ApplicationException("Exception caught while reauthenticating against PS: " + ex.ToString());
            }
            finally
            {
                if (objAuthMgmt != null)
                    objAuthMgmt.CloseAsync();
            }
        }

        // Company Address Book access

        /// <summary>Myphones.Buddies.WSInterface.GetAddressBookItemsForAccountID</summary>
        /// <author>sgregory</author>
        /// <date>12 December 2008</date>
        /// <remarks>Address Book details for Company Address Book (shared contacts)</remarks>
        /// <param name="accountId" type="int">Account ID</param>
        /// <param name="contactModeType" type="int">Contact Mode</param>
        /// <param name="filterKeyStart" type="string">Filter string</param>
        /// <param name="orderByNameIdx" type="int">Order By index</param>
        /// <param name="startItemIdx" type="int">Start Index</param>
        /// <param name="pageSize" type="int">Page size</param>
        /// <preconditions>UniMessage Web Service is accessible</preconditions>
        /// <postconditions>Get company address book details</postconditions>
        static public async System.Threading.Tasks.Task<XmlDocument> GetAddressBookItemsForAccountIDAsync(
            int accountId,
            int contactModeType,
            string filterKeyStart,
            int orderByNameIdx,
            int startItemIdx,
            int pageSize)
        {
            XmlDocument addressBook = null;

            //ConsoleDriver.LogSummaryMsgToConsole("[Myphones.Buddies.Comms.WSInterface] Looking up account address book items [" + contactModeType.ToString() + "]...");

            try
            {
                UserManagement.MyPhonesUsersSoapClient objUserMgmt = objUserMgmt = ConfigHelper.GetWebServiceProxyForProvisioningUsers("Configure", "Users", m_choseSSL);

                UserManagement.RemoteClientInfoHeader remoteClientInfoHeader = new UserManagement.RemoteClientInfoHeader();

                remoteClientInfoHeader.ipAddress = null;
                remoteClientInfoHeader.userAgent = "MPBuddies " + ConfigHelper.Buddies_VERSION;

                // Make the call - make it in a loop
                for (int idx = 0; idx <= 1; idx++)
                {
                    try
                    {
                        // Get address book
                        var addressBookEntries = (await objUserMgmt.GetAccountAddressBookEntriesAsXmlAsync(remoteClientInfoHeader,
                            m_strSessToken, accountId, contactModeType, false, filterKeyStart, orderByNameIdx, startItemIdx, pageSize));

                        string addressBookXml = addressBookEntries.GetAccountAddressBookEntriesAsXmlResult;
                        // Add to cache if we got something and it is differnt to the input (remember, an unresolved number producs the number itself i.e. 01288350249 -> 01288350249!)
                        if (addressBookXml != null && addressBookXml.Length > 0)
                        {
                            addressBookXml = Compression.Decompress(Convert.FromBase64String(addressBookXml));
                            //compress.
                            // Convert from base64, decompress and reload document
                            //addressBookXml = Compression.Decompress(Convert.FromBase64String(addressBookXml));
                            addressBook = new XmlDocument();
                            addressBook.LoadXml(addressBookXml);
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        //if (ex.Message.IndexOf(SESS_EXPIRY_ERROR) < 0)
                        //    throw;
                        //else
                        //    Reauthenticate(m_strPasswordHash);
                    }
                }
            }
            catch (Exception ex)
            {
                // Tell our caller
                throw new ApplicationException("Exception caught while accessing account address book details for account " + accountId.ToString() + " for contact type [" + contactModeType.ToString() + "]: " + ex.ToString());
            }
            finally
            {
                // Tidy
                //if (objUserMgmt != null)
                //    objUserMgmt.Dispose();
            }

            return addressBook;
        }

        // Private methods

        // Consts

        // Provisioning Failures
        static public string SESS_EXPIRY_ERROR = "The Session Token has expired";
        static public string ACCESS_DENIED_ERROR = "MyphonesAccessDeniedOnCallException";
        static public string NO_ROOT_FOLDER_ERROR = "'Failed to open root folder' for Mailbox [";
        static public string NO_SUBFOLDER_ERROR = "'Failed to open mailbox folder' for Mailbox [";
        static public string NO_SUBFOLDER_INBOX_QUALIFIER = "Folder [INBOX]";

        static public string VM_SERVER_BUSY = "timeout waiting on";
        static public string VM_SERVER_UNAVAILABLE = "System.Net.Sockets.SocketException";
        static public string VM_SERVER_UNAVAILABLE2 = "because the target machine actively refused";
        static public string VM_AUTH_FAILED = "Authentication failed";


        // Private data
        static private bool[] protocolOrderSSLOptions = { true, false };  // Try SSL first then http
        static private bool m_choseSSL = false;
        //static private string m_siteMoniker = "C";      // Assume Cogent for now

        static private string m_strSessToken = "";      // Our Session Token from Authentication
        static private string m_strPasswordHash = "";	// The password hash we used to get it
    }
}
