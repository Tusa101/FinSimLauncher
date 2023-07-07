using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
using FinSimLauncher.Dialogs;
using FinSimLauncher.ViewModel;
using Version = FinSimLauncher.ViewModel.Version;

namespace FinSimLauncher.View
{
    public partial class FinSimGamePage : Page
    {
        public Version outerVersion = Version.Default;
        FinSimPageVM finSimPageVM = new FinSimPageVM();
        public FinSimGamePage()
        {
            InitializeComponent();
            DataContext = new FinSimPageVM();
            finSimPageVM = (FinSimPageVM)DataContext;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(DataContext is FinSimPageVM) 
            {
                finSimPageVM.LauncherStatus = LauncherStatus.WaitingForDecision;   
            }
        }

        private void CheckCurrentVersion()
        {
            Version outerVersion = Version.Default;
            if (finSimPageVM.CheckForUpdates(ref outerVersion))
            {
                ApplyDialog applyDialog = new ApplyDialog();
                applyDialog.ShowDialog();
                if (applyDialog.Result)
                {
                    finSimPageVM.WebClient = new WebClient();
                    finSimPageVM.WebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadGameProgressChanged);
                    if (!outerVersion.Equals(Version.Default))
                    {
                        finSimPageVM.InstallGameFiles(true, outerVersion);
                        DownloadingPrB.Visibility = Visibility.Visible;
                        PBText.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        finSimPageVM.InstallGameFiles(false, Version.Default);
                        DownloadingPrB.Visibility = Visibility.Visible;
                        PBText.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                finSimPageVM.LauncherStatus = LauncherStatus.Ready;
            }
        }
        private void DownloadGameProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            finSimPageVM.DownloadingProgressPercentage = e.ProgressPercentage;
            DownloadingPrB.Value = e.ProgressPercentage;
            Percentage.Content = $"{e.ProgressPercentage}%";
            MBytes.Content = $"{Math.Round(e.BytesReceived / Math.Pow(1024, 2), 2)}MB / {Math.Round(e.TotalBytesToReceive / Math.Pow(1024, 2), 2)}MB";
            if (finSimPageVM.DownloadingProgressPercentage == 100)
            {
                DownloadingPrB.Value = 100;
                Percentage.Content = $"{100}%";
                MBytes.Visibility = Visibility.Collapsed;
                Percentage.Width = PBText.Width;
                Percentage.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                Percentage.VerticalAlignment = VerticalAlignment.Center;
                Percentage.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }
        //async Task ProgressChangerDownload
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(finSimPageVM.GameExe) && finSimPageVM.LauncherStatus == LauncherStatus.Ready)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(finSimPageVM.GameExe);
                startInfo.WorkingDirectory = System.IO.Path.Combine(finSimPageVM.RootPath, "FinSim");
                Process.Start(startInfo);
                
                Application.Current.Shutdown();
            }
            else if (finSimPageVM.LauncherStatus == LauncherStatus.WaitingForDecision) 
            {
                CheckCurrentVersion();
                if (finSimPageVM.LauncherStatus == LauncherStatus.Ready)
                {
                    MessageBox.Show("Latest game version installed!");
                }
            }
        }
    }
}
