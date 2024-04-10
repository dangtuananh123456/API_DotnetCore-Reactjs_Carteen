import { Link, NavLink } from "react-router-dom";

const navigationLinks = [
    { to: "dashboard", label: "Dashboard" },
    { to: "category", label: "Category" },
    { to: "menu", label: "Menu" },
    { to: "order", label: "Order" },
    { to: "customer", label: "Customer" },
    { to: "revenue", label: "Revenue" },
];

export default function Sidebar() {
    return (
        <div className="bg-white border-r-[1px] w-[200px] fixed top-0 left-0 z-30 h-full">
            <div className="flex justify-center py-10">
                <Link
                    to="/admin"
                    className="text-3xl font-bold text-primary-light"
                >
                    HCMUS
                </Link>
            </div>
            <ul className="mt-5 flex flex-col font-semibold text-white">
                {navigationLinks.map(({ to, label }) => (
                    <li className="w-full text-lg" key={to}>
                        <NavLink
                            to={to}
                            className={({ isActive }) =>
                                isActive
                                    ? "inline-block w-full text-primary-light border-r-[3px] bg-orange-100 border-r-primary-light px-6 py-3"
                                    : "text-black px-6 py-3 inline-block w-full hover:bg-orange-100 transition-all duration-300"
                            }
                        >
                            {label}
                        </NavLink>
                    </li>
                ))}
            </ul>
        </div>
    );
}
