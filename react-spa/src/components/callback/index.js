import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {useDispatch} from 'react-redux';
import { signinOrg } from '../../redux/actions/auth';
import { environments, redirectToEnvironment } from "../../api";
import CallbackStyles from "./Callback.module.css"

function Callback() {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [environmentList, setEnvironmentList] = useState([]);
  const [idToken, setIdToken] = useState('');

  useEffect(() => {
    console.log('Callback component mounted');
    const search = window.location.search;
    const query = new URLSearchParams(search);
    const idToken = query.get('id_token');
    const refreshToken = query.get('refresh_token');

    if (idToken) {
      setIdToken(idToken);
      // Handle successful login response
      console.log(`Organization login successful. idToken: ${idToken}\nrefreshToken: ${refreshToken}`);
      dispatch(signinOrg(idToken, refreshToken, navigate));
      
        const fetchEnvironments = async () => {
          try {
              const environmentsData = await environments(idToken);
              setEnvironmentList(environmentsData);
          } catch (error) {
              console.error('Error fetching environments:', error);
          }
      };

      fetchEnvironments();
    } else {
      // Handle error or invalid token
      console.error('ID token not found in the callback URL');
    }
  }, [navigate]);

  const handleButtonClick = async (id) => {
    try {
      await redirectToEnvironment(idToken, id);
  } catch (error) {
      console.error('Failed to redirect:', error);
  }
  };

  return (        
      <div className={CallbackStyles.container}>
          Select environment
          <div className={CallbackStyles.buttonContainer}>
                {environmentList.map(env => (
                    <button key={env.id} onClick={() => handleButtonClick(env.id)} className={CallbackStyles.button}>
                        {env.description}
                    </button>
                ))}
          </div>
      </div>
  );
};

export default Callback;
