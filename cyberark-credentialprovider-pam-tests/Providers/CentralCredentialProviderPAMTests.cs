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

using Keyfactor.Extensions.Pam.CyberArk;

namespace cyberark_credentialprovider_pam_tests.Providers;

public class CentralCredentialProviderPAMTests
{
    private readonly CentralCredentialProviderPAM _sut;
    
    public CentralCredentialProviderPAMTests()
    {
        _sut = new CentralCredentialProviderPAM();
    }
    
    [Theory]
    [InlineData("AppId")]
    [InlineData("Host")]
    [InlineData("Site")]
    public void GetPassword_MissingRequiredInitializationParameter_ThrowsException(string keyToRemove)
    {
        // Arrange
        var initializationInfo = new Dictionary<string, string>()
        {
            { "AppId", "TestAppId" },
            { "Host", "TestHost" },
            { "Site", "TestSite" }
        };

        var instanceParams = new Dictionary<string, string>()
        {
            {"Safe", "TestSafe"},
            {"Folder", "TestFolder"},
            {"Object", "TestObject"},
        };
        
        // Scenario 1: Key is missing from dictionary

        initializationInfo.Remove(keyToRemove);
        
        // Act
        var exception1 = Assert.Throws<ArgumentException>(() => _sut.GetPassword(instanceParams, initializationInfo));
        
        // Assert
        Assert.Equal($"Required field {keyToRemove} was missing a value or was not defined as expected in dictionary.", exception1.Message);
        
        
        // Scenario 2: Key is present but value is null or whitespace
        initializationInfo[keyToRemove] = "";
        
        // Act
        var exception2 = Assert.Throws<ArgumentException>(() => _sut.GetPassword(instanceParams, initializationInfo));
        
        // Assert
        Assert.Equal($"Required field {keyToRemove} was missing a value or was not defined as expected in dictionary.", exception2.Message);
    }
    
    [Theory]
    [InlineData("Safe")]
    [InlineData("Folder")]
    [InlineData("Object")]
    public void GetPassword_MissingRequiredInstanceParameter_ThrowsException(string keyToRemove)
    {
        // Arrange
        var initializationInfo = new Dictionary<string, string>()
        {
            { "AppId", "TestAppId" },
            { "Host", "TestHost" },
            { "Site", "TestSite" }
        };

        var instanceParams = new Dictionary<string, string>()
        {
            {"Safe", "TestSafe"},
            {"Folder", "TestFolder"},
            {"Object", "TestObject"},
        };
        
        // Scenario 1: Key is missing from dictionary

        instanceParams.Remove(keyToRemove);
        
        // Act
        var exception1 = Assert.Throws<ArgumentException>(() => _sut.GetPassword(instanceParams, initializationInfo));
        
        // Assert
        Assert.Equal($"Required field {keyToRemove} was missing a value or was not defined as expected in dictionary.", exception1.Message);
        
        
        // Scenario 2: Key is present but value is null or whitespace
        instanceParams[keyToRemove] = "";
        
        // Act
        var exception2 = Assert.Throws<ArgumentException>(() => _sut.GetPassword(instanceParams, initializationInfo));
        
        // Assert
        Assert.Equal($"Required field {keyToRemove} was missing a value or was not defined as expected in dictionary.", exception2.Message);
    }

    [Fact]
    public void GetPassword_ValidConfiguration_ReturnsSecret()
    {
        // Arrange
        var initializationInfo = new Dictionary<string, string>()
        {
            { "AppId", Secrets.AppId },
            { "Host", Secrets.Host },
            { "Site", Secrets.Site }
        };

        var instanceParams = new Dictionary<string, string>()
        {
            {"Safe", Secrets.Safe },
            {"Folder", Secrets.Folder },
            {"Object", Secrets.Object },
        };

        var password = _sut.GetPassword(instanceParams, initializationInfo);
        
        Assert.Equal(Secrets.ExpectedSecret, password);
    }
    
    [Fact]
    public void GetPassword_ObjectDoesNotExist_ThrowsException()
    {
        // Arrange
        var initializationInfo = new Dictionary<string, string>()
        {
            { "AppId", Secrets.AppId },
            { "Host", Secrets.Host },
            { "Site", Secrets.Site }
        };

        var instanceParams = new Dictionary<string, string>()
        {
            {"Safe", Secrets.Safe },
            {"Folder", Secrets.Folder },
            {"Object", "objectdoesnotexist"},
        };

        var exception = Assert.Throws<HttpClientException>(() => _sut.GetPassword(instanceParams, initializationInfo));
        
        Assert.Equal("Exception of type 'Keyfactor.Extensions.Pam.CyberArk.HttpClientException' was thrown.", exception.Message);
    }
}
