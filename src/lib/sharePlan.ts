import type { WeeklyPlanDay } from "@/types/domain";

export function formatWeeklyPlanForShare(days: WeeklyPlanDay[]): string {
  if (days.length === 0) return "";
  const lines = ["🍽️ Meal Decision — Weekly plan", "—".repeat(36), ""];
  for (const d of days) {
    lines.push(`${d.day.toUpperCase()}`);
    lines.push(`  Breakfast: ${d.breakfast?.name ?? "—"}`);
    lines.push(`  Lunch:     ${d.lunch?.name ?? "—"}`);
    lines.push(`  Dinner:    ${d.dinner?.name ?? "—"}`);
    lines.push(`  Snack:     ${d.snack?.name ?? "—"}`);
    lines.push("");
  }
  lines.push("Generated with Meal Decision • meal-decision");
  return lines.join("\n");
}
