# CyberArk PAM Provider

A Keyfactor PAM Provider plugin supporting credential retrieval with a CyberArk Credential Provider. The Central Credential Provider (cloud-hosted) can be used, or the standard Credential Provider with installed SDK.

#### Integration status: Production - Ready for use in production environments.


## About the Keyfactor PAM Provider

Keyfactor supports the retrieval of credentials from 3rd party Privileged Access Management (PAM) solutions. Secret values can normally be stored, encrypted at rest, in the Keyfactor Platform database. A PAM Provider can allow these secrets to be stored, managed, and rotated in an external platform. This integration is usually configured on the Keyfactor Platform itself, where the platform can request the credential values when needed. In certain scenarios, a PAM Provider can instead be run on a remote location in conjunction with a Keyfactor Orchestrator to allow credential requests to originate from a location other than the Keyfactor Platform.




## Support for CyberArk PAM Provider

CyberArk PAM Provider is supported by Keyfactor for Keyfactor customers. If you have a support issue, please open a support ticket with your Keyfactor representative.

###### To report a problem or suggest a new feature, use the **[Issues](../../issues)** tab. If you want to contribute actual bug fixes or proposed enhancements, use the **[Pull requests](../../pulls)** tab.



---






### Initial Configuration of PAM Provider
In order to allow Keyfactor to use the new CyberArk PAM Provider, the definition needs to be added to the application database.
This is done by running the provided `kfutil` tool to install the PAM definition, which only needs to be done one time. It uses API credentials to access the Keyfactor instance and create the PAM definition.

The `kfutil` tool, after being [configured for API access](https://github.com/Keyfactor/kfutil#quickstart), can be run in the following manner to install the PAM definition from the Keyfactor repository:

```
kfutil pam types-create -r cyberark-credentialprovider-pam -n CyberArk-CentralCredentialProvider
```

### Configuring Parameters
The following are the parameter names and a description of the values needed to configure the CyberArk PAM Provider.

#### Central Credential Provider
__Initialization Parameters for each defined PAM Provider instance__
| Initialization parameter | Display Name | Description |
| :---: | :---: | --- |
| AppId | Application ID | The Application ID to use that has access set up for the Safe |
| Host | CyberArk Host and Port | The hostname (IP address or domain name) including the port. E.G. my.cyberark.net:404 (note: no https:// included) |
| Site | CyberArk API Site | By default, AIMWebService is the site name, but may be deployed to another site name. |


__Instance Parameters for each retrieved secret field__
| Instance parameter | Display Name | Description |
| :---: | :---: | --- |
| Safe | Safe | The name of the Safe the credential resides in. |
| Folder | Folder | The folder path the credential lives in. If it is nested, use the forward slash e.g. Root\\Folder |
| Object | Object | The name of the password object that has the credential. |

When using the SDK Credential Provider, the parameters are instead defined as follows:
#### SDK Credential Provider
__Initialization Parameters for each defined PAM Provider instance__
| Initialization parameter | Display Name | Description |
| :---: | :---: | --- |
| AppId | Application ID | The Application ID to use that has access set up for the Safe |

__Instance Parameters for each retrieved secret field__
| Instance parameter | Display Name | Description |
| :---: | :---: | --- |
| Safe | Safe | The name of the Safe the credential resides in. |
| Folder | Folder | The folder path the credential lives in. If it is nested, use the forward slash e.g. Root\\Folder |
| Object | Object | The name of the password object that has the credential. |

![](images/config.png)

### Configuring for PAM Usage
#### For CyberArk Central Credential Provider
In order for the Central Credential Provider to work, the Safe / Secret being accessed need to be available to the Provider that the CyberArk server is using, and the Application ID needs to be usable from an external requestor.This may require adding IP address or other rules.

Certificate Authentication cannot be required. This may necessitate creating a Site that allows HTTPS requests but does not require a Client Certificate to authenticate. By default the site `AIMWebService` may require a Client Certificate, which would need to be edited or have another site created.

#### For SDK-based local Credential Provider
To use a local Credential Provider instead, the Credential Provider will need to be installed on the machine that is using the PAM Provider. After installing the Credential Provider, copy the `NetStandardPasswordSDK.dll` assembly from the install location into the PAM Provider install location. This dll __needs__ to be adjacent to `cyberark-credentialprovider-pam.dll` to be properly loaded.

After registering the Credential Provider during install, make sure the Provider for the machine has been granted permission to access the Safe, as well as the Application ID that will be used.

#### In Keyfactor - PAM Provider
##### Installation
In order to setup a new PAM Provider in the Keyfactor Platform for the first time, you will need to run the `kfutil` tool (see Initial Configuration of PAM Provider).

After the installation is run, the DLLs need to be installed to the correct location for the PAM Provider to function. From the release, the cyberark-credentialprovider-pam.dll should be copied to the following folder locations in the Keyfactor installation. Once the DLL has been copied to these folders, edit the corresponding config file. You will need to add a new Unity entry as follows under `<container>`, next to other `<register>` tags.

| Install Location | DLL Binary Folder | Config File |
| --- | --- | --- |
| WebAgentServices | WebAgentServices\bin\ | WebAgentServices\web.config |
| Service | Service\ | Service\CMSTimerService.exe.config |
| KeyfactorAPI | KeyfactorAPI\bin\ | KeyfactorAPI\web.config |
| WebConsole | WebConsole\bin\ | WebConsole\web.config |

When enabling a PAM provider for Orchestrators only, the first line for `WebAgentServices` is the only installation needed.

The Keyfactor service and IIS Server should be restarted after making these changes.

```xml
<register type="IPAMProvider" mapTo="Keyfactor.Extensions.Pam.CyberArk.CentralCredentialProviderPAM, cyberark-credentialprovider-pam" name="CyberArk-CentralCredentialProvider" />
```



For registering the CyberArk for use with the SDK-based Credential Provider, use the following `<register>` instead.

```xml
<register type="IPAMProvider" mapTo="Keyfactor.Extensions.Pam.CyberArk.SdkCredentialProviderPAM, cyberark-credentialprovider-pam" name="CyberArk-SdkCredentialProvider" />
```

##### Usage
In order to use the PAM Provider, the provider's configuration must be set in the Keyfactor Platform. In the settings menu (upper right cog) you can select the ___Priviledged Access Management___ option to configure your provider instance.

![](images/setting.png)

After it is set up, you can now use your PAM Provider when configuring certificate stores. Any field that is treated as a Keyfactor secret, such as server passwords and certificate store passwords can be retrieved from your PAM Provider instead of being entered in directly as a secret.

![](images/password.png)


---


#### Considerations for PAM on Universal Orchestrator

There are 2 PAM interfaces provided for using CyberArk as a PAM Provider. By default, the Central Credential Provider will be used when installing on a Universal Orchestrator, as that PAM interface is what is defined by the default `manifest.json`. The Initialization Parameters can be edited as normal in this file when you are planning to use the Central Credential Provider.

If instead, the SDK-based Credential Provider should be used on the Universal Orchestrator, the extension should be installed normally, with an additonal step after placing the files in the extension folder. In the CyberArk-CredentialProvider extension folder on the Keyfactor Universal Orchestrator, the `SDK-manifest.json` manifest file needs to be used instead of the standard `manifest.json`. You should delete or rename the original `manifest.json` to `Central-manifest.json` and rename the `SDK-manifest.json` to be the new `manifest.json`. At this point, you can edit the values in the manifest to properly load the SDK-based PAM Provider.

