"use client";

import { motion } from "framer-motion";
import { Moon, ShoppingCart, Sparkles, Sun, Wand2 } from "lucide-react";
import { useEffect, useMemo, useState } from "react";

import { readEvents, trackEvent } from "@/lib/analytics";
import type {
  DietMode,
  MealSlot,
  Preferences,
  Recipe,
  SuggestionResult,
  WeekdayKey,
  WeeklyPlanDay,
} from "@/types/domain";

const dayKeys: WeekdayKey[] = ["mon", "tue", "wed", "thu", "fri", "sat", "sun"];
const mealSlots: MealSlot[] = ["breakfast", "lunch", "dinner", "snack"];
const themes = ["light", "dark", "crimson"] as const;

type ThemeMode = (typeof themes)[number];

const defaultPreferences: Preferences = {
  dietMode: "mix",
  dayWiseDiet: {
    mon: "veg",
    tue: "veg",
    wed: "mix",
    thu: "veg",
    fri: "mix",
    sat: "non_veg",
    sun: "non_veg",
  },
  maxCookTimeMins: 35,
  spiceLevel: "medium",
  excludedIngredients: [],
  preferredMealSlots: mealSlots,
};

export default function Home() {
  const [theme, setTheme] = useState<ThemeMode>(() => {
    if (typeof window === "undefined") return "dark";
    const savedTheme = window.localStorage.getItem("meal-theme") as ThemeMode | null;
    return savedTheme && themes.includes(savedTheme) ? savedTheme : "dark";
  });
  const [language, setLanguage] = useState<"en" | "hi">("en");
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loggedIn, setLoggedIn] = useState(false);
  const [preferences, setPreferences] = useState<Preferences>(defaultPreferences);
  const [mealSlot, setMealSlot] = useState<MealSlot>("dinner");
  const [day, setDay] = useState<WeekdayKey>("mon");
  const [history, setHistory] = useState<string[]>([]);
  const [suggestion, setSuggestion] = useState<SuggestionResult | null>(null);
  const [selectedRecipe, setSelectedRecipe] = useState<Recipe | null>(null);
  const [weeklyPlan, setWeeklyPlan] = useState<WeeklyPlanDay[]>([]);
  const [partner, setPartner] = useState<"zepto" | "blinkit" | "instamart" | "bigbasket">("zepto");
  const [cartUrl, setCartUrl] = useState("");
  const [eventCount, setEventCount] = useState(() =>
    typeof window === "undefined" ? 0 : readEvents().length,
  );
  const [feedback, setFeedback] = useState<"cooked" | "skipped" | "saved" | null>(null);

  const text = useMemo(
    () =>
      language === "en"
        ? {
            title: "Aaj Kya Banaye - Meal Decision",
            subtitle: "Decide today's meal in under 90 seconds",
            login: "Login",
            suggest: "Suggest now",
            weekly: "Generate weekly routine",
            swap: "Swap suggestion",
            recipe: "Recipe details",
          }
        : {
            title: "Aaj Kya Banaye - Meal Decision",
            subtitle: "90 second mein aaj ka meal decide karein",
            login: "Login",
            suggest: "Ab suggest karo",
            weekly: "Weekly routine generate karo",
            swap: "Suggestion badlo",
            recipe: "Recipe details",
          },
    [language],
  );

  useEffect(() => {
    document.documentElement.dataset.theme = theme;
    window.localStorage.setItem("meal-theme", theme);
  }, [theme]);

  const cycleTheme = () => {
    const idx = themes.indexOf(theme);
    setTheme(themes[(idx + 1) % themes.length]);
  };

  const handleLogin = async () => {
    const response = await fetch("/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password, name }),
    });
    if (!response.ok) return;
    setLoggedIn(true);
    trackEvent({ name: "auth_login", ts: new Date().toISOString(), payload: { email } });
    setEventCount(readEvents().length);
  };

  const requestSuggestion = async () => {
    const response = await fetch("/api/suggest", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ mealSlot, day, preferences, history }),
    });
    if (!response.ok) return;
    const result = (await response.json()) as SuggestionResult;
    setSuggestion(result);
    setSelectedRecipe(result.primarySuggestion);
    setHistory((prev) => [...prev, result.primarySuggestion.id].slice(-8));
    trackEvent({
      name: "suggestion_requested",
      ts: new Date().toISOString(),
      payload: { mealSlot, day, recipeId: result.primarySuggestion.id },
    });
    setEventCount(readEvents().length);
  };

  const swapSuggestion = () => {
    if (!suggestion || suggestion.alternatives.length === 0) return;
    const [next, ...rest] = suggestion.alternatives;
    setSuggestion({
      primarySuggestion: next,
      alternatives: [...rest, suggestion.primarySuggestion].slice(0, 2),
      explanation: suggestion.explanation,
    });
    setSelectedRecipe(next);
    trackEvent({
      name: "suggestion_swapped",
      ts: new Date().toISOString(),
      payload: { to: next.id },
    });
    setEventCount(readEvents().length);
  };

  const generateWeek = async () => {
    const response = await fetch("/api/weekly-plan", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ preferences }),
    });
    if (!response.ok) return;
    const body = (await response.json()) as { days: WeeklyPlanDay[] };
    setWeeklyPlan(body.days);
    trackEvent({
      name: "weekly_plan_generated",
      ts: new Date().toISOString(),
      payload: { totalDays: body.days.length },
    });
    setEventCount(readEvents().length);
  };

  const previewCart = async () => {
    if (!selectedRecipe) return;
    const response = await fetch("/api/integrations/cart", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ partner, items: selectedRecipe.ingredients }),
    });
    if (!response.ok) return;
    const body = (await response.json()) as { checkoutUrl: string };
    setCartUrl(body.checkoutUrl);
    trackEvent({
      name: "cart_handoff_preview",
      ts: new Date().toISOString(),
      payload: { partner, recipeId: selectedRecipe.id },
    });
    setEventCount(readEvents().length);
  };

  const submitFeedback = (type: "cooked" | "skipped" | "saved") => {
    setFeedback(type);
    trackEvent({
      name: "suggestion_requested",
      ts: new Date().toISOString(),
      payload: { feedback: type, recipeId: selectedRecipe?.id ?? "none" },
    });
    setEventCount(readEvents().length);
  };

  const updateExcluded = (value: string) => {
    const excludedIngredients = value
      .split(",")
      .map((item) => item.trim())
      .filter(Boolean);
    setPreferences((prev) => ({ ...prev, excludedIngredients }));
  };

  return (
    <main className="px-4 py-8 sm:px-8">
      <div className="mx-auto max-w-6xl space-y-6">
        <motion.header
          initial={{ opacity: 0, y: -12 }}
          animate={{ opacity: 1, y: 0 }}
          className="glass sticky top-4 z-10 rounded-3xl p-4"
        >
          <div className="flex flex-wrap items-center justify-between gap-3">
            <div>
              <p className="text-xs uppercase tracking-[0.3em] text-[var(--muted)]">Meal OS</p>
              <h1 className="text-2xl font-semibold">{text.title}</h1>
              <p className="text-sm text-[var(--muted)]">{text.subtitle}</p>
            </div>
            <div className="flex items-center gap-2">
              <button
                type="button"
                onClick={() => setLanguage((prev) => (prev === "en" ? "hi" : "en"))}
                className="chip"
              >
                {language === "en" ? "Hindi" : "English"}
              </button>
              <motion.button
                whileHover={{ scale: 1.08, y: -2 }}
                whileTap={{ scale: 0.94 }}
                type="button"
                onClick={cycleTheme}
                className="chip inline-flex items-center gap-2"
              >
                {theme === "light" ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
                {theme}
              </motion.button>
            </div>
          </div>
        </motion.header>

        {!loggedIn ? (
          <motion.section
            initial={{ opacity: 0, y: 12 }}
            animate={{ opacity: 1, y: 0 }}
            className="glass grid gap-4 rounded-3xl p-6 sm:grid-cols-2"
          >
            <input
              value={name}
              onChange={(event) => setName(event.target.value)}
              placeholder="Name"
              className="rounded-2xl border border-[var(--panel-border)] bg-transparent px-4 py-3"
            />
            <input
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              placeholder="Email"
              className="rounded-2xl border border-[var(--panel-border)] bg-transparent px-4 py-3"
            />
            <input
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              placeholder="Password"
              className="rounded-2xl border border-[var(--panel-border)] bg-transparent px-4 py-3"
            />
            <motion.button
              whileHover={{ scale: 1.02, y: -2 }}
              whileTap={{ scale: 0.97 }}
              type="button"
              onClick={handleLogin}
              className="button-accent rounded-2xl px-4 py-3 font-semibold"
            >
              {text.login}
            </motion.button>
          </motion.section>
        ) : null}

        {loggedIn ? (
          <>
            <section className="grid gap-6 lg:grid-cols-[1fr_1fr]">
              <motion.div whileHover={{ y: -4 }} className="glass rounded-3xl p-5">
                <p className="mb-3 text-sm font-medium text-[var(--muted)]">Preferences</p>
                <div className="space-y-3">
                  <select
                    className="w-full rounded-2xl border border-[var(--panel-border)] bg-transparent px-4 py-3"
                    value={preferences.dietMode}
                    onChange={(event) =>
                      setPreferences((prev) => ({ ...prev, dietMode: event.target.value as DietMode }))
                    }
                  >
                    <option value="veg">Veg</option>
                    <option value="non_veg">Non-Veg</option>
                    <option value="mix">Mix</option>
                  </select>
                  <input
                    type="number"
                    min={10}
                    max={90}
                    value={preferences.maxCookTimeMins}
                    onChange={(event) =>
                      setPreferences((prev) => ({
                        ...prev,
                        maxCookTimeMins: Number(event.target.value),
                      }))
                    }
                    className="w-full rounded-2xl border border-[var(--panel-border)] bg-transparent px-4 py-3"
                    placeholder="Max cooking time"
                  />
                  <input
                    type="text"
                    placeholder="Excluded ingredients (comma separated)"
                    onChange={(event) => updateExcluded(event.target.value)}
                    className="w-full rounded-2xl border border-[var(--panel-border)] bg-transparent px-4 py-3"
                  />
                </div>
              </motion.div>

              <motion.div whileHover={{ y: -4 }} className="glass rounded-3xl p-5">
                <p className="mb-3 text-sm font-medium text-[var(--muted)]">Suggestion Controls</p>
                <div className="grid gap-3 sm:grid-cols-2">
                  <select
                    value={mealSlot}
                    onChange={(event) => setMealSlot(event.target.value as MealSlot)}
                    className="rounded-2xl border border-[var(--panel-border)] bg-transparent px-4 py-3"
                  >
                    {mealSlots.map((slot) => (
                      <option key={slot} value={slot}>
                        {slot}
                      </option>
                    ))}
                  </select>
                  <select
                    value={day}
                    onChange={(event) => setDay(event.target.value as WeekdayKey)}
                    className="rounded-2xl border border-[var(--panel-border)] bg-transparent px-4 py-3"
                  >
                    {dayKeys.map((item) => (
                      <option key={item} value={item}>
                        {item.toUpperCase()}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="mt-4 flex flex-wrap gap-3">
                  <motion.button
                    whileHover={{ scale: 1.03, y: -2 }}
                    whileTap={{ scale: 0.97 }}
                    onClick={requestSuggestion}
                    className="button-accent inline-flex items-center gap-2 rounded-2xl px-4 py-3 font-semibold"
                    type="button"
                  >
                    <Wand2 className="h-4 w-4" />
                    {text.suggest}
                  </motion.button>
                  <motion.button
                    whileHover={{ scale: 1.03 }}
                    whileTap={{ scale: 0.97 }}
                    onClick={swapSuggestion}
                    className="chip inline-flex items-center gap-2"
                    type="button"
                  >
                    <Sparkles className="h-4 w-4" />
                    {text.swap}
                  </motion.button>
                  <motion.button
                    whileHover={{ scale: 1.03 }}
                    whileTap={{ scale: 0.97 }}
                    onClick={generateWeek}
                    className="chip"
                    type="button"
                  >
                    {text.weekly}
                  </motion.button>
                </div>
              </motion.div>
            </section>

            <section className="grid gap-6 lg:grid-cols-[1fr_1fr]">
              <motion.article whileHover={{ y: -4 }} className="glass rounded-3xl p-5">
                <p className="text-sm font-medium text-[var(--muted)]">Primary suggestion</p>
                {suggestion ? (
                  <div className="mt-3 space-y-2">
                    <h3 className="text-2xl font-semibold">{suggestion.primarySuggestion.name}</h3>
                    <p className="text-sm text-[var(--muted)]">{suggestion.explanation}</p>
                    <div className="flex flex-wrap gap-2">
                      {suggestion.alternatives.map((item) => (
                        <button
                          key={item.id}
                          type="button"
                          onClick={() => setSelectedRecipe(item)}
                          className="chip"
                        >
                          {item.name}
                        </button>
                      ))}
                    </div>
                  </div>
                ) : (
                  <p className="mt-3 text-sm text-[var(--muted)]">
                    Generate a suggestion to see meal recommendations.
                  </p>
                )}
              </motion.article>

              <motion.article whileHover={{ y: -4 }} className="glass rounded-3xl p-5">
                <p className="text-sm font-medium text-[var(--muted)]">{text.recipe}</p>
                {selectedRecipe ? (
                  <div className="mt-3 space-y-3">
                    <h3 className="text-xl font-semibold">{selectedRecipe.name}</h3>
                    <p className="text-sm text-[var(--muted)]">
                      {selectedRecipe.prepTimeMins + selectedRecipe.cookTimeMins} mins ·{" "}
                      {selectedRecipe.cuisine}
                    </p>
                    <ul className="space-y-1 text-sm">
                      {selectedRecipe.ingredients.map((ingredient) => (
                        <li key={ingredient.name}>
                          - {ingredient.name}: {ingredient.quantity}
                        </li>
                      ))}
                    </ul>
                    <ol className="list-decimal space-y-1 pl-5 text-sm">
                      {selectedRecipe.steps.map((step) => (
                        <li key={step}>{step}</li>
                      ))}
                    </ol>
                    <div className="flex flex-wrap items-center gap-2">
                      <select
                        value={partner}
                        onChange={(event) =>
                          setPartner(event.target.value as "zepto" | "blinkit" | "instamart" | "bigbasket")
                        }
                        className="rounded-xl border border-[var(--panel-border)] bg-transparent px-3 py-2 text-sm"
                      >
                        <option value="zepto">Zepto</option>
                        <option value="blinkit">Blinkit</option>
                        <option value="instamart">Instamart</option>
                        <option value="bigbasket">BigBasket</option>
                      </select>
                      <motion.button
                        whileHover={{ scale: 1.02 }}
                        whileTap={{ scale: 0.97 }}
                        onClick={previewCart}
                        type="button"
                        className="chip inline-flex items-center gap-2"
                      >
                        <ShoppingCart className="h-4 w-4" />
                        Preview cart handoff
                      </motion.button>
                    </div>
                    {cartUrl ? (
                      <p className="text-xs text-[var(--muted)]">
                        Partner checkout preview: <span className="font-medium">{cartUrl}</span>
                      </p>
                    ) : null}
                  </div>
                ) : (
                  <p className="mt-3 text-sm text-[var(--muted)]">
                    Choose a recipe from suggestion to view full details.
                  </p>
                )}
              </motion.article>
            </section>

            <motion.section whileHover={{ y: -2 }} className="glass rounded-3xl p-5">
              <p className="mb-3 text-sm font-medium text-[var(--muted)]">Weekly routine</p>
              {weeklyPlan.length > 0 ? (
                <div className="overflow-x-auto">
                  <table className="w-full min-w-[640px] text-left text-sm">
                    <thead>
                      <tr className="text-[var(--muted)]">
                        <th className="pb-2">Day</th>
                        <th className="pb-2">Breakfast</th>
                        <th className="pb-2">Lunch</th>
                        <th className="pb-2">Dinner</th>
                        <th className="pb-2">Snack</th>
                      </tr>
                    </thead>
                    <tbody>
                      {weeklyPlan.map((item) => (
                        <tr key={item.day} className="border-t border-[var(--panel-border)]">
                          <td className="py-2 uppercase">{item.day}</td>
                          <td className="py-2">{item.breakfast?.name || "-"}</td>
                          <td className="py-2">{item.lunch?.name || "-"}</td>
                          <td className="py-2">{item.dinner?.name || "-"}</td>
                          <td className="py-2">{item.snack?.name || "-"}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              ) : (
                <p className="text-sm text-[var(--muted)]">Generate your weekly plan from the controls above.</p>
              )}
            </motion.section>

            <section className="grid gap-6 lg:grid-cols-2">
              <motion.article whileHover={{ y: -3 }} className="glass rounded-3xl p-5">
                <p className="text-sm font-medium text-[var(--muted)]">Premium plans</p>
                <div className="mt-4 grid gap-3 sm:grid-cols-3">
                  {[
                    { name: "Free", price: "INR 0", perks: "Daily suggestions + recipes" },
                    { name: "Plus", price: "INR 149/mo", perks: "Unlimited swaps + pantry mode" },
                    { name: "Pro Family", price: "INR 299/mo", perks: "Family voting + budget planner" },
                  ].map((plan) => (
                    <div key={plan.name} className="rounded-2xl border border-[var(--panel-border)] p-3">
                      <p className="font-semibold">{plan.name}</p>
                      <p className="mt-1 text-sm">{plan.price}</p>
                      <p className="mt-2 text-xs text-[var(--muted)]">{plan.perks}</p>
                    </div>
                  ))}
                </div>
              </motion.article>
              <motion.article whileHover={{ y: -3 }} className="glass rounded-3xl p-5">
                <p className="text-sm font-medium text-[var(--muted)]">Beta feedback loop</p>
                <p className="mt-2 text-sm">
                  Mark what happened with this recommendation to improve weekly quality.
                </p>
                <div className="mt-4 flex flex-wrap gap-2">
                  {[
                    { id: "cooked", label: "I cooked this" },
                    { id: "saved", label: "Saved for later" },
                    { id: "skipped", label: "Skipped" },
                  ].map((item) => (
                    <button
                      key={item.id}
                      type="button"
                      className="chip"
                      onClick={() => submitFeedback(item.id as "cooked" | "skipped" | "saved")}
                    >
                      {item.label}
                    </button>
                  ))}
                </div>
                {feedback ? <p className="mt-3 text-xs text-[var(--muted)]">Recorded: {feedback}</p> : null}
              </motion.article>
            </section>
          </>
        ) : null}

        <footer className="glass rounded-3xl p-4 text-sm text-[var(--muted)]">
          Events tracked in MVP: <span className="font-semibold text-[var(--foreground)]">{eventCount}</span>
        </footer>
      </div>
    </main>
  );
}
