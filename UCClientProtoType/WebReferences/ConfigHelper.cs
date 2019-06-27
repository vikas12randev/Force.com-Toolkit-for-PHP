using Microsoft.Extensions.Configuration;
using Myphones.Authentication;
using Myphones.UserManagement;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using Activity = Myphones.Buddies.Model.DataModel.Activity;

namespace Myphones.Buddies.WebReferences
{
    public class ConfigHelper
    {
        /// <summary>Myphones.Buddies.ConfigHelper.Buddies_DEFAULT_CAPTION</summary>
        static public string Buddies_DEFAULT_CAPTION = "Phone Buddy";

        // Versioning information
        public const string Buddies_VERSION = "1.0.0.0";
        public const string Buddies_LONGVERSION = "1.0.0.0";
        public const string Buddies_DATE = "20 May 2019";


        /// <summary>ConfigHelper.MYPHONES_CAPTION_SHORT</summary>
        public const string MYPHONES_CAPTION_SHORT = "MyConsole";
        /// <summary>ConfigHelper.NUMBER_WITHHELD</summary>
        public const string NUMBER_WITHHELD = "Number withheld";
        /// <summary>ConfigHelper.NUMBER_UNAVAILABLE</summary>
        public const string NUMBER_UNAVAILABLE = "Number unavailable";
        /// <summary>ConfigHelper.NUMBER_CTI_SERVICES</summary>
        public const string NUMBER_CTI_SERVICES = "cti-user";
        /// <summary>ConfigHelper.NUMBER_UNKNOWN</summary>
        //public const string NUMBER_UNKNOWN = "an unknown number";

        /// <summary>ConfigHelper.MY_PHONE_CATEGORY</summary>
        public const string MY_PHONE_CATEGORY = "My Phone";

        /// <summary>ConfigHelper.FONT_SIZE_DEFAULT</summary>
        public const int FONT_SIZE_DEFAULT = 8;
        /// <summary>ConfigHelper.IMAGE_SIZE_DEFAULT</summary>
        public const int IMAGE_SIZE_DEFAULT = 16;

        // Phone Types
        public const int APPLIANCE_TYPE_VIRTUAL_PHONE = 6;
        public const int APPLIANCE_TYPE_NORMAL_PHONE = 10;
        public const int APPLIANCE_TYPE_CONSOLE = 20;

        // Alert moniker
        public const string ALERT_MONIKER = "{[ALERT]}";
        public const string CRLF_MONIKER = "{CRLF}";


        // Toaster popup constants for last call and last text
        public const int TOASTER_CALL_POPUPDOWN_DELAY = 500;
        private const int TOASTER_CALLANDTEXT_POPUPSHOW_DEFAULT_DELAY = 10000;
        public const int TOASTER_TEXT_POPUPDOWN_DELAY = 500;
        public const int TOASTER_MAX_CONTENT_SIZE = 115;

        // How long to wait before changing Available to Away (~10 mins)
        public const int AWAY_IDLE_TIME_AFTER = 480;
        public const int UNAWAY_IDLE_TIME_AFTER = 5;

        // Call History depth
        public const int CALL_HISTORY_DEPTH_DAYS = -3;          // 3 days back
        public const int CALL_HISTORY_MAX_ITEMS = 500;          // 500 items max
        public const int CALL_MISSED_AFTER_SECS = 30;           // 30 secs after ringing do we declare this call missed

        private const int DEFAULT_MAX_QUICKCALL_CONTACTS = 10;  // Max by default if no overrides

        // Context menu indexes
        public const int CONTEXTMENU_IDX_DIAGNOSTIC_LOG = 0;
        public const int CONTEXTMENU_IDX_SEP_1 = 1;
        public const int CONTEXTMENU_IDX_WEB_PORTAL = 2;
        public const int CONTEXTMENU_IDX_SEP_3 = 3;
        public const int CONTEXTMENU_IDX_ACTIVITY_TODAY = 4;
        public const int CONTEXTMENU_IDX_CONTACT_LIST = 5;
        public const int CONTEXTMENU_IDX_SEP_6 = 6;
        public const int CONTEXTMENU_IDX_CALL_RECORDING_DOWNLOADER = 7;
        public const int CONTEXTMENU_IDX_SEP_8 = 8;
        public const int CONTEXTMENU_IDX_PHONE = 9;
        public const int CONTEXTMENU_IDX_SEP_10 = 10;
        public const int CONTEXTMENU_IDX_SETTINGS = 11;
        public const int CONTEXTMENU_IDX_CHANGE_STATUS = 12;
        public const int CONTEXTMENU_IDX_SEP_13 = 13;
        public const int CONTEXTMENU_IDX_ABOUT = 14;
        public const int CONTEXTMENU_IDX_EXIT = 15;
        public const int CONTEXTMENU_NUM_ITEMS = CONTEXTMENU_IDX_EXIT + 1;

