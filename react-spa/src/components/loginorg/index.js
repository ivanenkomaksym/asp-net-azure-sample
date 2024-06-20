import React, { useState, useEffect } from "react";
import LoginorgStyles from "./Loginorg.module.css"
import { queryDomains } from "../../api";
import { loginOrgAuthorizeUrl } from "../../authConfig";

function LoginOrg() {
    const [domains, setDomains] = useState([]);

    const [email, setEmail] = useState("");
    const [selectedOption, setSelectedOption] = useState('');

    // State to trigger getDomains
    const [emailSet, setEmailSet] = useState(false);

    const handleEmailSet = () => {
        setValidationErrors("");
        setEmailSet(!emailSet);
    };

    // Function to reset emailSet state
    const handleChangeEmail = () => {
        setEmailSet(false);
    };

    const [validationErrors, setValidationErrors] = useState(""); // State to store validation errors
    const errorHandler = (err) => {        
        console.log("error response: ", JSON.stringify(err, null, 2));
        if (err) {
            setValidationErrors("This email address is not supported.");
            setEmailSet(!emailSet);
        }
    };

    useEffect(() => {
        const getDomains = async () => {
            try {
                const data = await queryDomains(email, errorHandler);
                setDomains(data);
            } catch (error) {
                console.error('Error fetching domains:', error);
            }
        };

        if (emailSet) {
            getDomains();
        }
    }, [emailSet]);

    const [clicked, setClicked] = useState(false);
    useEffect(() => {
        if (clicked) {
            console.log(`email=${email}&organization=${selectedOption}`);
            window.location.assign(`${loginOrgAuthorizeUrl}?email=${email}&organization=${selectedOption}`);
        }
    });

    return (
        <div className={LoginorgStyles.loginContainer}>
            <div className={LoginorgStyles.loginContainerv2}>
                <h1>Welcome back</h1>

                {emailSet ? (
                    <>
                    <div className={LoginorgStyles.emailDisplayContainer}>
                        <label>Email address</label>
                        <br></br>
                        <div className={LoginorgStyles.emailAddress}>
                            {email}
                        </div>
                        <button onClick={handleChangeEmail} className={LoginorgStyles.changeEmailBTN}>Change</button>
                    </div>
                    <div className="dropdownContainer">
                        <label>I'm with</label>
                        {domains != null && (
                            <select value={selectedOption} onChange={e => setSelectedOption(e.target.value)} className="selectDropdown">
                                {domains.map(domain => (
                                    <option key={domain.id} value={domain.id}>{domain.description}</option>
                                ))}
                            </select>
                        )}
                    </div><button onClick={() => setClicked(true)} className={LoginorgStyles.loginBTN}>LOGIN</button>
                    </>
                ) : (
                    <>
                    <div className={LoginorgStyles.inputContainer}>
                        <label>EMAIL</label>
                        <input
                            onChange={e => setEmail(e.target.value)}
                            placeholder="enter your email"
                            type="email"
                        />
                    </div>
                    {validationErrors != null && (
                        <span className={LoginorgStyles.errorMsg}>{validationErrors}</span>
                    )}
                    <button onClick={handleEmailSet} className={LoginorgStyles.loginBTN}>Continue</button>
                    </>
                )}
            </div>
        </div>
    );
}

export default LoginOrg;