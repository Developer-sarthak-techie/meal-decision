import { formatUniqueCookIdea } from "@/lib/cookIdeas";
import { seedRecipes } from "@/lib/recipes";
import type {
  BudgetTier,
  MealSlot,
  OccasionKey,
  Preferences,
  Recipe,
  SuggestionResult,
  WeekdayKey,
} from "@/types/domain";

function maxCostINRForTier(tier: BudgetTier): number {
  if (tier === "budget") return 220;
  if (tier === "moderate") return 450;
  return 99999;
}

function normalizeToken(s: string) {
  return s
    .toLowerCase()
    .trim()
    .replace(/\s+/g, " ");
}

/** True if pantry stock covers this ingredient name (substring / word overlap). */
function ingredientCoveredByPantry(ingredientName: string, pantry: string[]): boolean {
  const n = normalizeToken(ingredientName);
  if (!n) return false;
  return pantry.some((p) => {
    const q = normalizeToken(p);
    if (!q) return false;
    return n.includes(q) || q.includes(n) || n.split(/\s+/).some((w) => w.length > 2 && (w === q || q.includes(w)));
  });
}

function recipeFullyFromPantry(recipe: Recipe, pantryRaw: string[]): boolean {
  if (pantryRaw.length === 0) return false;
  const pantry = pantryRaw.map(normalizeToken).filter(Boolean);
  return recipe.ingredients.every((ing) => ingredientCoveredByPantry(ing.name, pantry));
}

function byPantry(recipe: Recipe, preferences: Preferences): boolean {
  if (!preferences.pantryMode) return true;
  const list = preferences.pantryIngredients ?? [];
  if (list.length === 0) return true;
  return recipeFullyFromPantry(recipe, list);
}

function byOccasion(recipe: Recipe, occasion: OccasionKey | undefined): boolean {
  const o = occasion ?? "everyday";
  if (o === "everyday") return true;
  return recipe.occasionTags.includes(o);
}

function byBudget(recipe: Recipe, preferences: Preferences): boolean {
  if (!preferences.budgetMode) return true;
  const tier = preferences.budgetTier ?? "moderate";
  return recipe.estimatedCostINR <= maxCostINRForTier(tier);
}

function slotDietForDay(preferences: Preferences, day: WeekdayKey) {
  if (preferences.dietMode !== "mix") return preferences.dietMode;
  const dayDiet = preferences.dayWiseDiet?.[day];
  return dayDiet || "mix";
}

function byDiet(recipe: Recipe, dietMode: Preferences["dietMode"]) {
  if (dietMode === "mix") return true;
  return recipe.dietMode === dietMode;
}

function byCuisine(recipe: Recipe, preferredCuisines?: string[]) {
  if (!preferredCuisines) return true;
  if (preferredCuisines.length === 0) return true;
  return preferredCuisines.some(
    (cuisine) => cuisine.trim().toLowerCase() === recipe.cuisine.trim().toLowerCase(),
  );
}

function byExclusion(recipe: Recipe, excluded: string[]) {
  if (excluded.length === 0) return true;
  const lower = excluded.map((item) => item.trim().toLowerCase());
  return !recipe.ingredients.some((ingredient) => lower.includes(ingredient.name.toLowerCase()));
}

function allExcludedIngredients(prefs: Preferences): string[] {
  const allergens = prefs.allergenExclusions ?? [];
  return [...prefs.excludedIngredients, ...allergens];
}

function effectiveMaxCookMins(prefs: Preferences): number {
  return prefs.tonightMode ? 15 : prefs.maxCookTimeMins;
}

function scoreRecipe(recipe: Recipe, preferences: Preferences, history: string[]) {
  const noveltyLevel = preferences.noveltyLevel ?? "balanced";
  const totalTime = recipe.prepTimeMins + recipe.cookTimeMins;
  const timeGap = Math.abs(effectiveMaxCookMins(preferences) - totalTime);
  const timeFit = Math.max(0, 30 - timeGap);
  const spiceFit = recipe.spiceLevel === preferences.spiceLevel ? 20 : 8;
  let kidsBoost = 0;
  if (preferences.kidsFriendly) {
    if (recipe.spiceLevel === "low") kidsBoost = 18;
    else if (recipe.spiceLevel === "medium") kidsBoost = 8;
    else kidsBoost = -14;
  }
  const popularity = recipe.popularityScore / 2;
  const freshnessBoost = history.includes(recipe.id) ? -25 : 12;
  const noveltyBoost =
    noveltyLevel === "classic"
      ? recipe.popularityScore > 80
        ? 14
        : 2
      : noveltyLevel === "experimental"
        ? recipe.popularityScore < 85
          ? 16
          : 4
        : 8;
  let budgetBoost = 0;
  if (preferences.budgetMode) {
    const tier = preferences.budgetTier ?? "moderate";
    const cap = maxCostINRForTier(tier);
    const headroom = cap - recipe.estimatedCostINR;
    budgetBoost = Math.min(35, Math.max(0, headroom / 10));
  }
  return popularity + timeFit + spiceFit + freshnessBoost + noveltyBoost + budgetBoost + kidsBoost;
}