        // Enum for feature code
        public enum FeatureCode
        {
            ForwardAutoAttendant = 7,
            ForwardFailoverCalls = 6,
            ForwardAllCallsToVM = 5,
            DivertAllCallsToPhone = 4,
            DivertCallsToPhoneOnBusyNoAnswer = 3,
            FollowFindMe = 2,
            ForwardCallsToVMOnBusyNoAnswer = 1,
            NoneOfTheAbove = 0,
            OutboundAlias = -1
        }

        // Enum for feature subcode
        public enum FeatureSubCode
        {
            DivertCallsToPhoneOnNoAnswer = 6,
            DivertCallsToPhoneOnBusy = 5,
            FollowMe = 4,
            FindMe = 3,
            ForwardCallsToVMOnNoAnswer = 2,
            ForwardCallsToVMOnBusy = 1,
            NotApplicable = 0
        }

        // Enum for icon status
        public enum IconStatus
        {
            Blank = -1,
            Person = 0,
            Available,
            Away,
            Busy,
            Chat,
            ExtendedAway,
            Invisible,
            Logon,
            Logout,
            Offline,
            NewMessage,
            PhoneCall,
            Email,
            Unavailable,
            BeingCalled,
            InCall,
            Unknown
        }

        // Enum for call status
        public enum CallStatus
        {
            Unknown = -1,
            Idle = 0,
            Ringing,
            InCall,
            OnHold,
            Terminated
        }

        // Direction of a call - only required / valid on the initial setup (Ringing)
        // i.e. not used on InCall, OnHold etc.
        public enum CallDirection
        {
            Unknown = 0,
            Inbound = 1,
            Outbound = 2
        }

        // Enum for call verb - what we decide we are actually doing
        public enum CallVerb
        {
            Unknown = -1,
            Redirect,
            Answer,
            Reject,
            Hold,
            Resume,
            Park,
            ParkPickup,
            Transfer,
            QueueUp,
            QueueDown,
            QueueExpire,
            Hangup
        }

        // Enum for scope
        public enum ClientNotificationScope
        {
            User,
            Group,
            Account
        };


        // Strings representing the modes a PB can be in
        public const string BUDDIES_PERSONAL = "Personal";
        public const string BUDDIES_PERSONAL_ENH = "PersonalEnh";
        public const string BUDDIES_RECEPTIONIST = "Receptionist";

        // VM access via web
        public const string VOICEMAIL_WEB_PAGE_URL = "http://{3}/cgi-bin/vm-cd.cgi?action=audio&folder={0}&mailbox={1}&msgid={2}&format=WAV";


        private static IConfiguration config;

        public static IConfiguration AppConfig
        {
            get { return config; }
        }

        /// <summary>
        /// Initialize the config file
        /// </summary>
        public static void InitalizeConfig()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");

            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

        }

        static public string SHA256Hash(string input)
        {
            // Convert the passed string to a byte array
            System.Text.ASCIIEncoding e = new ASCIIEncoding();
            byte[] arrInput = e.GetBytes(input);

            // Simply ask the managed SHA256 (NOT Provider as that dont work on XP) to give us a Hash of the byte array
            byte[] hashVal = (new SHA256Managed()).ComputeHash(arrInput);
            return HexUp(hashVal).ToLower();
        }


