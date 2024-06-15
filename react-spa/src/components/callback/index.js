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
    console.log("query" + query);

    if (idToken) {
      // Handle successful login response
      console.log("Organization login successful:", idToken);
      dispatch(signinOrg(idToken, navigate));
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