function baseFilter(
  recipe: Recipe,
  input: { mealSlot: MealSlot; preferences: Preferences; day: WeekdayKey },
  options: { skipPantry?: boolean; skipBudget?: boolean; skipTime?: boolean },
) {
  const effectiveDiet = slotDietForDay(input.preferences, input.day);
  const prefs = input.preferences;
  if (recipe.mealSlot !== input.mealSlot) return false;
  if (!byDiet(recipe, effectiveDiet)) return false;
  if (!byCuisine(recipe, prefs.preferredCuisines ?? [])) return false;
  if (!byExclusion(recipe, allExcludedIngredients(prefs))) return false;
  if (!options.skipPantry && !byPantry(recipe, prefs)) return false;
  if (!byOccasion(recipe, prefs.occasion)) return false;
  if (!options.skipBudget && !byBudget(recipe, prefs)) return false;
  if (!options.skipTime && recipe.prepTimeMins + recipe.cookTimeMins > effectiveMaxCookMins(prefs))
    return false;
  return true;
}

function collectCandidates(
  input: { mealSlot: MealSlot; preferences: Preferences; day: WeekdayKey },
  options: { skipPantry?: boolean; skipBudget?: boolean; skipTime?: boolean },
) {
  return seedRecipes.filter((recipe) => baseFilter(recipe, input, options));
}

export function getSuggestion(input: {
  mealSlot: MealSlot;
  day: WeekdayKey;
  preferences: Preferences;
  history: string[];
}): SuggestionResult | null {
  const prefs = input.preferences;
  const tryLayers: Array<{ skipPantry?: boolean; skipBudget?: boolean; skipTime?: boolean; note: string }> = [
    { note: "" },
    { skipTime: true, note: " Cook time limit was relaxed to find a match." },
    { skipPantry: true, note: " Pantry filter was relaxed — list more ingredients for stricter pantry-only picks." },
    { skipBudget: true, note: " Budget cap was relaxed — raise tier or turn off budget mode for cheaper picks." },
    { skipTime: true, skipPantry: true, note: " Cook time and pantry filters were relaxed." },
    { skipTime: true, skipBudget: true, note: " Cook time and budget filters were relaxed." },
    { skipPantry: true, skipBudget: true, note: " Pantry and budget filters were relaxed." },
    {
      skipTime: true,
      skipPantry: true,
      skipBudget: true,
      note: " Several filters were relaxed to surface any suitable dish.",
    },
  ];

  let explanationSuffix = "";
  let ranked: Recipe[] = [];

  for (const layer of tryLayers) {
    const pool = collectCandidates(input, layer);
    ranked = [...pool].sort(
      (left, right) =>
        scoreRecipe(right, prefs, input.history) - scoreRecipe(left, prefs, input.history),
    );
    if (ranked.length > 0) {
      explanationSuffix = layer.note;
      break;
    }
  }

  if (ranked.length === 0) return null;

  const [primary, ...rest] = ranked;
  const parts: string[] = [
    "Matched to diet, cuisine, and history.",
    prefs.pantryMode && (prefs.pantryIngredients?.length ?? 0) > 0
      ? "Pantry mode: every ingredient in the dish is covered by your pantry list when possible."
      : null,
    prefs.occasion && prefs.occasion !== "everyday"
      ? `Occasion: dishes tagged for ${prefs.occasion.replace(/_/g, " ")}.`
      : null,
    prefs.budgetMode
      ? `Budget mode: estimated cost around ₹${primary.estimatedCostINR} (within your ${prefs.budgetTier} band).`
      : null,
    prefs.tonightMode ? "Tonight mode: 15-minute total cook budget." : null,
    (prefs.allergenExclusions?.length ?? 0) > 0 ? "Allergen-safe: avoided listed household allergens." : null,
    prefs.kidsFriendly ? "Kids-friendly bias: milder spice preferred." : null,
  ].filter(Boolean) as string[];

  return {
    primarySuggestion: primary,
    alternatives: rest.slice(0, 2),
    explanation: `${parts.join(" ")}${explanationSuffix}`,
    uniqueCookIdea: formatUniqueCookIdea(primary, prefs),
  };
}

export function generateWeeklyPlan(preferences: Preferences) {
  const days: WeekdayKey[] = ["mon", "tue", "wed", "thu", "fri", "sat", "sun"];
  const history: string[] = [];
  return days.map((day) => {
    const breakfast = getSuggestion({ mealSlot: "breakfast", day, preferences, history })?.primarySuggestion;
    if (breakfast) history.push(breakfast.id);
    const lunch = getSuggestion({ mealSlot: "lunch", day, preferences, history })?.primarySuggestion;
    if (lunch) history.push(lunch.id);
    const dinner = getSuggestion({ mealSlot: "dinner", day, preferences, history })?.primarySuggestion;
    if (dinner) history.push(dinner.id);
    const snack = getSuggestion({ mealSlot: "snack", day, preferences, history })?.primarySuggestion;
    if (snack) history.push(snack.id);
    return { day, breakfast, lunch, dinner, snack };
  });
}
