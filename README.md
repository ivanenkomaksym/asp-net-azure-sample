# asp-net-azure-sample

This sample demonstrates how to configure ASP.NET application for:

- Multitenant authentication (based on [Change your ASP.NET Core Web app to sign-in users in any org with the Microsoft identity platform](https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/blob/master/1-WebApp-OIDC/1-2-AnyOrg/README-1-1-to-1-2.md))
- Swagger client application access using signed-in user
- Application only permissions (based on [Get access without a user](https://learn.microsoft.com/en-us/graph/auth-v2-service) and [A .NET Core daemon console application calling a protected Web API with its own identity](https://github.com/Azure-Samples/active-directory-dotnetcore-daemon-v2/tree/master/2-Call-OwnApi))
- Use multiple authentication schemes: AzureAD, Google and Cookie-based
- Get Microsoft/Google access tokens programatically for automated testing

## Dependencies
[.NET8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

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

## Authentication with Google
1. Sign in to the [Google Console](https://console.cloud.google.com)
2. In **APIs & Services**/**Credentials** create new credentials
3. Configure necessary Javascript origins and redirect URIs, including https://developers.google.com/oauthplayground
4. Go to [OAuth2 Playground](https://developers.google.com/oauthplayground)
5. Press top right settings (gear) icon (OAuth 2.0 configuration)
6. Tick **Use your own OAuth credentials** and enter OAuth Client ID and OAuth Client secret
7. At the bottom enter scopes:
```
openid https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile
```
8. Press Authorize APIs.
9. On the next screen choose the account (optional screen) and give the permissions to the app.
10. Press **Exchange authorization code for tokens**
11. Get your non-expiring **refresh_token** and put it into **appsettings.json**
12. To get an **id_token** using **refresh_token**:
```
curl -d "client_id=YOUR_APP_CLIENT_ID&client_secret=YOUR_APP_CLIENT_SECRET&grant_type=refresh_token&refresh_token=YOUR_APP_REFRESH_TOKEN" "https://oauth2.googleapis.com/token"
```

## Authentication for Testing

In certain scenarios, it may be necessary to run the application with authentication enabled but without using a real user, especially during testing. This sample includes a special middleware that ensures the return of an authenticated user without actual authentication. This middleware is only added when the `ASPNETCORE_ENVIRONMENT` is set to `Testing`.

You can initiate the `testing` profile from Visual Studio or by running the following command:

```bash
dotnet run --environment Testing
```

When running in this mode, attempting to access the API without proper authentication in Swagger or a client will be restricted. However, you can still access the API by including the header `X-Testing-Name=<username>` in the request, for instance, when using Postman at https://localhost:44321/WeatherForecast.

## Multiple authentication schemes
1. In the **appsettings.json** set **AzureAd:Enable** and **Google:Enable** to **true** and fill in **Google:ClientId**.
2. Execute **npm start** in **react-spa** folder to launch frontend client on http://localhost:3000.
3. Click **Login** and choose one of the supported options:
	* Sign in with Google Access Token
	* Google one tap
	* Microsoft

![Alt text](docs/login.png?raw=true "Login")

4. Click **Get Weather** button. It will send the request to server with corresponding bearer token.

![Alt text](docs/callAPI.png?raw=true "Call API")

5. Click **Profile** to see your profile's information retrieved from the corresponding identity provider.

![Alt text](docs/profile.png?raw=true "Profile")

### Cookie-based authentication
1. Open Swagger UI via http://localhost:5000/swagger/index.html
2. Click **Authorize** button and authenticate with your Microsoft credentials
3. Confirm you can execute http://localhost:5000/WeatherForecast. In this case **Bearer** authentication scheme is challenged.
```
dbug: Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler[8]
      AuthenticationScheme: Bearer was successfully authenticated.
info: AspNetAzureSample.UserProviders.DefaultUserProvider[0]
      DefaultUserProvider: received '' user name.
```
4. Click **Authorize** button again and click **Logout**
5. Confirm you cannot anymore execute http://localhost:5000/WeatherForecast and get `401 Error: Unauthorized`
6. Execute `/register` with body:
```json
{
  "email": "alice@gmail.com",
  "password": "string1!"
}
```
7. Execute `/login` with the same body. Set `useCookies` to `true`. New cookie will appear in the browser, a sample cookie record is shown:
![Alt text](docs/cookie.png?raw=true "Cookie")
9. Confirm you can again execute http://localhost:5000/WeatherForecast. In this case **Identity.BearerAndApplication** authentication scheme is challenged.
```
dbug: Microsoft.Extensions.DependencyInjection.IdentityServiceCollectionExtensions+CompositeIdentityHandler[8]
      AuthenticationScheme: Identity.BearerAndApplication was successfully authenticated.
info: AspNetAzureSample.UserProviders.DefaultUserProvider[0]
      DefaultUserProvider: received 'alice@gmail.com' user name.
```
If you remove the cookie, authentication will again fail.

### Storage support

You can switch between storage types by **Storage:StorageType** property in the **appsettings.json**:
* InMemory
* MySql
* SqlServer

If you want to switch between different database:

```sh
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

3 dummy users are seeded to easily test solution:
```
admin@example.com   P@ssword1
alice@example.com   P@ssword1
bob@example.com     P@ssword1
```

## References
* [Authentication and ASP.NET Core Integration Testing using TestServer](https://medium.com/@zbartl/authentication-and-asp-net-core-integration-testing-using-testserver-15d47b03045a)
* [Use multiple authentication schemes](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-8.0#use-multiple-authentication-schemes)
* [How to Use Multiple Authentication Schemes in .NET](https://code-maze.com/dotnet-multiple-authentication-schemes/)
* [How to use Identity to secure a Web API backend for SPAs](https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/security/authentication/identity-api-authorization.md)
* [How to get Google access token programmatically (automated testing)](https://stackoverflow.com/questions/17657879/does-google-provide-test-users-for-integration-testing?newreg=7e561b60378a409f92ee3191cc92cdac)