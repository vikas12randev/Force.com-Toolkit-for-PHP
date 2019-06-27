using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Windows.Controls;
using Myphones.Buddies.ViewModel.Security;

namespace Myphones.Buddies.ViewModel.Base
{
    public class BaseValidatableBindable : BaseViewModel, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs>
           ErrorsChanged = delegate { };

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {

            if (_errors.ContainsKey(propertyName))
                return _errors[propertyName];
            else
                return null;
        }

        public bool HasErrors
        {
            get { return _errors.Count > 0; }
        }

        protected override void SetProperty<T>(ref T member, T val,
           [CallerMemberName] string propertyName = null)
        {

            base.SetProperty<T>(ref member, val, propertyName);
            ValidateProperty(propertyName, val,ref  member);
        }

        private void ValidateProperty<T>(string propertyName, T value, ref T member)
        {
            var results = new List<ValidationResult>();

            //ValidationContext context = new ValidationContext(this); 
            //context.MemberName = propertyName;
            //Validator.TryValidateProperty(value, context, results);

            if (results.Any())
            {
                //_errors[propertyName] = results.Select(c => c.ErrorMessage).ToList(); 
            }
            else
            {
                _errors.Remove(propertyName);
            }

            ValidatePropertyValue(value.ToString(), propertyName, ref member);

            ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public void AddError(string propertyName, string error)
        {
            // Add error to list
            _errors[propertyName] = new List<string>() { error };
            NotifyErrorsChanged(propertyName);
        }

        public void RemoveError(string propertyName)
        {
            // remove error
            if (_errors.ContainsKey(propertyName))
                _errors.Remove(propertyName);
            NotifyErrorsChanged(propertyName);
        }

        public void NotifyErrorsChanged(string propertyName)
        {
            // Notify
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ValidatePropertyValue<T>(string value, object property, ref T member)
        {
            PasswordBox passwordBox = member as PasswordBox;

            switch (property)
            {
                case "Username":

                    if (string.IsNullOrWhiteSpace(value))
                        AddError("Username", "Please enter username");
                    else
                        RemoveError("Username");
                    break;

                case "Password":

                    if (string.IsNullOrWhiteSpace(value))
                        AddError("Password", "Please enter password");
                    else
                        RemoveError("Password");
                    break;
            }

        }
    }
}
