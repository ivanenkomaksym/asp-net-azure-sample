import React from 'react';

function Profile() {
  // Retrieve user information from local storage
  const userInfo = JSON.parse(localStorage.getItem('user_info'));

  return (
    <div className="profile-container">
      {userInfo ? (
        <div>
          <h2>User Profile</h2>
          <table className="profile-table">
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
                <td><img src={userInfo.picture} alt="User Profile" className="profile-picture" /></td>
              </tr>
              <tr>
                <td>Identity Provider:</td>
                <td>{userInfo.identityProvider}</td>
              </tr>
            </tbody>
          </table>
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
