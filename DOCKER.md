# PostgreSQL pour le développement

L'API reste exécutée sur Windows, notamment pour le débogage Visual Studio.
Docker héberge uniquement PostgreSQL 17. Aucun Dockerfile n'est nécessaire.
Les quatre projets existants (API, Application, Domain, Infrastructure) ne sont pas modifiés.

## Configuration locale

Exécuter les commandes PowerShell depuis le dossier contenant `RoadTripGenerator.slnx`.
Le fichier `.env` contient le mot de passe local et est exclu par `.gitignore`.
Pour une nouvelle installation, copier `.env.example` vers `.env`, puis remplacer
la valeur d'exemple par un mot de passe local avant le premier démarrage.
Ne pas écraser un `.env` existant.

| Paramètre | Valeur |
| --- | --- |
| Hôte depuis Windows | `localhost` |
| Port depuis Windows | `5433` |
| Base | `roadtrip_dev` |
| Utilisateur | `roadtrip` |
| Mot de passe | valeur de `POSTGRES_PASSWORD` dans `.env` |

Le port est publié uniquement sur `127.0.0.1`. Le volume nommé
`roadtripgenerator_postgres_data` conserve les données après suppression du conteneur.
Changer `.env` après l'initialisation ne change pas le mot de passe enregistré dans la base.

## Démarrer et vérifier

```powershell
docker info
docker compose config --quiet
docker compose up -d --wait --wait-timeout 90 postgres
docker compose ps
docker compose logs --tail 50 postgres
docker compose exec postgres pg_isready -U roadtrip -d roadtrip_dev
```

`config --quiet` valide la configuration sans afficher le mot de passe interpolé.
L'état attendu est `healthy`.

## Tester avec psql

```powershell
docker compose exec postgres psql -U roadtrip -d roadtrip_dev
```

Dans l'invite SQL :

```sql
SELECT version();
SELECT current_database();
SELECT current_user;
```

Lister les tables avec `\dt`, puis quitter avec `\q`.
Une base neuve n'a pas encore de table métier : aucun DbContext, migration ou
repository n'est créé par cette configuration.

Pour vérifier aussi le mot de passe via TCP dans le conteneur :

```powershell
docker compose exec postgres psql -h 127.0.0.1 -U roadtrip -d roadtrip_dev -W
```

Saisir le mot de passe du `.env` à la demande ; il n'apparaît pas à l'écran.

## Arrêt et reprise

```powershell
# Arrêter en conservant les conteneurs.
docker compose stop

# Redémarrer.
docker compose up -d --wait postgres

# Supprimer conteneur et réseau, conserver les données.
docker compose down
```

**Destructif :** `docker compose down --volumes` supprime aussi les données PostgreSQL.
Cette commande n'est pas nécessaire pour un arrêt normal.

## Étape suivante : EF Core

L'installation du provider Npgsql, les secrets utilisateur de l'API, le DbContext
et les migrations restent à faire dans l'étape suivante. L'API ne se connecte pas
encore à cette base.

La future connexion de l'API utilisera `Host=localhost;Port=5433;Database=roadtrip_dev;Username=roadtrip`
avec le mot de passe local, à stocker dans les secrets utilisateur, sans le versionner.
