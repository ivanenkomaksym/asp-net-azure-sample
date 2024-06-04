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

    const [highlightedFields, setHighlightedFields] = useState([]);

    function handleGoogleLoginSuccess(tokenResponse) {

        const accessToken = tokenResponse.access_token;

        dispatch(signupGoogle(accessToken,nagivate))
    }

    function handleOnSubmit(e) {
        if (
            sForm.password !== "" &&
            sForm.confirmPassword !== "" &&
            sForm.email !== "" &&
            sForm.password === sForm.confirmPassword &&
            sForm.password.length >= 4
        ) {
            // Reset highlightedFields if all conditions are met
            setHighlightedFields([]);
            dispatch(signup(sForm, nagivate));
        } else {
            // Highlight empty fields
            const highlighted = Object.keys(sForm).filter(field => sForm[field] === "");
            console.log("Highlight", highlighted);
            setHighlightedFields(highlighted);
        }
    };

    const login = useGoogleLogin({onSuccess: handleGoogleLoginSuccess});
    return (
        <div className={SignUp.loginContainer}>
            <div className={SignUp.loginContainerv2}>
                <h1>Create your account</h1>
                
                <div className={SignUp.inputContainer}>
                    <label>EMAIL</label>
                    <input
                        name="email"
                        onChange={handleInputChange}
                        placeholder="enter your email"
                        type="email"
                        style={{ borderColor: highlightedFields.includes('email') ? 'red' : 'inherit' }}
                    />
                </div>

                <div className={SignUp.inputContainer}>
                    <label>PASSWORD</label>
                    <input
                        name="password"
                        onChange={handleInputChange}
                        placeholder="enter your password"
                        type="password"
                        style={{ borderColor: highlightedFields.includes('password') ? 'red' : 'inherit' }}
                    />
                </div>

                <div className={SignUp.inputContainer}>
                    <label>CONFIRM PASSWORD</label>
                    <input
                        name="confirmPassword"
                        onChange={handleInputChange}
                        placeholder="retype your password"
                        type="password"
                        style={{ borderColor: highlightedFields.includes('confirmPassword') ? 'red' : 'inherit' }}
                    />
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