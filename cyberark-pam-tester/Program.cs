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
using System.Reflection;
using Keyfactor.Extensions.Pam.CyberArk;

namespace cyberark_pam_tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting CyberArk Central (REST) Credential Provider type...");

            var restPam = new CentralCredentialProviderPAM();

            var initParams = new Dictionary<string, string>()
            {
                { "Host", "" },
                { "Site", "" },
                { "AppId", "" }
            };

            var instanceParams = new Dictionary<string, string>()
            {
                { "Safe", "Test" },
                { "Folder", "Root" },
                { "Object", "TestObject" }
            };

            Console.WriteLine($"Targeting Central Provider at:");
            PrintDictionary(initParams);
            Console.WriteLine("Getting Password from:");
            PrintDictionary(instanceParams);

            string password;
            try
            {
                password = restPam.GetPassword(instanceParams, initParams);
                Console.WriteLine($"Got password: {password}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Password retrieval FAILED:");
                PrintException(ex);
            }

            Console.WriteLine("Central Provider test completed.");

            Console.WriteLine("Starting CyberArk SDK Credential Provider type...");

            var sdkPam = new SdkCredentialProviderPAM();

            initParams = new Dictionary<string, string>()
            {
                { "AppId", "" }
            };

            instanceParams = new Dictionary<string, string>()
            {
                { "Safe", "Test" },
                { "Folder", "Root\\Secrets" },
                { "Object", "TestObject2" }
            };

            Console.WriteLine($"Targeting SDK Provider at:");
            Console.WriteLine(Assembly.GetExecutingAssembly().Location);
            PrintDictionary(initParams);
            Console.WriteLine("Getting Password from:");
            PrintDictionary(instanceParams);

            password = sdkPam.GetPassword(instanceParams, initParams);
            Console.WriteLine($"Got password: {password}");
        }

        static void PrintDictionary(Dictionary<string, string> dict)
        {
            foreach(var item in dict)
            {
                Console.WriteLine($"\t{item.Key}: {item.Value}");
            }
        }

        static void PrintException(Exception ex)
        {
            Console.WriteLine($"Type: {ex.GetType()}");
            Console.WriteLine($"Message: {ex.Message}");

            if (ex.InnerException != null)
            {
                Console.WriteLine("\n\tInner Exception:\n");
                PrintException(ex.InnerException);
            }
        }
    }
}
