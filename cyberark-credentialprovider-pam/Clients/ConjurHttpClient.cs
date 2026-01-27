using System.Net.Http;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;
using Uri = System.Uri;

namespace Keyfactor.Extensions.Pam.CyberArk.Clients
{
    public interface IConjurHttpClient
    {
        /// <summary>
        /// Retrieves a secret (password) from CyberArk Conjur using the REST API.
        /// </summary>
        /// <param name="baseAddress">
        /// The base URL of the CyberArk Vault or Conjur service, including scheme and port
        /// (e.g. <c>https://cyberark.vault.example.com:12345</c>).
        /// </param>
        /// <param name="site">
        /// The CyberArk site or cluster name hosting the target safe.
        /// </param>
        /// <param name="appId">
        /// The application identity (AppID) used to authenticate and authorize access
        /// to the secret.
        /// </param>
        /// <param name="safe">
        /// The name of the CyberArk Safe containing the secret.
        /// </param>
        /// <param name="folder">
        /// The folder path within the Safe where the secret object is stored.
        /// Use <c>Root</c> if the object is located at the Safe root.
        /// </param>
        /// <param name="obj">
        /// The object name of the secret (password) to retrieve.
        /// </param>
        /// <returns>
        /// An <see cref="HttpResponseMessage"/> containing the secret value if the request
        /// succeeds, or an error response if access is denied or the object is not found.
        /// </returns>
        /// <remarks>
        /// This method performs a synchronous HTTP request to the CyberArk REST endpoint.
        /// Callers are responsible for validating the response status code and securely
        /// handling the returned secret.
        /// </remarks>
        HttpResponseMessage GetPassword(string baseAddress, string site, string appId, string safe, string folder, string obj);
    }

    public class ConjurHttpClient : IConjurHttpClient
    {
        private readonly ILogger _logger;
        
        public ConjurHttpClient(ILogger logger)
        {
            _logger = logger;
        }
        
        public HttpResponseMessage GetPassword(string baseAddress, string site, string appId, string safe, string folder, string obj)
        {
            _logger.MethodEntry();
            
            using (HttpClient http = new HttpClient())
            {
                _logger.LogTrace($"Base address: {baseAddress}");
                
                http.BaseAddress = new Uri(baseAddress);

                var path = $"{site}/api/Accounts?AppID={appId}&Safe={safe};Folder={folder};Object={obj}";
            
                _logger.LogDebug($"Fetching secret from path: {path}");
                
                var response = http.GetAsync(path).GetAwaiter().GetResult();

                _logger.MethodExit();
                
                return response;
            };
        }
    }
}
