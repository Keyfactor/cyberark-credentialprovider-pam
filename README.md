<h1 align="center" style="border-bottom: none">
    CyberArk PAM Provider
</h1>

<p align="center">
  <!-- Badges -->
<img src="https://img.shields.io/badge/integration_status-production-3D1973?style=flat-square" alt="Integration Status: production" />
<a href="https://github.com/Keyfactor/cyberark-credentialprovider-pam/releases"><img src="https://img.shields.io/github/v/release/Keyfactor/cyberark-credentialprovider-pam?style=flat-square" alt="Release" /></a>
<img src="https://img.shields.io/github/issues/Keyfactor/cyberark-credentialprovider-pam?style=flat-square" alt="Issues" />
<img src="https://img.shields.io/github/downloads/Keyfactor/cyberark-credentialprovider-pam/total?style=flat-square&label=downloads&color=28B905" alt="GitHub Downloads (all assets, all releases)" />
</p>

<p align="center">
  <!-- TOC -->
  <a href="#support">
    <b>Support</b>
  </a> 
  ·
  <a href="#getting-started">
    <b>Installation</b>
  </a>
  ·
  <a href="#license">
    <b>License</b>
  </a>
  ·
  <a href="https://github.com/orgs/Keyfactor/repositories?q=pam">
    <b>Related Integrations</b>
  </a>
</p>

## Overview
The CyberArk PAM Provider integration in Keyfactor allows for the retrieval of credentials from a CyberArk Vault. Two different methods are supported for communicating securely with a CyberArk platform:
- **Central Credential Provider** - This method communicates with a CyberArk instance over HTTPS to retrieve credentials. This method does not require a local Credential Provider to be installed.
- **SDK-based local Credential Provider** - This method communicates with a CyberArk instance using CyberArk's installed Credential Provider. This method requires a local Credential Provider to be installed.

Each method has its own configuration and installation requirements. Both are able to operate running on either Keyfactor Command or a Universal Orchestrator.
Please refer to the below sections for the different considerations required for using each method.

#### Compatibility
This release was tested against CyberArk version 12.6.
Using this on a Universal Orchestrator requires UO version 10.1 or greater.

## Support
The CyberArk PAM Provider is supported by Keyfactor for Keyfactor customers. If you have a support issue, please open a support ticket with your Keyfactor representative. If you have a support issue, please open a support ticket via the Keyfactor Support Portal at https://support.keyfactor.com. 

> To report a problem or suggest a new feature, use the **[Issues](../../issues)** tab. If you want to contribute actual bug fixes or proposed enhancements, use the **[Pull requests](../../pulls)** tab.

## Getting Started

The CyberArk PAM Provider is used by Command to resolve PAM-eligible credentials for Universal Orchestrator extensions and for accessing Certificate Authorities. When configured, Command will use the CyberArk PAM Provider to retrieve credentials needed to communicate with the target system. There are two ways to install the CyberArk PAM Provider, and you may elect to use one or both methods:

1. **Locally on the Keyfactor Command server**: PAM credential resolution via the CyberArk PAM Provider will occur on the Keyfactor Command server each time an elegible credential is needed.
2. **Remotely On Universal Orchestrators**: When Jobs are dispatched to Universal Orchestrators, the associated Certificate Store extension assembly will use the CyberArk PAM Provider to resolve eligible PAM credentials.

Before proceeding with installation, you should consider which pattern is best for your requirements and use case.

### Installation

