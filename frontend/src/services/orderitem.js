import { createApi } from "@reduxjs/toolkit/query/react";
import { axiosBaseQuery } from "../api/axiosBaseQuery";

const baseQuery = "https://localhost:7190";

const orderItemApi = createApi({
    reducerPath: "orderItemApi",
    baseQuery: axiosBaseQuery({
        baseUrl: baseQuery,
    }),
    endpoints: (builder) => ({
        addOrderItems: builder.mutation({
            query: (payload) => ({
                url: "/api/OrderItem",
                method: "POST",
                body: payload,
            }),
        }),
    }),
});

export const { useAddOrderItemsMutation } = orderItemApi;

export default orderItemApi;
