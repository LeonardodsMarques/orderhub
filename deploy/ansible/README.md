# Deploy com Ansible

Playbook simples para fazer deploy do OrderHub em um VPS (ex: Railway).

## Pré-requisitos

Na sua máquina local:

```bash
# Ubuntu/Debian
sudo apt update && sudo apt install ansible

# macOS
brew install ansible
```

No servidor de produção:
- Docker instalado
- Docker Compose plugin instalado (`docker compose version` deve funcionar)
- Usuário com permissão para rodar Docker (adicione ao grupo `docker`)
- Python 3 instalado (Ansible precisa)

## Configuração

1. Edite `inventory.ini`:
   ```ini
   orderhub-prod ansible_host=SEU_IP_AQUI
   ansible_user=SEU_USUARIO_AQUI
   ansible_ssh_private_key_file=~/.ssh/id_rsa
   ```

2. Edite `group_vars/all.yml`:
   - Ajuste `app_dir`, `git_repo`, `git_branch`
   - **Mude todas as senhas e secrets** em `env_vars`
   - Atualize `VITE_API_URL` com o IP/domínio do servidor

3. (Opcional) Se quiser usar deploy key em vez de HTTPS, gere uma chave SSH e adicione no GitHub:
   ```bash
   ssh-keygen -t ed25519 -f ~/.ssh/orderhub_deploy -N ""
   ```
   Depois use `git_repo: git@github.com:SEU_USUARIO/OrderHub.git` e configure a deploy key.

## Rodar o deploy

```bash
cd deploy/ansible
./deploy.sh
```

Ou, para ver mais detalhes:

```bash
ansible-playbook playbook.yml -vv
```

## O que o playbook faz

1. Verifica se Docker e Docker Compose estão instalados
2. Garante que o diretório da aplicação existe
3. Faz `git pull` da branch configurada
4. Gera o `.env` de produção a partir das variáveis do Ansible
5. Roda `docker compose down` e `docker compose up --build -d`
6. Espera o healthcheck do Gateway responder
7. Mostra o status dos containers

## Segurança

- Nunca commite `group_vars/all.yml` com senhas reais.
- Considere usar Ansible Vault para criptografar `group_vars/all.yml`:
  ```bash
  ansible-vault encrypt group_vars/all.yml
  ```
  E rode o deploy com:
  ```bash
  ./deploy.sh --ask-vault-pass
  ```
