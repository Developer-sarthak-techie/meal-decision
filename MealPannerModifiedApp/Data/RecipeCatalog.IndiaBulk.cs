using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Data;

/// <summary>
/// Large catalog of India-inspired dish names with keyword-based ingredients and cooking-method steps.
/// Steps follow standard Indian home-kitchen order (prep → bloom spices → cook → finish); they are
/// representative for each dish style, not independently audited for every single title.
/// </summary>
public static partial class RecipeCatalog
{
    public const int IndianBulkPerCategory = 280;

    private static IReadOnlyList<Recipe> BuildIndianBulk(RecipeCategoryKind category)
    {
        var titles = ComposeIndianTitles(category, IndianBulkPerCategory);
        var list = new List<Recipe>(titles.Count);
        for (var i = 0; i < titles.Count; i++)
        {
            var title = titles[i];
            var diet = InferDiet(title);
            var seed = StableHash(title) + i * 31;
            var prep = 12 + Math.Abs(seed % 38);
            var passive = 10 + Math.Abs(seed >> 4) % 52;
            list.Add(new Recipe
            {
                Id = $"recipe-in-{category}-{i:D5}",
                Title = title,
                Category = category,
                ImageKey = $"recipe_dish_{(Math.Abs(StableHash(title)) % 16) + 1:D2}",
                Ingredients = IndianIngredientsFor(category, title, diet),
                Steps = IndianStepsFor(category, title, diet, seed),
                PrepMinutes = prep,
                TotalTimeMinutes = prep + passive,
                DietType = diet
            });
        }

        return list;
    }

    private static int StableHash(string s)
    {
        unchecked
        {
            var h = 19;
            foreach (var c in s)
                h = h * 31 + c;
            return h;
        }
    }

    private static List<string> ComposeIndianTitles(RecipeCategoryKind category, int cap) =>
        category switch
        {
            RecipeCategoryKind.Breakfast => TakeCap(BuildBreakfastTitleSet(), cap),
            RecipeCategoryKind.Lunch => TakeCap(BuildLunchTitleSet(), cap),
            RecipeCategoryKind.Dinner => TakeCap(BuildDinnerTitleSet(), cap),
            _ => TakeCap(BuildSnacksTitleSet(), cap)
        };

    private static List<string> TakeCap(HashSet<string> set, int cap) =>
        set.OrderBy(s => StableHash(s)).Take(cap).ToList();

    private static void Add(HashSet<string> h, string s)
    {
        if (!string.IsNullOrWhiteSpace(s))
            h.Add(s.Trim());
    }

    private static readonly string[] Regions =
    [
        "Hyderabadi", "Lucknowi", "Old Delhi", "Amritsari", "Kolkata", "Goan", "Malabari", "Chettinad",
        "Nizami", "Awadhi", "Bhopali", "Kolhapuri", "Sindhi", "Parsi", "Rajasthani", "Assamese",
        "Odia", "Mangalorean", "Udupi", "Northeastern", "Telangana", "Coastal-Andhra", "Rayalaseema",
        "Malwa", "Konkan", "Banaras", "Mughlai", "Indo-Chinese", "Dhaba", "Highway", "Railway",
        "Homestyle", "Festival", "Monsoon", "Summer", "Winter", "Coastal", "Himalayan", "Deccan"
    ];

