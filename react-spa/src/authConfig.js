/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { LogLevel } from "@azure/msal-browser";

export const gsiConfig = {
    client_id: process.env.REACT_APP_GOOGLE_CLIENT_ID,
    auto_select: false // automatically sign in, see: https://developers.google.com/identity/gsi/web/guides/automatic-sign-in-sign-out
}

/**
 * Configuration object to be passed to MSAL instance on creation. 
 * For a full list of MSAL.js configuration parameters, visit:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/configuration.md 
 */

export const msalConfig = {
    auth: {
        clientId: process.env.REACT_APP_MICROSOFT_CLIENT_ID,
        authority: `https://login.microsoftonline.com/${process.env.REACT_APP_MICROSOFT_TENANT_ID}`,
        redirectUri: "http://localhost:3000"
    },
    cache: {
        cacheLocation: "sessionStorage", // This configures where your cache will be stored
        storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
    },
    system: {	
        loggerOptions: {	
            loggerCallback: (level, message, containsPii) => {	
                if (containsPii) {		
                    return;		
                }		
                switch (level) {
                    case LogLevel.Error:
                        console.error(message);
                        return;
                    case LogLevel.Info:
                        console.info(message);
                        return;
                    case LogLevel.Verbose:
                        console.debug(message);
                        return;
                    case LogLevel.Warning:
                        console.warn(message);
                        return;
                    default:
                        return;
                }	
            }	
        }	
    }
};

export const auth0Config = {
    domain: process.env.REACT_APP_AUTH0_DOMAIN || "dev-123456.us.auth0.com",
    clientId: process.env.REACT_APP_AUTH0_CLIENT_ID || "1234567890abcdef",
    organization: process.env.REACT_APP_AUTH0_ORGANIZATION || "org_123456",
};

/**
 * Scopes you add here will be prompted for user consent during sign-in.
 * By default, MSAL.js will add OIDC scopes (openid, profile, email) to any login request.
 * For more information about OIDC scopes, visit: 
 * https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-permissions-and-consent#openid-connect-scopes
 */
export const loginRequest = {
    scopes: ["User.Read"]
};

/**
 * Add here the scopes to request when obtaining an access token for MS Graph API. For more information, see:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/resources-and-scopes.md
 */
export const graphConfig = {
    graphMeEndpoint: "https://graph.microsoft.com/v1.0/me",
};

/**
 * Add here the scopes to request when obtaining an access token for MS Graph API. For more information, see:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/resources-and-scopes.md
 */
export const weatherForecastTokenRequest = {
    scopes: [process.env.REACT_APP_MICROSOFT_SCOPE],
    forceRefresh: false, // Set this to "true" to skip a cached token and go to the server to get a new token
    prompt: "consent"
};

const featureFlags = {
    selectOrganization: false
};

export default featureFlags;

export const loginUrl = process.env.REACT_APP_LOGIN_URL || "http://localhost:5000/login";
export const registerUrl = process.env.REACT_APP_REGISTER_URL || "http://localhost:5000/register";
export const weatherForecastUrl = process.env.REACT_APP_WEATHER_FORECAST_URL || "http://localhost:5000/weatherforecast";
export const organizationsUrl = process.env.REACT_APP_ORGANIZATIONS_URL || "http://localhost:5000/organizations";
export const environmentsUrl = process.env.REACT_APP_ENVIRONMENTS_URL || "http://localhost:5000/environments";
export const redirectToEnvironmentUrl = process.env.REACT_APP_REDIRECT_TO_ENVIRONMENT_URL || "http://localhost:5000/redirectToEnvironment";
export const loginOrgAuthorizeUrl = process.env.REACT_APP_LOGIN_ORG_AUTHORIZE_URL || "http://localhost:5000/loginOrgAuthorize";
export const refreshTokenUrl = process.env.REACT_APP_REFRESH_TOKEN_URL || "http://localhost:5000/refreshToken";

export const signInToOrganizations = process.env.REACT_APP_MICROSOFT_SIGN_IN_TO_ORGANIZATION === "true";