import type { Preferences, Recipe } from "@/types/domain";

export function formatUniqueCookIdea(recipe: Recipe, preferences: Preferences): string {
  const noveltyLevel = preferences.noveltyLevel ?? "balanced";
  const firstIngredient = recipe.ingredients[0]?.name ?? "main ingredient";
  const noveltyTip =
    noveltyLevel === "classic"
      ? "Finish with a simple tempered ghee drizzle for comfort-style flavor."
      : noveltyLevel === "experimental"
        ? "Try a fusion plating: wrap the final dish in lettuce cups and add chili-lime yogurt."
        : "Serve it two ways: half classic, half with toasted seeds and lemon zest.";
  return `Unique twist: For ${recipe.name}, lightly roast ${firstIngredient.toLowerCase()} first to deepen flavor. ${noveltyTip}`;
}
