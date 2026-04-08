"use client";

import { AnimatePresence, motion } from "framer-motion";
import {
  Banknote,
  BookOpen,
  CalendarDays,
  ChefHat,
  Clock3,
  Languages,
  Leaf,
  LogIn,
  Mic,
  Moon,
  Package,
  PartyPopper,
  Share2,
  ShoppingCart,
  Sparkles,
  Sun,
  UserRound,
  Wand2,
  Zap,
} from "lucide-react";
import Link from "next/link";
import { useCallback, useEffect, useMemo, useState } from "react";

import { formatUniqueCookIdea } from "@/lib/cookIdeas";
import {
  applyBudgetWeek,
  applyFamilyVegWeek,
  applyPgHostelPack,
} from "@/lib/presets";
import { formatWeeklyPlanForShare } from "@/lib/sharePlan";
import {
  breadChoiceOptions,
  buildDetailedSteps,
  getBreadChoiceLabel,
  getBreadPairingNotes,
  getChefTips,
  getIngredientPrepNotes,
} from "@/lib/recipeDetails";
import { readEvents, trackEvent } from "@/lib/analytics";
import type {
  BreadChoice,
  BudgetTier,
  DietMode,
  MealSlot,
  NoveltyLevel,
  OccasionKey,
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

/** Browser speech (webkit-prefixed); avoid relying on global DOM typings in CI. */
type PantrySpeechRecognition = {
  lang: string;
  interimResults: boolean;
  maxAlternatives: number;
  onerror: (() => void) | null;
  onresult: ((event: { results: Array<{ 0: { transcript: string } }> }) => void) | null;
  start: () => void;
};
type PantrySpeechRecognitionCtor = new () => PantrySpeechRecognition;

const smoothEase: [number, number, number, number] = [0.22, 1, 0.36, 1];

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
  preferredCuisines: [],
  noveltyLevel: "balanced",
  pantryMode: false,
  pantryIngredients: [],
  occasion: "everyday",
  budgetMode: false,
  budgetTier: "moderate",
  detailedRecipeMode: false,
  breadChoice: "phulka",
  tonightMode: false,
  allergenExclusions: [],
  kidsFriendly: false,
};

