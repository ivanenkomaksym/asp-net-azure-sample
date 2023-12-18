# asp-net-azure-sample

This sample demonstrates how to configure ASP.NET application for:

- Multitenant authentication (based on [Change your ASP.NET Core Web app to sign-in users in any org with the Microsoft identity platform](https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/blob/master/1-WebApp-OIDC/1-2-AnyOrg/README-1-1-to-1-2.md))
- Swagger client application access using signed-in user
- Application only permissions (based on [Get access without a user](https://learn.microsoft.com/en-us/graph/auth-v2-service) and [A .NET Core daemon console application calling a protected Web API with its own identity](https://github.com/Azure-Samples/active-directory-dotnetcore-daemon-v2/tree/master/2-Call-OwnApi))

## Dependencies
[.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## How to run this sample
1. Sign in to the [Azure portal](https://portal.azure.com) using either a work or school account or a personal Microsoft account
2. Register the service web application
   - In the **Supported account types** section, select **Accounts in any organizational directory**
   - In the **Expose an API** make sure Application ID URI contains **"api://[client id]"**
   - in the **Expose an API** add scope **"api://[client id]/[Application name]"**
   - In the **App roles** add new **access_as_application** role for **Applications** as **Allowed member types**
   - In the **API permissions** add **Azure Service Management** and select **user_impersonation**
3. Register swagger client application
   - Add https://localhost:44321 to **Authentication**/**Redirect URIs**
   - In the **Authentication**/**Implicit grant and hybrid flows** section, check **ID tokens**
   - In the **Certificates & secrets** generate new client secret and save the value
   - In the **API permissions** add **My APIs**, select service application from step 2 and choose **Delegated permissions**
4. Register UI client application
   - In the **Supported account types** section, select **Accounts in any organizational directory**
   - in the **Redirect URIs** select **Single-page application (SPA)** and add https://localhost:44321
   - In the **API permissions** add **My APIs**, select service application from step 2 and choose **Delegated permissions**
   - Get back to service web application Azure configuration from step 2 and add this UI client application id to **Expose an API**/**Authorized client applications**
5. Register daemon client application
   - In the **Certificates & secrets** generate new client secret and save the value
   - In the **API permissions** add **My APIs**, select service application from step 2, choose **Application permissions** and **access_as_application**.
   - At this stage permissions are assigned correctly but the client app does not allow interaction. Therefore no consent can be presented via a UI and accepted to use the service app. Click the Grant/revoke admin consent for {tenant} button, and then select Yes when you are asked if you want to grant consent for the requested permissions for all account in the tenant. You need to be an Azure AD tenant admin to do this.
6. Fill in **appsettings.json**
7. Fill in **wwwroot\js\clientConfig.js**
8. Start application
9. Click **Sign in** using accounts from different tenants
10. Click **Weather Forecast** button to access service web application
   - https://localhost:44321/WeatherForecast is accessible from all accounts that belong to **AcceptedTenantIds** list in **appsettings.json**
11. Swagger page is accessible via https://localhost:44321/swagger
12. In **Postman** get an access token as described in [Get an access token](https://learn.microsoft.com/en-us/graph/auth-v2-service#4-get-an-access-token)
   - POST https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token
   - client_id={daemon app client id from step 4}
   - scope={service app client id from step2}/.default
   - client_secret={daemon app client secret from step 4}
   - grant_type=client_credentials
13. Using generated token access https://localhost:44321/Maintenance

## Authentication for Testing

In certain scenarios, it may be necessary to run the application with authentication enabled but without using a real user, especially during testing. This sample includes a special middleware that ensures the return of an authenticated user without actual authentication. This middleware is only added when the `ASPNETCORE_ENVIRONMENT` is set to `Testing`.

You can initiate the `testing` profile from Visual Studio or by running the following command:

```bash
dotnet run --environment Testing
```

When running in this mode, attempting to access the API without proper authentication in Swagger or a client will be restricted. However, you can still access the API by including the header `X-Testing-Name=<username>` in the request, for instance, when using Postman at https://localhost:44321/WeatherForecast.

## References
[Authentication and ASP.NET Core Integration Testing using TestServer](https://medium.com/@zbartl/authentication-and-asp-net-core-integration-testing-using-testserver-15d47b03045a)
