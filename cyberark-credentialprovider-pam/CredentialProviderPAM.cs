using Keyfactor.Platform.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    public class CredentialProviderPAM : IPAMProvider
    {
        public string Name => "CyberArk-CredentialProvider";

        public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
        {
            string appId = initializationInfo["AppId"];
            bool useLocalSdk = Convert.ToBoolean(initializationInfo["UseSDK"]);
            string host = initializationInfo["Host"];

            string safe = instanceParameters["Safe"];
            string folder = instanceParameters["Folder"];
            string obj = instanceParameters["Object"];

            if (useLocalSdk)
            {
                throw new NotImplementedException("Dynamic Local SDK usage is not yet implemented.");
            }

            var http = new HttpClient();
            http.BaseAddress = new Uri($"https://{host}/");

            var path = $"AIMWebService/api/Accounts?AppID={appId}&Safe={safe};Folder={folder};Object={obj}";
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
