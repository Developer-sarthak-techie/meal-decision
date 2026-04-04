# Grocery Integration Design

## Objective
Enable one-click ingredient handoff from selected recipe to grocery partners.

## Integration Layer
- Normalized ingredient model: `name`, `quantity`, `unit`.
- Partner adapter interface:
  - `buildCartPayload(items)`
  - `createPreviewCart(payload)`
  - `getCheckoutUrl(response)`

## MVP Partner Strategy
- Start with preview-mode adapters:
  - Zepto
  - Blinkit
  - Instamart
  - BigBasket
- Return preview URL for user review before ordering.

## Future Live Mode
- OAuth/API key onboarding for each partner.
- SKU mapping cache by city and partner.
- Retry queue for failed cart submissions.
