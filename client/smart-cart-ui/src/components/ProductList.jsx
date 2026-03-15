import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { getProducts, addToCart } from "../services/api";

function ProductList({ cartId, setCartId, setCartCount }) {
	const [products, setProducts] = useState([]);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState("");
	const [addingId, setAddingId] = useState(null);
	const [successMsg, setSuccessMsg] = useState("");
	const navigate = useNavigate();

	const loaction = useLocation(); // To trigger re-render when coming back from cart/confirmation pages.

	if (loaction.state?.isCartReset) {
		setCartCount(0);
	}

	useEffect(() => {
		getProducts()
			.then(setProducts)
			.catch(() => setError("Failed to load products. Is the API running?"))
			.finally(() => setLoading(false));

		//setCartCount(loaction.state?.cartCount || 0); // Update cart count when returning from other pages.
	}, []);

	const handleAddToCart = async (product) => {
		setAddingId(product.id);
		setError("");
		setSuccessMsg("");
		try {
			const updatedCart = await addToCart(cartId || undefined, product.id, 1);
			if (!cartId) {
				setCartId(updatedCart.id);
				localStorage.setItem("cartId", updatedCart.id);
			}
			setCartCount(updatedCart.items.reduce((sum, i) => sum + i.quantity, 0));
			setSuccessMsg(`"${product.name}" added to cart!`);
			setTimeout(() => setSuccessMsg(""), 2500);
		} catch (err) {
			setError(err.response?.data?.message || "Failed to add item.");
		} finally {
			setAddingId(null);
		}
	};

	if (loading) return <div className="loading">Loading products…</div>;

	return (
		<div className="page">
			<h1 className="page-title">🛍️ Products</h1>

			{error && <div className="alert alert-error">{error}</div>}
			{successMsg && <div className="alert alert-success">{successMsg}</div>}

			<div className="product-grid">
				{products.map((product) => (
					<div key={product.id} className="product-card">
						<div className="product-name">{product.name}</div>
						<div className="product-price">
							₹{product.price.toLocaleString("en-IN")}
						</div>
						<div
							className={`product-stock ${product.stock === 0 ? "out" : ""}`}
						>
							{product.stock > 0 ? `${product.stock} in stock` : "Out of stock"}
						</div>
						<button
							className="btn btn-primary"
							disabled={product.stock === 0 || addingId === product.id}
							onClick={() => handleAddToCart(product)}
						>
							{addingId === product.id ? "Adding…" : "Add to Cart"}
						</button>
					</div>
				))}
			</div>

			{cartId && (
				<div className="sticky-bar">
					<button className="btn btn-outline" onClick={() => navigate("/cart")}>
						View Cart →
					</button>
				</div>
			)}
		</div>
	);
}

export default ProductList;
