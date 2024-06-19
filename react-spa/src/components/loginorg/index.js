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
        setEmailSet(!emailSet);
      };

    useEffect(() => {
        const getDomains = async () => {
          try {
            const data = await queryDomains(email);
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

                <div className={LoginorgStyles.inputContainer}>
                    <label>EMAIL</label>
                    <input onChange={e => setEmail(e.target.value)} placeholder="enter your email" type="email" />
                </div>

                {emailSet ? (
                <><div className="dropdownContainer">
                        <label>ORGANIZATION</label>
                        <select value={selectedOption} onChange={e => setSelectedOption(e.target.value)} className="selectDropdown">
                            {domains.map(domain => (
                                <option key={domain.id} value={domain.id}>{domain.description}</option>
                            ))}
                        </select>
                    </div><button onClick={() => setClicked(true)} className={LoginorgStyles.loginBTN}>LOGIN</button></>
                ) :
                <button onClick={handleEmailSet} className={LoginorgStyles.loginBTN}>Continue</button>
                }
            </div>
        </div>
    );
}

export default LoginOrg;