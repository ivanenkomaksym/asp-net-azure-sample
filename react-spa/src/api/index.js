import axios from "axios"

import { loginUrl, registerUrl, organizationsUrl, refreshTokenUrl, weatherForecastUrl } from "../authConfig";

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

    const response = await instance.post(registerUrl.data);
    return response.data;
}

// Function to fetch domains data
export async function queryDomains() {
    try {
        const response = await fetch(`${organizationsUrl}`);
        if (!response.ok) {
            throw new Error('Failed to fetch domains');
        }
        return await response.json();
    } catch (error) {
        console.error('Error fetching domains:', error);
    }
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

export async function refreshIdToken(email, refreshToken) {
    try {
        const response = await fetch(`${refreshTokenUrl}?email=${email}&refresh_token=${refreshToken}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            }
        });

        if (!response.ok) {
            throw new Error('Failed to refresh token');
        }

        return await response.json();
    } catch (error) {
        console.error('Error refreshing token:', error);
    }
}