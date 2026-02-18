using System;
using System.Collections.Generic;
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
            
            AppId = GetRequiredValue(initializationInfo, "AppId");
            Host = GetRequiredValue(initializationInfo, "Host");
            Site = GetRequiredValue(initializationInfo, "Site");
            string pfxBase64 = GetRequiredValue(initializationInfo, "PfxBase64");
            string pfxPassword = GetRequiredValue(initializationInfo, "PfxPassword");
            
            Logger.LogDebug("Configured with Initialization Parameters:\n" + 
                             $"App ID: {AppId}, Host: {Host}, Site: {Site}");
            Logger.LogDebug($"PFX Base64: {pfxBase64}");

            Safe = GetRequiredValue(instanceParameters, "Safe");
            Folder = GetRequiredValue(instanceParameters, "Folder");
            Object = GetRequiredValue(instanceParameters, "Object");
            
            Logger.LogDebug("Configured with Instance Parameters:\n" + 
                             $"Safe: {Safe}, Folder: {Folder}, Object: {Object}");

            HttpClient = CyberArkVaultHttpClient.CreateWithClientCertificate(Logger, pfxBase64, pfxPassword, _handler);

            return GetPasswordFromCyberArk();
        }
    }
}
