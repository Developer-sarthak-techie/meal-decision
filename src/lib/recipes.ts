import type { MealSlot, OccasionKey, Recipe, SpiceLevel } from "@/types/domain";

const occasionPool: Exclude<OccasionKey, "everyday">[] = [
  "diwali",
  "holi",
  "eid",
  "christmas",
  "navratri",
  "ganesh_chaturthi",
  "onam",
  "birthday",
  "party",
];

function occasionTagsForIndex(i: number): OccasionKey[] {
  const extra = occasionPool[i % occasionPool.length];
  const extra2 = occasionPool[(i + 3) % occasionPool.length];
  if (i % 5 === 0) return ["everyday", extra, extra2];
  return ["everyday", extra];
}

const mealSlots: MealSlot[] = ["breakfast", "lunch", "dinner", "snack"];
const spiceLevels: SpiceLevel[] = ["low", "medium", "high"];

const cuisines = [
  "North Indian",
  "South Indian",
  "Maharashtrian",
  "Gujarati",
  "Punjabi",
  "Bengali",
  "Rajasthani",
  "Kerala",
  "Andhra",
  "Indo-Chinese",
  "Thai",
  "Mediterranean",
  "Continental",
  "Mexican",
  "Middle Eastern",
  "Japanese",
  "Korean",
  "Italian",
  "Turkish",
  "Sri Lankan",
];

const flavorNotes = [
  "Smoky",
  "Herbed",
  "Citrus",
  "Pepper",
  "Garlic",
  "Tandoori",
  "Tangy",
  "Coconut",
  "Mustard",
  "Masala",
  "Toasted",
  "Creamy",
  "Spicy",
];

const breakfastVegBases = ["Poha", "Upma", "Idli Bowl", "Paneer Toast", "Moong Chilla", "Dalia", "Besan Wrap"];
const breakfastNonVegBases = [
  "Egg Bhurji Wrap",
  "Chicken Toastie",
  "Masala Omelette",
  "Egg Fried Quinoa",
  "Turkey Sandwich",
  "Tuna Toast",
];
const lunchVegBases = ["Khichdi Bowl", "Rajma Rice", "Paneer Curry", "Veg Pulao", "Dal Tadka", "Veg Stir Fry"];
const lunchNonVegBases = [
  "Chicken Curry",
  "Fish Masala",
  "Mutton Stew",
  "Egg Keema Rice",
  "Chicken Stir Fry",
  "Prawn Coconut Curry",
];
const dinnerVegBases = [
  "Paneer Bhurji",
  "Tofu Bowl",
  "Mushroom Masala",
  "Veg Kofta",
  "Dal Makhani",
  "Soya Kebab Plate",
];
const dinnerNonVegBases = ["Egg Curry", "Chicken Handi", "Fish Roast", "Lemon Chicken", "Keema Bowl", "Prawn Stir Fry"];
const snackVegBases = ["Sprouts Chaat", "Veg Sandwich", "Corn Chaat", "Hummus Roll", "Paneer Tikka Bites", "Fruit Chaat"];
const snackNonVegBases = [
  "Chicken Sandwich",
  "Egg Mayo Roll",
  "Tuna Salad Cup",
  "Chicken Tikka Bites",
  "Prawn Toast",
  "Smoked Chicken Wrap",
];

const proteinsVeg = ["Paneer", "Tofu", "Mushroom", "Chickpea", "Lentil", "Soy Chunks", "Kidney Beans", "Green Peas"];
const proteinsNonVeg = ["Chicken", "Egg", "Fish", "Prawn", "Turkey", "Mutton"];
const carbBase = ["Rice", "Millet", "Quinoa", "Whole Wheat", "Oats", "Brown Rice", "Sourdough", "Noodles"];
const aromatics = ["Onion", "Garlic", "Ginger", "Tomato", "Curry Leaves", "Mint", "Coriander", "Spring Onion"];
const garnish = ["Lemon", "Sesame", "Peanuts", "Kasuri Methi", "Butter", "Chili Flakes", "Roasted Jeera"];

function pickBase(slot: MealSlot, isVeg: boolean, i: number) {
  const source =
    slot === "breakfast"
      ? isVeg
        ? breakfastVegBases
        : breakfastNonVegBases
      : slot === "lunch"
        ? isVeg
          ? lunchVegBases
          : lunchNonVegBases
        : slot === "dinner"
          ? isVeg
            ? dinnerVegBases
            : dinnerNonVegBases
          : isVeg
            ? snackVegBases
            : snackNonVegBases;
  return source[i % source.length];
}

