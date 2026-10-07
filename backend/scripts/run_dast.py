import json
import os
import subprocess
from urllib.request import Request, urlopen
from urllib.parse import urlparse

base_url = os.environ["DAST_BASE_URL"].rstrip("/")

# 1. Login com as credenciais recebidas por variáveis de ambiente.
payload = {
    "username": os.environ["DAST_USERNAME"],
    "password": os.environ["DAST_PASSWORD"],
}

login_request = Request(
    f"{base_url}/api/v1/Auth/Login",
    data=json.dumps(payload).encode("utf-8"),
    headers={
        "Content-Type": "application/json",
        "Accept": "application/json",
    },
    method="POST",
)

with urlopen(login_request, timeout=30) as response:
    login = json.load(response)

token = login.get("token")

if login.get("authenticated") is not True or not token:
    raise RuntimeError("Login falhou. O scan não será iniciado.")

# 2. Confirma acesso protegido antes de iniciar o scan.
check_request = Request(
    f"{base_url}/api/v1/Inscricao/Inscricoes",
    headers={
        "Authorization": f"Bearer {token}",
        "Accept": "application/json",
    },
    method="GET",
)

with urlopen(check_request, timeout=30) as response:
    if response.status != 200:
        raise RuntimeError("Acesso protegido não confirmado. Scan cancelado.")

print("Login e acesso protegido confirmados.", flush=True)
print(f"Expiração do JWT: {login.get('expiration')}", flush=True)

# 3. O ZAP usa estas variáveis para adicionar o cabeçalho.
# O token não é escrito no comando nem impresso pelo script.
scan_env = os.environ.copy()
scan_env["ZAP_AUTH_HEADER"] = "Authorization"
scan_env["ZAP_AUTH_HEADER_VALUE"] = f"Bearer {token}"
scan_env["ZAP_AUTH_HEADER_SITE"] = urlparse(base_url).hostname

# O scanner não precisa receber a senha.
scan_env.pop("DAST_PASSWORD", None)

print("Iniciando scan ativo autenticado...", flush=True)

result = subprocess.run(
    [
        "zap-api-scan.py",
        "-t", f"{base_url}/swagger/v1/swagger.json",
        "-f", "openapi",
        "-r", "dast-auth.html",
        "-J", "dast-auth.json",
    ],
    env=scan_env,
    cwd="/zap/wrk",
)

print(f"Código de saída do scan: {result.returncode}", flush=True)
raise SystemExit(result.returncode)