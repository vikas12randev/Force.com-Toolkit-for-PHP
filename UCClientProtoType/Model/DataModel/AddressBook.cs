// --------------------------------------------------------------------------------
// Module:      AddressBook.cs
// Author:      sgregory
// Date:        10 November 2004
// Description: Address Book class
// --------------------------------------------------------------------------------
// $Archive: <VSS Path To File>/AddressBook.cs $
// $Revision: 1 $ changed on $Date: 10 November 2004 15:54 $
// Last changed by $Author: sgregory $
// --------------------------------------------------------------------------------
using System;

// --------------------------------------------------------------------------------
// Namespace:   Myphones.Buddies
// Author:      sgregory
// Date:        10 November 2004
// Description: Buddies namespace
// 
// --------------------------------------------------------------------------------
namespace Myphones.Buddies.Model.DataModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;                  // For StringBuilder


    // Enums

    /* Contact Type
     * 		Subscriber :    R/O view of same subscribers within the logged on users account
     * 		Adhoc :	        R/W view of adhoc contacts
     * 		AllMerged :	    All calls version for search
     */
    public enum ContactType
    {
        Subscriber = 1,
        Adhoc = 2,
        AllMerged = 3
    };


    /* Category
     * 		Undefined :         Undefined
     * 		Family :	        Family
     * 		Friends :	        Friends
     * 		Colleagues :	    Colleagues
     * 		BusinessContacts :	Business Contacts
     */
    public enum Category
    {
        Undefined = 0,
        Family,
        Friends,
        Colleagues,
        BusinessContacts
    };


    /* Activity
     *      Unknown,
     *      Available,
     *      Away,
     *      Busy,
     *      Invisible,
     *      Offline,
     *      BeingCalled,
     *      OnThePhone,
     */
    public enum Activity
    {
        Unknown,
        Available,
        Away,
        Busy,
        Offline,
        BeingCalled,
        OnThePhone,
        InferredAway    // If they typed "Playing golf" etc.
    };

    
    public class AddressBookEntry
    {
        /// <summary>Myphones.Buddies.ApplianceAddressBookEntryMap.ToString</summary>
        /// <author>sgregory</author>
        /// <date>13 January 2005</date>
        /// <remarks>Dump the contents of the object to a formatted string</remarks>
        /// <returns type="string">String representation</returns>
        public override string ToString()
        {
            StringBuilder sbObject = new StringBuilder(128);
            sbObject.Append("ID:");
            sbObject.Append(internalId.ToString());
            sbObject.Append(", L:");
            sbObject.Append(ownerLogonId);
            sbObject.Append(" -> N: ");
            sbObject.Append(ownerFullName);
            if (ownerCompany.Length > 0)
            {
                sbObject.Append(" of ");
                sbObject.Append(ownerCompany);
            }
            if (ownerDepartment.Length > 0)
            {
                sbObject.Append(" (");
                sbObject.Append(ownerDepartment);
                sbObject.Append(")");
            }
            if (ownerJobTitle.Length > 0)
            {
                sbObject.Append(" (");
                sbObject.Append(ownerJobTitle);
                sbObject.Append(")");
            }
            sbObject.Append(",UQR:[");
            sbObject.Append(uniqueRef);
            sbObject.Append("],INT:(");
            sbObject.Append(isInternalEntry.ToString());
            sbObject.Append("),MON:");
            sbObject.Append(isMonitored.ToString());
            sbObject.Append(",");
            sbObject.Append(isBuddyAppliance.ToString());
            sbObject.Append(",");
            sbObject.Append(isVIPAppliance.ToString());
            sbObject.Append(",");
            sbObject.Append(isDenyAppliance.ToString());
            sbObject.Append(",[");
            sbObject.Append(primaryServiceApplianceName);
            sbObject.Append(" (");
            sbObject.Append(primaryServiceShortCode);
            sbObject.Append(")");
            sbObject.Append(" (");
            sbObject.Append(primaryServiceApplianceSupportsSMS.ToString());
            sbObject.Append(")");
            sbObject.Append(" T(");
            sbObject.Append(primaryServiceApplianceType.ToString());
            sbObject.Append(")");
            sbObject.Append("],FN[");
            sbObject.Append(primaryServiceFriendlyName);
            sbObject.Append("],PID[");
            sbObject.Append(primaryServiceProductId.ToString());
            sbObject.Append("],[");
            if (isInternalEntry && lstAllServicePhones != null && lstAllServicePhones.Length > 0)
                sbObject.Append(String.Join(",", lstAllServicePhones));
            sbObject.Append("],S/C: [");
            if (isInternalEntry && lstAllServiceShortCodes != null && lstAllServiceShortCodes.Length > 0)
                sbObject.Append(String.Join(",", lstAllServiceShortCodes));
            sbObject.Append("],TYP: [");
            if (isInternalEntry && lstAllServiceApplianceTypes != null && lstAllServiceApplianceTypes.Length > 0)
            {
                foreach (int serviceApllianceType in lstAllServiceApplianceTypes)
                {
                    sbObject.Append(serviceApllianceType.ToString());
                    sbObject.Append(":");
                }
                sbObject.Length = sbObject.Length - 1;
            }
            sbObject.Append("],FN[");
            if (isInternalEntry && lstAllServiceFriendlyNames != null && lstAllServiceFriendlyNames.Length > 0)
                sbObject.Append(String.Join(",", lstAllServiceFriendlyNames));
            sbObject.Append("],PID[");
            if (isInternalEntry && lstAllServiceProductIds != null && lstAllServiceProductIds.Length > 0)
            {
                foreach (int serviceProductId in lstAllServiceProductIds)
                {
                    sbObject.Append(serviceProductId.ToString());
                    sbObject.Append(":");
                }
                sbObject.Length = sbObject.Length - 1;
            }
            sbObject.Append("],[");
            sbObject.Append(emailAddress);
            sbObject.Append("],BA[");
            sbObject.Append(landlineOrBusinessApplianceName);
            sbObject.Append("],HA[");
            sbObject.Append(homeApplianceName);
            sbObject.Append("],MA[");
            sbObject.Append(mobileApplianceName);
            sbObject.Append("])");
            sbObject.Append(",C: ");
            sbObject.Append(category.ToString());
            sbObject.Append(",TZ: ");
            sbObject.Append(timezoneInfo);
            sbObject.Append(",AS: ");
            sbObject.Append(activityStatus.ToString());
            sbObject.Append(" (");
            sbObject.Append(activityNote);
            sbObject.Append("),NT: ");
            sbObject.Append(note.ToString());
            sbObject.Append(",DND: ");
            sbObject.Append(dndOn.ToString());
            sbObject.Append(",CFWD: ");
            sbObject.Append(callFwdOn.ToString());
            sbObject.Append(",EXP: ");
            if (regExpires == DateTime.MinValue)
                sbObject.Append("N/a");
            else
                sbObject.Append(regExpires.ToString("yyyy-MM-dd HH:mm:ss"));

            return sbObject.ToString();
        }


        // This version just looking at the first appliance setting (normally the default appliance)
        public bool DeriveDNDFromFirstApplianceDNDOnly(
            IList<int> lstAllServiceApplianceDNDs)
        {
            bool dndOn = false;
            if (lstAllServiceApplianceDNDs != null && lstAllServiceApplianceDNDs.Count > 0)
            {
                if (lstAllServiceApplianceDNDs[0] == 1)
                    dndOn = true;
            }
            return dndOn;
        }


        public bool DeriveDNDFromApplianceDNDs(
            IList<int> lstAllServiceApplianceDNDs)
        {
            bool dndOn = false;
            if (lstAllServiceApplianceDNDs != null && lstAllServiceApplianceDNDs.Count > 0)
            {
                for (int idx = 0; idx < lstAllServiceApplianceDNDs.Count; idx++)
                {
                    if (lstAllServiceApplianceDNDs[idx] == 1)
                    {
                        dndOn = true;
                        break;
                    }
                }
            }
            return dndOn;
        }


        // This version just looking at the first appliance setting (normally the default appliance)
        public bool DeriveCallFwdFromFirstApplianceCallFwdOnly(
            IList<int> lstAllServiceApplianceCFwds)
        {
            bool callFwdOn = false;
            if (lstAllServiceApplianceCFwds != null && lstAllServiceApplianceCFwds.Count > 0)
            {
                if (lstAllServiceApplianceCFwds[0] == 1)
                    callFwdOn = true;
            }
            return callFwdOn;
        }


        public bool DeriveCallFwdFromApplianceDNDs(
            IList<int> lstAllServiceApplianceCFwds)
        {
            bool callFwdOn = false;
            if (lstAllServiceApplianceCFwds != null && lstAllServiceApplianceCFwds.Count > 0)
            {
                for (int idx = 0; idx < lstAllServiceApplianceCFwds.Count; idx++)
                {
                    if (lstAllServiceApplianceCFwds[idx] == 1)
                    {
                        callFwdOn = true;
                        break;
                    }
                }
            }
            return callFwdOn;
        }

        // Member data

        public bool isUserEntry = false;
        public bool isInternalEntry = false;
        public bool isMonitored = false;
        public bool isBuddyAppliance = false;
        public bool isVIPAppliance = false;
        public bool isDenyAppliance = false;
        public Int64 internalId { get; set; } = -1;

        public string ownerFirstName = "";
        public string ownerLastName = "";
        public string ownerFullName { get; set; } = "";
        public string ownerCompany { get; set; } = "";

        public string ownerLogonId { get; set; } = "";
        public string ownerDepartment = "";
        public string ownerJobTitle = "";
        public string uniqueRef = "";
        public string timezoneInfo = "";
        public string primaryServiceApplianceName = "";
        public string primaryServiceShortCode = "";
        public bool primaryServiceApplianceSupportsSMS = false;
        public int primaryServiceApplianceType = -1;
        public string primaryServiceFriendlyName = "";
        public int primaryServiceProductId = -1;
        public string[] lstAllServicePhones = null;
        public string[] lstAllServiceShortCodes = null;
        public int[] lstAllServiceApplianceTypes = null;
        public string[] lstAllServiceFriendlyNames = null;
        public int[] lstAllServiceProductIds = null;
        public string landlineOrBusinessApplianceName = "";
        public string homeApplianceName = "";
        public string mobileApplianceName { get; set; } = "";
        public string emailAddress { get; set; } = "";

        public Category category = Category.Undefined;
        public Activity activityStatus { get; set; } = Activity.Unknown;  // Our lowest common denominator state
        public string activityNote = "";    // e.g. note field in Bria
        public bool dndOn = false;      // aka ForwardAllToVM
        public bool callFwdOn = false;  // aka DivertAllToPhone
        public string note = "";

        // Reg expiry for this internal contact - MinValue if never registered
        public DateTime regExpires = DateTime.MinValue;

        public string ownerContactNumber { get; set; }

        //ActivityIcon related stuff
        public string ActivityIcon { get; set; }
        public string ActivityIconBackground { get; set; }

        public string ActivityIconForeground { get; set; }

        public string ActivityIconBorderBackground { get; set; }

    }


    /// <summary>Myphones.Buddies.ApplianceAddressBookEntryMap</summary>
    /// <author>sgregory</author>
    /// <date>28 December 2004</date>
    /// <remarks>Map of address book entries, based on an appliance id</remarks>
    public class AccountAddressBookEntryMap : Hashtable
    {
        /// <summary>Myphones.Buddies.Call.CallArray</summary>
        public AccountAddressBookEntryMap() : base() { }
        /// <summary>Myphones.Buddies.Call.CallArray</summary>
        public AccountAddressBookEntryMap(Hashtable c) : base(c) { }
    }


    public class MatchedAddressBookEntry
    {
        public AddressBookEntry addressBookEntry = null;
        public string wasMatchedOn = "";
        public string typeOfMatch = "";
    }
}

