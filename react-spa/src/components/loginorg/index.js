import React, { useState, useEffect } from "react";
import LoginStyles from "./Login.module.css"
import { loginOrgAuthorizeUrl, domains } from "../../authConfig";

function LoginOrg() {
    const [email, setEmail] = useState("");
    const [selectedOption, setSelectedOption] = useState(domains[0]);

    const [clicked, setClicked] = useState(false);
    useEffect(() => {
        if (clicked) {
            console.log(`email=${email}&domain_hint=${selectedOption}`);
            window.location.assign(`${loginOrgAuthorizeUrl}?email=${email}&domain_hint=${selectedOption}`);
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

                <div className="dropdownContainer">
                    <label>ORGANIZATION</label>
                    <select value={selectedOption} onChange={e => setSelectedOption(e.target.value)} className="selectDropdown">
                    {domains.map(domain => (
                        <option key={domain} value={domain}>{domain}</option>
                    ))}
                    </select>
                </div>

                <button onClick={() => setClicked(true)} className={LoginStyles.loginBTN}>LOGIN</button>
            </div>
        </div>
    );
}

export default LoginOrg;