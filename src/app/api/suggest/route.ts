import { getSuggestion } from "@/lib/recommendation";
import type { MealSlot, Preferences, WeekdayKey } from "@/types/domain";

export async function POST(request: Request) {
  const body = (await request.json()) as {
    mealSlot?: MealSlot;
    day?: WeekdayKey;
    preferences?: Preferences;
    history?: string[];
  };

  if (!body.mealSlot || !body.day || !body.preferences) {
    return Response.json({ error: "mealSlot, day, and preferences are required." }, { status: 400 });
  }

  const result = getSuggestion({
    mealSlot: body.mealSlot,
    day: body.day,
    preferences: body.preferences,
    history: body.history ?? [],
  });

  if (!result) {
    return Response.json({ error: "No recipes available for current filters." }, { status: 404 });
  }

  return Response.json(result);
}
