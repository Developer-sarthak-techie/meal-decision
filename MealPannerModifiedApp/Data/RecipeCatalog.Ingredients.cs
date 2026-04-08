using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Data;

public static partial class RecipeCatalog
{
    private static IReadOnlyList<string> IngredientsFor(RecipeCategoryKind category, string baseName)
    {
        var map = category switch
        {
            RecipeCategoryKind.Breakfast => BreakfastIngredients,
            RecipeCategoryKind.Lunch => LunchIngredients,
            RecipeCategoryKind.Dinner => DinnerIngredients,
            _ => SnacksIngredients
        };
        if (map.TryGetValue(baseName, out var list))
            return list;

        return
        [
            $"fresh produce & staples for {baseName}",
            "cooking oil or ghee",
            "salt",
            "warm spices as you prefer",
            "water or stock as needed",
            "lemon or herbs to finish"
        ];
    }

    private static readonly Dictionary<string, string[]> BreakfastIngredients = new(StringComparer.Ordinal)
    {
        ["Masala Oats Bowl"] =
        [
            "rolled oats (½–¾ cup)",
            "milk or water",
            "small onion & tomato (optional)",
            "turmeric, red chili powder, salt",
            "ghee or neutral oil",
            "green chilies & fresh coriander"
        ],
        ["Vegetable Upma"] =
        [
            "rava / sooji (1 cup)",
            "mixed vegetables (peas, carrot, beans)",
            "mustard seeds, urad dal, chana dal",
            "curry leaves, ginger, green chili",
            "ghee or oil",
            "lemon juice & fresh coriander"
        ],
        ["Avalakki Poha"] =
        [
            "thick beaten rice (poha), rinsed & drained",
            "peanuts, mustard seeds, curry leaves",
            "onion, turmeric, green chili",
            "sugar pinch + salt",
            "lemon juice",
            "fresh coriander & grated coconut (optional)"
        ],
        ["Stuffed Aloo Paratha"] =
        [
            "whole wheat atta for dough",
            "boiled potatoes (mashed)",
            "green chili, ginger, coriander",
            "amchur, jeera, coriander powder, salt",
            "oil or ghee for rolling & pan-frying",
            "warm water for kneading"
        ],
        ["Idli with Sambar"] =
        [
            "idli rice & urad dal (or readymade idli batter)",
            "toor dal for sambar",
            "sambar vegetables (drumstick, onion, tomato)",
            "sambar powder + tamarind pulp",
            "mustard seeds, curry leaves (tempering)",
            "oil + fresh coriander"
        ],
        ["Moong Dal Chilla"] =
        [
            "split yellow moong dal (soaked & ground)",
            "ginger, green chili, coriander",
            "hing, turmeric, salt",
            "finely chopped onion (optional)",
            "oil for shallow-frying",
            "water to adjust batter consistency"
        ],
        ["Besan Cheela"] =
        [
            "gram flour (besan)",
            "onion, tomato, coriander",
            "ajwain, turmeric, red chili powder",
            "water for thin batter",
            "oil to cook",
            "salt"
        ],
        ["Ragi Dosa Stack"] =
        [
            "ragi (finger millet) flour",
            "rice flour or leftover dosa batter (optional, for crispness)",
            "buttermilk or water",
            "onion, green chili, curry leaves",
            "salt",
            "oil to smear on pan"
        ],
        ["Fruit Yogurt Parfait"] =
        [
            "thick yogurt (Greek or hung curd)",
            "seasonal fresh fruit (banana, berries, mango)",
            "honey or maple syrup",
            "homemade or store granola",
            "mixed nuts (almond, walnut) optional",
            "pinch of cinnamon (optional)"
        ],
        ["Masala Egg Bhurji"] =
        [
            "eggs (3–4)",
            "onion, tomato, green chili",
            "turmeric, red chili powder, garam masala",
            "butter or oil",
            "salt, black pepper",
            "fresh coriander"
        ],
        ["Paneer Veg Sandwich"] =
        [
            "bread slices (whole wheat or white)",
            "paneer (sliced or crumbled)",
            "cucumber & tomato slices",
            "butter or mayo (optional)",
            "mint-coriander chutney or tomato ketchup",
            "chaat masala, black salt pinch"
        ],
        ["Cinnamon French Toast"] =
        [
            "thick bread slices (stale works best)",
            "eggs",
            "milk or cream",
            "sugar, cinnamon powder",
            "vanilla extract (optional)",
            "butter for frying"
        ],
        ["Granola Honey Bowl"] =
        [
            "rolled oats",
            "mixed nuts & seeds (almond, pumpkin)",
            "honey or brown sugar",
            "light oil or coconut oil",
            "dried cranberries or raisins",
            "thick yogurt to serve"
        ]
    };

