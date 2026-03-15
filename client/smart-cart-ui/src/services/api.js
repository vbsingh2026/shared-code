import axios from "axios";

const BASE_URL = "https://localhost:7040/api";

const api = axios.create({ baseURL: BASE_URL });

// ── Product API ──────────────────────────────────────────────────────────────
export const getProducts = () => api.get("/products").then((r) => r.data);

// ── Cart API ─────────────────────────────────────────────────────────────────
export const addToCart = (cartId, productId, quantity) =>
	api.post("/cart/items", { cartId, productId, quantity }).then((r) => r.data);

export const getCart = (cartId) =>
	api.get(`/cart/${cartId}`).then((r) => r.data);

export const applyCoupon = (cartId, couponCode) =>
	api.post(`/cart/${cartId}/apply-coupon`, { couponCode }).then((r) => r.data);

export const checkout = (cartId) =>
	api.post(`/cart/${cartId}/checkout`).then((r) => r.data);

// ── Order API ────────────────────────────────────────────────────────────────
export const getOrder = (orderId) =>
	api.get(`/orders/${orderId}`).then((r) => r.data);
