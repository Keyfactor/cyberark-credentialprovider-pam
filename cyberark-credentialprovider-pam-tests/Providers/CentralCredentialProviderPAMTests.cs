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
using cyberark_credentialprovider_pam_tests.Fakes;
using Keyfactor.Extensions.Pam.CyberArk;
using Keyfactor.Extensions.Pam.CyberArk.Clients;
using MartinCostello.Logging.XUnit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace cyberark_credentialprovider_pam_tests.Providers;

public class CentralCredentialProviderPAMTests
{
    private readonly ILogger _logger;
    private readonly CentralCredentialProviderPAM _sut;
    private readonly TestHttpMessageHandler _testHttpMessageHandler;
    
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
        _logger = loggerFactory.CreateLogger<CentralCredentialProviderPAMTests>();
        
        _testHttpMessageHandler = new TestHttpMessageHandler();
        var httpClient = new CyberArkVaultHttpClient(_logger, _testHttpMessageHandler);
        _sut = new CentralCredentialProviderPAM(_logger, httpClient);
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
    
    private void SetupSuccessfulPasswordRetrieval(string secret = ExpectedSecret, Action<HttpRequestMessage> onRequest = null)
    {
        _testHttpMessageHandler.HandlerFunc = (req, ct) =>
        {
            onRequest?.Invoke(req);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"{{\"Content\":\"{secret}\"}}")
            };
            return Task.FromResult(httpResponse);
        };
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

        HttpRequestMessage capturedRequest = null;
        SetupSuccessfulPasswordRetrieval(ExpectedSecret, req => capturedRequest = req);

        var instanceParams = CreateInstanceParams();

        // Act
        var password = _sut.GetPassword(instanceParams, initializationInfo);

        // Assert
        Assert.Equal(ExpectedSecret, password);
        Assert.NotNull(capturedRequest);
        Assert.Equal("test.example.com", capturedRequest.RequestUri.Host);
        Assert.Equal("https", capturedRequest.RequestUri.Scheme);
    }
    
    [Theory]
    [InlineData("https://test.example.com:1234/")]
    [InlineData("http://test.example.com:1234/")]
    public void GetPassword_HostIncludesScheme_KeepsProvidedScheme(string host)
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        initializationInfo["Host"] = host;

        HttpRequestMessage capturedRequest = null;
        SetupSuccessfulPasswordRetrieval(ExpectedSecret, req => capturedRequest = req);

        var instanceParams = CreateInstanceParams();

        // Act
        var password = _sut.GetPassword(instanceParams, initializationInfo);

        // Assert
        Assert.Equal(ExpectedSecret, password);
        Assert.NotNull(capturedRequest);
        Assert.StartsWith(host, capturedRequest.RequestUri.ToString());
    }
    
    [Fact]
    public void GetPassword_ObjectDoesNotExist_ThrowsException()
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        instanceParams["Object"] = "objectdoesnotexist";

        var errorResponse = "{\"ErrorCode\":\"APPAP004E\",\"ErrorMsg\":\"Password object matching query [Safe=partner;Folder=Root\\\\Secrets;Object=objectdoesnotexist] was not found (Diagnostic Info: 5). Please check that there is a password object that answers your query in the Vault and that both the Provider and the application user have the appropriate permissions needed in order to use the password.\"}";

        _testHttpMessageHandler.HandlerFunc = (req, ct) =>
        {
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(errorResponse)
            };
            return Task.FromResult(httpResponse);
        };

        // Act
        var exception = Assert.Throws<HttpClientException>(() =>
            _sut.GetPassword(instanceParams, initializationInfo));

        // Assert
        Assert.Contains("Failed to retrieve secret from CyberArk Central Credential Provider", exception.Message);
        Assert.Contains(errorResponse, exception.Message);
    }
}