    private static readonly Dictionary<string, string[]> LunchIngredients = new(StringComparer.Ordinal)
    {
        ["Rajma Chawal Bowl"] =
        [
            "kidney beans (rajma), soaked overnight",
            "basmati rice",
            "onion–tomato masala base",
            "ginger-garlic paste, rajma masala powder",
            "ghee for finishing",
            "fresh coriander"
        ],
        ["Yellow Dal Tadka Meal"] =
        [
            "toor / arhar dal",
            "turmeric, hing, salt",
            "onion, tomato, green chili",
            "ginger-garlic, jeera, red chili",
            "ghee for tempering with mustard & curry leaves",
            "steamed rice"
        ],
        ["Lemon Rice Thali"] =
        [
            "cooked & cooled rice",
            "fresh lemon juice",
            "mustard seeds, urad dal, chana dal",
            "peanuts or cashews, curry leaves",
            "turmeric, hing, green chili",
            "ghee & fresh coriander"
        ],
        ["Chickpea Curry Plate"] =
        [
            "kabuli chana (chickpeas), soaked & boiled",
            "onion, tomato gravy",
            "ginger-garlic, chole/chana masala",
            "tea bag or amchur (color & tang)",
            "kasuri methi, butter optional",
            "roti, bhatura, or rice"
        ],
        ["Veg Biryani Lite"] =
        [
            "basmati rice (soaked)",
            "mixed vegetables (carrot, beans, cauliflower)",
            "biryani masala, yogurt (veg)",
            "fried onions, mint & coriander",
            "ghee, whole spices (cardamom, bay)",
            "saffron milk (optional)"
        ],
        ["Palak Paneer Combo"] =
        [
            "fresh spinach (blanched & pureed)",
            "paneer cubes",
            "cream or cashew paste",
            "onion, ginger-garlic, green chili",
            "garam masala, kasuri methi",
            "butter or oil, roti or rice"
        ],
        ["Chole Kulcha Plate"] =
        [
            "chickpeas boiled",
            "onion-tomato, ginger-garlic",
            "chole masala, amchur, anardana (optional)",
            "tea bag or dry amla for dark color",
            "kulcha, naan, or rice",
            "pickle & onion rings"
        ],
        ["Mixed Veg Khichdi"] =
        [
            "rice & moong dal (equal or 2:1)",
            "mixed vegetables",
            "turmeric, hing, cumin",
            "ghee tempering with jeera",
            "salt",
            "papad, pickle, or ghee on top"
        ],
        ["Caprese Pesto Pasta"] =
        [
            "pasta (penne or fusilli)",
            "fresh mozzarella balls or cubed",
            "ripe tomatoes or cherry tomatoes",
            "basil pesto (fresh basil, pine nuts, parmesan, olive oil)",
            "olive oil, salt, black pepper",
            "fresh basil leaves"
        ],
        ["Tandoori Prawn Quinoa"] =
        [
            "large prawns, cleaned & deveined",
            "thick hung yogurt",
            "tandoori masala or Kashmiri chili, lemon",
            "ginger-garlic paste",
            "cooked quinoa",
            "onion rings & mint for salad"
        ],
        ["Thai Basil Chicken Rice"] =
        [
            "boneless chicken thigh (cubed)",
            "jasmine or steamed rice",
            "Thai holy basil or Italian basil",
            "soy sauce, oyster sauce or fish sauce",
            "garlic, bird’s eye chilies",
            "veg oil, sugar pinch"
        ],
        ["Grilled Fish Wrap Bowl"] =
        [
            "white firm fish fillet",
            "yogurt, ginger-garlic, lemon",
            "chili powder, turmeric, garam masala",
            "whole-wheat tortillas or roti",
            "cabbage slaw & onion",
            "mint chutney or yogurt sauce"
        ],
        ["Teriyaki Chicken Don"] =
        [
            "chicken thigh (boneless, bite-sized)",
            "short-grain or sushi rice",
            "soy sauce, mirin or sugar, sake optional",
            "ginger, garlic",
            "toasted sesame seeds",
            "spring onion tops"
        ]
    };

