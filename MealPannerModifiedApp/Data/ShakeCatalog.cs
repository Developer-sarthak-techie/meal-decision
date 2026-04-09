using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Data;

public static class ShakeCatalog
{
    public static int Count => All.Count;

    private static readonly Lazy<IReadOnlyList<ShakeRecipe>> Cache = new(Build);
    private static readonly Lazy<IReadOnlyDictionary<string, ShakeRecipe>> ById = new(() =>
        Cache.Value.ToDictionary(s => s.Id, StringComparer.OrdinalIgnoreCase));

    public static IReadOnlyList<ShakeRecipe> All => Cache.Value;

    public static ShakeRecipe? FindById(string id) =>
        ById.Value.TryGetValue(id, out var s) ? s : null;

    private static IReadOnlyList<ShakeRecipe> Build() =>
    [
        new ShakeRecipe
        {
            Id = "shake-mango-lassi",
            Title = "Classic Mango Lassi",
            Subtitle = "Creamy yogurt & ripe mango — cooling and filling",
            ImageKey = "recipe_dish_03",
            PrepMinutes = 12,
            Ingredients =
            [
                "1 cup ripe mango cubes (fresh or thawed frozen)",
                "1 cup plain chilled yogurt (whole milk works best)",
                "2–3 tbsp sugar or honey (adjust to mango sweetness)",
                "¼ tsp cardamom powder (optional)",
                "2–4 ice cubes",
                "2 tbsp cold milk or water (only if blender needs help)",
                "Pinch of salt (optional — brightens flavor)"
            ],
            Steps =
            [
                "Wash hands. Cube mango and discard the pit; if using frozen mango, thaw slightly so the blender doesn’t struggle.",
                "Chill your serving glasses in the freezer for 5 minutes — optional, but keeps the lassi colder longer.",
                "Add mango, yogurt, sweetener, cardamom (if using), and ice to the blender jar.",
                "Blend on medium until completely smooth — 30–45 seconds. Stop once; scrape sides if you see mango fibers stuck.",
                "Taste: if too thick, add cold milk or water 1 tbsp at a time and pulse. If too thin, add 1–2 mango cubes.",
                "If you want a silkier texture, blend 10 seconds more after adding liquid.",
                "Pour into chilled glasses. For garnish: a tiny sprinkle of cardamom or a thin mango slice on the rim.",
                "Serve immediately. If storing, keep up to 24 hours sealed in the fridge; stir before drinking as yogurt may settle."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-watermelon-mint",
            Title = "Watermelon Mint Cooler",
            Subtitle = "Light, hydrating — no dairy",
            ImageKey = "recipe_dish_08",
            PrepMinutes = 10,
            Ingredients =
            [
                "3 cups chilled seedless watermelon chunks",
                "8–12 fresh mint leaves (no tough stems)",
                "1 tbsp lime or lemon juice, freshly squeezed",
                "1–2 tsp sugar or agave (optional — watermelon varies)",
                "Pinch of black salt or regular salt",
                "Ice cubes as needed",
                "Cold sparkling water (optional — for fluff)"
            ],
            Steps =
            [
                "Refrigerate watermelon for at least 1 hour before blending — warm melon tastes flatter.",
                "Pick mint leaves and rinse; pat dry — wet mint can foam oddly in some blenders.",
                "Add watermelon, mint, lime juice, sweetener (if using), salt, and a handful of ice to the blender.",
                "Pulse 3–4 times, then blend high for 15–25 seconds until mint is fully broken down and texture is uniform.",
                "Strain through a fine sieve if you dislike pulp; for more fiber, skip straining.",
                "Taste: adjust lime for brightness, salt for savoriness, sweetener only if the melon is underripe.",
                "For a fizzy version: pour ¾ of the blend into glasses, top with cold sparkling water, stir gently.",
                "Serve right away — watermelon separates quickly; if waiting, stir before serving."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-strawberry-banana-oat",
            Title = "Strawberry Banana Oat Smoothie",
            Subtitle = "Filling breakfast shake with fiber",
            ImageKey = "recipe_dish_05",
            PrepMinutes = 10,
            Ingredients =
            [
                "1 medium ripe banana (fresh or sliced frozen)",
                "6–8 fresh strawberries (hulled) or 1 cup frozen",
                "¼ cup rolled oats (raw — they blend into creaminess)",
                "1 cup chilled milk (dairy or oat/almond milk)",
                "1 tsp honey or maple syrup",
                "¼ tsp cinnamon (optional)",
                "Ice cubes if not using frozen fruit"
            ],
            Steps =
            [
                "Peel banana and hull berries. If using frozen banana, break into 2–3 chunks for even blending.",
                "Add oats first, then milk — letting oats sit 2 minutes softens them slightly (optional but smoother).",
                "Add fruit, sweetener, cinnamon, and ice (if needed). Cap the blender tightly.",
                "Start on low 10 seconds, then high 40–60 seconds until completely smooth — oats should disappear visually.",
                "If too thick, add milk 2 tbsp at a time. If too thin, add half a banana or a few more berries.",
                "Pour into a tall glass. Optional: top with a sliced strawberry or a dusting of cinnamon.",
                "Best fresh; if saving, refrigerate up to 8 hours in an airtight jar — shake hard before drinking."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-pina-colada",
            Title = "Coconut Pineapple Colada (Virgin)",
            Subtitle = "Tropical summer treat — no alcohol",
            ImageKey = "recipe_dish_12",
            PrepMinutes = 12,
            Ingredients =
            [
                "1 cup chilled pineapple chunks (fresh or canned in juice, drained)",
                "½ cup thick coconut milk (canned, shaken)",
                "½ cup coconut water or cold milk",
                "1–2 tbsp sugar or condensed milk (optional)",
                "½ cup ice cubes",
                "Tiny pinch of salt",
                "Toasted coconut flakes for garnish (optional)"
            ],
            Steps =
            [
                "If using canned pineapple, drain well — excess syrup dilutes flavor and sweetness unpredictably.",
                "Shake coconut milk can; scoop any solid cream — blend the solids for richness and the thin liquid for body.",
                "Combine pineapple, coconut milk, liquid (coconut water or milk), sweetener, salt, and ice in blender.",
                "Blend high 45–60 seconds until silky; tiny pineapple fibers might remain — blend longer if you want smoother.",
                "Taste: pineapple should lead, coconut should follow. Add a squeeze of lime (optional) for balance.",
                "For extra frost: freeze pineapple chunks overnight and reduce added ice.",
                "Pour into glasses; garnish with coconut flakes or a pineapple wedge on the rim.",
                "Serve cold immediately — coconut fat can solidify oddly if left in a very cold fridge; a quick stir fixes it."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-cucumber-mint-yogurt",
            Title = "Cucumber Mint Yogurt Shake",
            Subtitle = "Savory-sweet chaas-inspired cooler",
            ImageKey = "recipe_dish_02",
            PrepMinutes = 11,
            Ingredients =
            [
                "1 small cucumber (English or peeled regular)",
                "1 cup chilled plain yogurt",
                "½ cup cold water",
                "6–10 mint leaves",
                "¼ tsp roasted cumin powder",
                "Salt and black pepper to taste",
                "Ice cubes"
            ],
            Steps =
            [
                "Peel cucumber if the skin is bitter or thick; for thin English cucumber skin, you may keep it.",
                "Rough-chop cucumber so blades catch evenly — large rounds can bounce in weak blenders.",
                "Blend cucumber and mint with water first on high 20 seconds for a smooth green base.",
                "Add yogurt, cumin, salt, pepper, and ice. Blend medium 25–35 seconds until frothy but pourable.",
                "Taste like soup: it should be refreshing, not bland — bump salt or cumin carefully in tiny pinches.",
                "For traditional texture, strain once; for fiber, keep pulp.",
                "Serve in tall glasses with extra mint on top and a pinch of cumin.",
                "Consume within a few hours; cucumber flavors fade and yogurt can sour if left warm."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-rose-thandai",
            Title = "Rose Thandai Milkshake",
            Subtitle = "Festive nuts & rose — make spice mix ahead",
            ImageKey = "recipe_dish_13",
            PrepMinutes = 25,
            Ingredients =
            [
                "For paste (makes extra — store 3 days): 10 almonds, 8 pistachios, 1 tbsp poppy seeds or melon seeds, ½ tsp fennel, 2 green cardamom pods (seeds only), pinch black pepper, pinch saffron (optional)",
                "2 cups chilled full-fat milk",
                "2–3 tbsp sugar or jaggery powder",
                "1–2 tsp rose water (food-grade)",
                "Ice cubes",
                "Crushed pistachios and dried rose petals to garnish"
            ],
            Steps =
            [
                "Soak almonds and pistachios in hot water 15 minutes; slip off almond skins for smoother paste.",
                "In a small grinder, blend soaked nuts with poppy/melon seeds, fennel, cardamom seeds, pepper, saffron, and 2–3 tbsp milk until very fine.",
                "Whisk or blend this paste with chilled milk, sugar, and rose water — start with less rose; it’s strong.",
                "Add ice and blend 15 seconds for frost, or shake in a closed jar if you want less foam.",
                "Strain if you prefer perfectly smooth texture; thandai is often lightly textured — your choice.",
                "Balance: not too perfumey from rose, not too sweet — taste between additions.",
                "Garnish with crushed nuts and petals; serve in chilled metal cups if you have them.",
                "Leftover paste freezes in an ice-cube tray up to 2 weeks — thaw in milk for instant shakes."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-cold-coffee",
            Title = "Cold Coffee Frappe",
            Subtitle = "Iced blended coffee — café style at home",
            ImageKey = "recipe_dish_09",
            PrepMinutes = 10,
            Ingredients =
            [
                "1 cup strong chilled coffee (brewed, cooled — espresso diluted works)",
                "½ cup chilled milk",
                "2–3 tbsp sugar or simple syrup",
                "1 cup ice cubes",
                "½ tsp vanilla extract (optional)",
                "1 small scoop vanilla ice cream (optional — for indulgence)",
                "Cocoa powder or chocolate shavings (optional garnish)"
            ],
            Steps =
            [
                "Brew coffee stronger than usual — ice will mute flavor. Cool completely before blending (hot + ice = bitter oils).",
                "If using sugar crystals, dissolve in 2 tbsp hot coffee first; syrup blends easier cold.",
                "Add coffee, milk, sweetener, vanilla, ice, and optional ice cream to blender.",
                "Blend high 30–45 seconds until you hear fewer ice chips — texture should be thick-pourable.",
                "Too thin? Add ice. Too bitter? Tiny pinch salt or more milk. Too weak? Add a shot of espresso or coffee concentrate.",
                "Pour into glass; optional whipped cream layer, then dust with cocoa.",
                "Drink through a wide straw if very thick — serve immediately before melt dilutes it.",
                "For a lighter version: skip ice cream, use extra ice and a touch more milk."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-peach-iced",
            Title = "Peach Iced Yogurt Shake",
            Subtitle = "Stone-fruit summer classic",
            ImageKey = "recipe_dish_07",
            PrepMinutes = 10,
            Ingredients =
            [
                "2 ripe peaches, pitted and chopped (or 1½ cups frozen peach slices)",
                "¾ cup plain Greek yogurt",
                "½ cup cold milk",
                "2–3 tbsp honey",
                "½ tsp vanilla extract",
                "Ice cubes",
                "Fresh peach slice for garnish"
            ],
            Steps =
            [
                "Blanch peaches 30 seconds if skins are tough, then cool in ice water and peel — smooth skin improves texture.",
                "Remove pits completely — bits of pit taste awful and damage blades.",
                "Combine peach, yogurt, milk, honey, vanilla, and ice in blender.",
                "Blend medium-high 40 seconds until peach fibers disappear — frozen peaches may need 10 seconds longer.",
                "Taste: peaches vary wildly in sugar — adjust honey by ½ tsp steps.",
                "If tangy: add 1 tbsp milk; if flat: tiny squeeze of lemon (surprising lift).",
                "Pour over fresh ice in glasses; garnish with peach wedge.",
                "Keeps refrigerated ~24 hours; color may brown slightly — a pinch of vitamin C powder (optional) slows oxidation."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-blueberry-greek",
            Title = "Blueberry Greek Protein Smoothie",
            Subtitle = "Thick, purple, satisfying",
            ImageKey = "recipe_dish_11",
            PrepMinutes = 8,
            Ingredients =
            [
                "1 cup blueberries (fresh or frozen)",
                "¾ cup plain Greek yogurt",
                "½ banana (for sweetness and silk)",
                "½ cup milk",
                "1 tbsp almond butter or peanut butter (optional)",
                "1 tbsp honey or maple syrup",
                "Ice if using fresh berries only"
            ],
            Steps =
            [
                "Rinse fresh blueberries and remove stems; frozen berries go straight in.",
                "Add banana first at the bottom — it pulls toward blades and blends more evenly.",
                "Stack yogurt, berries, nut butter, sweetener, milk, then ice.",
                "Blend low 15 seconds (breaks air pockets), then high 45 seconds until color is even purple with no specks.",
                "Scrape lid and sides once mid-blend if clumps stick.",
                "Adjust thickness: more milk thins; more yogurt or frozen banana thickens.",
                "Pour; top with a few whole berries if presenting.",
                "Best consumed same day — blueberries + dairy can raise pH questions for very long storage; 24h max chilled."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-aam-panna",
            Title = "Raw Mango Summer Cooler (Aam Panna style)",
            Subtitle = "Tangy, salted-sweet — beat the heat",
            ImageKey = "recipe_dish_04",
            PrepMinutes = 30,
            Ingredients =
            [
                "1 medium unripe green mango (raw)",
                "4 cups water (for boiling mango — you’ll use pulp + some liquid)",
                "½ tsp roasted cumin powder",
                "½ tsp black salt (kala namak) or combo of table salt + pinch of chaat masala",
                "Sugar or jaggery to taste (often 3–6 tbsp depending on sourness)",
                "Mint leaves (handful)",
                "Ice and chilled water to finish"
            ],
            Steps =
            [
                "Wash mango; pressure-boil or simmer whole with skin until very soft — 12–18 minutes depending on size.",
                "Cool until handleable; squeeze pulp from skin and pit into a bowl — discard skin and pit.",
                "Mash pulp smooth with a fork or blend briefly — tiny fibers are OK.",
                "Dissolve sweetener in ½ cup warm water from the boil; mix into pulp until sugar fully integrates.",
                "Add cumin, black salt, chopped mint; taste: should be sour-sweet-savory, like a thirst quencher.",
                "Thin with chilled water to a drinkable pour — usually 2:1 water to thick pulp, adjustable.",
                "For shake-style: blend pulp mixture with ice and a cup of cold water until frothy.",
                "Store concentrate sealed in fridge up to 4 days — dilute fresh each serving; do not leave unrefrigerated in heat."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-chaas-lemon",
            Title = "Lemon Mint Buttermilk (Masala Chaas)",
            Subtitle = "Savory Indian buttermilk — digestive & cooling",
            ImageKey = "recipe_dish_01",
            PrepMinutes = 8,
            Ingredients =
            [
                "1½ cups chilled plain buttermilk or watered yogurt (1 cup yogurt + ½ cup water whisked)",
                "1–2 tbsp fresh lemon or lime juice",
                "6–8 mint leaves, torn",
                "¼ tsp roasted cumin powder",
                "Pinch hing (asafoetida), optional",
                "Salt to taste",
                "Ice cubes"
            ],
            Steps =
            [
                "If starting from thick yogurt, whisk with cold water until no lumps — consistency like thin soup.",
                "Crush mint lightly in mortar or chop fine — releases oil without bitterness.",
                "Whisk in lemon juice, cumin, hing, salt — taste before adding ice so seasoning is accurate.",
                "Add ice to a jar; pour buttermilk; shake 15 seconds for froth OR blend 10 seconds for foam cap.",
                "If blended, rest 1 minute so foam settles slightly — easier to drink.",
                "Adjust: more lemon for hot days, more salt if bland, more cumin for aroma.",
                "Serve in steel or glass with mint on top.",
                "Finish within 2–3 hours if unrefrigerated; if chilled, same day is best."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-papaya-orange",
            Title = "Papaya Orange Sunrise",
            Subtitle = "Vitamin C forward — bright color",
            ImageKey = "recipe_dish_06",
            PrepMinutes = 10,
            Ingredients =
            [
                "1½ cups ripe papaya cubes",
                "Juice of 1 large orange (about ½ cup)",
                "½ cup chilled coconut water or water",
                "1 tbsp honey or sugar (optional)",
                "Pinch of salt",
                "Ice cubes",
                "Orange wheel garnish"
            ],
            Steps =
            [
                "Choose fragrant papaya — underripe tastes chalky; overripe can ferment fast.",
                "Scoop seeds, peel if skin is thick, cube uniformly.",
                "Juice orange fresh; strain if you hate pulp — pulp adds body though.",
                "Blend papaya, orange juice, coconut water, sweetener, salt, and ice until ultra-smooth.",
                "Layer trick (optional): pour half plain into glass, slowly pour remainder on a spoon back for gradient.",
                "Taste: orange should brighten papaya’s earthy edge; add lime splash if too sweet.",
                "Garnish glass rim with salt-sugar mix (optional, tajín-style without chili).",
                "Drink immediately — papaya oxidizes; separation is normal after sitting — stir."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-chocolate-banana",
            Title = "Chocolate Banana Oat Shake",
            Subtitle = "Treat-like but still a shake",
            ImageKey = "recipe_dish_14",
            PrepMinutes = 9,
            Ingredients =
            [
                "1 large frozen banana chunks",
                "2 tbsp unsweetened cocoa powder",
                "1 cup milk",
                "2 tbsp rolled oats",
                "2–3 tbsp maple syrup or dates (pitted)",
                "Pinch of salt",
                "Ice optional if banana not frozen"
            ],
            Steps =
            [
                "Freeze banana peeled and sliced at least 4 hours — overnight is better.",
                "If using dates, soak in warm water 5 minutes if very dry — softer dates blend cleaner.",
                "Add oats and milk first; pulse to wet oats, wait 1 minute (optional softening).",
                "Add banana, cocoa, sweetener, salt, ice — blend high 50 seconds until mousse-like.",
                "Scrape down cocoa that clings to walls — blend again 15 seconds.",
                "Taste: cocoa can read bitter → sweeten slowly. A drop of vanilla rounds chocolate.",
                "Serve in chilled glass; shave dark chocolate on top if you want presentation.",
                "Texture sets thick; add milk in tablespoons if stuck in blender."
            ]
        },
        new ShakeRecipe
        {
            Id = "shake-avocado-lime",
            Title = "Avocado Lime Green Shake",
            Subtitle = "Creamy dairy-free option with coconut",
            ImageKey = "recipe_dish_10",
            PrepMinutes = 10,
            Ingredients =
            [
                "½ ripe avocado (no brown strings)",
                "1 cup coconut water or cold water",
                "2–3 tbsp lime juice",
                "2 tbsp honey or agave",
                "Pinch salt",
                "Handful spinach (optional — deepens color)",
                "Ice cubes"
            ],
            Steps =
            [
                "Halve avocado, remove pit safely with spoon — never stab pit with knife held in hand.",
                "Scoop flesh; discard any brown spots — they taste oxidized.",
                "Blend avocado, lime, sweetener, salt, spinach (if using), and half the liquid first until silky.",
                "Add remaining liquid and ice; blend 30 seconds — should look like pale green velvet.",
                "Too thick (common): more coconut water. Too flat: more lime in ½ tsp steps.",
                "Serve with thin lime wheel; drink fast — avocado browns top layer after ~20 minutes (stir fixes slightly).",
                "Not ideal for long storage — make to order.",
                "For protein: add a scoop of neutral plant protein and extra water."
            ]
        }
    ];
}
