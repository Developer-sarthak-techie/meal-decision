# MealPannerModifiedApp — Product roadmap, development guide & tester handoff

This document is for **you** (building the app) and for **testers** (installing builds). It targets **`MealPannerModifiedApp/`** (`.NET MAUI`: Android, iOS, Mac Catalyst).

---

## 1. Feature inventory (what “all features” means today)

Use this as a **checklist** so UI/UX work does not skip a surface.

| Area | Routes / entry | What users get |
|------|----------------|----------------|
| **Home** | `home` → `LandingPage` | Themes, text scaling, quick nav to Recipes/Suggest, chef card; flyout hint |
| **Recipes** | `recipes` → `RecipesHomePage` → `RecipeListPage` → `RecipeDetailPage` | Large recipe catalog (collections by meal type), detail views |
| **Pantry mode** | `pantry` → `PantryPage` → `RecipeDetailPage` | Meal type + pantry list → ranked recipe matches from catalog |
| **Summer shakes** | `shakes` → `ShakesPage` → `ShakeDetailPage` | Curated cold drinks/smoothies with full step-by-step instructions |
| **Salads** | `salads` → `SaladsPage` → `SaladDetailPage` | Fresh salads (global + Indian-inspired) with ingredients + numbered steps |
| **Home remedies** | `remedies` → list + detail | Browse/search remedies |
| **Spices & herbs** | `spices` → `SpicesPage` → `SpiceDetailPage` | 1000+ spice/herb entries (variants), benefits & kitchen uses (educational) |
| **Suggest meal** | `suggest` → `SuggestMealPage` | Meal suggestion flow |
| **Fitness goal** | `fitnessgoal` → `FitnessGoalPage` | Interactive fitness UI; PDF export where implemented |
| **Subscription** | `subscription` | Subscription / monetisation UI |
| **Dashboard** | `dashboard` | Overview (placeholder until wired to real data) |
| **Settings** | `settings` | Text size, theme copy, preferences |
| **Global** | `AppShell` flyout | Navigation, themed flyout header/background |

**Cross-cutting:** `ThemeManager` (palettes + `UserAppTheme`), `FontScaleManager` (dynamic font resource keys), `CommunityToolkit.Maui`, semantic colors in **`Resources/Themes/YellowTheme.xaml`**, premium patterns in **`Resources/Styles/PremiumHome.xaml`** (extend app-wide — see §4).

---

## 2. Roadmap (phased)

Work in **vertical slices** so each phase delivers something testable.

### Phase A — Stability & parity (1–2 weeks)

- [ ] Confirm every flyout item opens without errors on **Android** and **iOS** (and Mac Catalyst if you ship it).
- [ ] Verify **Shell** routes (`//recipes`, `//suggest`, `//remedies`, …) match `Route=` in `AppShell.xaml`.
- [ ] Smoke-test **deep navigation**: Recipes → list → detail → back.
- [ ] Note **minimum OS versions** from `MealPannerModifiedApp.csproj` (`SupportedOSPlatformVersion`).

### Phase B — UI/UX consistency (2–4 weeks)

Goal: one visual system, not mixed templates.

- [ ] Add **`Resources/Styles/AppChrome.xaml`** (or generalise `PremiumHome.xaml`) with:
  - Page header pattern (eyebrow + title + subtitle)
  - `StandardCard`, `PrimaryCta`, `SecondaryCta`
  - Shared scroll padding (`24` horizontal, `16–32` vertical) on main pages
- [ ] Pick **one interaction pattern** for tappables: `Border` + `TouchBehavior` + `TapGestureRecognizer` *or* `Button` + `VisualStateManager` — use consistently by role.
- [ ] Align **Recipes**, **Settings**, **Suggest** first (high traffic), then Remedies, Fitness, Subscription, Dashboard.
- [ ] Resolve **duplicate controls** (theme/font on Home vs Settings) with clear copy (“Quick” vs “All preferences”).

### Phase C — Feature depth (ongoing)

- [ ] **Dashboard:** real summaries (last suggestions, saves, streaks) backed by local storage or API.
- [ ] **Subscription:** StoreKit / Play Billing when ready; label betas **“Purchases disabled”** until live.
- [ ] **Accessibility:** `SemanticProperties`, ≥44 pt tap targets, contrast on every theme.

### Phase D — Release quality (parallel)

- [ ] CI: `dotnet build` for `net9.0-android` + `net9.0-ios` on each PR.
- [ ] Optional **crash analytics** before wide external test.
- [ ] Bump **`ApplicationDisplayVersion` / `ApplicationVersion`** per tester build.

---

## 3. How to develop (daily workflow)

### 3.1 Prerequisites