        /// <summary>Myphones.Buddies.ConfigHelper.HexUp</summary>
        /// <author>SGregory</author>
        /// <date>18 February 2002</date>
        /// <summary>Converts byte array to hex representation</summary>
        /// <remarks>
        /// Given a byte string, hex's it up for a nice easy return.
        /// This avoids nasty characters in a return buffer (classically
        /// an issue for COM)
        /// </remarks>
        /// <param name="varrInString" type="byte[]">Input string</param>
        /// <returns type="string">Hex'd representation</returns>
        /// <preconditions>Valid input array</preconditions>
        /// <postconditions>String representing Hex'd version</postconditions>
        static public string HexUp(
            byte[] varrInString)
        {
            StringBuilder sbHex = new StringBuilder();

            // Loop the byte array
            for (int lIdx = 0; lIdx < varrInString.Length; lIdx++)
            {
                // For each byte, get the corresponding ASCII character
                int i = Convert.ToInt32((byte)varrInString.GetValue(lIdx));
                // Convert this to a Hex value
                sbHex.Append(i.ToString("X2"));
            }

            // Done
            return sbHex.ToString();
        }

        /// <summary>Myphones.Buddies.ConfigHelper.GetWebServiceProxyEndPoint</summary>
        /// <author>sgregory</author>
        /// <date>12 January 2004</date>
        /// <remarks>Get Uri endpoint for relevant Myphones Web Service</remarks>
        /// <param name="serviceType" type="string">Service Type</param>
        /// <param name="serviceName" type="string">Functional name of Web Service interface e.g. Status, Authentication etc.</param>
        /// <param name="needSSL" type="bool">true if we want the https version</param>
        /// <returns type="string">Endpoint Uri</returns>
        /// <preconditions>Keyname exists in App.Config</preconditions>
        /// <postconditions>String representing endpoint is found or exception is thrown to our caller</postconditions>
        static public string GetWebServiceProxyEndPoint(
            string serviceType,
            string serviceName,
            bool needSSL)
        {

            //Console.WriteLine($" Hello  !" + AppConfig["PhoneBuddy_BaseServiceName"]);

            // Top level app config dictates if we are in test mode or live mode
            string baseServiceName = AppConfig["PhoneBuddy_BaseServiceName"];
            baseServiceName = (baseServiceName == null) ? "altos" : baseServiceName.ToLower();

            // Protocol?
            string protocol = (needSSL) ? "https" : "http";

            // Muck with ConfigHelper.LocalSiteName if it is "B" - remap to "C"
            string lsn = ConfigHelper.LocalSiteName.ToLower();
            if (lsn == "b")
                lsn = "c";

            // No exception handling by design - caller will trap
            ConsoleDriver.LogSummaryMsgToConsole(String.Format("[Myphones.Buddies.ConfigHelper] Getting URI for [{0}] API access to [{1}] using {2} protocol", serviceType, baseServiceName, protocol));
            switch (serviceType)
            {
                case "Authentication":
                    if (baseServiceName == "test") return "http://localhost/AuthenticationWS/MyPhonesAuthentication.asmx";
                    if (baseServiceName == "tringtest") return "https://prov1-t.myphones.net:8890/MyPhonesAuthentication.asmx";
                    return $"{protocol}://status-{lsn}.myphones.net:8890/MyPhonesAuthentication.asmx";

                case "Configure":
                    if (baseServiceName == "test") return "http://localhost/ConfigureWS/MyPhonesUsers.asmx";
                    if (baseServiceName == "tringtest") return "https://prov1-t.myphones.net:8895/MyPhonesUsers.asmx";
                    return $"{protocol}://status-{lsn}.myphones.net:8895/MyPhonesUsers.asmx";

                case "Operation":
                    if (baseServiceName == "test") return $"http://localhost/OperationWS/MyPhones{serviceName}.asmx";
                    if (baseServiceName == "tringtest") return $"https://prov1-t.myphones.net:8894/MyPhones{serviceName}.asmx";
                    return $"{protocol}://status-{lsn}.myphones.net:8894/MyPhones{serviceName}.asmx";

                case "CallControl":
                    if (baseServiceName == "test") return "http://localhost/CallControlWS/MyPhonesCallControl.asmx";
                    if (baseServiceName == "tringtest") return "https://prov1-t.myphones.net:8892/MyPhonesCallControl.asmx";
                    return $"{protocol}://status-{lsn}.myphones.net:8892/MyPhonesCallControl.asmx";

                case "UniMessage":
                    if (baseServiceName == "test") return "http://localhost/UniMessageWS/MyPhonesUniMessage.asmx";
                    if (baseServiceName == "tringtest") return "https://prov1-t.myphones.net:8891/MyPhonesUniMessage.asmx";
                    return $"{protocol}://status-{lsn}.myphones.net:8891/MyPhonesUniMessage.asmx";

                default:
                    // We dont support this service yet
                    throw new ArgumentException($"Unexpected serviceType [{serviceType}] found when trying to get Web Service Proxy endpoint");
            }
        }

