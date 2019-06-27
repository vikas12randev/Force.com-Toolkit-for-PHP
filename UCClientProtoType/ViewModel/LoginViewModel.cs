using Myphones.Buddies.Model.DataModel;
using Myphones.Buddies.Model.Event;
using Myphones.Buddies.ViewModel.Base;
using Myphones.Buddies.ViewModel.Expressions;
using Myphones.Buddies.WebReferences;
using Prism.Events;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Myphones.Buddies.ViewModel
{
    public class LoginViewModel : BaseValidatableBindable   
    {
        /// <summary>
        /// A flag to enable or disable the login button
        /// </summary>
        private bool m_canLogin = false;

        /// <summary>
        /// A flag for successful login
        /// </summary>
        private bool m_loggedIn = false;

        ///// <summary>        
        ///// The current page of the application
        ///// </summary>
        public int PresenceState { get; set; }

        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool LoginIsRunning { get; set; }

        public ICommand SelectectionChangedCommand;

        private IEventAggregator _eventAggregator;

        /// <summary>
        /// The command to Login
        /// </summary>
        public ICommand LogonCommand { get; set; }

        /// <summary>
        /// The command to cancel the Logon
        /// </summary>
        public ICommand CancelCommand { get; set; }

        /// <summary>
        /// Special tigger command for password box
        /// </summary>
        public ICommand PasswordChangedCommand { get; set; }

        private void SelectionChanged()
        {

        }

        public bool CanLogin
        {
            get
            {
                return m_canLogin;
            }
            set
            {
                if (m_canLogin == value)
                    return;

                SetProperty(ref m_canLogin, value);
            }
        }

        public bool LoggedIn
        {
            get { return m_loggedIn; }
            set { SetProperty(ref m_loggedIn, value); }
        } 

        private string m_username;
        private string m_password;    

        public string Username
        {
            get { return m_username; }
            set
            {
                SetProperty(ref m_username, value);
                SetLogin();
            }
        }

        public bool IsError { get; set; } = true;

        public string Password
        {
            get { return m_password; }
            set
            {
                SetProperty(ref m_password, value);
                SetLogin();
            }
        }


        private PasswordBox m_passwordBox;

        public PasswordBox LoginPasswordBox
        {
            get; set;
        }


        private void SetLogin()
        {
            if (HasErrors)
                CanLogin = false;
            else if(((!string.IsNullOrEmpty(Username) && (!string.IsNullOrEmpty(Password)))))
                CanLogin = true;

            OnProperyChanged(nameof(CanLogin));
        }

        public LoginViewModel(IEventAggregator eventAggregator)
        {
            //Enable the login button for now
            m_canLogin = false;

            //Set the error visibility to true as data binding visibilty is set to false
            m_loggedIn = true;

            //Set the default state to available
            PresenceState = (int)State.Available;

            // Create commands
            //LogonCommand = new RelayCommandWithParameter(LogonAsnc);
            LogonCommand = new RelayCommand(LogonAsnc);
            PasswordChangedCommand = new RelayCommandWithParameter(OnPasswordBoxChanged);
            CancelCommand = new RelayCommand(CancelLogon);

            //Initialize config
            ConfigHelper.InitalizeConfig();
            ConsoleDriver.InitialiseLogFileLocation();

            //Store the reference
            _eventAggregator = eventAggregator;
        }

        private void OnPasswordBoxChanged(object obj)
        {
            PasswordBox passwordBox = obj as PasswordBox;
            m_passwordBox = passwordBox;
            SetProperty(ref m_passwordBox, passwordBox);
        }

        public async void LogonAsnc()
        {
            //We have data binding visibilty set to false
            LoggedIn = true;

            await RunCommand(() => this.LoginIsRunning, async () =>
            {
                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                {
                    //Password = password;

                    await Task.Delay(100);

                    //Make an authenticaion request and wait until we get something back
                    LoggedIn = await WSInterface.AuthenticateAsync(Username, Password);

                    if(LoggedIn)
                        _eventAggregator.GetEvent<LogonViewEvent>().Publish();
                    else
                        OnProperyChanged(nameof(LoggedIn));

                    // If we got here, we logged on - get the user details
                    //var userDetails = await WSInterface.GetUserDetailsForLogonIDAsync("vikass");

                    //Myphones.UserManagement.SoftswitchUser[] users = userDetails;

                    //ConsoleDriver.LogSummaryMsgToConsole(this, " We have got the user details : " + userDetails[0].companyFullName + " : " + userDetails[0].companyId + " : " + userDetails[0].firstName);

                    //

                    
                }
            });
        }

        /// <summary>
        /// Raise an event to close down the application
        /// </summary>
        public void CancelLogon()
        {
            _eventAggregator.GetEvent<CancelViewEvent>().Publish();
        }



        #region Command Helpers

        /// <summary>
        /// Runs a command if the updating flag is not set.
        /// If the flag is true (indicating the function is already running) then the action is not run.
        /// If the flag is false (indicating no running function) then the action is run.
        /// Once the action is finished if it was run, then the flag is reset to false
        /// </summary>
        /// <param name="updatingFlag">The boolean property flag defining if the command is already running</param>
        /// <param name="action">The action to run if the command is not already running</param>
        /// <returns></returns>
        protected async Task RunCommand(Expression<Func<bool>> updatingFlag, Func<Task> action)
        {
            // Check if the flag property is true (meaning the function is already running)
            if (updatingFlag.GetPropertyValue())
                return;

            // Set the property flag to true to indicate we are running
            updatingFlag.SetPropertyValue(true);

            try
            {
                OnProperyChanged("LoginIsRunning");
                CanLogin = false;
                // Run the passed in action
                await action();
                OnProperyChanged("LoginIsRunning");
                CanLogin = true;

            }
            finally
            {
                // Set the property flag back to false now it's finished
                updatingFlag.SetPropertyValue(false);

                OnProperyChanged("LoginIsRunning");
            }
        }

        #endregion
        //public IList<State> PresenceTypes
        //{
        //    get
        //    {
        //        // Will result in a list like {"Tester", "Engineer"}
        //        return Enum.GetValues(typeof(State)).Cast<State>().ToList<State>();
        //    }
        //}

        //public State PresenceType
        //{
        //    get;
        //    set;
        //    }
        //}
    }
}
