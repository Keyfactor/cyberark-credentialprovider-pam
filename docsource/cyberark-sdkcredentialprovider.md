## Overview
The Cyber Ark SDK Credential Provider uses the Cyber Ark SDK in order to communicate with a locally installed Credential Provider.
When the Credential Provider is installed locally, authentication needs to be configured correctly for the provider to communicate with a Cyber Ark instance over their proprietary protocol.

## Requirements
To use a local Credential Provider instead, the Credential Provider will need to be installed on the machine that is using the PAM Provider. After installing the Credential Provider, copy the `NetStandardPasswordSDK.dll` assembly from the install location into the PAM Provider install location. This dll should be adjacent to `cyberark-credentialprovider-pam.dll` to be properly loaded.

After registering the Credential Provider during install, make sure the Provider for the machine has been granted permission to access the Safe, as well as the Application ID that will be used.

The default <code>manifest.json</code> needs to be replaced with the included <code>SDK-manifest.json</code>. Rename the existing <code>manifest.json</code> as <code>Central-manifest.json</code> and then rename the <code>SDK-manifest.json</code> to replace the original <code>manifest.json</code>.

## Mechanics
The `CyberArk-SdkCredentialProvider` PAM Provider Type uses an installed Credential Provider to communicate over a proprietary protocol to a Cyber Ark instance. The specifics of this communication, such as the port used, the Provider name, and the authenticated Cyber Ark user, are specified during the installation of the Credential Provider and are not managed by the Keyfactor PAM Provider plugin.
Requests are sent using the specified Application ID through the Credential Provider. The Application Id, configured Provider name, and the user authenticated in the Credential Provider all need to have the correct permissions set to access Secrets directly in a Cyber Ark Vault.

_About `Options`:_
Additional options can be set in the `manifest.json` file in the `Options` section. The available options are already included in the sample `SDK-manifest.json`.
- `UsingFrameworkSdk`: this option tells the SdkCredentialProvider to load the `NetPasswordSDK.dll` instead of the `NetStandardPasswordSDK.dll`.
- `SdkPath`: if the SDK DLL is not in the same directory as the PAM Provider, this option can be used to specify the path to the SDK DLL. It should point to the directory where the SDK resides, and not directly to the DLL file.

__Important__: When running the SDK Credential Provider on Keyfactor Command versions prior to version 11, the `NetPasswordSDK.dll` needs to be copied instead of `NetStandardPasswordSDK.dll`. This library is compatible with .NET Framework which is necessary to work in Keyfactor Command prior to version 11.
If `NetPasswordSDK.dll` is used instead of the `NetStandardPasswordSDK.dll`, the `UsingFrameworkSdk` option should be set to `true`.