        /// <summary>Myphones.Buddies.ConfigHelper.TermCodeToShortDescription</summary>
        /// <author>sgregory</author>
        /// <date>26 January 2009</date>
        /// <remarks>Given an error string (typically from the back end), interprets it as a user viewable string, and qualifies it with error type and visibility attributes.</remarks>
        /// <param name="error" type="string">Error as string, typically from the back end</param>
        /// <param name="typeOfErrMsg" type="out string">Slot for type of error - if filled in, provides more info about the actually error</param>
        /// <param name="isErrorWeShouldKnowAbout" type="bool">Slot for visibility i.e. how important we should know about it</param>
        /// <returns type="string">Associated user-oriented message string to be popped in a logon dialog or similar</returns>
        static public string ResolveExceptionToErrorMessage(
            string error,
            out string typeOfErrMsg,
            out bool isErrorWeShouldKnowAbout)
        {
            string errMsg = "";
            typeOfErrMsg = "";
            isErrorWeShouldKnowAbout = false;

            if (error.IndexOf("Already logged on elsewhere") >= 0)
                errMsg = "You are already logged on elsewhere with these credentials.";
            else if (error.IndexOf("User privileges too low for access") >= 0)
                errMsg = "Your account has insufficient access privileges. Please contact your support team quoting code 1.";
            else if (error.IndexOf("User privileges too high for access") >= 0)
                errMsg = "Your account has insufficient access privileges. Please contact your support team quoting code 2.";
            else if (error.IndexOf("Maximum allowed logon attempts breached") >= 0)
                errMsg = "Your user credentials are incorrect. Please reenter them at the prompt.";
            else if (error.IndexOf("Password match failure, so") >= 0)
                errMsg = "Your user credentials are incorrect. Please reenter them at the prompt.";
            else if (error.IndexOf("User not found in Provisioning") >= 0)
                errMsg = "Your user credentials are incorrect. Please reenter them at the prompt.";
            else if (error.IndexOf("Multiple Users map to LoginID in Provisioning") >= 0)
                errMsg = "Your user credentials are incorrect. Please contact your support team quoting code 3.";
            else if (error.IndexOf("User not marked as Active, so") >= 0)
                errMsg = "Your account has not been activated. Please contact your support team quoting code 4.";
            else if (error.IndexOf("User not fully activated") >= 0)
                errMsg = "Your account has not been fully activated. Please log on to the portal to update your password before starting Phone Buddy.";
            else if ((error.IndexOf("No connection could be made because the target machine actively refused it") >= 0) ||
                (error.IndexOf("A socket operation was attempted to an unreachable host") >= 0) ||
                (error.IndexOf("The remote name could not be resolved") >= 0) ||
                (error.IndexOf("The underlying connection was closed") >= 0) ||
                (error.IndexOf("A connection attempt failed because the connected party did not properly respond after a period of time") >= 0))
            {
                errMsg = "Please check your connection to the internet and try again (code 5).";
                typeOfErrMsg = "INFO: Code 5 is a CPE connectivity issue e.g. DNS lookup or firewall issue.";
                isErrorWeShouldKnowAbout = true;
            }
            else if (error.IndexOf("Can't connect to MySQL server on") >= 0)
            {
                errMsg = "Please try again in a few moments (code 6).";
                isErrorWeShouldKnowAbout = true;
            }
            else
            {
                errMsg = "Please contact your support team quoting code 7 - detail: " + error;
                typeOfErrMsg = "INFO: Code 7 is a generic application issue.";
                isErrorWeShouldKnowAbout = true;
            }

            return errMsg;
        }


        // W/S Proxy Access

