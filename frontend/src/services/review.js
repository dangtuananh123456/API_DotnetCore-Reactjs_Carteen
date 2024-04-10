import { createApi } from "@reduxjs/toolkit/query/react";
import { axiosBaseQuery } from "../api/axiosBaseQuery";

const baseQuery = "https://localhost:7190";

const reviewApi = createApi({
    reducerPath: "reviewApi",
    baseQuery: axiosBaseQuery({
        baseUrl: baseQuery,
    }),
    endpoints: (builder) => ({
        addReview: builder.mutation({
            query: (payload) => ({
                url: "/api/Review",
                method: "POST",
                body: payload,
            }),
        }),
    }),
});

export const { useAddReviewMutation } = reviewApi;

export default reviewApi;
