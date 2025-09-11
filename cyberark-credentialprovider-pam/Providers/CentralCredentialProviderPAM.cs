// Copyright 2025 Keyfactor
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

using Keyfactor.Logging;
using Keyfactor.Platform.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    public class CentralCredentialProviderPAM : BaseCyberArkProvider, IPAMProvider
    {
        public string Name => "CyberArk-CentralCredentialProvider";

        private readonly ILogger Logger;

        public class Options
        {
            // currently no CentralCredentialProvider options
        }

        public CentralCredentialProviderPAM()
        {
            Logger = LogHandler.GetClassLogger<CentralCredentialProviderPAM>();
        }
        
        public CentralCredentialProviderPAM(IOptions<Options> options)
        {
            Logger = LogHandler.GetClassLogger<CentralCredentialProviderPAM>();
        }

        public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
        {
            var http = new HttpClient();
            var path = "";
            try
            {
                string appId = GetRequiredValue(initializationInfo, "AppId");
                string host = GetRequiredValue(initializationInfo, "Host");
                string site = GetRequiredValue(initializationInfo, "Site");
                Logger.LogDebug($"Configured with Initialization Parameters: AppId = {appId} ; Host = {host} ; Site = {site}");

                string safe = GetRequiredValue(instanceParameters, "Safe");
                string folder = GetRequiredValue(instanceParameters, "Folder");
                string obj = GetRequiredValue(instanceParameters, "Object");
                Logger.LogDebug($"Configured with Instance Parameters: Safe = {safe} ; Folder = {folder} ; Object = {obj}");

                http.BaseAddress = new Uri($"https://{host}/");
                path = $"{site}/api/Accounts?AppID={appId}&Safe={safe};Folder={folder};Object={obj}";
            }
            catch (Exception e)
            {
                Logger.LogError("Error occurred when trying to access PAM Parameters");
                Logger.LogError(e.Message);
                throw;
            }

            HttpResponseMessage response;
            try
            {
                Logger.LogTrace($"Sending GET request to {http.BaseAddress}{path}");
                response = http.GetAsync(path).Result;
            }
            catch (Exception e)
            {
                Logger.LogError("Error occurred when trying to complete GET request");
                Logger.LogError(e.Message);
                throw;
            }

            string json = ReadHttpResponse(response);
            var account = JsonConvert.DeserializeObject<AccountsResponse>(json);

            return account.Content;
        }

        private string ReadHttpResponse(HttpResponseMessage response)
        {
            Logger.LogTrace($"Reading response message of HTTP response with Status Code {response.StatusCode}");
            string responseMessage = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                Logger.LogTrace("Returning response message of successful HTTP request.");
                return responseMessage;
            }
            else
            {
                Logger.LogError("HTTP Response Status Code indicates an error occurred:");
                Logger.LogError($"Status Code: {response.StatusCode}");
                Logger.LogError($"Reason Phrase: {response.ReasonPhrase}");
                Logger.LogError($"Error Response Message: {responseMessage}");
                throw new HttpClientException(responseMessage, response.StatusCode);
            }
        }
    }
}
