const express = require('express');
const { connect } = require('./consumer');

const app = express();
const PORT = process.env.PORT || 5002;

app.use(express.json());

app.get('/health', (req, res) => {
  res.json({ status: 'ok' });
});

app.listen(PORT, () => {
  console.log(`[NOTIFICAÇÃO] Servidor ouvindo na porta ${PORT}`);
});

connect().catch((err) => {
  console.error('[NOTIFICAÇÃO] Falha na conexão inicial do consumidor', err);
});
