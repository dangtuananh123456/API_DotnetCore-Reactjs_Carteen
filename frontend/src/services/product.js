import { createApi } from "@reduxjs/toolkit/query/react";
import { axiosBaseQuery } from "../api/axiosBaseQuery";

const baseQuery = "https://localhost:7190";

const productApi = createApi({
    reducerPath: "productApi",
    baseQuery: axiosBaseQuery({
        baseUrl: baseQuery,
    }),
    endpoints: (builder) => ({
        getAllProduct: builder.query({
            query: () => ({
                url: "/api/Items",
                method: "GET",
            }),
        }),
        getTopSales: builder.query({
            query: () => ({
                url: "/api/Items/top5",
                method: "GET",
            }),
            keepUnusedDataFor: 10,
        }),
        addProduct: builder.mutation({
            query: (payload) => ({
                url: "/api/Items",
                method: "POST",
                body: payload,
            }),
        }),
        modifyProduct: builder.mutation({
            query: (payload) => ({
                url: `/api/Items/${payload.id}`,
                method: "PUT",
                body: payload,
            }),
        }),
        deleteProduct: builder.mutation({
            query: ({ id }) => ({
                url: `/api/Items/${id}`,
                method: "DELETE",
            }),
        }),
    }),
});

export const {
    useGetTopSalesQuery,
    useLazyGetAllProductQuery,
    useAddProductMutation,
    useModifyProductMutation,
    useDeleteProductMutation,
} = productApi;

export default productApi;