        /// <summary>Myphones.Buddies.ConfigHelper.GetWebServiceProxyForAuthentication</summary>
        /// <author>sgregory</author>
        /// <date>10 November 2006</date>
        /// <remarks>Creates a proxy for interacting with Authentication services, adds a custom header giving more information about the caller, and returns the proxy ref to the caller</remarks>
        /// <param name="serviceType" type="string">Type of the service</param>
        /// <param name="serviceName" type="string">Name of the service</param>
        /// <param name="needSSL" type="bool">true if we need SSL</param>
        /// <returns type="MyPhonesAuthentication">Reference to a proxy for interactiing with Authentication services</returns>
        static public MyPhonesAuthenticationSoapClient GetWebServiceProxyForAuthentication(
            string serviceType,
            string serviceName,
            bool needSSL)
        {
            //Get the proxy binding and address
            var proxyBindingAddress = GetProxyBindingAndAddress(serviceType, serviceName, needSSL);

            MyPhonesAuthenticationSoapClient objAuthMgmt = new MyPhonesAuthenticationSoapClient(proxyBindingAddress.Item1, proxyBindingAddress.Item2);            

           //Will uncomment the blow code when Auto Provisioning is done

            //OrderedDictionary collIni = (ConfigHelper.AutoProvSettings != null) ? ConfigHelper.AutoProvSettings : ConfigHelper.BuildCollectionFromIni(ConfigHelper.LastAutoProvSettings);
            //if (collIni != null && collIni.Contains("Debug-LogExtraInfo") && (string)collIni["Debug-LogExtraInfo"] == "1")
            //{
            //    StackFrame frame = new StackFrame(1);
            //    var methodName = frame.GetMethod().Name;
            //    ConsoleDriver.LogSummaryMsgToConsole($"[Myphones.Buddies.ConfigHelper]  >>> MyPhonesAuthentication URI for DC [{ConfigHelper.LocalSiteName}] is [{address.Uri}] - will call [{methodName}] ***");
            //}
            return objAuthMgmt;
        }


        /// <summary>Myphones.Buddies.ConfigHelper.GetWebServiceProxyForProvisioningUsers</summary>
        /// <author>sgregory</author>
        /// <date>10 November 2006</date>
        /// <remarks>Creates a proxy for interacting with UserManagement services, adds a custom header giving more information about the caller, and returns the proxy ref to the caller</remarks>
        /// <param name="serviceType" type="string">Type of the service</param>
        /// <param name="serviceName" type="string">Name of the service</param>
        /// <param name="needSSL" type="bool">true if we need SSL</param>
        /// <returns type="MyPhonesUsers">Reference to a proxy for interactiing with UserManagement services</returns>
        static public MyPhonesUsersSoapClient GetWebServiceProxyForProvisioningUsers(
            string serviceType,
            string serviceName,
            bool needSSL)
        {
            //Get the proxy binding and address
            var proxyBindingAddress = GetProxyBindingAndAddress(serviceType, serviceName, needSSL);

            MyPhonesUsersSoapClient objWS = new MyPhonesUsersSoapClient(proxyBindingAddress.Item1, proxyBindingAddress.Item2);

            //Will uncomment the blow code when Auto Provisioning is done

            //OrderedDictionary collIni = (ConfigHelper.AutoProvSettings != null) ? ConfigHelper.AutoProvSettings : ConfigHelper.BuildCollectionFromIni(ConfigHelper.LastAutoProvSettings);
            //if (collIni != null && collIni.Contains("Debug-LogExtraInfo") && (string)collIni["Debug-LogExtraInfo"] == "1")
            //{
            //    StackFrame frame = new StackFrame(1);
            //    var methodName = frame.GetMethod().Name;
            //    ConsoleDriver.LogSummaryMsgToConsole($"[Myphones.Buddies.ConfigHelper]  >>> MyPhonesUsers URI for DC [{ConfigHelper.LocalSiteName}] is [{objWS.Url}] - will call [{methodName}] ***");
            //}
            return objWS;
        }

        /// <summary>
        /// Returns HttpBinding and EndPoint Address to create a proxy class
        /// </summary>
        /// <param name="serviceType" type="string">Type of the service</param>
        /// <param name="serviceName" type="string">Name of the service</param>
        /// <param name="needSSL" type="bool">true if we need SSL</param>
        /// <returns></returns>
        private static (BasicHttpsBinding, EndpointAddress) GetProxyBindingAndAddress(string serviceType, string serviceName, bool needSSL)
        {
            BasicHttpsBinding binding = new BasicHttpsBinding();

            EndpointAddress address = new EndpointAddress(ConfigHelper.GetWebServiceProxyEndPoint(serviceType, serviceName, needSSL));

            return (binding, address);
        }


