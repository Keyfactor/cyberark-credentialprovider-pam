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
