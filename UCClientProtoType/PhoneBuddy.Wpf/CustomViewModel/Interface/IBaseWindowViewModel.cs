using System.Windows;
using System.Windows.Input;

namespace PhoneBuddy.Wpf
{
    interface IBaseWindowViewModel
    {
        /// <summary>
        /// The smallest width the window can go to
        /// </summary>
        double WindowMinimumWidth { get; set; }

        /// <summary>
        /// The smallest height the window can go to
        /// </summary>
        double WindowMinimumHeight { get; set; }

        /// <summary>
        /// True if the window should be borderless because it is docked or maximized
        /// </summary>
        bool Borderless { get; }

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        int ResizeBorder { get; }

        /// <summary>
        /// The size of the resize border around the window, taking into account the outer margin
        /// </summary>
        Thickness ResizeBorderThickness { get; }

        /// <summary>
        /// The padding of the inner content of the main window
        /// </summary>
        Thickness InnerContentPadding { get; set; }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        int OuterMarginSize { get; set; }        

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        Thickness OuterMarginSizeThickness { get; }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        int WindowRadius { get; set; }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        CornerRadius WindowCornerRadius { get; }

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        int TitleHeight { get; set; }
        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        GridLength TitleHeightGridLength { get; }

        /// <summary>
        /// The current page of the application
        /// </summary>
        //ApplicationPage CurrentPage { get; set; }

        #region Commands

        /// <summary>
        /// The command to minimize the window
        /// </summary>
        ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// The command to maximize the window
        /// </summary>
        ICommand MaximizeCommand { get; set; }

        /// <summary>
        /// The command to close the window
        /// </summary>
        ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to show the system menu of the window
        /// </summary>
        ICommand MenuCommand { get; set; }

        #endregion

        #region Constructor

        #endregion

        #region Private Helpers

        /// <summary>
        /// Gets the current mouse position on the screen
        /// </summary>
        /// <returns></returns>
        Point GetMousePosition();

        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        void WindowResized();

        #endregion
    }
}