        /// <summary>Myphones.Buddies.ConfigHelper.GetWebServiceProxyForProvisioningUsers</summary>
        /// <author>sgregory</author>
        /// <date>10 November 2006</date>
        /// <remarks>Creates a proxy for interacting with UserManagement services, adds a custom header giving more information about the caller, and returns the proxy ref to the caller</remarks>
        /// <param name="serviceType" type="string">Type of the service</param>
        /// <param name="serviceName" type="string">Name of the service</param>
        /// <param name="needSSL" type="bool">true if we need SSL</param>
        /// <returns type="MyPhonesUsers">Reference to a proxy for interactiing with UserManagement services</returns>
        //static public UserManagement.MyPhonesUsersSoap GetWebServiceProxyForProvisioningUsers(
        //    string serviceType,
        //    string serviceName,
        //    bool needSSL)
        //{
        //    UserManagement.MyPhonesUsersSoap objWS;

        //    //OrderedDictionary collIni = (ConfigHelper.AutoProvSettings != null) ? ConfigHelper.AutoProvSettings : ConfigHelper.BuildCollectionFromIni(ConfigHelper.LastAutoProvSettings);
        //    //if (collIni != null && collIni.Contains("Debug-LogExtraInfo") && (string)collIni["Debug-LogExtraInfo"] == "1")
        //    //{
        //    //    StackFrame frame = new StackFrame(1);
        //    //    var methodName = frame.GetMethod().Name;
        //    //    ConsoleDriver.LogSummaryMsgToConsole($"[Myphones.Buddies.ConfigHelper]  >>> MyPhonesUsers URI for DC [{ConfigHelper.LocalSiteName}] is [{objWS.Url}] - will call [{methodName}] ***");
        //    //}
        //    //return objWS;
        //}


        /// <summary>Myphones.Buddies.ConfigHelper.LocalSiteName</summary>
        /// <author>sgregory</author>
        /// <date>08 November 2004</date>
        /// <remarks>Gets / sets the value of the Local Site Name aka DC moniker. Even though there are copies of this field in UserInfo and CompanyInfo...
        /// we dont get access to those until AFTER successful logon...but we need th evalue before - in order to targget the correct DC on the first W/S call.</remarks>
        /// <returns type="Myphones.UserManagement.SoftswitchCompany">Company segment</returns>
        static public string LocalSiteName
        {
            get
            {
                return m_localSiteName;
            }
            set
            {
                m_localSiteName = value;
                //ConsoleDriver.LogSummaryMsgToConsole("[Myphones.Buddies.ConfigHelper] LocalSiteName updated to be [" + m_localSiteName + "]");
            }
        }


        /// <summary>Myphones.Buddies.ConfigHelper.FileLogLocation</summary>
        /// <author>sgregory</author>
        /// <date>08 October 2008</date>
        /// <remarks>Gets / sets the logging location</remarks>
        static public string FileLogLocation
        {
            get
            {
                return m_fileLogLocation;
            }
            set
            {
                m_fileLogLocation = value;
            }
        }

        // XML routines

        // --------------------------------------------------------------------------------
        // Routine:     DecodeFromXml
        // Author:      sgregory
        // Date:        01 Sept 2004
        // Description: Converts an Xml encoded string back to it's native equivalent
        // --------------------------------------------------------------------------------
        static public string DecodeFromXml(
            string data)
        {
            string encodedData = data.Replace("&amp;", "&");
            encodedData = encodedData.Replace("&nbsp;", " ");
            encodedData = encodedData.Replace("&quot;", "\"");
            encodedData = encodedData.Replace("&apos;", "'");
            encodedData = encodedData.Replace("&lt;", "<");
            encodedData = encodedData.Replace("&gt;", ">");
            return encodedData;
        }


