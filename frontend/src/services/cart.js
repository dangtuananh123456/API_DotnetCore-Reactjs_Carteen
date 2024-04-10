import { createApi } from "@reduxjs/toolkit/query/react";
import { axiosBaseQuery } from "../api/axiosBaseQuery";

const baseQuery = "https://localhost:7190";

const cartApi = createApi({
    reducerPath: "cartApi",
    baseQuery: axiosBaseQuery({
        baseUrl: baseQuery,
    }),
    endpoints: (builder) => ({
        addCartItems: builder.mutation({
            query: (payload) => ({
                url: "/api/CartItem",
                method: "POST",
                body: payload,
            }),
        }),
        modifyCartItems: builder.mutation({
            query: (payload) => ({
                url: "/api/CartItem",
                method: "PUT",
                body: payload,
            }),
        }),
        deleteCartItems: builder.mutation({
            query: ({ id }) => ({
                url: `/api/CartItem/${id}`,
                method: "DELETE",
            }),
        }),
    }),
});

export const {
    useAddCartItemsMutation,
    useModifyCartItemsMutation,
    useDeleteCartItemsMutation,
} = cartApi;

export default cartApi;
