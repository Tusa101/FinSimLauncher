using FinSimLauncher.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinSimLauncher.View
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class AccountPage : Page
    {
        public AccountPage()
        {
            InitializeComponent();
        }

        private void AccountName_Loaded(object sender, RoutedEventArgs e)
        {
            var _mainWindow = Application.Current.Windows
                                                 .Cast<Window>()
                                                 .FirstOrDefault(w => w is MainWindow) as MainWindow;
            AccountName.Content = _mainWindow.PMainVM.Account;
        }
    }
}