        // --------------------------------------------------------------------------------
        // Routine:     EncodeAsXml
        // Author:      sgregory
        // Date:        01 Sept 2004
        // Description: Converts an unencoded string to its Xml encoded equivalent
        // --------------------------------------------------------------------------------
        static public string EncodeAsXml(
            string data)
        {
            string encodedData = data.Replace("&", "&amp;");
            encodedData = encodedData.Replace(" ", "&nbsp;");
            encodedData = encodedData.Replace("\"", "&quot;");
            encodedData = encodedData.Replace("'", "&apos;");
            encodedData = encodedData.Replace("<", "&lt;");
            encodedData = encodedData.Replace(">", "&gt;");
            return encodedData;
        }

        

        /// <summary>Myphones.Buddies.ConfigHelper.ResolveActivityFromString</summary>
        /// <author>sgregory</author>
        /// <date>13 January 2005</date>
        /// <remarks>
        /// Given a string representing an Activty, returns an enumerated Activity type from the Activity list. One of:
        /// Unknown,
        ///     Available,
        ///     Away,
        ///     Busy,
        ///     Offline,
        ///     BeingCalled,
        ///     OnThePhone,
        ///     InferredAway    i.e. If they typed "Playing golf" etc.
        /// </remarks>
        /// <param name="activityString" type="string">Subscription type</param>
        /// <returns type="Activity">Associated enumeration if a match can be found, else Activity.Unknown</returns>
        static public Activity ResolveActivityFromString(
            string activityString)
        {
            Activity act = Activity.Unknown;
            switch (activityString)
            {
                // Basics
                case "Available": act = Activity.Available; break;
                case "Away": act = Activity.Away; break;
                case "Busy": act = Activity.Busy; break;
                case "Offline": act = Activity.Offline; break;
                case "BeingCalled": act = Activity.BeingCalled; break;
                case "OnThePhone": act = Activity.OnThePhone; break;
                // Bria specials
                case "Do not disturb": act = Activity.Busy; break;
                case "Not available for calls": act = Activity.Busy; break;
                default: break;
            }

            return act;
        }
        // Private member data

        // Start time
        static DateTime m_dtStarted = DateTime.Now;

        // Log file path
        static string m_fileLogLocation = "";

        // Home DC aka Loal Site Name aka DC moniker - we need this to fashion URLs
        static string m_localSiteName = "L";    // Vodafone if all else fails

        // Reseller, user and appliance (incl default ) details for logged on user
        static Myphones.UserManagement.SoftswitchCompany m_companyDetails = null;
        static Myphones.UserManagement.SoftswitchUser m_userDetails = null;
        static Myphones.UserManagement.SoftswitchAppliance m_defaultApplianceDetails = null;
        static Myphones.UserManagement.SoftswitchAppliance[] m_allApplianceDetailsForUser = null;

        // Autoprovisioning configuration
        static OrderedDictionary m_autoProvSettings = null;

        // Client scope for subscriptions
        static string m_clientNotificationScope = "User";

        // Show popups for various events
        static bool m_showPopupsForInboundCalls = true;
        static bool m_showPopupsForAnsweredCalls = false;
        static bool m_showPopupsForVoicemail = true;
        static bool m_showPopupsForTextMessages = true;
        static bool m_showPopupsForFaxes = true;
        static bool m_showPopupsForStatusChanges = false;
        static int m_popupType = 1;                                             // Original pop-from-bottom stylee
        static int m_showPopupsTime = (TOASTER_CALLANDTEXT_POPUPSHOW_DEFAULT_DELAY / 1000);

        // SoftPhone
        static string m_rtDir = "";
        static string m_rt0 = "";
        static string m_rt2 = "";
        static string m_rt3 = "";
        static string m_rt4 = "";
        static string m_rt8 = "";
        static bool m_runSoftPhoneInSW = false;

        // Resolve numbers to names / popups
        static int m_resolveNumbersToNames = 1;                                 // Using Our own system repository
        static int m_resolveNumbersToNamesUsing3rdPartyAsLastResort = 2;        // Using whatever 3rd party repositories we support and can be found. 0 = only, 1 = first, 2 = last, 3 = never
        static bool m_includeCompanyInfo = false;                               // Either way, see if we can get any Company info to augment the owning name with
        static bool m_linkTo3rdPartyContactInfo = false;
        static string m_chosen3rdPartyContactProvider = "3PCNONE";              // None chosen by default

        // Sounds when popups occur
        static bool m_playSounds = true;


