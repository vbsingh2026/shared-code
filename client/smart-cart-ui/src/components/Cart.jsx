import { useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { getCart, addToCart, applyCoupon, checkout } from "../services/api";

function Cart({ cartId, setCartCount }) {
	const [cart, setCart] = useState(null);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState("");
	const [couponInput, setCouponInput] = useState("");
	const [couponMsg, setCouponMsg] = useState({ text: "", type: "" });
	const [applyingCoupon, setApplyingCoupon] = useState(false);
	const [checkingOut, setCheckingOut] = useState(false);
	const navigate = useNavigate();

	const getCartData = (cartId) => {
		getCart(cartId)
			.then(setCart)
			.catch(() => setError("Failed to load cart."))
			.finally(() => setLoading(false));
	};

	const loadCart = useCallback(() => {
		if (!cartId) {
			setLoading(false);
			return;
		}
		setLoading(true);
		getCartData(cartId);
	}, [cartId]);

	useEffect(() => {
		loadCart();

		//console.log(cart);
	}, []);

	const handleQuantityChange = async (item, newQty) => {
		if (newQty < 1) return;
		setError("");
		try {
			//console.log("Updating quantity for item:", item);
			const updated = await addToCart(cartId, item.productId, newQty);
			setCartCount(updated.items.reduce((sum, i) => sum + i.quantity, 0));
			//setCart(updated);
			getCartData(cartId); // Refresh cart data after quantity update.
		} catch (err) {
			setError(err.response?.data?.message || "Could not update quantity.");
		}
	};

	const handleApplyCoupon = async () => {
		if (!couponInput.trim()) return;
		setApplyingCoupon(true);
		setCouponMsg({ text: "", type: "" });
		setError("");
		try {
			const updated = await applyCoupon(
				cartId,
				couponInput.trim().toUpperCase(),
			);
			setCart(updated);
			setCouponMsg({
				text: `Coupon "${couponInput.toUpperCase()}" applied! You saved ₹${updated.discount}.`,
				type: "success",
			});
			console.log("Coupon applied successfully:", updated);
		} catch (err) {
			setCouponMsg({
				text: err.response?.data?.message || "Invalid coupon.",
				type: "error",
			});
		} finally {
			setApplyingCoupon(false);
		}
	};

	const handleCheckout = async () => {
		setCheckingOut(true);
		setError("");
		try {
			const order = await checkout(cartId);
			localStorage.removeItem("cartId");
			navigate("/order-confirmation", { state: { order } });
		} catch (err) {
			setError(
				err.response?.data?.message || "Checkout failed. Please try again.",
			);
			setCheckingOut(false);
		}
	};

	if (loading) return <div className="loading">Loading cart…</div>;
	if (!cartId || (!cart && !loading)) {
		return (
			<div className="page">
				<h1 className="page-title">🛒 Your Cart</h1>
				<div className="empty-state">
					Your cart is empty. <a href="/">Browse products</a>.
				</div>
			</div>
		);
	}

	return (
		<div className="page">
			<h1 className="page-title">🛒 Your Cart</h1>

			{error && <div className="alert alert-error">{error}</div>}

			{!cart?.items?.length ? (
				<div className="empty-state">
					Your cart is empty. <a href="/">Browse products</a>.
				</div>
			) : (
				<>
					<table className="cart-table">
						<thead>
							<tr>
								<th>Product</th>
								<th>Unit Price</th>
								<th>Quantity</th>
								<th>Total</th>
							</tr>
						</thead>
						<tbody>
							{cart.items.map((item) => (
								<tr key={item.productId}>
									<td>{item.productName}</td>
									<td>₹{item.unitPrice}</td>
									<td>
										<div className="qty-control">
											<button
												className="qty-btn"
												onClick={() =>
													handleQuantityChange(item, item.quantity - 1)
												}
											>
												−
											</button>
											<span className="qty-value">{item.quantity}</span>
											<button
												className="qty-btn"
												onClick={() =>
													handleQuantityChange(item, item.quantity + 1)
												}
											>
												+
											</button>
										</div>
									</td>
									<td>₹{item.lineTotal}</td>
								</tr>
							))}
						</tbody>
					</table>

					{/* Coupon Section */}
					<div className="coupon-section">
						<h3>Apply Coupon</h3>
						<div className="coupon-row">
							<input
								className="coupon-input"
								type="text"
								placeholder="e.g. FLAT50 or SAVE10"
								value={couponInput}
								onChange={(e) => setCouponInput(e.target.value.toUpperCase())}
								onKeyDown={(e) => e.key === "Enter" && handleApplyCoupon()}
							/>
							<button
								className="btn btn-secondary"
								onClick={handleApplyCoupon}
								disabled={applyingCoupon}
							>
								{applyingCoupon ? "Applying…" : "Apply"}
							</button>
						</div>
						{couponMsg.text && (
							<div className={`alert alert-${couponMsg.type}`}>
								{couponMsg.text}
							</div>
						)}
						<p className="coupon-hints">
							<strong>FLAT50</strong> — ₹50 off on orders ≥ ₹500 &nbsp;|&nbsp;
							<strong>SAVE10</strong> — 10% off (max ₹200) on orders ≥ ₹1000
						</p>
					</div>

					{/* Pricing Summary */}
					<div className="summary-card">
						<div className="summary-row">
							<span>Subtotal</span>
							<span>₹{cart.subtotal}</span>
						</div>
						{cart.discount > 0 && (
							<div className="summary-row discount">
								<span>Discount ({cart.couponCode})</span>
								<span>−₹{cart.discount}</span>
							</div>
						)}
						<div className="summary-row subtotal-after">
							<span>Amount after discount</span>
							<span>₹{cart.subtotal - cart.discount}</span>
						</div>
						<div className="summary-row grand">
							<span>Estimated Total (incl. 18% GST)</span>
							<span>₹{cart.grandTotal}</span>
						</div>
						<button
							className="btn btn-primary btn-checkout"
							onClick={handleCheckout}
							disabled={checkingOut}
						>
							{checkingOut ? "Processing…" : "Proceed to Checkout"}
						</button>
					</div>
				</>
			)}
		</div>
	);
}

export default Cart;
