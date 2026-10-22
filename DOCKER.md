# API et PostgreSQL avec Docker

Exécuter les commandes depuis le dossier contenant `RoadTripGenerator.slnx` et `compose.yaml`.

## Configuration

Le fichier `.env` local doit contenir `POSTGRES_PASSWORD` et `REST_COUNTRIES_API_KEY`.
Il est exclu de Git et du contexte Docker. `.env.example` montre les noms attendus sans secret.
Sur une autre machine, copier `.env.example` vers `.env` et renseigner les valeurs réelles.
Ne pas écraser un `.env` existant.

Le Dockerfile compile avec le SDK .NET 10, puis copie la publication dans une seconde étape utilisant la même image locale.
Cette image contient déjà le runtime ASP.NET Core 10. Elle est plus volumineuse qu'une image runtime seule,
mais évite un téléchargement actuellement indisponible depuis le registre Microsoft.
Quand le registre sera accessible, remplacer l'image de l'étape `runtime` par `mcr.microsoft.com/dotnet/aspnet:10.0`
permettra de réduire la taille, sans changer la commande de lancement.
Le conteneur final lance `dotnet RoadTripGenerator.Api.dll` avec un utilisateur non root.
Les secrets sont transmis au démarrage, jamais pendant la construction de l'image.

## Démarrer

```powershell
docker compose config --quiet
docker compose up -d --build
docker compose ps
docker compose logs --tail 50 api
```

L'API attend que PostgreSQL soit healthy. Elle est accessible sur `http://localhost:5103`.
Dans Docker, elle se connecte à `postgres:5432`. Depuis Windows, PostgreSQL reste accessible sur `localhost:5433`.
Le mode Development permet de consulter le document OpenAPI sur `http://localhost:5103/openapi/v1.json`.

Après un changement de code, relancer `docker compose up -d --build api` pour reconstruire l'image.
Suivre les logs avec `docker compose logs -f api` (Ctrl+C quitte seulement le suivi).

## Données et migrations

Le service postgres et le volume `roadtripgenerator_postgres_data` existants sont conservés.
Aucune migration n'est appliquée automatiquement au démarrage. Cette configuration utilise la base déjà migrée.
Sur une base neuve, appliquer les migrations existantes avant de démarrer l'API.
L'initialisation des pays au démarrage reste inchangée : elle est ignorée si la table contient déjà des pays.

```powershell
docker compose exec postgres psql -U roadtrip -d roadtrip_dev
```

Dans psql :

```sql
SELECT COUNT(*) FROM "Countries";
SELECT "Id", "UserId" FROM "RoadTrips";
```

Quitter avec `\q`.

## Tester l'API depuis PowerShell

Les écritures ci-dessous concernent l'utilisateur `docker-demo`, pour préserver les itinéraires existants.

```powershell
curl.exe -i "http://localhost:5103/api/countries?page=1&pageSize=20"
curl.exe -i -H "X-User-ID: docker-demo" "http://localhost:5103/api/roadtrip"

'{"countries":["FRA","DEU","HRV"]}' | curl.exe -i -X PUT "http://localhost:5103/api/roadtrip" -H "X-User-ID: docker-demo" -H "Content-Type: application/json" --data-binary '@-'

'{"cca3":"BEL"}' | curl.exe -i -X POST "http://localhost:5103/api/roadtrip/country" -H "X-User-ID: docker-demo" -H "Content-Type: application/json" --data-binary '@-'

curl.exe -i -X DELETE -H "X-User-ID: docker-demo" "http://localhost:5103/api/roadtrip/country/DEU"
curl.exe -i -H "X-User-ID: docker-demo" "http://localhost:5103/api/roadtrip"
```

## Arrêter

```powershell
docker compose stop
```

Redémarrer avec `docker compose up -d`.
`docker compose down` retire les conteneurs et le réseau mais conserve le volume.
Ne pas utiliser `docker compose down --volumes` : cela supprimerait les données PostgreSQL.
