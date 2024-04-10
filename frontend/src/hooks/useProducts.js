import axios from "axios";
import { useState } from "react";
import { useEffect } from "react";

const BASE_URL = "https://localhost:7190";

export default function useProducts(
    { page, per_page, keyword, categoryId } = {
        page: 1,
        per_page: 10,
        keyword: "",
        categoryId: 0,
    }
) {
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState(null);
    const [data, setData] = useState([]);

    useEffect(() => {
        async function fetchProducts() {
            try {
                setIsLoading(true);
                const res = await axios.get(
                    `${BASE_URL}/api/Items?page=${page}&per_page=${per_page}&keyword=${keyword}&categoryId=${categoryId}`
                );
                if (res.status !== 200) {
                    throw new Error("Fetching products failed");
                }
                setData(res.data);
            } catch (error) {
                setError(error.message);
            } finally {
                setIsLoading(false);
            }
        }
        fetchProducts();
    }, [page, per_page, keyword, categoryId]);

    return { isLoading, error, data };
}
