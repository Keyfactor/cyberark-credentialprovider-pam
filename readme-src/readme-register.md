
For registering the CyberArk for use with the SDK-based Credential Provider, use the following `<register>` instead.

```xml
<register type="IPAMProvider" mapTo="Keyfactor.Extensions.Pam.CyberArk.SdkCredentialProviderPAM, cyberark-credentialprovider-pam" name="CyberArk-SdkCredentialProvider">
  <constructor>
    <param name="extensionPath">
      <value value="C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\bin"/>
    </param>
  </constructor>
</register>
```
