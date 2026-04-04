# Product PRD - Meal Decision

## Objective
Help Indian urban households decide "what to cook today" in under 90 seconds with personalized suggestions, clear recipes, and weekly planning.

## Primary Personas
- Working couple (age 25-40): low time, high decision fatigue after office.
- Homemaker (age 28-50): wants variety and weekly structure.
- Cook/Maid assistant user: needs clear daily instruction with minimal back-and-forth.

## User Stories
- As a user, I can sign in and save my food preferences.
- As a user, I can select veg/non-veg/mix and day-wise restrictions.
- As a user, I can request one-click suggestions for breakfast/lunch/dinner/snacks.
- As a user, I can view complete recipe steps and ingredient quantities.
- As a user, I can generate and edit a weekly cooking routine.
- As a user, I can switch light/dark theme and language (English/Hindi labels).
- As a user, I can replace a suggested dish with another suitable option.

## Functional Requirements (MVP)
1. Authentication flow (email + password for MVP).
2. Profile and preference onboarding.
3. Suggestion engine with anti-repeat behavior.
4. Recipe detail page/section with steps and ingredients.
5. Weekly planner (7 days x 4 meal slots).
6. Edit/replace dish per slot.
7. Basic analytics events.
8. Responsive UI for mobile + desktop.

## Non-functional Requirements
- Suggestion response under 500ms with local/mock data.
- Mobile-first and accessible controls.
- Data persistence through browser local storage for MVP.
- API-first architecture for future mobile app support.

## Acceptance Criteria
- User can complete onboarding in <= 2 minutes.
- At least one valid suggestion appears for every meal slot.
- Weekly planner can auto-fill and manual-edit.
- Recipe card includes ingredients, prep/cook time, and steps.
- Theme toggle persists across refresh.
- Events are logged for sign-up, suggestion, swap, and plan save.

## Out of Scope (MVP)
- Real payment collection.
- Live partner cart sync.
- AI-generated custom recipes.
- Multi-user household collaboration in real-time.
