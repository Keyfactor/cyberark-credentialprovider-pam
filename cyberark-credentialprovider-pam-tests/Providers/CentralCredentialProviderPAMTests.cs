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

using System.Net;
using Keyfactor.Extensions.Pam.CyberArk;
using Keyfactor.Extensions.Pam.CyberArk.Clients;
using MartinCostello.Logging.XUnit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace cyberark_credentialprovider_pam_tests.Providers;

public class CentralCredentialProviderPAMTests
{
    private readonly CentralCredentialProviderPAM _sut;
    private readonly Mock<IConjurHttpClient> _mockConjurHttpClient;
    
    public CentralCredentialProviderPAMTests(ITestOutputHelper output)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddProvider(new XUnitLoggerProvider(output, new XUnitLoggerOptions()))
                .SetMinimumLevel(LogLevel.Trace));
        var logger = loggerFactory.CreateLogger<CentralCredentialProviderPAMTests>();

        _mockConjurHttpClient = new Mock<IConjurHttpClient>();
        
        _sut = new CentralCredentialProviderPAM(logger, _mockConjurHttpClient.Object);
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
            { "AppId", "TestAppId" },
            { "Host", "TestHost" },
            { "Site", "TestSite" }
        };

        var instanceParams = new Dictionary<string, string>()
        {
            {"Safe", "TestSafe" },
            {"Folder", "TestFolder" },
            {"Object", "TestObject"},
        };

        var expectedSecret = "foobar";

        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent($"{{\"Content\":\"{expectedSecret}\"}}")
        };

        _mockConjurHttpClient
            .Setup(p => p.GetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()))
            .Returns(httpResponse);

        // Act
        var password = _sut.GetPassword(instanceParams, initializationInfo);
        
        // Assert
        Assert.Equal(expectedSecret, password);
    }
    
    [Fact]
    public void GetPassword_HostDoesNotIncludeScheme_AddsHttpsScheme()
    {
        // Arrange
        var initializationInfo = new Dictionary<string, string>()
        {
            { "AppId", "TestAppId" },
            { "Host", "test.example.com:1234" },
            { "Site", "TestSite" }
        };

        var instanceParams = new Dictionary<string, string>()
        {
            {"Safe", "TestSafe" },
            {"Folder", "TestFolder" },
            {"Object", "TestObject"},
        };
        
        var expectedHostname = "https://test.example.com:1234/";

        var expectedSecret = "foobar";

        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent($"{{\"Content\":\"{expectedSecret}\"}}")
        };

        _mockConjurHttpClient
            .Setup(p => p.GetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()))
            .Returns(httpResponse);

        // Act
        var password = _sut.GetPassword(instanceParams, initializationInfo);
        
        // Assert
        _mockConjurHttpClient.Verify(p => p.GetPassword(expectedHostname, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
    
    [Theory]
    [InlineData("https://test.example.com:1234/")]
    [InlineData("http://test.example.com:1234/")]
    public void GetPassword_HostIncludesScheme_KeepsProvidedScheme(string host)
    {
        // Arrange
        var initializationInfo = new Dictionary<string, string>()
        {
            { "AppId", "TestAppId" },
            { "Host", host },
            { "Site", "TestSite" }
        };

        var instanceParams = new Dictionary<string, string>()
        {
            {"Safe", "TestSafe" },
            {"Folder", "TestFolder" },
            {"Object", "TestObject"},
        };
        
        var expectedHostname = host;

        var expectedSecret = "foobar";

        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent($"{{\"Content\":\"{expectedSecret}\"}}")
        };

        _mockConjurHttpClient
            .Setup(p => p.GetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()))
            .Returns(httpResponse);

        // Act
        var password = _sut.GetPassword(instanceParams, initializationInfo);
        
        // Assert
        _mockConjurHttpClient.Verify(p => p.GetPassword(expectedHostname, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public void GetPassword_ObjectDoesNotExist_ThrowsException()
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
            {"Safe", "TestSafe" },
            {"Folder", "TestFolder" },
            {"Object", "objectdoesnotexist"},
        };
        
        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent($"{{\"ErrorCode\":\"APPAP004E\",\"ErrorMsg\":\"Password object matching query [Safe=partner;Folder=Root\\\\Secrets;Object=objectdoesnotexist] was not found (Diagnostic Info: 5). Please check that there is a password object that answers your query in the Vault and that both the Provider and the application user have the appropriate permissions needed in order to use the password.\"}}")
        };
        
        _mockConjurHttpClient
            .Setup(p => p.GetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()))
            .Returns(httpResponse);

        // Act
        var exception = Assert.Throws<HttpClientException>(() => _sut.GetPassword(instanceParams, initializationInfo));
        
        // Assert
        Assert.Equal("Failed to retrieve secret from CyberArk Central Credential Provider. Status Code: 404 (NotFound). Response message: {\"ErrorCode\":\"APPAP004E\",\"ErrorMsg\":\"Password object matching query [Safe=partner;Folder=Root\\\\Secrets;Object=objectdoesnotexist] was not found (Diagnostic Info: 5). Please check that there is a password object that answers your query in the Vault and that both the Provider and the application user have the appropriate permissions needed in order to use the password.\"}", exception.Message);
    }
}