To install CyberArk PAM Provider, you must install [kfutil](https://github.com/Keyfactor/kfutil). Kfutil is a command-line tool that simplifies the process of creating PAM Types in Keyfactor Command, among many other useful automation features.

The CyberArk PAM Provider implements 2 PAM Types. Depending on your use case, you may elect to install one, or all of these PAM Types. An overview for each type is linked below:
* [CyberArk-CentralCredentialProvider](docs/cyberark-centralcredentialprovider.md)
* [CyberArk-SdkCredentialProvider](docs/cyberark-sdkcredentialprovider.md)






<details><summary>CyberArk-CentralCredentialProvider</summary>


#### Prerequisites

1. Follow the [requirements section](docs/cyberark-centralcredentialprovider.md#requirements) to configure a Service Account, grant necessary API permissions, and create secrets.

    <details><summary>Requirements</summary>
    In order for the Central Credential Provider to work, the Safe / Secret being accessed need to be available to the Provider that the Cyber Ark server is using, and the Application ID needs to be usable from an external requestor.This may require adding IP address or other rules.

    Certificate Authentication is not currently supported and needs to be disabled. This may necessitate creating a Site that allows HTTPS requests but does not require a Client Certificate to authenticate. By default the site `AIMWebService` may require a Client Certificate, which would need to be edited or have another site created.

    </details>

2. Use kfutil to create the required PAM Types in the connected Command platform.

    ```shell
    # CyberArk-CentralCredentialProvider
    kfutil pam types-create -r cyberark-credentialprovider-pam -n CyberArk-CentralCredentialProvider
    ```

#### Install on Keyfactor Command (Local)



1. On the server that hosts Keyfactor Command, download and unzip the latest release of the CyberArk PAM Provider from the [Releases](../../releases) page.

2. Copy the assemblies to the appropriate directories on the Keyfactor Command server:

    <details><summary>Keyfactor Command 11+</summary>

    1. Copy the unzipped assemblies to each of the following directories:

        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\Extensions\PamProviders\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\Extensions\PamProviders\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\Extensions\PamProviders`

    </details>

    <details><summary>Keyfactor Command 10</summary>

    1. Copy the assemblies to each of the following directories:
    
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\bin\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\bin\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\bin\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\Service\cyberark-credentialprovider-pam`

    2. Open a text editor on the Keyfactor Command server as an administrator and open the `web.config` file located in the `WebAgentServices` directory.

    3. In the `web.config` file, locate the `<container> </container>` section and add the following registration:

        ```xml
        <container>
            ...
            <!--The following are PAM Provider registrations. Uncomment them to use them in the Keyfactor Product:-->
            
            <!--Add the following line exactly to register the PAM Provider-->
            <register type="IPAMProvider" mapTo="Keyfactor.Extensions.Pam.CyberArk.CentralCredentialProviderPAM, Keyfactor.Command.PAMProviders" name="CyberArk-CentralCredentialProvider" />
        </container>
        ```

    4. Repeat steps 2 and 3 for each of the directories listed in step 1. The configuration files are located in the following paths by default:

        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\web.config`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\web.config`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\web.config`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\Service\CMSTimerService.exe.config`

    </details>

3. Restart the Keyfactor Command services (`iisreset`).




#### Install on a Universal Orchestrator (Remote)


1. Install the CyberArk PAM Provider assemblies.

    * **Using kfutil**: On the server that that hosts the Universal Orchestrator, run the following command:

        ```shell
        # Windows Server
        kfutil orchestrator extension -e cyberark-credentialprovider-pam@latest --out "C:\Program Files\Keyfactor\Keyfactor Orchestrator\extensions"

        # Linux
        kfutil orchestrator extension -e cyberark-credentialprovider-pam@latest --out "/opt/keyfactor/orchestrator/extensions"
        ```

    * **Manually**: Download the latest release of the CyberArk PAM Provider from the [Releases](../../releases) page. Extract the contents of the archive to:

        * **Windows Server**: `C:\Program Files\Keyfactor\Keyfactor Orchestrator\extensions\cyberark-credentialprovider-pam`
        * **Linux**: `/opt/keyfactor/orchestrator/extensions/cyberark-credentialprovider-pam`

2. Included in the release is a `manifest.json` file that contains the following object:

    ```json
    // cyberark-credentialprovider-pam/manifest.json

    {
        "Keyfactor:PAMProviders:CyberArk-CentralCredentialProvider:InitializationInfo": {
            "AppId": "myappid",
            "Host": "my.cyberark.instance:99999",
            "Site": "WithOutCert"
        }
    }

    ```

    Populate the fields in this object with credentials and configuration data collected in the [requirements](docs/cyberark-centralcredentialprovider.md#requirements) section.

3. Restart the Universal Orchestrator service.





</details>







<details><summary>CyberArk-SdkCredentialProvider</summary>


#### Prerequisites

1. Follow the [requirements section](docs/cyberark-sdkcredentialprovider.md#requirements) to configure a Service Account, grant necessary API permissions, and create secrets.

    <details><summary>Requirements</summary>
    To use a local Credential Provider instead, the Credential Provider will need to be installed on the machine that is using the PAM Provider. After installing the Credential Provider, copy the `NetStandardPasswordSDK.dll` assembly from the install location into the PAM Provider install location. This dll __needs__ to be adjacent to `cyberark-credentialprovider-pam.dll` to be properly loaded.
    __Important__: When running the SDK Credential Provider on Keyfactor Command versions prior to version 11, the `NetPasswordSDK.dll` needs to be copied instead of `NetStandardPasswordSDK.dll`. This library is compatible with .NET Framework which is necessary to work in Keyfactor Command.

    After registering the Credential Provider during install, make sure the Provider for the machine has been granted permission to access the Safe, as well as the Application ID that will be used.

    The default <code>manifest.json</code> needs to be replaced with the included <code>SDK-manifest.json</code>. Rename the existing <code>manifest.json</code> as <code>Central-manifest.json</code> and then rename the <code>SDK-manifest.json</code> to replace the original <code>manifest.json</code>.

    </details>

2. Use kfutil to create the required PAM Types in the connected Command platform.

    ```shell
    # CyberArk-SdkCredentialProvider
    kfutil pam types-create -r cyberark-credentialprovider-pam -n CyberArk-SdkCredentialProvider
    ```

#### Install on Keyfactor Command (Local)



1. On the server that hosts Keyfactor Command, download and unzip the latest release of the CyberArk PAM Provider from the [Releases](../../releases) page.

2. Copy the assemblies to the appropriate directories on the Keyfactor Command server:

    <details><summary>Keyfactor Command 11+</summary>

    1. Copy the unzipped assemblies to each of the following directories:

        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\Extensions\PamProviders\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\Extensions\PamProviders\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\Extensions\PamProviders`

    </details>

    <details><summary>Keyfactor Command 10</summary>

    1. Copy the assemblies to each of the following directories:
    
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\bin\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\bin\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\bin\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\Service\cyberark-credentialprovider-pam`

    2. Open a text editor on the Keyfactor Command server as an administrator and open the `web.config` file located in the `WebAgentServices` directory.

    3. In the `web.config` file, locate the `<container> </container>` section and add the following registration:

        ```xml
        <container>
            ...
            <!--The following are PAM Provider registrations. Uncomment them to use them in the Keyfactor Product:-->
            
            <!--Add the following line exactly to register the PAM Provider-->
            <register type="IPAMProvider" mapTo="Keyfactor.Extensions.Pam.CyberArk.CentralCredentialProviderPAM, Keyfactor.Command.PAMProviders" name="CyberArk-SdkCredentialProvider" />
        </container>
        ```

    4. Repeat steps 2 and 3 for each of the directories listed in step 1. The configuration files are located in the following paths by default:

        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\web.config`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\web.config`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\web.config`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\Service\CMSTimerService.exe.config`

    </details>

3. Restart the Keyfactor Command services (`iisreset`).




#### Install on a Universal Orchestrator (Remote)


1. Install the CyberArk PAM Provider assemblies.

    * **Using kfutil**: On the server that that hosts the Universal Orchestrator, run the following command:

        ```shell
        # Windows Server
        kfutil orchestrator extension -e cyberark-credentialprovider-pam@latest --out "C:\Program Files\Keyfactor\Keyfactor Orchestrator\extensions"

        # Linux
        kfutil orchestrator extension -e cyberark-credentialprovider-pam@latest --out "/opt/keyfactor/orchestrator/extensions"
        ```

    * **Manually**: Download the latest release of the CyberArk PAM Provider from the [Releases](../../releases) page. Extract the contents of the archive to:

        * **Windows Server**: `C:\Program Files\Keyfactor\Keyfactor Orchestrator\extensions\cyberark-credentialprovider-pam`
        * **Linux**: `/opt/keyfactor/orchestrator/extensions/cyberark-credentialprovider-pam`

2. Included in the release is a `manifest.json` file that contains the following object:

    ```json
    // cyberark-credentialprovider-pam/manifest.json

    {
        "Keyfactor:PAMProviders:CyberArk-CentralCredentialProvider:InitializationInfo": {
            "AppId": "myappid",
            "Host": "my.cyberark.instance:99999",
            "Site": "WithOutCert"
        }
    }

    ```

    Populate the fields in this object with credentials and configuration data collected in the [requirements](docs/cyberark-sdkcredentialprovider.md#requirements) section.

3. Restart the Universal Orchestrator service.





</details>





### Usage





<details><summary>CyberArk-CentralCredentialProvider</summary>


#### Keyfactor Command (Local)



##### Define a PAM provider in Command
1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider. Click the dropdown for **Provider Type** and select **CyberArk-CentralCredentialProvider**.

    > If you're running Keyfactor Command 11+, make sure "Remote Provider" is unchecked.

3. Populate the fields with the necessary information collected in the [requirements](docs/cyberark-centralcredentialprovider.md#requirements) section:

| Initialization parameter | Display Name | Description |
| --- | --- | --- |
| AppId | Application ID | The Application ID with access set up for the Safe used to identify and authenticate requests. |
| Host | CyberArk Host and Port | The hostname (IP address or domain name) and (optionally) port. It should take the format: my.cyberark.instance:404 (note: no https:// included) |
| Site | CyberArk API Site | By default, AIMWebService is the site name, but may be deployed to another site name. |


4. Click **Save**. The PAM provider is now available for use in Keyfactor Command.

##### Using the PAM provider

Now, when defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-CentralCredentialProvider** will be available as a PAM provider option. When defining new Certificate Stores, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**. 

Select the **Load From PAM Provider** tab, choose the **CyberArk-CentralCredentialProvider** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| Safe | Safe | The name of the Safe the credential resides in. |
| Folder | Folder | The folder path the credential lives in. If it is nested, use the forward slash e.g. Root\Folder |
| Object | Object | The name of the password object that has the credential. |





#### Universal Orchestrator (Remote)



<details><summary>Keyfactor Command 11+</summary>

##### Define a remote PAM provider in Command

In Command 11 and greater, before using the CyberArk-CentralCredentialProvider PAM type, you must define a Remote PAM Provider in the Command portal.

1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider.

3. Make sure that "Remote Provider" is checked.

4. Click the dropdown for **Provider Type** and select **CyberArk-CentralCredentialProvider**. 

5. Give the provider a unique name.

6. Click "Save".

##### Using the PAM provider

When defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-CentralCredentialProvider** can be used as a PAM provider. When defining a new Certificate Store, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**.

Select the **Load From PAM Provider** tab, choose the **CyberArk-CentralCredentialProvider** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| Safe | Safe | The name of the Safe the credential resides in. |
| Folder | Folder | The folder path the credential lives in. If it is nested, use the forward slash e.g. Root\Folder |
| Object | Object | The name of the password object that has the credential. |


</details>

<details><summary>Keyfactor Command 10</summary>

When defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-CentralCredentialProvider** can be used as a PAM provider.

When entering Secret fields, select the **Load From Keyfactor Secrets** tab, and populate the **Secret Value** field with the following JSON object:

```json
{"Safe": "The name of the Safe the credential resides in.","Folder": "The folder path the credential lives in. If it is nested, use the forward slash e.g. Root\Folder","Object": "The name of the password object that has the credential."}

```

> We recommend creating this JSON object in a text editor, and copying it into the Secret Value field.

</details>





</details>


> Additional information on CyberArk-CentralCredentialProvider can be found in the [supplimental documentation](docs/cyberark-centralcredentialprovider.md).





<details><summary>CyberArk-SdkCredentialProvider</summary>


#### Keyfactor Command (Local)



##### Define a PAM provider in Command
1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider. Click the dropdown for **Provider Type** and select **CyberArk-SdkCredentialProvider**.

    > If you're running Keyfactor Command 11+, make sure "Remote Provider" is unchecked.

3. Populate the fields with the necessary information collected in the [requirements](docs/cyberark-sdkcredentialprovider.md#requirements) section:

| Initialization parameter | Display Name | Description |
| --- | --- | --- |
| AppId | Application ID | The Application ID with access set up for the Safe used to identify and authenticate requests. |


4. Click **Save**. The PAM provider is now available for use in Keyfactor Command.

##### Using the PAM provider

Now, when defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-SdkCredentialProvider** will be available as a PAM provider option. When defining new Certificate Stores, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**. 

Select the **Load From PAM Provider** tab, choose the **CyberArk-SdkCredentialProvider** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| Safe | Safe | The name of the Safe the credential resides in. |
| Folder | Folder | The folder path the credential lives in. If it is nested, use the forward slash e.g. Root\Folder |
| Object | Object | The name of the password object that has the credential. |





#### Universal Orchestrator (Remote)



<details><summary>Keyfactor Command 11+</summary>

##### Define a remote PAM provider in Command

In Command 11 and greater, before using the CyberArk-SdkCredentialProvider PAM type, you must define a Remote PAM Provider in the Command portal.

1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider.

3. Make sure that "Remote Provider" is checked.

4. Click the dropdown for **Provider Type** and select **CyberArk-SdkCredentialProvider**. 

5. Give the provider a unique name.

6. Click "Save".

##### Using the PAM provider

When defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-SdkCredentialProvider** can be used as a PAM provider. When defining a new Certificate Store, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**.

Select the **Load From PAM Provider** tab, choose the **CyberArk-SdkCredentialProvider** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| Safe | Safe | The name of the Safe the credential resides in. |
| Folder | Folder | The folder path the credential lives in. If it is nested, use the forward slash e.g. Root\Folder |
| Object | Object | The name of the password object that has the credential. |


</details>

<details><summary>Keyfactor Command 10</summary>

When defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-SdkCredentialProvider** can be used as a PAM provider.

When entering Secret fields, select the **Load From Keyfactor Secrets** tab, and populate the **Secret Value** field with the following JSON object:

```json
{"Safe": "The name of the Safe the credential resides in.","Folder": "The folder path the credential lives in. If it is nested, use the forward slash e.g. Root\Folder","Object": "The name of the password object that has the credential."}

```

> We recommend creating this JSON object in a text editor, and copying it into the Secret Value field.

</details>





</details>


> Additional information on CyberArk-SdkCredentialProvider can be found in the [supplimental documentation](docs/cyberark-sdkcredentialprovider.md).



## License

Apache License 2.0, see [LICENSE](LICENSE)

## Related Integrations

See all [Keyfactor PAM Provider extensions](https://github.com/orgs/Keyfactor/repositories?q=pam).