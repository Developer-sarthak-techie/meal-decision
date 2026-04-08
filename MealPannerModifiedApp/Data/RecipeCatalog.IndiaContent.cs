using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Data;

public static partial class RecipeCatalog
{
    private static string ClassifyIndianCooking(string title)
    {
        var t = title.ToLowerInvariant();
        if (t.Contains("biryani") || t.Contains("pulao") || t.Contains("yakhni")) return "biryani";
        if (t.Contains("khichdi") || t.Contains("pongal") || t.Contains("bisibele")) return "khichdi";
        if (t.Contains("dosa") || t.Contains("uttapam") || t.Contains("adai")) return "dosa";
        if (t.Contains("vada") && !t.Contains("pav")) return "fried";
        if (t.Contains("idli")) return "idli";
        if (t.Contains("paratha") || t.Contains("thepla") || t.Contains("roti") || t.Contains("naan") ||
            t.Contains("kulcha") || t.Contains("poori") || t.Contains("puri"))
            return "bread";
        if (t.Contains("poha") || t.Contains("upma") || t.Contains("aval")) return "poha";
        if (t.Contains("chaat") || t.Contains("bhel") || t.Contains("papdi") || t.Contains("puchka") ||
            t.Contains("phuchka") || t.Contains("ragda"))
            return "chaat";
        if (t.Contains("pakora") || t.Contains("bajji") || t.Contains("bonda") || t.Contains("bhaji") ||
            t.Contains("fry") && (t.Contains("fish") || t.Contains("prawn") || t.Contains("chicken")))
            return "fried";
        if (t.Contains("momos") || t.Contains("momo")) return "momos";
        if (t.Contains("roll") || t.Contains("frankie") || t.Contains("kathi")) return "roll";
        if (t.Contains("kebab") || t.Contains("tikka") || t.Contains("seekh")) return "kebab";
        if (t.Contains("dal ") || t.Contains(" dal") || t.Contains("sambar") || t.Contains("rasam") ||
            t.Contains("kadhi") || t.Contains("kuzhambu"))
            return "dal";
        if (t.Contains("fish") || t.Contains("mach") || t.Contains("maach") || t.Contains("prawn") ||
            t.Contains("jhinga") || t.Contains("karimeen") || t.Contains("bombil") || t.Contains("meen"))
            return "seafood";
        if (t.Contains("chicken") || t.Contains("murgh") || t.Contains("mutton") || t.Contains("gosht") ||
            t.Contains("keema") || t.Contains("nihari") || t.Contains("paya") || t.Contains("lamb") ||
            t.Contains("beef") || t.Contains("pork") || t.Contains("kodi") || t.Contains("natu kodi"))
            return "meat";
        if (t.Contains("egg") || t.Contains("anda") || t.Contains("omelette") || t.Contains("bhurji"))
            return "egg";
        if (t.Contains("dhokla") || t.Contains("khaman") || t.Contains("handvo") || t.Contains("patra"))
            return "steam";
        if (t.Contains("appam") || t.Contains("puttu") || t.Contains("idiyappam")) return "steam";
        if (t.Contains("chilla") || t.Contains("cheela")) return "chilla";
        if (t.Contains("haleem") || t.Contains("khichra")) return "slow";
        if (t.Contains("curry") || t.Contains("korma") || t.Contains("masala") || t.Contains("handi") ||
            t.Contains("xacuti") || t.Contains("vindaloo") || t.Contains("gassi"))
            return "curry";
        return "default";
    }

