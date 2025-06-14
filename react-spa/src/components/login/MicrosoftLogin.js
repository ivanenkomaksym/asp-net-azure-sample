import React from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch } from 'react-redux';
import { useMsal } from "@azure/msal-react";
import { signinMicrosoft } from "../../redux/actions/auth";
import { loginRequest } from "../../authConfig";
import LoginStyles from "./Login.module.css";

function MicrosoftLogin() {
    const navigate = useNavigate();
    const dispatch = useDispatch();
    const { instance } = useMsal();

    const microsoftLogin = () => {
        instance.loginPopup(loginRequest)
            .then(response => {
                // Handle successful login response
                console.log("Microsoft login successful:", response);
                dispatch(signinMicrosoft(response, navigate));
            }).catch(e => {
                console.log(e);
            });
    }

    return (
        <button onClick={microsoftLogin} className={LoginStyles.microsoftBTN}>
            Sign in with Microsoft
        </button>
    );
}

export default MicrosoftLogin; 