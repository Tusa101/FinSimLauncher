using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup.Localizer;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;

namespace FinSimLauncher.ViewModel
{
    public enum LauncherStatus
    {
        Failed,
        Ready,
        DownloadingGame,
        DownloadingUpdate,
        WaitingForDecision

    }
    public struct Version
    {
        internal static Version Default = new Version(0, 0, 0);
        public short Major { get; set; }
        public short Minor { get; set; }
        public short SubMinor { get; set; }

        internal Version(short major, short minor, short subMinor)
        {
            Major = major;
            Minor = minor;
            SubMinor = subMinor;
        }

        internal Version(string version)
        {
            string[] parts = version.Split('.');
            if (parts.Length < 3)
            {
                Major = 0;
                Minor = 0;
                SubMinor = 0;
                return;
            }
            Major = short.Parse(parts[0]);
            Minor = short.Parse(parts[1]);
            SubMinor = short.Parse(parts[2]);

        }

        internal bool Equals(Version other)
        {
            if (Major == other.Major && Minor == other.Minor && SubMinor == other.SubMinor)
            {
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return $"{Major}.{Minor}.{SubMinor}";
        }
    }
     public class FinSimPageVM : ViewModelBase
    {
        private string rootPath;
        private string versionFile;
        private string version = "0.0.0";
        private string gameZip;
        private string gameExe;
        private int downloadingProgressPercentage = 0;
        private LauncherStatus _launcherStatus;
        private string _status = "null";
        private WebClient webClient;


        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        public string GameVersion
        {
            get => version;
            set
            { 
                version = value; 
                OnPropertyChanged(nameof(GameVersion));
            }
        }
        public LauncherStatus LauncherStatus
        {
            get => _launcherStatus;
            set
            {
                _launcherStatus = value;
                switch (_launcherStatus)
                {
                    case LauncherStatus.Failed:
                        {
                            Status = "Update Failed - Retry";
                        }
                        break;
                    case LauncherStatus.Ready:
                        {
                            Status = "Play";
                        }
                        break;
                    case LauncherStatus.DownloadingGame:
                        {
                            Status = "Downloading Game";
                        }
                        break;
                    case LauncherStatus.DownloadingUpdate:
                        {
                            Status = "Downloading Update";
                        }
                        break;
                    case LauncherStatus.WaitingForDecision:
                        {
                            Status = "Check Updates";
                        }
                        break;
                    default:
                        {
                            Status = "Error";
                        }
                        break;
                }
            }
        }

        public string GameExe { get => gameExe; set => gameExe = value; }
        public string GameZip { get => gameZip; set => gameZip = value; }
        public string VersionFile { get => versionFile; set => versionFile = value; }
        public string RootPath { get => rootPath; set => rootPath = value; }
        public int DownloadingProgressPercentage 
        { 
            get => downloadingProgressPercentage;
            set
            { 
                downloadingProgressPercentage = value;
                OnPropertyChanged(nameof(DownloadingProgressPercentage));
            } 
        }

        public WebClient WebClient { get => webClient; set => webClient = value; }

        public FinSimPageVM()
        {
            RootPath = Directory.GetCurrentDirectory();
            VersionFile = System.IO.Path.Combine(RootPath, "version.txt");
            GameZip = System.IO.Path.Combine(RootPath, "FinSim.zip");
            GameExe = System.IO.Path.Combine(RootPath, "FinSim", "first_proj.exe");
            LauncherStatus = LauncherStatus.Failed;
        }

        public bool CheckForUpdates(ref Version outerVersion)
        {
            if (System.IO.File.Exists(VersionFile))
            {
                Version localVersion = new Version(System.IO.File.ReadAllText(VersionFile));
                GameVersion = localVersion.ToString();
                try
                {
                    WebClient webClient = new WebClient();
                    Version onlineVersion = new Version(webClient.DownloadString("https://www.googleapis.com/drive/v3/files/1VLkTDWroBkVh5Bvc35bJML9-dUu9z3-m?alt=media&key=AIzaSyCO2k1tyV6PAIK0j3TiGeoKt-oc6FmI-6Q"));
                    if (!onlineVersion.Equals(localVersion))
                    {
                        outerVersion = onlineVersion;
                        return true;
                        
                    }
                    else 
                    {
                        LauncherStatus = LauncherStatus.Ready;
                        return false;
                        
                    }
                }
                catch (Exception ex)
                {
                    LauncherStatus = LauncherStatus.Failed;
                    MessageBox.Show($"Error checking for game updates: {ex}");
                    return false;
                }
            }
            else
            {
                outerVersion = Version.Default; 
                return true;
            }
        }

        public void InstallGameFiles(bool isUpdate, Version version)
        {
            try
            {
                if (isUpdate) 
                {
                    LauncherStatus = LauncherStatus.DownloadingUpdate;
                }
                else
                {
                    LauncherStatus = LauncherStatus.DownloadingGame;
                    version = new Version(webClient.DownloadString("https://www.googleapis.com/drive/v3/files/1VLkTDWroBkVh5Bvc35bJML9-dUu9z3-m?alt=media&key=AIzaSyCO2k1tyV6PAIK0j3TiGeoKt-oc6FmI-6Q"));
                }
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
                webClient.DownloadFileAsync(new Uri("https://www.googleapis.com/drive/v3/files/1I11XOadCGf2Qq1uQ6UVm6cND-6XiuG2f?alt=media&key=AIzaSyCO2k1tyV6PAIK0j3TiGeoKt-oc6FmI-6Q"), GameZip, version);
                
            }
            catch (Exception ex)
            {
                LauncherStatus = LauncherStatus.Failed;
                MessageBox.Show($"Error installing game files: {ex}");
            }
        }
        

        private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string onlineVersion = ((Version)e.UserState).ToString();
                ZipFile.ExtractToDirectory(GameZip, RootPath);
                System.IO.File.Delete(GameZip);

                System.IO.File.WriteAllText(VersionFile, onlineVersion);

                GameVersion = onlineVersion;
                LauncherStatus = LauncherStatus.Ready;
            }
            catch (Exception ex)
            {
                LauncherStatus = LauncherStatus.Failed;
                MessageBox.Show($"Error finishing download: {ex}");
            }
        }
    }
}
