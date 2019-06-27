using Myphones.Buddies.Model.DataModel;
using Myphones.Buddies.ViewModel.Base;
using Myphones.Buddies.WebReferences;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Windows.Controls;

namespace Myphones.Buddies.ViewModel
{
    public class PBMainViewModel : BaseViewModel
    {
        private ObservableCollection<AddressBookEntry> addressBookCollection;

        /// <summary>
        /// Flags for open and close menu
        /// </summary>
        private bool openMenuVisibility;
        private bool closeMenuVisibility;
        private int scrollViewMarginLeft;
        private string activityIcon;
        private Orientation orientation = Orientation.Vertical;
        private string listTogggleIcon;

        // Create a Random object  
        private Random rand = new Random();

        public string ActivityIcon
        {
            get
            {
                return activityIcon;
            }
            set
            {
                activityIcon = value;
                OnProperyChanged("ActivityIcon");
            }
        }

        public int ScrollViewerMarginLeft
        {
            get { return scrollViewMarginLeft; }
            set
            {
                scrollViewMarginLeft = value;
                OnProperyChanged(nameof(ScrollViewerMarginLeft));
            }
        }


        public Orientation ListViewOrientation
        {
            get { return orientation; }
            set { orientation = value;
                OnProperyChanged(nameof(ListViewOrientation));
            }
        }

        public PBMainViewModel()
        {
            addressBookCollection = new ObservableCollection<AddressBookEntry>();

            activityIcon = "alarm";

            //Get AddressBookEntries
            Task.Run(async () => { await FillAddressBookEntryAsXmlAsync(); });

            OpenMenuCommand = new RelayCommand(OpenMenu);
            CloseMenuCommand = new RelayCommandWithParameter(CloseMenu);
            ListOrientationCommand = new RelayCommand(ChangeListOrientation);

            closeMenuVisibility = true;

            scrollViewMarginLeft = 0;

            orientation = Orientation.Horizontal;

            listTogggleIcon = "FormatAlignJustify";
        }

        private void ChangeListOrientation()
        {
            if (orientation == Orientation.Horizontal)
            {
                ListViewOrientation = Orientation.Vertical;
                ListTogggleIcon = "FormatColumns";
            }                
            else
            {
                ListViewOrientation = Orientation.Horizontal;
                ListTogggleIcon = "FormatAlignJustify";
            }                
        }

        private void CloseMenu(object obj)
        {
            ButtonCloseMenuVisibilty = true;
            ButtonOpenMenuVisibilty = false;
            ScrollViewerMarginLeft = 0;
        }

        private void OpenMenu()
        {
            ButtonCloseMenuVisibilty = false;
            ButtonOpenMenuVisibilty = true;
            ScrollViewerMarginLeft = 200;
            //ContactScrollViewer.Margin = new Thickness(200, 62, 0, 0);
        }

        public ObservableCollection<AddressBookEntry> AddressBook
        {
            get { return addressBookCollection; }

            set
            {
                addressBookCollection = value;

                OnProperyChanged(nameof(AddressBook));
            }
        }

        public bool ButtonOpenMenuVisibilty
        {
            get { return openMenuVisibility; }
            set
            {
                openMenuVisibility = value;
                OnProperyChanged(nameof(ButtonOpenMenuVisibilty));
            }
        }

        public bool ButtonCloseMenuVisibilty
        {
            get { return closeMenuVisibility; }
            set
            {
                closeMenuVisibility = value;
                OnProperyChanged(nameof(ButtonCloseMenuVisibilty));
            }
        }
        

        public string ListTogggleIcon
        {
            get { return listTogggleIcon; }
            set { listTogggleIcon = value;
                OnProperyChanged(nameof(ListTogggleIcon));
            }
        }

        /// <summary>
        /// The command to open menu
        /// </summary>
        public ICommand OpenMenuCommand { get; set; }

        /// <summary>
        /// The command to close menu
        /// </summary>
        public ICommand CloseMenuCommand { get; set; }

        public ICommand ListOrientationCommand { get; set; }

