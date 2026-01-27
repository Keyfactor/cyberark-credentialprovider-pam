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
using Keyfactor.Extensions.Pam.CyberArk.Clients;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    public class CentralCredentialProviderPAM : CyberArkProvider, IPAMProvider
    {
        private readonly ILogger _logger;
        private readonly IConjurHttpClient _httpClient;

        // Default constructor used by agent services
        public CentralCredentialProviderPAM()
        {
            _logger = LogHandler.GetClassLogger<CentralCredentialProviderPAM>();
            _httpClient = new ConjurHttpClient(_logger);
        }

        // Constructor used by unit tests
        public CentralCredentialProviderPAM(ILogger logger, IConjurHttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }
        
        public string Name => "CyberArk-CentralCredentialProvider";

        public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
        {
            _logger.MethodEntry();
            
            _logger.LogTrace("InitializationInfo: {}", JsonConvert.SerializeObject(initializationInfo));
            _logger.LogTrace("InstanceParameters: {}", JsonConvert.SerializeObject(instanceParameters));
            
            string appId = GetRequiredValue(initializationInfo, "AppId");
            string host = GetRequiredValue(initializationInfo, "Host");
            string site = GetRequiredValue(initializationInfo, "Site");
            
            _logger.LogTrace("Retrieved required initialization parameters:");
            _logger.LogTrace($"App ID: {appId}, Host: {host}, Site: {site}");

            string safe = GetRequiredValue(instanceParameters, "Safe");
            string folder = GetRequiredValue(instanceParameters, "Folder");
            string obj = GetRequiredValue(instanceParameters, "Object");
            
            _logger.LogDebug("Retrieved required instance parameters:");
            _logger.LogDebug($"Safe: {safe}, Folder: {folder}, Object: {obj}");

            var baseAddress = host;
            if (!host.StartsWith("http"))
            {
                _logger.LogTrace($"Host '{host}' does not include scheme. Prepending 'https://'.");
                baseAddress = $"https://{host}";
            }
            
            var response = _httpClient.GetPassword(baseAddress, site, appId, safe, folder, obj);
            
            string json = ReadHttpResponse(response);
            var account = JsonConvert.DeserializeObject<AccountsResponse>(json);
            
            _logger.LogInformation($"Successfully retrieved secret for object '{obj}' from safe '{safe}'.");
            
            _logger.MethodExit();

            return account.Content;
        }

        private string ReadHttpResponse(HttpResponseMessage response)
        {
            _logger.MethodEntry();
            
            _logger.LogDebug("Reading HTTP response from CyberArk Central Credential Provider...");

            string responseMessage = response.Content.ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();
            
            _logger.LogDebug($"Request returned status code: {(int)response.StatusCode} {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("Successfully retrieved secret from CyberArk Central Credential Provider.");
                
                _logger.MethodExit();
                return responseMessage;
            }
            
            throw new HttpClientException(responseMessage, response.StatusCode);
        }
    }
}
