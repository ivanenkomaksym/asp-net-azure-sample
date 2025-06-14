import { loadUserFromLocalStorage } from '../../redux/reducers/auth';
import { useAuth0 } from '@auth0/auth0-react';
import { useMsal } from '@azure/msal-react';

function Logout() {
    const { logout: auth0Logout } = useAuth0();
    const { instance: msalInstance } = useMsal();

    const logoutUser = () => {
        const userInfoFromLocalStorage = loadUserFromLocalStorage();

        console.log("[Logout] userInfoFromLocalStorage", userInfoFromLocalStorage);

        if (!userInfoFromLocalStorage) {
            console.log("[Logout] No user found in localStorage");
            return;
        }

        const { identityProvider } = userInfoFromLocalStorage;

        switch (identityProvider) {
            case "Auth0":
                console.log("[Logout] Auth0 logout");
                auth0Logout({ 
                    logoutParams: {
                        returnTo: window.location.origin
                    }
                });
                break;
            
            case "Microsoft":
                console.log("[Logout] Microsoft logout");
                msalInstance.logoutRedirect({
                    postLogoutRedirectUri: window.location.origin
                });
                break;
            
            case "Google":
            case "Organization":
            default:
                console.log("[Logout] Standard logout for", identityProvider);
                // For Google, Organization, and other providers, just clear localStorage
                // The Redux LOGOUT action will handle this
                break;
        }
    };

    return { logoutUser };
}

export default Logout; 