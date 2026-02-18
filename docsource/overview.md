## Overview
The CyberArk PAM Provider integration in Keyfactor allows for the retrieval of credentials from a CyberArk Vault. Two different methods are supported for communicating securely with a CyberArk platform:
- **Central Credential Provider** - This method communicates with a CyberArk instance over HTTPS to retrieve credentials. This method does not require a local Credential Provider to be installed.
- **SDK-based local Credential Provider** - This method communicates with a CyberArk instance using CyberArk's installed Credential Provider. This method requires a local Credential Provider to be installed.

Each method has its own configuration and installation requirements. Both are able to operate running on either Keyfactor Command or a Universal Orchestrator.
Please refer to the below sections for the different considerations required for using each method.

#### Compatibility
This release was tested against CyberArk version 12.6.
Using this on a Universal Orchestrator requires UO version 10.1 or greater.
