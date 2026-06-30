import { useEffect, useState } from 'react';
import { getStock, removeStock, setStock } from '../services/api.js';

export default function Stock() {
  const [items, setItems] = useState([]);
  const [productName, setProductName] = useState('');
  const [quantity, setQuantity] = useState('');
  const [message, setMessage] = useState('');

  const load = async () => {
    try {
      const res = await getStock();
      setItems(Object.entries(res.data).sort(([a], [b]) => a.localeCompare(b)));
    } catch (err) {
      console.error('Failed to load stock', err);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await setStock(productName, parseInt(quantity, 10));
      setMessage(`Stock for ${productName} set to ${quantity}`);
      setProductName('');
      setQuantity('');
      await load();
    } catch (err) {
      console.error('Failed to update stock', err);
      setMessage('Failed to update stock');
    }
  };

  const handleRemove = async (name) => {
    try {
      await removeStock(name);
      await load();
    } catch (err) {
      console.error('Failed to remove stock', err);
    }
  };

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">Stock Management</h1>

      <form onSubmit={handleSubmit} className="bg-white p-4 rounded shadow mb-6 flex flex-wrap gap-3 items-end">
        <div>
          <label className="block text-sm font-medium mb-1">Product Name</label>
          <input
            required
            className="border rounded p-2"
            value={productName}
            onChange={(e) => setProductName(e.target.value)}
          />
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">Quantity</label>
          <input
            type="number"
            min="0"
            required
            className="border rounded p-2 w-28"
            value={quantity}
            onChange={(e) => setQuantity(e.target.value)}
          />
        </div>
        <button
          type="submit"
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          Set Stock
        </button>
        {message && <span className="text-sm text-gray-600">{message}</span>}
      </form>

      <div className="bg-white rounded shadow overflow-x-auto">
        <table className="w-full text-left">
          <thead>
            <tr className="border-b">
              <th className="p-3">Product</th>
              <th className="p-3">Quantity</th>
              <th className="p-3">Actions</th>
            </tr>
          </thead>
          <tbody>
            {items.map(([name, qty]) => (
              <tr key={name} className="border-b last:border-0">
                <td className="p-3">{name}</td>
                <td className="p-3">{qty}</td>
                <td className="p-3">
                  <button
                    onClick={() => handleRemove(name)}
                    className="text-red-500 text-sm hover:underline"
                  >
                    Remove
                  </button>
                </td>
              </tr>
            ))}
            {items.length === 0 && (
              <tr>
                <td colSpan="3" className="p-6 text-center text-gray-500">
                  No stock items.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
