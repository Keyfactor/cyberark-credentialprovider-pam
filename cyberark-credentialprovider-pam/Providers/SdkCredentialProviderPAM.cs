// Copyright 2025 Keyfactor
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

using Keyfactor.Extensions.Pam.CyberArk.Providers;
using Keyfactor.Logging;
using Keyfactor.Platform.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    public class SdkCredentialProviderPAM : BaseCyberArkProvider, IPAMProvider
    {
        public string Name => "CyberArk-SdkCredentialProvider";

        private readonly ILogger Logger;
        private readonly Constants.SDK SDKConstants;
        private readonly string SdkPath;
        private readonly bool UsingFrameworkSdk;

        public SdkCredentialProviderPAM()
        {
            Logger = LogHandler.GetClassLogger<SdkCredentialProviderPAM>();
            Logger.LogTrace($"Starting up {Name} with no Options provided.");
            SDKConstants = new Constants.NetStandard();
            Logger.LogTrace($"SDK to be targeted will be {SDKConstants.DLL}");

            // when lookup path is not provided, use executing assembly location (running on UO)
            SdkPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Logger.LogInformation($"{Name} determined it will use the following directory to load the SDK: {SdkPath}");
        }

        public SdkCredentialProviderPAM(IOptions<SdkOptions> options)
        {
            Logger = LogHandler.GetClassLogger<SdkCredentialProviderPAM>();
            Logger.LogTrace($"Starting up {Name} with Options provided.");
            Logger.LogTrace($"Reading option UseFrameworkLibrary to determine which SDK constants to load.");
            UsingFrameworkSdk = options.Value.UsingFrameworkSdk;
            if (UsingFrameworkSdk)
            {
                SDKConstants = new Constants.NetFramework();
            }
            else
            {
                SDKConstants = new Constants.NetStandard();
            }
            Logger.LogDebug($"SDK to be targeted will be {SDKConstants.DLL}");

            // Extension Path (for looking up DLL) can be passed in as an Option (loaded from manifest.json)
            if (string.IsNullOrEmpty(options.Value.SdkPath))
            {
                Logger.LogDebug("No SdkPath was provided in Options. Using executing assembly location.");
                SdkPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                Logger.LogDebug("Using SdkPath provided in Options");
                SdkPath = options.Value.SdkPath;
            }
            Logger.LogInformation($"{Name} determined it will use the following directory to load the SDK: {SdkPath}");
        }

        public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
        {
            string appId = GetRequiredValue(initializationInfo, "AppId");
            Logger.LogInformation($"Configured with Initialization Parameters: AppId = {appId}");

            string safe = GetRequiredValue(instanceParameters, "Safe");
            string folder = GetRequiredValue(instanceParameters, "Folder");
            string obj = GetRequiredValue(instanceParameters, "Object");
            Logger.LogInformation($"Configured with Instance Parameters: Safe = {safe} ; Folder = {folder} ; Object = {obj}");

            string dll = Path.Combine(SdkPath, SDKConstants.DLL);
            Logger.LogDebug($"Loading DLL: {dll}");
            var sdk = Assembly.LoadFrom(dll);
            Logger.LogTrace("Loaded SDK DLL.");

            // get the types from the dll
            Type PasswordSDKType = sdk.GetType(SDKConstants.PasswordSDKType);
            Type PasswordRequestType = sdk.GetType(SDKConstants.PasswordRequestType);
            Type PasswordSDKExceptionType = sdk.GetType(SDKConstants.PasswordSDKExceptionType);
            Type PasswordResponseType = sdk.GetType(SDKConstants.PasswordResponseType);

            // Create Password Request
            ConstructorInfo ctor = PasswordRequestType.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                throw new Exception("Could not create Password Request");
            }

            object passwordRequest;
            Logger.LogTrace("Attempting to invoke constructor of PSDKPasswordRequest type.");
            passwordRequest = ctor.Invoke(null);
            Logger.LogTrace($"Constructed PSDKPasswordRequest. {passwordRequest}");

            PropertyInfo propertyInfo = PasswordRequestType.GetProperty("ConnectionTimeout");
            propertyInfo.SetValue(passwordRequest, 30);

            // Query propreties
            propertyInfo = PasswordRequestType.GetProperty("AppID");
            propertyInfo.SetValue(passwordRequest, appId);

            propertyInfo = PasswordRequestType.GetProperty("Safe");
            propertyInfo.SetValue(passwordRequest, safe);

            propertyInfo = PasswordRequestType.GetProperty("Folder");
            propertyInfo.SetValue(passwordRequest, folder);

            propertyInfo = PasswordRequestType.GetProperty("Object");
            propertyInfo.SetValue(passwordRequest, obj);

            propertyInfo = PasswordRequestType.GetProperty("Reason");
            propertyInfo.SetValue(passwordRequest, "Automated request from Keyfactor PAM Provider.");


            // Sending the request to get the password
            Logger.LogTrace("Attempting to invoke GetPassword method of PasswordSDK type.");
            object passwordResponse;
            try
            {
                passwordResponse = PasswordSDKType.GetMethod("GetPassword").Invoke(null, new object[] { passwordRequest });
            }
            catch (TargetInvocationException ex)
            {
                Logger.LogError(ex.InnerException, "Error occurred when invoking GetPassword method.");
                Logger.LogError(ex.ToString());
                Logger.LogError(ex.InnerException.ToString());

                throw ex.InnerException;
            }
            Logger.LogTrace("Invoked GetPassword method.");


            // Analyzing the response
            propertyInfo = PasswordResponseType.GetProperty("Content");
            if (UsingFrameworkSdk)
            {
                // for netframework
                var password = (string)propertyInfo.GetValue(passwordResponse, null);
                return password;
            }
            else
            {
                //for netstandard
                var password = (char[])propertyInfo.GetValue(passwordResponse, null);
                return new string(password);
            }
        }
    }
}
