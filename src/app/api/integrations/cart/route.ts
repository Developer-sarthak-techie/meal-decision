import { previewCartUrl, smartCartItemHints, type GroceryPartner } from "@/lib/integrations";

export async function POST(request: Request) {
  const body = (await request.json()) as {
    partner?: GroceryPartner;
    items?: Array<{ name: string; quantity: string }>;
  };
  if (!body.partner || !body.items || body.items.length === 0) {
    return Response.json({ error: "partner and items are required." }, { status: 400 });
  }
  const smartItems = smartCartItemHints(body.items);
  return Response.json({
    checkoutUrl: previewCartUrl(body.partner),
    status: "preview",
    itemCount: body.items.length,
    smartItems,
  });
}
