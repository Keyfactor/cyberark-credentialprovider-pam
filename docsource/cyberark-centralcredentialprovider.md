## Overview
The CyberArk Central Credential Provider (CCP) communicates with CyberArk over HTTPS with [REST API calls](https://docs.cyberark.com/credential-providers/latest/en/content/ccp/calling-the-web-service-using-rest.htm).

It does not require a local instance of the CyberArk Credential Provider to be installed on the machine using the PAM Provider, however this PAM type **only supports anonymous authentication** to the Central Credential Provider API. Client authentication to Central Credential Provider is supported with the `CyberArk-ClientAuth-CentralCredentialProvider` PAM type.

## Requirements
In order for the Central Credential Provider to work, the Safe / Secret being accessed need to be available to the Provider that the CyberArk server is using, and the Application ID needs to be usable from an external requestor. This may require adding IP address or other rules.

Certificate Authentication is not currently supported and needs to be disabled. This may necessitate creating a Site that allows HTTPS requests but does not require a Client Certificate to authenticate. By default the site `AIMWebService` may require a Client Certificate, which would need to be edited or have another site created.

To read secrets stored in a CyberArk Vault safe, the Application ID must have at least the following permissions on the safe:
- Monitor Safe
- Retrieve files from Safe

## Mechanics
The `CyberArk-CentralCredentialProvider` PAM Provider Type communicates to a CyberArk instance using HTTPS. REST API calls are made to the host and site specified.

As this PAM type only supports anonymous authentication, the target Site on the CyberArk instance needs to be configured for anonymous access.

Importantly, the way authentication and restriction works for requests with the Central Credential provider are determined by the Application ID used. Additional rules can be set in CyberArk to enact restrictions on where the request for an Application ID needs to come from.
After the Application ID is approved, the Central Credential Provider passes calls through an internally specified Provider object in CyberArk to the Vault.