import React from 'react';
import { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom'
import * as api from "../../api/index"

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

    async function handleGetWeather() {
        try {
            const response = await api.weatherForecast()
            console.log("data: ", JSON.stringify(response.data, null, 2));
            setWeatherData(response.data);
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
