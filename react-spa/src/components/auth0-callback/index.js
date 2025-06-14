import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useDispatch } from 'react-redux';
import { useAuth0 } from '@auth0/auth0-react';
import { signinAuth0 } from '../../redux/actions/auth';
import { auth0Config } from '../../authConfig';
import Auth0CallbackStyles from './Auth0Callback.module.css';

function Auth0Callback() {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { user, getAccessTokenSilently, isAuthenticated, isLoading, error } = useAuth0();

  useEffect(() => {
    console.log('Auth0 Callback component mounted');
    
    if (isLoading) {
      console.log('Auth0 is loading...');
      return;
    }

    if (error) {
      console.error('Auth0 error:', error);
      navigate('/account/login');
      return;
    }

    if (isAuthenticated && user) {
      console.log('Auth0 user authenticated:', user);
      
      // Get the access token
      getAccessTokenSilently({
        authorizationParams: {
          audience: auth0Config.audience
        }
      })
      .then(token => {
        console.log('Auth0 token obtained:', token);
        // Dispatch the Redux action with user and token
        dispatch(signinAuth0(user, token, navigate));
      })
      .catch(tokenError => {
        console.error('Failed to get Auth0 token:', tokenError);
        navigate('/account/login');
      });
    } else if (!isLoading) {
      // Not authenticated and not loading, redirect to login
      console.log('Not authenticated, redirecting to login');
      navigate('/account/login');
    }
  }, [isAuthenticated, isLoading, error, user, getAccessTokenSilently, dispatch, navigate]);

  if (isLoading) {
    return (
      <div className={Auth0CallbackStyles.container}>
        <div className={Auth0CallbackStyles.loading}>
          <h2>Authenticating...</h2>
          <p>Please wait while we complete your authentication.</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={Auth0CallbackStyles.container}>
        <div className={Auth0CallbackStyles.error}>
          <h2>Authentication Error</h2>
          <p>{error.message}</p>
          <button onClick={() => navigate('/account/login')}>
            Return to Login
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className={Auth0CallbackStyles.container}>
      <div className={Auth0CallbackStyles.success}>
        <h2>Authentication Successful</h2>
        <p>Welcome, {user?.name || user?.email}!</p>
        <p>Redirecting you to the home page...</p>
      </div>
    </div>
  );
}

export default Auth0Callback; 