## CyberArk-SdkCredentialProvider

The Cyber Ark SDK Credential Provider uses the Cyber Ark SDK in order to communicate with a locally installed Credential Provider.
When the Credential Provider is installed locally, authentication needs to be configured correctly for the provider to communicate with a Cyber Ark instance over their proprietary protocol.

## Requirements

To use a local Credential Provider instead, the Credential Provider will need to be installed on the machine that is using the PAM Provider. After installing the Credential Provider, copy the `NetStandardPasswordSDK.dll` assembly from the install location into the PAM Provider install location. This dll __needs__ to be adjacent to `cyberark-credentialprovider-pam.dll` to be properly loaded.
__Important__: When running the SDK Credential Provider on Keyfactor Command versions prior to version 11, the `NetPasswordSDK.dll` needs to be copied instead of `NetStandardPasswordSDK.dll`. This library is compatible with .NET Framework which is necessary to work in Keyfactor Command.

After registering the Credential Provider during install, make sure the Provider for the machine has been granted permission to access the Safe, as well as the Application ID that will be used.

The default <code>manifest.json</code> needs to be replaced with the included <code>SDK-manifest.json</code>. Rename the existing <code>manifest.json</code> as <code>Central-manifest.json</code> and then rename the <code>SDK-manifest.json</code> to replace the original <code>manifest.json</code>.



