# Guide d'utilisation de l'API FlexPlanner avec API Keys

## 1. Créer une API Key

### Via l'interface d'administration (authentifié JWT)
```http
POST /api/apikeys
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "name": "Integration Mobile App",
  "allowedEndpoints": [
    "GET:/api/public/teams",
    "GET:/api/public/teams/*/planning",
    "GET:/api/public/teams/*/members"
  ],
  "expiresAt": "2024-12-31T23:59:59Z"
}
```

### Réponse
```json
{
  "apiKey": "fp_A7x9K2mN8pQ4rS6tV1wX3yZ5bC8dE0fG2hJ",
  "message": "Clé créée avec succès. Sauvegardez-la, elle ne sera plus affichée."
}
```

## 2. Utiliser l'API avec la clé

### Récupérer les équipes
```http
GET /api/public/teams
X-API-Key: fp_A7x9K2mN8pQ4rS6tV1wX3yZ5bC8dE0fG2hJ
```

### Récupérer le planning d'une équipe
```http
GET /api/public/teams/123e4567-e89b-12d3-a456-426614174000/planning?startDate=2024-01-01&endDate=2024-01-31
X-API-Key: fp_A7x9K2mN8pQ4rS6tV1wX3yZ5bC8dE0fG2hJ
```

### Récupérer les membres d'une équipe
```http
GET /api/public/teams/123e4567-e89b-12d3-a456-426614174000/members
X-API-Key: fp_A7x9K2mN8pQ4rS6tV1wX3yZ5bC8dE0fG2hJ
```

## 3. Gestion des permissions

### Créer une clé avec accès total
```json
{
  "name": "Super Admin Key",
  "allowedEndpoints": ["*"],
  "expiresAt": null
}
```

### Créer une clé avec accès limité
```json
{
  "name": "Lecture seule équipes",
  "allowedEndpoints": [
    "GET:/api/public/teams",
    "GET:/api/public/teams/*/members"
  ],
  "expiresAt": "2024-06-30T23:59:59Z"
}
```

## 4. Exemples d'intégration

### JavaScript/Node.js
```javascript
const API_BASE_URL = 'https://yourapi.com/api/public';
const API_KEY = 'fp_A7x9K2mN8pQ4rS6tV1wX3yZ5bC8dE0fG2hJ';

async function getTeams() {
  const response = await fetch(`${API_BASE_URL}/teams`, {
    headers: {
      'X-API-Key': API_KEY,
      'Content-Type': 'application/json'
    }
  });
  
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  
  return await response.json();
}

async function getTeamPlanning(teamId, startDate, endDate) {
  const url = `${API_BASE_URL}/teams/${teamId}/planning?startDate=${startDate}&endDate=${endDate}`;
  const response = await fetch(url, {
    headers: {
      'X-API-Key': API_KEY,
      'Content-Type': 'application/json'
    }
  });
  
  return await response.json();
}
```

### Python
```python
import requests

API_BASE_URL = 'https://yourapi.com/api/public'
API_KEY = 'fp_A7x9K2mN8pQ4rS6tV1wX3yZ5bC8dE0fG2hJ'

def get_teams():
    headers = {
        'X-API-Key': API_KEY,
        'Content-Type': 'application/json'
    }
    response = requests.get(f'{API_BASE_URL}/teams', headers=headers)
    response.raise_for_status()
    return response.json()

def get_team_planning(team_id, start_date, end_date):
    headers = {
        'X-API-Key': API_KEY,
        'Content-Type': 'application/json'
    }
    url = f'{API_BASE_URL}/teams/{team_id}/planning'
    params = {
        'startDate': start_date,
        'endDate': end_date
    }
    response = requests.get(url, headers=headers, params=params)
    response.raise_for_status()
    return response.json()
```

### cURL
```bash
# Récupérer les équipes
curl -X GET "https://yourapi.com/api/public/teams" \
  -H "X-API-Key: fp_A7x9K2mN8pQ4rS6tV1wX3yZ5bC8dE0fG2hJ"

# Récupérer le planning d'une équipe
curl -X GET "https://yourapi.com/api/public/teams/123e4567-e89b-12d3-a456-426614174000/planning?startDate=2024-01-01&endDate=2024-01-31" \
  -H "X-API-Key: fp_A7x9K2mN8pQ4rS6tV1wX3yZ5bC8dE0fG2hJ"
```

## 5. Gestion des erreurs

### Clé manquante
```json
{
  "error": "API Key manquante"
}
```
Status: 401

### Clé invalide ou expirée
```json
{
  "error": "API Key invalide ou non autorisée"
}
```
Status: 401

### Endpoint non autorisé
```json
{
  "error": "API Key invalide ou non autorisée"
}
```
Status: 401

## 6. Bonnes pratiques

### Sécurité
- Stockez les API Keys de manière sécurisée
- Utilisez HTTPS uniquement
- Définissez des dates d'expiration appropriées
- Limitez les permissions aux endpoints nécessaires
- Révérifiez régulièrement les clés utilisées

### Monitoring
- Surveillez l'utilisation via `last_used`
- Implémentez des logs d'accès
- Mettez en place des alertes en cas d'usage anormal

### Endpoints disponibles par défaut
- `GET:/api/public/teams` - Liste des équipes
- `GET:/api/public/teams/{teamId}/planning` - Planning d'équipe
- `GET:/api/public/teams/{teamId}/members` - Membres d'équipe
- `GET:/api/public/vacation-types` - Types de congés

Vous pouvez facilement ajouter d'autres endpoints en les déclarant dans le `PublicApiController` et en ajoutant l'attribut `[ApiKeyRequired]`.
