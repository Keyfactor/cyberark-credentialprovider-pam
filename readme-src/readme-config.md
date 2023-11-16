### Configuring for PAM Usage
#### For CyberArk Central Credential Provider
In order for the Central Credential Provider to work, the Safe / Secret being accessed need to be available to the Provider that the CyberArk server is using, and the Application ID needs to be usable from an external requestor.This may require adding IP address or other rules.

Certificate Authentication cannot be required. This may necessitate creating a Site that allows HTTPS requests but does not require a Client Certificate to authenticate. By default the site `AIMWebService` may require a Client Certificate, which would need to be edited or have another site created.

#### For SDK-based local Credential Provider
To use a local Credential Provider instead, the Credential Provider will need to be installed on the machine that is using the PAM Provider. After installing the Credential Provider, copy the `NetStandardPasswordSDK.dll` assembly from the install location into the PAM Provider install location. This dll __needs__ to be adjacent to `cyberark-credentialprovider-pam.dll` to be properly loaded.

After registering the Credential Provider during install, make sure the Provider for the machine has been granted permission to access the Safe, as well as the Application ID that will be used.

#### On Keyfactor Universal Orchestrator
<details>
<summary>Installation - CyberArk Central Credential Provider </summary>
<p>
Install the CyberArk Central Credential Provider as an extension by copying the release contents into a new extension folder named <code>CyberArk-CentralCredentialProvider</code>.
A <code>manifest.json</code> file is included in the release. This file needs to be edited to enter in the "initialization" parameters for the PAM Provider. Specifically values need to be entered for the parameters in the <code>manifest.json</code> of the <b>PAM Provider extension</b>:

~~~ json
"Keyfactor:PAMProviders:CyberArk-CentralCredentialProvider:InitializationInfo": {
    "AppId": "myappid",
    "Host": "https://my.cyberark.instance:99999",
    "Site": "WithOutCert"
  }
~~~
</p>
</details>

<details>
<summary>Installation - SDK-based local Credential Provider</summary>
<p>
Install the CyberArk SDK-based local Credential Provider as an extension by copying the release contents into a new extension folder named <code>CyberArk-SdkCredentialProvider</code>. The <code>NetStandardPasswordSDK.dll</code> assembly will still need to be copied over to the installation location as well.
The default <code>manifest.json</code> needs to be replaced with the included <code>SDK-manifest.json</code>. Rename the existing <code>manifest.json</code> as <code>Central-manifest.json</code> and then rename the <code>SDK-manifest.json</code> to replace the original <code>manifest.json</code>.
This file then needs to be edited to enter in the "initialization" parameters for the PAM Provider. Specifically values need to be entered for the parameters in the <code>manifest.json</code> of the <b>PAM Provider extension</b>:

~~~json
"Keyfactor:PAMProviders:CyberArk-SdkCredentialProvider:InitializationInfo": {
    "AppId": "myappid"
  }
~~~
</p>
</details>

#### Usage with the Keyfactor Universal Orchestrator
To use the PAM Provider to resolve a field, for example a Server Password, instead of entering in the actual value for the Server Password, enter a `json` object with the parameters specifying the field.
The parameters needed are the "instance" parameters above (with appropriate characters escaped for correct JSON formatting):

~~~ json
{"Safe":"MySafe","Folder":"Root\\Secrets","Object":"MySecret"}
~~~

If a field supports PAM but should not use PAM, simply enter in the actual value to be used instead of the `json` format object above.
