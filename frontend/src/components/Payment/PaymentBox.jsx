import { useSelector } from "react-redux";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import WalletIcon from "@mui/icons-material/Wallet";
import CreditCardIcon from "@mui/icons-material/CreditCard";
import PaymentsIcon from "@mui/icons-material/Payments";

export default function PaymentBox({ onPay, onPayLater, method, amount }) {
    const user = useSelector((state) => state.auth.user);

    return (
        <div className="px-5 sm:px-10 py-4 lg:px-5 mt-5 border rounded-md">
            <p className="font-medium flex items-center">
                <span>
                    <PaymentsIcon className="mr-2" />
                </span>
                PAYMENT
            </p>
            <div className="px-0 py-2 sm:p-5">
                <h3 className="text-lg font-semibold uppercase">User</h3>
                <div className="cursor-pointer flex items-center justify-between gap-0 sm:gap-10 mt-3 border py-2 px-4 rounded">
                    <div className="flex items-center gap-x-2">
                        <CheckCircleIcon className="text-green-500" />
                        <div>
                            <p>Full Name</p>
                            <p className="font-medium text-lg">
                                {user.fullName}
                            </p>
                        </div>
                    </div>
                </div>
                <div className="cursor-pointer flex items-center justify-between gap-0 sm:gap-10 mt-2 border py-2 px-4 rounded">
                    <div className="flex items-center gap-x-2">
                        <CheckCircleIcon className="text-green-500" />
                        <div>
                            <p>Phone Number</p>
                            <p className="font-medium text-lg">
                                {user.phoneNumber}
                            </p>
                        </div>
                    </div>
                </div>
            </div>
            <div className="p-5">
                <h3 className="text-lg font-semibold uppercase">
                    PAYMENT METHODS
                </h3>
                <div className="cursor-pointer flex items-center gap-x-3 mt-2 py-2 px-4 rounded">
                    {method === "MOMO" ? (
                        <>
                            <CheckCircleIcon className="text-green-500" />
                            <p className="uppercase text-lg flex items-center gap-x-1">
                                <WalletIcon /> MOMO WALLET
                            </p>
                        </>
                    ) : (
                        <>
                            <CheckCircleIcon className="text-green-500" />
                            <p className="uppercase text-lg flex items-center gap-x-1">
                                <CreditCardIcon /> ATM
                            </p>
                        </>
                    )}
                </div>
            </div>
            <div className="p-5">
                <h3 className="text-lg font-semibold uppercase">TOTAL PRICE</h3>
                <div className="flex items-center gap-x-3 py-2 px-4 ">
                    <CheckCircleIcon className="text-green-500" />
                    <p className="uppercase text-lg flex items-center gap-x-1">
                        {amount} $
                    </p>
                </div>
            </div>
            <div className="p-5 flex gap-5">
                <button
                    type="button"
                    className="uppercase font-bold text-lg py-[10px] bg-primary hover:bg-primary-dark w-full rounded text-white transition-all"
                    onClick={onPay}
                >
                    PAY NOW
                </button>
                <button
                    type="button"
                    className="uppercase font-bold text-lg py-[10px] bg-gray-400 hover:bg-gray-300 w-full rounded text-white transition-all"
                    onClick={onPayLater}
                >
                    PAY LATER
                </button>
            </div>
        </div>
    );
}
