import React from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch } from 'react-redux';
import { useGoogleLogin } from '@react-oauth/google';
import { GoogleLogin as GoogleLoginButton } from '@react-oauth/google';
import { signinGoogleWithAccessToken, signinGoogleWithIdToken } from "../../redux/actions/auth";
import LoginStyles from "./Login.module.css";

function GoogleLogin() {
    const navigate = useNavigate();
    const dispatch = useDispatch();

    function handleGoogleLoginSuccess(tokenResponse) {
        console.log("GoogleLoginSuccess. Response: ", JSON.stringify(tokenResponse, null, 2));
        const accessToken = tokenResponse.access_token;
        console.log("accessToken: ", accessToken);
        dispatch(signinGoogleWithAccessToken(accessToken, navigate));
    }

    const googleLogin = useGoogleLogin({ onSuccess: handleGoogleLoginSuccess });

    return (
        <>
            <button onClick={() => googleLogin()} className={LoginStyles.googleBTN}>
                <i className="fa-brands fa-google"></i>Sign in with Google Access Token
            </button>
            <p/>
            <GoogleLoginButton
                onSuccess={credentialResponse => {
                    console.log(credentialResponse);
                    dispatch(signinGoogleWithIdToken(credentialResponse.credential, navigate));
                }}
                onError={() => {
                    console.log('Login Failed');
                }}
                useOneTap
            />
        </>
    );
}

export default GoogleLogin; 