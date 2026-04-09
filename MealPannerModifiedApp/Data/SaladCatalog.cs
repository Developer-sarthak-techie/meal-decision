using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Data;

public static class SaladCatalog
{
    public static int Count => All.Count;

    private static readonly Lazy<IReadOnlyList<SaladRecipe>> Cache = new(Build);
    private static readonly Lazy<IReadOnlyDictionary<string, SaladRecipe>> ById = new(() =>
        Cache.Value.ToDictionary(s => s.Id, StringComparer.OrdinalIgnoreCase));

    public static IReadOnlyList<SaladRecipe> All => Cache.Value;

    public static SaladRecipe? FindById(string id) =>
        ById.Value.TryGetValue(id, out var s) ? s : null;

    private static IReadOnlyList<SaladRecipe> Build() =>
    [
        new SaladRecipe
        {
            Id = "salad-garden-vinaigrette",
            Title = "Classic Garden Salad",
            Subtitle = "Crisp greens with mustard vinaigrette",
            ImageKey = "recipe_dish_02",
            PrepMinutes = 20,
            ServesNote = "Serves 2 as a main side",
            Ingredients =
            [
                "1 small head mixed lettuce or 4 cups washed salad greens, torn",
                "½ cucumber, sliced half-moons",
                "1 medium carrot, shredded or ribboned",
                "6 radishes, thinly sliced (optional)",
                "For dressing: 3 tbsp extra-virgin olive oil",
                "1½ tbsp red wine vinegar or lemon juice",
                "1 tsp Dijon mustard",
                "½ tsp honey or sugar",
                "Salt and black pepper",
                "2 tbsp chopped fresh parsley or chives (optional)"
            ],
            Steps =
            [
                "Wash greens in cold water; spin very dry — water on leaves dilutes dressing and kills crunch.",
                "Prep vegetables uniformly so each forkful has similar texture — shave carrot with peeler for ribbons if you prefer.",
                "Whisk dressing in a jar: oil, vinegar, mustard, honey, pinch salt, several grinds pepper — emulsion should look slightly thick.",
                "Taste dressing alone: bright and balanced; adjust acid if flat, oil if too sharp.",
                "For best texture: toss greens in a wide bowl with only half the dressing first, add more by tablespoon — leaves should glisten, not pool.",
                "Add cucumber, carrot, radish; toss gently with hands or tongs to avoid bruising.",
                "Finish with herbs on top so they stay vivid.",
                "Serve on chilled plates. If not eating immediately, store undressed greens airtight with a paper towel; dress within 2 hours."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-greek",
            Title = "Greek Village Salad (Horiatiki)",
            Subtitle = "Tomato, cucumber, olive, feta — no lettuce",
            ImageKey = "recipe_dish_04",
            PrepMinutes = 15,
            ServesNote = "Serves 2–3",
            Ingredients =
            [
                "2 large ripe tomatoes, cut into wedges",
                "1 cucumber, partly peeled in stripes, thick slices",
                "½ red onion, thin half-moons (soak in cold water 10 min if strong)",
                "1 green bell pepper, chunks (optional)",
                "80–120g feta cheese, block or thick slice",
                "12–16 Kalamata olives, pitted",
                "1 tsp dried oregano",
                "3–4 tbsp extra-virgin olive oil",
                "1 tbsp red wine vinegar",
                "Salt — go easy until feta is tasted",
                "Dried or fresh oregano for finish"
            ],
            Steps =
            [
                "Cut tomatoes with a sharp serrated knife to avoid crushing; keep juices in the bowl — they become part of the dressing.",
                "Layer tomato, cucumber, pepper, onion in a shallow wide bowl — traditional style is rustic, not tiny dice.",
                "Set feta on top in one piece or large cubes rather than crumbles if you want authentic texture.",
                "Scatter olives around; sprinkle half the dried oregano.",
                "Drizzle olive oil and vinegar over everything; finish with remaining oregano and a light pinch of salt (feta is salty).",
                "Let sit 5–10 minutes at room temperature — juices meld and flavor improves.",
                "Toss gently at the table or serve composed with crusty bread for sopping juices.",
                "Best same day; refrigerate leftovers up to 24 hours but texture of tomato will soften."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-caprese",
            Title = "Caprese Salad",
            Subtitle = "Tomato, mozzarella, basil — Italian summer",
            ImageKey = "recipe_dish_07",
            PrepMinutes = 12,
            ServesNote = "Serves 2 as appetizer",
            Ingredients =
            [
                "2 large heirloom or beefsteak tomatoes, room temperature",
                "200g fresh mozzarella (bufala or fior di latte), sliced",
                "15–20 fresh basil leaves",
                "3 tbsp extra-virgin olive oil (good quality)",
                "Flaky sea salt and black pepper",
                "1 tsp balsamic glaze or aged balsamic (optional, use sparingly)",
                "Small pinch of sugar if tomatoes are underripe"
            ],
            Steps =
            [
                "Slice tomatoes and mozzarella to similar thickness (~½ cm) so stacks look balanced.",
                "Arrange alternating tomato, mozzarella, basil leaves on a platter in overlapping rows or circles.",
                "Never refrigerate tomatoes before serving — cold kills aroma.",
                "Drizzle olive oil in a thin stream; sprinkle salt from height for even coverage.",
                "If tomatoes taste flat, a pinch of sugar on the cut sides 5 minutes before plating helps.",
                "Add balsamic only at the end and lightly — it should accent, not overpower olive oil.",
                "Grind pepper over top; tear extra basil leaves for garnish.",
                "Serve immediately; caprese does not hold well — make to order."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-chickpea-lemon",
            Title = "Chickpea Cucumber Lemon Salad",
            Subtitle = "Protein-rich, no cooking",
            ImageKey = "recipe_dish_05",
            PrepMinutes = 18,
            ServesNote = "Serves 3–4 as side",
            Ingredients =
            [
                "2 cups cooked chickpeas (drained canned, rinsed well)",
                "1 English cucumber, quartered lengthwise, chopped",
                "1 cup cherry tomatoes, halved",
                "½ red onion, finely diced (rinsed after cutting reduces bite)",
                "3 tbsp extra-virgin olive oil",
                "2–3 tbsp fresh lemon juice",
                "1 tsp ground cumin (optional)",
                "Salt, black pepper",
                "½ cup chopped parsley or mint",
                "2 tbsp toasted pine nuts or sunflower seeds (optional)"
            ],
            Steps =
            [
                "Pat chickpeas dry on a towel — less water means dressing clings better.",
                "If chickpea skins bother you, rub gently in a towel to loosen some — optional texture upgrade.",
                "Combine cucumber, tomato, onion in a bowl; add chickpeas.",
                "Whisk lemon, oil, cumin, salt, pepper; taste — should be tart but rounded.",
                "Pour half dressing over salad; toss and rest 10 minutes for flavors to enter chickpeas.",
                "Add parsley or mint and remaining dressing; toss again.",
                "Top with nuts or seeds last so they stay crunchy.",
                "Keeps refrigerated 2 days; refresh with extra lemon before serving again."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-quinoa-rainbow",
            Title = "Quinoa Rainbow Bowl Salad",
            Subtitle = "Meal-prep friendly grain salad",
            ImageKey = "recipe_dish_11",
            PrepMinutes = 35,
            ServesNote = "Serves 3 as lunch",
            Ingredients =
            [
                "1 cup dry quinoa, rinsed",
                "2 cups water or vegetable stock",
                "1 red bell pepper, small dice",
                "1 cup shredded red cabbage",
                "1 cup grated carrot",
                "½ cup corn kernels (cooked or thawed)",
                "For dressing: 3 tbsp lime juice, 2 tbsp olive oil, 1 tbsp honey, ½ tsp chili flakes, salt",
                "¼ cup chopped cilantro",
                "Optional: ½ avocado cubed (add when serving)"
            ],
            Steps =
            [
                "Rinse quinoa in fine mesh until water runs clear — removes bitterness.",
                "Simmer quinoa in water/stock covered 12–15 minutes until tails show; rest off heat 5 minutes; fluff with fork.",
                "Cool quinoa completely on a tray — warm grains wilt cabbage and make salad soggy.",
                "Prep all vegetables while quinoa cools; keep textures distinct — dice pepper small, shred cabbage fine.",
                "Whisk dressing; adjust heat from chili flakes carefully.",
                "In a large bowl, combine cool quinoa, pepper, cabbage, corn, carrot — toss with two forks.",
                "Add dressing and cilantro; toss thoroughly. Salt if needed — quinoa absorbs salt oddly when warm vs cold.",
                "Refrigerate up to 4 days. Fold avocado in just before eating so it doesn’t brown in bulk."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-kachumber",
            Title = "Kachumber (Indian Relish Salad)",
            Subtitle = "Onion, tomato, cucumber with lemon",
            ImageKey = "recipe_dish_06",
            PrepMinutes = 12,
            ServesNote = "Serves 4 as side to curry",
            Ingredients =
            [
                "2 medium tomatoes, fine dice",
                "1 small cucumber, fine dice",
                "½ small red onion, fine dice (rinse under water)",
                "1 green chili, minced (seeded for mild)",
                "2–3 tbsp lemon or lime juice",
                "½ tsp roasted cumin powder",
                "Salt to taste",
                "2 tbsp chopped coriander leaves",
                "Pinch chaat masala (optional)"
            ],
            Steps =
            [
                "Dice vegetables small and even — kachumber is eaten with roti or dal; large chunks fall off bread.",
                "Combine tomato, cucumber, onion, chili in a bowl.",
                "Add lemon juice, cumin, salt; toss — tomato will release juice quickly.",
                "Rest 5 minutes; taste: should be tangy and fresh, not swimming — drain a tablespoon of liquid if too wet.",
                "Stir in coriander; chaat masala last if using — it volatilizes faster.",
                "Serve cool alongside meals; add more lemon if it sits — acidity fades.",
                "Do not keep more than 8 hours — texture turns mushy.",
                "For meal prep: keep onions separate until serving to reduce harshness."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-sprouted-moong",
            Title = "Sprouted Moong Salad",
            Subtitle = "High fiber, Indian home style",
            ImageKey = "recipe_dish_01",
            PrepMinutes = 15,
            ServesNote = "Serves 2–3 (sprouts need advance prep)",
            Ingredients =
            [
                "2 cups sprouted green moong (lightly steamed or raw if very fresh)",
                "1 small carrot, grated",
                "1 small cucumber, chopped",
                "1 tomato, deseeded, chopped",
                "2 tbsp roasted peanuts, crushed",
                "2 tbsp lemon juice",
                "¼ tsp chaat masala (optional)",
                "Salt, black salt (kala namak) pinch",
                "1 tbsp oil for tempering",
                "½ tsp mustard seeds, 4–6 curry leaves, pinch hing",
                "Fresh coriander"
            ],
            Steps =
            [
                "If sprouts are long, steam 3–5 minutes until tender-crisp (especially for sensitive digestion); cool completely.",
                "Combine sprouts, carrot, cucumber, tomato in bowl.",
                "Add lemon, chaat masala, salts; toss gently — sprouts crush easily.",
                "Heat oil in a small pan; pop mustard, curry leaves, hing — pour over salad immediately (careful: spatter).",
                "Toss once more; top with peanuts and coriander for crunch.",
                "Eat within a few hours for best crunch; refrigerated overnight sprouts soften but stay edible.",
                "If omitting tempering, add extra lemon and peanut for flavor backbone.",
                "Never serve visibly slimy sprouts — discard batch if smell is off."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-pumpkin-feta",
            Title = "Roasted Pumpkin & Spinach Salad",
            Subtitle = "Warm–cold contrast with honey dressing",
            ImageKey = "recipe_dish_08",
            PrepMinutes = 45,
            ServesNote = "Serves 3",
            Ingredients =
            [
                "400g pumpkin or butternut, peeled, 2–3 cm cubes",
                "2 tbsp olive oil, salt, pepper for roasting",
                "4 cups baby spinach, washed dry",
                "80g feta, crumbled",
                "¼ cup pecans or walnuts, toasted",
                "2 tbsp pomegranate seeds (optional)",
                "For dressing: 2 tbsp olive oil, 1 tbsp apple cider vinegar, 1 tsp honey, pinch mustard",
                "2 tsp pumpkin seeds toasted (garnish)"
            ],
            Steps =
            [
                "Heat oven 200°C (400°F). Toss pumpkin cubes with oil, salt, pepper on a baking sheet in one layer.",
                "Roast 25–35 minutes until edges caramelize; flip once halfway for even color.",
                "Cool pumpkin to warm — hot pieces wilt spinach instantly.",
                "Whisk dressing; taste: sweet-tang balance.",
                "In a large bowl, layer spinach, then warm pumpkin (toss gently — heat slightly wilts spinach which is OK).",
                "Add half the feta and nuts; drizzle dressing; toss lightly.",
                "Top with remaining feta, nuts, pomegranate, pumpkin seeds.",
                "Serve immediately. If prepping components ahead, keep pumpkin, spinach, and dressing separate until assembly."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-green-papaya-style",
            Title = "Green Papaya Slaw (Som Tam–inspired)",
            Subtitle = "Vegetarian crunchy slaw — no fish sauce",
            ImageKey = "recipe_dish_12",
            PrepMinutes = 25,
            ServesNote = "Serves 2–3",
            Ingredients =
            [
                "2 cups shredded green papaya OR 3 cups green cabbage + carrot mix",
                "1 carrot, julienned",
                "4–6 green beans, blanched 2 min, chopped (optional)",
                "2 tbsp roasted peanuts, crushed",
                "2 cloves garlic",
                "1–2 bird’s eye chilies (optional)",
                "2 tbsp lime juice",
                "1½ tbsp soy sauce or tamari",
                "1 tsp palm sugar or brown sugar",
                "2 tbsp tamarind pulp dissolved in 2 tbsp water, strained (optional depth)",
                "Cherry tomatoes halved, handful"
            ],
            Steps =
            [
                "If using green papaya: peel, seed, shred with julienne peeler; soak in ice water 10 minutes for crunch, drain well.",
                "Mortar: pound garlic and chili to paste OR mince fine — mortar gives authentic texture.",
                "Dissolve sugar in lime juice; add soy and optional tamarind; taste — should be sour-salty-sweet spike.",
                "In a large bowl, optionally bruise shredded papaya very gently with a pestle (or skip) so dressing clings while keeping crunch.",
                "Toss papaya slaw with dressing using hands; work in tomatoes, beans, half the peanuts.",
                "Taste again — heat builds as it sits; add chili carefully.",
                "Serve on a plate with extra peanuts on top for look and texture.",
                "Best within 4 hours; cabbage version holds 24h better than papaya for meal prep."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-waldorf",
            Title = "Light Waldorf Salad",
            Subtitle = "Apple, celery, walnut — yogurt dressing",
            ImageKey = "recipe_dish_15",
            PrepMinutes = 15,
            ServesNote = "Serves 3",
            Ingredients =
            [
                "2 crisp apples (e.g. Gala), cubed, tossed in 1 tsp lemon",
                "3 celery stalks, sliced on bias",
                "⅓ cup raisins or halved grapes",
                "⅓ cup toasted walnuts, chopped",
                "½ cup plain Greek yogurt",
                "2 tbsp mayonnaise (optional — for classic richness)",
                "1 tsp honey, pinch salt",
                "1 tsp lemon juice in dressing",
                "Butter lettuce cups to serve (optional)"
            ],
            Steps =
            [
                "Cube apples last minute or acidulate with lemon to prevent browning.",
                "Toast walnuts 6–8 minutes at 170°C until fragrant; cool — warm nuts soften yogurt dressing.",
                "Mix yogurt, mayo if using, honey, lemon, salt until smooth.",
                "Combine apple, celery, grapes/raisins in bowl; add half walnuts.",
                "Fold dressing in gently — avoid smashing apples.",
                "Chill 20 minutes if you prefer firmer texture; or serve right away for crunch.",
                "Top with remaining walnut pieces for contrast.",
                "Keeps refrigerated 24 hours; apples may soften slightly."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-pasta-pesto",
            Title = "Summer Pasta Salad with Pesto",
            Subtitle = "Picnic-friendly — serve cool",
            ImageKey = "recipe_dish_13",
            PrepMinutes = 30,
            ServesNote = "Serves 4",
            Ingredients =
            [
                "300g short pasta (fusilli, penne, rotini)",
                "Salt for pasta water",
                "1 cup cherry tomatoes, halved",
                "1 small zucchini, diced small raw or grilled",
                "½ cup fresh mozzarella pearls or diced",
                "⅓ cup basil pesto (store-bought or homemade)",
                "2 tbsp olive oil",
                "1 tbsp lemon juice",
                "Parmesan shavings (optional)",
                "Baby spinach handful (optional)"
            ],
            Steps =
            [
                "Boil pasta in heavily salted water until al dente — one minute shy of package time if chilling after.",
                "Drain; rinse briefly only if you dislike stickiness for cold salad — many chefs drain hot and toss with oil instead.",
                "Spread on tray; cool 10 minutes; drizzle 1 tbsp oil; toss — prevents clumping.",
                "Whisk pesto with remaining oil and lemon for pourable consistency.",
                "In bowl: cooled pasta, tomatoes, zucchini, mozzarella, spinach if using.",
                "Add pesto mix; toss until every curve catches sauce — add pasta water 1 tbsp if tight.",
                "Taste: pesto is salty — salt pasta water should carry baseline seasoning.",
                "Chill 1 hour or serve room temp. Top parmesan at service. Keeps 2 days refrigerated."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-tabbouleh",
            Title = "Tabbouleh (Parsley Bulgur Salad)",
            Subtitle = "Levantine classic — parsley-forward",
            ImageKey = "recipe_dish_03",
            PrepMinutes = 40,
            ServesNote = "Serves 4",
            Ingredients =
            [
                "½ cup fine bulgur wheat",
                "¾ cup boiling water or hot tomato juice",
                "3 bunches flat-leaf parsley, stems min fine tops chopped fine",
                "1 small bunch mint leaves, chopped",
                "4 ripe tomatoes, fine dice, drained",
                "4 green onions, white and green, sliced thin",
                "¼ cup lemon juice (about 2 lemons)",
                "¼ cup olive oil",
                "Salt, black pepper",
                "Optional: pinch cinnamon (some home styles)"
            ],
            Steps =
            [
                "Soak bulgur in boiling water covered 15–20 minutes; fluff; cool — grain should be tender not mushy; drain excess.",
                "Fine chop parsley — tedious but defines tabbouleh; food processor risks puree.",
                "Dice tomatoes; rest in sieve 10 minutes with pinch salt to shed water — wet tabbouleh dilutes flavor.",
                "Combine cooled bulgur, parsley, mint, onion in very large bowl — volume is huge before it settles.",
                "Add tomato last; drizzle lemon and oil; toss with hands for even coating.",
                "Rest 30 minutes refrigerated — herbs relax and grains drink dressing.",
                "Taste: should shout lemon and herb; bulgur supports, not leads.",
                "Eat within 24 hours — parsley dulls after; stir before serving if liquid pools."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-corn-bean",
            Title = "Black Bean & Corn Lime Salad",
            Subtitle = "Southwest flavors — fast",
            ImageKey = "recipe_dish_09",
            PrepMinutes = 15,
            ServesNote = "Serves 4 as side",
            Ingredients =
            [
                "1 can black beans (15 oz), drained rinsed",
                "1½ cups corn kernels",
                "1 red bell pepper, diced",
                "¼ cup red onion, minced",
                "⅓ cup chopped cilantro",
                "3 tbsp lime juice",
                "2 tbsp olive oil",
                "1 tsp ground cumin",
                "½ tsp chili powder or smoked paprika",
                "Salt, pepper",
                "1 avocado (optional, add at end)"
            ],
            Steps =
            [
                "Rinse beans until foam stops — cleaner flavor.",
                "Char corn dry in hot skillet 3–4 minutes for smoky notes or use raw sweet corn if peak season.",
                "Combine beans, corn, pepper, onion, cilantro.",
                "Whisk lime, oil, cumin, chili, salt, pepper — should taste assertively lime-forward.",
                "Pour dressing; toss; rest 15 minutes at room temp or chill 1 hour — deeper flavor cold.",
                "Dice avocado; fold in last minute with gentle toss.",
                "Serve as dip with chips or over greens.",
                "Keeps 2 days without avocado; with avocado, same day best."
            ]
        },
        new SaladRecipe
        {
            Id = "salad-asian-slaw",
            Title = "Asian Cabbage Slaw with Sesame",
            Subtitle = "Crunchy, nutty dressing",
            ImageKey = "recipe_dish_10",
            PrepMinutes = 22,
            ServesNote = "Serves 4",
            Ingredients =
            [
                "½ head green cabbage, shredded fine",
                "1 carrot, julienned or shredded",
                "½ red bell pepper, thin strips",
                "3 green onions, sliced",
                "2 tbsp toasted sesame seeds",
                "For dressing: 2 tbsp toasted sesame oil",
                "2 tbsp rice vinegar",
                "1 tbsp soy sauce",
                "1 tbsp honey or brown sugar",
                "1 tsp grated ginger",
                "1 small garlic clove, grated"
            ],
            Steps =
            [
                "Shred cabbage uniformly — mandoline saves time and improves mouthfeel.",
                "Soak shredded cabbage in ice water 10 minutes, drain very dry — maximizes crunch (optional step).",
                "Whisk all dressing ingredients until honey dissolves — emulsion will separate; shake before pour.",
                "Combine cabbage, carrot, pepper, most of green onion in huge bowl.",
                "Toss with half dressing 5 minutes before eating for best texture — full soak softens cabbage.",
                "Top sesame seeds and remaining onion; extra dressing on table.",
                "Holds 24 hours dressed lightly; fully dressed overnight becomes softer slaw (some people prefer that).",
                "Add shredded chicken or baked tofu for a meal — not required."
            ]
        }
    ];
}
