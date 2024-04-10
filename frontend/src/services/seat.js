import { createApi } from "@reduxjs/toolkit/query/react";
import { axiosBaseQuery } from "../api/axiosBaseQuery";

const baseQuery = "https://localhost:7190";

const seatApi = createApi({
    reducerPath: "seatApi",
    baseQuery: axiosBaseQuery({
        baseUrl: baseQuery,
    }),
    endpoints: (builder) => ({
        getAllSeatReservation: builder.query({
            query: () => ({
                url: "/api/SeatReservation",
                method: "GET",
            }),
        }),
        addSeatReservation: builder.mutation({
            query: (payload) => ({
                url: "/api/SeatReservation",
                method: "POST",
                body: payload,
            }),
        }),
        deleteSeatReservation: builder.mutation({
            query: (payload) => ({
                url: `/api/SeatReservation?seatNumber=${payload.seatNumber}`,
                method: "DELETE",
            }),
        }),
    }),
});

export const {
    useLazyGetAllSeatReservationQuery,
    useAddSeatReservationMutation,
    useDeleteSeatReservationMutation,
} = seatApi;

export default seatApi;
