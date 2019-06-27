using System;
using System.Windows.Input;

namespace PhoneBuddy.Wpf
{
    class CustomCommand :ICommand
    {
        #region Private Members

        /// <summary>
        /// The action to run
        /// </summary>
        private Action _execute;
        private Predicate<object> _canExecute;

        #endregion

        #region Public Events

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="action">Action that will be run</param>
        /// <param name="canExecute">Tells if an action can be executed or not</param>
        public CustomCommand(Action action)
        {
            _execute = _execute ?? throw new ArgumentNullException("execute");
            _execute = action;
        }

        #endregion


        #region Custom command methods

        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };


        /// <summary>
        /// Returns true if the command can be executed otherwise returns false
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }


        /// <summary>
        /// Executes the command's action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _execute();
        }

        #endregion
    }
}
