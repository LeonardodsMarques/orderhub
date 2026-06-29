import { useEffect, useState } from 'react';
import { getOrders } from '../services/api.js';

const TAKE = 10;

export default function Orders() {
  const [orders, setOrders] = useState([]);
  const [skip, setSkip] = useState(0);

  useEffect(() => {
    loadOrders();
  }, [skip]);

  const loadOrders = async () => {
    try {
      const res = await getOrders(skip, TAKE);
      setOrders(res.data || []);
    } catch (err) {
      console.error('Failed to load orders', err);
    }
  };

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">Orders</h1>
      <div className="bg-white rounded shadow overflow-x-auto">
        <table className="w-full text-left">
          <thead>
            <tr className="border-b">
              <th className="p-3">ID</th>
              <th className="p-3">Customer</th>
              <th className="p-3">Status</th>
              <th className="p-3">Total</th>
              <th className="p-3">Created</th>
            </tr>
          </thead>
          <tbody>
            {orders.map((o) => (
              <tr key={o.id} className="border-b last:border-0">
                <td className="p-3 font-mono">{o.id.slice(0, 8)}</td>
                <td className="p-3">
                  {o.customerName}
                  <br />
                  <span className="text-sm text-gray-500">{o.customerEmail}</span>
                </td>
                <td className="p-3">
                  <span className="px-2 py-1 rounded text-sm bg-gray-100">{o.status}</span>
                </td>
                <td className="p-3">
                  {o.totalAmount?.amount} {o.totalAmount?.currency}
                </td>
                <td className="p-3">{new Date(o.createdAt).toLocaleString()}</td>
              </tr>
            ))}
            {orders.length === 0 && (
              <tr>
                <td colSpan="5" className="p-6 text-center text-gray-500">
                  No orders found.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
      <div className="flex gap-2 mt-4">
        <button
          disabled={skip === 0}
          onClick={() => setSkip(Math.max(0, skip - TAKE))}
          className="px-4 py-2 bg-gray-200 rounded disabled:opacity-50"
        >
          Previous
        </button>
        <button
          onClick={() => setSkip(skip + TAKE)}
          disabled={orders.length < TAKE}
          className="px-4 py-2 bg-gray-200 rounded disabled:opacity-50"
        >
          Next
        </button>
      </div>
    </div>
  );
}
