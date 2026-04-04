import { seedRecipes } from "@/lib/recipes";
import type { MealSlot, Preferences, Recipe, SuggestionResult, WeekdayKey } from "@/types/domain";

function slotDietForDay(preferences: Preferences, day: WeekdayKey) {
  const dayDiet = preferences.dayWiseDiet[day];
  return dayDiet || preferences.dietMode;
}

function byDiet(recipe: Recipe, dietMode: Preferences["dietMode"]) {
  if (dietMode === "mix") return true;
  return recipe.dietMode === dietMode;
}

function byExclusion(recipe: Recipe, excluded: string[]) {
  if (excluded.length === 0) return true;
  const lower = excluded.map((item) => item.trim().toLowerCase());
  return !recipe.ingredients.some((ingredient) => lower.includes(ingredient.name.toLowerCase()));
}

function scoreRecipe(recipe: Recipe, preferences: Preferences, history: string[]) {
  const totalTime = recipe.prepTimeMins + recipe.cookTimeMins;
  const timeGap = Math.abs(preferences.maxCookTimeMins - totalTime);
  const timeFit = Math.max(0, 30 - timeGap);
  const spiceFit = recipe.spiceLevel === preferences.spiceLevel ? 20 : 8;
  const popularity = recipe.popularityScore / 2;
  const freshnessPenalty = history.includes(recipe.id) ? -25 : 10;
  return popularity + timeFit + spiceFit + freshnessPenalty;
}

export function getSuggestion(input: {
  mealSlot: MealSlot;
  day: WeekdayKey;
  preferences: Preferences;
  history: string[];
}): SuggestionResult | null {
  const effectiveDiet = slotDietForDay(input.preferences, input.day);
  const candidates = seedRecipes
    .filter((recipe) => recipe.mealSlot === input.mealSlot)
    .filter((recipe) => byDiet(recipe, effectiveDiet))
    .filter((recipe) => byExclusion(recipe, input.preferences.excludedIngredients))
    .filter((recipe) => recipe.prepTimeMins + recipe.cookTimeMins <= input.preferences.maxCookTimeMins);

  const relaxedCandidates =
    candidates.length > 0
      ? candidates
      : seedRecipes
          .filter((recipe) => recipe.mealSlot === input.mealSlot)
          .filter((recipe) => byDiet(recipe, effectiveDiet))
          .filter((recipe) => byExclusion(recipe, input.preferences.excludedIngredients));

  const ranked = [...relaxedCandidates].sort(
    (left, right) =>
      scoreRecipe(right, input.preferences, input.history) -
      scoreRecipe(left, input.preferences, input.history),
  );

  if (ranked.length === 0) return null;

  const [primary, ...rest] = ranked;
  return {
    primarySuggestion: primary,
    alternatives: rest.slice(0, 2),
    explanation: "Based on your diet, time budget, and recent history to avoid repetition.",
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
