import { useEffect, useState } from 'react';
import { getStock, removeStock, setStock } from '../services/api.js';

export default function Stock() {
  const [items, setItems] = useState([]);
  const [form, setForm] = useState({
    productName: '',
    quantity: '',
    unitPrice: '',
    currency: 'BRL'
  });
  const [message, setMessage] = useState('');
  const [isEditing, setIsEditing] = useState(false);

  const load = async () => {
    try {
      const res = await getStock();
      setItems(res.data || []);
    } catch (err) {
      console.error('Erro ao carregar estoque', err);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const resetForm = () => {
    setForm({ productName: '', quantity: '', unitPrice: '', currency: 'BRL' });
    setIsEditing(false);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await setStock(
        form.productName,
        parseInt(form.quantity, 10),
        parseFloat(form.unitPrice),
        form.currency
      );
      setMessage(`Produto ${form.productName} salvo com sucesso`);
      resetForm();
      await load();
    } catch (err) {
      console.error('Erro ao salvar produto', err);
      setMessage('Falha ao salvar produto');
    }
  };

  const handleEdit = (product) => {
    setForm({
      productName: product.productName,
      quantity: product.quantity,
      unitPrice: product.unitPrice,
      currency: product.currency
    });
    setIsEditing(true);
  };

  const handleRemove = async (name) => {
    try {
      await removeStock(name);
      if (form.productName === name) resetForm();
      await load();
    } catch (err) {
      console.error('Erro ao remover produto', err);
    }
  };

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">Gerenciamento de Estoque</h1>

      <form onSubmit={handleSubmit} className="bg-white p-4 rounded shadow mb-6 flex flex-wrap gap-3 items-end">
        <div>
          <label className="block text-sm font-medium mb-1">Nome do Produto</label>
          <input
            required
            disabled={isEditing}
            className="border rounded p-2 disabled:bg-gray-100"
            value={form.productName}
            onChange={(e) => setForm({ ...form, productName: e.target.value })}
          />
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">Quantidade</label>
          <input
            type="number"
            min="0"
            required
            className="border rounded p-2 w-28"
            value={form.quantity}
            onChange={(e) => setForm({ ...form, quantity: e.target.value })}
          />
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">Preço unitário</label>
          <input
            type="number"
            step="0.01"
            min="0"
            required
            className="border rounded p-2 w-32"
            value={form.unitPrice}
            onChange={(e) => setForm({ ...form, unitPrice: e.target.value })}
          />
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">Moeda</label>
          <input
            required
            className="border rounded p-2 w-24"
            value={form.currency}
            onChange={(e) => setForm({ ...form, currency: e.target.value })}
          />
        </div>
        <button
          type="submit"
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          {isEditing ? 'Atualizar' : 'Adicionar'}
        </button>
        {isEditing && (
          <button
            type="button"
            onClick={resetForm}
            className="bg-gray-200 text-gray-800 px-4 py-2 rounded hover:bg-gray-300"
          >
            Cancelar
          </button>
        )}
        {message && <span className="text-sm text-gray-600">{message}</span>}
      </form>

      <div className="bg-white rounded shadow overflow-x-auto">
        <table className="w-full text-left">
          <thead>
            <tr className="border-b">
              <th className="p-3">Produto</th>
              <th className="p-3">Quantidade</th>
              <th className="p-3">Preço unitário</th>
              <th className="p-3">Moeda</th>
              <th className="p-3">Ações</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => (
              <tr key={item.productName} className="border-b last:border-0">
                <td className="p-3">{item.productName}</td>
                <td className="p-3">{item.quantity}</td>
                <td className="p-3">{item.unitPrice.toFixed(2)}</td>
                <td className="p-3">{item.currency}</td>
                <td className="p-3 flex gap-3">
                  <button
                    onClick={() => handleEdit(item)}
                    className="text-blue-600 text-sm hover:underline"
                  >
                    Editar
                  </button>
                  <button
                    onClick={() => handleRemove(item.productName)}
                    className="text-red-500 text-sm hover:underline"
                  >
                    Remover
                  </button>
                </td>
              </tr>
            ))}
            {items.length === 0 && (
              <tr>
                <td colSpan="5" className="p-6 text-center text-gray-500">
                  Nenhum item em estoque.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
