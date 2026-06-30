import { Routes, Route, Link } from 'react-router-dom';
import Dashboard from './pages/Dashboard.jsx';
import Orders from './pages/Orders.jsx';
import NewOrder from './pages/NewOrder.jsx';
import Stock from './pages/Stock.jsx';

function App() {
  return (
    <div className="min-h-screen">
      <nav className="bg-blue-600 text-white p-4 flex gap-6 items-center flex-wrap">
        <Link to="/" className="font-bold text-lg">OrderHub</Link>
        <Link to="/" className="hover:underline">Dashboard</Link>
        <Link to="/orders" className="hover:underline">Orders</Link>
        <Link to="/orders/new" className="hover:underline">New Order</Link>
        <Link to="/stock" className="hover:underline">Stock</Link>
      </nav>

      <main className="p-6 max-w-5xl mx-auto">
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/orders" element={<Orders />} />
          <Route path="/orders/new" element={<NewOrder />} />
          <Route path="/stock" element={<Stock />} />
        </Routes>
      </main>
    </div>
  );
}

export default App;
