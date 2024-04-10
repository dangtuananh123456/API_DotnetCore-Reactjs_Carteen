import { createApi } from "@reduxjs/toolkit/query/react";
import { axiosBaseQuery } from "../api/axiosBaseQuery";

const baseQuery = "https://localhost:7190";

const privateAuthApi = createApi({
    reducerPath: "privateAuthApi",
    baseQuery: axiosBaseQuery({
        baseUrl: baseQuery,
    }),
    endpoints: (builder) => ({
        getAllUser: builder.query({
            query: () => ({
                url: "/api/User",
            }),
        }),
        addUser: builder.mutation({
            query: (payload) => ({
                url: "/register",
                method: "POST",
                body: payload,
            }),
        }),
        modifyUser: builder.mutation({
            query: (payload) => ({
                url: `/api/User/${payload.id}`,
                method: "PUT",
                body: payload,
            }),
        }),
        deleteUser: builder.mutation({
            query: (payload) => ({
                url: `/api/User/${payload.id}`,
                method: "DELETE",
            }),
        }),
        logout: builder.mutation({
            query: () => ({
                url: "/logout",
                method: "POST",
                options: {
                    withCredentials: true,
                },
            }),
        }),
    }),
});

export const {
    useLogoutMutation,
    useLazyGetAllUserQuery,
    useGetAllUserQuery,
    useAddUserMutation,
    useModifyUserMutation,
    useDeleteUserMutation,
} = privateAuthApi;
export default privateAuthApi;