    private static readonly Dictionary<string, string[]> DinnerIngredients = new(StringComparer.Ordinal)
    {
        ["Dal Makhani Dinner"] =
        [
            "whole black urad + kidney beans (rajma), soaked",
            "butter, cream",
            "tomato puree, ginger-garlic",
            "dal makhani masala, garam masala",
            "kasuri methi",
            "water or vegetable stock for simmering"
        ],
        ["Baingan Bharta Night"] =
        [
            "large brinjal (roasted until charred)",
            "onion, tomato, green chili",
            "mustard oil or regular oil",
            "jeera, turmeric, coriander powder",
            "salt",
            "fresh coriander"
        ],
        ["Stuffed Bell Peppers"] =
        [
            "large bell peppers (tops removed)",
            "potato-paneer or quinoa stuffing",
            "onion, peas, corn (optional)",
            "garam masala, turmeric",
            "tomato sauce or light gravy (optional)",
            "cheese topping optional"
        ],
        ["Malai Kofta Curry"] =
        [
            "paneer & potato (grated for kofta)",
            "cashew–poppy or cashew–melon seed paste",
            "cream, tomato, ginger-garlic",
            "garam masala, sugar pinch",
            "cornstarch or maida to bind kofta",
            "oil to shallow-fry kofta"
        ],
        ["Coconut Veg Stew"] =
        [
            "thick & thin coconut milk",
            "mixed vegetables (carrot, potato, beans)",
            "curry leaves, mustard seeds",
            "green chili, ginger",
            "coconut oil",
            "steamed appam or rice (optional)"
        ],
        ["Kadhai Paneer Feast"] =
        [
            "paneer cubes",
            "capsicum, onion (thick slices)",
            "onion-tomato kadhai gravy",
            "kadhai masala, kasuri methi",
            "cream (optional)",
            "ginger julienne, coriander"
        ],
        ["Fish Malabari Curry Bowl"] =
        [
            "firm fish steaks / fillets",
            "coconut milk",
            "kudampuli / tamarind",
            "curry leaves, mustard seeds, fenugreek seeds (pinch)",
            "onion, tomato, green chili",
            "coriander, turmeric, Kashmiri chili"
        ],
        ["Mushroom Stroganoff"] =
        [
            "button or cremini mushrooms (sliced)",
            "onion, garlic",
            "sour cream or cooking cream",
            "paprika, black pepper",
            "butter, white wine optional",
            "wide pasta noodles or rice"
        ],
        ["Chicken Kofta Taco Night"] =
        [
            "chicken mince",
            "breadcrumbs & egg to bind (optional)",
            "taco spice mix, onion, garlic",
            "hard taco shells or soft tortillas",
            "iceberg or cabbage, salsa & cheese",
            "yogurt-lime sauce"
        ],
        ["Jackfruit Taco Night"] =
        [
            "young green jackfruit (canned or fresh)",
            "onion, garlic, taco seasoning",
            "smoked paprika, cumin",
            "corn tortillas",
            "lime, avocado or guacamole",
            "red cabbage slaw"
        ],
        ["Chicken Lasagna Bake"] =
        [
            "lasagna sheets",
            "chicken mince ragu (tomato, onion, herbs)",
            "béchamel (butter, flour, milk)",
            "mozzarella & parmesan",
            "garlic, oregano, basil",
            "olive oil"
        ],
        ["Soya Chunk Curry Bowl"] =
        [
            "soya chunks (rehydrated, squeezed)",
            "onion-tomato gravy",
            "ginger-garlic, garam masala",
            "cream optional",
            "kasuri methi",
            "steamed rice or roti"
        ],
        ["Vegetable Korma Plate"] =
        [
            "mixed vegetables (cauliflower, carrot, beans)",
            "yogurt & cashew–poppy paste (white korma)",
            "whole spices (bay, cardamom, clove)",
            "ginger-garlic paste",
            "cream or coconut milk",
            "naan, roti, or jeera rice"
        ]
    };

