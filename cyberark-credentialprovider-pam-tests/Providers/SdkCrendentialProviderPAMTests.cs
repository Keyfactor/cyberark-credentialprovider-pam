using Keyfactor.Extensions.Pam.CyberArk;

namespace cyberark_credentialprovider_pam_tests.Providers;

public class SdkCrendentialProviderPAMTests : BaseCredentialProviderPAMTest
{
    private readonly SdkCredentialProviderPAM _sut = new();
    
    protected override Dictionary<string, string> CreateInitializationInfo() => new()
    {
        { "AppId", TestAppId },
        { "Host", TestHost },
        { "Site", TestSite }
    };

    protected override Dictionary<string, string> CreateInstanceParams() => new()
    {
        { "Safe", TestSafe },
        { "Folder", TestFolder },
        { "Object", TestObject }
    };

    [Theory]
    [InlineData("AppId")]
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
    
    // TODO: Unit tests for successful secret retrieval once SDK client is abstracted (more difficult than expected!)
}
