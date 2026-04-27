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
using Keyfactor.Extensions.Pam.CyberArk.Clients;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    public abstract class BaseCentralCredentialProviderPAM : CyberArkProvider
    {
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
                Logger.MethodEntry();
                
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
