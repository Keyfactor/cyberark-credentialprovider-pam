
For registering the CyberArk for use with the SDK-based Credential Provider, use the following `<register>` instead.
Make sure to enter in the correct full path to the directory for `extensionPath` that has the SDK DLL for the PAM Provider.

```xml
<register type="IPAMProvider" mapTo="Keyfactor.Extensions.Pam.CyberArk.SdkCredentialProviderPAM, cyberark-credentialprovider-pam" name="CyberArk-SdkCredentialProvider">
  <constructor>
    <param name="extensionPath">
      <value value="C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\bin"/>
    </param>
  </constructor>
</register>
```
