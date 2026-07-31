# CallFlow

API de gestion de campagnes d'appels sortants pour centre de contacts.

> Projet de Fin d'Année — support d'apprentissage backend orienté fondamentaux.
> L'accent est mis sur des **règles métier réelles** (concurrence, transactions,
> file priorisée, temps réel) plutôt que sur un simple CRUD.

**Stack** : ASP.NET Core · Entity Framework Core · PostgreSQL · SignalR · Hangfire · Docker · Clean Architecture

[![CI](https://github.com/ismaillaa/CallFlow/actions/workflows/ci.yml/badge.svg)](https://github.com/ismaillaa/CallFlow/actions/workflows/ci.yml)

---

## Contexte

Un centre de contacts réalise des campagnes d'appels sortants pour une plateforme
de comparaison de prix (énergie, télécoms, assurance). Les téléconseillers appellent
des prospects, enregistrent le résultat de chaque échange et programment des rappels.

Sans outil dédié, cette activité repose sur des fichiers partagés, ce qui provoque
des doublons d'appel, des rappels oubliés et une absence de mesure fiable. CallFlow
répond à ces problèmes par une API qui garantit qu'un prospect n'est traité que par
un agent à la fois, qu'aucun rappel n'est perdu, et que chaque appel est tracé.

## Démonstration

Tableau de bord temps réel : chaque appel enregistré apparaît instantanément sur
l'écran superviseur, sans rafraîchissement (SignalR / WebSocket).

<!-- Ajoute ton GIF ici, par exemple : -->
<!-- ![Dashboard temps réel](docs/images/dashboard-temps-reel.gif) -->

## Architecture

Le projet suit une architecture en couches (Clean Architecture). La règle de
dépendance est stricte : **les références pointent toujours vers l'intérieur**, et
la couche `Domain` ne référence aucun autre projet.

```
┌───────────────────────────────────────────────┐
│  Api            contrôleurs, auth, DI, config  │
│   ├─ dépend de Application, Infrastructure,    │
│   │            Domain                          │
│                                                │
│  Infrastructure EF Core, dépôts, accès base    │
│   ├─ dépend de Application, Domain             │
│                                                │
│  Application    cas d'usage, DTOs, validation  │
│   ├─ dépend de Domain                          │
│                                                │
│  Domain         entités, enums, interfaces     │
│   └─ ne dépend de rien                         │
└───────────────────────────────────────────────┘
```

Ce découpage rend la logique métier indépendante de la base de données : elle peut
être testée sans infrastructure, et la technologie d'accès aux données peut changer
sans toucher au métier.

## Structure du dépôt

```
CallFlow/
├─ src/
│  ├─ Domain/           entités et interfaces (aucune dépendance)
│  ├─ Application/      cas d'usage, DTOs, validation
│  ├─ Infrastructure/   DbContext EF Core, dépôts, migrations, auth
│  └─ Api/              point d'entrée Web API, hubs SignalR, middleware
├─ tests/
│  └─ Domain.Tests/     tests unitaires + test d'intégration de concurrence
├─ .github/workflows/   pipeline d'intégration continue
├─ Dockerfile           image multi-stage de l'API
├─ docker-compose.yml   orchestration API + PostgreSQL
└─ docs/
   ├─ cahier-des-charges.md
   └─ decisions/        une note par décision technique (ADR)
```

## Démarrage rapide (Docker)

L'ensemble de la stack (API + PostgreSQL) démarre d'une seule commande. L'API
applique ses migrations automatiquement au démarrage.

```bash
cp .env.example .env      # renseigner les secrets (mot de passe, clé JWT)
docker compose up --build
```

Vérifier que l'API répond :

```bash
curl http://localhost:8080/health
# {"status":"ok"}
```

## Démarrage manuel (sans Docker Compose)

### Prérequis

- .NET SDK 10
- Docker (pour PostgreSQL)
- `dotnet-ef` : `dotnet tool install --global dotnet-ef`

### 1. Lancer la base de données

```bash
docker run --name callflow-db \
  -e POSTGRES_PASSWORD=<mot_de_passe> \
  -e POSTGRES_DB=callflow \
  -p 5432:5432 -d postgres:16
```

### 2. Configurer les secrets

La chaîne de connexion et la clé JWT ne sont pas versionnées. Elles sont fournies
via les user-secrets en développement :

```bash
dotnet user-secrets init --project src/Api
dotnet user-secrets set "ConnectionStrings:Default" \
  "Host=localhost;Port=5432;Database=callflow;Username=postgres;Password=<mot_de_passe>" \
  --project src/Api
dotnet user-secrets set "Jwt:Key" "<clé_secrète_min_32_caractères>" --project src/Api
dotnet user-secrets set "Jwt:Issuer" "CallFlow" --project src/Api
```

### 3. Appliquer les migrations

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

### 4. Lancer l'API

```bash
dotnet run --project src/Api
```

## Authentification

Les endpoints sont protégés par JWT. Pour obtenir un jeton :

```bash
POST /api/auth/login
{ "identifiant": "agent1", "motDePasse": "..." }
# → { "token": "eyJ..." }
```

Le jeton est ensuite fourni dans l'en-tête `Authorization: Bearer <token>`.
L'identité de l'agent (id, rôle) est lue depuis le jeton signé, jamais depuis
l'URL — un agent ne peut pas agir au nom d'un autre.

## Règles de gestion notables

| Règle | Description |
|---|---|
| Réservation exclusive | Un prospect n'est traité que par un agent à la fois (verrouillage optimiste) |
| Enregistrement atomique | Appel + statut + compteur écrits en une seule transaction |
| File priorisée | Les prospects sont distribués selon un ordre de priorité |
| Rappels garantis | Un rappel demandé est toujours enregistré et jamais perdu |
| Expiration automatique | Une réservation inactive est libérée après 15 minutes |
| Clôture automatique | Un prospect injoignable est clôturé après 5 tentatives |
| Unicité | Un numéro est unique au sein d'une campagne |

## Concepts techniques illustrés

| Domaine | Mise en œuvre |
|---|---|
| Clean Architecture | Découpage en 4 couches, règle de dépendance stricte |
| Accès aux données | EF Core, chargement `Include`, prévention du N+1 |
| Performance | Index SQL sur la recherche par téléphone |
| Concurrence | Verrouillage optimiste (via `xmin` natif PostgreSQL) |
| Transactions | Enregistrement atomique d'un appel (tout ou rien) |
| Sécurité | Authentification JWT, hachage BCrypt, autorisation par rôle |
| Tâches de fond | Expiration automatique des réservations (Hangfire) |
| Temps réel | Tableau de bord live des appels (SignalR / WebSocket) |
| Qualité | Tests unitaires (xUnit, Moq) + test d'intégration de concurrence |
| Conteneurisation | Dockerfile multi-stage, orchestration Docker Compose |
| CI/CD | Pipeline GitHub Actions (build + tests avec PostgreSQL) |
| Observabilité | Journalisation structurée, gestion centralisée des erreurs |

## Tests

```bash
dotnet test
```

La suite comprend des tests unitaires des règles de gestion (mock du dépôt) et un
**test d'intégration de concurrence** qui lance deux réservations en parallèle sur
une base PostgreSQL réelle et vérifie qu'une seule aboutit. Ces tests s'exécutent
automatiquement à chaque `push` via GitHub Actions.

## Avancement

- [x] **Lot 0 — Socle** : solution 4 couches, PostgreSQL, migration initiale, endpoint de santé
- [x] **Lot 1 — Noyau métier** : entités, file d'appel priorisée, réservation concurrente, enregistrement d'appel transactionnel, rappels, import CSV
- [x] **Lot 2 — Sécurité et qualité** : authentification JWT, autorisation par rôle, DTOs, gestion centralisée des erreurs, tests unitaires et d'intégration
- [x] **Lot 3 — Fonctions avancées** : tâches planifiées (Hangfire), tableau de bord temps réel (SignalR)
- [x] **Lot 4 — Industrialisation** : conteneurisation (Docker Compose), intégration continue (GitHub Actions)

## Documentation

- [Cahier des charges](docs/cahier-des-charges.md)
- [Décisions techniques](docs/decisions/)

## Auteur

Ismail Laaouan
