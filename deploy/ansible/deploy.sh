#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

echo "==> Rodando deploy do OrderHub..."

# Testa conectividade antes de tudo
ansible prod -m ping || {
  echo "Erro: não foi possível conectar no servidor. Verifique inventory.ini e a chave SSH."
  exit 1
}

# Executa o playbook
ansible-playbook playbook.yml "$@"

echo "==> Deploy finalizado."
