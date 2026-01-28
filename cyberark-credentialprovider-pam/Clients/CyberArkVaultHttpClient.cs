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

using System.Net.Http;
using System.Threading.Tasks;
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

        public async Task<string> GetPassword(string host, string site, string appId, string safe, string folder, string obj)
        {
            _logger.MethodEntry();
            
            var baseAddress = host;
            if (!host.StartsWith("http"))
            {
                _logger.LogTrace($"Host '{host}' does not include scheme. Prepending 'https://'.");
                baseAddress = $"https://{host}/";
            }

            using (var http = new HttpClient(_httpMessageHandler, false))
            {
                _logger.LogTrace($"Base address: {baseAddress}");
                http.BaseAddress = new Uri(baseAddress);
                
                var path = $"{site}/api/Accounts?AppID={appId}&Safe={safe};Folder={folder};Object={obj}";
                
                _logger.LogDebug($"Fetching secret from path: {path}");
                var response = await http.GetAsync(path);
                
                _logger.LogDebug($"Request returned status code: {(int)response.StatusCode} {response.StatusCode}");
                var responseMessage = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogDebug("Successfully retrieved secret from CyberArk Central Credential Provider.");
                    _logger.MethodExit();
                    
                    var result = JsonConvert.DeserializeObject<AccountsResponse>(responseMessage);
                    return result.Content;
                }
                
                _logger.LogCritical($"Failed to retrieve secret from CyberArk Central Credential Provider: {responseMessage}");
                throw new HttpClientException(responseMessage, response.StatusCode);
            }
        }
    }
}
