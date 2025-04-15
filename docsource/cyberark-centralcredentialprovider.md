## Overview
The Cyber Ark Central Credential Provider (CCP) communicates with Cyber Ark over HTTPS with REST API calls.
It does not require a local instance of the Cyber Ark Credential Provider to be installed.

## Requirements
In order for the Central Credential Provider to work, the Safe / Secret being accessed need to be available to the Provider that the Cyber Ark server is using, and the Application ID needs to be usable from an external requestor.This may require adding IP address or other rules.

Certificate Authentication is not currently supported and needs to be disabled. This may necessitate creating a Site that allows HTTPS requests but does not require a Client Certificate to authenticate. By default the site `AIMWebService` may require a Client Certificate, which would need to be edited or have another site created.
