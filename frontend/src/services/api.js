import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

export const getOrders = (skip = 0, take = 10) =>
  api.get('/orders', { params: { skip, take } });

export const getOrder = (id) => api.get(`/orders/${id}`);

export const createOrder = (data) => api.post('/orders', data);

export const updateStatus = (id, status) =>
  api.patch(`/orders/${id}/status`, { status });

export default api;
