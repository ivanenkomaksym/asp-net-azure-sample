/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { LogLevel } from "@azure/msal-browser";

export const gsiConfig = {
    client_id: '438557524836-iule8uso69s7n6jsho4q3j2i5umh0qp0.apps.googleusercontent.com',
    auto_select: false // automatically sign in, see: https://developers.google.com/identity/gsi/web/guides/automatic-sign-in-sign-out
}

/**
 * Configuration object to be passed to MSAL instance on creation. 
 * For a full list of MSAL.js configuration parameters, visit:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/configuration.md 
 */

export const msalConfig = {
    auth: {
        clientId: "ea684d3c-e084-49b8-a429-5075aff9d32b",
        authority: "https://login.microsoftonline.com/6956efa6-895b-430f-b07b-45feb79f24eb",
        redirectUri: "http://localhost:3000",
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
    scopes: ["0a870d70-60dc-46ef-b913-58942a5ba00d/access_as_user"],
    forceRefresh: false, // Set this to "true" to skip a cached token and go to the server to get a new token
    prompt: "consent"
};

export const loginUrl = "http://localhost:5000/login";
export const registerUrl = "http://localhost:5000/register";
export const weatherForecastUrl = "http://localhost:5000/WeatherForecast";
export const loginOrgAuthorizeUrl = "[Enter your /authorize endpoint]";