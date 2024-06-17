import React, { useState, useEffect } from "react";
import LoginStyles from "./Login.module.css"
import { organizationsUrl, loginOrgAuthorizeUrl } from "../../authConfig";

function LoginOrg() {
    const [domains, setDomains] = useState([]);

    const [email, setEmail] = useState("");
    const [selectedOption, setSelectedOption] = useState('');

    useEffect(() => {
        // Function to fetch domains data
        const fetchDomains = async () => {
          try {
            const response = await fetch(`${organizationsUrl}`);
            if (!response.ok) {
              throw new Error('Failed to fetch domains');
            }
            const data = await response.json();
            setDomains(data);
          } catch (error) {
            console.error('Error fetching domains:', error);
          }
        };
    
        fetchDomains();
      }, []); // Empty dependency array ensures useEffect runs only once on component mount

    const [clicked, setClicked] = useState(false);
    useEffect(() => {
        if (clicked) {
            console.log(`email=${email}&organization=${selectedOption}`);
            window.location.assign(`${loginOrgAuthorizeUrl}?email=${email}&organization=${selectedOption}`);
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
                    {/* Default "autodetect" option */}
                    <option value="">Autodetect</option>
                    {domains.map(domain => (
                        <option key={domain.tenantId} value={domain.tenantId}>{domain.name}</option>
                    ))}
                    </select>
                </div>

                <button onClick={() => setClicked(true)} className={LoginStyles.loginBTN}>LOGIN</button>
            </div>
        </div>
    );
}

export default LoginOrg;