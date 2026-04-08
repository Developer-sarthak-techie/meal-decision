# Aaj Kya Banaye - MAUI MVP

Production-ready `.NET MAUI` mobile MVP that combines meal decisioning with fitness intelligence and a structured 21-day transformation journey.

## 1) How To Run The App

1. Install prerequisites:
   - `.NET SDK 9+`
   - `.NET MAUI workload`
   - Xcode (for iOS/Mac), Android SDK + emulator, or Windows SDK as needed.
2. From terminal:
   - `cd MealPlannerApp`
   - `dotnet restore`
   - `dotnet build`
3. Run by target:
   - Android: `dotnet build -t:Run -f net9.0-android`
   - iOS simulator (Mac): `dotnet build -t:Run -f net9.0-ios`
   - Mac Catalyst: `dotnet build -t:Run -f net9.0-maccatalyst`
   - Windows (on Windows): `dotnet build -t:Run -f net9.0-windows10.0.19041.0`

### Mac-only provision (temporary)

Use this to run on Mac while skipping Android/iOS framework validation:

- Build: `dotnet build -f net9.0-maccatalyst -p:EnableMacOnlyRun=true`
- Run: `dotnet build -t:Run -f net9.0-maccatalyst -p:EnableMacOnlyRun=true`

## 2) Folder Structure

- `Views/`: XAML pages for onboarding, dashboard, meal, diet, exercises, food library, journey, settings.
- `ViewModels/`: MVVM presentation logic with `INotifyPropertyChanged`.
- `Models/`: Domain entities like `Recipe`, `UserProfile`, `JourneyTask`.
- `Services/`: Business logic and local persistence interfaces + implementations.
- `Helpers/`: Shared utility (`ServiceLocator`) for resolving DI in XAML pages.
- `Themes/`: Premium dark theme resources and reusable styles.
- `Resources/`: MAUI app assets (fonts, icons, splash, images).
- `Data/`: Seed data for recipes, exercises, foods, and 21-day tasks.

## 3) Meal Engine Logic

`MealEngineService` powers both "Aaj Kya Banaye?" and full day diet plans.

Decision factors:
- Goal (Lose Weight, Gain Weight, Build Muscle, Stay Fit, Athletic Body)
- Diet preference (Veg/Non-veg)
- Time (Breakfast/Lunch/Dinner)
- Mood (Light/Heavy)

Flow:
1. Filter recipes by meal time + mood + diet preference.
2. Rank candidates with goal-specific nutrition scoring.
3. Shuffle ties for variety and return top suggestions.
4. If no matches, return safe fallback list by preference.

## 4) How To Modify

### Add New Recipes

1. Open `Data/AppSeedData.cs`.
2. Add a new `Recipe` object in `Recipes` list:
   - Set `DishName`, `Ingredients`, `QuickRecipe`
   - Include `Calories`, `ProteinGrams`, `HealthScore`
   - Set `MealTime`, `Mood`, `IsVegetarian`

### Change Recommendation Logic

1. Open `Services/MealEngineService.cs`.
2. Update:
   - `CalculateGoalAlignmentScore` for scoring strategy
   - filtering blocks in `GetMealSuggestionsAsync`

## 5) How To Extend (AI + Backend)

### Add AI Later

- Introduce `IAiRecommendationService`.
- In `MealEngineService`, call AI to:
  - Generate dynamic recipes from pantry ingredients
  - Personalize suggestions using historical user behavior
- Keep static `AppSeedData` as fallback for offline mode.

### Add Backend

- Add API client service layer (`HttpClient`) under `Services/`.
- Replace/augment `PreferencesStorageService` with:
  - authenticated profile sync
  - cloud journey persistence
  - analytics + progress insights
- Keep interfaces (`IUserProfileService`, `IJourneyService`) unchanged to minimize refactor.

## Architecture Notes

- Pattern: Clean MVVM + Dependency Injection.
- Local storage: `Preferences` with JSON serialization.
- Core UI controls used:
  - `CollectionView`
  - `Grid`
  - `FlexLayout`
- UX:
  - Dark theme default
  - Rounded premium cards
  - Loading states and smooth page animations
  - Bottom tab navigation via `Shell`.