    private static HashSet<string> BuildBreakfastTitleSet()
    {
        var h = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var plates =
            new[]
            {
                "Poha Special", "Sabudana Khichdi", "Rava Upma", "Bread Upma", "Vermicelli Upma", "Tomato Bath",
                "Lemon Rice Lite", "Curd Rice Morning", "Chitranna", "Ghee Pongal", "Khara Bath", "Kesari Combo",
                "Rava Idli Meal", "Khichdi Chaas", "Moong Dal Khichdi", "Bajra Roti Jaggery", "Jowar Roti Bhaji",
                "Missi Roti Dahi", "Aloo Paratha", "Gobi Paratha", "Paneer Paratha", "Mooli Paratha", "Methi Paratha",
                "Sattu Paratha", "Keema Paratha", "Egg Paratha", "Chole Bhature", "Poori Aloo", "Halwa Poori",
                "Bedmi Aloo", "Kachori Sabzi", "Matar Kachori", "Pyaz Kachori", "Medu Vada Sambar", "Masala Vada",
                "Rasa Vada", "Dahi Vada", "Sambar Idli", "Podi Idli", "Ghee Podi Idli", "Plain Dosa", "Masala Dosa",
                "Mysore Masala Dosa", "Set Dosa", "Rava Dosa", "Neer Dosa", "Benne Dosa", "Cheese Dosa", "Paneer Dosa",
                "Egg Dosa", "Millet Dosa", "Oats Dosa", "Adai Avial", "Onion Uttapam", "Tomato Uttapam", "Mini Idli",
                "Idiyappam Kurma", "Appam Stew", "Puttu Kadala", "Dhokla", "Khaman", "Handvo", "Patra", "Pesarattu",
                "Akuri Pav", "Masala Omelette Pav", "Boiled Egg Chaat", "Besan Chilla", "Moong Chilla", "Stuffed Chilla",
                "Misal Pav", "Usal Pav", "Bhaji Pav", "Vada Pav", "Sev Khamani", "Litti Chokha", "Jhalmuri Bowl",
                "Aloo Tikki Chana", "Chana Kulcha", "Chole Kulche", "Suji Halwa Puri", "Sheera Poori", "Nihari Paratha",
                "Anda Bhurji Pav", "Keema Pav", "Kothu Parotta", "Roomali Roll", "Momo Soup Bowl", "Thukpa Light",
                "Appam Egg Roast", "Neer Mor Sadam", "Bisi Bele Light", "Tomato Oats Khichdi", "Palak Poha", "Corn Upma"
            };

        foreach (var r in Regions)
            foreach (var p in plates)
                Add(h, $"{r} {p}");

        foreach (var adj in new[] { "Crispy", "Soft", "Spicy", "Mild", "Temple", "Tiffin", "Heritage", "Low-Oil",
                     "Street", "Udupi", "Madurai", "Banaras", "Nashik", "Jaipur" })
            foreach (var b in new[] { "Dosa", "Uttapam", "Idli Meal", "Vada Sambar", "Poori Thali", "Paratha Thali" })
                Add(h, $"{adj} {b}");

        return h;
    }

    private static HashSet<string> BuildLunchTitleSet()
    {
        var h = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var veg =
            new[]
            {
                "Rajma Chawal", "Chole Chawal", "Dal Tadka Jeera Rice", "Dal Makhani Rice", "Palak Paneer Rice",
                "Kadhi Pakora Chawal", "Lemon Rice Thali", "Bagara Rice Sambar", "Ghee Rice Avial", "Curd Rice Thali",
                "Puliyodharai", "Sambar Rice", "Rasam Rice", "Ven Pongal Sambar", "Khichdi Papad", "Vegetable Biryani",
                "Jackfruit Biryani", "Paneer Biryani", "Mushroom Biryani", "Veg Dum Biryani", "Tawa Pulao", "Mint Pulao",
                "Peas Pulao", "Jeera Pulao", "Kashmiri Pulao", "Tomato Rice Thali", "Brinji Rice", "Undhiyu Roti",
                "Gujarati Thali Veg", "Punjabi Veg Thali", "South Veg Meals", "Chettinad Veg Meals", "Andhra Veg Meals",
                "Odia Pakhala Bowl", "Bengali Shukto Meal", "Assamese Jolpan Rice", "Sindhi Kadhi Chawal",
                "Malvani Veg Sukka Rice", "Goan Veg Xacuti Rice", "Railway Veg Cutlet Rice", "Dhaba Panchmel Dal Thali",
                "Highway Aloo Tamatar Chawal", "Baingan Bharta Roti", "Aloo Gobi Roti", "Bhindi Masala Roti",
                "Lauki Kofta Roti", "Malai Kofta Naan", "Navratan Korma Rice", "Kaju Curry Rice", "Chana Masala Meal",
                "Chole Bhature Lunch", "Mixed Veg Handi Rice", "Kadhai Mushroom Rice", "Soya Keema Roti",
                "Veg Kolhapuri Rice", "Paneer Tikka Masala Rice", "Methi Malai Mutter Meal", "Dum Aloo Kashmiri Roti",
                "Stuffed Capsicum Gravy Roti", "Stuffed Tomato Gravy Rice", "Ennai Kathirikai Rice", "Vankaya Pachadi Meals"
            };

        var nv =
            new[]
            {
                "Chicken Curry Rice", "Mutton Curry Rice", "Egg Curry Rice", "Fish Curry Rice", "Prawn Masala Rice",
                "Keema Matar Rice", "Chicken Dum Biryani", "Mutton Dum Biryani", "Fish Biryani", "Prawn Biryani",
                "Andhra Chilli Chicken Meal", "Chettinad Chicken Rice", "Kerala Chicken Stew Appam", "Rogan Josh Rice",
                "Butter Chicken Rice", "Kadai Chicken Thali", "Doi Maach Meal", "Machher Jhol Rice", "Goan Fish Curry Rice",
                "Bombil Fry Meal", "Tandoori Chicken Thali", "Afghani Chicken Bowl", "Hyderabadi Chicken Dum",
                "Lucknowi Chicken Korma Meal", "Kolhapuri Mutton Thali", "Sindhi Sai Bhaji Mutton", "Railway Omelette Curry Rice",
                "Egg Biryani", "Nihari Kulcha", "Paya Nihari", "Chicken 65 Biryani", "Pepper Chicken Meals", "Gongura Mutton Rice",
                "Laal Maas Roti", "Butter Garlic Prawns Rice", "Karimeen Pollichathu Meal", "Meen Moilee Rice",
                "Chicken Chettinad Biryani", "Mutton Keema Pav Bhaji", "Fish Gassi Rice", "Dry Prawn Masala Rice"
            };

        foreach (var r in Regions)
        {
            foreach (var v in veg)
                Add(h, $"{r} {v}");
            foreach (var n in nv)
                Add(h, $"{r} {n}");
        }

        foreach (var street in new[] { "Street", "College Canteen", "Office", "Highway", "Dhaba", "Railway" })
            foreach (var x in new[] { "Egg Rice Box", "Chicken Rice Box", "Veg Thali Quick", "Parotta Salna", "Kothu Parotta" })
                Add(h, $"{street} {x}");

        return h;
    }

