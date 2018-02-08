# Cosign Identity Provider for IdentityServer4

Read about Cosign here http://weblogin.org/

Cosing Identity Provider redirects requests for authentication to Cosign server and gets authenticated user identity from Cosign server through tcp backchannel.

You will need to install Cosign handler to redirect request from Cosign server back to Cosign Identity Provider.

Cosign Identity Provider uses Redis for storing authenticated users https://github.com/MicrosoftArchive/redis/releases

You can implement ILoggedUsersStorage and use your own storage. 

External identity provider provides identity to IdentityServer4 through external security provider https://github.com/kvoyk/ExternalSecurityProvider

Other samples

IdentityServer4 QuickStart_4 sample with external security provider and two additional identity providers Mvc and Cosign https://github.com/kvoyk/4_ImplicitFlowAuthenticationWithExternal

IdentityServer4 QuickStart_5 sample with external security provider and two additional identity providers Mvc and Cosign https://github.com/kvoyk/5_HybridFlowAuthenticationWithApiAccess
