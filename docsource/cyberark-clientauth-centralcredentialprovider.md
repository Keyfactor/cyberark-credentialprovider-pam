## Overview
The Cyber Ark Client-Auth Central Credential Provider (CCP) communicates with Cyber Ark over HTTPS with [REST API calls](https://docs.cyberark.com/credential-providers/latest/en/content/ccp/calling-the-web-service-using-rest.htm), using client certificate authentication for secure communication.

It does not require a local instance of the Cyber Ark Credential Provider to be installed on the machine using the PAM Provider.

## Requirements

> [!IMPORTANT]
> 
> If using this PAM type, you will need to replace the `manifest.json` file with the contents of `ClientAuth-manifest.json`. Please see the `Install PAM provider on a Universal Orchestrator Host (Remote) - manifest.json` section below for more details.

In order for the Client-Auth Central Credential Provider to work, the Safe / Secret being accessed need to be available to the Provider that the Cyber Ark server is using, and the Application ID needs to be usable from an external requestor. This may require adding IP address or other rules.

In order for the integration to take advantage of Client Certificate auth, please ensure that HTTPS is enabled and configured to require a Client Certificate. By default the site `AIMWebService` may be configured to require a Client Certificate.

To read secrets stored in a CyberArk Vault safe, the Application ID must have at least the following permissions on the safe:
- Monitor Safe
- Retrieve files from Safe

### Install PAM provider on a Universal Orchestrator Host (Remote) - manifest.json

The default <code>manifest.json</code> needs to be replaced with the included <code>ClientAuth-manifest.json</code>. Rename the existing <code>manifest.json</code> as <code>Central-manifest.json</code> and then rename the <code>ClientAuth-manifest.json</code> to replace the original <code>manifest.json</code>.


## Mechanics
The `CyberArk-ClientAuth-CentralCredentialProvider` PAM Provider Type communicates to a Cyber Ark instance using HTTPS. REST API calls are made to the host and site specified.

The Client Authentication certificate may be provided in one of two ways via the initialization parameters:

- **PFX Base64** (`PfxBase64`): the PFX certificate encoded as a Base64 string.
- **PFX File Path** (`PfxFilePath`): a relative or absolute path to a PFX file on the machine running the Universal Orchestrator. Relative paths are resolved against the working directory of the Orchestrator / Keyfactor Command service. The service running the Orchestrator / Keyfactor Command must have read access to the file.

If both are supplied, `PfxBase64` takes precedence and `PfxFilePath` is ignored. If either field is set to an empty value, whitespace, or the literal string `none`, it is treated as if it was not provided. If neither field resolves to a usable value, the PAM job will fail with an error.

> [!WARNING]
> If `PfxFilePath` is specified but the file does not exist or the Orchestrator service account does not have read access to it, the PAM job will fail with a message similar to:
> `Could not find file '/path/to/certificate.pfx'`

The PFX password (`PfxPassword`) is always required regardless of which method is used to supply the certificate.

To obtain the Base64 encoded PFX, you can encode your PFX file using the following commands (subject to your operating system):

### PowerShell

```powershell
$pfxBytes = Get-Content -Path "path\to\your\certificate.pfx" -Encoding Byte
$pfxBase64 = [Convert]::ToBase64String($pfxBytes)
Write-Output $pfxBase64
```

### Bash

```bash
pfxBase64=$(base64 -i /path/to/your/certificate.pfx)
echo $pfxBase64
```

Importantly, authorization in the Central Credential Provider is governed by the Application ID. The Application in CyberArk can be configured with authentication restrictions that control exactly which callers are permitted to use it. For client certificate authentication specifically, CyberArk supports restricting an Application to only accept requests authenticated with a certificate matching specific attributes — such as serial number, subject, or issuer. This means administrators can pin the Application to the specific certificate issued to the Keyfactor Orchestrator, preventing any other caller from accessing it even if they know the Application ID. See [Application authentication methods](https://docs.cyberark.com/credential-providers/14.2/en/content/cp%20and%20ascp/application-authentication-methods-general.htm#) in the CyberArk documentation for details on configuring these restrictions.

After the Application ID is approved, the Central Credential Provider passes calls through an internally specified Provider object in CyberArk to the Vault.