const occasionOptions: { value: OccasionKey; label: string }[] = [
  { value: "everyday", label: "Everyday" },
  { value: "diwali", label: "Diwali" },
  { value: "holi", label: "Holi" },
  { value: "eid", label: "Eid" },
  { value: "christmas", label: "Christmas" },
  { value: "navratri", label: "Navratri" },
  { value: "ganesh_chaturthi", label: "Ganesh Chaturthi" },
  { value: "onam", label: "Onam" },
  { value: "birthday", label: "Birthday" },
  { value: "party", label: "Party" },
];

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
  const [smartCartHints, setSmartCartHints] = useState<
    Array<{ name: string; quantity: string; searchQuery: string }> | null
  >(null);
  const [voiceError, setVoiceError] = useState<string | null>(null);
  const avatarStack = ["SR", "NK", "AM", "RK"];
  const quickStats = [
    { label: "Dish catalog", value: "1200+", icon: ChefHat },
    { label: "Meals planned", value: `${history.length}`, icon: CalendarDays },
    { label: "Events tracked", value: `${eventCount}`, icon: Sparkles },
    { label: "Mode", value: preferences.dietMode.replace("_", " "), icon: Leaf },
  ] as const;

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
      uniqueCookIdea: formatUniqueCookIdea(next, preferences),
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
    const body = (await response.json()) as {
      checkoutUrl: string;
      smartItems?: Array<{ name: string; quantity: string; searchQuery: string }>;
    };
    setCartUrl(body.checkoutUrl);
    setSmartCartHints(body.smartItems ?? null);
    trackEvent({
      name: "cart_handoff_preview",
      ts: new Date().toISOString(),
      payload: { partner, recipeId: selectedRecipe.id },
    });
    setEventCount(readEvents().length);
  };

  const shareWeeklyPlan = async () => {
    if (weeklyPlan.length === 0) return;
    const text = formatWeeklyPlanForShare(weeklyPlan);
    try {
      await navigator.clipboard.writeText(text);
      trackEvent({
        name: "weekly_plan_shared",
        ts: new Date().toISOString(),
        payload: { days: weeklyPlan.length },
      });
      setEventCount(readEvents().length);
    } catch {
      /* clipboard denied */
    }
  };

  const submitProofCooking = () => {
    if (!selectedRecipe) return;
    setFeedback("cooked");
    trackEvent({
      name: "feedback_submitted",
      ts: new Date().toISOString(),
      payload: { feedback: "cooked", recipeId: selectedRecipe.id },
    });
    trackEvent({
      name: "proof_cooking_confirmed",
      ts: new Date().toISOString(),
      payload: { recipeId: selectedRecipe.id },
    });
    setEventCount(readEvents().length);
  };

  const appendPantryTokens = useCallback((raw: string) => {
    const parts = raw
      .split(/,| aur | and | और /i)
      .map((s) => s.trim())
      .filter(Boolean);
    if (parts.length === 0) return;
    setPreferences((prev) => ({
      ...prev,
      pantryIngredients: [...new Set([...prev.pantryIngredients, ...parts])],
    }));
  }, []);

  const startPantryVoice = () => {
    setVoiceError(null);
    if (typeof window === "undefined") return;
    const w = window as Window & {
      webkitSpeechRecognition?: PantrySpeechRecognitionCtor;
      SpeechRecognition?: PantrySpeechRecognitionCtor;
    };
    const SpeechRec = w.webkitSpeechRecognition ?? w.SpeechRecognition;
    if (!SpeechRec) {
      setVoiceError("Voice input is not supported in this browser.");
      return;
    }
    const recognition = new SpeechRec();
    recognition.lang = language === "hi" ? "hi-IN" : "en-IN";
    recognition.interimResults = false;
    recognition.maxAlternatives = 1;
    recognition.onerror = () => setVoiceError("Voice capture failed. Try again or type manually.");
    recognition.onresult = (event) => {
      const said = event.results[0]?.[0]?.transcript ?? "";
      appendPantryTokens(said);
    };
    recognition.start();
  };

  const submitFeedback = (type: "cooked" | "skipped" | "saved") => {
    setFeedback(type);
    trackEvent({
      name: "feedback_submitted",
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

  const updatePreferredCuisines = (value: string) => {
    const preferredCuisines = value
      .split(",")
      .map((item) => item.trim())
      .filter(Boolean);
    setPreferences((prev) => ({ ...prev, preferredCuisines }));
  };

  const updatePantryIngredients = (value: string) => {
    const pantryIngredients = value
      .split(",")
      .map((item) => item.trim())
      .filter(Boolean);
    setPreferences((prev) => ({ ...prev, pantryIngredients }));
  };

  const updateAllergenExclusions = (value: string) => {
    const allergenExclusions = value
      .split(",")
      .map((item) => item.trim())
      .filter(Boolean);
    setPreferences((prev) => ({ ...prev, allergenExclusions }));
  };

  const mergePreset = (partial: Partial<Preferences>) => {
    setPreferences((prev) => ({ ...prev, ...partial }));
  };

  const recipeDetailExtras = useMemo(() => {
    if (!selectedRecipe) return null;
    return {
      breadNotes: getBreadPairingNotes(selectedRecipe, preferences.breadChoice),
      detailedSteps: buildDetailedSteps(selectedRecipe),
      tips: getChefTips(selectedRecipe),
      ingredientPrep: getIngredientPrepNotes(selectedRecipe),
    };
  }, [selectedRecipe, preferences.breadChoice]);

  return (
    <main className="relative overflow-hidden px-4 py-8 sm:px-8">
      <div className="floating-orb floating-orb--one" />
      <div className="floating-orb floating-orb--two" />
      <div className="modern-shell mx-auto max-w-6xl space-y-6">
        <motion.header
          initial={{ opacity: 0, y: -12 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.55, ease: smoothEase }}
          className="glass sticky top-4 z-10 rounded-3xl p-5"
        >
          <div className="flex flex-wrap items-center justify-between gap-4">
            <div className="space-y-1">
              <p className="inline-flex items-center gap-2 text-xs uppercase tracking-[0.25em] text-[var(--muted)]">
                <ChefHat className="h-4 w-4" />
                Meal OS
              </p>
              <h1 className="text-2xl font-semibold">{text.title}</h1>
              <p className="text-sm text-[var(--muted)]">{text.subtitle}</p>
            </div>
            <div className="flex flex-wrap items-center gap-2">
              <Link
                href="/pitch"
                className="chip inline-flex items-center gap-2 no-underline"
              >
                Investor pitch
              </Link>
              <button
                type="button"
                onClick={() => setLanguage((prev) => (prev === "en" ? "hi" : "en"))}
                className="chip inline-flex items-center gap-2"
              >
                <Languages className="h-4 w-4" />
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
            transition={{ duration: 0.5, ease: smoothEase }}
            className="glass relative overflow-hidden rounded-3xl p-6"
          >
            <div className="grid gap-6 lg:grid-cols-[1.2fr_1fr]">
              <div className="space-y-4">
                <p className="chip inline-flex items-center gap-2">
                  <Sparkles className="h-4 w-4" />
                  AI meal recommendation with modern dashboard
                </p>
                <h2 className="text-3xl font-semibold leading-tight">
                  Cook smarter with smooth decisions for every day.
                </h2>
                <p className="max-w-xl text-sm text-[var(--muted)]">
                  Login and get personalized suggestions, weekly planning, and instant cart handoff.
                  The interface now adapts to your theme and feels faster with subtle animations.
                </p>
                <div className="flex items-center gap-3">
                  <div className="flex -space-x-2">
                    {avatarStack.map((avatar) => (
                      <div key={avatar} className="avatar-badge">
                        {avatar}
                      </div>
                    ))}
                  </div>
                  <p className="text-xs text-[var(--muted)]">Trusted by urban families and flatmates</p>
                </div>
                <div className="grid gap-2 sm:grid-cols-3">
                  {quickStats.map((stat) => (
                    <div key={stat.label} className="rounded-2xl border border-[var(--panel-border)] p-3">
                      <stat.icon className="mb-2 h-4 w-4 text-[var(--accent)]" />
                      <p className="text-xs text-[var(--muted)]">{stat.label}</p>
                      <p className="text-base font-semibold capitalize">{stat.value}</p>
                    </div>
                  ))}
                </div>
              </div>

              <div className="rounded-2xl border border-[var(--panel-border)] bg-[color-mix(in_oklab,var(--panel)_90%,transparent)] p-4">
                <p className="mb-4 inline-flex items-center gap-2 text-sm font-medium">
                  <UserRound className="h-4 w-4 text-[var(--accent)]" />
                  Sign in to continue
                </p>
                <div className="space-y-3">
                  <input
                    value={name}
                    onChange={(event) => setName(event.target.value)}
                    placeholder="Name"
                    className="input-modern"
                  />
                  <input
                    value={email}
                    onChange={(event) => setEmail(event.target.value)}
                    placeholder="Email"
                    className="input-modern"
                  />
                  <input
                    type="password"
                    value={password}
                    onChange={(event) => setPassword(event.target.value)}
                    placeholder="Password"
                    className="input-modern"
                  />
                  <motion.button
                    whileHover={{ scale: 1.02, y: -2 }}
                    whileTap={{ scale: 0.97 }}
                    type="button"
                    onClick={handleLogin}
                    className="button-accent inline-flex w-full items-center justify-center gap-2 rounded-2xl px-4 py-3 font-semibold"
                  >
                    <LogIn className="h-4 w-4" />
                    {text.login}
                  </motion.button>
                </div>
              </div>
            </div>
          </motion.section>
        ) : null}

        {loggedIn ? (
          <>
            <section className="grid gap-3 sm:grid-cols-4">
              {quickStats.map((stat) => (
                <motion.article
                  key={stat.label}
                  whileHover={{ y: -2 }}
                  className="glass rounded-2xl p-4"
                  transition={{ duration: 0.2 }}
                >
                  <div className="flex items-center justify-between">
                    <p className="text-xs uppercase tracking-wider text-[var(--muted)]">{stat.label}</p>
                    <stat.icon className="h-4 w-4 text-[var(--accent)]" />
                  </div>
                  <p className="mt-3 text-2xl font-semibold capitalize">{stat.value}</p>
                </motion.article>
              ))}
            </section>

            <motion.section whileHover={{ y: -2 }} className="glass rounded-3xl p-5">
              <p className="mb-3 inline-flex items-center gap-2 text-sm font-medium text-[var(--muted)]">
                <Zap className="h-4 w-4 text-[var(--accent)]" />
                Bold moves — tonight & presets
              </p>
              <p className="mb-4 text-xs text-[var(--muted)]">
                Tonight locks suggestions to about 15 minutes total time. Presets tune budget and diet for PGs,
                vegetarian families, or frugal weeks.
              </p>
              <div className="flex flex-wrap items-center gap-2">
                <motion.button
                  type="button"
                  whileTap={{ scale: 0.97 }}
                  onClick={() =>
                    setPreferences((prev) => ({ ...prev, tonightMode: !prev.tonightMode }))
                  }
                  className={
                    preferences.tonightMode
                      ? "button-accent rounded-full px-4 py-2 text-sm font-semibold"
                      : "chip rounded-full px-4 py-2 text-sm font-medium"
                  }
                >
                  <Zap className="mr-1 inline h-4 w-4 align-text-bottom" />
                  {preferences.tonightMode ? "Tonight on (≤15 min)" : "Tonight mode — quick cook"}
                </motion.button>
                <button
                  type="button"
                  className="chip text-sm"
                  onClick={() => mergePreset(applyPgHostelPack())}
                >
                  PG / hostel pack
                </button>
                <button
                  type="button"
                  className="chip text-sm"
                  onClick={() => mergePreset(applyFamilyVegWeek())}
                >
                  Family veg week
                </button>
                <button
                  type="button"
                  className="chip text-sm"
                  onClick={() => mergePreset(applyBudgetWeek())}
                >
                  Budget week
                </button>
              </div>
            </motion.section>

            <section className="grid gap-4 lg:grid-cols-3">
              <motion.div whileHover={{ y: -3 }} className="glass rounded-3xl p-5">
                <p className="mb-3 inline-flex items-center gap-2 text-sm font-medium text-[var(--muted)]">
                  <Package className="h-4 w-4" />
                  Pantry mode
                </p>
                <p className="mb-3 text-xs text-[var(--muted)]">
                  Only suggests dishes where every ingredient matches something in your pantry (comma-separated).
                </p>
                <label className="mb-3 flex cursor-pointer items-center gap-2 text-sm">
                  <input
                    type="checkbox"
                    className="rounded border-[var(--panel-border)]"
                    checked={preferences.pantryMode}
                    onChange={(event) =>
                      setPreferences((prev) => ({ ...prev, pantryMode: event.target.checked }))
                    }
                  />
                  Use pantry-only matching
                </label>
                <div className="flex gap-2">
                  <input
                    type="text"
                    className="input-modern min-w-0 flex-1"
                    placeholder="e.g. onion, tomato, rice, paneer, eggs"
                    value={preferences.pantryIngredients.join(", ")}
                    onChange={(event) => updatePantryIngredients(event.target.value)}
                  />
                  <motion.button
                    type="button"
                    whileTap={{ scale: 0.96 }}
                    title="Add by voice (EN/HI)"
                    onClick={startPantryVoice}
                    className="chip shrink-0 inline-flex items-center justify-center px-3"
                  >
                    <Mic className="h-4 w-4" />
                  </motion.button>
                </div>
                {voiceError ? <p className="text-xs text-amber-600 dark:text-amber-400">{voiceError}</p> : null}
              </motion.div>

              <motion.div whileHover={{ y: -3 }} className="glass rounded-3xl p-5">
                <p className="mb-3 inline-flex items-center gap-2 text-sm font-medium text-[var(--muted)]">
                  <PartyPopper className="h-4 w-4" />
                  Festival / occasion
                </p>
                <p className="mb-3 text-xs text-[var(--muted)]">
                  Picks dishes tagged for the occasion (everyday shows all tagged recipes).
                </p>
                <select
                  className="input-modern"
                  value={preferences.occasion}
                  onChange={(event) =>
                    setPreferences((prev) => ({
                      ...prev,
                      occasion: event.target.value as OccasionKey,
                    }))
                  }
                >
                  {occasionOptions.map((opt) => (
                    <option key={opt.value} value={opt.value}>
                      {opt.label}
                    </option>
                  ))}
                </select>
              </motion.div>

              <motion.div whileHover={{ y: -3 }} className="glass rounded-3xl p-5">
                <p className="mb-3 inline-flex items-center gap-2 text-sm font-medium text-[var(--muted)]">
                  <Banknote className="h-4 w-4" />
                  Budget mode
                </p>
                <p className="mb-3 text-xs text-[var(--muted)]">
                  Cost-aware suggestions using estimated ₹ per meal. Budget ≤₹220, Moderate ≤₹450, Premium open.
                </p>
                <label className="mb-3 flex cursor-pointer items-center gap-2 text-sm">
                  <input
                    type="checkbox"
                    className="rounded border-[var(--panel-border)]"
                    checked={preferences.budgetMode}
                    onChange={(event) =>
                      setPreferences((prev) => ({ ...prev, budgetMode: event.target.checked }))
                    }
                  />
                  Limit by budget band
                </label>
                <select
                  className="input-modern"
                  disabled={!preferences.budgetMode}
                  value={preferences.budgetTier}
                  onChange={(event) =>
                    setPreferences((prev) => ({
                      ...prev,
                      budgetTier: event.target.value as BudgetTier,
                    }))
                  }
                >
                  <option value="budget">Budget (≤ ₹220)</option>
                  <option value="moderate">Moderate (≤ ₹450)</option>
                  <option value="premium">Premium (any)</option>
                </select>
              </motion.div>
            </section>

            <motion.section
              whileHover={{ y: -2 }}
              className="glass rounded-3xl p-5"
            >
              <p className="mb-3 inline-flex items-center gap-2 text-sm font-medium text-[var(--muted)]">
                <BookOpen className="h-4 w-4" />
                Recipe display & bread
              </p>
              <p className="mb-4 text-xs text-[var(--muted)]">
                Turn on detailed mode for expanded steps, prep notes, and chef tips. Pick your bread or carb for
                pairing guidance with every recipe.
              </p>
              <div className="grid gap-4 sm:grid-cols-2">
                <label className="flex cursor-pointer items-start gap-2 text-sm">
                  <input
                    type="checkbox"
                    className="mt-0.5 rounded border-[var(--panel-border)]"
                    checked={preferences.detailedRecipeMode}
                    onChange={(event) =>
                      setPreferences((prev) => ({ ...prev, detailedRecipeMode: event.target.checked }))
                    }
                  />
                  <span>
                    <span className="font-medium text-[var(--foreground)]">Detailed recipe mode</span>
                    <span className="mt-1 block text-xs text-[var(--muted)]">
                      Shows timing breakdown, ingredient prep notes, expanded steps, and full bread pairing text.
                    </span>
                  </span>
                </label>
                <div className="space-y-2">
                  <p className="text-xs font-medium uppercase tracking-wide text-[var(--muted)]">
                    Bread / carb of choice
                  </p>
                  <select
                    className="input-modern"
                    value={preferences.breadChoice}
                    onChange={(event) =>
                      setPreferences((prev) => ({
                        ...prev,
                        breadChoice: event.target.value as BreadChoice,
                      }))
                    }
                  >
                    {breadChoiceOptions.map((opt) => (
                      <option key={opt.value} value={opt.value}>
                        {opt.label}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
            </motion.section>

            <section className="grid gap-6 lg:grid-cols-[1fr_1fr]">
              <motion.div whileHover={{ y: -4 }} className="glass rounded-3xl p-5">
                <p className="mb-3 inline-flex items-center gap-2 text-sm font-medium text-[var(--muted)]">
                  <Leaf className="h-4 w-4" />
                  Preferences
                </p>
                <div className="space-y-3">
                  <select
                    className="input-modern"
                    value={preferences.dietMode}
                    onChange={(event) =>
                      setPreferences((prev) => ({ ...prev, dietMode: event.target.value as DietMode }))
                    }
                  >
                    <option value="veg">Veg</option>
                    <option value="non_veg">Non-Veg</option>
                    <option value="mix">Mix</option>
                  </select>
                  <select
                    className="input-modern"
                    value={preferences.spiceLevel}
                    onChange={(event) =>
                      setPreferences((prev) => ({
                        ...prev,
                        spiceLevel: event.target.value as Preferences["spiceLevel"],
                      }))
                    }
                  >
                    <option value="low">Low spice</option>
                    <option value="medium">Medium spice</option>
                    <option value="high">High spice</option>
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
                    className="input-modern"
                    placeholder="Max cooking time"
                  />
                  <select
                    className="input-modern"
                    value={preferences.noveltyLevel}
                    onChange={(event) =>
                      setPreferences((prev) => ({
                        ...prev,
                        noveltyLevel: event.target.value as NoveltyLevel,
                      }))
                    }
                  >
                    <option value="classic">Classic dishes</option>
                    <option value="balanced">Balanced variety</option>
                    <option value="experimental">Experimental dishes</option>
                  </select>
                  <input
                    type="text"
                    placeholder="Preferred cuisines (comma separated)"
                    onChange={(event) => updatePreferredCuisines(event.target.value)}
                    className="input-modern"
                  />
                  <input
                    type="text"
                    placeholder="Excluded ingredients (comma separated)"
                    onChange={(event) => updateExcluded(event.target.value)}
                    className="input-modern"
                  />
                  <input
                    type="text"
                    placeholder="Household allergens (comma separated, e.g. peanut, shellfish)"
                    value={preferences.allergenExclusions.join(", ")}
                    onChange={(event) => updateAllergenExclusions(event.target.value)}
                    className="input-modern"
                  />
                  <label className="flex cursor-pointer items-center gap-2 text-sm">
                    <input
                      type="checkbox"
                      className="rounded border-[var(--panel-border)]"
                      checked={preferences.kidsFriendly}
                      onChange={(event) =>
                        setPreferences((prev) => ({ ...prev, kidsFriendly: event.target.checked }))
                      }
                    />
                    Kids-friendly (prefer milder spice in rankings)
                  </label>
                  <p className="text-xs text-[var(--muted)]">
                    Diet is strict: selecting veg gives only veg, selecting non-veg gives only non-veg.
                  </p>
                </div>
              </motion.div>

              <motion.div whileHover={{ y: -4 }} className="glass rounded-3xl p-5">
                <p className="mb-3 inline-flex items-center gap-2 text-sm font-medium text-[var(--muted)]">
                  <Wand2 className="h-4 w-4" />
                  Suggestion Controls
                </p>
                <div className="grid gap-3 sm:grid-cols-2">
                  <select
                    value={mealSlot}
                    onChange={(event) => setMealSlot(event.target.value as MealSlot)}
                    className="input-modern"
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
                    className="input-modern"
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
                    className="chip inline-flex items-center gap-2"
                    type="button"
                  >
                    <CalendarDays className="h-4 w-4" />
                    {text.weekly}
                  </motion.button>
                </div>
              </motion.div>
            </section>

            <section className="grid gap-6 lg:grid-cols-[1fr_1fr]">
              <motion.article whileHover={{ y: -4 }} className="glass rounded-3xl p-5">
                <p className="inline-flex items-center gap-2 text-sm font-medium text-[var(--muted)]">
                  <Sparkles className="h-4 w-4" />
                  Primary suggestion
                </p>
                <AnimatePresence mode="wait">
                  {suggestion ? (
                    <motion.div
                      key={suggestion.primarySuggestion.id}
                      initial={{ opacity: 0, y: 8 }}
                      animate={{ opacity: 1, y: 0 }}
                      exit={{ opacity: 0, y: -6 }}
                      transition={{ duration: 0.32, ease: smoothEase }}
                      className="mt-3 space-y-2"
                    >
                      <h3 className="text-2xl font-semibold">{suggestion.primarySuggestion.name}</h3>
                      <p className="text-sm text-[var(--muted)]">{suggestion.explanation}</p>
                      <p className="rounded-xl border border-[var(--panel-border)] px-3 py-2 text-xs text-[var(--muted)]">
                        {suggestion.uniqueCookIdea}
                      </p>
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
                    </motion.div>
                  ) : (
                    <motion.p
                      key="empty-suggestion"
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      exit={{ opacity: 0 }}
                      className="mt-3 text-sm text-[var(--muted)]"
                    >
                      Generate a suggestion to see meal recommendations.
                    </motion.p>
                  )}
                </AnimatePresence>
              </motion.article>

              <motion.article whileHover={{ y: -4 }} className="glass rounded-3xl p-5">
                <p className="inline-flex items-center gap-2 text-sm font-medium text-[var(--muted)]">
                  <ChefHat className="h-4 w-4" />
                  {text.recipe}
                </p>
                <AnimatePresence mode="wait">
                  {selectedRecipe ? (
                    <motion.div
                      key={selectedRecipe.id}
                      initial={{ opacity: 0, y: 8 }}
                      animate={{ opacity: 1, y: 0 }}
                      exit={{ opacity: 0, y: -6 }}
                      transition={{ duration: 0.32, ease: smoothEase }}
                      className="mt-3 space-y-3"
                    >
                      <h3 className="text-xl font-semibold">{selectedRecipe.name}</h3>
                      <p className="flex flex-wrap items-center gap-x-2 gap-y-1 text-sm text-[var(--muted)]">
                        <span className="inline-flex items-center gap-2">
                          <Clock3 className="h-4 w-4" />
                          {selectedRecipe.prepTimeMins + selectedRecipe.cookTimeMins} mins total
                        </span>
                        <span className="text-[var(--panel-border)]">|</span>
                        <span>{selectedRecipe.cuisine}</span>
                        <span className="text-[var(--panel-border)]">|</span>
                        <span className="inline-flex items-center gap-1">
                          <Banknote className="h-4 w-4" />~₹{selectedRecipe.estimatedCostINR} est.
                        </span>
                        <span className="text-[var(--panel-border)]">|</span>
                        <span className="chip py-1 text-[0.7rem]">
                          Bread: {getBreadChoiceLabel(preferences.breadChoice)}
                        </span>
                      </p>

                      {preferences.detailedRecipeMode && recipeDetailExtras ? (
                        <div className="space-y-4 pt-1">
                          <div className="grid gap-2 sm:grid-cols-3">
                            <div className="rounded-xl border border-[var(--panel-border)] bg-[color-mix(in_oklab,var(--panel)_85%,transparent)] px-3 py-2 text-center">
                              <p className="text-[0.65rem] uppercase tracking-wide text-[var(--muted)]">Prep</p>
                              <p className="text-lg font-semibold">{selectedRecipe.prepTimeMins} min</p>
                            </div>
                            <div className="rounded-xl border border-[var(--panel-border)] bg-[color-mix(in_oklab,var(--panel)_85%,transparent)] px-3 py-2 text-center">
                              <p className="text-[0.65rem] uppercase tracking-wide text-[var(--muted)]">Cook</p>
                              <p className="text-lg font-semibold">{selectedRecipe.cookTimeMins} min</p>
                            </div>
                            <div className="rounded-xl border border-[var(--panel-border)] bg-[color-mix(in_oklab,var(--accent-soft)_40%,transparent)] px-3 py-2 text-center">
                              <p className="text-[0.65rem] uppercase tracking-wide text-[var(--muted)]">Active total</p>
                              <p className="text-lg font-semibold">
                                {selectedRecipe.prepTimeMins + selectedRecipe.cookTimeMins} min
                              </p>
                            </div>
                          </div>

                          <div>
                            <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-[var(--muted)]">
                              Ingredients & prep notes
                            </p>
                            <ul className="space-y-2 text-sm">
                              {recipeDetailExtras.ingredientPrep.map((row) => (
                                <li
                                  key={row.name}
                                  className="rounded-lg border border-[var(--panel-border)] px-3 py-2"
                                >
                                  <span className="font-medium text-[var(--foreground)]">
                                    {row.name}
                                  </span>
                                  <span className="text-[var(--muted)]"> — {row.quantity}</span>
                                  <p className="mt-1 text-xs text-[var(--muted)]">{row.note}</p>
                                </li>
                              ))}
                            </ul>
                          </div>

                          <div>
                            <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-[var(--muted)]">
                              Detailed steps
                            </p>
                            <ol className="list-none space-y-4">
                              {recipeDetailExtras.detailedSteps.map((block) => (
                                <li
                                  key={block.stepNumber}
                                  className="rounded-xl border border-[var(--panel-border)] p-3"
                                >
                                  <p className="font-medium text-[var(--foreground)]">
                                    Step {block.stepNumber}: {block.summary}
                                  </p>
                                  <ul className="mt-2 list-disc space-y-1 pl-5 text-xs text-[var(--muted)]">
                                    {block.microSteps.map((micro) => (
                                      <li key={micro}>{micro}</li>
                                    ))}
                                  </ul>
                                </li>
                              ))}
                            </ol>
                          </div>

                          <div className="rounded-xl border border-[var(--panel-border)] bg-[color-mix(in_oklab,var(--accent-soft)_25%,transparent)] px-3 py-3">
                            <p className="mb-1 text-xs font-semibold uppercase tracking-wide text-[var(--muted)]">
                              Bread / carb pairing — {getBreadChoiceLabel(preferences.breadChoice)}
                            </p>
                            <p className="text-sm leading-relaxed text-[var(--foreground)]">
                              {recipeDetailExtras.breadNotes}
                            </p>
                          </div>

                          <div>
                            <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-[var(--muted)]">
                              Chef tips
                            </p>
                            <ul className="list-disc space-y-1 pl-5 text-sm text-[var(--muted)]">
                              {recipeDetailExtras.tips.map((tip) => (
                                <li key={tip}>{tip}</li>
                              ))}
                            </ul>
                          </div>
                        </div>
                      ) : (
                        <>
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
                          {preferences.breadChoice !== "none" && recipeDetailExtras ? (
                            <p className="rounded-xl border border-[var(--panel-border)] px-3 py-2 text-xs leading-relaxed text-[var(--muted)]">
                              <span className="font-medium text-[var(--foreground)]">Pairing: </span>
                              {recipeDetailExtras.breadNotes}
                            </p>
                          ) : null}
                        </>
                      )}

                      {selectedRecipe ? (
                        <motion.button
                          type="button"
                          whileHover={{ scale: 1.02 }}
                          whileTap={{ scale: 0.98 }}
                          onClick={submitProofCooking}
                          className="button-accent w-full rounded-2xl px-4 py-3 text-sm font-semibold"
                        >
                          I made this — confirm proof of cooking
                        </motion.button>
                      ) : null}

                      <div className="flex flex-wrap items-center gap-2">
                        <select
                          value={partner}
                          onChange={(event) =>
                            setPartner(event.target.value as "zepto" | "blinkit" | "instamart" | "bigbasket")
                          }
                          className="input-modern max-w-[170px] text-sm"
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
                      {smartCartHints && smartCartHints.length > 0 ? (
                        <div className="rounded-xl border border-[var(--panel-border)] p-3">
                          <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-[var(--muted)]">
                            Smart cart search hints
                          </p>
                          <p className="mb-2 text-[0.7rem] text-[var(--muted)]">
                            Optimised queries for quick-commerce search (MVP mapping — expand to SKUs with
                            partners).
                          </p>
                          <ul className="max-h-36 space-y-1 overflow-y-auto text-xs">
                            {smartCartHints.map((row) => (
                              <li key={`${row.name}-${row.searchQuery}`}>
                                <span className="font-medium text-[var(--foreground)]">{row.name}</span>
                                <span className="text-[var(--muted)]"> → </span>
                                <span>{row.searchQuery}</span>
                              </li>
                            ))}
                          </ul>
                        </div>
                      ) : null}
                    </motion.div>
                  ) : (
                    <motion.p
                      key="empty-recipe"
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      exit={{ opacity: 0 }}
                      className="mt-3 text-sm text-[var(--muted)]"
                    >
                      Choose a recipe from suggestion to view full details.
                    </motion.p>
                  )}
                </AnimatePresence>
              </motion.article>
            </section>

            <motion.section whileHover={{ y: -2 }} className="glass rounded-3xl p-5">
              <div className="mb-3 flex flex-wrap items-center justify-between gap-3">
                <p className="inline-flex items-center gap-2 text-sm font-medium text-[var(--muted)]">
                  <CalendarDays className="h-4 w-4" />
                  Weekly routine
                </p>
                {weeklyPlan.length > 0 ? (
                  <motion.button
                    type="button"
                    whileTap={{ scale: 0.97 }}
                    onClick={() => void shareWeeklyPlan()}
                    className="chip inline-flex items-center gap-2 text-sm"
                  >
                    <Share2 className="h-4 w-4" />
                    Copy shareable plan
                  </motion.button>
                ) : null}
              </div>
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
                <p className="inline-flex items-center gap-2 text-sm font-medium text-[var(--muted)]">
                  <Sparkles className="h-4 w-4" />
                  Premium plans
                </p>
                <div className="mt-4 grid gap-3 sm:grid-cols-3">
                  {[
                    { name: "Free", price: "INR 0", perks: "Daily suggestions + recipes" },
                    { name: "Plus", price: "INR 149/mo", perks: "Unlimited swaps + pantry mode" },
                    { name: "Pro Family", price: "INR 299/mo", perks: "Family voting + budget planner" },
                  ].map((plan) => (
                    <div
                      key={plan.name}
                      className="rounded-2xl border border-[var(--panel-border)] bg-[color-mix(in_oklab,var(--panel)_88%,transparent)] p-3"
                    >
                      <p className="font-semibold">{plan.name}</p>
                      <p className="mt-1 text-sm">{plan.price}</p>
                      <p className="mt-2 text-xs text-[var(--muted)]">{plan.perks}</p>
                    </div>
                  ))}
                </div>
              </motion.article>
              <motion.article whileHover={{ y: -3 }} className="glass rounded-3xl p-5">
                <p className="inline-flex items-center gap-2 text-sm font-medium text-[var(--muted)]">
                  <ChefHat className="h-4 w-4" />
                  Beta feedback loop
                </p>
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
                <AnimatePresence>
                  {feedback ? (
                    <motion.p
                      initial={{ opacity: 0, y: 6 }}
                      animate={{ opacity: 1, y: 0 }}
                      exit={{ opacity: 0 }}
                      className="mt-3 text-xs text-[var(--muted)]"
                    >
                      Recorded: {feedback}
                    </motion.p>
                  ) : null}
                </AnimatePresence>
              </motion.article>
            </section>
          </>
        ) : null}

        <footer className="glass rounded-3xl p-4 text-sm text-[var(--muted)]">
          <div className="flex flex-wrap items-center justify-between gap-3">
            <p className="inline-flex items-center gap-2">
              <Sparkles className="h-4 w-4" />
              Events tracked in MVP:{" "}
              <span className="font-semibold text-[var(--foreground)]">{eventCount}</span>
            </p>
            <Link href="/pitch" className="text-[var(--accent)] underline-offset-4 hover:underline">
              Read investor pitch →
            </Link>
          </div>
        </footer>
      </div>
    </main>
  );
}
