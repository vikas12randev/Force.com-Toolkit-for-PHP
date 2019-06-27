using System.Windows;

namespace PhoneBuddy.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {            
            InitializeComponent();
            this.DataContext = new CustomMainViewModel(Application.Current.MainWindow, ApplicationService.Instance.EventAggregator);
        }
    }
}
