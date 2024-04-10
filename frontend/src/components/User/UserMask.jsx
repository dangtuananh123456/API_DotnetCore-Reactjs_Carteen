import { Popover, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import { logoutSuccess } from "../../features/auth/authSlice";
import { resetCart } from "../../features/cart/cartSlice";
import { useLogoutMutation } from "../../services/privateAuth";

const styleTypo = {
    p: 2,
    cursor: "pointer",
    transition: "all",
    transitionDuration: "0.3s",
    fontWeight: "bold",
    "&:hover": { bgcolor: "#ffa56d", color: "white" },
};

export default function UserMask({
    imageUrl = "https://res.cloudinary.com/dlzyiprib/image/upload/v1694617729/e-commerces/user/kumz90hy8ufomdgof8ik.jpg",
}) {
    const dispatch = useDispatch();
    const [logout, { isSuccess, isError }] = useLogoutMutation();
    const navigate = useNavigate();
    const [anchor, setAnchor] = useState("");

    function handleOpen(e) {
        e.preventDefault();
        setAnchor(e.currentTarget);
    }
    function handleClose() {
        setAnchor(null);
    }
    function handleNavigateOrder() {
        navigate("/order");
        setAnchor(null);
    }
    function handleLogout() {
        logout();
        setAnchor(null);
    }
    useEffect(() => {
        if (isSuccess && !isError) {
            dispatch(resetCart());
            dispatch(logoutSuccess());
            navigate("/");
        }
    }, [isSuccess, isError, navigate, dispatch]);

    return (
        <>
            <div
                className="cursor-pointer"
                onClick={(e) => handleOpen(e)}
                aria-describedby={anchor ? "mask-popover" : null}
            >
                <img
                    src={imageUrl}
                    className="object-cover object-center w-[40px] h-[40px] md:w-[50px] md:h-[50px] rounded-full"
                    alt="Mask"
                />
            </div>
            <Popover
                id={anchor ? "mask-popover" : null}
                open={Boolean(anchor)}
                anchorEl={anchor}
                onClose={handleClose}
                anchorOrigin={{
                    vertical: "bottom",
                    horizontal: "center",
                }}
                disableScrollLock={true}
            >
                <Typography
                    sx={styleTypo}
                    onClick={() => handleNavigateOrder()}
                >
                    My Order
                </Typography>
                <Typography sx={styleTypo} onClick={() => handleLogout()}>
                    Logout
                </Typography>
            </Popover>
        </>
    );
}
