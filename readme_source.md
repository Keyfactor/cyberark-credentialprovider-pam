___Note: This PAM Provider is already available in the main Keyfactor Platform, there is no release on GitHub.___

### Configuring Parameters

| Initialization parameter | Description | Instance parameter | Description |
| :---: | --- | :---: | --- |
| Safe | The name of the safe where the password to be retrieved lives. | Object | The name of the password to be retrieved. |
| AppId | The user / application name that is configured in AIM to talk to CyberArk from the machine. | Folder | The folder the password is inside in the vault. |

### Special Considerations
The CyberArk PAM Provider included in the Keyfactor Platform is originally targeted to support version 10.5.1.3 of CyberArk. However, it is possible to patch in support for later versions of CyberArk (tested up to 12.4.1.8). In order to do so, an assembly redirect needs to be added when enabling the CyberArk plugin.

For 12.4.1.8 support:
```xml
<dependentAssembly>
    <publisherPolicy apply="no" />
    <assemblyIdentity name="NetPasswordSDK" publicKeyToken="40be1dbc8718670f" />
    <bindingRedirect oldVersion="10.5.1.0-10.5.1.3" newVersion="12.4.1.0" />
</dependentAssembly>
```

Note that the `newVersion` attribute omits the 4th number and should be a 0 instead, when targeting any different CyberArk version in an assembly redirect.