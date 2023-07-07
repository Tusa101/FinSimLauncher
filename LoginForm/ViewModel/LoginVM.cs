using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
    }
}
