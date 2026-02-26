using System;
using System.Collections.Generic;
using Keyfactor.Extensions.Pam.CyberArk.Clients;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    public abstract class BaseCentralCredentialProviderPAM : CyberArkProvider
    {
        protected ILogger Logger;
        protected CyberArkVaultHttpClient HttpClient;
        
        protected string Host { get; set; }
        protected string Site { get; set; }
        protected string AppId { get; set; }
        protected string Safe { get; set; }
        protected string Folder { get; set; }
        protected string Object { get; set; }
        
        public string GetPasswordFromCyberArk()
        {
            try
            {
                string password = HttpClient.GetPassword(Host, Site, AppId, Safe, Folder, Object)
                    .GetAwaiter()
                    .GetResult();

                Logger.LogInformation(
                    $"Successfully retrieved secret for object '{Object}' from safe '{Safe}' (AppID: {AppId}).");
                Logger.MethodExit();

                return password;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving secret for object '{Object}' from safe '{Safe}' (AppID: {AppId}): {ex.Message}");

                throw;
            }
        }
    }
}
