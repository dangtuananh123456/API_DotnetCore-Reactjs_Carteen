import axios from "axios";
import { useState } from "react";
import { useEffect } from "react";

const BASE_URL = "https://localhost:7190";

export default function useTopSales() {
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);
    const [data, setData] = useState([]);

    useEffect(() => {
        async function fetchTopSaleProducts() {
            try {
                setIsLoading(true);
                const res = await axios.get(`${BASE_URL}/api/Items/top5`);
                if (res.status !== 200) {
                    throw new Error("Fetching top products failed");
                }
                setData(res.data);
            } catch (error) {
                setError(error.message);
            } finally {
                setIsLoading(false);
            }
        }
        fetchTopSaleProducts();
    }, []);

    return { isLoading, error, data };
}
