using Myphones.Buddies.ViewModel;
using System.Windows.Controls;

namespace PhoneBuddy.Wpf.View
{
    /// <summary>
    /// Interaction logic for LoginUserControlView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel(ApplicationService.Instance.EventAggregator);
        }
    }
}