        public async Task FillAddressBookEntryAsXmlAsync()
        {
            ObservableCollection<AddressBookEntry> tempCollection = new ObservableCollection<AddressBookEntry>();

            XmlDocument addressBook = await WSInterface.GetAddressBookItemsForAccountIDAsync(5333, 2, "", 0, -1, -1);

            //Now loop them and pull the data, to fashion the inernal part of the Account Address Book
            XmlNodeList addrBookEntries = addressBook.SelectNodes("//a");

            if (addrBookEntries.Count > 0)
            {     
                for (int idx = 0; idx < addrBookEntries.Count; idx++)
                {
                    XmlElement entry = (XmlElement)addrBookEntries[idx];
                    Int64 addrBookId = Convert.ToInt64(entry.GetAttribute("id"));
                    string firstName = entry.GetAttribute("f");
                    string lastName = entry.GetAttribute("l");
                    string fullName = firstName + " " + lastName;
                    fullName = fullName.Trim();
                    string companyName = entry.GetAttribute("cn");
                    string department = entry.GetAttribute("d");
                    string jobTitle = entry.GetAttribute("j");
                    string busPhone = entry.GetAttribute("cvbpv");
                    string homePhone = entry.GetAttribute("cvhpv");
                    string mobilePhone = entry.GetAttribute("cvmpv");
                    string emailAddr = entry.GetAttribute("cveav");
                    int category = Convert.ToInt32(entry.GetAttribute("ct"));
                   // string activity = entry.GetAttribute("act");

                    // A array of activity  
                    string[] activiyArray = { "Available", "Away", "Busy", "Offline" };

                    // Ok to add?
                    if (busPhone.Length == 0 && homePhone.Length == 0 && mobilePhone.Length == 0 && emailAddr.Length == 0)
                        //ConsoleDriver.LogSummaryMsgToConsole(this, String.Format("  -> Skip adding [{0} {1}] as External AccountAddressBookEntry as no useful fields are set", firstName, lastName));
                        continue;
                    else if (fullName.Length > 0) // && !m_sharedContacts.ContainsKey(fullName))
                    {
                        // Create and prep the object
                        AddressBookEntry contact = new AddressBookEntry();
                        contact.isUserEntry = false;
                        contact.isInternalEntry = false;
                        contact.isMonitored = false;
                        contact.internalId = addrBookId;
                        contact.ownerFirstName = ConfigHelper.DecodeFromXml(firstName);
                        contact.ownerLastName = ConfigHelper.DecodeFromXml(lastName);
                        contact.ownerFullName = ConfigHelper.DecodeFromXml(fullName);
                        contact.ownerCompany = ConfigHelper.DecodeFromXml(companyName);
                        contact.ownerDepartment = ConfigHelper.DecodeFromXml(department);
                        contact.ownerJobTitle = ConfigHelper.DecodeFromXml(jobTitle);
                        contact.ownerLogonId = "";
                        contact.uniqueRef = "";
                        contact.timezoneInfo = "";
                        contact.primaryServiceApplianceName = "";
                        contact.primaryServiceShortCode = "";
                        contact.primaryServiceApplianceSupportsSMS = false;
                        contact.primaryServiceApplianceType = -1;
                        contact.landlineOrBusinessApplianceName = busPhone;
                        contact.homeApplianceName = homePhone;
                        contact.emailAddress = emailAddr;
                        contact.mobileApplianceName = mobilePhone;
                        contact.isBuddyAppliance = false;
                        contact.isVIPAppliance = false;
                        contact.isDenyAppliance = false;
                        contact.category = (Category)category;
                        //contact.activityStatus = Activity.Offline;  // Shared contacts dont have activity status
                        contact.activityStatus = ConfigHelper.ResolveActivityFromString(activiyArray[rand.Next(activiyArray.Length)]);

                        if (!string.IsNullOrEmpty(contact.landlineOrBusinessApplianceName))
                            contact.ownerContactNumber = contact.landlineOrBusinessApplianceName;
                        else
                            contact.ownerContactNumber = contact.mobileApplianceName;


                        switch (contact.activityStatus)
                        {
                            case Activity.Offline:
                                //contact.ActivityIcon = "HighLightOff";
                                //contact.ActivityIconBorderBackground = "#f5f6fa";
                                //contact.ActivityIconForeground = "#00000";
                                break;

                            case Activity.Away:
                                contact.ActivityIcon = "QueryBuilder";
                                contact.ActivityIconBorderBackground = "#FFFFFF";
                                contact.ActivityIconForeground = "#ffb142";
                                break;

                            case Activity.Busy:
                                contact.ActivityIcon = "MinusCircle";
                                contact.ActivityIconBorderBackground = "#FFFFFF";
                                contact.ActivityIconForeground = "#e84118";
                                break;

                            case Activity.Available:
                                contact.ActivityIcon = "CheckCircle";
                                contact.ActivityIconForeground = "#44bd32";
                                contact.ActivityIconBorderBackground = "#FFFFFF";
                                break;

                            default:
                                break;
                        }
                        tempCollection.Add(contact);
                    }
                }
            }
            AddressBook = tempCollection;
        }
    }
}
