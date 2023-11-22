// Copyright 2023 Keyfactor
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

using Keyfactor.Logging;
using Keyfactor.Platform.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    public class SdkCredentialProviderPAM : CyberArkProvider, IPAMProvider
    {
        public string Name => "CyberArk-SdkCredentialProvider";

        private readonly ILogger Logger;
        private readonly string ExtensionPath;

        public SdkCredentialProviderPAM()
        {
            Logger = LogHandler.GetClassLogger<SdkCredentialProviderPAM>();
            Logger.LogTrace($"Starting up {Name} with no constructor parameters.");

            // when lookup path is not provided, use executing assembly location (running on UO)
            ExtensionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Logger.LogInformation($"{Name} determined it will use the following directory to load the SDK: {ExtensionPath}");
        }

        public SdkCredentialProviderPAM(string extensionPath)
        {
            Logger = LogHandler.GetClassLogger<SdkCredentialProviderPAM>();
            Logger.LogTrace($"Starting up {Name} with constructor parameters provided.");

            // Extension Path (for looking up DLL) can be passed in as a Unity Constructor parameter (running on Command)
            ExtensionPath = extensionPath;
            Logger.LogInformation($"{Name} determined it will use the following directory to load the SDK: {ExtensionPath}");
        }

        public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
        {
            string appId = GetRequiredValue(initializationInfo, "AppId");
            Logger.LogInformation($"Configured with Initialization Parameters: AppId = {appId}");

            string safe = GetRequiredValue(instanceParameters, "Safe");
            string folder = GetRequiredValue(instanceParameters, "Folder");
            string obj = GetRequiredValue(instanceParameters, "Object");
            Logger.LogInformation($"Configured with Instance Parameters: Safe = {safe} ; Folder = {folder} ; Object = {obj}");

            string dll = Path.Combine(ExtensionPath, "NetStandardPasswordSDK.dll");
            Logger.LogDebug($"Loading DLL: {dll}");
            var sdk = Assembly.LoadFrom(dll);
            Logger.LogTrace("Loaded SDK DLL.");

            // get the types from the dll
            Type PasswordSDKType = sdk.GetType("CyberArk.AAM.NetStandardPasswordSDK.PasswordSDK");
            Type PasswordRequestType = sdk.GetType("CyberArk.AAM.NetStandardPasswordSDK.PSDKPasswordRequest");
            Type PasswordSDKExceptionType = sdk.GetType("CyberArk.AAM.NetStandardPasswordSDK.Exceptions.PSDKException");
            Type PasswordResponseType = sdk.GetType("CyberArk.AAM.NetStandardPasswordSDK.PSDKPassword");

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
            var password = (char[])propertyInfo.GetValue(passwordResponse, null);

            return new string(password);
        }
    }
}
