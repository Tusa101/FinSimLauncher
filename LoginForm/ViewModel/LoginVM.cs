using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using CefSharp.DevTools.WebAuthn;
using Octokit;
using System.Windows;
using FinSimLauncher.View;

namespace FinSimLauncher.Login
{
    internal class LoginVM
    {
        public LoginVM() { }
        [Serializable]
        public class Emails
        {
            public string EString
            { get; set; }
            public string PString
            { get; set; }
            public Emails(string eString, string pString) 
            { 
                EString = eString;
                PString = pString;
            } 
        }

        public bool Login(string login, string password) 
        {
            WebClient webClient = new WebClient();

            var json = webClient.DownloadString("https://www.googleapis.com/drive/v3/files/1PhpXjfF2k7edZL1__Ub1STMK34SfRGrL?alt=media&key=AIzaSyAhzVXN_NGRbuEfzafUgPOSP2c78oQtGPI");

            Emails[] mailAddresses = JsonConvert.DeserializeObject<Emails[]>(json);

            foreach (var item in mailAddresses)
            {
                if (item.PString == password && item.EString == login)
                {
                    var launcherForm = new FinSimLauncher.View.MainWindow();
                    launcherForm.PMainVM.Account = login;
                    launcherForm.Show();
                    return true;
                }
            }

            return false;
        }
        
        public async Task<bool> LoginAsync(string subID)
        {
            var ghClient = new GitHubClient(new Octokit.ProductHeaderValue("FinSimTest"));
            ghClient.Credentials = new Credentials("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");

            // github variables
            var owner = "Tusa101";
            var repo = "AuthInfo";
            var branch = "master";
            var targetFile = "dat.json";
            FinSimLauncher.View.MainWindow launcherForm;
            var mainWindow = System.Windows.Application.Current.Windows
                                                 .Cast<Window>()
                                                 .FirstOrDefault(w => w is FinSimLauncher.Login.MainWindow) as FinSimLauncher.Login.MainWindow;
            try
            {
                // try to get the file (and with the file the last commit sha)
                var existingFile = await ghClient.Repository.Content.GetAllContentsByRef(owner, repo, targetFile, branch);
                var str = existingFile.First().Content;

                List<Emails> mailAddresses = JsonConvert.DeserializeObject<List<Emails>>(str);

                foreach (var item in mailAddresses)
                {
                    if (item.PString == "" && item.EString == subID)
                    {
                        mainWindow.GoogleAuthButton.IsEnabled = true;
                        

                        launcherForm = new FinSimLauncher.View.MainWindow();
                        launcherForm.PMainVM.Account = subID;
                        
                        launcherForm.Show();
                        mainWindow.Close();
                        return true;
                    }
                }

                mailAddresses.Add(new Emails(subID, ""));
                var updContent = JsonConvert.SerializeObject(mailAddresses);
                // update the file
                var updateChangeSet = await ghClient.Repository.Content.UpdateFile(owner, repo, targetFile,
                   new UpdateFileRequest("API File update", updContent, existingFile.First().Sha, branch));
                var a = updateChangeSet.Commit;

                mainWindow.GoogleAuthButton.IsEnabled = true;
                

                launcherForm = new FinSimLauncher.View.MainWindow();
                launcherForm.PMainVM.Account = subID;
                launcherForm.Show();
                mainWindow.Close();



            }
            catch (Octokit.NotFoundException)
            {
                // if file is not found, create it
                var createChangeSet = await ghClient.Repository.Content.CreateFile(owner, repo, targetFile, new CreateFileRequest("API File creation", "Hello Universe! " + DateTime.UtcNow, branch));
                return false;
            }
            return true;
        }
    }
}
