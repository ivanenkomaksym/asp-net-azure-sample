# asp-net-azure-sample

This sample demonstrates how to configure ASP.NET application for:

- Multitenant authentication (based on [Change your ASP.NET Core Web app to sign-in users in any org with the Microsoft identity platform](https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/blob/master/1-WebApp-OIDC/1-2-AnyOrg/README-1-1-to-1-2.md))
- Application only permissions (based on [Get access without a user](https://learn.microsoft.com/en-us/graph/auth-v2-service))

## Dependencies
[.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## How to run this sample
1. Sign in to the [Azure portal](https://portal.azure.com) using either a work or school account or a personal Microsoft account
2. Register the web application
   - In the **Supported account types** section, select **Accounts in any organizational directory**
   - In the **Authentication**/**Implicit grant and hybrid flows** section, check **ID tokens**
   - In the **Certificates & secrets** generate new client secret and save the value
   - In the **Expose an API** make sure Application ID URI contains **"api://[client id]"**
   - in the **Expose an API** add scope **"api://[client id]/[Application name]"**
   - In the **App roles** add new **access_as_application** role for **Applications** as **Allowed member types**
   - In the **API permissions** add **Azure Service Management** and select **user_impersonation**
   - In the **API permissions** add **My APIs**, select current application, choose **Application permissions** and **access_as_application**. Administrator must grant the permissions your app needs
3. Register swagger client application
   - In the **Certificates & secrets** generate new client secret and save the value
4. Configure swagger client application
   - In the **API permissions** add **My APIs**, select application from step 2 and choose **Delegated permissions**
5. Fill in **appsettings.json**
6. Start application
7. Click authorize in Swagger, select your work or school account
   - https://localhost:44321/WeatherForecast is accessible from all accounts that belong to **AcceptedTenantIds** list in **appsettings.json**
8. In **Postman** get an access token as described in [Get an access token](https://learn.microsoft.com/en-us/graph/auth-v2-service#4-get-an-access-token)
   - POST https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token
   - client_id={client id from step 2}
   - scope=api://{client id from step2}/.default
   - client_secret={client secret from step 2}
   - grant_type=client_credentials
9. Using generated token access https://localhost:44321/Maintenance
