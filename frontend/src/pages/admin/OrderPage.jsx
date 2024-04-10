import { toast } from "react-toastify";
import { useEffect } from "react";
import CancelIcon from "@mui/icons-material/Cancel";
import KeyboardReturnIcon from "@mui/icons-material/KeyboardReturn";
import { IconButton, Tooltip } from "@mui/material";
import {
    useDeleteSeatReservationMutation,
    useLazyGetAllSeatReservationQuery,
} from "../../services/seat";
import {
    useLazyGetAllOrderQuery,
    useModifyOrderMutation,
} from "../../services/order";
import {
    useGetAllPaymentQuery,
    useModifyPaymentMutation,
} from "../../services/payment";
import { useGetAllUserQuery } from "../../services/privateAuth";
import {
    formatDate,
    isSeatReturned,
    getPaymentByOrderId,
    getUserByOrder,
} from "../../utils";
import Toast from "../../components/Toast/Toast";
import AdminTable from "../../components/Table/Table";
import Status from "../../components/Status/Status";

export default function OrderPage() {
    const { data: payments } = useGetAllPaymentQuery();
    const { data: users } = useGetAllUserQuery();
    const [getAllSeats, { data: seats }] = useLazyGetAllSeatReservationQuery();
    const [getAllOrders, { data: orders }] = useLazyGetAllOrderQuery();
    const [cancelOrder, { isSuccess: cancelOrderSuccess }] =
        useModifyOrderMutation();
    const [cancelPayment, { isSuccess: cancelPaymentSuccess }] =
        useModifyPaymentMutation();
    const [deleteSeat, { isSuccess: deleteSeatSuccess }] =
        useDeleteSeatReservationMutation();
    const [returnSeat, { isSuccess: returnSeatSuccess }] =
        useDeleteSeatReservationMutation();

    function handleReturnTable(seatNumber) {
        returnSeat({ seatNumber });
    }
    function handleCancelOrder(order) {
        const payment = getPaymentByOrderId(payments?.data, order.id);
        cancelOrder({ ...order, status: "Canceled" });
        cancelPayment({ ...payment, status: "Canceled" });
        deleteSeat({ seatNumber: order.seatNumber });
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
            setTimeout(() => {
                getAllOrders();
            }, 500);
        }
    }, [
        cancelOrderSuccess,
        cancelPaymentSuccess,
        deleteSeatSuccess,
        getAllOrders,
    ]);
    useEffect(() => {
        getAllSeats();
    }, [getAllSeats]);
    useEffect(() => {
        getAllOrders();
    }, [getAllOrders]);

    const columns = [
        {
            field: "id",
            headerName: "Tracking No.",
            headerAlign: "center",
            align: "center",
            width: 150,
        },
        {
            field: "userAvatar",
            headerName: "User",
            headerAlign: "center",
            align: "center",
            width: 150,
            renderCell: (rowData) => {
                const user = getUserByOrder(users?.data, rowData.row.userId);
                return (
                    <div
                        className="w-[50px] h-[50px] bg-center bg-cover rounded-md border-[1px] border-primary-light"
                        style={{
                            backgroundImage: `url(${
                                user?.avatar ??
                                "https://res.cloudinary.com/dlzyiprib/image/upload/v1694617729/e-commerces/user/kumz90hy8ufomdgof8ik.jpg"
                            })`,
                        }}
                    />
                );
            },
        },
        {
            field: "fullName",
            headerName: "Name",
            headerAlign: "center",
            align: "center",
            width: 200,
            renderCell: (rowData) => {
                const user = getUserByOrder(users?.data, rowData.row.userId);
                return <p>{user?.fullName}</p>;
            },
        },
        {
            field: "orderDate",
            headerName: "Order Date",
            headerAlign: "center",
            align: "center",
            width: 200,
            renderCell: (rowData) => {
                return <p>{formatDate(rowData.row.orderDate)}</p>;
            },
        },
        {
            field: "paymentMethod",
            headerName: "Payment Method",
            headerAlign: "center",
            align: "center",
            width: 200,
            renderCell: (rowData) => {
                const payment = getPaymentByOrderId(
                    payments?.data,
                    rowData.row.id
                );
                return <p>{payment?.paymentMethod}</p>;
            },
        },
        {
            field: "total",
            headerName: "Total Amount",
            headerAlign: "center",
            align: "center",
            type: "number",
            width: 150,
        },
        {
            field: "seatNumber",
            headerName: "Seat Number",
            headerAlign: "center",
            align: "center",
            width: 150,
        },
        {
            field: "status",
            headerName: "Status",
            headerAlign: "center",
            align: "center",
            width: 200,
            renderCell: (rowData) => {
                return <Status status={rowData.row.status} />;
            },
        },
        {
            field: "actions",
            headerName: "Actions",
            width: 200,
            headerAlign: "center",
            align: "center",
            sortable: false,
            renderCell: (rowData) => {
                return rowData.row.status === "Pending" ? (
                    <Tooltip title="Cancel Order">
                        <IconButton
                            onClick={() => handleCancelOrder(rowData.row)}
                        >
                            <CancelIcon className="hover:text-red-500 transition-all" />
                        </IconButton>
                    </Tooltip>
                ) : rowData.row.status === "Success" ? (
                    isSeatReturned(seats?.data, {
                        orderId: rowData.row.id,
                        seatNumber: rowData.row.seatNumber,
                    }) ? null : (
                        <Tooltip title="Return Table">
                            <IconButton
                                onClick={() =>
                                    handleReturnTable(rowData.row.seatNumber)
                                }
                            >
                                <KeyboardReturnIcon className="hover:text-primary transition-all" />
                            </IconButton>
                        </Tooltip>
                    )
                ) : null;
            },
        },
    ];
    const rows = orders?.data ?? [];

    return (
        <div className="p-5">
            <h1 className="text-3xl font-bold text-gray-700 w-fit border-b-[3px] border-primary-light">
                ORDER
            </h1>
            <div className="bg-white px-6 py-4 rounded-md my-10">
                <AdminTable
                    rows={rows}
                    columns={columns}
                    pageSizeOptions={[10, 20, 30, 40, 50]}
                />
            </div>
            <Toast />
        </div>
    );
}
