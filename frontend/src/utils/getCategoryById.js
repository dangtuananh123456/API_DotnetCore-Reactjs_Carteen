import axios from "axios";

const BASE_URL = "https://localhost:7190";

export default async function getCategoryById({ params, request }) {
    const { id } = params;
    const res = await axios.get(`${BASE_URL}/api/Categories/${id}`, {
        signal: request.signal,
    });
    if (res.status !== 200) {
        throw new Error("Fetching category failed");
    }
    return res.data;
}
