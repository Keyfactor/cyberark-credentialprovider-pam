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

TODO this section is required

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

The CyberArk PAM Provider implements 2 PAM Types. Depending on your use case, you may elect to install one, or all of these PAM Types. An overview for each type is linked below:
* [CyberArk-CentralCredentialProvider](docs/cyberark-centralcredentialprovider.md)
* [CyberArk-SdkCredentialProvider](docs/cyberark-sdkcredentialprovider.md)






<details><summary>CyberArk-CentralCredentialProvider</summary>


#### Requirements
   TODO Requirements is a required section

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
            "InstanceLevel": false
        },
        {
            "Name": "Host",
            "DisplayName": "CyberArk Host and Port",
            "DataType": 1,
            "InstanceLevel": false
        },
        {
            "Name": "Site",
            "DisplayName": "CyberArk API Site",
            "DataType": 1,
            "InstanceLevel": false
        },
        {
            "Name": "Safe",
            "DisplayName": "Safe",
            "DataType": 1,
            "InstanceLevel": true
        },
        {
            "Name": "Folder",
            "DisplayName": "Folder",
            "DataType": 1,
            "InstanceLevel": true
        },
        {
            "Name": "Object",
            "DisplayName": "Object",
            "DataType": 1,
            "InstanceLevel": true
        }
    ]
}
```

#### Install PAM provider on Keyfactor Command Host (Local)


("TODO Platform Install is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)


#### Install PAM provider on a Universal Orchestrator Host (Remote)


("TODO Orchestrator Install is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)



</details>







<details><summary>CyberArk-SdkCredentialProvider</summary>


#### Requirements
   TODO Requirements is a required section

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
            "InstanceLevel": false
        },
        {
            "Name": "Safe",
            "DisplayName": "Safe",
            "DataType": 1,
            "InstanceLevel": true
        },
        {
            "Name": "Folder",
            "DisplayName": "Folder",
            "DataType": 1,
            "InstanceLevel": true
        },
        {
            "Name": "Object",
            "DisplayName": "Object",
            "DataType": 1,
            "InstanceLevel": true
        }
    ]
}
```

#### Install PAM provider on Keyfactor Command Host (Local)


("TODO Platform Install is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)


#### Install PAM provider on a Universal Orchestrator Host (Remote)


("TODO Orchestrator Install is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)



</details>





### Usage





<details><summary>CyberArk-CentralCredentialProvider</summary>


#### From Keyfactor Command Host (Local)


("TODO Platform Usage is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)


#### From a Universal Orchestrator Host (Remote)


("TODO Orchestrator Usage is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)



</details>


> [!NOTE]
> Additional information on CyberArk-CentralCredentialProvider can be found in the [supplemental documentation](docs/cyberark-centralcredentialprovider.md).





<details><summary>CyberArk-SdkCredentialProvider</summary>


#### From Keyfactor Command Host (Local)


("TODO Platform Usage is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)


#### From a Universal Orchestrator Host (Remote)


("TODO Orchestrator Usage is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)



</details>


> [!NOTE]
> Additional information on CyberArk-SdkCredentialProvider can be found in the [supplemental documentation](docs/cyberark-sdkcredentialprovider.md).



## License

Apache License 2.0, see [LICENSE](LICENSE)

## Related Integrations

See all [Keyfactor PAM Provider extensions](https://github.com/orgs/Keyfactor/repositories?q=pam).