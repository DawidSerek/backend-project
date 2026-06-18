# Backend Project

Final project for the "Programowanie aplikacji backend" laboratory course.
A contacts CRM backend: people, companies and organizations with PESEL/NIP/KRS
validation, polymorphic contacts, interactions (email/SMS/meeting),
CSV/JSON import, JWT auth with refresh tokens, tag management, and
Jaro-Winkler fuzzy deduplication.

- **Author:** Dawid Serek
- **Repository:** https://github.com/DawidSerek/backend-project

---

## Table of contents

- [Backend Project](#backend-project)
  - [Table of contents](#table-of-contents)
  - [Tech stack \& architecture](#tech-stack--architecture)
  - [How to run](#how-to-run)
  - [Authentication](#authentication)
  - [Implemented functions (task → feature map)](#implemented-functions-task--feature-map)
  - [Under the hood](#under-the-hood)
    - [Value Objects (records + EF converters)](#value-objects-records--ef-converters)
    - [Validation (testable error paths)](#validation-testable-error-paths)
    - [Deduplication](#deduplication)
    - [Ownership rule](#ownership-rule)
    - [Polymorphic JSON discriminators](#polymorphic-json-discriminators)
  - [API tour](#api-tour)
    - [Auth](#auth)
    - [Contacts (polymorphic)](#contacts-polymorphic)
    - [Persons](#persons)
    - [Companies](#companies)
    - [Organizations](#organizations)
    - [Interactions](#interactions)
    - [Tags](#tags)
    - [Positions](#positions)
    - [Deduplication](#deduplication-1)
    - [Import](#import)
    - [Admin (users \& roles)](#admin-users--roles)
  - [Notes \& known limitations](#notes--known-limitations)
  - [Testing](#testing)

---

## Tech stack & architecture

- **.NET 9**, ASP.NET Core, **EF Core** on **SQLite** (`Web/backend-project.db`).
- **ASP.NET Core Identity** + **JWT** (HS256) with DB-backed refresh tokens.
- **Clean Architecture**, four projects:
  - `ApplicationCore` — domain entities, Value Objects, service/repository **interfaces**. No EF, no ASP.NET.
  - `Infrastructure` — EF `DbContext`, repository & service implementations, Identity, JWT, validation, seeders, DI modules.
  - `Web` — controllers, `Program.cs`, `appsettings.json`.
  - `Tests` — xUnit; integration tests use `WebApplicationFactory<Program>`.

Two DI modules wired from `Web/Program.cs`:
`Infrastructure/Modules/AddEfModule.cs` (DbContext + Identity + repos + services)
and `Infrastructure/Modules/AddJwtModule.cs` (JWT bearer + authorization policies).

---

## How to run

On a fresh checkout, apply the EF migrations to create the schema at
`Web/backend-project.db`, then start the API:

```sh
dotnet ef database update \
  --project Infrastructure/Infrastructure.csproj \
  --startup-project Web/Web.csproj

dotnet run --project Web/Web.csproj
# API on http://localhost:5041  (HTTPS on https://localhost:7247)
```

> Migrations are **not** applied automatically at startup — you must run
> `dotnet ef database update` once (or use the `updatedb` skill in this
> repo). If you skip it, the app will start but the first DB access will
> fail with "no such table" because the seeders / endpoints will find an
> empty SQLite file.

On startup, two seeders run against the existing schema:
- `IdentitySeeder` — creates roles `Admin`, `User`, `SalesManager`, `Manager`,
  `Guest` and one admin user.
- `PositionSeeder` — inserts 20 default English position names
  (Developer, Senior Developer, Tech Lead, …).

EF migrations live in `Infrastructure/Migrations/`. To add a new migration
after a model change:

```sh
dotnet ef migrations add <Name> \
  --project Infrastructure/Infrastructure.csproj \
  --startup-project Web/Web.csproj
dotnet ef database update \
  --project Infrastructure/Infrastructure.csproj \
  --startup-project Web/Web.csproj
```

> Gotcha: the dev server (`dotnet watch`) holds the DB open; stop it
> before running `database update` or the command will fail with
> "database is locked".

---

## Authentication

JWT bearer (HS256). Access token expires in **60 min**, refresh token in
**7 days** (`Web/appsettings.json → Jwt`).

**Almost every endpoint requires a Bearer token.** The app sets an
authorization **FallbackPolicy** (`Infrastructure/Modules/AddJwtModule.cs`)
that requires an authenticated user for any endpoint without an explicit
`[AllowAnonymous]`, `[Authorize]`, or role attribute. Only
`POST /api/auth/login` and `POST /api/auth/refresh` are publicly callable.

Seed credentials (created by `IdentitySeeder`):

```
email:    admin@local
password: Admin123!
roles:    Admin
```

Login and use the returned `accessToken`:

```sh
TOKEN=$(curl -s -X POST http://localhost:5041/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@local","password":"Admin123!"}' | jq -r .accessToken)

curl http://localhost:5041/api/auth/me -H "Authorization: Bearer $TOKEN"
```

Role-gated endpoints:
- `AdminController` → `Admin`.
- `DeduplicationController` → `Admin` or `SalesManager`.
- Everything else → any authenticated user.

Additionally, the `ActiveUser` policy blocks users whose `status` claim
is not `Active` (set by the admin deactivate/activate endpoints).

---

## Implemented functions (task → feature map)

| # | Assignment task | What was built |
|---|---|---|
| 1 | PESEL + Organization + search | `Pesel` ValueObject (11-digit, validates birthdate encoding, check digit, gender). `Organization` contact type; `Person` can belong to an Organization. `IContactRepository` search methods (email domain, organization/company membership). PESEL EF value converter. |
| 2 | NIP + Company + employer/employees | `Nip` ValueObject (10-digit, check digit, exposes tax-office number + full tax-office name). `Company` contact type; `Person` has an `Employer`; company employees list, search, and sort (by name/email/birthdate). |
| 3 | CSV/JSON import + report | `POST /api/import/contacts` accepts `.csv`/`.json` (up to 100 MB). CSV groups separated by `People`/`Companies`/`Organizations` header lines, any common delimiter auto-detected. JSON shape `{ "people":[], "companies":[], "organizations":[] }`. Returns an `ImportReport` (imported list + errors with messages). Duplicates **within the file** and **against the DB** are rejected. |
| 4 | Polymorphic controller + PhoneNumber | `POST /api/contacts` with discriminator `contactType` ∈ `{Person, Company, Organization}`. `PhoneNumber` ValueObject (E.164, country + dial code). |
| 5 | Admin API + EmailAddress | `AdminController` for user/role management, activate/deactivate. `EmailAddress` ValueObject (user + domain). |
| 6 | Interactions | `EmailInteraction`, `SmsInteraction`, `MeetingInteraction` (TPH). Record interactions per contact, query history by date range / type / paged. |
| 7 | Deduplication + RemovedContacts | Two strategies: **Exact** and **Fuzzy (Jaro-Winkler)**, configurable via DTO (threshold + which properties). Admin/SalesManager-only. Removed contacts are hard-deleted and snapshotted to a `RemovedContacts` audit table (date, type, JSON, user id). Create-path also rejects exact duplicates. |
| 8 | Tags + KRS/Website + Position | `Tag` M:N on all contacts; add/remove/search-by-tag/list-tags. `Organization` add validates **KRS** (10-digit format check; real public-API validator is implemented in `KrsValidator.cs` but the mock is wired by default — see Notes) and **Website** reachability (HTTP GET must return 2xx). `Position` entity with English names; the position name in a Person DTO must already exist in the DB. `GET /api/positions` lists all. |

---

## Under the hood

### Value Objects (records + EF converters)

`ApplicationCore/ValueObjects/` — `Pesel`, `Nip`, `PhoneNumber`,
`EmailAddress`. Each is a `record` persisted as a string via an EF
`ValueConverter` + `ValueComparer` pair in
`Infrastructure/Configurations/ValueConverters/` and `…/ValueComparers/`.
Construct them explicitly: `new Pesel("44051401359")`, etc. — there is no
implicit string conversion. Constructing one with bad input throws
`ArgumentException`, which controllers map to **400**.

### Validation (testable error paths)

| Send this | Get this |
|---|---|
| Malformed PESEL (wrong length, bad check digit, impossible date) | `400` |
| Malformed NIP (wrong length, bad check digit) | `400` |
| Invalid phone number / email format | `400` |
| `Position` name not present in DB on Person create | `400` (polymorphic) / `404` (dedicated `/api/persons`) |
| Organization `website` not reachable (non-2xx or timeout) | `400 "Website '...' is not reachable"` |
| Organization `krs` not 10 digits | `400` (mock validator) |
| Interaction `date` in the future | `400` |
| Duplicate Person (same name/email/phone) on `POST /api/contacts` | `400 "Contact already exists"` |
| Duplicate within an import file | row moved to `report.errors` with `"Duplicate within file"` |
| Dedup `threshold` outside `[0,1]` | `400` |
| Missing Bearer token on any non-`login`/`refresh` endpoint | `401` |
| Non-owner, non-admin trying to edit/delete a contact | `403` |
| Non-Admin/SalesManager calling dedup | `403` |

### Deduplication

`ApplicationCore/Services/JaroWinkler/` implements Jaro-Winkler string
similarity. `DeduplicationStrategyService` compares the configured
properties (`name`, `email`, `phonenumber`) and reports pairs whose
per-property scores meet the threshold. `Exact` strategy requires
identical values; `Fuzzy` uses Jaro-Winkler. `POST /deduplication/find`
reports; `POST /deduplication/remove` deletes the higher-id duplicate
and writes a `RemovedContact` snapshot (date + user id of the
deduplicator, per the brief).

### Ownership rule

Every contact carries `CreatedById`. Edit/delete on the polymorphic
`ContactsController` check `c.CreatedById == userId || isAdmin` → `403`
otherwise. (Other list/search endpoints are read-only and don't enforce
ownership.)

### Polymorphic JSON discriminators

Abstract DTOs use `[JsonPolymorphic]`:
- `ContactCreateBase` → discriminator property **`contactType`** (`Person`/`Company`/`Organization`).
- `CreateInteractionDto` → discriminator property **`type`** (`Email`/`Sms`/`Meeting`).

Omitting the discriminator or sending a wrong value → `400`.

---

## API tour

All endpoints accept/return JSON. Bearer token required unless marked
`[AllowAnonymous]`. Errors are usually `{ "error": "..." }` plus the
status code shown. `[ApiController]` also returns automatic `400` on
invalid model state.

### Auth

| Method | Path | Auth | Body → Response |
|---|---|---|---|
| POST | `/api/auth/login` | `[AllowAnonymous]` | `{ "email","password" }` → `200 AuthResponseDto` (below) or `401` |
| POST | `/api/auth/refresh` | `[AllowAnonymous]` | `{ "accessToken","refreshToken" }` → `200 AuthResponseDto` or `401` |
| POST | `/api/auth/revoke` | Bearer | `{ "token":"<refresh>" }` → `204` |
| GET  | `/api/auth/me` | Bearer | `200 UserDto` (from JWT claims) |

`AuthResponseDto`:
```json
{
  "accessToken":  "eyJ...",
  "refreshToken": "base64-512-bits",
  "expiresAt":    "2026-06-18T17:30:00Z",
  "user": { "id":"guid","firstName":"...","lastName":"...","email":"...",
            "department":"...","status":"Active","roles":["Admin"] }
}
```

### Contacts (polymorphic)

The canonical create/edit/delete surface. Discriminator `contactType`.

| Method | Path | Auth | Notes |
|---|---|---|---|
| GET | `/api/contacts/{id}` | Bearer | `200` contact (subtype) or `404` |
| POST | `/api/contacts` | Bearer | `201` + `Location` or `400` (validation/dedup) |
| PUT | `/api/contacts/{id}` | Bearer (owner/Admin) | `200` updated, `403`, `404`, or `400` |
| DELETE | `/api/contacts/{id}` | Bearer (owner/Admin) | `204`, `403`, or `404` |

**POST Person:**
```json
{
  "contactType": "Person",
  "name": "Adam Kowalski",
  "email": "adam@example.com",
  "phone": "+48 123 456 789",
  "pesel": "44051401359",
  "organizationId": "guid-or-omit",
  "employerId": "guid-or-omit",
  "position": "Developer"
}
```
`position` must exist in the Positions table → `400` if not. `organizationId`/`employerId` must reference existing orgs/companies → `400`. After build, dedup pre-check rejects exact duplicates → `400 "Contact already exists"`.

**POST Company:**
```json
{ "contactType":"Company","name":"Acme Corp","email":"contact@acme.pl",
  "phone":"+48 22 1234567","nip":"1234567890","regon":"...","industry":"IT",
  "website":"https://acme.pl" }
```
NIP validated (check digit + tax-office) → `400`.

**POST Organization:**
```json
{ "contactType":"Organization","name":"Fundacja Przyszłość",
  "email":"info@fundacja.pl","phone":"+48 22 1234567",
  "krs":"0000123456","website":"https://fundacja.pl" }
```
`website` is HTTP-checked (2xx within 5s) → `400 "Website '...' is not reachable"`. `krs` format-checked.

> **Note:** `PUT` does not update `Position` (only the create path applies it).

curl example:
```sh
curl -X POST http://localhost:5041/api/contacts \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"contactType":"Person","name":"Adam Kowalski","pesel":"44051401359",
       "email":"adam@example.com","phone":"+48 123 456 789","position":"Developer"}'
```

### Persons

Dedicated Person endpoints (use `phoneNumber`, no discriminator).

| Method | Path | Auth | Notes |
|---|---|---|---|
| GET | `/api/persons` | Bearer | `200 List<ResultPersonDto>` |
| GET | `/api/persons/{id}` | Bearer | `200` or `404` |
| POST | `/api/persons` | Bearer | `CreatePersonDto` → `201`; bad position → `404` |
| DELETE | `/api/persons/{id}` | Bearer | `204` |

### Companies

| Method | Path | Auth | Notes |
|---|---|---|---|
| GET | `/api/companies` | Bearer | `200 List<Company>` |
| GET | `/api/companies/{id}` | Bearer | `200` or `404` |
| GET | `/api/companies/by-nip/{nip}` | Bearer | `200` or `404` |
| GET | `/api/companies/search?name=&nip=` | Bearer | one query required, else `400`. `nip`→single or `404`; `name`→`Contains` list |
| POST | `/api/companies` | Bearer | `CreateCompanyDto` → `201` or `400` (bad NIP/phone/email) |
| DELETE | `/api/companies/{id}` | Bearer | `204` or `404` |
| GET | `/api/companies/{id}/employees` | Bearer | `200 List<Person>` or `404` |
| GET | `/api/companies/{id}/employees/sorted?sortBy=name\|email\|birthdate&desc=false` | Bearer | `200` sorted list or `404` |

### Organizations

| Method | Path | Auth | Notes |
|---|---|---|---|
| GET | `/api/organizations` | Bearer | `200 List<Organization>` |
| GET | `/api/organizations/{id}` | Bearer | `200` or `404` |
| POST | `/api/organizations` | Bearer | `CreateContactDto` (`Name/Email/PhoneNumber` only — **no KRS/Website**; use the polymorphic `POST /api/contacts` for full Organization validation) → `201` |
| DELETE | `/api/organizations/{id}` | Bearer | `204` |

> **Limitation:** cross-criteria search (email domain, org/company
> membership) is implemented on `IContactRepository` and unit-tested but
> not exposed as an HTTP endpoint. Use the dedicated list/search endpoints
> above for HTTP queries.

### Interactions

`[Authorize]` on the whole controller. Discriminator `type` ∈ `{Email,Sms,Meeting}`.

| Method | Path | Auth | Notes |
|---|---|---|---|
| POST | `/api/contacts/{contactId}/interactions` | Bearer | `CreateInteractionDto` → `201` + `Location`, `400` (future date), `404` (contact) |
| GET | `/api/contacts/{contactId}/interactions?from=&to=&type=&page=1&pageSize=20` | Bearer | `200 List<InteractionResultDto>` |
| DELETE | `/api/interactions/{id}` | Bearer (creator/Admin) | `204`, `403`, or `404` |

Bodies:
```json
{ "type":"Email","date":"2026-06-18T12:00:00Z","content":"Quick check-in",
  "subject":"Follow up","fromAddress":"a@x.pl","toAddress":"b@y.pl" }
```
```json
{ "type":"Sms","date":"...","content":"Hello","phoneNumber":"+48..." }
```
```json
{ "type":"Meeting","date":"...","content":"...","location":"...",
  "endTime":"...","attendees":["a","b"] }
```
Query semantics: `from`+`to` → range; only `type` → by type; neither → paged (default 20). `date` cannot be in the future (1 min tolerance).

curl:
```sh
curl -X POST http://localhost:5041/api/contacts/$CID/interactions \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"type":"Email","date":"2026-06-18T12:00:00Z","content":"Hi",
       "subject":"Follow up","fromAddress":"a@x.pl","toAddress":"b@y.pl"}'
```

### Tags

| Method | Path | Auth | Notes |
|---|---|---|---|
| GET | `/api/tags` | Bearer | `200 [{id,name,color}]` |
| POST | `/api/tags` | Bearer | `{ "name":"VIP","color":"#FF0000" }` → `201 + Location`. Idempotent on `name`. |
| POST | `/api/tags/contacts/{contactId}/{tagName}` | Bearer | `204`. Creates the tag if missing. |
| DELETE | `/api/tags/contacts/{contactId}/{tagName}` | Bearer | `204` |
| GET | `/api/tags/contacts/by-tag/{tagName}` | Bearer | `200 List<Contact>` (mixed subtypes) |

### Positions

| Method | Path | Auth | Notes |
|---|---|---|---|
| GET | `/api/positions` | Bearer | `200 [{id,name,description}]`. Seeded with 20 English names. |

The Person-create validator's error message points here for the list of valid positions.

### Deduplication

`[Authorize(Roles = "Admin,SalesManager")]` on the whole controller.

| Method | Path | Body → Response |
|---|---|---|
| POST | `/api/deduplication/find` | `DeduplicationConfigDto` → `200 DeduplicationReportDto` (no removal) or `400` |
| POST | `/api/deduplication/remove` | same body → `200 DeduplicationReportDto` with `removedCount>0`; snapshots to `RemovedContacts` |

`DeduplicationConfigDto`:
```json
{ "threshold":0.85, "properties":["name","email","phonenumber"], "strategy":0 }
```
`strategy`: `0=Exact`, `1=Fuzzy`, `2=Both`. `threshold` must be in `[0,1]` else `400`.

`DeduplicationReportDto`:
```json
{
  "totalContactsScanned": 123,
  "removedCount": 0,
  "duplicates": [
    { "id1":"...","id2":"...","name1":"...","name2":"...",
      "score":1.0, "perPropertyScores": {"name":1.0,"email":0.0,"phonenumber":0.0} }
  ]
}
```
On remove, the higher-id contact is hard-deleted; a `RemovedContact` row records date, type, JSON snapshot, and the deduplicator's user id.

curl (as admin):
```sh
curl -X POST http://localhost:5041/api/deduplication/find \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"threshold":0.85,"properties":["name","email","phonenumber"],"strategy":1}'
```

### Import

| Method | Path | Auth | Notes |
|---|---|---|---|
| POST | `/api/import/contacts` | Bearer | multipart/form-data, field `file`, `.csv`/`.json`, max 100 MB → `200 ImportReport` |

CSV: groups introduced by header lines `People`, `Companies`, `Organizations`; column headers match DTO fields; delimiter auto-detected (comma, semicolon, tab, pipe, …).
JSON: `{ "people":[...], "companies":[...], "organizations":[...] }`.

`ImportReport`:
```json
{
  "imported": [ { "name":"...","type":"Person","id":"..." } ],
  "errors":   [ { "data":{...}, "errorMessages":["PESEL invalid"] } ]
}
```
Duplicates within the file **and** against the DB are rejected (moved to `errors` with `"Duplicate within file"`).

curl:
```sh
curl -X POST http://localhost:5041/api/import/contacts \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@contacts.csv"
```

### Admin (users & roles)

`[Authorize(Roles = "Admin")]` on the whole controller.

| Method | Path | Notes |
|---|---|---|
| GET | `/api/admin/users` | `200 List<AdminUserDto>` with roles |
| GET | `/api/admin/users/{id}` | `200` or `404` |
| POST | `/api/admin/users?password=Temp123!` | body `{ email,firstName,lastName,roles[] }` → `201` or `400` |
| PATCH | `/api/admin/users/{id}` | `{ firstName?,lastName?,email? }` → `204` or `400` |
| POST | `/api/admin/users/{id}/deactivate` | `204` (cannot deactivate the last active Admin) |
| POST | `/api/admin/users/{id}/activate` | `204` |
| POST | `/api/admin/users/{id}/roles` | body `"RoleName"` (raw string) → `204` or `400` |
| DELETE | `/api/admin/users/{id}/roles/{role}` | `204` or `400` |

---

## Notes & known limitations

- **KRS validation is currently a mock.** `Web/Program.cs` registers
  `MockKrsValidator` (10-digit format check only). The real
  `KrsValidator` (calling `https://api-krs.ms.gov.pl/api/krs/{krs}`) is
  implemented and HTTP-client-injected but commented out. Swap the
  registration to enable real public-API verification.
- **Imports are not attributed to the importing user.** `ImportController`
  passes `Guid.Empty` as the owner id (marked `// TODO: current user`),
  so imported contacts are not editable/deletable by their importer under
  the ownership rule (an Admin still can).
- **No seeded `SalesManager` user.** Only `admin@local` is loginable, so
  the dedup role gate can currently be exercised only as Admin.
- **Cross-criteria contact search** (email domain, org/company membership)
  is implemented on the repository and unit-tested but not exposed as an
  HTTP endpoint.

---

## Testing

xUnit. `Tests/UnitTests/` covers pure logic — PESEL (birthdate, check
digit, gender), NIP (tax-office number, full name, check digit),
Jaro-Winkler. `Tests/IntegrationTests/` uses
`WebApplicationFactory<Program>` against the production SQLite DB
(`DeduplicationControllerTest` logs in as `admin@local` and exercises
the full find/remove flow, including the `401`-without-auth case).

```sh
dotnet test
dotnet test --filter "Pesel"        # by class name fragment
```
