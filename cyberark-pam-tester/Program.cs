using System;
using System.Collections.Generic;
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
                { "Safe", "" },
                { "Folder", "" },
                { "Object", "" }
            };

            Console.WriteLine($"Targeting Central Provider at:");
            PrintDictionary(initParams);
            Console.WriteLine("Getting Password from:");
            PrintDictionary(instanceParams);

            try
            {
                var password = restPam.GetPassword(instanceParams, initParams);
                Console.WriteLine($"Got password: {password}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Password retrieval FAILED:");
                PrintException(ex);
            }

            Console.WriteLine("Central Provider test completed.");
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
