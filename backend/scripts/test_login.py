import json
import os
from urllib.request import Request, urlopen
from urllib.error import HTTPError

base_url = os.environ.get(
    "DAST_BASE_URL", "http://localhost:5000"
).rstrip("/")

protected_url = f"{base_url}/api/v1/Inscricao/Inscricoes"


def get_status(url, token=None):
    headers = {"Accept": "application/json"}

    if token:
        headers["Authorization"] = f"Bearer {token}"

    request = Request(url, headers=headers, method="GET")

    try:
        with urlopen(request, timeout=30) as response:
            return response.status
    except HTTPError as error:
        return error.code


# Testa a rota sem autenticação.
status_without_token = get_status(protected_url)
print(f"Rota SEM JWT: HTTP {status_without_token}")

# Faz login.
payload = {
    "username": os.environ["DAST_USERNAME"],
    "password": os.environ["DAST_PASSWORD"],
}

request = Request(
    f"{base_url}/api/v1/Auth/Login",
    data=json.dumps(payload).encode("utf-8"),
    headers={
        "Content-Type": "application/json",
        "Accept": "application/json",
    },
    method="POST",
)

with urlopen(request, timeout=30) as response:
    result = json.load(response)

token = result.get("token")

if result.get("authenticated") is not True or not token:
    raise RuntimeError("Login não confirmou autenticação ou não retornou JWT.")

print("Login confirmado; JWT recebido — valor ocultado")

# Testa a mesma rota com autenticação.
status_with_token = get_status(protected_url, token)
print(f"Rota COM JWT: HTTP {status_with_token}")

if status_without_token == 401 and 200 <= status_with_token < 300:
    print("SUCESSO: acesso protegido confirmado com o JWT do usuário.")
else:
    print("Resultado diferente do esperado; confira os status acima.")