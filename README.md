# HNG Stage 1 - Profiles API

## 📌 Base URL

```
https://your-deployed-url.com
```

---

## 📖 API Overview

This API accepts a name, retrieves data from three external APIs (Genderize, Agify, Nationalize), applies classification logic, stores the result in a PostgreSQL database, and exposes endpoints to manage the data.

---

## ⚙️ Technology Stack

* ASP.NET Core (.NET 8)
* Entity Framework Core
* PostgreSQL
* HttpClient
* UUID v7 (UUIDNext)

---

## 🔗 External APIs

* https://api.genderize.io?name={name}
* https://api.agify.io?name={name}
* https://api.nationalize.io?name={name}

---

## 📡 Endpoints

### 1. Create Profile

**POST** `/api/profiles`

#### Request Body

```json
{
  "name": "ella"
}
```

#### Success Response (201)

```json
{
  "status": "success",
  "data": {
    "id": "uuid",
    "name": "ella",
    "gender": "female",
    "gender_probability": 0.99,
    "sample_size": 1234,
    "age": 46,
    "age_group": "adult",
    "country_id": "US",
    "country_probability": 0.85,
    "created_at": "2026-04-01T12:00:00Z"
  }
}
```

#### Duplicate Response

```json
{
  "status": "success",
  "message": "Profile already exists",
  "data": { ...existing profile... }
}
```

---

### 2. Get Single Profile

**GET** `/api/profiles/{id}`

#### Success Response (200)

```json
{
  "status": "success",
  "data": {
    "id": "uuid",
    "name": "emmanuel",
    "gender": "male",
    "gender_probability": 0.99,
    "sample_size": 1234,
    "age": 25,
    "age_group": "adult",
    "country_id": "NG",
    "country_probability": 0.85,
    "created_at": "2026-04-01T12:00:00Z"
  }
}
```

---

### 3. Get All Profiles

**GET** `/api/profiles`

#### Query Parameters (optional, case-insensitive)

* `gender`
* `country_id`
* `age_group`

#### Example

```
/api/profiles?gender=male&country_id=NG
```

#### Success Response (200)

```json
{
  "status": "success",
  "count": 2,
  "data": [
    {
      "id": "id-1",
      "name": "emmanuel",
      "gender": "male",
      "age": 25,
      "age_group": "adult",
      "country_id": "NG"
    }
  ]
}
```

---

### 4. Delete Profile

**DELETE** `/api/profiles/{id}`

#### Success Response

```
204 No Content
```

---

## ❌ Error Handling

All errors follow this format:

```json
{
  "status": "error",
  "message": "error message"
}
```

### Status Codes

* **400** → Missing or empty name
* **422** → Invalid type
* **404** → Profile not found
* **502** → External API failure

---

## ⚠️ External API Failure Format

```json
{
  "status": "error",
  "message": "Genderize returned an invalid response"
}
```

---

## 🧠 Classification Logic

### Age Groups

* 0–12 → child
* 13–19 → teenager
* 20–59 → adult
* 60+ → senior

### Nationality

* Country with the highest probability from Nationalize API

---

## 🔒 Edge Cases Handled

* Genderize returns null or count = 0 → 502
* Agify returns null age → 502
* Nationalize returns no country → 502
* Duplicate names return existing profile

---

## 🌍 CORS

```
Access-Control-Allow-Origin: *
```

---

## 🆔 ID Format

* UUID v7

---

## 🕒 Timestamps

* UTC
* ISO 8601 format

---

## 🚀 Deployment

* Publicly accessible base URL
* HTTPS enabled
* CORS configured

---

## 👤 Author

Precious Adjerese
