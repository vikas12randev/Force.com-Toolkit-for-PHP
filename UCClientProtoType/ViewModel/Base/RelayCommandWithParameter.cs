using System;
using System.Windows.Input;

namespace Myphones.Buddies.ViewModel.Base
{
    /// <summary>
    /// A basic command that runs an Action
    /// </summary>
    public class RelayCommandWithParameter : ICommand
    {
        #region Private Members

        /// <summary>
        /// The action to run
        /// </summary>
        private Action<object> mAction;
        private Action<string> logonAsnc;

        #endregion

        #region Public Events

        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RelayCommandWithParameter(Action<object> action)
        {
            mAction = action;
        }

        public RelayCommandWithParameter(Action<string> logonAsnc)
        {
            this.logonAsnc = logonAsnc;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the commands Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            mAction(parameter);
        }

        public static implicit operator string(RelayCommandWithParameter v)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
