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
