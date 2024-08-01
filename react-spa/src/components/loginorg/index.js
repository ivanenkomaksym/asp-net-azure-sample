import React, { useState, useEffect } from "react";
import LoginorgStyles from "./Loginorg.module.css"
import { queryDomains } from "../../api";
import { loginOrgAuthorizeUrl } from "../../authConfig";
import User from "../../models/user"
import { ORGANIZATION } from "../../redux/const/actionsTypes";
import { saveUserToLocalStorage } from "../../redux/reducers/auth";
import isSelectOrganizationEnabled from "../../useFeatureFlags";

function LoginOrg() {
    const isSelectOrganization = isSelectOrganizationEnabled();

    const [domains, setDomains] = useState([]);

    const [email, setEmail] = useState("");
    const [selectedOrganization, setSelectedOrganization] = useState('');

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
                setSelectedOrganization(data[0].id);
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
            const user = new User(/*firstName       */null,
                                  /*lastName        */null,
                                  /*email           */email,
                                  /*picture         */null,
                                  /*identityProvider*/ORGANIZATION,
                                  /*id_token        */null,
                                  /*refresh_token   */null,
                                  /*organization    */selectedOrganization);
            saveUserToLocalStorage(user);
            console.log(`email=${email}&organization=${selectedOrganization}`);

            const loginUri = `${loginOrgAuthorizeUrl}?email=${email}`;
            if (isSelectOrganization)
                loginUri += `&organization=${selectedOrganization}`;
            window.location.assign(loginUri);
        }
    });

    return (
        <>
            {!isSelectOrganization ? (
                <>
                    <div className={LoginorgStyles.loginContainer}>
                        <div className={LoginorgStyles.loginContainerv2}>
                            <h1>Welcome back</h1>
                            <div className={LoginorgStyles.inputContainer}>
                                <label>EMAIL</label>
                                <input
                                    onChange={e => setEmail(e.target.value)}
                                    placeholder="enter your email"
                                    type="email"
                                />
                            </div>
                            {validationErrors && (
                                <span className={LoginorgStyles.errorMsg}>{validationErrors}</span>
                            )}
                            <div>
                                <button onClick={() => setClicked(true)} className={LoginorgStyles.loginBTN}>LOGIN</button>
                            </div>
                        </div>
                    </div>
                </>
            ) : (
                <>
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
                                            <select value={selectedOrganization} onChange={e => setSelectedOrganization(e.target.value)} className="selectDropdown">
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
                </>
            )}
        </>
    );
}

export default LoginOrg;