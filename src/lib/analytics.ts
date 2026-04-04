import type { AnalyticsEvent } from "@/types/domain";

const EVENT_STORAGE_KEY = "meal-decision-events";

export function trackEvent(event: AnalyticsEvent) {
  if (typeof window === "undefined") return;
  const raw = window.localStorage.getItem(EVENT_STORAGE_KEY);
  const current: AnalyticsEvent[] = raw ? JSON.parse(raw) : [];
  const next = [...current, event].slice(-500);
  window.localStorage.setItem(EVENT_STORAGE_KEY, JSON.stringify(next));
  void fetch("/api/analytics", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(event),
  }).catch(() => undefined);
}

export function readEvents() {
  if (typeof window === "undefined") return [] as AnalyticsEvent[];
  const raw = window.localStorage.getItem(EVENT_STORAGE_KEY);
  return raw ? (JSON.parse(raw) as AnalyticsEvent[]) : [];
}
