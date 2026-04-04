export type GroceryPartner = "zepto" | "blinkit" | "instamart" | "bigbasket";

export function previewCartUrl(partner: GroceryPartner) {
  return `https://${partner}.example.com/cart/preview`;
}
