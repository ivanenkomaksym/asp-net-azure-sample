import React, { useState, useEffect } from "react";
import LoginStyles from "./Login.module.css"
import { loginOrgAuthorizeUrl } from "../../authConfig";

function LoginOrg() {
    const [email, setEmail] = useState("");

    const [clicked, setClicked] = useState(false);
    useEffect(() => {
        if (clicked) {
          window.location.assign(`${loginOrgAuthorizeUrl}?email=${email}`);
        }
      });

    return (
        <div className={LoginStyles.loginContainer}>
            <div className={LoginStyles.loginContainerv2}>
                <h1>Welcome back</h1>

                <div className={LoginStyles.inputContainer}>
                    <label>EMAIL</label>
                    <input onChange={e => setEmail(e.target.value)} placeholder="enter your email" type="email" />
                </div>

                <div className={LoginStyles.inputContainer}>
                    <label>ORGANIZATION</label>
                    <select>
                        <option value="option1">Option 1</option>
                        <option value="option2">Option 2</option>
                        <option value="option3">Option 3</option>
                    </select>
                </div>

                <button onClick={() => setClicked(true)} className={LoginStyles.loginBTN}>LOGIN</button>
            </div>
        </div>
    );
}

export default LoginOrg;