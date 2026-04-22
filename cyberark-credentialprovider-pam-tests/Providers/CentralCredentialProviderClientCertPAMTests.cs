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
using Keyfactor.Extensions.Pam.CyberArk.Exceptions;
using MartinCostello.Logging.XUnit;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace cyberark_credentialprovider_pam_tests.Providers;

public class CentralCredentialProviderClientCertPAMTests : BaseCredentialProviderPAMTest
{
    private readonly CentralCredentialProviderClientCertPAM _sut;
    private readonly TestHttpMessageHandler _testHttpMessageHandler;

    // A sample PFX certificate encoded in Base64 format for testing purposes only. Not used in any real environment.
    private readonly string _pfxBase64 =
        "MIIMFgIBAzCCC8wGCSqGSIb3DQEHAaCCC70Eggu5MIILtTCCBdoGCSqGSIb3DQEHBqCCBcswggXHAgEAMIIFwAYJKoZIhvcNAQcBMF8GCSqGSIb3DQEFDTBSMDEGCSqGSIb3DQEFDDAkBBDiQl49PJE42aWduSebGS65AgJOIDAMBggqhkiG9w0CCQUAMB0GCWCGSAFlAwQBKgQQ7ZJmX665zpxtsXlx7n/xk4CCBVCFsS79UR4fkmry7NdjcqsHl7XmWuAIfwv1NwgzDreFX3W7pFev8aXFfBK0ZcVXItLlYLcz/jDmX5X2vBrIMJKb4rPDYLnjTEdATtE+q22rdNaA8dqKh68VW2vooMBI0iLM+aqkvMqKYLUmrV79pXV4MU8Z0Ic32gpmsyDNNWATFzuj4GBe8rrYIL4mAUxA9kWuV2lR61Uc321kqfGXhNuaj1nFOtF5Re0QNofIAl07fy/W5mpdVSj1/eDG7HdvBrzkB1JeHeob41mmmRP78hg0CeOSHHDSHrag4K4swuzmcmX3sfdj3ZBr7HrfkYmYp/acdpHCNu2MKMb28bvDjBRZ/UAOVioNrWfTmetuB1xG/SdYS2eMQlLjSKLC2bNZsbSP77EGAtOKofY9zB1hC6O0EmOIabnnBIE8o5Z1AVmQtSsCswWUMxChj0DEeT1JXpAVrC8BAwXkvZw7DiVdSF4L5aUJ1zKYsRulpl/zTM+2hqsyduWAcc4MMD4lSgdh0RWBH/bftSoNsCzSpK5z9BNgR8bO5LO6R8mBX2yn0cSA37WqOtf0tMtw2wNx1klv5bJKiDqKPqTZuDe1E4wF/tlpDkULYv0YPULQ49psvb0e+aTVuQZZJaveml0zlvEnkr1Cx2C3hoAGQX+XGEyghhNM/pO8wxMb32rMBsmX9nHRnUyt2gzH+NxdYm3/o5c4sBapBl7KPtSkGGBPjSAAKd8gbhuDp+JYU02iPzWvWDFYCYiliGpyS/1tlfOd0+sYsuXy0hTTxcb+YKjoE2x25LlPOwrOuq4aPTcVMNraRjOuTH+RC8xAZyJH74Mi2/Ap5p/efg2rbSl+X9ct0sq/76mPoSwLDA7bw3JL6uPQr4niPj50fcf0I+9+lnnNwqAmo8s60sC4bODEQEIL5hyaJgMowxctSB5oMo13oqhGmH/6g9JscmUQ16owAdmIUuuc5VgW/iqyCs+PkW0oXj79ABRWTesA5GjIF9TU3haDxLaEkokmARKv6nc+ST8TLOkrw+9g4iMkoaLtojaoWffsJqAv6nAYkUZJmlnOTJ73YrcNGxV8md5x4OsV1Aq2DebT4tUSNX7X2NNbzgN+NBfg1w2phLfu6CWK602utz2PyTj4nrrI4zyj4JINTaPDcLqRHSDAymBvsLfWMlECnQBGObYTl82G/VP5Q+YO5qC4cEVuJkFHHqeYTIFF/V9uwvUBNUU1I60MANcnlw61pfxjnbcaBLQf0a0Mqv0JgYjxFXg+v3FaeJdh16ABcDqjh+o8YcP2dMjUeu7s37MEj5BTtsG2ALOMcmcvL88JZPs9Taioz9tZYhe8UKpNYNMiI2Ie93QgYkaw8cDcQeUdRw00FaV4N5yeyeOqRJF1OrCSfMRwo2vWmFvQplskTyHpHhuBuRpTHcj+N6R+W5WTKrhGBEBFtBKPSAefW5DjaZMijoI2TwIr6KVyU+nZd2gdlXwSbUn3mwpfDwm6DyXZgTEla97bP+o7ybmQFB4mHg6h+uWkMG6pGNk5ShuKjB2d7K2AnwUB2NdvQxT2kYnw4rsgR+po0qSaLIoGaoTTZCwyO6YjCv1hDLdwh+z2KGvWopZx/5qVH6r1NRr2TNwqm10wXr9eBXlLCco6nSmA4hN4Dqpi5nEbQrnlIpUjT1KyT6dElykZz8F3p0ItK7F+ZQ4GKRQ7yxOJxt5Exy4l7o5y1YTCbRDDphwk3/r/pV4iAa8gUpxb1sVyX7vFm6DaCS5ZMFVqqU29yJiLrMeCQ2P3CNRKFmJBzG6yGwRQdvFZU8GxLD1k9kONxnkR3oQ1BTHsY4p1MIIF0wYJKoZIhvcNAQcBoIIFxASCBcAwggW8MIIFuAYLKoZIhvcNAQwKAQKgggU5MIIFNTBfBgkqhkiG9w0BBQ0wUjAxBgkqhkiG9w0BBQwwJAQQv50smQbexwEddTRWEqkTNgICTiAwDAYIKoZIhvcNAgkFADAdBglghkgBZQMEASoEEI2GOPulY9Lmb2Kd3LvNXd8EggTQ7ZP5M5Monn8fa7L9WWkRaM+XIxiJ4xbVB9tEkaI3IB+4/3oraQWfdRXw9ts/M3dszZVkW/DWP9n3Drdzz0KVSBRp5dlE2irJnzE6jlo3EOFPtco/YIyCEM82+BY7sAOqur6h15UkDCG6RmsJwJetwnfrFAkz00DlX4YjwcYg5QdI9FRs7ToOosYvv9SJYiLOxLsfLkU6Gpk7FIhnm6Zaw2rEXJqUvDbaXW9HAp4U0XuxRZ3WNVWBTi/Vwy4h1s3AmBgsQDzDy8dWNXV0nw28X3rOdvavcbDIHjn8rfue+V8eqw03iy02cFM4en8VcLcmtCEYmKgBbr6T9FvXffghJLPaDz39G1/UFa0l8t0ql5AeHdrbQqVU6vMoynX4sjG4Qy2NJ6NAsOFHj0qiDt8Xu07Y1ICQ2u/GEb8viKul10ZmKhjSd/eLm9d/ewSIR+LVl2LSfdNgsYv7/StvhXfTRtvLZz107D6622bV3OkINt8to4YqxvNzI95m48oXeyGyBSS9T+4eSESe4aKw4mFEle9UpBfAvTYGW1NRup5D59Mpxs8yJX2cBJToG+maQg4VX242TDVI8l4HmX7tyUQKSJltX70OncBEddh6AR5JViPQHM3epSYpAkmUw0uNY4pzo5c+KxprPYye32iIRQLAfEMFFaQ7D4qvNgzrH6TkQ1xquRPxyrV1MQh3oxAJbleaz6SIFMQzOAyV03iujhYiVEFcorgw4Bcm8aA0KIwb/1TM5m/g/EZXxTk5fnBeMxXqtGWN6iEkIk7060jkmd8GVzJar6CHcm7Zi4jvxI9Ipo1n4nvaqOf+IgugTy+khyG8ECFjzTHzsyOXTDeJ6pmNG7yZfqNtzOX5r+xn0LmXgLxDZxCdYg2zVit0vqUo0N5Xr8ZMiHv85QfZAPmfZuU+ik63YfvzOubg8lbAquBJ0+VCDpi04cWb0HTNIZQd8KQfOaRAQtRA+6ad2IHMsA2A3ryhe6j6SN113pKrV6HM8n6Hk+MlD+2oVfXSb5kXwiNvcYbN+vsrDFThsCIPJ6nKXy12vOIEzOw54MZdSbd3oPmjhLOrD/dv8MxhCkPYvN3AYFU9enkAx3T9SLGmaa47jYrGhpdN0vvn5iDWYYkBb60LEfZXE2djQM1dfuVuA/b7Nxa1X0ulMCOUJRCgwH3+wmaQypTV8ta6N75y9+O407BDQtL9Arxqx9dbCqoDMQxTUk7PfK/cYN1cmGckFInraEBDM13K7jfA+ek4htfNpQ6kazrlb+XGGbyzMSD88eSnl7D7Z27myCWWRU99CzZVTVW4J66wOd/A5GDNgbwJFMquo05X/hc5/YcpsIHrHkineY5gYdZbLzlEggaSb1qotc4HGtPvhZ+H25IL8d06VgSjLFXc4OljFRldxpO39BnxiwJv5WCKaAVs3k7ZWf/QioCYbmXwWk1EVGQImV4telMcuhOz0/U77vwzc9dU4X66Kzh+ga8KlUtHxBRVvY6x+md6ESHIpaKXm92K6ryhF9jdLmiRegg33ybrthd+cBvLkLDGfPRfLXfa4+ChE8bokANcG4DMSh4fxTydinB1aClY6R9SmcRuDuDZsIMJLUnnbNSBQ2kEVAJOpqYOAURw8x8zIfgah5arR3PDs1Nv3/wxbDAjBgkqhkiG9w0BCRUxFgQUxefibt9V/6VU3QWtODsivbhOwXEwRQYJKoZIhvcNAQkUMTgeNgBDAHkAYgBlAHIAQQByAGsAIABDAGwAaQBlAG4AdAAgAEMAZQByAHQAaQBmAGkAYwBhAHQAZTBBMDEwDQYJYIZIAWUDBAIBBQAEIMttJSjb7+f6kpzPpB6mIqld1V2g0g7Bkrja3rZ2F6YpBAhT+fNiHaB6HAICCAA=";

