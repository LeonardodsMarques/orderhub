FROM node:20-alpine

WORKDIR /app

COPY frontend/package.json ./
RUN npm install

COPY frontend/ .

ENV PORT=3000
ENV VITE_API_URL=http://localhost:5000/api

EXPOSE 3000

HEALTHCHECK CMD wget -qO- http://localhost:3000/ || exit 1

CMD ["npm", "run", "dev"]
