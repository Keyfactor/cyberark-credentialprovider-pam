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

namespace cyberark_credentialprovider_pam_tests.Providers;

public abstract class BaseCredentialProviderPAMTest
{
    // Test data constants
    protected const string ExpectedSecret = "foobar";
    protected const string TestAppId = "TestAppId";
    protected const string TestHost = "TestHost";
    protected const string TestSite = "TestSite";
    protected const string TestSafe = "TestSafe";
    protected const string TestFolder = "TestFolder";
    protected const string TestObject = "TestObject";
    
    protected abstract Dictionary<string, string> CreateInitializationInfo();
    protected abstract Dictionary<string, string> CreateInstanceParams();
}