    // The password used to protect the PFX certificate above. Needs to be readable in tests, so it is intentionally simple and not secure.
    private readonly string _pfxPassword = "VerySecurePassword";

    private string ValidPfxFilePath => Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "pfx_files", "valid_pfx_base64.pfx");

    public CentralCredentialProviderClientCertPAMTests(ITestOutputHelper output)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddProvider(new XUnitLoggerProvider(output, new XUnitLoggerOptions()))
                .SetMinimumLevel(LogLevel.Trace));
        var logger = loggerFactory.CreateLogger<CentralCredentialProviderClientCertPAMTests>();

        _testHttpMessageHandler = new TestHttpMessageHandler();
        _sut = new CentralCredentialProviderClientCertPAM(logger, _testHttpMessageHandler);
    }

    protected override Dictionary<string, string> CreateInitializationInfo() => new()
    {
        { "AppId", TestAppId },
        { "Host", TestHost },
        { "Site", TestSite },
        { "PfxBase64", _pfxBase64 },
        { "PfxPassword", _pfxPassword }
    };

    protected override Dictionary<string, string> CreateInstanceParams() => new()
    {
        { "Safe", TestSafe },
        { "Folder", TestFolder },
        { "Object", TestObject }
    };

    private void SetupSuccessfulPasswordRetrieval(string secret = ExpectedSecret, Action<HttpRequestMessage>? onRequest = null)
    {
        _testHttpMessageHandler.HandlerFunc = (req, ct) =>
        {
            onRequest?.Invoke(req);
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"{{\"Content\":\"{secret}\"}}")
            });
        };
    }

    // -------------------------------------------------------------------------
    // Required field validation
    // -------------------------------------------------------------------------

    // null = key is absent from dictionary; otherwise the value is set to that string.
    public static TheoryData<string, string?> InvalidRequiredInitializationFields => new()
    {
        { "AppId",       null  }, { "AppId",       "" }, { "AppId",       "   " },
        { "Host",        null  }, { "Host",        "" }, { "Host",        "   " },
        { "Site",        null  }, { "Site",        "" }, { "Site",        "   " },
        { "PfxPassword", null  }, { "PfxPassword", "" }, { "PfxPassword", "   " },
    };

    [Theory]
    [MemberData(nameof(InvalidRequiredInitializationFields))]
    public void GetPassword_InvalidRequiredInitializationField_ThrowsException(string key, string? value)
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        if (value is null) initializationInfo.Remove(key);
        else initializationInfo[key] = value;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _sut.GetPassword(instanceParams, initializationInfo));
        Assert.Equal($"Required initialization info field {key} was missing a value or was not defined as expected in dictionary.", ex.Message);
    }

    public static TheoryData<string, string?> InvalidRequiredInstanceParameterFields => new()
    {
        { "Safe",   null }, { "Safe",   "" }, { "Safe",   "   " },
        { "Folder", null }, { "Folder", "" }, { "Folder", "   " },
        { "Object", null }, { "Object", "" }, { "Object", "   " },
    };

    [Theory]
    [MemberData(nameof(InvalidRequiredInstanceParameterFields))]
    public void GetPassword_InvalidRequiredInstanceParameter_ThrowsException(string key, string? value)
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        if (value is null) instanceParams.Remove(key);
        else instanceParams[key] = value;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _sut.GetPassword(instanceParams, initializationInfo));
        Assert.Equal($"Required instance parameter field {key} was missing a value or was not defined as expected in dictionary.", ex.Message);
    }

    // -------------------------------------------------------------------------
    // PFX source selection
    // -------------------------------------------------------------------------

    // null = key absent; "" / "   " / "none" / "NONE" = explicitly ignored values.
    [Theory]
    [InlineData(null,    null   )]  // both absent
    [InlineData("",      null   )]  // base64 empty,      path absent
    [InlineData("   ",   null   )]  // base64 whitespace,  path absent
    [InlineData("none",  null   )]  // base64 = "none",    path absent
    [InlineData("NONE",  null   )]  // base64 = "NONE",    path absent
    [InlineData(null,    ""     )]  // base64 absent,      path empty
    [InlineData(null,    "   "  )]  // base64 absent,      path whitespace
    [InlineData(null,    "none" )]  // base64 absent,      path = "none"
    [InlineData("none",  "none" )]  // both "none"
    [InlineData("   ",   "   "  )]  // both whitespace
    public void GetPassword_NoPfxSourceIsEffective_ThrowsException(string? base64Value, string? pathValue)
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();

        if (base64Value is null) initializationInfo.Remove("PfxBase64");
        else initializationInfo["PfxBase64"] = base64Value;

        if (pathValue is null) initializationInfo.Remove("PfxFilePath");
        else initializationInfo["PfxFilePath"] = pathValue;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _sut.GetPassword(instanceParams, initializationInfo));
        Assert.Equal("Either PfxBase64 or PfxFilePath must be provided in initialization info", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("none")]
    [InlineData("NONE")]
    public void GetPassword_PfxBase64IsIgnored_FallsBackToPfxFilePath(string ignoredBase64)
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        initializationInfo["PfxBase64"] = ignoredBase64;
        initializationInfo["PfxFilePath"] = ValidPfxFilePath;
        SetupSuccessfulPasswordRetrieval();

        // Act
        var password = _sut.GetPassword(instanceParams, initializationInfo);

        // Assert
        Assert.Equal(ExpectedSecret, password);
    }
    
    [Fact]
    public void GetPassword_PfxFilePathDoesNotExist_ThrowsException()
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        initializationInfo.Remove("PfxBase64");
        initializationInfo["PfxFilePath"] = "/nonexistent/path/to/certificate.pfx";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _sut.GetPassword(instanceParams, initializationInfo));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("none")]
    [InlineData("NONE")]
    public void GetPassword_PfxFilePathIsIgnored_PfxBase64IsUsed(string ignoredFilePath)
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        initializationInfo["PfxFilePath"] = ignoredFilePath;
        SetupSuccessfulPasswordRetrieval();

        // Act
        var password = _sut.GetPassword(instanceParams, initializationInfo);

        // Assert
        Assert.Equal(ExpectedSecret, password);
    }

    [Theory]
    [InlineData(49)]
    [InlineData(50)]
    [InlineData(51)]
    public void GetPassword_BothPfxBase64AndPfxFilePathProvided_PfxBase64TakesPrecedence(int base64Length)
    {
        // Arrange - invalid base64 ensures the test fails at decoding, not at file I/O, proving PfxBase64 was chosen
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        initializationInfo["PfxBase64"] = new string('a', base64Length);
        initializationInfo["PfxFilePath"] = ValidPfxFilePath;

        // Act & Assert
        Assert.Throws<FormatException>(() => _sut.GetPassword(instanceParams, initializationInfo));
    }

    // -------------------------------------------------------------------------
    // Happy path
    // -------------------------------------------------------------------------

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
    public void GetPassword_PfxFilePathIsUsed_ReturnsSecret()
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        initializationInfo.Remove("PfxBase64");
        initializationInfo["PfxFilePath"] = ValidPfxFilePath;
        SetupSuccessfulPasswordRetrieval();

        // Act
        var password = _sut.GetPassword(instanceParams, initializationInfo);

        // Assert
        Assert.Equal(ExpectedSecret, password);
    }

    // -------------------------------------------------------------------------
    // Host / URI handling
    // -------------------------------------------------------------------------

    [Fact]
    public void GetPassword_HostDoesNotIncludeScheme_AddsHttpsScheme()
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        initializationInfo["Host"] = "test.example.com:1234";
        var instanceParams = CreateInstanceParams();

        HttpRequestMessage? capturedRequest = null;
        SetupSuccessfulPasswordRetrieval(ExpectedSecret, req => capturedRequest = req);

        // Act
        var password = _sut.GetPassword(instanceParams, initializationInfo);

        // Assert
        Assert.Equal(ExpectedSecret, password);
        Assert.NotNull(capturedRequest);
        Assert.Equal("test.example.com", capturedRequest.RequestUri!.Host);
        Assert.Equal("https", capturedRequest.RequestUri.Scheme);
    }

    [Theory]
    [InlineData("https://test.example.com:1234/")]
    [InlineData("http://test.example.com:1234/")]
    [InlineData("http://test.example.com:1234")]
    public void GetPassword_HostIncludesScheme_KeepsProvidedScheme(string host)
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        initializationInfo["Host"] = host;
        var instanceParams = CreateInstanceParams();

        HttpRequestMessage? capturedRequest = null;
        SetupSuccessfulPasswordRetrieval(ExpectedSecret, req => capturedRequest = req);

        // Act
        var password = _sut.GetPassword(instanceParams, initializationInfo);

        // Assert
        Assert.Equal(ExpectedSecret, password);
        Assert.NotNull(capturedRequest);
        Assert.StartsWith(host, capturedRequest.RequestUri!.ToString());
    }

    // -------------------------------------------------------------------------
    // CyberArk API error handling
    // -------------------------------------------------------------------------

    [Fact]
    public void GetPassword_ObjectDoesNotExist_ThrowsException()
    {
        // Arrange
        var initializationInfo = CreateInitializationInfo();
        var instanceParams = CreateInstanceParams();
        instanceParams["Object"] = "objectdoesnotexist";

        var errorResponse = "{\"ErrorCode\":\"APPAP004E\",\"ErrorMsg\":\"Password object matching query [Safe=partner;Folder=Root\\\\Secrets;Object=objectdoesnotexist] was not found (Diagnostic Info: 5). Please check that there is a password object that answers your query in the Vault and that both the Provider and the application user have the appropriate permissions needed in order to use the password.\"}";

        _testHttpMessageHandler.HandlerFunc = (req, ct) => Task.FromResult(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = new StringContent(errorResponse)
        });

        // Act
        var exception = Assert.Throws<HttpClientException>(() => _sut.GetPassword(instanceParams, initializationInfo));

        // Assert
        Assert.Contains("Failed to retrieve secret from CyberArk Central Credential Provider", exception.Message);
        Assert.Contains(errorResponse, exception.Message);
    }
}
