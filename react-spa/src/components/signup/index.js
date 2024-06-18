import React, {useState} from "react";
import SignUp from "./Signup.module.css"
import {Link,useNavigate} from "react-router-dom"

import {useGoogleLogin} from '@react-oauth/google';
import {useDispatch} from 'react-redux';
import {signup, signupGoogle} from "../../redux/actions/auth";

const InitState = {
    email: '',
    password: '',
    confirmPassword: ''
}

function Signup() {
    const nagivate = useNavigate();
    const dispatch = useDispatch();
    const [sForm, setSForm] = useState({
        email: '',
        password: '',
        confirmPassword: ''
    });

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setSForm({ ...sForm, [name]: value });
    };

    const [validationErrors, setValidationErrors] = useState({}); // State to store validation errors
    const errorHandler = (err) => {        
        if (err.response && err.response.status === 400 && err.response.data && err.response.data.errors) {
            console.log("SigUp error response: ", JSON.stringify(err.response.data, null, 2));
            setValidationErrors(err.response.data.errors); // Set validationErrors state with the received errors
        }
    };

    const [highlightedFields, setHighlightedFields] = useState([]);

    function handleGoogleLoginSuccess(tokenResponse) {

        const accessToken = tokenResponse.access_token;

        dispatch(signupGoogle(accessToken,nagivate))
    }

    function handleOnSubmit(e) {
        const errors = {};

        // Basic validation
        if (!sForm.email) {
            errors.email = 'Email is required';
        }
        if (!sForm.password) {
            errors.password = 'Password is required';
        }
        if (!sForm.confirmPassword) {
            errors.confirmPassword = 'Confirm Password is required';
        }
        if (sForm.password !== sForm.confirmPassword) {
            errors.confirmPassword = 'Passwords do not match';
        }

        // Highlight fields with errors
        setHighlightedFields(Object.keys(errors));

        // If there are errors, stop form submission
        if (Object.keys(errors).length > 0) {
            return;
        }

        // If no errors, proceed with form submission
        // Dispatch action or perform other tasks
        dispatch(signup(sForm, nagivate, errorHandler));
    };

    const login = useGoogleLogin({onSuccess: handleGoogleLoginSuccess});
    return (
        <div className={SignUp.loginContainer}>
            <div className={SignUp.loginContainerv2}>
                <h1>Create your account</h1>
                
                <div className={highlightedFields.includes('email') ? SignUp.inputContainerInvalid : SignUp.inputContainer}>
                    <label>EMAIL</label>
                    <input
                        name="email"
                        onChange={handleInputChange}
                        placeholder="enter your email"
                        type="email"
                        className={highlightedFields.includes('email') ? 'invalid-field' : ''}
                    />
                {(highlightedFields.includes('email') || (validationErrors && validationErrors.DuplicateEmail)) && (
                    <span className={SignUp.errorMsg}>
                        {highlightedFields.includes('email') ? 'Email is required' : validationErrors.DuplicateEmail[0]}
                    </span>
                )}
                </div>

                <div className={highlightedFields.includes('password') ? SignUp.inputContainerInvalid : SignUp.inputContainer}>
                    <label>PASSWORD</label>
                    <input
                        name="password"
                        onChange={handleInputChange}
                        placeholder="enter your password"
                        type="password"
                        className={highlightedFields.includes('password') ? 'invalid-field' : ''}
                    />
                    {highlightedFields.includes('password') && <span className={SignUp.errorMsg}>Password is required</span>}
                </div>

                <div className={highlightedFields.includes('confirmPassword') ? SignUp.inputContainerInvalid : SignUp.inputContainer}>
                    <label>CONFIRM PASSWORD</label>
                    <input
                        name="confirmPassword"
                        onChange={handleInputChange}
                        placeholder="retype your password"
                        type="password"
                    />
                    {highlightedFields.includes('confirmPassword') && <span className={SignUp.errorMsg}>Passwords do not match</span>}
                </div>

                <div className={SignUp.footerContainer}>
                        <div>
                            Already Signed Up? <Link to="/account/login">Login</Link>
                        </div>
                        <div>
                            <Link to="/account/forgotpassword">Forgot Password?</Link>
                        </div>
                    </div>

                <button onClick={handleOnSubmit} className={SignUp.loginBTN}>REGISTER</button>
                 <span className={SignUp.or}>or</span>
                 <button  onClick={() => login()}  className={SignUp.googleBTN}>
                    <i class="fa-brands fa-google"></i>  Sign up with google</button>

                 
            </div>

        </div>
    )
}

export default Signup;