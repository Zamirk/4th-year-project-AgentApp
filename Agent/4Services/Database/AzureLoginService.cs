using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using Agent._2ViewModel;

namespace RAT.ZTry
{
    public class AzureLoginService
    {
        public AzureLoginService()
        {
            try
            {
                //Create our client
                Client = new MobileServiceClient("http://agentapp.azurewebsites.net");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ayy");
                System.Diagnostics.Debug.WriteLine("Connection project");
                string messageBoxText = "Failed to connect to database";
                string caption = "Word Processor";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;

                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }

        private MobileServiceClient Client { get; set; } = null;
        private List<LoginAgent> loginObjects = null;
        IMobileServiceTable<LoginAgent> loginTable;

        public async Task<bool> GetLogin(string a, string b)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Level 1");

                loginTable = Client.GetTable<LoginAgent>();

                System.Diagnostics.Debug.WriteLine("Level 2");

                List<LoginAgent> items = await loginTable
                    .Where(r => r.Username == a)
                    .Where(r => r.Password == b)
                    .ToListAsync();

                System.Diagnostics.Debug.WriteLine("Level 3");

                items.Add(new LoginAgent() {Password = "Empty", Username = "Empty"});

                if (items[0].Username.Equals(a) && items[0].Password.Equals(b))
                {
                    System.Diagnostics.Debug.WriteLine("User found: " + items[0].Username);
                    UserData.HostLink = items[0].HostLink;
                    UserData.ConnectionCode = items[0].ConnectionCode;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            Client.Dispose();
        }
    }
}