    private static HashSet<string> BuildDinnerTitleSet()
    {
        var h = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var veg =
            new[]
            {
                "Dal Makhani Night", "Dal Tadka Night", "Panchmel Dal Baati", "Rajma Masala Roti", "Chole Masala Roti",
                "Palak Paneer Feast", "Paneer Lababdar Roti", "Shahi Paneer Naan", "Matar Paneer Meal", "Kadai Paneer Roti",
                "Malai Kofta Curry", "Lauki Kofta Curry", "Mixed Veg Korma", "Navratan Korma Roti", "Veg Jalfrezi Roti",
                "Baingan Ka Bharta Roti", "Bharwa Bhindi Roti", "Stuffed Karela Masala", "Aloo Dum Kashmiri Roti",
                "Dum Aloo Banarasi", "Chhole Palak Roti", "Sarson Saag Makki", "Undhiyu Puris", "Gujarati Kadhi Khichdi",
                "Undhiyu Roti Night", "Sambar Rice Dinner", "Rasam Rice Dinner", "Avial Rice", "Kaalan Olan Rice",
                "Meals Mor Kuzhambu", "Ennai Kathirikai Kuzhambu", "Vegetable Stew Appam", "Ishtu Appam", "Tomato Garlic Rasam Meal",
                "Vegetable Biryani Night", "Jackfruit Biryani Night", "Paneer Tikka Biryani", "Tawa Pulao Dinner",
                "Jeera Rice Dal Fry", "Bagara Baingan Rice", "Mirchi Ka Salan Biryani Veg", "Veg Hyderabadi Biryani",
                "Veg Lucknowi Biryani", "Subz Handi Naan", "Veg Rara", "Soya Chaap Masala Roti", "Kathal Biryani",
                "Mushroom Pepper Masala Roti", "Gobi Manchurian Fried Rice", "Chilli Paneer Rice", "Baby Corn Manchurian Rice"
            };

        var nv =
            new[]
            {
                "Chicken Butter Masala Naan", "Chicken Tikka Masala Roti", "Mutton Rogan Josh Rice", "Laal Maas Bajra Roti",
                "Chicken Chettinad Roti", "Mutton Chettinad Rice", "Fish Gassi Roti", "Fish Moilee Appam",
                "Prawn Balchao Rice", "Chicken Vindaloo Pao", "Pork Vindaloo Rice", "Mutton Vindaloo Rice",
                "Chicken Xacuti Rice", "Mutton Sukka Roti", "Kori Gassi Roti", "Chicken Korma Naan", "Mutton Korma Naan",
                "Nihari Kulcha Night", "Paya Soup Roti", "Keema Paratha Dinner", "Egg Masala Curry Roti", "Egg Biryani Night",
                "Chicken Dum Biryani Night", "Mutton Dum Biryani Night", "Hyderabadi Mutton Biryani", "Kolkata Mutton Biryani",
                "Ambur Chicken Biryani", "Thalassery Chicken Biryani", "Beary Chicken Biryani", "Sindhi Biryani Chicken",
                "Fish Malabar Curry Rice", "Karimeen Curry Rice", "Butter Garlic Crab", "Tandoori Chicken Platter",
                "Seekh Kebab Roti", "Chicken Tangdi Dinner", "Mutton Rara Roti", "Chicken Pepper Fry Meals",
                "Gongura Chicken Biryani", "Natu Kodi Pulusu Rice", "Kerala Beef Fry Parotta", "Mutton Dum Roti",
                "Yakhni Chicken Pulao", "Mutton Yakhni Pulao", "Fish Pulao", "Prawn Pulao", "Chicken Haleem Bowl"
            };

        foreach (var r in Regions)
        {
            foreach (var v in veg)
                Add(h, $"{r} {v}");
            foreach (var n in nv)
                Add(h, $"{r} {n}");
        }

        foreach (var mood in new[] { "Comfort", "Feast", "Light", "Spicy", "Mild", "One-Pot", "Party", "Rainy Night" })
            foreach (var core in new[] { "Biryani", "Khichdi", "Curry Rice", "Roti Curry", "Pulao", "Handi" })
                Add(h, $"{mood} {core}");

        return h;
    }

