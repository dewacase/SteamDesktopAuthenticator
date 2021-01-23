using Newtonsoft.Json;
using SteamAuth;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steam_Desktop_Authenticator
{
    public class DWApi
    {
        static HttpClient client = new HttpClient()
        {
#if DEBUG
            BaseAddress = new Uri("http://192.168.41.25:5002")
#else
            BaseAddress = new Uri("http://dwcase-bot.latency2.com")
#endif
        };

        public void updateAuth() { }

        public void updateAuthSession() { }

        public static async void updateManifestFiles(List<string> jsonAccounts)
        {
            string jsons = "[" + string.Join(",", jsonAccounts) + "]";
            Console.WriteLine("updateManifestFiles: send update manifest to server\n" + jsons);
            List<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>();

            nameValueCollection.Add(new KeyValuePair<string, string>("size", jsonAccounts.Count + ""));
            nameValueCollection.Add(new KeyValuePair<string, string>("accounts", jsons));

            FormUrlEncodedContent formContent = new FormUrlEncodedContent(nameValueCollection);
            string status = "FAILED";
            try
            {
                HttpResponseMessage response = await client.PostAsync("/authenticator/auth", formContent);
                string data = await response.Content.ReadAsStringAsync();
                if (string.Equals("OK", data, StringComparison.OrdinalIgnoreCase))
                {
                    status = "OK";
                }
                else
                {
                    status = data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("updateManifestFiles..." + status);
                if (status != "OK")
                {
                    MessageBox.Show("Error: Update manifest failed. \"" + status + "\"", "Api Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static async void updateManifestFile(string jsonAccount)
        {
            List<string> accounts = new List<string>();
            accounts.Add(jsonAccount);
            updateManifestFiles(accounts);
        }

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
                else
                {
                    status = data;
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

        public static async void reconnect(string username)
        {
            List<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>();
            nameValueCollection.Add(new KeyValuePair<string, string>("username", username));
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(nameValueCollection);
            string status = "FAILED";
            try
            {
                HttpResponseMessage response = await client.PostAsync("/authenticator/refresh", formContent);
                string data = await response.Content.ReadAsStringAsync();
                if (string.Equals("OK", data, StringComparison.OrdinalIgnoreCase))
                {
                    status = "OK";
                }
                else
                {
                    status = data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("reconnect..." + status);
            }
        }
    }
}
