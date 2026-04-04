# Recommendation Engine Spec

## Goal
Return a relevant dish for a given user, meal slot, and day while minimizing repetition.

## Inputs
- `dietMode`: `veg` | `non_veg` | `mix`
- `mealSlot`: `breakfast` | `lunch` | `dinner` | `snack`
- `dayType`: weekday/weekend
- `maxCookTimeMins`
- `spiceLevel`: low/medium/high
- `excludedIngredients[]`
- `history[]`: recently suggested/cooked recipe IDs

## Candidate Filtering
1. Match meal slot.
2. Respect diet mode and day-wise restriction.
3. Remove recipes with excluded ingredients.
4. Remove recipes above max cook time.

## Scoring
`score = freshness + popularity + timeFit + preferenceFit`

- `freshness`: penalize recipes from last 3 suggestions.
- `popularity`: static seed weight for common household acceptance.
- `timeFit`: higher score for recipes closer to preferred cook time.
- `preferenceFit`: boost by cuisine or spice preference.

## Anti-repetition
- Hard block: exact recipe suggested in previous day same meal slot.
- Soft penalty: recipe repeated within last 3 days.

## Fallbacks
- If filtered set is empty, relax cook time by +10 mins.
- If still empty, return top popular recipe for slot and selected diet mode.

## Output
- `primarySuggestion`
- `alternatives` (2 items)
- `explanation` (why this suggestion)

## Personalization V2
- Add feedback weights from cooked/skipped/rated/swap signals.
- Per-household preference vector updated nightly.
