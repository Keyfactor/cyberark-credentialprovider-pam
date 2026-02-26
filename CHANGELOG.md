# 2.2.0
## Features
- Added client authentication support for the CyberArk Credential Provider with the `CyberArk-ClientAuth-CentralCredentialProvider` PAM type.

## Chores
- Added more logging around the initialization, configuration and execution of the CyberArk CredentialProvider PAM Provider to assist with troubleshooting and debugging
- Updated README with latest Command configuration steps and tooling

# 2.1.0 
- Added additional parameter option when using Unity Configuration to target the correct directory with the Net Password SDK DLL
- Added additional parameter option when using Unity Configuration to specify if the Net Framework or Net Standard Password SDK should be used
- Improve logging and error handling to report errors correctly

# 2.0.1
- Bug fix for DLL compatibility issue preventing CyberArk from loading correctly on the Keyfactor Platform
- Include `manifest.json` and alternate `SDK-manifest.json` files and instructions on their use

# 2.0.0
- Re-release of the original CyberArk PAM Provider as a separate integration
- Supports running on Universal Orchestrator or Keyfactor platform
- Defines 2 types of CyberArk PAM Provider:
  - _Central Credential Provider_
    - Targets a URL endpoint of a hosted CyberArk instance
    - Does not require installing a separate Credential Provider to run
  - _SDK Credential Provider_
    - Mimics the original CyberArk PAM Provider
    - Requires a Credential Provider is installed and configured on the machine using PAM
    - Requires a separate `NetStandardPasswordSDK.dll` from CyberArk to function