    private static HashSet<string> BuildSnacksTitleSet()
    {
        var h = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var items =
            new[]
            {
                "Bhel Puri", "Sev Puri", "Pani Puri Plate", "Dahi Puri", "Ragda Pattice", "Aloo Tikki Chaat",
                "Papdi Chaat", "Chana Chaat", "Aloo Chaat", "Samosa Chaat", "Chole Tikki", "Dahi Bhalla",
                "Raj Kachori", "Pyaaz Kachori", "Mirchi Bajji", "Pakora Platter", "Palak Pakora", "Onion Pakora",
                "Bread Pakora", "Paneer Pakora", "Gobi Pakora", "Moong Dal Vada", "Masala Peanuts", "Roasted Chana",
                "Chana Jor Garam", "Churmura", "Murmura Chikki", "Peanut Chikki", "Sesame Chikki", "Shakkar Para",
                "Namak Para", "Mathri", "Khakra Chips", "Fafda Jalebi", "Handvo Bites", "Khandvi Rolls", "Patra Bites",
                "Dhokla Bites", "Idli Fry", "Chilli Idli", "Baby Corn Manchurian", "Gobi Manchurian", "Paneer Chilli",
                "Chicken Chilli", "Chilli Prawn", "Spring Roll Veg", "Spring Roll Chicken", "Veg Momo", "Chicken Momo",
                "Tandoori Momo", "Steamed Momo", "Kathi Roll Paneer", "Kathi Roll Egg", "Kathi Roll Chicken",
                "Frankie Veg", "Frankie Chicken", "Chicken Lollipop", "Fish Amritsari", "Egg Chilli", "Kebab Plate Veg",
                "Hara Bhara Kebab", "Shami Kebab", "Galouti Kebab", "Seekh Kebab Bites", "Chicken Tikka Bites",
                "Prawn Koliwada", "Bombay Sandwich", "Masala Toast", "Chilli Cheese Toast", "Corn Chaat", "Sprouts Chaat",
                "Fruit Chaat", "Jhal Muri", "Churmur", "Ghugni", "Phuchka", "Aloo Kabli", "Singara", "Misti Doi Pot",
                "Rasgulla Bite", "Gulab Jamun Mini", "Jalebi Rabri Cup", "Shrikhand Shot", "Kulfi Falooda", "Sugar Cane Juice",
                "Masala Shikanji", "Kala Khatta", "Mango Slice Masala", "Roasted Corn", "Bhutta Masala",
                "Banana Chips Packet", "Tapioca Chips", "Jackfruit Chips", "Murukku", "Ribbon Pakoda", "Mixture",
                "Chakli", "Khara Sev", "Boondi Raita Cup", "Dahi Vada Shot", "Pav Bhaji Bites", "Misal Pav Snack"
            };

        foreach (var r in Regions)
            foreach (var it in items)
                Add(h, $"{r} {it}");

        foreach (var adj in new[] { "Extra Spicy", "Sweet", "Tangy", "Chatpata", "Cheesy", "Street Cart", "Evening", "Monsoon" })
            foreach (var s in new[] { "Chaat Bowl", "Pakora Basket", "Roll", "Momos", "Kebab Skewer", "Puri Plate" })
                Add(h, $"{adj} {s}");

        return h;
    }
}
