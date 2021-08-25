# GraphDatabaseSandbox
Sandbox project for testing all things new.

Testing project.

> Async Console Application
> Makes use of Host Builder
> Supports Http requests
> Docker Support
> Hosted Services

ToDo: 
> Add Graph DB 

To Run:

For Auth, in user secrets set the override for the app settings:

```
{
    "AzureAd": {
      "Instance": "https://login.microsoftonline.com/",
      "Domain": "[Enter the domain of your tenant, e.g. contoso.onmicrosoft.com]",
      "ClientId": "Enter_the_Application_Id_here",
      "TenantId": "common",
      "CallbackPath": "/signin-oidc"
    }
}

Registration in azure AD for an app client ID is required.
Then on accessing https://localhost:44321/private - you will need to authenticate.
  
