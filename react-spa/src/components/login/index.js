import React, {useState} from "react";
import { Link,useNavigate } from "react-router-dom";
import LoginStyles from "./Login.module.css"
import {useGoogleLogin} from '@react-oauth/google';
import {useDispatch} from 'react-redux';
import {signinGoogle, signin, signinMicrosoft} from "../../redux/actions/auth";
import { useMsal } from "@azure/msal-react";
import { loginRequest } from "../../authConfig";

function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const navigate = useNavigate();
    const dispatch = useDispatch();

    // Google login

    function handleGoogleLoginSuccess(tokenResponse) {
        const accessToken = tokenResponse.access_token;
        console.log("accessToken: ", accessToken);
        dispatch(signinGoogle(accessToken, navigate));
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

    // Function to handle form submission
    function handleSubmit(e) {
        e.preventDefault();
        if (email !== "" && password !== "") {
            dispatch(signin({ email, password }, navigate));
        }
    }

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
                <span className={LoginStyles.or}>or</span>
                <button onClick={() => googleLogin()} className={LoginStyles.googleBTN}>
                    <i class="fa-brands fa-google"></i>  Sign in with Google
                </button>
                <p/>
                <button onClick={microsoftLogin} className={LoginStyles.microsoftBTN}>
                    Sign in with Microsoft
                </button>
                <span className={LoginStyles.notreg}>Not registered yet? <Link className={LoginStyles.singupBTN} to="/account/signup">Signup</Link></span>

            </div>
        </div>
    );
}

export default Login;