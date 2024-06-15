import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

function Callback() {
  const navigate = useNavigate();

  useEffect(() => {
    console.log('Callback component mounted');
    const hash = window.location.hash;
    const query = new URLSearchParams(hash.substring(1));
    const idToken = query.get('id_token');

    if (idToken) {
      // Save the token (e.g., to localStorage)
      localStorage.setItem('id_token', idToken);

      // Redirect to the main page
      navigate.push('/');
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
