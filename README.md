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

> [!IMPORTANT]
> For the most up-to-date and complete documentation on how to install a PAM provider extension, please visit our [product documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/ReferenceGuide/Preparing%20Third%20Party%20PAM%20Providers%20to%20Work%20with.htm?Highlight=pam%20provider#InstallingCustomPAMProviderExtensions)


To install CyberArk PAM Provider, it is recommended you install [kfutil](https://github.com/Keyfactor/kfutil). `kfutil` is a command-line tool that simplifies the process of creating PAM Types in Keyfactor Command.

The CyberArk PAM Provider implements 3 PAM Types. Depending on your use case, you may elect to install one, or all of these PAM Types. An overview for each type is linked below:
* [CyberArk-CentralCredentialProvider](docs/cyberark-centralcredentialprovider.md)
* [CyberArk-ClientAuth-CentralCredentialProvider](docs/cyberark-clientauth-centralcredentialprovider.md)
* [CyberArk-SdkCredentialProvider](docs/cyberark-sdkcredentialprovider.md)






<details><summary>CyberArk-CentralCredentialProvider</summary>


#### Requirements
   In order for the Central Credential Provider to work, the Safe / Secret being accessed need to be available to the Provider that the Cyber Ark server is using, and the Application ID needs to be usable from an external requestor. This may require adding IP address or other rules.

   Certificate Authentication is not currently supported and needs to be disabled. This may necessitate creating a Site that allows HTTPS requests but does not require a Client Certificate to authenticate. By default the site `AIMWebService` may require a Client Certificate, which would need to be edited or have another site created.

#### Create PAM type in Keyfactor Command


##### Using `kfutil`
Create the required PAM Types in the connected Command platform.

```shell
# CyberArk-CentralCredentialProvider
kfutil pam types-create -r cyberark-credentialprovider-pam -n CyberArk-CentralCredentialProvider
```

##### Using the API
For full API docs please visit our [product documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/WebAPI/KeyfactorAPI/PAMProvidersPOSTTypes.htm?Highlight=pam%20type)

Below is the payload to `POST` to the Keyfactor Command API
```json
{
    "Name": "CyberArk-CentralCredentialProvider",
    "Parameters": [
        {
            "Name": "AppId",
            "DisplayName": "Application ID",
            "DataType": 1,
            "InstanceLevel": false,
            "Description": "The Application ID with access set up for the Safe used to identify and authenticate requests."
        },
        {
            "Name": "Host",
            "DisplayName": "CyberArk Host and Port",
            "DataType": 1,
            "InstanceLevel": false,
            "Description": "The hostname (IP address or domain name) and (optionally) port. It should take the format: my.cyberark.instance:404 (note: no https:// included)."
        },
        {
            "Name": "Site",
            "DisplayName": "CyberArk API Site",
            "DataType": 1,
            "InstanceLevel": false,
            "Description": "By default, AIMWebService is the site name, but may be deployed to another site name."
        },
        {
            "Name": "Safe",
            "DisplayName": "Safe",
            "DataType": 1,
            "InstanceLevel": true,
            "Description": "The name of the Safe the credential resides in."
        },
        {
            "Name": "Folder",
            "DisplayName": "Folder",
            "DataType": 1,
            "InstanceLevel": true,
            "Description": "The folder path the credential lives in. If it is nested, use the backwards slash e.g. Root\\Folder"
        },
        {
            "Name": "Object",
            "DisplayName": "Object",
            "DataType": 1,
            "InstanceLevel": true,
            "Description": "The name of the password object that has the credential."
        }
    ]
}
```

#### Install PAM provider on Keyfactor Command Host (Local)



1. On the server that hosts Keyfactor Command, download and unzip the latest release of the CyberArk PAM Provider from the [Releases](../../releases) page.

2. Copy the assemblies to the appropriate directories on the Keyfactor Command server:

    <details><summary>Keyfactor Command 11+</summary>

    1. Copy the unzipped assemblies to each of the following directories:

        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\Extensions\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\Extensions\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\Extensions\cyberark-credentialprovider-pam`

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




#### Install PAM provider on a Universal Orchestrator Host (Remote)


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







<details><summary>CyberArk-ClientAuth-CentralCredentialProvider</summary>


#### Requirements
   In order for the Client-Auth Central Credential Provider to work, the Safe / Secret being accessed need to be available to the Provider that the Cyber Ark server is using, and the Application ID needs to be usable from an external requestor. This may require adding IP address or other rules.

   In order for the integration to take advantage of Client Certificate auth, please ensure that HTTPS is enabled and configured to require a Client Certificate. By default the site `AIMWebService` may be configured to require a Client Certificate.

#### Create PAM type in Keyfactor Command


##### Using `kfutil`
Create the required PAM Types in the connected Command platform.

```shell
# CyberArk-ClientAuth-CentralCredentialProvider
kfutil pam types-create -r cyberark-credentialprovider-pam -n CyberArk-ClientAuth-CentralCredentialProvider
```

##### Using the API
For full API docs please visit our [product documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/WebAPI/KeyfactorAPI/PAMProvidersPOSTTypes.htm?Highlight=pam%20type)

Below is the payload to `POST` to the Keyfactor Command API
```json
{
    "Name": "CyberArk-ClientAuth-CentralCredentialProvider",
    "Parameters": [
        {
            "Name": "AppId",
            "DisplayName": "Application ID",
            "DataType": 1,
            "InstanceLevel": false,
            "Description": "The Application ID with access set up for the Safe used to identify and authenticate requests."
        },
        {
            "Name": "Host",
            "DisplayName": "CyberArk Host and Port",
            "DataType": 1,
            "InstanceLevel": false,
            "Description": "The hostname (IP address or domain name) and (optionally) port. It should take the format: my.cyberark.instance:404 (note: no https:// included)."
        },
        {
            "Name": "Site",
            "DisplayName": "CyberArk API Site",
            "DataType": 1,
            "InstanceLevel": false,
            "Description": "By default, AIMWebService is the site name, but may be deployed to another site name."
        },
        {
            "Name": "PfxBase64",
            "DisplayName": "PFX Base64",
            "DataType": 1,
            "InstanceLevel": false,
            "Description": "The Base64-encoded PFX certificate used for authentication."
        },
        {
            "Name": "PfxPassword",
            "DisplayName": "PFX Password",
            "DataType": 2,
            "InstanceLevel": false,
            "Description": "The password for the PFX certificate used for authentication."
        },
        {
            "Name": "Safe",
            "DisplayName": "Safe",
            "DataType": 1,
            "InstanceLevel": true,
            "Description": "The name of the Safe the credential resides in."
        },
        {
            "Name": "Folder",
            "DisplayName": "Folder",
            "DataType": 1,
            "InstanceLevel": true,
            "Description": "The folder path the credential lives in. If it is nested, use the backwards slash e.g. Root\\Folder"
        },
        {
            "Name": "Object",
            "DisplayName": "Object",
            "DataType": 1,
            "InstanceLevel": true,
            "Description": "The name of the password object that has the credential."
        }
    ]
}
```

#### Install PAM provider on Keyfactor Command Host (Local)



1. On the server that hosts Keyfactor Command, download and unzip the latest release of the CyberArk PAM Provider from the [Releases](../../releases) page.

2. Copy the assemblies to the appropriate directories on the Keyfactor Command server:

    <details><summary>Keyfactor Command 11+</summary>

    1. Copy the unzipped assemblies to each of the following directories:

        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\Extensions\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\Extensions\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\Extensions\cyberark-credentialprovider-pam`

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
            <register type="IPAMProvider" mapTo="Keyfactor.Extensions.Pam.CyberArk.CentralCredentialProviderPAM, Keyfactor.Command.PAMProviders" name="CyberArk-ClientAuth-CentralCredentialProvider" />
        </container>
        ```

    4. Repeat steps 2 and 3 for each of the directories listed in step 1. The configuration files are located in the following paths by default:

        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\web.config`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\web.config`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\web.config`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\Service\CMSTimerService.exe.config`

    </details>

3. Restart the Keyfactor Command services (`iisreset`).




#### Install PAM provider on a Universal Orchestrator Host (Remote)


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

    {
        "Keyfactor:PAMProviders:CyberArk-CentralCredentialProvider:InitializationInfo": {
            "AppId": "myappid",
            "Host": "my.cyberark.instance:99999",
            "Site": "WithOutCert"
        }
    }

    ```

    Populate the fields in this object with credentials and configuration data collected in the [requirements](docs/cyberark-clientauth-centralcredentialprovider.md#requirements) section.

3. Restart the Universal Orchestrator service.





</details>







<details><summary>CyberArk-SdkCredentialProvider</summary>


#### Requirements
   After installing the Credential Provider, copy the `NetStandardPasswordSDK.dll` assembly from the install location into the PAM Provider install location. This dll should be stored in the same directory as `cyberark-credentialprovider-pam.dll` to be properly loaded. The name `NetStandardPasswordSDK.dll` is case-sensitive, so make sure the name is copied to the directory as `NetStandardPasswordSDK.dll`.

   After registering the Credential Provider during install, make sure the Provider for the machine has been granted permission to access the Safe, as well as the Application ID that will be used.

   To read secrets stored in a CyberArk Vault safe, the Partner must have at least the following permissions on the safe:
   - Monitor Safe
   - Retrieve files from Safe

   The default <code>manifest.json</code> needs to be replaced with the included <code>SDK-manifest.json</code>. Rename the existing <code>manifest.json</code> as <code>Central-manifest.json</code> and then rename the <code>SDK-manifest.json</code> to replace the original <code>manifest.json</code>.

   ### IMPORTANT NOTE FOR LINUX INSTALLATIONS

   If you have installed the Credential Provider on a Linux instance and run this PAM extension within a [Universal Orchestrator as a Linux service](https://software.keyfactor.com/Core-OnPrem/Current/Content/InstallingAgents/NetCoreOrchestrator/InstalltheOrchestratorLinux.htm), you may run into issues communicating with the Credential Provider service and receive an `ENCPR019E` error code when the PAM instance tries to retrieve a credential. 

   By default, the Universal Orchestrator Linux service has [PrivateTmp](https://www.redhat.com/en/blog/new-red-hat-enterprise-linux-7-security-feature-privatetmp) enabled, which creates a separate, isolated `/tmp` directory for the service. The CyberArk Credential Provider creates a named pipe in the `/tmp` directory to communicate with other applications, so when PrivateTmp is enabled, the PAM instance cannot access this pipe to communicate with the Credential Provider. To resolve this issue, you can disable PrivateTmp for the Universal Orchestrator Linux service by creating an override file with the command `sudo systemctl edit keyfactor-orchestrator-default.service` and adding the following lines:

   ```
   [Service]
   PrivateTmp=false
   ```

   and then run the following commands to restart the service deemon and the Universal Orchestrator service:

   ```bash
   sudo systemctl daemon-reload
   sudo systemctl restart keyfactor-orchestrator-default.service
   ```

#### Create PAM type in Keyfactor Command


##### Using `kfutil`
Create the required PAM Types in the connected Command platform.

```shell
# CyberArk-SdkCredentialProvider
kfutil pam types-create -r cyberark-credentialprovider-pam -n CyberArk-SdkCredentialProvider
```

##### Using the API
For full API docs please visit our [product documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/WebAPI/KeyfactorAPI/PAMProvidersPOSTTypes.htm?Highlight=pam%20type)

Below is the payload to `POST` to the Keyfactor Command API
```json
{
    "Name": "CyberArk-SdkCredentialProvider",
    "Parameters": [
        {
            "Name": "AppId",
            "DisplayName": "Application ID",
            "DataType": 1,
            "InstanceLevel": false,
            "Description": "The Application ID with access set up for the Safe used to identify and authenticate requests."
        },
        {
            "Name": "Safe",
            "DisplayName": "Safe",
            "DataType": 1,
            "InstanceLevel": true,
            "Description": "The name of the Safe the credential resides in."
        },
        {
            "Name": "Folder",
            "DisplayName": "Folder",
            "DataType": 1,
            "InstanceLevel": true,
            "Description": "The folder path the credential lives in. If it is nested, use the backwards slash e.g. Root\\Folder"
        },
        {
            "Name": "Object",
            "DisplayName": "Object",
            "DataType": 1,
            "InstanceLevel": true,
            "Description": "The name of the password object that has the credential."
        }
    ]
}
```

#### Install PAM provider on Keyfactor Command Host (Local)



1. On the server that hosts Keyfactor Command, download and unzip the latest release of the CyberArk PAM Provider from the [Releases](../../releases) page.

2. Copy the assemblies to the appropriate directories on the Keyfactor Command server:

    <details><summary>Keyfactor Command 11+</summary>

    1. Copy the unzipped assemblies to each of the following directories:

        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\Extensions\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\Extensions\cyberark-credentialprovider-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\Extensions\cyberark-credentialprovider-pam`

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




#### Install PAM provider on a Universal Orchestrator Host (Remote)


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


#### From Keyfactor Command Host (Local)



##### Define a PAM provider in Command
1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider. Click the dropdown for **Provider Type** and select **CyberArk-CentralCredentialProvider**.

> [!IMPORTANT]
> If you're running Keyfactor Command 11+, make sure `Remote Provider` is unchecked.

3. Populate the fields with the necessary information collected in the [requirements](docs/cyberark-centralcredentialprovider.md#requirements) section:

| Initialization parameter | Display Name | Description |
| --- | --- | --- |
| AppId | Application ID | The Application ID with access set up for the Safe used to identify and authenticate requests. |
| Host | CyberArk Host and Port | The hostname (IP address or domain name) and (optionally) port. It should take the format: my.cyberark.instance:404 (note: no https:// included). |
| Site | CyberArk API Site | By default, AIMWebService is the site name, but may be deployed to another site name. |


4. Click **Save**. The PAM provider is now available for use in Keyfactor Command.

##### Using the PAM provider

Now, when defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-CentralCredentialProvider** will be available as a PAM provider option. When defining new Certificate Stores, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**. 

Select the **Load From PAM Provider** tab, choose the **CyberArk-CentralCredentialProvider** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| Safe | Safe | The name of the Safe the credential resides in. |
| Folder | Folder | The folder path the credential lives in. If it is nested, use the backwards slash e.g. Root\Folder |
| Object | Object | The name of the password object that has the credential. |





#### From a Universal Orchestrator Host (Remote)



<details><summary>Keyfactor Command 11+</summary>

##### Define a remote PAM provider in Command

In Command 11 and greater, before using the CyberArk-CentralCredentialProvider PAM type, you must define a Remote PAM Provider in the Command portal.

1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider.

3. Make sure that `Remote Provider` is checked.

4. Click the dropdown for **Provider Type** and select **CyberArk-CentralCredentialProvider**. 

5. Give the provider a unique name.

6. Click "Save".

##### Using the PAM provider

When defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-CentralCredentialProvider** can be used as a PAM provider. When defining a new Certificate Store, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**.

Select the **Load From PAM Provider** tab, choose the **CyberArk-CentralCredentialProvider** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| Safe | Safe | The name of the Safe the credential resides in. |
| Folder | Folder | The folder path the credential lives in. If it is nested, use the backwards slash e.g. Root\Folder |
| Object | Object | The name of the password object that has the credential. |


</details>

<details><summary>Keyfactor Command 10</summary>

When defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-CentralCredentialProvider** can be used as a PAM provider.

When entering Secret fields, select the **Load From Keyfactor Secrets** tab, and populate the **Secret Value** field with the following JSON object:

```json
{"Safe": "The name of the Safe the credential resides in.","Folder": "The folder path the credential lives in. If it is nested, use the backwards slash e.g. Root\Folder","Object": "The name of the password object that has the credential."}

```

> We recommend creating this JSON object in a text editor, and copying it into the Secret Value field.

</details>





</details>


> [!NOTE]
> Additional information on CyberArk-CentralCredentialProvider can be found in the [supplemental documentation](docs/cyberark-centralcredentialprovider.md).





<details><summary>CyberArk-ClientAuth-CentralCredentialProvider</summary>


#### From Keyfactor Command Host (Local)



##### Define a PAM provider in Command
1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider. Click the dropdown for **Provider Type** and select **CyberArk-ClientAuth-CentralCredentialProvider**.

> [!IMPORTANT]
> If you're running Keyfactor Command 11+, make sure `Remote Provider` is unchecked.

3. Populate the fields with the necessary information collected in the [requirements](docs/cyberark-clientauth-centralcredentialprovider.md#requirements) section:

| Initialization parameter | Display Name | Description |
| --- | --- | --- |
| AppId | Application ID | The Application ID with access set up for the Safe used to identify and authenticate requests. |
| Host | CyberArk Host and Port | The hostname (IP address or domain name) and (optionally) port. It should take the format: my.cyberark.instance:404 (note: no https:// included). |
| Site | CyberArk API Site | By default, AIMWebService is the site name, but may be deployed to another site name. |
| PfxBase64 | PFX Base64 | The Base64-encoded PFX certificate used for authentication. |
| PfxPassword | PFX Password | The password for the PFX certificate used for authentication. |


4. Click **Save**. The PAM provider is now available for use in Keyfactor Command.

##### Using the PAM provider

Now, when defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-ClientAuth-CentralCredentialProvider** will be available as a PAM provider option. When defining new Certificate Stores, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**. 

Select the **Load From PAM Provider** tab, choose the **CyberArk-ClientAuth-CentralCredentialProvider** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| Safe | Safe | The name of the Safe the credential resides in. |
| Folder | Folder | The folder path the credential lives in. If it is nested, use the backwards slash e.g. Root\Folder |
| Object | Object | The name of the password object that has the credential. |





#### From a Universal Orchestrator Host (Remote)



<details><summary>Keyfactor Command 11+</summary>

##### Define a remote PAM provider in Command

In Command 11 and greater, before using the CyberArk-ClientAuth-CentralCredentialProvider PAM type, you must define a Remote PAM Provider in the Command portal.

1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider.

3. Make sure that `Remote Provider` is checked.

4. Click the dropdown for **Provider Type** and select **CyberArk-ClientAuth-CentralCredentialProvider**. 

5. Give the provider a unique name.

6. Click "Save".

##### Using the PAM provider

When defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-ClientAuth-CentralCredentialProvider** can be used as a PAM provider. When defining a new Certificate Store, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**.

Select the **Load From PAM Provider** tab, choose the **CyberArk-ClientAuth-CentralCredentialProvider** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| Safe | Safe | The name of the Safe the credential resides in. |
| Folder | Folder | The folder path the credential lives in. If it is nested, use the backwards slash e.g. Root\Folder |
| Object | Object | The name of the password object that has the credential. |


</details>

<details><summary>Keyfactor Command 10</summary>

When defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-ClientAuth-CentralCredentialProvider** can be used as a PAM provider.

When entering Secret fields, select the **Load From Keyfactor Secrets** tab, and populate the **Secret Value** field with the following JSON object:

```json
{"Safe": "The name of the Safe the credential resides in.","Folder": "The folder path the credential lives in. If it is nested, use the backwards slash e.g. Root\Folder","Object": "The name of the password object that has the credential."}

```

> We recommend creating this JSON object in a text editor, and copying it into the Secret Value field.

</details>





</details>


> [!NOTE]
> Additional information on CyberArk-ClientAuth-CentralCredentialProvider can be found in the [supplemental documentation](docs/cyberark-clientauth-centralcredentialprovider.md).





<details><summary>CyberArk-SdkCredentialProvider</summary>


#### From Keyfactor Command Host (Local)



##### Define a PAM provider in Command
1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider. Click the dropdown for **Provider Type** and select **CyberArk-SdkCredentialProvider**.

> [!IMPORTANT]
> If you're running Keyfactor Command 11+, make sure `Remote Provider` is unchecked.

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
| Folder | Folder | The folder path the credential lives in. If it is nested, use the backwards slash e.g. Root\Folder |
| Object | Object | The name of the password object that has the credential. |





#### From a Universal Orchestrator Host (Remote)



<details><summary>Keyfactor Command 11+</summary>

##### Define a remote PAM provider in Command

In Command 11 and greater, before using the CyberArk-SdkCredentialProvider PAM type, you must define a Remote PAM Provider in the Command portal.

1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Priviledged Access Management**.

2. Select the **Add** button to create a new PAM provider.

3. Make sure that `Remote Provider` is checked.

4. Click the dropdown for **Provider Type** and select **CyberArk-SdkCredentialProvider**. 

5. Give the provider a unique name.

6. Click "Save".

##### Using the PAM provider

When defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-SdkCredentialProvider** can be used as a PAM provider. When defining a new Certificate Store, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**.

Select the **Load From PAM Provider** tab, choose the **CyberArk-SdkCredentialProvider** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| Safe | Safe | The name of the Safe the credential resides in. |
| Folder | Folder | The folder path the credential lives in. If it is nested, use the backwards slash e.g. Root\Folder |
| Object | Object | The name of the password object that has the credential. |


</details>

<details><summary>Keyfactor Command 10</summary>

When defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-SdkCredentialProvider** can be used as a PAM provider.

When entering Secret fields, select the **Load From Keyfactor Secrets** tab, and populate the **Secret Value** field with the following JSON object:

```json
{"Safe": "The name of the Safe the credential resides in.","Folder": "The folder path the credential lives in. If it is nested, use the backwards slash e.g. Root\Folder","Object": "The name of the password object that has the credential."}

```

> We recommend creating this JSON object in a text editor, and copying it into the Secret Value field.

</details>





</details>


> [!NOTE]
> Additional information on CyberArk-SdkCredentialProvider can be found in the [supplemental documentation](docs/cyberark-sdkcredentialprovider.md).



## License

Apache License 2.0, see [LICENSE](LICENSE)

## Related Integrations

See all [Keyfactor PAM Provider extensions](https://github.com/orgs/Keyfactor/repositories?q=pam).