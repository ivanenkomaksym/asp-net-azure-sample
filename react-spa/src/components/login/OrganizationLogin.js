import React from "react";
import { useNavigate } from "react-router-dom";
import LoginStyles from "./Login.module.css";

function OrganizationLogin() {
    const navigate = useNavigate();

    const handleLoginOrgClick = () => {
        navigate('/account/loginorg');
    };

    return (
        <button className={LoginStyles.organizationBTN} onClick={handleLoginOrgClick}>
            Sign in with organization
        </button>
    );
}

export default OrganizationLogin; 