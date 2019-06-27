using System.Windows;
using System.Windows.Input;
using static PhoneBuddy.Wpf.CustomWindowResizer;
using Prism.Events;
using Myphones.Buddies.Model.Event;
using Myphones.Buddies.Model.DataModel;
using Myphones.Buddies.ViewModels;
using Myphones.Buddies.ViewModel.Base;
using System;

namespace PhoneBuddy.Wpf
{
    /// <summary>
    /// We are crating our own custom window to have full control over the styles etc.
    /// </summary>
    internal class CustomMainViewModel : MainWindowViewModel, IBaseWindowViewModel
    {
        /// <summary>
        /// The window this view model controls
        /// </summary>
        private Window mWindow;

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int mOuterMarginSize = 10;

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int mWindowRadius = 10;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;

        private IEventAggregator _eventAggregator;

        public CustomMainViewModel(Window window, IEventAggregator eventAggregator)
        {
            mWindow = (Window)window;

            mWindow = window;

            // Listen out for the window resizing
            mWindow.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                WindowResized();
            };

            // Create commands
            MinimizeCommand = new RelayCommand(() => mWindow.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => mWindow.Close());
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow, GetMousePosition()));

            // Fix window resize issue
            var resizer = new WindowResizer(mWindow);

            // Listen out for dock changes
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                mDockPosition = dock;

                // Fire off resize events
                WindowResized();
            };

            _eventAggregator = eventAggregator;

            //Subscribe to events
            this._eventAggregator.GetEvent<LogonViewEvent>().Subscribe(OnLoggedIn);
            this._eventAggregator.GetEvent<CancelViewEvent>().Subscribe(OnCancel);
        }

        private void OnCancel()
        {
            //Close down the window
            mWindow.Close();
        }

        private void OnLoggedIn()
        {
            //Now we can switch to PB Main Window
            CurrentPage = (int)ApplicationPage.PBGeneral;

            // Raise the property changed event
            base.OnProperyChanged("CurrentPage");

            mWindow.Width = 900;
            mWindow.Height = 900;
        }

        public double WindowMinimumWidth { get; set; } = 400;
        public double WindowMinimumHeight { get; set; } = 400;

        public bool Borderless { get { return (mWindow.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked); } }

        public int ResizeBorder { get { return Borderless ? 0 : 6; } }

        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        public Thickness InnerContentPadding { get; set; } = new Thickness(0);

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mOuterMarginSize;
            }
            set
            {
                mOuterMarginSize = value;
            }
        }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mWindowRadius;
            }
            set
            {
                mWindowRadius = value;
            }
        }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public int TitleHeight { get; set; } = 42;
        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + ResizeBorder); } }

        /// <summary>
        /// The current page of the application
        /// </summary>
        //public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Login;

        /// <summary>
        /// The command to minimize the window
        /// </summary>
        public ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// The command to maximize the window
        /// </summary>
        public ICommand MaximizeCommand { get; set; }

        /// <summary>
        /// The command to close the window
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to show the system menu of the window
        /// </summary>
        public ICommand MenuCommand { get; set; }

        public void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            base.OnProperyChanged(nameof(Borderless));
            base.OnProperyChanged(nameof(ResizeBorderThickness));
            base.OnProperyChanged(nameof(OuterMarginSize));
            base.OnProperyChanged(nameof(OuterMarginSizeThickness));
            base.OnProperyChanged(nameof(WindowRadius));
            base.OnProperyChanged(nameof(WindowCornerRadius));
        }

        /// <summary>
        /// Gets the current mouse position on the screen
        /// </summary>
        /// <returns></returns>
        Point GetMousePosition()
        {
            // Position of the mouse relative to the window
            var position = Mouse.GetPosition(mWindow);

            // Add the window position so its a "ToScreen"
            return new Point(position.X + mWindow.Left, position.Y + mWindow.Top);
        }

        Point IBaseWindowViewModel.GetMousePosition()
        {
            // Position of the mouse relative to the window
            var position = Mouse.GetPosition(mWindow);

            // Add the window position so its a "ToScreen"
            return new Point(position.X + mWindow.Left, position.Y + mWindow.Top);
        }
    }
}
