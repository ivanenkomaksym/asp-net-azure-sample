import axios from "axios"

export async function signIn(data) {
    // Set withCredentials to true globally
    axios.defaults.withCredentials = true;

    const instance = axios.create({
        baseURL: "http://localhost:5000/",
        headers: {
            'Access-Control-Allow-Origin': '*',
            'Content-Type': 'application/json',
            withCredentials: true,
            mode: 'no-cors'
        }
    });

    const response = await instance.post("/login?useCookies=true", data);
    return response.data;
}

export async function weatherForecast(accessToken) {
    const instance = axios.create({
        baseURL: "http://localhost:5000/",
        headers: {
            'Authorization': `Bearer ${accessToken}`,
            'Access-Control-Allow-Origin': '*',
            'Content-Type': 'application/json',
            withCredentials: true,
            mode: 'no-cors'
        }
    });

    const response = await instance.get("/WeatherForecast");
    return response.data;
}

export async function weatherForecastWithCookies() {
    const instance = axios.create({
        baseURL: "http://localhost:5000/",
        headers: {
            'Access-Control-Allow-Origin': '*',
            'Content-Type': 'application/json',
            withCredentials: true,
            mode: 'no-cors'
        }
    });

    const response = await instance.get("/WeatherForecast");
    return response.data;
}