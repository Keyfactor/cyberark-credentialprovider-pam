## CyberArk-ClientAuth-CentralCredentialProvider

The Cyber Ark Client-Auth  Central Credential Provider (CCP) communicates with Cyber Ark over HTTPS with REST API calls.
It does not require a local instance of the Cyber Ark Credential Provider to be installed.

## Requirements

In order for the Client-Auth Central Credential Provider to work, the Safe / Secret being accessed need to be available to the Provider that the Cyber Ark server is using, and the Application ID needs to be usable from an external requestor. This may require adding IP address or other rules.

In order for the integration to take advantage of Client Certificate auth, please ensure that HTTPS is enabled and configured to require a Client Certificate. By default the site `AIMWebService` may be configured to require a Client Certificate.

To read secrets stored in a CyberArk Vault safe, the Application ID must have at least the following permissions on the safe:
- Monitor Safe
- Retrieve files from Safe

### Install PAM provider on a Universal Orchestrator Host (Remote) - manifest.json

The default <code>manifest.json</code> needs to be replaced with the included <code>ClientAuth-manifest.json</code>. Rename the existing <code>manifest.json</code> as <code>Central-manifest.json</code> and then rename the <code>ClientAuth-manifest.json</code> to replace the original <code>manifest.json</code>.




## Mechanics
The `CyberArk-ClientAuth-CentralCredentialProvider` PAM Provider Type communicates to a Cyber Ark instance using HTTPS. REST API calls are made to the host and site specified.
The Client Authentication certificate is provided by a Base64 encoded PFX and a password for the PFX, which are initialization parameters for this PAM provider type. The HTTP client will be configured to use the client certificate when making requests to the Cyber Ark instance.

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

Importantly, the way authentication and restriction works for requests with the Central Credential provider are determined by the Application ID used. Additional rules can be set in Cyber Ark to enact restrictions on where the request for an Application ID needs to come from.

After the Application ID is approved, the Central Credential Provider passes calls through an internally specified Provider object in Cyber Ark to the Vault.
