import React from 'react';
import { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom'
import * as api from "../../api/index"

import { PublicClientApplication, InteractionRequiredAuthError } from '@azure/msal-browser';
import { weatherForecastTokenRequest, msalConfig } from '../../authConfig';

function Home() {
    const [userData, setUserData] = useState(null);
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

        request.account = myMSALObj.setActiveAccount(username);

        console.log("request: ", JSON.stringify(request, null, 2));

        return myMSALObj.acquireTokenSilent(request)
            .catch(error => {
                console.warn("silent token acquisition fails. acquiring token using popup");
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
                }
            });
    }

    async function weatherForecast() {
        return await getTokenPopup(weatherForecastTokenRequest)
            .then(response => {
                api.weatherForecast(response.accessToken).then((result) => {
                    console.log("data: ", JSON.stringify(result, null, 2));
                    setWeatherData(result);
                });
            }).catch(error => {
                console.error(error);
            });
    }

    async function handleGetWeather() {
        try {
            weatherForecast();
        } catch (error) {
            console.error("Error fetching weather data:", error);
        }
    };

    return (
        <div>
            {userData && (
                <button onClick={handleGetWeather} className="getWeatherButton">
                    Get Weather
                </button>
            )}
            {weatherData && (
                <div className="weatherData">
                    <h2>Weather Forecast</h2>
                    {weatherData.map((forecast, index) => (
                        <div key={index} className="forecastItem">
                            <p>Date: {forecast.date}</p>
                            <p>Temperature: {forecast.temperatureC}°C ({forecast.temperatureF}°F)</p>
                            <p>Summary: {forecast.summary}</p>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}

export default Home;
