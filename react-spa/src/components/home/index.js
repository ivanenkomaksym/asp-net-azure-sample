import React from 'react';
import { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom'
import * as api from "../../api/index"
import { Link  } from 'react-router-dom';
import HomeStyles from "./Home.module.css"

import { PublicClientApplication, InteractionRequiredAuthError } from '@azure/msal-browser';
import { weatherForecastTokenRequest, msalConfig } from '../../authConfig';
import { MICROSOFT_IP, GOOGLE_IP, ORGANIZATION } from '../../redux/const/actionsTypes';

function Home() {
    const [userData, setUserData] = useState(null);
    const [errorMessage, setError] = useState(null);
    const [weatherData, setWeatherData] = useState(null);
    const location = useLocation();

    useEffect(() => {
        // Assuming loadUser is a function that loads user data
        const localUser = JSON.parse(localStorage.getItem("user_info"));

        console.log("localUser: ", localUser);
        if (localUser) {
            setUserData(localUser);
        } else {
            setUserData(null);
        }
    }, [location]);

    async function getTokenPopup(request) {
        const myMSALObj = new PublicClientApplication(msalConfig);
        await myMSALObj.initialize();

        let username = "";
        const currentAccounts = myMSALObj.getAllAccounts();
        if (currentAccounts.length === 0) {
            return;
        } else if (currentAccounts.length > 1) {
            // Add choose account code here
            console.warn("Multiple accounts detected.");
        } else if (currentAccounts.length === 1) {
            username = currentAccounts[0].username;
        }

        request.account = currentAccounts[0];

        console.log("request: ", JSON.stringify(request, null, 2));

        return myMSALObj.acquireTokenSilent(request)
            .catch(error => {
                console.warn("silent token acquisition fails. acquiring token using popup, error: ", error);
                if (error instanceof InteractionRequiredAuthError) {
                    // fallback to interaction when silent call fails
                    return myMSALObj.acquireTokenPopup(request)
                        .then(tokenResponse => {
                            console.log(tokenResponse);
                            return tokenResponse;
                        }).catch(error => {
                            console.error(error);
                        });
                } else {
                    console.warn(error);
                    setError(error.message);
                }
            });
    }

    async function weatherForecast() {
        if (userData.identityProvider == MICROSOFT_IP) {
            return await getTokenPopup(weatherForecastTokenRequest)
                .then(response => {
                    api.weatherForecast(response.accessToken).then((result) => {
                        console.log("data: ", JSON.stringify(result, null, 2));
                        setWeatherData(result);
                    }).catch(error => {
                        console.error(error);
                        setError(error.message);
                    });
                }).catch(error => {
                    console.error(error);
                    setError(error.message);
                });
        } else if (userData.identityProvider == GOOGLE_IP ||
                   userData.identityProvider == ORGANIZATION) {
            api.weatherForecast(userData.id_token).then((result) => {
                console.log("data: ", JSON.stringify(result, null, 2));
                setWeatherData(result);
            }).catch(error => {
                console.error(error);
                setError(error.message);
            });
        } else {
            api.weatherForecastWithCookies().then((result) => {
                console.log("data: ", JSON.stringify(result, null, 2));
                setWeatherData(result);
            }).catch(error => {
                console.error(error);
                setError(error.message);
            });
        }
    }

    async function handleGetWeather() {
        try {
            weatherForecast();
        } catch (error) {
            console.error("Error fetching weather data:", error);
            setError(error.message);
        }
    };

    const checkTokenValidity = () => {
        const userInfo = JSON.parse(localStorage.getItem('user_info'));
        if (userInfo) {
            const { expirationDate } = userInfo;
            if (new Date(expirationDate) < new Date()) {
                console.log("Clearing local storage due to token expiration");
                localStorage.clear();
                return false;
            }
            return true;
        }
        return false;
    };

    return (
        <div className={HomeStyles.container}>
            {checkTokenValidity() ? (
                <>
                    {userData && (
                        <button onClick={handleGetWeather} className={HomeStyles.getWeatherButton}>
                            Get Weather
                        </button>
                    )}
                    {userData && weatherData && (
                        <div className={HomeStyles.weatherData}>
                            <h2 className={HomeStyles.weatherTitle}>Weather Forecast</h2>
                            <table className={HomeStyles.forecastTable}>
                                <thead>
                                    <tr>
                                        <th>Date</th>
                                        <th>Temperature (°C)</th>
                                        <th>Temperature (°F)</th>
                                        <th>Summary</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {weatherData.map((forecast, index) => (
                                        <tr key={index}>
                                            <td>{forecast.date}</td>
                                            <td>{forecast.temperatureC}</td>
                                            <td>{forecast.temperatureF}</td>
                                            <td>{forecast.summary}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                    {errorMessage && (
                        <div className={HomeStyles.errorContainer}>
                            <p className={HomeStyles.errorMessage}>{errorMessage}</p>
                        </div>
                    )}
                </>
            ) : (
                <div className={HomeStyles.welcomeMessage}>
                    Welcome. <Link to="/account/login">Login</Link> to continue.
                </div>
            )}
        </div>
    );
}

export default Home;
