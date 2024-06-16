import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {useDispatch} from 'react-redux';
import { signinOrg } from '../../redux/actions/auth';

function Callback() {
  const navigate = useNavigate();
  const dispatch = useDispatch();

  useEffect(() => {
    console.log('Callback component mounted');
    const search = window.location.search;
    const query = new URLSearchParams(search);
    const idToken = query.get('id_token');
    const refreshToken = query.get('refresh_token');

    if (idToken) {
      // Handle successful login response
      console.log(`Organization login successful. idToken: ${idToken}\nrefreshToken: ${refreshToken}`);
      dispatch(signinOrg(idToken, refreshToken, navigate));
    } else {
      // Handle error or invalid token
      console.error('ID token not found in the callback URL');
    }
  }, [navigate]);

  return (
    <div>
      <h1>Logging in...</h1>
    </div>
  );
};

export default Callback;
