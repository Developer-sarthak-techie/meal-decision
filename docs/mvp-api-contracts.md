# MVP API Contracts

## POST /api/auth/login
Request:
```json
{ "email": "user@example.com", "password": "secret" }
```
Response:
```json
{ "user": { "id": "u_1", "name": "Riya", "email": "user@example.com" }, "token": "mock-token" }
```

## GET /api/recipes?mealSlot=lunch&dietMode=veg
Response:
```json
{ "recipes": [{ "id": "r1", "name": "Veg Pulao" }] }
```

## POST /api/suggest
Request:
```json
{
  "mealSlot": "dinner",
  "preferences": { "dietMode": "mix", "maxCookTimeMins": 35, "excludedIngredients": [] },
  "history": ["r2", "r4"]
}
```
Response:
```json
{
  "primarySuggestion": { "id": "r7", "name": "Paneer Bhurji" },
  "alternatives": [{ "id": "r8", "name": "Dal Tadka" }, { "id": "r9", "name": "Egg Curry" }],
  "explanation": "Fast prep and not repeated recently"
}
```

## POST /api/weekly-plan
Request:
```json
{
  "preferences": { "dietMode": "veg", "maxCookTimeMins": 40 },
  "dayWiseDiet": { "sun": "non_veg", "mon": "veg" }
}
```
Response:
```json
{
  "days": [
    { "day": "Mon", "breakfastRecipeId": "r1", "lunchRecipeId": "r2", "dinnerRecipeId": "r3", "snackRecipeId": "r4" }
  ]
}
```

## POST /api/integrations/cart
Request:
```json
{ "partner": "zepto", "items": [{ "name": "Tomato", "quantity": "500g" }] }
```
Response:
```json
{ "checkoutUrl": "https://partner.example/cart/mock-token", "status": "preview" }
```
