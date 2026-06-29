export default function Toast({ message, onClose }) {
  if (!message) return null;

  return (
    <div className="fixed bottom-4 right-4 bg-green-600 text-white px-4 py-3 rounded shadow-lg flex items-center gap-3">
      <span>{message}</span>
      <button onClick={onClose} className="font-bold text-xl leading-none">×</button>
    </div>
  );
}
