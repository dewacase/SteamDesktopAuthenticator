using SteamAuth;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Steam_Desktop_Authenticator
{
    public class DWApi
    {
        static HttpClient client = new HttpClient()
        {
#if DEBUG
            BaseAddress = new Uri("http://dwcase-bot.latency2.com")
#else
            BaseAddress = new Uri("http://dwcase-bot.latency2.com")
#endif
        };

        public void updateAuth() { }

        public void updateAuthSession() { }

        public static async void updateAuthCodes(SteamGuardAccount[] allAccounts)
        {
            if (allAccounts.Length == 0) return;
            List<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < allAccounts.Length; i++)
            {
                SteamGuardAccount account = allAccounts[i];
                nameValueCollection.Add(new KeyValuePair<string, string>(account.AccountName, account.AuthCode));
            }
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(nameValueCollection);
            string status = "FAILED";
            try
            {
                HttpResponseMessage response = await client.PostAsync("/authenticator/authCodes", formContent);
                string data = await response.Content.ReadAsStringAsync();
                if (string.Equals("OK", data, StringComparison.OrdinalIgnoreCase))
                {
                    status = "OK";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("updateAuthCodes..." + status);
            }
        }
    }
}
