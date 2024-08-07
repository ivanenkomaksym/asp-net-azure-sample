import React from 'react';
import PortalStyles from "./Portal.module.css"

function Portal() {
  return (
      <div className={PortalStyles.container}>
        <h1>Customer Portal</h1>
        <button className={PortalStyles.profileButton} onClick={() => window.location.assign('/account/profile')}>Go to Profile</button>
      </div>
  );
};

export default Portal;