        // Consts
        static public string LEFT_CHAT_MONIKER = "has left this chat <<";
        static public string CLICK_2_DIAL_MONIKER = "Click the Phone Buddy system tray icon to call ";

        // Credentials 

        // ACT Credentials, packed
        static string m_credentialsACT = "";

        // Salesforce Credentials, packed
        static string m_credentialsSForce = "";

        // Generic URL entry point
        static string m_credentialsGenURL = "";

        // Other creds are in a map keyed by the credentials moniker
        static Dictionary<string, string> m_credientialsOther = new Dictionary<string, string>();

        // Call Recording downloader settings
        static string m_callRecordingBaseFolder = "";                           // Where to download to

        // Persistence of user activity across sessions
        static bool m_persistStateAcrossSessions = true;

        // User-chosen state when logging on
        static string m_logonAsState = "";

        // Auto-Away
        static bool m_autoAway = false;

        // SyncLync
        static bool m_syncLync = false;

        // Start with windows logon
        static bool m_startWithWindows = false;

        // Auto logon if we have creds
        static bool m_autoLogonWithCreds = false;

        // Start with banner only mode
        static bool m_startBannerOnly = false;

        // Show on top
        static bool m_showOnTop = false;

        // Main window dialog mode
        static int m_mainDialogMode = 2;            // Top Right
        static int m_mainDialogModeScreenIdx = 0;   // Primary Screen (i.e. "1")

        // Optional extras
        static string m_optionalExtras = "";

        // Last set of AutoProvisioning settings, in case we cant contact the host
        static string m_lastAutoProvSettings = "";

        // Dialler options
        // a) Clipboard
        static bool m_dialFromClipboard = false;
        // b) Protocol dialling
        static bool m_dialProtocolHyperlinks = true;
        // c) Confim before dialling
        static bool m_dialPreConfirm = true;
        // d) Show number presentation options floating dialog
        static bool m_showNumberPresOptions = false;

        // CTI options
        static bool m_showCTIDialogs = false;
        static int m_ctiDialogMode = 0;

        // My Contacts options
        // a) Group by default
        static bool m_groupContactsByDefault = false;       // Internal Directory
        static bool m_peristGroupContactsSettings = false;  // Perists the expanded / collapsed state for Internal Directory (only)
        static bool m_groupOtherContactsByDefault = false;  // Personal / Shared
        // b) Double click internal contact
        static bool m_liveContactsSearchByDefault = false;
        // c) Double click internal contact
        static bool m_dialContactOnDoubleClick = true;

        // Filter by favourites by default
        static bool m_showFavouritesByDefault = false;

        // Filter out colleagues with no tel numbers
        static bool m_showCallableContactsByDefault = false;

        // Show or hide my contact in the internal directory
        static bool m_includeMeInInternalContacts = false;

        // News items read status
        static string m_newsItemsReadCDS = "";

        // Quick Calls bar system tray
        static bool m_quickCallBarSysTray = false;

        // Speed dials are in a map keyed by "name"
        static Dictionary<string, string> m_quickCallsMap = new Dictionary<string, string>();

        // Collapsed internal groups from peristed from last session
        static Dictionary<string, string> m_collapsedInternalGroupsList = new Dictionary<string, string>();

        // Slots for queue monitoring
        static Dictionary<int, string> m_slotsForAppliancesWithQOnList = new Dictionary<int, string>();

        // Some of those speed dials may in fact be shared VMs
        static Dictionary<string, int> m_sharedVMNewCountsMap = null;

        // Soft Phone stuff
        static int m_softPhoneRecordVolume = 30;
        static int m_softPhonePlaybackVolume = 30;
        static bool m_softPhoneCallWaitingOn = false;
        static int m_softPhoneAutoAnswerOn = 0;
        static int m_softPhoneAutoGainControlOn = 0;
        static int m_softPhoneNoiseReductionOn = 0;
        static int m_softPhoneEchoCancellationOn = 0;
        static string m_softPhoneActiveRecordVolume = "";
        static string m_softPhoneActivePlaybackVolume = "";

        // Receptionist stuff
        static bool m_receptionistActivateOnCall = false;
        static bool m_receptionistIncludeQueuing = false;
        static string m_receptionistPositionInfo = "";
    }
}
