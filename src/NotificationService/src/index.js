const express = require('express');
const { connect } = require('./consumer');

const app = express();
const PORT = process.env.PORT || 5002;

app.use(express.json());

app.get('/health', (req, res) => {
  res.json({ status: 'ok' });
});

app.listen(PORT, () => {
  console.log(`[NOTIFICATION] Server listening on port ${PORT}`);
});

connect().catch((err) => {
  console.error('[NOTIFICATION] Initial consumer connection failed', err);
});
