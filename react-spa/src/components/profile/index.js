import React, { useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';
import { refreshIdToken } from '../../api';
import ProfileStyles from "./Profile.module.css"
import { loadUserFromLocalStorage } from '../../redux/reducers/auth';

function Profile() {
  const [userInfo, setUserInfo] = useState(null);
  const dispatch = useDispatch();

  // Retrieve user information from local storage
  useEffect(() => {
    const userInfoFromLocalStorage = loadUserFromLocalStorage()
    setUserInfo(userInfoFromLocalStorage);
  }, []);

  const handleRefreshToken = async () => {
    const { id_token } = await refreshIdToken(userInfo.email, userInfo.refresh_token, userInfo.organization);

    // Update userInfo with new id_token
    const updatedUserInfo = { ...userInfo, id_token };
    setUserInfo(updatedUserInfo);

    // Dispatch an action to update Redux state
    dispatch({ type: 'AUTH', data: updatedUserInfo });
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
                <td>Organization:</td>
                <td>{userInfo.organization}</td>
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
          <br />
          {userInfo.refresh_token ?
            (
              <button onClick={handleRefreshToken} className={ProfileStyles.linkBTN}>
                Refresh token
              </button>
            )
            : <p />
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
