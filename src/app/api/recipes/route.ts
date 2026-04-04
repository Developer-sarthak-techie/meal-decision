import { seedRecipes } from "@/lib/recipes";
import type { DietMode, MealSlot } from "@/types/domain";

export async function GET(request: Request) {
  const { searchParams } = new URL(request.url);
  const mealSlot = searchParams.get("mealSlot") as MealSlot | null;
  const dietMode = searchParams.get("dietMode") as DietMode | null;

  const recipes = seedRecipes.filter((recipe) => {
    const slotOk = mealSlot ? recipe.mealSlot === mealSlot : true;
    const dietOk = !dietMode || dietMode === "mix" ? true : recipe.dietMode === dietMode;
    return slotOk && dietOk;
  });

  return Response.json({ recipes });
}
