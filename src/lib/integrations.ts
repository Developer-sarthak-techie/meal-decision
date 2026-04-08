export type GroceryPartner = "zepto" | "blinkit" | "instamart" | "bigbasket";

export function previewCartUrl(partner: GroceryPartner) {
  return `https://${partner}.example.com/cart/preview`;
}

/** Normalizes ingredient names toward high-intent quick-commerce search queries (MVP “smart cart” hints). */
const searchSynonyms: Record<string, string> = {
  onion: "onion 1kg",
  tomato: "tomato hybrid",
  rice: "sona masoori rice",
  "flattened rice": "poha thick",
  peanuts: "roasted peanuts",
  eggs: "white eggs 6",
  chicken: "chicken curry cut",
  paneer: "fresh paneer 200g",
  tofu: "tofu block",
  mushroom: "button mushroom",
  chickpea: "kabuli chana",
  lentil: "toor dal",
  "soy chunks": "soya chunks",
  "kidney beans": "rajma",
  "green peas": "frozen green peas",
  quinoa: "quinoa white",
  millet: "foxtail millet",
  noodles: "instant hakka noodles",
  garlic: "garlic peeled",
  ginger: "ginger fresh",
  coriander: "coriander bunch",
  mint: "mint leaves",
  lemon: "lemon",
  butter: "amul butter",
  "chili flakes": "chilli flakes",
  oatmeal: "oats quick cook",
};

export function smartCartItemHints(items: Array<{ name: string; quantity: string }>) {
  return items.map((item) => {
    const key = item.name.toLowerCase().trim();
    const searchQuery = searchSynonyms[key] ?? item.name;
    return {
      name: item.name,
      quantity: item.quantity,
      searchQuery,
    };
  });
}
