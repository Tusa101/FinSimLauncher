using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace FinSimLauncher.Login
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoginVM loginVM;
        public MainWindow()
        {
            InitializeComponent();
            loginVM = new LoginVM();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void OpenTypeConnect(object sender, RoutedEventArgs e)
        {
            if (TypesConnectPanel.Visibility == Visibility.Hidden)
            {
                if (SettingsPanel.Visibility == Visibility.Visible)
                {
                    SettingsPanel.Visibility = Visibility.Hidden;
                }
                TypesConnectPanel.Visibility = Visibility.Visible;
            }
            else
            {
                TypesConnectPanel.Visibility = Visibility.Hidden;
            }
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            if (SettingsPanel.Visibility == Visibility.Hidden)
            {
                if (TypesConnectPanel.Visibility == Visibility.Visible)
                {
                    TypesConnectPanel.Visibility = Visibility.Hidden;
                }
                SettingsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                SettingsPanel.Visibility = Visibility.Hidden;
            }
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordB.Password.Length > 0)
            {
                Watermark.Visibility = Visibility.Collapsed;
            }
            else
            {
                Watermark.Visibility = Visibility.Visible;
            }
        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void TB_Email_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            WrongDatMsg.Visibility = Visibility.Hidden;
            if (TypesConnectPanel.Visibility == Visibility.Visible)
            {
                TypesConnectPanel.Visibility = Visibility.Hidden;
            }
            if (SettingsPanel.Visibility == Visibility.Visible)
            {
                SettingsPanel.Visibility = Visibility.Hidden;
            }
        }

        private void PasswordB_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            WrongDatMsg.Visibility = Visibility.Hidden;
            if (TypesConnectPanel.Visibility == Visibility.Visible)
            {
                TypesConnectPanel.Visibility = Visibility.Hidden;
            }
            if (SettingsPanel.Visibility == Visibility.Visible)
            {
                SettingsPanel.Visibility = Visibility.Hidden;
            }
        }

        private void AuthorizeAction(object sender, RoutedEventArgs e)
        {
            var enteredPassword = PasswordB.Password;
            var enteredEmail = TB_Email.Text;
            try
            {
                MailAddress m = new MailAddress(enteredEmail);
                if (loginVM.Login(enteredEmail, enteredPassword))
                {
                    WrongDatMsg.Visibility = Visibility.Hidden;
                    this.Close();
                }
                else
                {
                    WrongDatMsg.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid email\nFull error message:\n{ex}");
                WrongDatMsg.Visibility = Visibility.Visible;
            }
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                var enteredPassword = PasswordB.Password;
                var enteredEmail = TB_Email.Text;
                try
                {
                    MailAddress m = new MailAddress(enteredEmail);
                    if (loginVM.Login(enteredEmail, enteredPassword))
                    {
                        WrongDatMsg.Visibility = Visibility.Hidden;
                        this.Close();
                    }
                    else
                    {
                        WrongDatMsg.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Invalid email\nFull error message:\n{ex}");
                    WrongDatMsg.Visibility = Visibility.Visible;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