    private static readonly Dictionary<string, string[]> SnacksIngredients = new(StringComparer.Ordinal)
    {
        ["Masala Murmura"] =
        [
            "plain puffed rice (murmura)",
            "peanuts, curry leaves",
            "turmeric, chili powder, salt",
            "mustard seeds, dry red chili",
            "oil for tempering",
            "optional sev or fried dal on top"
        ],
        ["Roasted Makhana"] =
        [
            "fox nuts (makhana)",
            "ghee or olive oil",
            "rock salt",
            "black pepper, chaat masala (optional)",
            "roast until crisp",
            "store airtight"
        ],
        ["Sprouts Chaat Cup"] =
        [
            "mixed sprouts (moong, chana) boiled/steamed",
            "onion, tomato, green chili",
            "lemon juice, chaat masala",
            "green chutney & tamarind chutney",
            "sev & coriander",
            "optional pomegranate"
        ],
        ["Baked Samosa Bites"] =
        [
            "puff pastry or samosa patti / shortcrust",
            "boiled potato & peas filling",
            "amchur, coriander powder, garam masala",
            "green chili, ginger",
            "oil brush for baking",
            "salt"
        ],
        ["Hummus Veg Sticks"] =
        [
            "cooked chickpeas (canned ok)",
            "tahini",
            "lemon juice, garlic",
            "olive oil, salt",
            "cumin powder",
            "carrot & cucumber batons"
        ],
        ["Peanut Chikki Squares"] =
        [
            "raw peanuts (roasted)",
            "jaggery or sugar",
            "ghee pinch",
            "flat tray greased",
            "cardamom optional",
            "cool & cut"
        ],
        ["Fruit Chaat Bowl"] =
        [
            "mixed ripe fruit (apple, banana, papaya)",
            "black salt, roasted cumin powder",
            "chaat masala, lemon juice",
            "mint & coriander optional",
            "chili powder pinch optional",
            "chilled before serving"
        ],
        ["Dry Dhokla Squares"] =
        [
            "gram flour (besan)",
            "yogurt, water",
            "eno or fruit salt (raising)",
            "mustard seeds, curry leaves tempering",
            "sugar-salt in batter",
            "grated coconut & coriander top"
        ],
        ["Moong Salad Jar"] =
        [
            "sprouted moong (steamed)",
            "cucumber, tomato, onion",
            "lemon juice, black pepper",
            "olive oil optional",
            "salt",
            "fresh coriander"
        ],
        ["Bhel Puri Cup"] =
        [
            "puffed rice",
            "sev, crushed papdi optional",
            "boiled potato, onion, tomato",
            "tamarind & green chutney",
            "chaat masala, coriander",
            "squeeze of lime"
        ],
        ["Egg Chilli Cheese Toast"] =
        [
            "bread slices",
            "eggs (beaten)",
            "grated cheese (cheddar or mozzarella)",
            "green chili, onion (fine)",
            "butter",
            "salt, black pepper"
        ],
        ["Yogurt Berry Pot"] =
        [
            "thick chilled yogurt",
            "mixed berries or pomegranate",
            "honey or maple syrup",
            "granola layer optional",
            "nuts & seeds",
            "vanilla extract optional"
        ],
        ["Chicken Seekh Kebab Bites"] =
        [
            "chicken mince (lean)",
            "ginger-garlic paste",
            "seekh kebab masala, garam masala",
            "besan or bread tie (optional)",
            "onion & green chili (fine)",
            "skewers or shape as patties, oil to brush"
        ]
    };
}
