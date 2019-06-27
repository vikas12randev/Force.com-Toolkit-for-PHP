using Myphones.Buddies.Model.DataModel;
using Myphones.Buddies.ViewModel.Base;

namespace Myphones.Buddies.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        //private ApplicationPage _mApplicationPage;

        //public ApplicationPage CurrentPage
        //{
        //    get { return _mApplicationPage; }
        //    set
        //    {
        //        if (value != _mApplicationPage)
        //        {
        //            _mApplicationPage = value;

        //            base.OnProperyChanged("CurrentPage");
        //        }
        //    }
        //}

        /// <summary>
        /// The current page of the application
        /// </summary>
        public int CurrentPage { get; set; }

        public MainWindowViewModel()
        {
            CurrentPage = (int)ApplicationPage.Login;

            //WSInterface.Authenticate("vikass", "Jeetna12");
        }
    }
}
