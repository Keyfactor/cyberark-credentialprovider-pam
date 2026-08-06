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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Keyfactor.Extensions.Pam.CyberArk.Clients;
using Keyfactor.Logging;
using Keyfactor.Platform.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    public class CentralCredentialProviderClientCertPAM : BaseCentralCredentialProviderPAM, IPAMProvider
    {
        // private readonly ILogger _logger;
        private readonly HttpMessageHandler _handler;
        
        // Default constructor used by agent services
        public CentralCredentialProviderClientCertPAM()
        {
            Logger = LogHandler.GetClassLogger<CentralCredentialProviderClientCertPAM>();
            _handler = null;
        }
        
        // Constructor used by unit tests
        public CentralCredentialProviderClientCertPAM(ILogger logger, HttpMessageHandler handler = null)
        {
            Logger = logger;
            _handler = handler;
        }
        
        public string Name => "CyberArk-CentralCredentialProvider-ClientCert";
        
        public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
        {
            Logger.MethodEntry();
            
            Logger.LogTrace($"InstanceParameters: {JsonConvert.SerializeObject(instanceParameters)}");
            
            AppId = GetRequiredValue(initializationInfo, "AppId", InitializationInfoDictionaryName);
            Host = GetRequiredValue(initializationInfo, "Host", InitializationInfoDictionaryName);
            Site = GetRequiredValue(initializationInfo, "Site", InitializationInfoDictionaryName);
            string pfxPassword = GetRequiredValue(initializationInfo, "PfxPassword", InitializationInfoDictionaryName);
            
            // TODO: Fetch either the pfxBase64 or pfxFilePath
            string pfxBase64 = GetPfxValue(initializationInfo);
            
            Logger.LogDebug("Configured with Initialization Parameters:\n" + 
                             $"App ID: {AppId}, Host: {Host}, Site: {Site}");

            Safe = GetRequiredValue(instanceParameters, "Safe", InstanceParametersDictionaryName);
            Folder = GetRequiredValue(instanceParameters, "Folder", InstanceParametersDictionaryName);
            Object = GetRequiredValue(instanceParameters, "Object", InstanceParametersDictionaryName);
            
            Logger.LogDebug("Configured with Instance Parameters:\n" + 
                             $"Safe: {Safe}, Folder: {Folder}, Object: {Object}");

            HttpClient = CyberArkVaultHttpClient.CreateWithClientCertificate(Logger, pfxBase64, pfxPassword, _handler);

            return GetPasswordFromCyberArk();
        }

        private string GetPfxValue(Dictionary<string, string> initializationInfo)
        {
            Logger.MethodEntry();
            Logger.LogTrace("Fetching PfxBase64 from initialization info parameters...");
            var pfxBase64 = initializationInfo.TryGetValue("PfxBase64", out string base64Param) ? base64Param : null;

            if (IsValueProvided(pfxBase64, "pfxBase64"))
            {
                // Inform Intellisense that pfxBase64 is not null for the code below
                Debug.Assert(pfxBase64 != null, nameof(pfxBase64) + " != null");
                
                Logger.LogTrace("Using PFX provided as Base64 string in initialization parameters.");
                int displayLength = Math.Min(50, pfxBase64.Length);
                if (pfxBase64.Length > displayLength)
                {
                    Logger.LogTrace(
                        $"PFX Base64 Value (truncated): {pfxBase64.Substring(0, displayLength)}... (length: {pfxBase64.Length} characters)");
                }
                else
                {
                    Logger.LogTrace($"PFX Base64 Value (full): {pfxBase64} (length: {pfxBase64.Length} characters)");
                }

                Logger.MethodExit();
                
                return pfxBase64;
            }
            
            var pfxFilePath = initializationInfo.TryGetValue("PfxFilePath", out string filePathParam) ? filePathParam : null;
            
            if (IsValueProvided(pfxFilePath, "pfxFilePath"))
            {
                Logger.LogTrace("Using PFX provided as file path in initialization parameters.");
                Logger.LogDebug($"PFX File Path: {pfxFilePath}, current working directory of service: {Directory.GetCurrentDirectory()}");
                
                try
                {
                    // Inform Intellisense that pfxFilePath is not null for the code below
                    Debug.Assert(pfxFilePath != null, nameof(pfxFilePath) + " != null");
                    
                    var pfxBytes = System.IO.File.ReadAllBytes(pfxFilePath);
                    var pfxBase64FromFile = Convert.ToBase64String(pfxBytes);
                    Logger.LogDebug("Successfully read PFX file and converted to Base64 string.");
                    
                    Logger.MethodExit();
                    
                    return pfxBase64FromFile;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error reading PFX file from path '{pfxFilePath}': {ex.Message}");
                    throw new ArgumentException($"Error reading PFX file from path '{pfxFilePath}': {ex.Message}", ex);
                }
            }

            throw new ArgumentException("Either PfxBase64 or PfxFilePath must be provided in initialization info");
        }

        private bool IsValueProvided(string value, string descriptor)
        {
            var isValueProvided = !string.IsNullOrWhiteSpace(value) &&
                   !value.Equals("none", StringComparison.InvariantCultureIgnoreCase);
            Logger.LogTrace($"Is {descriptor} value provided: {isValueProvided}");
            return isValueProvided;
        }
    }
}
