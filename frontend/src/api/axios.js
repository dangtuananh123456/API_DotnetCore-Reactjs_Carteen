import axios from "axios";
import { updateAccessToken } from "../features/auth/authSlice";

let store;
const BASE_URL = "https://localhost:7190";

export const injectStore = (_store) => {
    store = _store;
};

const refreshToken = () => {
    return axios.get(`${BASE_URL}/refresh-token`, {
        withCredentials: true,
    });
};

const createAxiosInstance = () => {
    const accessToken = store.getState().auth.accessToken;
    const axiosInstance = axios.create({
        baseURL: BASE_URL,
        headers: {
            Authorization: `Bearer ${accessToken}`,
        },
    });
    axiosInstance.interceptors.request.use(
        (config) => {
            return config;
        },
        (error) => {
            return Promise.reject(error);
        }
    );
    axiosInstance.interceptors.response.use(
        (response) => {
            return response;
        },
        async (error) => {
            if (error.response && error.response.status === 401) {
                const res = await refreshToken();
                const { accessToken: newAccessToken } = res.data;
                store.dispatch(updateAccessToken({ newAccessToken }));
                const originalRequest = error.config;
                originalRequest.headers[
                    "Authorization"
                ] = `Bearer ${newAccessToken}`;
                return axios(originalRequest);
            }
            return Promise.reject(error);
        }
    );
    return axiosInstance;
};

export default createAxiosInstance;
