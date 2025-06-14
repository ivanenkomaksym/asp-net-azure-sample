import React from "react";
import { Link } from "react-router-dom";
import LoginStyles from "./Login.module.css"
import LoginForm from "./LoginForm";
import GoogleLogin from "./GoogleLogin";
import MicrosoftLogin from "./MicrosoftLogin";
import OrganizationLogin from "./OrganizationLogin";
import Auth0Login from "./Auth0Login";

function Login() {
    return (
        <div className={LoginStyles.loginContainer}>
            <div className={LoginStyles.loginContainerv2}>
                <h1>Welcome back</h1>

                <LoginForm />

                <span className={LoginStyles.or}>or</span>
                
                <GoogleLogin />
                <p />
                
                <MicrosoftLogin />
                <p />

                <OrganizationLogin />
                <p />

                <Auth0Login />
                <p />

                <span className={LoginStyles.notreg}>Not registered yet? <Link className={LoginStyles.singupBTN} to="/account/signup">Signup</Link></span>
            </div>
        </div>
    );
}

export default Login;