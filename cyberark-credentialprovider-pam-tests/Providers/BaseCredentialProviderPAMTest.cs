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