- `.NET SDK 9` + MAUI workload: `dotnet workload install maui`
- **Android:** SDK + emulator or device
- **iOS / Catalyst:** Xcode on Mac; Apple Developer account for device/TestFlight

### 3.2 Restore and run

```bash
cd MealPannerModifiedApp
dotnet restore
```

**Mac Catalyst (fast iteration):**

```bash
dotnet build -t:Run -f net9.0-maccatalyst -c Debug
```

**Android:**

```bash
dotnet build -t:Run -f net9.0-android -c Debug
```

**iOS Simulator:**

```bash
dotnet build -t:Run -f net9.0-ios -c Debug
```

### 3.3 Where things live

| Location | Responsibility |
|----------|----------------|
| `Views/` | XAML pages; keep code-behind thin as you adopt MVVM |
| `Models/`, `Data/` | Domain + catalogs |
| `Resources/Styles/`, `Resources/Themes/` | Typography, colors, cards — **single source of truth** |
| `ThemeManager.cs` | Runtime palette updates |

### 3.4 MVVM (recommended as complexity grows)

- One ViewModel per heavy screen; `BindingContext` from constructor or DI in `MauiProgram`.
- Abstract `Shell.Current.GoToAsync` behind `INavigationService` for testability.

---

## 4. UI/UX consistency playbook

### Rules

1. **Colors:** use `{DynamicResource …}` (`PageBackground`, `TextPrimary`, `AccentBright`, `SurfaceCard`, `PanelBorder`) — avoid page-level hex except rare illustration assets.
2. **Type:** always `{DynamicResource FontSize*}` keys from `FontSizes.xaml` so **font scale** applies everywhere.
3. **Spacing:** **8 pt grid** (8, 16, 24, 32).
4. **Cards:** `Border` + `RoundRectangle 16–24` + themed stroke + soft `Shadow` (match `PremiumElevatedCard` / `PremiumGlassPanel`).
5. **Motion:** 120–480 ms, `CubicOut` / `SinInOut`; don’t animate every list cell at once on scroll.

### Consistency pass order

1. Lock **3 label roles** + **2 CTA roles** in shared styles.
2. Migrate **Recipes** + **Settings**.
3. Remaining pages + **Shell** labels (“FitnessGoal” → “Fitness” if you want production tone).

---

## 5. Giving the app to testers

Match the channel to the tester’s device.

### 5.1 Android — quick debug APK (trusted testers)

```bash
cd MealPannerModifiedApp
dotnet publish -f net9.0-android -c Debug -p:AndroidPackageFormat=apk
```

Output is typically under `bin/Debug/net9.0-android/publish/`. Testers may need to allow install from your file app. **Debug builds are not for Play Store.**

### 5.2 Android — Google Play internal testing (best for real users)

1. Configure **release signing** (keystore) per [MAUI Android publish](https://learn.microsoft.com/dotnet/maui/android/deployment/overview).
2. `dotnet publish -f net9.0-android -c Release`
3. Upload **AAB** to Play Console → **Internal testing** → invite by email → share opt-in link.

### 5.3 iOS — TestFlight

1. Apple Developer Program + correct bundle ID / signing.
2. `dotnet publish` / Xcode archive for **Release**.
3. Upload to App Store Connect → **TestFlight** → invite testers.

External betas may need a short **Beta App Review**.

### 5.4 Mac Catalyst

Sharing a local `.app` is possible for Mac-only testers; notarisation matters for wide distribution. For product validation, prioritise **phone** builds.

### 5.5 Email template for testers

```
Subject: Meal Planner beta — v1.0.x (Android / iOS)

Install: [Play internal link OR TestFlight link]

Please try:
- Home: change theme + text size, use Recipes and Suggest shortcuts
- Recipes: open a category, open a recipe, go back
- Suggest meal, Home remedies, Fitness: complete one path each
- Kill and relaunch — confirm Settings / font choice sticks

Report: screen name, steps, expected vs actual, screenshot if UI.

Build: [version + commit hash]
```

---

## 6. Definition of done (consistent + feature-complete MVP)

- [ ] All **§1** destinations reachable without crash on your target stores’ minimum OS.
- [ ] Theme + font scale verified on **≥2 themes** and **≥2 scale steps** across **≥3 pages**.
- [ ] Shared chrome styles applied across most `Views/*.xaml`.
- [ ] At least one **external** test round via **Play internal** or **TestFlight**.

---

## 7. Key files

| File | Role |
|------|------|
| `AppShell.xaml` | Flyout + routes |
| `Resources/Themes/YellowTheme.xaml` | Default semantic colors |
| `Resources/Styles/PremiumHome.xaml` | Reference premium styles (generalise next) |
| `ThemeManager.cs` | Palettes |
| `Views/LandingPage.xaml` | Reference layout density + motion |

---

*Update this doc when routes, features, or distribution method change.*
