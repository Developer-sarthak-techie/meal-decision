namespace MealPannerModifiedApp.Data;

/// <summary>Named symptom / mild self-limiting topics — conservative scope only.</summary>
internal static class HomeRemedyBaseConditions
{
    public static IReadOnlyList<string> All { get; } = Build();

    public static string CategoryFor(string baseName)
    {
        var n = baseName.ToLowerInvariant();
        if (n.Contains("sleep") || n.Contains("stress") || n.Contains("anxiety") || n.Contains("insomnia") ||
            n.Contains("jet lag") || n.Contains("fatigue") || n.Contains("restless"))
            return "Sleep & stress";
        if (n.Contains("skin") || n.Contains("sunburn") || n.Contains("itch") || n.Contains("rash") ||
            n.Contains("bite") || n.Contains("bruise") || n.Contains("scrape") || n.Contains("lip") ||
            n.Contains("chap") || n.Contains("blister"))
            return "Skin & minor injury";
        if (n.Contains("eye") || n.Contains("vision strain"))
            return "Eyes";
        if (n.Contains("ear") || n.Contains("tooth") || n.Contains("dental") || n.Contains("gum") || n.Contains("throat") ||
            n.Contains("hoarse") || n.Contains("voice"))
            return "ENT & mouth";
        if (n.Contains("cold") || n.Contains("cough") || n.Contains("nose") || n.Contains("sinus") ||
            n.Contains("congestion") || n.Contains("sneez") || n.Contains("flu-like") || n.Contains("post-nasal"))
            return "Respiratory (mild)";
        if (n.Contains("allergy") || n.Contains("hay fever") || n.Contains("pollen"))
            return "Allergy (mild)";
        if (n.Contains("nausea") || n.Contains("vomit") || n.Contains("diarr") || n.Contains("constip") ||
            n.Contains("bloat") || n.Contains("gas") || n.Contains("indigest") || n.Contains("heartburn") ||
            n.Contains("reflux") || n.Contains("stomach") || n.Contains("bowel") || n.Contains("appetite") ||
            n.Contains("burp") || n.Contains("retch") || n.Contains("hemorrhoid") || n.Contains("rectal"))
            return "Digestive";
        if (n.Contains("heat") || n.Contains("dehydrat") || n.Contains("prickly") || n.Contains("sunstroke"))
            return "Heat & hydration";
        if (n.Contains("motion") || n.Contains("travel") || n.Contains("altitude") || n.Contains("hangover"))
            return "Travel & motion";
        if (n.Contains("headache") || n.Contains("migraine") || n.Contains("cramp") || n.Contains("ache") ||
            n.Contains("pain") || n.Contains("strain") || n.Contains("stiff") || n.Contains("tension") ||
            n.Contains("menstrual") || n.Contains("period"))
            return "Pain & comfort";
        return "General wellness";
    }

