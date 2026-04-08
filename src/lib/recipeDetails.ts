import type { BreadChoice, Recipe } from "@/types/domain";

export const breadChoiceOptions: { value: BreadChoice; label: string }[] = [
  { value: "none", label: "No bread / carb pairing" },
  { value: "phulka", label: "Soft phulka (roti)" },
  { value: "tandoori_roti", label: "Tandoori roti" },
  { value: "butter_naan", label: "Butter naan" },
  { value: "garlic_naan", label: "Garlic naan" },
  { value: "laccha_paratha", label: "Laccha paratha" },
  { value: "aloo_paratha", label: "Aloo paratha" },
  { value: "missi_roti", label: "Missi roti" },
  { value: "bhakri", label: "Bhakri (jowar/bajra)" },
  { value: "steamed_rice", label: "Steamed rice" },
  { value: "jeera_rice", label: "Jeera rice" },
  { value: "bread_toast", label: "Bread / toast" },
];

export function getBreadChoiceLabel(choice: BreadChoice): string {
  return breadChoiceOptions.find((o) => o.value === choice)?.label ?? choice;
}

export function getBreadPairingNotes(recipe: Recipe, choice: BreadChoice): string {
  if (choice === "none") {
    return "No bread or rice pairing selected — serve the main dish on its own or add your favourite side later.";
  }
  const main = recipe.name;
  const map: Record<Exclude<BreadChoice, "none">, string> = {
    phulka:
      `Roll soft phulkas fresh while ${main} is resting. Keep rotis wrapped in a cloth-lined casserole so they stay puffy. Tear pieces to scoop up the gravy.`,
    tandoori_roti:
      `Brush tandoori rotis with a little water before reheating on a hot tawa for crisp edges. Pair with ${main} for a smoky, restaurant-style plate.`,
    butter_naan:
      `Warm naan on a dry pan, brush lightly with melted butter, and serve hot alongside ${main} — fold naan to soak up the sauce.`,
    garlic_naan:
      `Garlic naan pairs best with creamy or tomato-based gravies. Finish ${main} with a sprinkle of coriander before serving with hot naan.`,
    laccha_paratha:
      `Laccha paratha adds flaky layers — use torn pieces to pick up ${main}. Reheat on low heat so the layers stay crisp, not dry.`,
    aloo_paratha:
      `If serving aloo paratha with ${main}, keep the gravy slightly thicker so the plate is not too heavy. Balance with a light salad or raita.`,
    missi_roti:
      `Missi roti is savoury and dense — pair with ${main} when you want a rustic flavour. Serve with a dollop of white butter or pickle.`,
    bhakri:
      `Bhakri is sturdy and gluten-free friendly — break into pieces and dip into ${main}. Add a thin drizzle of ghee on the bhakri if serving dry.`,
    steamed_rice:
      `Steamed rice is ideal for saucy curries. Fluff rice after resting 5 minutes, then ladle ${main} on the side or over the top.`,
    jeera_rice:
      `Jeera rice adds aroma without competing — plate ${main} beside the rice so guests can mix flavours as they like.`,
    bread_toast:
      `Toast bread until golden for contrast with saucy ${main}, or use soft slices for wraps if the dish is dry enough to hold.`,
  };
  return map[choice as Exclude<BreadChoice, "none">];
}

export interface DetailedStepBlock {
  stepNumber: number;
  summary: string;
  microSteps: string[];
}

export function buildDetailedSteps(recipe: Recipe): DetailedStepBlock[] {
  return recipe.steps.map((step, index) => {
    const microSteps = [
      `Gather everything needed for: ${step.slice(0, 80)}${step.length > 80 ? "…" : ""}`,
      `Work on a stable heat: medium for aromatics, adjust higher only after liquids are in.`,
      `Taste for salt and spice before finishing — ${recipe.spiceLevel} heat profile for this dish.`,
    ];
    return {
      stepNumber: index + 1,
      summary: step,
      microSteps,
    };
  });
}

export function getChefTips(recipe: Recipe): string[] {
  const tips: string[] = [
    `Mise en place: chop all ingredients for ${recipe.name} before you turn on the flame.`,
    `This ${recipe.cuisine} dish uses a ${recipe.spiceLevel} spice level — adjust chili and salt at the end.`,
  ];
  if (recipe.dietMode === "veg") {
    tips.push("For veg proteins (paneer/tofu), add them after the gravy is nearly done to avoid rubbery texture.");
  } else {
    tips.push("For meats/eggs, rest the dish 2–3 minutes off heat so juices settle before serving.");
  }
  tips.push("Leftovers: cool quickly, refrigerate within 2 hours, and reheat until piping hot.");
  return tips;
}

export function getIngredientPrepNotes(recipe: Recipe): { name: string; quantity: string; note: string }[] {
  return recipe.ingredients.map((ing) => ({
    ...ing,
    note:
      ing.name.toLowerCase().includes("paneer") || ing.name.toLowerCase().includes("tofu")
        ? "Cube or crumble; add late if cooking in gravy."
        : ing.name.toLowerCase().includes("rice") || ing.name.toLowerCase().includes("quinoa")
          ? "Rinse until water runs clearer; drain well."
          : "Prep uniformly so everything cooks evenly.",
  }));
}
