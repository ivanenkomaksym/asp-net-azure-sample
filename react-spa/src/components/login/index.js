import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import LoginStyles from "./Login.module.css"
import { useGoogleLogin } from '@react-oauth/google';
import { useDispatch } from 'react-redux';
import { signinGoogleWithAccessToken, signinGoogleWithIdToken, signin, signinMicrosoft, signinAuth0 } from "../../redux/actions/auth";
import { useMsal } from "@azure/msal-react";
import { auth0Config, loginRequest } from "../../authConfig";
import { GoogleLogin } from '@react-oauth/google';
import { useAuth0 } from "@auth0/auth0-react";

function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const navigate = useNavigate();
    const dispatch = useDispatch();

    // Google login

    function handleGoogleLoginSuccess(tokenResponse) {
        console.log("GoogleLoginSuccess. Response: ", JSON.stringify(tokenResponse, null, 2));
        const accessToken = tokenResponse.access_token;
        console.log("accessToken: ", accessToken);
        dispatch(signinGoogleWithAccessToken(accessToken, navigate));
    }

    const googleLogin = useGoogleLogin({ onSuccess: handleGoogleLoginSuccess });

    // Microsoft login
    const { instance } = useMsal();

    const microsoftLogin = (loginType) => {
        instance.loginPopup(loginRequest)
            .then(response => {
                // Handle successful login response
                console.log("Microsoft login successful:", response);
                dispatch(signinMicrosoft(response, navigate));
            }).catch(e => {
                console.log(e);
            });
    }

    // Auth0 login
    const { user, loginWithPopup, getAccessTokenWithPopup } = useAuth0();

    const Auth0Login = async () => {
        loginWithPopup({
            authorizationParams: {
            org_id: auth0Config.organization
            }
        }).then(async (response) => {
            console.log("Auth0 login successful:", response);
            console.log("auth0 user:", user);
            console.log("auth0 audience:", auth0Config.audience);

            const token = await getAccessTokenWithPopup({ 
                    authorizationParams: { 
                        audience: auth0Config.audience,
                        organization: auth0Config.organization
                    }
                });
            console.log("Auth0 token:", token);
            // You can dispatch an action here if needed
            dispatch(signinAuth0(user, token, navigate));
        }).catch((error) => {
            console.error("Auth0 login failed:", error);
            // Handle error appropriately
        });
    }

    const [validationErrors, setValidationErrors] = useState(""); // State to store validation errors
    const errorHandler = (err) => {        
        if (err.response && err.response.status === 401) {
            setValidationErrors("User name or password is incorrect.");
        }
    };

    // Function to handle form submission
    function handleSubmit(e) {
        e.preventDefault();
        if (email !== "" && password !== "") {
            dispatch(signin({ email, password }, navigate, errorHandler));
        }
    }

    const handleLoginOrgClick = () => {
        navigate('/account/loginorg');
    };

    return (
        <div className={LoginStyles.loginContainer}>
            <div className={LoginStyles.loginContainerv2}>
                <h1>Welcome back</h1>

                <div className={LoginStyles.inputContainer}>
                    <label>EMAIL</label>
                    <input onChange={e => setEmail(e.target.value)} placeholder="enter your email" type="email" />
                </div>

                <div className={LoginStyles.inputContainer}>
                    <label>PASSWORD</label>
                    <input onChange={e => setPassword(e.target.value)} placeholder="enter your password" type="password" />
                </div>

                <div className={LoginStyles.forgetmeContainer}>
                    <div>
                        Remember Me <input type="checkbox" />
                    </div>
                    <div>
                        <Link to="/account/forgotpassowrd">Forgot password?</Link>
                    </div>
                </div>

                <button onClick={handleSubmit} className={LoginStyles.loginBTN}>LOGIN</button>

                {validationErrors != null && (
                        <span className={LoginStyles.errorMsg}>{validationErrors}</span>
                    )}

                <span className={LoginStyles.or}>or</span>
                <button onClick={() => googleLogin()} className={LoginStyles.googleBTN}>
                    <i class="fa-brands fa-google"></i>  Sign in with Google Access Token
                </button>
                <GoogleLogin
                    onSuccess={credentialResponse => {
                        console.log(credentialResponse);
                        dispatch(signinGoogleWithIdToken(credentialResponse.credential, navigate));
                    }}

                    onError={() => {
                        console.log('Login Failed');
                    }}
                    useOneTap
                />
                <p />
                <button onClick={microsoftLogin} className={LoginStyles.microsoftBTN}>
                    Sign in with Microsoft
                </button>
                <p />

                <button className={LoginStyles.organizationBTN} onClick={handleLoginOrgClick}>
                    Sign in with organization
                </button>
                <p />

                <button className={LoginStyles.auth0BTN} onClick={Auth0Login}>
                   Sign in with Auth0
                </button>
                <p />

                <span className={LoginStyles.notreg}>Not registered yet? <Link className={LoginStyles.singupBTN} to="/account/signup">Signup</Link></span>
            </div>
        </div>
    );
}

export default Login;