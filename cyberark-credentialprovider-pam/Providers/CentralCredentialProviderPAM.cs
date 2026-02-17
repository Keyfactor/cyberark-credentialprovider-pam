// Copyright 2026 Keyfactor
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

using System;
using Keyfactor.Platform.Extensions;
using Newtonsoft.Json;
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
        private readonly CyberArkVaultHttpClient _httpClient;

        // Default constructor used by agent services
        public CentralCredentialProviderPAM()
        {
            _logger = LogHandler.GetClassLogger<CentralCredentialProviderPAM>();
            _httpClient = new CyberArkVaultHttpClient(_logger);
        }

        // Constructor used by unit tests
        public CentralCredentialProviderPAM(ILogger logger, CyberArkVaultHttpClient httpClient)
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
            
            _logger.LogDebug("Configured with Initialization Parameters:\n" + 
                                   $"App ID: {appId}, Host: {host}, Site: {site}");

            string safe = GetRequiredValue(instanceParameters, "Safe");
            string folder = GetRequiredValue(instanceParameters, "Folder");
            string obj = GetRequiredValue(instanceParameters, "Object");
            
            _logger.LogDebug("Configured with Instance Parameters:\n" + 
                             $"Safe: {safe}, Folder: {folder}, Object: {obj}");

            try
            {
                string password = _httpClient.GetPassword(host, site, appId, safe, folder, obj)
                    .GetAwaiter()
                    .GetResult();

                _logger.LogInformation(
                    $"Successfully retrieved secret for object '{obj}' from safe '{safe}' (AppID: {appId}).");
                _logger.MethodExit();

                return password;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving secret for object '{obj}' from safe '{safe}' (AppID: {appId}): {ex.Message}");

                throw;
            }
            
        }
    }
}
