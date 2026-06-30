const amqp = require('amqplib');

const RABBITMQ_URL = process.env.RABBITMQ_URL || 'amqp://guest:guest@localhost:5672/';
const EXCHANGE = 'OrderCreated';
const QUEUE = 'order-created';
const ROUTING_KEYS = ['OrderCreated', 'order-created'];

let connection = null;
let channel = null;

function log(message) {
  console.log(`[NOTIFICAÇÃO] ${message}`);
}

async function connect() {
  try {
    connection = await amqp.connect(RABBITMQ_URL);
    channel = await connection.createChannel();

    await channel.assertExchange(EXCHANGE, 'fanout', { durable: true });
    await channel.assertQueue(QUEUE, { durable: true });

    for (const key of ROUTING_KEYS) {
      await channel.bindQueue(QUEUE, EXCHANGE, key);
    }

    await channel.consume(QUEUE, (msg) => {
      if (!msg) return;

      try {
        const payload = JSON.parse(msg.content.toString());
        const data = payload.message || payload;

        const amount = data.totalAmount?.amount ?? data.totalAmount;
        const currency = data.totalAmount?.currency ?? '';

        console.log(`📧 [NOTIFICAÇÃO] Pedido ${data.orderId} criado para ${data.customerEmail} — Total: ${amount} ${currency}`);
        channel.ack(msg);
      } catch (err) {
        console.error('[NOTIFICAÇÃO] Falha ao processar mensagem', err);
        channel.nack(msg, false, false);
      }
    });

    log('Conectado ao RabbitMQ e consumindo order-created');

    connection.on('error', (err) => {
      console.error('[NOTIFICAÇÃO] Erro de conexão', err.message);
    });

    connection.on('close', () => {
      log('Conexão fechada, reconectando em 5s...');
      setTimeout(connect, 5000);
    });
  } catch (err) {
    console.error('[NOTIFICAÇÃO] Falha ao conectar ao RabbitMQ', err.message);
    setTimeout(connect, 5000);
  }
}

module.exports = { connect };
