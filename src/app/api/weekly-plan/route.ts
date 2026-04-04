import { generateWeeklyPlan } from "@/lib/recommendation";
import type { Preferences } from "@/types/domain";

export async function POST(request: Request) {
  const body = (await request.json()) as { preferences?: Preferences };
  if (!body.preferences) {
    return Response.json({ error: "preferences are required." }, { status: 400 });
  }

  const days = generateWeeklyPlan(body.preferences);
  return Response.json({ days });
}
