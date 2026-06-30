import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

export const getOrders = (skip = 0, take = 10) =>
  api.get('/orders', { params: { skip, take } });

export const getOrder = (id) => api.get(`/orders/${id}`);

export const createOrder = (data) => api.post('/orders', data);

export const getStock = () => api.get('/inventory/stock');

export const setStock = (productName, quantity, unitPrice, currency = 'BRL') =>
  api.post('/inventory/stock', { productName, quantity, unitPrice, currency });

export const removeStock = (productName) =>
  api.delete(`/inventory/stock/${encodeURIComponent(productName)}`);

export default api;
