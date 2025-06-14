import React from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { auth0Config } from "../../authConfig";
import LoginStyles from "./Login.module.css";

function Auth0Login() {
    const { loginWithRedirect } = useAuth0();

    const Auth0Login = () => {
        loginWithRedirect({
            authorizationParams: {
                organization: auth0Config.organization
            }
        });
    };

    return (
        <button className={LoginStyles.auth0BTN} onClick={Auth0Login}>
           Sign in with Auth0
        </button>
    );
}

export default Auth0Login; 