import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createOrder, getStock } from '../services/api.js';
import Toast from '../components/Toast.jsx';

export default function NewOrder() {
  const navigate = useNavigate();
  const [toast, setToast] = useState('');
  const [stock, setStock] = useState([]);
  const [form, setForm] = useState({
    customerName: '',
    customerEmail: '',
    items: [{ productName: '', quantity: 1 }]
  });

  useEffect(() => {
    getStock()
      .then((res) => setStock(res.data || []))
      .catch((err) => console.error('Erro ao carregar estoque', err));
  }, []);

  const addItem = () => {
    setForm({
      ...form,
      items: [...form.items, { productName: '', quantity: 1 }]
    });
  };

  const removeItem = (index) => {
    const items = form.items.filter((_, i) => i !== index);
    setForm({ ...form, items });
  };

  const updateItem = (index, field, value) => {
    const items = [...form.items];
    items[index][field] = value;
    setForm({ ...form, items });
  };

  const getProduct = (name) => stock.find((p) => p.productName === name);

  const handleSubmit = async (e) => {
    e.preventDefault();

    const payload = {
      customerName: form.customerName,
      customerEmail: form.customerEmail,
      items: form.items
        .filter((i) => i.productName)
        .map((i) => ({
          productName: i.productName,
          quantity: parseInt(i.quantity, 10)
        }))
    };

    try {
      await createOrder(payload);
      setToast('Pedido criado com sucesso');
      setTimeout(() => navigate('/orders'), 1500);
    } catch (err) {
      console.error('Erro ao criar pedido', err);
      setToast('Falha ao criar pedido');
    }
  };

  const canAddMore = stock.length > form.items.length;

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">Novo Pedido</h1>
      <form onSubmit={handleSubmit} className="bg-white p-6 rounded shadow space-y-4">
        <div>
          <label className="block text-sm font-medium mb-1">Nome do Cliente</label>
          <input
            required
            className="w-full border rounded p-2"
            value={form.customerName}
            onChange={(e) => setForm({ ...form, customerName: e.target.value })}
          />
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">E-mail do Cliente</label>
          <input
            type="email"
            required
            className="w-full border rounded p-2"
            value={form.customerEmail}
            onChange={(e) => setForm({ ...form, customerEmail: e.target.value })}
          />
        </div>

        <div>
          <h2 className="font-semibold mb-2">Itens</h2>
          {stock.length === 0 && (
            <p className="text-sm text-gray-600 mb-2">Nenhum produto disponível no estoque.</p>
          )}
          {form.items.map((item, idx) => {
            const product = getProduct(item.productName);
            return (
              <div key={idx} className="flex flex-wrap gap-2 mb-2 items-end">
                <div className="flex-1 min-w-[250px]">
                  <label className="block text-xs text-gray-500 mb-1">Produto</label>
                  <select
                    required
                    className="w-full border rounded p-2"
                    value={item.productName}
                    onChange={(e) => updateItem(idx, 'productName', e.target.value)}
                  >
                    <option value="">Selecione um produto</option>
                    {stock.map((p) => (
                      <option key={p.productName} value={p.productName}>
                        {p.productName} — {p.unitPrice.toFixed(2)} {p.currency} ({p.quantity} em estoque)
                      </option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-xs text-gray-500 mb-1">Quantidade</label>
                  <input
                    type="number"
                    min="1"
                    required
                    className="w-24 border rounded p-2"
                    value={item.quantity}
                    onChange={(e) => updateItem(idx, 'quantity', e.target.value)}
                  />
                </div>
                {product && (
                  <div className="text-sm text-gray-600 py-2">
                    Total: {(product.unitPrice * item.quantity).toFixed(2)} {product.currency}
                  </div>
                )}
                <button
                  type="button"
                  onClick={() => removeItem(idx)}
                  className="text-red-500 px-2 mb-1"
                >
                  Remover
                </button>
              </div>
            );
          })}
          <button
            type="button"
            onClick={addItem}
            disabled={!canAddMore || stock.length === 0}
            className="text-blue-600 text-sm hover:underline disabled:opacity-50 disabled:cursor-not-allowed"
          >
            + Adicionar item
          </button>
        </div>

        <button
          type="submit"
          disabled={stock.length === 0 || form.items.some((i) => !i.productName)}
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Criar Pedido
        </button>
      </form>

      <Toast message={toast} onClose={() => setToast('')} />
    </div>
  );
}
