import { Link, useLocation } from "react-router-dom";
import { toast } from "react-toastify";
import { useEffect } from "react";
import { useSelector } from "react-redux";
import ArrowBackIosNewIcon from "@mui/icons-material/ArrowBackIosNew";
import PersonIcon from "@mui/icons-material/Person";
import OrderItem from "../../components/Order/OrderItem";
import {
    useDeleteSeatReservationMutation,
    useLazyGetAllSeatReservationQuery,
} from "../../services/seat";
import {
    useLazyGetAllOrderByUserIdQuery,
    useModifyOrderMutation,
} from "../../services/order";
import { useModifyPaymentMutation } from "../../services/payment";
import Toast from "../../components/Toast/Toast";
import OrderPagination from "../../components/Pagination/DefaultPagination";

export default function OrderPage() {
    const location = useLocation();
    const query = new URLSearchParams(location.search);
    const page = parseInt(query.get("page") || "1", 10);
    const [getAllSeats, { data: seats }] = useLazyGetAllSeatReservationQuery();
    const [getAllOrders, { data: order }] = useLazyGetAllOrderByUserIdQuery();
    const user = useSelector((state) => state.auth.user);
    const [cancelOrder, { isSuccess: cancelOrderSuccess }] =
        useModifyOrderMutation();
    const [cancelPayment, { isSuccess: cancelPaymentSuccess }] =
        useModifyPaymentMutation();
    const [updateOrder, { isSuccess: updateOrderSuccess }] =
        useModifyOrderMutation();
    const [updatePayment, { isSuccess: updatePaymentSuccess }] =
        useModifyPaymentMutation();
    const [returnSeat, { isSuccess: returnSeatSuccess }] =
        useDeleteSeatReservationMutation();
    const [deleteSeat, { isSuccess: deleteSeatSuccess }] =
        useDeleteSeatReservationMutation();

    function handleReturnTable(seatNumber) {
        returnSeat({ seatNumber });
    }
    function handleCancelOrder({ order, payment, seatNumber }) {
        cancelOrder({ ...order, status: "Canceled" });
        cancelPayment({ ...payment, status: "Canceled" });
        deleteSeat({ seatNumber });
    }
    function handlePayOrder({ order, payment }) {
        updateOrder({ ...order, status: "Success" });
        updatePayment({
            ...payment,
            status: "Success",
            paymentDate: new Date(),
        });
    }

    useEffect(() => {
        if (returnSeatSuccess) {
            toast.success("Return table successfully !", {
                position: toast.POSITION.BOTTOM_RIGHT,
            });
            setTimeout(() => {
                getAllSeats();
            }, 500);
        }
    }, [returnSeatSuccess, getAllSeats]);
    useEffect(() => {
        if (cancelOrderSuccess && cancelPaymentSuccess && deleteSeatSuccess) {
            toast.success("Cancel order successfully !", {
                position: toast.POSITION.BOTTOM_RIGHT,
            });
        }
    }, [cancelOrderSuccess, cancelPaymentSuccess, deleteSeatSuccess]);
    useEffect(() => {
        if (updateOrderSuccess && updatePaymentSuccess) {
            toast.success("Pay order successfully !", {
                position: toast.POSITION.BOTTOM_RIGHT,
            });
        }
    }, [updateOrderSuccess, updatePaymentSuccess]);
    useEffect(() => {
        if (
            (cancelOrderSuccess && cancelPaymentSuccess && deleteSeatSuccess) ||
            (updateOrderSuccess && updatePaymentSuccess)
        ) {
            setTimeout(() => {
                getAllOrders({ userId: user?.id, page, per_page: 10 });
            }, 500);
        }
    }, [
        cancelOrderSuccess,
        cancelPaymentSuccess,
        deleteSeatSuccess,
        updateOrderSuccess,
        updatePaymentSuccess,
        getAllOrders,
        page,
        user?.id,
    ]);
    useEffect(() => {
        getAllOrders({ userId: user?.id, page, per_page: 10 });
    }, [getAllOrders, page, user?.id]);
    useEffect(() => {
        getAllSeats();
    }, [getAllSeats]);

    return (
        <div className="min-h-[600px] px-5 py-8">
            <Link to="/" className="hover:underline text-primary">
                <ArrowBackIosNewIcon />
                BACK TO HOME PAGE
            </Link>
            <div className="rounded-lg mt-4 min-h-[500px]">
                <div className="flex items-center px-3 py-2 gap-1">
                    <PersonIcon />
                    MY ORDER
                </div>
                <div className="z-[1000] w-full h-[1px] bg-black/20" />
                <div className="p-5 overflow-x-scroll">
                    <table className="min-w-full text-center table-auto">
                        <thead className="border-b font-medium dark:border-neutral-500">
                            <tr>
                                <th scope="col" className="px-6 py-4">
                                    Tracking No.
                                </th>
                                <th scope="col" className="px-6 py-4">
                                    Order ID
                                </th>
                                <th scope="col" className="px-6 py-4">
                                    Order Date
                                </th>
                                <th scope="col" className="px-6 py-4">
                                    Payment Method
                                </th>
                                <th scope="col" className="px-6 py-4">
                                    Total Price
                                </th>
                                <th scope="col" className="px-6 py-4">
                                    Seat Number
                                </th>
                                <th scope="col" className="px-6 py-4">
                                    Status
                                </th>
                                <th scope="col" className="px-6 py-4">
                                    Action
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            {order?.data?.map((item, index) => (
                                <OrderItem
                                    key={index}
                                    item={item}
                                    seats={seats?.data}
                                    onReturnTable={handleReturnTable}
                                    onCancelOrder={handleCancelOrder}
                                    onPayOrder={handlePayOrder}
                                    t={(page - 1) * 10 + index + 1}
                                />
                            ))}
                        </tbody>
                    </table>
                </div>
                <div className="flex justify-center items-center mt-10">
                    <OrderPagination
                        total={order?.["total_pages"]}
                        page={page}
                    />
                </div>
            </div>
            <Toast />
        </div>
    );
}
