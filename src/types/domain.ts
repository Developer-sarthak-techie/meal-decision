export type DietMode = "veg" | "non_veg" | "mix";
export type MealSlot = "breakfast" | "lunch" | "dinner" | "snack";
export type SpiceLevel = "low" | "medium" | "high";
export type WeekdayKey = "mon" | "tue" | "wed" | "thu" | "fri" | "sat" | "sun";

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
}

export interface SuggestionResult {
  primarySuggestion: Recipe;
  alternatives: Recipe[];
  explanation: string;
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
    | "cart_handoff_preview";
  ts: string;
  payload?: Record<string, string | number | boolean | null>;
}
