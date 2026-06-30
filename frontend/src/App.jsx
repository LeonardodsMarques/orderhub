import { useState } from 'react';
import { Routes, Route, Link } from 'react-router-dom';
import Dashboard from './pages/Dashboard.jsx';
import Orders from './pages/Orders.jsx';
import NewOrder from './pages/NewOrder.jsx';
import Stock from './pages/Stock.jsx';
import { login } from './services/api.js';

function App() {
  const [token, setToken] = useState(localStorage.getItem('orderhub_token'));

  const handleLogin = async () => {
    const email = window.prompt('Digite seu e-mail:');
    if (!email) return;

    try {
      const { data } = await login(email);
      localStorage.setItem('orderhub_token', data.token);
      setToken(data.token);
    } catch {
      window.alert('Falha no login');
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('orderhub_token');
    setToken(null);
  };

  return (
    <div className="min-h-screen">
      <nav className="bg-blue-600 text-white p-4 flex gap-6 items-center flex-wrap">
        <Link to="/" className="font-bold text-lg">OrderHub</Link>
        <Link to="/" className="hover:underline">Início</Link>
        <Link to="/orders" className="hover:underline">Pedidos</Link>
        <Link to="/orders/new" className="hover:underline">Novo Pedido</Link>
        <Link to="/stock" className="hover:underline">Estoque</Link>
        <div className="ml-auto">
          {token ? (
            <button onClick={handleLogout} className="text-sm underline">Sair</button>
          ) : (
            <button onClick={handleLogin} className="text-sm underline">Entrar</button>
          )}
        </div>
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
