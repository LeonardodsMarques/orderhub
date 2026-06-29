FROM node:20-alpine

WORKDIR /app

COPY src/NotificationService/package.json ./
RUN npm install

COPY src/NotificationService/src ./src

ENV PORT=5002
ENV RABBITMQ_URL=amqp://guest:guest@rabbitmq:5672/

EXPOSE 5002

HEALTHCHECK CMD wget -qO- http://localhost:5002/health || exit 1

CMD ["npm", "start"]
