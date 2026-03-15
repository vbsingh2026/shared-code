import { useLocation, useNavigate } from "react-router-dom";

function OrderConfirmation() {
	const { state } = useLocation();
	const navigate = useNavigate();
	const order = state?.order;

	const handleContinueShopping = () => {
		navigate("/", { state: { isCartReset: true } }); // Pass a flag to indicate cart should be reset.
	};

	if (!order) {
		return (
			<div className="page">
				<h1 className="page-title">Order Not Found</h1>
				<button className="btn btn-primary" onClick={() => navigate("/")}>
					Back to Shop
				</button>
			</div>
		);
	}

	return (
		<div className="page">
			<div className="confirmation-header">
				<span className="check-icon">✅</span>
				<h1>Order Confirmed!</h1>
				<p className="order-id">
					Order ID: <strong>{order.orderId}</strong>
				</p>
			</div>

			<table className="cart-table">
				<thead>
					<tr>
						<th>Product</th>
						<th>Qty</th>
						<th>Unit Price</th>
						<th>Total</th>
					</tr>
				</thead>
				<tbody>
					{order.items.map((item) => (
						<tr key={item.productId}>
							<td>{item.productName}</td>
							<td>{item.quantity}</td>
							<td>₹{item.unitPrice}</td>
							<td>₹{item.lineTotal}</td>
						</tr>
					))}
				</tbody>
			</table>

			<div className="summary-card">
				<div className="summary-row">
					<span>Subtotal</span>
					<span>₹{order.subtotal}</span>
				</div>
				{order.discount > 0 && (
					<div className="summary-row discount">
						<span>Discount</span>
						<span>−₹{order.discount}</span>
					</div>
				)}
				<div className="summary-row">
					<span>GST (18%)</span>
					<span>₹{order.tax}</span>
				</div>
				<div className="summary-row grand">
					<span>Grand Total</span>
					<span>₹{order.grandTotal}</span>
				</div>
			</div>

			<div className="confirmation-actions">
				<button className="btn btn-primary" onClick={handleContinueShopping}>
					Continue Shopping
				</button>
			</div>
		</div>
	);
}

export default OrderConfirmation;
