using FinSimLauncher.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinSimLauncher.View
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var _mainWindow = Application.Current.Windows
                                                 .Cast<Window>()
                                                 .FirstOrDefault(w => w is MainWindow) as MainWindow;
            _mainWindow.PMainVM.Account = "";
            var launcherForm = new FinSimLauncher.Login.MainWindow();
            launcherForm.Show();
            _mainWindow.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ind = LangCB.SelectedIndex;
            switch (ind)
            {
                case 0:
                    {
                        ResourceService.Current.ChangeCulture("ru");
                        Application.Current.Resources["StandardFontSize"] = 12.0;
                    }
                    break;
                case 1:
                    {
                        ResourceService.Current.ChangeCulture("en");
                        Application.Current.Resources["StandardFontSize"] = 16.0;
                    }
                    break;
                default:
                    {
                        ResourceService.Current.ChangeCulture("en");
                        Application.Current.Resources["StandardFontSize"] = 16.0;
                    }
                    break;
            }
        }
    }
}
