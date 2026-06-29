import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createOrder } from '../services/api.js';
import Toast from '../components/Toast.jsx';

export default function NewOrder() {
  const navigate = useNavigate();
  const [toast, setToast] = useState('');
  const [form, setForm] = useState({
    customerName: '',
    customerEmail: '',
    items: [
      { productName: '', quantity: 1, unitPrice: '', currency: 'USD' }
    ]
  });

  const addItem = () => {
    setForm({
      ...form,
      items: [...form.items, { productName: '', quantity: 1, unitPrice: '', currency: 'USD' }]
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

  const handleSubmit = async (e) => {
    e.preventDefault();

    const payload = {
      customerName: form.customerName,
      customerEmail: form.customerEmail,
      items: form.items.map((i) => ({
        productName: i.productName,
        quantity: parseInt(i.quantity, 10),
        unitPrice: parseFloat(i.unitPrice),
        currency: i.currency
      }))
    };

    try {
      await createOrder(payload);
      setToast('Order created successfully');
      setTimeout(() => navigate('/orders'), 1500);
    } catch (err) {
      console.error('Failed to create order', err);
      setToast('Failed to create order');
    }
  };

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">New Order</h1>
      <form onSubmit={handleSubmit} className="bg-white p-6 rounded shadow space-y-4">
        <div>
          <label className="block text-sm font-medium mb-1">Customer Name</label>
          <input
            required
            className="w-full border rounded p-2"
            value={form.customerName}
            onChange={(e) => setForm({ ...form, customerName: e.target.value })}
          />
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">Customer Email</label>
          <input
            type="email"
            required
            className="w-full border rounded p-2"
            value={form.customerEmail}
            onChange={(e) => setForm({ ...form, customerEmail: e.target.value })}
          />
        </div>

        <div>
          <h2 className="font-semibold mb-2">Items</h2>
          {form.items.map((item, idx) => (
            <div key={idx} className="flex flex-wrap gap-2 mb-2">
              <input
                placeholder="Product"
                required
                className="flex-1 min-w-[150px] border rounded p-2"
                value={item.productName}
                onChange={(e) => updateItem(idx, 'productName', e.target.value)}
              />
              <input
                type="number"
                min="1"
                required
                className="w-20 border rounded p-2"
                value={item.quantity}
                onChange={(e) => updateItem(idx, 'quantity', e.target.value)}
              />
              <input
                type="number"
                step="0.01"
                min="0"
                required
                className="w-28 border rounded p-2"
                value={item.unitPrice}
                onChange={(e) => updateItem(idx, 'unitPrice', e.target.value)}
              />
              <input
                placeholder="Currency"
                required
                className="w-24 border rounded p-2"
                value={item.currency}
                onChange={(e) => updateItem(idx, 'currency', e.target.value)}
              />
              <button
                type="button"
                onClick={() => removeItem(idx)}
                className="text-red-500 px-2"
              >
                Remove
              </button>
            </div>
          ))}
          <button
            type="button"
            onClick={addItem}
            className="text-blue-600 text-sm hover:underline"
          >
            + Add item
          </button>
        </div>

        <button
          type="submit"
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        >
          Create Order
        </button>
      </form>

      <Toast message={toast} onClose={() => setToast('')} />
    </div>
  );
}
