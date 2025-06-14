import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';

import { Provider } from 'react-redux';
import { PublicClientApplication } from '@azure/msal-browser';
import { MsalProvider } from '@azure/msal-react';
import { msalConfig, gsiConfig, auth0Config } from './authConfig';
import { GoogleOAuthProvider } from '@react-oauth/google';
import {BrowserRouter} from "react-router-dom"

import thunk from "redux-thunk"
import { createStore, applyMiddleware, compose} from "redux"
import { reducers } from "./redux/reducers"

// Bootstrap components
import 'bootstrap/dist/css/bootstrap.min.css';
import { Auth0Provider } from '@auth0/auth0-react';

const msalInstance = new PublicClientApplication(msalConfig);

const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;
const store = createStore(reducers, {}, composeEnhancers(applyMiddleware(thunk)))

const root = ReactDOM.createRoot(document.getElementById('root'));

/**
 * We recommend wrapping most or all of your components in the MsalProvider component. It's best to render the MsalProvider as close to the root as possible.
 */
 root.render(
    <React.StrictMode>
      <Provider store={store}>
        <BrowserRouter>
            <GoogleOAuthProvider clientId={gsiConfig.client_id}>
                <MsalProvider instance={msalInstance}>
                    <Auth0Provider
                      domain={auth0Config.domain}
                      clientId={auth0Config.clientId}    
                      authorizationParams={{
                        // Make sure to include audience to get JWT token instead of opaque token
                        audience: auth0Config.audience,
                        org_id: auth0Config.organization,
                        redirect_uri: `${window.location.origin}/auth0-callback`
                      }}>
                        <App />
                    </Auth0Provider>
                </MsalProvider>
            </GoogleOAuthProvider>,
      </BrowserRouter>
    </Provider>
  </React.StrictMode>
);