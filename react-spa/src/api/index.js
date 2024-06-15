import axios from "axios"

import { loginUrl, registerUrl, weatherForecastUrl } from "../authConfig";

export async function signIn(data) {
    const instance = axios.create({
        headers: {
            'Access-Control-Allow-Origin': '*',
            'Content-Type': 'application/json',
            mode: 'no-cors'
        },
        withCredentials: true,
    });

    const response = await instance.post(`${loginUrl}?useCookies=true`, data);
    return response.data;
}

export async function signUp(data) {
    const instance = axios.create({
        headers: {
            'Access-Control-Allow-Origin': '*',
            'Content-Type': 'application/json',
            mode: 'no-cors'
        }
    });

    const response = await instance.post(registerUrl. data);
    return response.data;
}

export async function weatherForecast(accessToken) {
    const instance = axios.create({
        headers: {
            'Authorization': `Bearer ${accessToken}`,
            'Access-Control-Allow-Origin': '*',
            'Content-Type': 'application/json',
            mode: 'no-cors'
        }
    });

    const response = await instance.get(weatherForecastUrl);
    return response.data;
}

export async function weatherForecastWithCookies() {
    const instance = axios.create({
        headers: {
            'Access-Control-Allow-Origin': '*',
            'Content-Type': 'application/json',
            mode: 'no-cors'
        },
        withCredentials: true,
    });

    const response = await instance.get(weatherForecastUrl);
    return response.data;
}