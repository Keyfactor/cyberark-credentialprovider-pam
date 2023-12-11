using System;
using System.Collections.Generic;
using System.Text;

namespace Keyfactor.Extensions.Pam.CyberArk
{
    public static class Constants
    {
        public interface SDK
        {
            string DLL { get; }
            string PasswordSDKType { get; }
            string PasswordRequestType { get; }
            string PasswordSDKExceptionType { get; }
            string PasswordResponseType { get; }
        }

        public class NetStandard : SDK
        {
            public string DLL => "NetStandardPasswordSDK.dll";
            public string PasswordSDKType => "CyberArk.AAM.NetStandardPasswordSDK.PasswordSDK";
            public string PasswordRequestType => "CyberArk.AAM.NetStandardPasswordSDK.PSDKPasswordRequest";
            public string PasswordSDKExceptionType => "CyberArk.AAM.NetStandardPasswordSDK.Exceptions.PSDKException";
            public string PasswordResponseType => "CyberArk.AAM.NetStandardPasswordSDK.PSDKPassword";
        }

        public class NetFramework : SDK
        {
            public string DLL => "NetPasswordSDK.dll";
            public string PasswordSDKType => "CyberArk.AIM.NetPasswordSDK.PasswordSDK";
            public string PasswordRequestType => "CyberArk.AIM.NetPasswordSDK.PSDKPasswordRequest";
            public string PasswordSDKExceptionType => "CyberArk.AIM.NetPasswordSDK.Exceptions.PSDKException";
            public string PasswordResponseType => "CyberArk.AIM.NetPasswordSDK.PSDKPassword";
        }
    }
}
