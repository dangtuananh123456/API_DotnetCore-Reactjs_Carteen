import axios from "axios";

const BASE_URL = "https://localhost:7190";

export default async function getProductById({ params, request }) {
    const { id } = params;
    const res = await axios.get(`${BASE_URL}/api/Items/${id}`, {
        signal: request.signal,
    });
    if (res.status !== 200) {
        throw new Error("Fetching product failed");
    }
    return res.data;
}
