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
    
    // Test data constants
    private const string ExpectedSecret = "foobar";
    private const string TestAppId = "TestAppId";
    private const string TestHost = "TestHost";
    private const string TestSite = "TestSite";
    private const string TestSafe = "TestSafe";
    private const string TestFolder = "TestFolder";
    private const string TestObject = "TestObject";
    
    public CentralCredentialProviderPAMTests(ITestOutputHelper output)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddProvider(new XUnitLoggerProvider(output, new XUnitLoggerOptions()))
                .SetMinimumLevel(LogLevel.Trace));
        var logger = loggerFactory.CreateLogger<CentralCredentialProviderPAMTests>();

        _mockConjurHttpClient = new Mock<IConjurHttpClient>();
        
        _sut = new CentralCredentialProviderPAM(logger, _mockConjurHttpClient.Object);
    }
    
    private static Dictionary<string, string> CreateInitializationInfo() => new()
    {
        { "AppId", TestAppId },
        { "Host", TestHost },
        { "Site", TestSite }
    };

    private static Dictionary<string, string> CreateInstanceParams() => new()
    {
        { "Safe", TestSafe },
        { "Folder", TestFolder },
        { "Object", TestObject }
    };
    
    private void SetupSuccessfulPasswordRetrieval(string secret = ExpectedSecret)
    {
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent($"{{\"Content\":\"{secret}\"}}")
        };

        _mockConjurHttpClient
            .Setup(p => p.GetPassword(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(),
                It.IsAny<string>(), 
                It.IsAny<string>()))
            .Returns(httpResponse);
    }
    
    [Theory]
    [InlineData("AppId")]
    [InlineData("Host")]
    [InlineData("Site")]
    public void GetPassword_MissingRequiredInitializationParameter_ThrowsException(string keyToRemove)
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        var expectedMessage = $"Required field {keyToRemove} was missing a value or was not defined as expected in dictionary.";
        
        // Act & Assert - Scenario 1: Key is missing from dictionary
        initializationInfo.Remove(keyToRemove);
        var exception1 = Assert.Throws<ArgumentException>(() => 
            _sut.GetPassword(instanceParams, initializationInfo));
        Assert.Equal(expectedMessage, exception1.Message);
        
        // Act & Assert - Scenario 2: Key is present but value is empty
        initializationInfo[keyToRemove] = "";
        var exception2 = Assert.Throws<ArgumentException>(() => 
            _sut.GetPassword(instanceParams, initializationInfo));
        Assert.Equal(expectedMessage, exception2.Message);
    }
    
    [Theory]
    [InlineData("Safe")]
    [InlineData("Folder")]
    [InlineData("Object")]
    public void GetPassword_MissingRequiredInstanceParameter_ThrowsException(string keyToRemove)
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        var expectedMessage = $"Required field {keyToRemove} was missing a value or was not defined as expected in dictionary.";
        
        // Act & Assert - Scenario 1: Key is missing from dictionary
        instanceParams.Remove(keyToRemove);
        var exception1 = Assert.Throws<ArgumentException>(() => 
            _sut.GetPassword(instanceParams, initializationInfo));
        Assert.Equal(expectedMessage, exception1.Message);
        
        // Act & Assert - Scenario 2: Key is present but value is empty
        instanceParams[keyToRemove] = "";
        var exception2 = Assert.Throws<ArgumentException>(() => 
            _sut.GetPassword(instanceParams, initializationInfo));
        Assert.Equal(expectedMessage, exception2.Message);
    }

    [Fact]
    public void GetPassword_ValidConfiguration_ReturnsSecret()
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        SetupSuccessfulPasswordRetrieval();

        // Act
        var password = _sut.GetPassword(instanceParams, initializationInfo);
        
        // Assert
        Assert.Equal(ExpectedSecret, password);
    }
    
    [Fact]
    public void GetPassword_HostDoesNotIncludeScheme_AddsHttpsScheme()
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        initializationInfo["Host"] = "test.example.com:1234";
        
        var instanceParams = CreateInstanceParams();
        var expectedHostname = "https://test.example.com:1234/";

        SetupSuccessfulPasswordRetrieval();

        // Act
        _sut.GetPassword(instanceParams, initializationInfo);
        
        // Assert
        _mockConjurHttpClient.Verify(p => p.GetPassword(
            expectedHostname, 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(),
            It.IsAny<string>(), 
            It.IsAny<string>()), Times.Once);
    }
    
    [Theory]
    [InlineData("https://test.example.com:1234/")]
    [InlineData("http://test.example.com:1234/")]
    public void GetPassword_HostIncludesScheme_KeepsProvidedScheme(string host)
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        initializationInfo["Host"] = host;
        
        var instanceParams = CreateInstanceParams();
        SetupSuccessfulPasswordRetrieval();

        // Act
        _sut.GetPassword(instanceParams, initializationInfo);
        
        // Assert
        _mockConjurHttpClient.Verify(p => p.GetPassword(
            host, 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(),
            It.IsAny<string>(), 
            It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public void GetPassword_ObjectDoesNotExist_ThrowsException()
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        instanceParams["Object"] = "objectdoesnotexist";
        
        var errorResponse = "{\"ErrorCode\":\"APPAP004E\",\"ErrorMsg\":\"Password object matching query [Safe=partner;Folder=Root\\\\Secrets;Object=objectdoesnotexist] was not found (Diagnostic Info: 5). Please check that there is a password object that answers your query in the Vault and that both the Provider and the application user have the appropriate permissions needed in order to use the password.\"}";
        
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent(errorResponse)
        };
        
        _mockConjurHttpClient
            .Setup(p => p.GetPassword(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(),
                It.IsAny<string>(), 
                It.IsAny<string>()))
            .Returns(httpResponse);

        // Act
        var exception = Assert.Throws<HttpClientException>(() => 
            _sut.GetPassword(instanceParams, initializationInfo));
        
        // Assert
        Assert.Equal($"Failed to retrieve secret from CyberArk Central Credential Provider. Status Code: 404 (NotFound). Response message: {errorResponse}", exception.Message);
    }
}