    private static IReadOnlyList<string> Build()
    {
        var a = new List<string>(400);
        void AddRange(params string[] items) => a.AddRange(items);

        AddRange(
            "Nausea after eating", "Morning queasiness", "Motion sickness queasiness", "Mild nausea all day",
            "Dry retching no vomit", "One-off vomiting episode", "Stomach feels upside down", "Too rich meal discomfort",
            "Spicy food stomach upset", "Travel tummy rumble", "Loose stools short bout", "Watery diarrhea mild",
            "Stool frequency increased", "Constipation two to three days", "Hard stools straining mild",
            "Bloating after lentils", "Gas and flatulence", "Burping more than usual", "Sour stomach taste",
            "Heartburn after lying down", "Mild acid reflux flare", "Indigestion heavy feeling", "Loss of appetite mild",
            "Stomach ache vague mild", "Cramps before bowel movement", "Post-party stomach regret", "Overeating fullness",
            "Fiber increase adjustment discomfort", "Coffee stomach jitters", "Mild hemorrhoid irritation",
            "Rectal soreness after strain", "Dry mouth after GI bug", "Want bland diet transition",
            "Sensitive stomach day");

        AddRange(
            "Runny nose cold", "Stuffy nose congestion", "Alternating stuffy and runny", "Sneezing fits daytime",
            "Common cold day one", "Common cold day three", "Mild scratchy throat", "Sore throat swallowing",
            "Dry cough irritating", "Chesty cough with phlegm", "Cough worse at night", "Post-nasal drip tickle",
            "Hoarse voice mild", "Voice tired from talking", "Sinus pressure forehead", "Sinus pressure cheeks",
            "Ear fullness with cold", "Tickle in throat cough", "Flu-like achy mild", "Low-grade fever cold",
            "Chills with cold mild", "Chest tight mild with cold", "Throat clearing habit", "Winter sniffles",
            "Season change sniffles", "Dry winter throat", "Humidity stuffy feeling");

        AddRange(
            "Tension headache", "Temple pressure headache", "Dehydration headache", "Eye strain headache",
            "Neck tension ache", "Shoulder blade stiffness", "Upper back ache desk", "Lower back strain mild",
            "Calf cramp night", "Foot arch tired", "Wrist overuse ache", "Knee tweak walking mild",
            "Muscle soreness workout", "Menstrual cramp mild", "Perimenstrual bloating ache", "Ovulation twinge mild",
            "Growing pains style ache child", "Growing pains reassurance topic", "Earache outer mild", "Jaw clench soreness",
            "Tooth sensitivity cold drink", "Gum irritation brushing", "Sinus tooth ache confusion");

        AddRange(
            "Dry itchy skin patch", "Winter dry legs itch", "Minor sunburn pink", "Sunburn shoulders mild",
            "Heat rash prickly", "Insect bite itch", "Mosquito bite welt", "Ant bite sting mild",
            "Minor scrape knee", "Small cut cleaned", "Bruise forming tender", "Chapped lips windy day",
            "Cold sore tingling note care", "Minor friction rash belt line", "Detergent itch mild", "Plant brush itch mild");

        AddRange(
            "Trouble falling asleep", "Wakeful middle of night", "Light sleep fragmented", "Early waking anxiety",
            "Jet lag sleep shift", "Shift worker sleep rough", "Racing thoughts evening", "Mild daytime anxiety spike",
            "Sunday night worry", "Exam week stress body", "Deadline tension shoulders", "Restless legs evening mild",
            "Screen time sleep steal", "Caffeine too late regret", "Nap oversleep groggy", "Daytime fatigue mild");

        AddRange(
            "Eye tired screen day", "Dry eyes air conditioning", "Gritty eye feeling mild", "Pollen eyes water",
            "Contact lens dryness day", "Vision blur temporary strain", "Light sensitivity mild headache");

        AddRange(
            "Ear popping flight", "Ear fullness after swim outer", "Water in ear slosh", "Outer ear itch mild",
            "Throat dryness AC room", "Hoarse teacher voice recovery", "Voice rest need");

        AddRange(
            "Hot day overheated mild", "Heat exhaustion early signs education", "Need rehydration reminder",
            "Sweating electrolyte reminder", "Long hike fluid reminder", "Athlete post-run dehydration mild",
            "Prickly heat baby note", "Summer headache hydration");

        AddRange(
            "Seasonal pollen sniffle", "Hay fever mild day", "Dust mite morning sneeze", "Pet dander sniffle visitor",
            "Mold damp season stuffy", "Allergy eyes itch mild");

        AddRange(
            "Car sickness prone day", "Boat motion uneasy", "Bus windy road nausea", "Altitude uneasy mild",
            "Travel constipation flip", "Jet lag constipation", "Hangover headache mild", "Hangover nausea morning",
            "Too much salt puffiness", "Airplane dry everything");

        AddRange(
            "Feeling off need basics", "Post-viral tired week mild", "Recovery weak appetite", "Need rest reminder",
            "Post-exertional need recovery", "Mind body wind down", "Breathing calm practice", "Gentle recovery day plan",
            "Parents caregiver fatigue note", "Student exam body care", "Office ergonomics ache prevention",
            "Gardening back precaution", "New exercise soreness", "Shoveling snow strain prevention",
            "Holiday overbusy stress", "Family gathering cold exposure", "Smoke haze throat scratch",
            "Firework noise ear care", "Loud concert ring mild", "Swimming chlorine nose", "Pool ear dry tip",
            "Beach sand skin care", "Hiking blister prevention", "Camping GI hygiene", "Festival food caution tummy",
            "Monsoon humidity mold note", "AC vs fan sleep note", "Heater dry nose note", "Winter lip care",
            "Spring pollen surge", "Summer heat wave prep", "Autumn cough lingering mild", "Year-end burnout body",

            "Thirst not drinking enough", "Urine dark concentrate mild", "Dizzy standing fast mild", "Orthostatic dizzy education",
            "Hand tremor after coffee", "Shaky hungry hypoglycemia suspicion education", "Sweet craving stress day",
            "Shoulder knot keyboard", "Thumb scroll soreness", "Pinkie phone hand ache", "Neck pillow wrong height",
            "Stomach growling embarrassment", "Hiccups persistent annoying", "Belching acid taste", "Tight pants bloat",
            "Tight belt reflux trigger", "Late dinner reflux risk", "Chocolate heartburn trigger day",
            "Tomato sauce sensitivity day", "Onion gas day", "Beans gas predictable", "Dairy trial discomfort",
            "Spice level regret mild", "Chili mouth burn cooling", "Wasabi nose rush", "Too much salt thirst",
            "Pickle craving salt", "Ginger tea comfort goal", "Peppermint tea belly calm",
            "Chamomile wind down cup", "Turmeric milk ritual caution meds", "Honey lemon tea throat",
            "Broth soup cold comfort", "Rice kanji recovery", "Toast banana stomach gentle",
            "BRAT style transition", "Fiber ramp slow advice", "Water before meals reminder",
            "Chew more bloating help", "Eat slow swallow air less", "Gum chewing air swallow",

            "Scratchy voice allergy", "Dry cough allergy", "Clear mucus cough", "Yellow mucus watch infection",
            "Green mucus not always bacteria education", "Chest wall cough soreness", "Rib sore from cough",
            "Steam shower congestion", "Saline mist bottle", "Neti pot caution sterile water",
            "Humidifier clean weekly", "Bedroom dust mite cover note", "Pillow wash allergy",
            "Window pollen morning", "Pet hair sofa sniffle", "Construction dust tickle",

            "Ice pack bruise day one", "Warm pack bruise day three", "Arnica caution interact education",
            "Calamine itch classic", "Oatmeal bath itch", "Aloe sunburn cool", "After-sun lotion sting check",
            "Tick bite watch bullseye education", "Bee sting tweezers venom sack", "Wasp sting cold pack",
            "Jellyfish rinse vinegar note region", "Sea urchin spine prompt care", "Coral scrape rinse",
            "Splinter warm soak", "Glass shard urgent care", "Rusty nail tetanus question education",

            "Phone bedtime scroll guilt", "Alarm anxiety Sunday", "Revenge bedtime procrastination",
            "Partner snoring earplug", "Neighbour noise white noise", "Baby monitor hypervigilance tired",
            "Night shift flip flop", "Long flight eastward jet", "Long flight westward jet",
            "Altitude insomnia short trip", "Cabin pressure ear click", "Mask dryness skin note",

            "Screen blue light glasses", "20-20-20 eye rule", "Blink exercise dry office",
            "Contact lens overwear red eye stop", "Swimming goggles eye rub", "Chlorine red eye rinse",
            "Dust blow eye rinse sterile", "Eyelid twitch fatigue", "Stye warm compress education",
            "Stye no squeeze education", "Allergic shiners mild", "Dark circle sleep debt",

            "Wisdom tooth gum flap soreness", "Braces wire poke wax", "Retainer soreness new",
            "Mouth ulcer canker small", "Tongue bite healing", "Burned roof pizza", "Too hot soup palate",
            "Lime acidity tooth zing", "Acid reflux tooth erosion longterm note dentist",
            "Bleeding gums brushing hard", "Floss beginner gum sore", "Electric toothbrush pressure",

            "Muscle knot between shoulders", "Hip flexor tight desk", "IT band walk sore", "Shin splint early walk",
            "Plantar fascia morning step", "Achilles tight hill day", "Hamstring stretch forget",
            "Quad sore stairs", "DOMS day two legs", "DOMS arms new gym", "Neck crack habit caution",
            "Posture slump reminder", "Standing desk tired feet", "High heel foot ache night",

            "PMS mood body day", "PMS craving chocolate", "Cycle back ache mild", "Mid-cycle spotting question education",
            "Hot water bottle period", "Light exercise period day one", "Iron food period fatigue note test",
            "Hydration period headache", "Magnesium question clinician", "Herb tea period comfort",
            "Ovulation mittelschmerz", "Breast tenderness cycle", "Bloating cycle water retention",

            "Vaccine arm soreness day", "Blood test bruise arm", "IV site tender after discharge",
            "Antibiotic stomach upset course", "Probiotic timing question pharmacist", "NSAID stomach caution",
            "Aspirin kids never education", "Reye syndrome awareness education", "Acetaminophen liver alcohol warning",

            "New perfume headache", "Paint fumes window open", "Cleaning spray throat burn",
            "Incense smoke cough", "Campfire smoke eye sting", "Barbecue smoke sinus",
            "Swimming pool red eyes", "Hot tub folliculitis watch", "Sauna dizzy too long",
            "Ice bath shock education", "Cold plunge trend caution", "Heat pad asleep burn risk",

            "Lactation mastitis fever urgent education", "Breastfeeding engorgement comfort", "Blocked duct warm pack education",
            "Newborn sneeze normal education", "Infant hiccups harmless", "Toddler cold watch breathing",
            "School cold exposure week", "Daycare cough month", "Playground scrape knee",
            "Bike fall elbow scrape", "Skateboard wrist bruise", "Soccer shin bump",

            "Older adult dizzy new medication", "Older adult fall fear anxiety", "Polypharmacy dizzy education",
            "Blood pressure cuff anxiety white coat", "Sugar check finger sore rotate sites"
        );

        return a.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }
}
