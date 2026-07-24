# CallFlow

API de gestion de campagnes d'appels sortants pour centre de contacts.

> Projet de Fin d'Année — support d'apprentissage backend orienté fondamentaux.
> L'accent est mis sur des **règles métier réelles** (concurrence, transactions,
> file priorisée) plutôt que sur un simple CRUD.

**Stack** : ASP.NET Core · Entity Framework Core · PostgreSQL · Clean Architecture

---

## Contexte

Un centre de contacts réalise des campagnes d'appels sortants pour une plateforme
de comparaison de prix (énergie, télécoms, assurance). Les téléconseillers appellent
des prospects, enregistrent le résultat de chaque échange et programment des rappels.

Sans outil dédié, cette activité repose sur des fichiers partagés, ce qui provoque
des doublons d'appel, des rappels oubliés et une absence de mesure fiable. CallFlow
répond à ces problèmes par une API qui garantit qu'un prospect n'est traité que par
un agent à la fois, qu'aucun rappel n'est perdu, et que chaque appel est tracé.

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
│  ├─ Infrastructure/   DbContext EF Core, dépôts, migrations
│  └─ Api/              point d'entrée Web API
├─ tests/
│  └─ Domain.Tests/     tests unitaires des règles de gestion
└─ docs/
   ├─ cahier-des-charges.md
   └─ decisions/        une note par décision technique (ADR)
```

## Démarrage

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

### 2. Configurer la chaîne de connexion

La chaîne de connexion n'est pas versionnée. Elle est fournie via les user-secrets
en développement :

```bash
dotnet user-secrets init --project src/Api
dotnet user-secrets set "ConnectionStrings:Default" \
  "Host=localhost;Port=5432;Database=callflow;Username=postgres;Password=<mot_de_passe>" \
  --project src/Api
```

### 3. Appliquer les migrations

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

### 4. Lancer l'API

```bash
dotnet run --project src/Api
```

Vérifier que l'API répond :

```bash
curl http://localhost:5264/health
# {"status":"ok"}
```

## Concepts techniques illustrés

Le projet met en œuvre et justifie les choix suivants :

| Domaine | Mise en œuvre |
|---|---|
| Clean Architecture | Découpage en 4 couches, règle de dépendance |
| Accès aux données | EF Core, chargement `Include`, prévention du N+1 |
| Performance | Index SQL sur la recherche par téléphone |
| Concurrence | Verrouillage optimiste sur la réservation d'un prospect |
| Transactions | Enregistrement atomique d'un appel |
| Sécurité | Authentification JWT et autorisation par rôle |
| Qualité | Tests unitaires (xUnit, Moq) des règles de gestion |
| Observabilité | Journalisation structurée des requêtes |

## Avancement

- [x] **Lot 0 — Socle** : solution 4 couches, PostgreSQL, migration initiale, endpoint de santé
- [ ] **Lot 1 — Noyau métier** : entités, file d'appel priorisée, réservation concurrente, enregistrement d'appel
- [ ] **Lot 2 — Sécurité et qualité** : JWT, rôles, validation, index, correction N+1, documentation
- [ ] **Lot 3 — Fonctions avancées** : tâches planifiées, tableau de bord temps réel, cache
- [ ] **Lot 4 — Industrialisation** : conteneurisation, intégration continue, tests d'intégration

## Documentation

- [Cahier des charges](docs/cahier-des-charges.md)
- [Décisions techniques](docs/decisions/)

## Auteur

Ismail Laaouan
