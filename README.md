# CyberArk PAM Provider

A Keyfactor PAM Provider plugin supporting credential retrieval from a CyberArk Private Ark instance using a Credential Provider.

#### Integration status: Production - Ready for use in production environments.

## About the Keyfactor PAM Provider

Keyfactor supports the retrieval of credentials from 3rd party Priviledged Access Management (PAM) solutions. Secret values can normally be stored, encrypted at rest, in the Keyfactor Platform database. A PAM Provider can allow these secrets to be stored, managed, and rotated in an external platform. This integration is usually configured on the Keyfactor Platform itself, where the platform can request the credential values when needed. In certain scenarios, a PAM Provider can instead be run on a remote location in conjunction with a Keyfactor Orchestrator to allow credential requests to originate from a location other than the Keyfactor Platform.

---

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
