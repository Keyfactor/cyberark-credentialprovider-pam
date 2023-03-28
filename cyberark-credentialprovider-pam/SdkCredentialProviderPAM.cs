using Keyfactor.Platform.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    public class SdkCredentialProviderPAM : IPAMProvider
    {
        public string Name => "CyberArk-SdkCredentialProvider";

        public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
        {
            string appId = initializationInfo["AppId"];

            string safe = instanceParameters["Safe"];
            string folder = instanceParameters["Folder"];
            string obj = instanceParameters["Object"];

            string executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = Path.Combine(executingDir, "NetStandardPasswordSDK.dll");
            var sdk = Assembly.LoadFrom(dll);

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
            passwordRequest = ctor.Invoke(null);

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
            object passwordResponse = PasswordSDKType.GetMethod("GetPassword").Invoke(null, new object[] { passwordRequest });


            // Analyzing the response
            propertyInfo = PasswordResponseType.GetProperty("Content");
            var password = (char[])propertyInfo.GetValue(passwordResponse, null);

            return new string(password);
        }
    }
}
