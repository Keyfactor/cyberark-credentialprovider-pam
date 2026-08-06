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
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Keyfactor.Extensions.Pam.CyberArk.Exceptions;
using Keyfactor.Extensions.Pam.CyberArk.Models;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Uri = System.Uri;

namespace Keyfactor.Extensions.Pam.CyberArk.Clients
{
    public class CyberArkVaultHttpClient
    {
        private readonly ILogger _logger;
        private readonly HttpMessageHandler _httpMessageHandler;

        public CyberArkVaultHttpClient(ILogger logger, HttpMessageHandler httpMessageHandler = null)
        {
            _logger = logger;
            _httpMessageHandler = httpMessageHandler ?? new HttpClientHandler();
        }

        // Static factory method for client cert scenarios
        public static CyberArkVaultHttpClient CreateWithClientCertificate(
            ILogger logger, 
            string base64Pfx, 
            string pfxPassword,
            HttpMessageHandler innerHandler = null)
        {
            logger.MethodEntry();
            logger.LogTrace("Creating CyberArkVaultHttpClient with client certificate authentication.");
            
            byte[] pfxBytes = Convert.FromBase64String(base64Pfx);
            var clientCert = new X509Certificate2(
                pfxBytes, 
                pfxPassword,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet
            );
            
            logger.LogTrace("Successfully loaded client certificate with subject '{Subject}' from provided PFX data.", clientCert.Subject);
            logger.LogDebug("Client certificate subject: {Subject}, issuer: {Issuer}, serial number: {SerialNumber}, thumbprint: {Thumbprint}", clientCert.Subject, clientCert.Issuer, clientCert.SerialNumber, clientCert.Thumbprint);

            var handler = new ClientCertificateHandler(logger, clientCert, innerHandler);
            logger.MethodExit();
            return new CyberArkVaultHttpClient(logger, handler);
        }

        public async Task<string> GetPassword(string host, string site, string appId, string safe, string folder, string obj)
        {
            _logger.MethodEntry();
            
            var baseAddress = host;
            if (!host.StartsWith("http"))
            {
                _logger.LogDebug($"Host '{host}' does not include scheme. Prepending 'https://'.");
                baseAddress = $"https://{host}/";
            }

            if (baseAddress.StartsWith("http://"))
            {
                _logger.LogWarning($"Using unsecure HTTP to connect to CyberArk Central Credential Provider at '{baseAddress}'. It is recommended to use HTTPS instead.");
            }

            using (var http = new HttpClient(_httpMessageHandler, false))
            {
                _logger.LogTrace($"Base address: {baseAddress}");
                http.BaseAddress = new Uri(baseAddress);
                
                var path = $"{site}/api/Accounts?AppID={appId}&Safe={safe}&Object={obj};Folder={folder}";
                
                _logger.LogDebug($"Fetching secret from URL: {baseAddress}/{path}");
                var response = await http.GetAsync(path);
                
                _logger.LogDebug($"Request returned status code: {(int)response.StatusCode} {response.StatusCode}");
                var responseMessage = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogTrace("Successfully retrieved secret from CyberArk Central Credential Provider HTTP API.");
                    _logger.MethodExit();
                    
                    var result = JsonConvert.DeserializeObject<AccountsResponse>(responseMessage);
                    return result.Content;
                }
                
                _logger.LogCritical($"Failed to retrieve secret from CyberArk Central Credential Provider HTTP API: {responseMessage}");
                throw new HttpClientException(responseMessage, response.StatusCode);
            }
        }
    }
}

public class ClientCertificateHandler : DelegatingHandler
{
    public ClientCertificateHandler(ILogger logger, X509Certificate2 clientCertificate, HttpMessageHandler innerHandler = null)
        : base(innerHandler ?? new HttpClientHandler())
    {
        logger.MethodEntry();
        
        if (InnerHandler is HttpClientHandler httpClientHandler)
        {
            logger.LogInformation("Adding client certificate with subject '{Subject}' to HTTP client handler (thumbprint: '{Thumbprint}').", clientCertificate.Subject, clientCertificate.Thumbprint);
            httpClientHandler.ClientCertificates.Add(clientCertificate);
            
            logger.MethodExit();
            return;
        }
        
        logger.LogWarning("Inner handler is not an HttpClientHandler. Client certificate will not be added to HTTP requests.");
        logger.MethodExit();
    }
}
