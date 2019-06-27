using Myphones.Buddies.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhoneBuddy.Wpf.View.UserControls
{
    /// <summary>
    /// Interaction logic for PBMain.xaml
    /// </summary>
    public partial class PBMainView : UserControl
    {
        public PBMainView()
        {
            InitializeComponent();
            this.DataContext = new PBMainViewModel();
        }

        //private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonCloseMenu.Visibility = Visibility.Visible;
        //    ButtonOpenMenu.Visibility = Visibility.Collapsed;
        //    ContactScrollViewer.Margin = new Thickness(200, 62, 0, 0);
        //}

        //private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonCloseMenu.Visibility = Visibility.Collapsed;
        //    ButtonOpenMenu.Visibility = Visibility.Visible;
        //    ContactScrollViewer.Margin = new Thickness(0, 62, 0, 0);
        //}

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            GridMain.Children.Clear();

            //switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            //{
            //    case "ItemHome":
            //        usc = new UserControlHome();
            //        GridMain.Children.Add(usc);
            //        break;
            //    case "ItemCreate":
            //        usc = new UserControlCreate();
            //        GridMain.Children.Add(usc);
            //        break;
            //    default:
            //        break;
            //}
        }

        //private void TextBlock_PreviewKeyDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    //FrameworkElementFactory factoryPanel = new FrameworkElementFactory(typeof(WrapPanel));

        //    //factoryPanel.SetValue(WrapPanel.IsItemsHostProperty, true);

        //    //ItemsPanelTemplate template = new ItemsPanelTemplate();

        //    //template.VisualTree = factoryPanel;

        //    //myListView.ItemsPanel = template;
        //}

        //private void TextBlock_PreviewKeyDown1(object sender, MouseButtonEventArgs e)
        //{
        //    //FrameworkElementFactory factoryPanel = new FrameworkElementFactory(typeof(StackPanel));
        //    //factoryPanel.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);

        //    //factoryPanel.SetValue(StackPanel.IsItemsHostProperty, true);

        //    //ItemsPanelTemplate template = new ItemsPanelTemplate();

        //    //template.VisualTree = factoryPanel;

        //    //myListView.ItemsPanel = template;
        //}
    }
}