function buildRecipe(i: number): Recipe {
  const slot = mealSlots[i % mealSlots.length];
  const isVeg = i % 2 === 0;
  const dietMode = isVeg ? "veg" : "non_veg";
  const cuisine = cuisines[i % cuisines.length];
  const note = flavorNotes[i % flavorNotes.length];
  const spiceLevel = spiceLevels[i % spiceLevels.length];
  const base = pickBase(slot, isVeg, i);
  const protein = isVeg ? proteinsVeg[i % proteinsVeg.length] : proteinsNonVeg[i % proteinsNonVeg.length];
  const carb = carbBase[i % carbBase.length];
  const aromatic = aromatics[i % aromatics.length];
  const finish = garnish[i % garnish.length];
  const prepTimeMins = 6 + (i % 12);
  const cookTimeMins = slot === "snack" ? 8 + (i % 12) : 14 + (i % 26);
  const popularityScore = 65 + (i % 34);
  const name = `${cuisine} ${note} ${base}`;
  const slotCost = slot === "dinner" ? 55 : slot === "lunch" ? 35 : slot === "breakfast" ? 15 : 10;
  const spiceCost = spiceLevel === "high" ? 25 : spiceLevel === "medium" ? 12 : 0;
  const estimatedCostINR = Math.min(
    920,
    Math.round(
      75 + slotCost + spiceCost + (isVeg ? 0 : 110) + (i % 140) * 4 + (i % 7) * 12,
    ),
  );

  return {
    id: `g-${i + 1}`,
    name,
    mealSlot: slot,
    cuisine,
    dietMode,
    spiceLevel,
    prepTimeMins,
    cookTimeMins,
    ingredients: [
      { name: protein, quantity: "200g" },
      { name: carb, quantity: "1.5 cups" },
      { name: aromatic, quantity: "1 cup chopped" },
      { name: finish, quantity: "1 tbsp" },
    ],
    steps: [
      `Prep and marinate ${protein.toLowerCase()} with ${note.toLowerCase()} spices.`,
      `Cook ${aromatic.toLowerCase()} and combine with ${carb.toLowerCase()}.`,
      `Finish with ${finish.toLowerCase()} and serve ${slot}.`,
    ],
    popularityScore,
    estimatedCostINR,
    occasionTags: occasionTagsForIndex(i),
  };
}

function generateRecipeCatalog(targetCount: number) {
  const generated: Recipe[] = [];
  for (let i = 0; i < targetCount; i += 1) {
    generated.push(buildRecipe(i));
  }
  return generated;
}

export const seedRecipes: Recipe[] = [
  {
    id: "r1",
    name: "Poha",
    mealSlot: "breakfast",
    cuisine: "Maharashtrian",
    dietMode: "veg",
    spiceLevel: "low",
    prepTimeMins: 8,
    cookTimeMins: 12,
    ingredients: [
      { name: "Flattened rice", quantity: "2 cups" },
      { name: "Onion", quantity: "1 medium" },
      { name: "Peanuts", quantity: "2 tbsp" },
    ],
    steps: ["Rinse poha lightly.", "Saute onion and peanuts.", "Mix poha and steam 2 mins."],
    popularityScore: 85,
    estimatedCostINR: 95,
    occasionTags: ["everyday", "navratri", "ganesh_chaturthi"],
  },
  {
    id: "r2",
    name: "Masala Omelette",
    mealSlot: "breakfast",
    cuisine: "Indian",
    dietMode: "non_veg",
    spiceLevel: "medium",
    prepTimeMins: 5,
    cookTimeMins: 10,
    ingredients: [
      { name: "Eggs", quantity: "3" },
      { name: "Onion", quantity: "1 small" },
      { name: "Tomato", quantity: "1 small" },
    ],
    steps: ["Beat eggs with vegetables.", "Cook on pan till set.", "Serve hot."],
    popularityScore: 78,
    estimatedCostINR: 120,
    occasionTags: ["everyday", "birthday", "party"],
  },
  {
    id: "r3",
    name: "Rajma Chawal",
    mealSlot: "lunch",
    cuisine: "North Indian",
    dietMode: "veg",
    spiceLevel: "medium",
    prepTimeMins: 15,
    cookTimeMins: 30,
    ingredients: [
      { name: "Rajma", quantity: "1 cup" },
      { name: "Rice", quantity: "1.5 cups" },
      { name: "Tomato puree", quantity: "1 cup" },
    ],
    steps: ["Pressure cook rajma.", "Cook masala gravy.", "Simmer and serve with rice."],
    popularityScore: 89,
    estimatedCostINR: 185,
    occasionTags: ["everyday", "diwali", "holi"],
  },
  {
    id: "r4",
    name: "Chicken Curry",
    mealSlot: "lunch",
    cuisine: "Indian",
    dietMode: "non_veg",
    spiceLevel: "high",
    prepTimeMins: 20,
    cookTimeMins: 35,
    ingredients: [
      { name: "Chicken", quantity: "500g" },
      { name: "Onion", quantity: "2 medium" },
      { name: "Ginger garlic paste", quantity: "1 tbsp" },
    ],
    steps: ["Brown onions.", "Add chicken and spices.", "Cook till tender."],
    popularityScore: 82,
    estimatedCostINR: 320,
    occasionTags: ["everyday", "eid", "party"],
  },
  ...generateRecipeCatalog(1200),
];

export function recipesBySlot(slot: MealSlot) {
  return seedRecipes.filter((recipe) => recipe.mealSlot === slot);
}
