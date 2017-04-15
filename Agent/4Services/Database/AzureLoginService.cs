using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RAT.ZTry
{
    public class AzureLoginService
    {
        public AzureLoginService()
        {
            try
            {
                //Create our client
                Client = new MobileServiceClient("https://zmtool.azurewebsites.net/");
            }
            catch (Exception e)
            {
                
            }
        }

        private MobileServiceClient Client { get; set; } = null;
        private List<Login> loginObjects = null;
        IMobileServiceTable<Login> loginTable;

        public async Task<bool> GetLogin(string a, string b)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Level 1");

                loginTable = Client.GetTable<Login>();

                System.Diagnostics.Debug.WriteLine("Level 2");

                List<Login> items = await loginTable.
                    ToListAsync();

                System.Diagnostics.Debug.WriteLine("Level 3");

                items.Add(new Login() {Password = "Empty", Username = "Empty"});

                if (items[0].Username.Equals(a) && items[0].Password.Equals(b))
                {
                    System.Diagnostics.Debug.WriteLine("User found: " + items[0].Username);
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

