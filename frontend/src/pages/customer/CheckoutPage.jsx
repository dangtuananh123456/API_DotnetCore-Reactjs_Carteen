import { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useAddOrderMutation } from "../../services/order";
import ArrowBackIosNewIcon from "@mui/icons-material/ArrowBackIosNew";
import SeatReservationDialog from "../../components/Dialog/SeatReservationDialog";
import CheckoutBox from "../../components/Checkout/CheckoutBox";
import { formatPrice } from "../../utils";
import {
    useDeleteCartItemsMutation,
    useModifyCartItemsMutation,
} from "../../services/cart";
import { useAddOrderItemsMutation } from "../../services/orderitem";
import {
    useAddSeatReservationMutation,
    useLazyGetAllSeatReservationQuery,
} from "../../services/seat";
import { useAddPaymentMutation } from "../../services/payment";
import getItemsInCart from "../../features/cart/getItemsInCart";
import Toast from "../../components/Toast/Toast";
import { toast } from "react-toastify";

export default function CheckoutPage() {
    const location = useLocation();
    const navigate = useNavigate();
    const dispatch = useDispatch();
    const user = useSelector((state) => state.auth.user);
    const cartItems = useSelector((state) => state.cart.items);
    const items = location.state?.items;
    const [getAllSeat, { data }] = useLazyGetAllSeatReservationQuery();
    const [addOrder, { data: order, isSuccess: addOrderSuccess }] =
        useAddOrderMutation();
    const [deleteCartItem, { isSuccess: deleteCartItemSuccess }] =
        useDeleteCartItemsMutation();
    const [modifyCartItem, { isSuccess: modifyCartItemSuccess }] =
        useModifyCartItemsMutation();
    const [addOrderItem, { isSuccess: addOrderItemSuccess }] =
        useAddOrderItemsMutation();
    const [addSeatReservation, { isSuccess: addSeatReservationSuccess }] =
        useAddSeatReservationMutation();
    const [addPayment, { isSuccess: addPaymentSuccess }] =
        useAddPaymentMutation();
    const [open, setOpen] = useState(false);
    const [seatNumber, setSeatNumber] = useState(null);
    const [method, setMethod] = useState("MOMO");

    const getTotalPrice = useCallback(() => {
        return items.reduce((total, item) => {
            total += item.quantity * item.item.price;
            return total;
        }, 0);
    }, [items]);
    function handleDialogOpen() {
        getAllSeat();
        setOpen(true);
    }
    function handleDialogClose() {
        setOpen(false);
    }
    function handleSeatNumber(number) {
        setSeatNumber(number);
    }

    function handleMethod(eTarget = "MOMO") {
        if (eTarget === "MOMO") {
            if (method === "ATM") {
                setMethod("MOMO");
            }
        } else {
            if (method === "MOMO") {
                setMethod("ATM");
            }
        }
    }
    function handleCheckout() {
        if (seatNumber) {
            const payload = {
                orderDate: new Date(),
                total: Math.round(formatPrice(getTotalPrice())),
                seatNumber,
                status: "Pending",
                userId: user.id,
            };
            addOrder(payload);
            items.forEach((item) => {
                const found = cartItems.find(
                    (cartItem) => cartItem.id == item.id
                );
                if (item.quantity === found.quantity) {
                    deleteCartItem({ id: found.id });
                } else if (item.quantity < found.quantity) {
                    modifyCartItem({
                        ...found,
                        quantity: found.quantity - item.quantity,
                    });
                }
            });
        }
    }

    useEffect(() => {
        if (addOrderSuccess) {
            const orderId = order.id;
            items.forEach((item) => {
                addOrderItem({
                    itemId: item.item.id,
                    orderId,
                    quantity: item.quantity,
                });
            });
            addSeatReservation({ seatNumber, orderId });
            addPayment({
                paymentMethod: method,
                amount: Math.round(formatPrice(getTotalPrice())),
                status: "Pending",
                userId: user.id,
                orderId: order.id,
            });
        }
    }, [
        addOrderSuccess,
        items,
        seatNumber,
        addOrderItem,
        addSeatReservation,
        addPayment,
        method,
        order?.id,
        user?.id,
        getTotalPrice,
    ]);
    useEffect(() => {
        if (deleteCartItemSuccess || modifyCartItemSuccess) {
            dispatch(getItemsInCart({ userId: user.id }));
        }
    }, [deleteCartItemSuccess, modifyCartItemSuccess, dispatch, user?.id]);
    useEffect(() => {
        if (
            (deleteCartItemSuccess || modifyCartItemSuccess) &&
            addOrderItemSuccess &&
            addSeatReservationSuccess &&
            addPaymentSuccess
        ) {
            toast.success("Place order successfully !", {
                position: toast.POSITION.BOTTOM_RIGHT,
            });
            setTimeout(() => {
                navigate("/payment", {
                    state: {
                        payload: {
                            orderId: order.id,
                        },
                    },
                });
            }, 2000);
        }
    }, [
        deleteCartItemSuccess,
        modifyCartItemSuccess,
        addOrderItemSuccess,
        addSeatReservationSuccess,
        addPaymentSuccess,
        addPayment,
        navigate,
        getTotalPrice,
        method,
        order?.id,
    ]);

    return (
        <div className="min-h-[600px] px-5 py-8">
            <Link to="/" className="hover:underline text-primary">
                <ArrowBackIosNewIcon />
                BACK TO HOME PAGE
            </Link>
            <div className="block lg:flex gap-4 mt-4">
                <div className="flex-1">
                    <p className="font-medium text-xl sm:text-2xl text-center px-2">
                        Current Cart
                    </p>
                    <div className="p-5 overflow-x-scroll">
                        <table className="min-w-full text-center table-auto">
                            <thead className="font-medium border-b border-primary">
                                <tr>
                                    <th scope="col" className="px-6 py-4">
                                        #
                                    </th>
                                    <th scope="col" className="px-6 py-4">
                                        Thumbnail
                                    </th>
                                    <th scope="col" className="px-6 py-4">
                                        Name
                                    </th>
                                    <th scope="col" className="px-6 py-4">
                                        Quantity
                                    </th>
                                    <th scope="col" className="px-6 py-4">
                                        Price
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                {items.map((item, index) => (
                                    <tr key={index}>
                                        <td className="whitespace-nowrap px-6 py-4">
                                            {index + 1}
                                        </td>
                                        <td className="whitespace-nowrap px-6 py-4 flex justify-center">
                                            <div
                                                className="w-[50px] h-[50px] bg-center bg-cover rounded-md"
                                                style={{
                                                    backgroundImage: `url(${item.item.thumbnail})`,
                                                }}
                                            />
                                        </td>
                                        <td className="whitespace-nowrap px-6 py-4">
                                            {item.item.name}
                                        </td>
                                        <td className="whitespace-nowrap px-6 py-4">
                                            {item.quantity}
                                        </td>
                                        <td className="whitespace-nowrap px-6 py-4">
                                            {formatPrice(
                                                item.item.price * item.quantity
                                            )}{" "}
                                            $
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                    <div className="px-5">
                        <p className="text-xl font-medium mt-4 text-primary-light">
                            TOTAL PRICE:{" "}
                            <span className="font-bold text-primary">
                                {formatPrice(getTotalPrice())}$
                            </span>
                        </p>
                        <button
                            type="button"
                            onClick={() => handleDialogOpen()}
                            className="mt-2 py-2 px-5 bg-primary hover:bg-primary-dark text-white rounded font-semibold transition-all"
                        >
                            SEAT RESERVATION
                        </button>
                        <p className="mt-2 text-primary font-medium">
                            No. seat:{" "}
                            <span className="font-bold">
                                {seatNumber ?? "None"}
                            </span>
                        </p>
                        <SeatReservationDialog
                            open={open}
                            data={data}
                            currentSeat={seatNumber}
                            onSetSeatNumber={handleSeatNumber}
                            onSetClose={handleDialogClose}
                        />
                    </div>
                </div>
                <CheckoutBox
                    onCheckout={handleCheckout}
                    method={method}
                    onSetMethod={handleMethod}
                />
            </div>
            <Toast />
        </div>
    );
}
