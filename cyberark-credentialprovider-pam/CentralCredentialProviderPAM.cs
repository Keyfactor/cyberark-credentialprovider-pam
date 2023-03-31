// Copyright 2023 Keyfactor
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