    private static IReadOnlyList<string> IndianIngredientsFor(RecipeCategoryKind category, string title, MealDietType diet)
    {
        var kind = ClassifyIndianCooking(title);
        var nonVeg = diet == MealDietType.NonVegetarian;

        if (kind == "biryani")
        {
            var protein = nonVeg
                ? "main protein (chicken/mutton/fish/prawn/egg) cut for dum"
                : "vegetables, paneer, soya chaap, or jackfruit & mushrooms";
            return
            [
                "basmati rice (aged, rinsed & soaked 20–30 min)",
                protein,
                "thick yogurt, ginger-garlic paste, green chili paste",
                "fried onions (birista), whole spices (shahi jeera, cardamom, cinnamon, bay, cloves)",
                "biryani or garam masala, red chili & turmeric",
                "ghee or neutral oil, saffron milk / kewra / rose water (optional)",
                "mint & coriander, lemon juice / raw papaya paste for mutton tenderizing (optional)",
                "water or light stock for rice par-boil / absorption method"
            ];
        }

        if (kind == "dosa" || kind == "idli")
        {
            return
            [
                category == RecipeCategoryKind.Breakfast && kind == "dosa"
                    ? "dosa rice & urad dal batter (fermented) or instant rava dosa mix"
                    : "idli rice & urad dal batter (well-fermented) or store batter",
                "salt, water to adjust pour consistency",
                "ghee or oil to smear / crisp edges",
                "potato pody filling (potato, onion, mustard, curry leaves) for masala dosa if needed",
                "sambar vegetables + toor dal, sambar powder, tamarind for sambar",
                "coconut chutney ingredients (coconut, roasted dal, green chili, curry leaves temper)",
                "gunpowder / milagai podi with sesame oil (optional)"
            ];
        }

        if (kind == "bread")
        {
            return
            [
                "whole wheat atta or maida as style demands",
                "stuffing: spiced potatoes / paneer / dal / keema (if non-veg) / sattu mix",
                "ajwain, salt, oil or ghee for dough & brushing",
                "warm water / milk for soft dough",
                "sides: pickle, chutney, curd, chhole, or kadhi",
                "extra flour for dusting while rolling"
            ];
        }

        if (kind == "poha" || kind == "chilla")
        {
            return
            [
                kind == "poha"
                    ? "thick beaten rice rinsed & drained well"
                    : "besan or ground moong dal batter",
                "onion, green chili, curry leaves, ginger",
                "mustard seeds, turmeric, hing (asafoetida), salt",
                "peanuts / cashews for crunch, lemon juice, sugar pinch (typical poha)",
                "fresh coriander, sev (optional for topping)",
                "oil for tempering / shallow fry"
            ];
        }

        if (kind == "chaat")
        {
            return
            [
                "base: sev, murmura, papdi, boiled potato & chickpea as recipe demands",
                "chutneys: tangy tamarind-dates, mint-coriander green, spicy garlic red",
                "chaat masala, roasted cumin powder, black salt",
                "onion, tomato, coriander, nylon sev",
                "thick yogurt & sweet boondi for dahi variants",
                "crisp puris for pani puri / chilled spiced water with jaljeera profile"
            ];
        }

        if (kind == "fried")
        {
            return
            [
                "veg or protein to coat (onion, potato, paneer, fish, chicken pieces)",
                "besan or rice flour coating mix, ajwain, chili, turmeric, salt",
                "cold water or soda for airy batter (optional)",
                "oil for deep-fry, stabilize at 170–185°C for crisp pakora",
                "green chutney & ketchup / tamarind dip",
                "kitchen towel & slotted spoon"
            ];
        }

        if (kind == "momos")
        {
            return
            [
                "maida or half-whole-wheat dough, rested",
                nonVeg
                    ? "minced chicken/mutton with aromatics OR paneer-veg for veg"
                    : "cabbage-carrot-mushroom filling with soy & sesame",
                "ginger-garlic, soy, vinegar, white pepper, scallions",
                "steamer / idli stand & cloth or cabbage leaf lining",
                "chilli-garlic dip & tomato dip"
            ];
        }

        if (kind == "roll")
        {
            return
            [
                "roomali / paratha / tortilla base",
                nonVeg ? "tandoori chicken tikka or egg scramble" : "paneer tikka / spiced sautéed veg",
                "onion rings, lemon, green chutney, ketchup / mayo optional",
                "chaat masala sprinkle, butter for sear",
                "foil wrap for holding shape"
            ];
        }

        if (kind == "kebab")
        {
            return
            [
                nonVeg ? "mince or boneless cuts with fat balance" : "chana dal / spinach / paneer binding",
                "ginger-garlic, garam masala, red chili, kasuri methi",
                "besan or roasted chickpea flour binder, egg optional",
                "mustard oil or ghee for smoke aroma (optional)",
                "skewers, tawa or oven grill, brush with butter while turning"
            ];
        }

        if (kind == "dal")
        {
            return
            [
                "lentils: toor / masoor / moong / urad / chana as dish demands",
                "onion-tomato masala base, ginger-garlic",
                "turmeric, coriander-cumin, red chili, garam masala / sambar powder / rasam powder",
                "tadka oil/ghee with mustard, cumin, dried red chili, curry leaves, hing",
                "tamarind / kokum / lemon for sour balance",
                "ghee finish & fresh coriander"
            ];
        }

        if (kind == "seafood" || kind == "meat" || kind == "egg")
        {
            return
            [
                kind == "seafood" ? "fresh fish / prawns cleaned & patted dry" :
                    kind == "egg" ? "eggs as whole / boiled / bhurji base" :
                    "clean cuts of chicken / mutton; marinate with yogurt & salt",
                "onion-tomato or coconut-onion base depending on region",
                "ginger-garlic, green chili, ground spice blend matching style",
                "thickener: poppy-coconut (coastal) or browned onion (north)",
                "fat: coconut oil / mustard oil / ghee as regional",
                "fresh coriander, curry leaves, lime wedge to finish"
            ];
        }

        if (kind == "steam")
        {
            return
            [
                "besan / dal batters for dhokla-khaman or handvo mix with eno / soda timing",
                "ginger-green chili paste, citric acid / lemon for tang",
                "mustard-curry leaf temper with sesame & coconut (Gujarat style)",
                "oiled tray or dhokla stand, steam 12–18 min till set",
                "grated coconut & coriander garnish"
            ];
        }

        if (kind == "curry" || kind == "slow" || kind == "khichdi")
        {
            return
            [
                category == RecipeCategoryKind.Dinner && kind == "slow"
                    ? "broken wheat / grains & slow-cooked meat or veg per haleem style"
                    : "rice + moong or masoor for khichdi base",
                "whole & ground spices, ginger-garlic",
                "ghee, onions browned where recipe needs depth",
                "liquids: water / stock / coconut milk",
                nonVeg ? "protein cuts with bones for collagen (optional)" : "seasonal vegetables",
                "tadka / temper and fresh herbs to finish"
            ];
        }

        return
        [
            $"fresh produce matching {title}",
            "onion, tomato, ginger-garlic paste",
            "turmeric, coriander, cumin, chili, garam masala to taste",
            "cooking oil or ghee, salt",
            "whole spices for tadka as you prefer",
            nonVeg ? "chosen protein trimmed & marinated briefly" : "paneer / legumes / vegetables as hero",
            "lemon / amchur / yogurt / tamarind for sour balance",
            "fresh coriander / mint / curry leaves for finish"
        ];
    }

