import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useDispatch } from 'react-redux';
import { signin } from "../../redux/actions/auth";
import LoginStyles from "./Login.module.css";

function LoginForm() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [validationErrors, setValidationErrors] = useState("");

    const navigate = useNavigate();
    const dispatch = useDispatch();

    const errorHandler = (err) => {        
        if (err.response && err.response.status === 401) {
            setValidationErrors("User name or password is incorrect.");
        }
    };

    function handleSubmit(e) {
        e.preventDefault();
        if (email !== "" && password !== "") {
            dispatch(signin({ email, password }, navigate, errorHandler));
        }
    }

    return (
        <>
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

            {validationErrors && (
                <span className={LoginStyles.errorMsg}>{validationErrors}</span>
            )}
        </>
    );
}

export default LoginForm; 