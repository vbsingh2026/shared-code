import { useState } from "react";
import { BrowserRouter, Routes, Route, NavLink } from "react-router-dom";
import ProductList from "./components/ProductList";
import Cart from "./components/Cart";
import OrderConfirmation from "./components/OrderConfirmation";
import "./App.css";

function App() {
	const [cartId, setCartId] = useState(null);
	const [cartCount, setCartCount] = useState(0);

	return (
		<BrowserRouter>
			<header className="navbar">
				<NavLink to="/" className="brand">
					SmartCart
				</NavLink>
				<nav className="nav-links">
					<NavLink
						to="/"
						end
						className={({ isActive }) =>
							isActive ? "nav-link active" : "nav-link"
						}
					>
						Products
					</NavLink>
					<NavLink
						to="/cart"
						className={({ isActive }) =>
							isActive ? "nav-link active" : "nav-link"
						}
					>
						Cart{" "}
						{cartCount > 0 && <span className="cart-badge">{cartCount}</span>}
					</NavLink>
				</nav>
			</header>

			<main className="main-container">
				<Routes>
					<Route
						path="/"
						element={
							<ProductList
								cartId={cartId}
								setCartId={setCartId}
								setCartCount={setCartCount}
							/>
						}
					/>
					<Route
						path="/cart"
						element={<Cart setCartCount={setCartCount} cartId={cartId} />}
					/>
					<Route path="/order-confirmation" element={<OrderConfirmation />} />
				</Routes>
			</main>
		</BrowserRouter>
	);
}

export default App;
