using Myphones.Buddies.Model.Event;
using Myphones.Buddies.ViewModel.Base;
using Prism.Events;

namespace Myphones.Buddies.ViewModel.Base
{
    public class PBGeneralViewModel : BaseViewModel
    {
        private IEventAggregator _eventAggregator;

        public PBGeneralViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<LogonViewEvent>().Subscribe(OnLogOn);
        }

        private void OnLogOn()
        {

        }
    }
}
