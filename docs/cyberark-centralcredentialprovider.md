## CyberArk-CentralCredentialProvider

The Cyber Ark Central Credential Provider (CCP) communicates with Cyber Ark over HTTPS with REST API calls.
It does not require a local instance of the Cyber Ark Credential Provider to be installed.

## Requirements

In order for the Central Credential Provider to work, the Safe / Secret being accessed need to be available to the Provider that the Cyber Ark server is using, and the Application ID needs to be usable from an external requestor.This may require adding IP address or other rules.

Certificate Authentication is not currently supported and needs to be disabled. This may necessitate creating a Site that allows HTTPS requests but does not require a Client Certificate to authenticate. By default the site `AIMWebService` may require a Client Certificate, which would need to be edited or have another site created.




## Mechanics
The `CyberArk-CentralCredentialProvider` PAM Provider Type communicates to a Cyber Ark instance using HTTPS. REST API calls are made to the host and site specified.
As Client Certificate Auth is not currently supported, the target Site on the Cyber Ark instance needs to not require a certificate for authentication.

Importantly, the way authentication and restriction works for requests with the Central Credential provider are determined by the Application Id used. Additional rules can be set in Cyber Ark to enact restrictions on where the request for an Application Id needs to come from.
After the Application Id is approved, the Central Credential Provider passes calls through an internally specified Provider object in Cyber Ark to the Vault.
