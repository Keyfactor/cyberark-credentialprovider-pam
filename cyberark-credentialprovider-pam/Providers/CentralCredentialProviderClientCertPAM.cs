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
    public class CentralCredentialProviderClientCertPAM : CyberArkProvider, IPAMProvider
    {
        private readonly ILogger _logger;
        private readonly HttpMessageHandler _handler;
        
        // Default constructor used by agent services
        public CentralCredentialProviderClientCertPAM()
        {
            _logger = LogHandler.GetClassLogger<CentralCredentialProviderClientCertPAM>();
            _handler = null;
        }
        
        // Constructor used by unit tests
        public CentralCredentialProviderClientCertPAM(ILogger logger, HttpMessageHandler handler = null)
        {
            _logger = logger;
            _handler = handler;
        }
        
        public string Name => "CyberArk-CentralCredentialProvider-ClientCert";
        
        public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
        {
            _logger.MethodEntry();
            
            _logger.LogTrace("InstanceParameters: {}", JsonConvert.SerializeObject(instanceParameters));
            
            string appId = GetRequiredValue(initializationInfo, "AppId");
            string host = GetRequiredValue(initializationInfo, "Host");
            string site = GetRequiredValue(initializationInfo, "Site");
            string pfxBase64 = GetRequiredValue(initializationInfo, "PfxBase64");
            string pfxPassword = GetRequiredValue(initializationInfo, "PfxPassword");
            
            _logger.LogDebug("Configured with Initialization Parameters:\n" + 
                             $"App ID: {appId}, Host: {host}, Site: {site}");
            _logger.LogDebug($"PFX Base64: {pfxBase64}");

            string safe = GetRequiredValue(instanceParameters, "Safe");
            string folder = GetRequiredValue(instanceParameters, "Folder");
            string obj = GetRequiredValue(instanceParameters, "Object");
            
            _logger.LogDebug("Configured with Instance Parameters:\n" + 
                             $"Safe: {safe}, Folder: {folder}, Object: {obj}");

            var httpClient = CyberArkVaultHttpClient.CreateWithClientCertificate(_logger, pfxBase64, pfxPassword, _handler);
            
            try
            {
                string password = httpClient.GetPassword(host, site, appId, safe, folder, obj)
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
