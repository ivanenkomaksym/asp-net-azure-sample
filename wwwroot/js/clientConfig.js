const clientConfig = {
    // 'Application (client) ID' of app registration in Azure portal - this value is a GUID
    clientId: [Enter the UI client id here],
    // Full directory URL, in the form of https://login.microsoftonline.com/<tenant-id>
    authority: "https://login.microsoftonline.com/organizations",
    // Full redirect URL, in form of http://localhost:3000
    redirectUri: "https://localhost:44321",
    // Scope
    scope: [Enter the server scope here],
    weatherForecastURI: "https://localhost:44321/WeatherForecast"
};