import React, { useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';
import { refreshTokenUrl } from "../../authConfig";
import ProfileStyles from "./Profile.module.css"

function Profile() {
  const [userInfo, setUserInfo] = useState(null);
  const dispatch = useDispatch();

  // Retrieve user information from local storage
  useEffect(() => {
    const userInfoFromLocalStorage = JSON.parse(localStorage.getItem('user_info'));
    setUserInfo(userInfoFromLocalStorage);
  }, []);

  const handleRefreshToken = async () => {
    try {
      // Make HTTP request to refresh token
      const response = await fetch(`${refreshTokenUrl}?email=${userInfo.email}&refresh_token=${userInfo.refresh_token}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        }
      });

      if (!response.ok) {
        throw new Error('Failed to refresh token');
      }

      const data = await response.json();
      const { id_token } = data; // Assuming the response includes id_token

      // Update userInfo with new id_token
      const updatedUserInfo = { ...userInfo, id_token };
      setUserInfo(updatedUserInfo);

      // Dispatch an action to update Redux state
      dispatch({ type: 'AUTH', data: updatedUserInfo });

    } catch (error) {
      console.error('Error refreshing token:', error);
    }
  };

  return (
    <div className={ProfileStyles.profileContainer}>
      {userInfo ? (
        <div>
          <h2>User Profile</h2>
          <table className={ProfileStyles.profileTable}>
            <tbody>
              <tr>
                <td>First Name:</td>
                <td>{userInfo.firstName}</td>
              </tr>
              <tr>
                <td>Last Name:</td>
                <td>{userInfo.lastName}</td>
              </tr>
              <tr>
                <td>Email:</td>
                <td>{userInfo.email}</td>
              </tr>
              <tr>
                <td>Profile Picture:</td>
                <td><img src={userInfo.picture} alt="User Profile" className={ProfileStyles.profilePicture} /></td>
              </tr>
              <tr>
                <td>Identity Provider:</td>
                <td>{userInfo.identityProvider}</td>
              </tr>
              <tr>
                <td>Id token:</td>
                <td className={ProfileStyles.tokenCell}>{userInfo.id_token}</td>
              </tr>
              <tr>
                <td>Refresh token:</td>
                <td className={ProfileStyles.tokenCell}>{userInfo.refresh_token}</td>
              </tr>
            </tbody>
          </table>
          <br/>
          { userInfo.refresh_token ? 
            (
              <button onClick={handleRefreshToken} className={ProfileStyles.linkBTN}>
                Refresh token
              </button>
            )
            : <p/>
          }
        </div>
      ) : (
        <div>
          <p>No user information available.</p>
        </div>
      )}
    </div>
  );
}

export default Profile;
