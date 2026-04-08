export type DietMode = "veg" | "non_veg" | "mix";
export type MealSlot = "breakfast" | "lunch" | "dinner" | "snack";
export type SpiceLevel = "low" | "medium" | "high";
export type WeekdayKey = "mon" | "tue" | "wed" | "thu" | "fri" | "sat" | "sun";
export type NoveltyLevel = "classic" | "balanced" | "experimental";

/** Festival / occasion — filters recipes by `occasionTags` on each dish. */
export type OccasionKey =
  | "everyday"
  | "diwali"
  | "holi"
  | "eid"
  | "christmas"
  | "navratri"
  | "ganesh_chaturthi"
  | "onam"
  | "birthday"
  | "party";

/** Budget band — used when budget mode is on (matches `estimatedCostINR` on recipes). */
export type BudgetTier = "budget" | "moderate" | "premium";

/** Bread / carb to pair with the main dish (lunch/dinner-style meals). */
export type BreadChoice =
  | "none"
  | "phulka"
  | "tandoori_roti"
  | "butter_naan"
  | "garlic_naan"
  | "laccha_paratha"
  | "aloo_paratha"
  | "missi_roti"
  | "bhakri"
  | "steamed_rice"
  | "jeera_rice"
  | "bread_toast";

export interface UserProfile {
  id: string;
  name: string;
  email: string;
  gender?: "male" | "female" | "other" | "prefer_not_to_say";
  preferredLanguage: "en" | "hi";
}

export interface Preferences {
  dietMode: DietMode;
  dayWiseDiet: Record<WeekdayKey, DietMode>;
  maxCookTimeMins: number;
  spiceLevel: SpiceLevel;
  excludedIngredients: string[];
  preferredMealSlots: MealSlot[];
  preferredCuisines: string[];
  noveltyLevel: NoveltyLevel;
  /** When true, only dishes whose ingredients are all covered by `pantryIngredients` are suggested (ignored if pantry list is empty). */
  pantryMode: boolean;
  pantryIngredients: string[];
  occasion: OccasionKey;
  /** When true, only dishes at or below `budgetTier` estimated cost are suggested. */
  budgetMode: boolean;
  budgetTier: BudgetTier;
  /** Richer step breakdown, tips, and bread pairing in the recipe panel. */
  detailedRecipeMode: boolean;
  /** User-chosen bread or carb to show prep and pairing notes for. */
  breadChoice: BreadChoice;
  /** ≤15 min total time for this suggestion run (tonight / emergency cook). */
  tonightMode: boolean;
  /** Household allergies — excluded from dishes (like exclusions, separate UX). */
  allergenExclusions: string[];
  /** Prefer milder dishes (boost low/medium spice in ranking). */
  kidsFriendly: boolean;
}

export interface RecipeIngredient {
  name: string;
  quantity: string;
}

export interface Recipe {
  id: string;
  name: string;
  mealSlot: MealSlot;
  cuisine: string;
  dietMode: Exclude<DietMode, "mix">;
  spiceLevel: SpiceLevel;
  prepTimeMins: number;
  cookTimeMins: number;
  ingredients: RecipeIngredient[];
  steps: string[];
  popularityScore: number;
  /** Rough estimated ingredient cost for one meal (INR), for budget-aware suggestions. */
  estimatedCostINR: number;
  /** Tags for festival / occasion matching; always includes `everyday`. */
  occasionTags: OccasionKey[];
}

export interface SuggestionResult {
  primarySuggestion: Recipe;
  alternatives: Recipe[];
  explanation: string;
  uniqueCookIdea: string;
}

export interface WeeklyPlanDay {
  day: WeekdayKey;
  breakfast?: Recipe;
  lunch?: Recipe;
  dinner?: Recipe;
  snack?: Recipe;
}

export interface AnalyticsEvent {
  name:
    | "auth_login"
    | "onboarding_complete"
    | "suggestion_requested"
    | "suggestion_swapped"
    | "weekly_plan_generated"
    | "cart_handoff_preview"
    | "feedback_submitted"
    | "weekly_plan_shared"
    | "proof_cooking_confirmed";
  ts: string;
  payload?: Record<string, string | number | boolean | null>;
}
