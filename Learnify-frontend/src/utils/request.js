import axios from "axios";

const request = axios.create({
    baseURL: "https://localhost:7134/api",
});
request.interceptors.request.use(config => {
    const token = localStorage.getItem("jwtToken");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export default request;
