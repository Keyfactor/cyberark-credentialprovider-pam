using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            
            Logger.LogTrace("InstanceParameters: {}", JsonConvert.SerializeObject(instanceParameters));
            
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

            if (IsValueProvided(pfxBase64))
            {
                Debug.Assert(pfxBase64 != null, nameof(pfxBase64) + " != null");
                
                Logger.LogDebug("Using PFX provided as Base64 string in initialization parameters.");
                int displayLength = Math.Min(30, pfxBase64.Length);
                if (pfxBase64.Length > displayLength)
                {
                    Logger.LogDebug(
                        $"PFX Base64 Value (truncated): {pfxBase64.Substring(0, displayLength)}... (length: {pfxBase64.Length} characters)");
                }
                else
                {
                    Logger.LogDebug($"PFX Base64 Value (full): {pfxBase64} (length: {pfxBase64.Length} characters)");
                }

                Logger.MethodExit();
                
                return pfxBase64;
            }
            
            var pfxFilePath = initializationInfo.TryGetValue("PfxFilePath", out string filePathParam) ? filePathParam : null;
            
            if (IsValueProvided(pfxFilePath))
            {
                Logger.LogDebug("Using PFX provided as file path in initialization parameters.");
                Logger.LogDebug($"PFX File Path: {pfxFilePath}");
                
                try
                {
                    Debug.Assert(pfxFilePath != null, nameof(pfxFilePath) + " != null");
                    
                    var pfxBytes = System.IO.File.ReadAllBytes(pfxFilePath);
                    var pfxBase64FromFile = Convert.ToBase64String(pfxBytes);
                    Logger.LogDebug($"Successfully read PFX file and converted to Base64 string. (Original file size: {pfxBytes.Length} bytes; Base64 string length: {pfxBase64FromFile.Length} characters)");
                    
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

        private bool IsValueProvided(string value)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   !value.Equals("none", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
