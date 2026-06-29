import { useEffect, useState } from 'react';
import { getOrders } from '../services/api.js';

export default function Dashboard() {
  const [stats, setStats] = useState({ total: 0, pending: 0, totalValue: 0 });

  useEffect(() => {
    getOrders(0, 1000)
      .then((res) => {
        const orders = res.data || [];
        const pending = orders.filter((o) => o.status === 'Pending').length;
        const totalValue = orders.reduce(
          (sum, o) => sum + (o.totalAmount?.amount || 0),
          0
        );
        setStats({ total: orders.length, pending, totalValue });
      })
      .catch((err) => console.error('Failed to load dashboard', err));
  }, []);

  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">Dashboard</h1>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <StatCard title="Total Orders" value={stats.total} />
        <StatCard title="Pending Orders" value={stats.pending} />
        <StatCard title="Total Value" value={`${stats.totalValue.toFixed(2)}`} />
      </div>
    </div>
  );
}

function StatCard({ title, value }) {
  return (
    <div className="bg-white p-6 rounded shadow">
      <h2 className="text-gray-500 text-sm font-medium uppercase">{title}</h2>
      <p className="text-3xl font-bold mt-2">{value}</p>
    </div>
  );
}
