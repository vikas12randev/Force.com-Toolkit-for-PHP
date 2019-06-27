using Ninject;
using System.Windows;
using Myphones.Buddies.WebReferences;

namespace PhoneBuddy.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        IKernel Container = new StandardKernel();

        protected override void OnStartup(StartupEventArgs e)
        {
            
            ConfigurationContainer();
            //ComposeObjects();
            base.OnStartup(e);
            //Application.Current.MainWindow.Show();
        }

        private void ComposeObjects()
        {
            //Application.Current.MainWindow = Container.Get<MainWindow>();
        }

        private void ConfigurationContainer()
        {
            //Need to sort out dependency injection

            ConsoleDriver consoleDriver = new ConsoleDriver();
        
            Container.Bind<ConsoleDriver>().ToMethod(context => new ConsoleDriver());
            //Container.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            //Container.Bind<MainWindowViewModel>().To<CustomMainViewModel>().WithConstructorArgument<Window>(Container.Get<MainWindow>());
            //Container.Bind<MainWindow>().To<MainWindowViewModel>();
           
            //Container.Bind<Login>();
            //Container.Bind<LoginViewModel>();
            //           
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //If you do not set e.Handled to true, the application will close due to crash.
            MessageBox.Show("Application is going to close! ", "Uncaught Exception " + e.Exception);
            e.Handled = false;
        }
    }
}
