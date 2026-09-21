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
    public class CentralCredentialProviderPAM : BaseCentralCredentialProviderPAM, IPAMProvider
    {

        // Default constructor used by agent services
        public CentralCredentialProviderPAM()
        {
            Logger = LogHandler.GetClassLogger<CentralCredentialProviderPAM>();
            HttpClient = new CyberArkVaultHttpClient(Logger);
        }

        // Constructor used by unit tests
        public CentralCredentialProviderPAM(ILogger logger, CyberArkVaultHttpClient httpClient)
        {
            Logger = logger;
            HttpClient = httpClient;
        }
        
        public string Name => "CyberArk-CentralCredentialProvider";

        public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
        {
            Logger.MethodEntry();
            
            Logger.LogTrace("InitializationInfo: {}", JsonConvert.SerializeObject(initializationInfo));
            Logger.LogTrace("InstanceParameters: {}", JsonConvert.SerializeObject(instanceParameters));
            
            AppId = GetRequiredValue(initializationInfo, "AppId", InitializationInfoDictionaryName);
            Host = GetRequiredValue(initializationInfo, "Host", InitializationInfoDictionaryName);
            Site = GetRequiredValue(initializationInfo, "Site", InitializationInfoDictionaryName);
            
            Logger.LogDebug("Configured with Initialization Parameters:\n" + 
                                   $"App ID: {AppId}, Host: {Host}, Site: {Site}");

            Safe = GetRequiredValue(instanceParameters, "Safe", InstanceParametersDictionaryName);
            Folder = GetRequiredValue(instanceParameters, "Folder", InstanceParametersDictionaryName);
            Object = GetRequiredValue(instanceParameters, "Object", InstanceParametersDictionaryName);
            
            Logger.LogDebug("Configured with Instance Parameters:\n" + 
                             $"Safe: {Safe}, Folder: {Folder}, Object: {Object}");

            return GetPasswordFromCyberArk();
        }
    }
}
