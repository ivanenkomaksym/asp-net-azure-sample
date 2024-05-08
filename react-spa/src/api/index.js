import axios from "axios"

const API = axios.create({baseURL:"http://localhost:5000"})

API.interceptors.request.use((req)=>{
    if(localStorage.getItem("user_info")){
        req.headers.Authorization = ``
    }

    return req;
})

export const weatherForecast = () => API.get("/WeatherForecast")