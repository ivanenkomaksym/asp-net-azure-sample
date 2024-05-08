import React from "react";

function Profile() {
    // Retrieve user information from local storage
    const userInfo = JSON.parse(localStorage.getItem('user_info'));

    return (
        <div>
            {/* Check if user information exists */}
            {userInfo && (
                <div>
                    <h2>User Profile</h2>
                    <p>First Name: {userInfo.firstName}</p>
                    <p>Last Name: {userInfo.lastName}</p>
                    <p>Email: {userInfo.email}</p>
                    <img src={userInfo.picture} alt="User Profile" />
                    <p>Identity Provider: {userInfo.identityProvider}</p>
                </div>
            )}
            {/* Display a message if user information is not available */}
            {!userInfo && (
                <div>
                    <p>No user information available.</p>
                </div>
            )}
        </div>
    );
}

export default Profile;
