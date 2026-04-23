## CyberArk-SdkCredentialProvider

The CyberArk SDK Credential Provider uses the CyberArk SDK in order to communicate with a locally installed [Credential Provider](https://docs.cyberark.com/credential-providers/latest/en/content/cp%20and%20ascp/lp_cp.htm).
When the Credential Provider is installed locally, authentication needs to be configured correctly for the provider to communicate with a CyberArk instance over their proprietary protocol.

## Requirements

> [!IMPORTANT]
> 
> If using this PAM type, you will need to replace the `manifest.json` file with the contents of `SDK-manifest.json`. Please see the `Install PAM provider on a Universal Orchestrator Host (Remote) - manifest.json` section below for more details.

After installing the Credential Provider, copy the `NetStandardPasswordSDK.dll` assembly from the install location into the PAM Provider install location. This dll should be stored in the same directory as `cyberark-credentialprovider-pam.dll` to be properly loaded. The name `NetStandardPasswordSDK.dll` is case-sensitive, so make sure the name is copied to the directory as `NetStandardPasswordSDK.dll`.

After registering the Credential Provider during install, make sure the Provider for the machine has been granted permission to access the Safe, as well as the Application ID that will be used.

To read secrets stored in a CyberArk Vault safe, the Partner must have at least the following permissions on the safe:
- Monitor Safe
- Retrieve files from Safe

### Install PAM provider on a Universal Orchestrator Host (Remote) - manifest.json

The default <code>manifest.json</code> needs to be replaced with the included <code>SDK-manifest.json</code>. Rename the existing <code>manifest.json</code> as <code>Central-manifest.json</code> and then rename the <code>SDK-manifest.json</code> to replace the original <code>manifest.json</code>.

### IMPORTANT NOTE FOR LINUX INSTALLATIONS

If you have installed the Credential Provider on a Linux instance and run this PAM extension within a [Universal Orchestrator as a Linux service](https://software.keyfactor.com/Core-OnPrem/Current/Content/InstallingAgents/NetCoreOrchestrator/InstalltheOrchestratorLinux.htm), you may run into issues communicating with the Credential Provider service and receive an `ENCPR019E` error code when the PAM instance tries to retrieve a credential. 

By default, the Universal Orchestrator Linux service has [PrivateTmp](https://www.redhat.com/en/blog/new-red-hat-enterprise-linux-7-security-feature-privatetmp) enabled, which creates a separate, isolated `/tmp` directory for the service. The CyberArk Credential Provider creates a named pipe in the `/tmp` directory to communicate with other applications, so when PrivateTmp is enabled, the PAM instance cannot access this pipe to communicate with the Credential Provider. To resolve this issue, you can disable PrivateTmp for the Universal Orchestrator Linux service by creating an override file with the command `sudo systemctl edit keyfactor-orchestrator-default.service` and adding the following lines:

```
[Service]
PrivateTmp=false
```

and then run the following commands to restart the service daemon and the Universal Orchestrator service:

```bash
sudo systemctl daemon-reload
sudo systemctl restart keyfactor-orchestrator-default.service
```




## Mechanics
The `CyberArk-SdkCredentialProvider` PAM Provider Type uses an installed Credential Provider to communicate over a proprietary protocol to a CyberArk instance. The specifics of this communication, such as the port used, the Provider name, and the authenticated CyberArk user, are specified during the installation of the Credential Provider and are not managed by the Keyfactor PAM Provider plugin.
Requests are sent using the specified Application ID through the Credential Provider. The Application Id, configured Provider name, and the user authenticated in the Credential Provider all need to have the correct permissions set to access Secrets directly in a CyberArk Vault.

_About `Options`:_
Additional options can be set in the `manifest.json` file in the `Options` section. The available options are already included in the sample `SDK-manifest.json`.
- `UsingFrameworkSdk`: this option tells the SdkCredentialProvider to load the `NetPasswordSDK.dll` instead of the `NetStandardPasswordSDK.dll`.
- `SdkPath`: if the SDK DLL is not in the same directory as the PAM Provider, this option can be used to specify the path to the SDK DLL. It should point to the directory where the SDK resides, and not directly to the DLL file.

__Important__: When running the SDK Credential Provider on Keyfactor Command versions prior to version 11, the `NetPasswordSDK.dll` needs to be copied instead of `NetStandardPasswordSDK.dll`. This library is compatible with .NET Framework which is necessary to work in Keyfactor Command prior to version 11.
If `NetPasswordSDK.dll` is used instead of the `NetStandardPasswordSDK.dll`, the `UsingFrameworkSdk` option should be set to `true`.
