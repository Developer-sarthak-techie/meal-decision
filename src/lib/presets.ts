import type { Preferences, WeekdayKey } from "@/types/domain";

const allVegDays = (): Record<WeekdayKey, Preferences["dietMode"]> => ({
  mon: "veg",
  tue: "veg",
  wed: "veg",
  thu: "veg",
  fri: "veg",
  sat: "veg",
  sun: "veg",
});

export function applyPgHostelPack(): Partial<Preferences> {
  return {
    dietMode: "veg",
    dayWiseDiet: allVegDays(),
    maxCookTimeMins: 30,
    spiceLevel: "medium",
    budgetMode: true,
    budgetTier: "budget",
    pantryMode: false,
    noveltyLevel: "classic",
    tonightMode: false,
  };
}

export function applyFamilyVegWeek(): Partial<Preferences> {
  return {
    dietMode: "veg",
    dayWiseDiet: allVegDays(),
    maxCookTimeMins: 45,
    budgetMode: false,
    budgetTier: "moderate",
    noveltyLevel: "balanced",
    tonightMode: false,
  };
}

export function applyBudgetWeek(): Partial<Preferences> {
  return {
    dietMode: "mix",
    maxCookTimeMins: 40,
    budgetMode: true,
    budgetTier: "budget",
    noveltyLevel: "classic",
    tonightMode: false,
    dayWiseDiet: {
      mon: "veg",
      tue: "veg",
      wed: "mix",
      thu: "veg",
      fri: "mix",
      sat: "non_veg",
      sun: "non_veg",
    },
  };
}