    private static IReadOnlyList<string> IndianStepsFor(
        RecipeCategoryKind category,
        string title,
        MealDietType diet,
        int seed)
    {
        var kind = ClassifyIndianCooking(title);
        var nonVeg = diet == MealDietType.NonVegetarian;
        var saltLater = (seed % 2) == 0;

        string[] head =
        [
            $"Mise en place for {title}: measure spices, chop uniformly, keep a tasting spoon ready.",
            nonVeg
                ? "If using meat or fish: pat dry, marinate with salt & acid as needed; keep separate boards from raw veg."
                : "For paneer: soak in warm water 10 min if store-bought firm; drain before Indian gravies.",
            saltLater
                ? "Bloom whole spices in hot fat first; add ground spices only after onions are well cooked."
                : "Toast dry spice powders 30–60 seconds after aromatics to remove raw taste.",
        ];

        string[] tail =
        [
            "Taste: balance salt, heat, sour (lime / tamarind / yogurt), and sweetness if the style uses it.",
            "Finish with ghee drizzle / tadka / fresh herbs so aroma hits before serving.",
            "Rest covered 2–5 minutes where rice or biryani needs settling; fluff gently, not mash.",
            "Cool fully before storing; reheat with splash of water or stock to revive texture."
        ];

        var body = kind switch
        {
            "biryani" => new[]
            {
                "Rinse rice until water runs clearer; soak 20–30 minutes; drain.",
                nonVeg
                    ? "Marinate protein with yogurt, ginger-garlic, chili, turmeric, half the fried onions; rest 30–120 min."
                    : "Blanch or sauté sturdy vegetables; marinate paneer briefly or brown edges for texture.",
                "Par-boil rice with whole spices & salt until 70% cooked (grain still firm); drain.",
                "In handi: layer partial masala, rice, saffron milk, fried onions, mint; repeat; seal edges with dough / foil for dum.",
                "Dum on lowest heat 18–28 minutes; rest off heat 8–10 before opening.",
                "Serve with raita / salan; never stir vigorously once cooked."
            },
            "dosa" => new[]
            {
                "Heat tawa until water droplets dance; wipe with onion half or oiled cloth for non-stick.",
                "Pour batter center-out in thin spiral; drizzle ghee at edges for crisp lace.",
                "Cook undisturbed until surface looks dry and base golden; flip only for uttapam style.",
                "For masala dosa: smear red chutney if using, add potato filling, fold or roll.",
                "Sambar: pressure cook dal; boil tamarind extract with veggies; simmer with sambar powder; temper last."
            },
            "idli" => new[]
            {
                "Grease idli plates lightly; pour fermented batter ¾ full for rise.",
                "Steam 10–14 minutes till tester comes clean; rest 2 minutes before demolding.",
                "If sticky: batter fermentation or greasing needs adjustment next batch.",
                "Serve with sambar & chutneys; re-steam leftovers wrapped in cloth."
            },
            "bread" => new[]
            {
                "Knead soft pliable dough; rest covered 15–30 minutes for gluten relaxation.",
                "Stuff evenly; seal edges; roll gently without bursting (dust lightly).",
                "Cook on hot tawa with ghee presses for puff layers / crisp spots.",
                "Flip when brown spots appear; finish with ghee brush for aroma.",
                "Keep warm in insulated box; serve with sides immediately for best texture."
            },
            "poha" => new[]
            {
                "Rinse thick poha briefly; drain 10–15 minutes so grains stay separate.",
                "Temper mustard, peanuts, curry leaves; sauté onion to translucent with turmeric.",
                "Fold in poha on low; salt, sugar pinch, lemon; toss gently, not mash.",
                "Top with sev, coriander, coconut if desired; serve warm."
            },
            "chilla" => new[]
            {
                "Whisk batter to lump-free pouring consistency; rest 5–10 minutes.",
                "Shallow fry like thin pancake on medium; bubbles mean flip soon.",
                "Layer cheese / paneer / veg filling optional; fold.",
                "Cook both sides golden; pat excess oil; serve with chutney."
            },
            "chaat" => new[]
            {
                "Prepare chutneys ahead; adjust thickness for drizzling vs dipping.",
                "Assemble just before eating: crisp base first, wet ingredients last.",
                "Sprinkle chaat masala & cumin powder between layers for depth.",
                "Serve immediately; textural contrast is the soul of chaat."
            },
            "fried" => new[]
            {
                "Heat oil steadily; test with tiny batter drop—it should rise steadily, not scorch.",
                "Coat in spiced besan batter; shake excess; fry in small batches without crowding.",
                "Drain on rack or towel; season lightly while hot if needed.",
                "Serve with chutney; refry briefly in air oven if soggy (optional)."
            },
            "momos" => new[]
            {
                "Roll thin wrappers; fill without air pockets; pleat tightly.",
                "Oil steamer; steam 8–12 minutes till wrapper looks slightly translucent.",
                "Rest 1 minute; serve with fiery red dip; freeze uncooked on tray then bag if meal-prep.",
                "Pan-fry bottom for pot-sticker style if you want contrast."
            },
            "roll" => new[]
            {
                "Warm base soft; sear filling with high heat for char.",
                "Layer chutneys thin so base doesn’t tear.",
                "Roll tight in foil; toast seam-side for seal.",
                "Slice bias optional; serve immediately."
            },
            "kebab" => new[]
            {
                "Marinate mince with spice paste; rest for binding.",
                "Shape on skewers or as patties; refrigerate 15 minutes if soft.",
                "Cook on griddle / oven with turns; brush ghee for maillard.",
                "Char lightly; baste with butter; serve with onions & lemon."
            },
            "dal" => new[]
            {
                "Pressure cook or boil dal to completely tender; whisk for silkiness if needed.",
                "Separately brown onion-tomato masala with spice powders until fat oozes.",
                "Combine; simmer; adjust water for desired pour vs thick.",
                "Temper smoking hot ghee with whole spices; pour over dal; cover 30 seconds.",
                "Finish with coriander; squeeze lemon if style permits."
            },
            "seafood" => new[]
            {
                "Clean fish / shellfish; score thick fillets; salt briefly.",
                "Build masala: sauté aromatics, add wet ground paste or tomato where coastal.",
                "Slide protein in; simmer gently—overcooking toughens fish and rubberizes prawns.",
                "Coconut milk last if using; boil briefly; finish with curry leaves crackle.",
                "Serve in bowl with rice or appam; drizzle coconut oil if Kerala style."
            },
            "meat" => new[]
            {
                "Brown meat in batches for deeper gravy; don’t crowd the pan.",
                "Yogurt / tomato addition: cook until oil separates for cleaner finish.",
                "Low simmer until collagen breaks—timing varies by cut.",
                "Skim excess fat if heavy; adjust salt after reduction.",
                "Rest poultry curries slightly; mutton benefits from next-day flavor often."
            },
            "egg" => new[]
            {
                "Boil / scramble / fry eggs per sub-style before folding into gravy if needed.",
                "Onion-tomato masala to jammy consistency before liquids.",
                "Add eggs last for boiled variants to avoid rubber.",
                "Garnish with pepper / garam masala dust and coriander.",
                "Great with roti or rice; pack for lunchboxes when dry-ish."
            },
            "khichdi" => new[]
            {
                "Wash rice + dal; optional soak 10–15 minutes.",
                "Temper whole spices; sauté aromatics optional.",
                "Add grains, water ~2.5–3.5x depending on mush preference; pressure or simmer.",
                "Whisk to creamy; top with ghee tadka and pickle side.",
                "Ideal comfort food; hydrate more if reheating."
            },
            "steam" => new[]
            {
                "Rest batter after mixing leaveners; don’t over-beat post eno if using.",
                "Steam on medium so center cooks without collapsing.",
                "Temper mustard seeds, sesame, curry leaves; pour over top.",
                "Cool slightly before cutting diamonds; stays fluffy if not overcooked.",
                "Store refrigerated up to 2 days; steam refresh."
            },
            "slow" => new[]
            {
                "Soak grains / pulses overnight if traditional haleem path.",
                "Low flame constant stir early to prevent scorch as it thickens.",
                "Bone-in meat adds body; skim and adjust spices late.",
                "Finish with birista, mint, lemon, ghee.",
                "Thickens as it cools; loosen with stock when reheating."
            },
            "curry" => new[]
            {
                "Bhuno masala until oil surfaces—shortcut here causes raw taste.",
                "Add protein / veg in stages based on cook time (roots first).",
                "Simmer covered; stir at intervals; use splash hot water if sticking.",
                "Balance cream / nut paste last if North-style richness.",
                "Serve with matching starch: rice, roti, or both."
            },
            _ => new[]
            {
                "Follow regional cues in the name: coastal often means coconut / tamarind; north often means onion-tomato gravies.",
                "Use a wide pan for even reduction; deep pan for liquids.",
                "Layer salt in two stages if ingredients release water.",
                "Taste at three checkpoints: after spices, mid simmer, before finish.",
                "Plate with color contrast: herb, pickle, papad as you like."
            }
        };

        return head.Concat(body).Concat(tail).ToArray();
    }
}
