using Keyfactor.Platform.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    public class CentralCredentialProviderPAM : IPAMProvider
    {
        public string Name => "CyberArk-CentralCredentialProvider";

        public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
        {
            string appId = initializationInfo["AppId"];
            string host = initializationInfo["Host"];
            string site = initializationInfo["Site"];

            string safe = instanceParameters["Safe"];
            string folder = instanceParameters["Folder"];
            string obj = instanceParameters["Object"];

            var http = new HttpClient();
            http.BaseAddress = new Uri($"https://{host}/");

            var path = $"{site}/api/Accounts?AppID={appId}&Safe={safe};Folder={folder};Object={obj}";
            var response = http.GetAsync(path).Result;
            string json = ReadHttpResponse(response);
            var account = JsonConvert.DeserializeObject<AccountsResponse>(json);

            return account.Content;
        }

        private string ReadHttpResponse(HttpResponseMessage response)
        {
            string responseMessage = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                return responseMessage;
            }
            else
            {
                throw new HttpClientException(responseMessage, response.StatusCode);
            }
        }
    }
}
