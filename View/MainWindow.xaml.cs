using CefSharp;
using FinSimLauncher.ViewModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinSimLauncher.View

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainVM vm;
        public MainWindow()
        {
            InitializeComponent();
            PMainVM = new MainVM();
        }
        public bool _sidePanelClosed = false;
        public bool SidePanelClosed { get { return _sidePanelClosed; } set { _sidePanelClosed = value; } }
        public MainVM PMainVM { get => vm; set => vm = value; }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(1000);
            Close();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed) 
            {
                DragMove();
            }
        }

        private void OpenCloseSlideMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Page currentPage = Framer.Content as Page;

            
            
            if (!SidePanelClosed)
            {
                SidePanelClosed = true;
                PMainVM.ChangeBrowserSize(SidePanelClosed, currentPage);
            }
            else
            {
                SidePanelClosed = false;
                PMainVM.ChangeBrowserSize(SidePanelClosed, currentPage);
            }
        }
    }
}

