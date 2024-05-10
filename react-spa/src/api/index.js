import axios from "axios"

export async function weatherForecast(accessToken) {
    const instance = axios.create({
        baseURL: "http://localhost:5000",
        headers: {
            'Authorization': `Bearer ${accessToken}`
        }
    });

    const response = await instance.get("/WeatherForecast");
    return response.data;
}