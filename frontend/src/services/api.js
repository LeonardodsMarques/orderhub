import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('orderhub_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
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

export const login = (email) =>
  api.post('/auth/login', { email });

export default api;
