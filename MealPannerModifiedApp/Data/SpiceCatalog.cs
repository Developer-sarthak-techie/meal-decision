using System.Text;
using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Data;

/// <summary>
/// Large educational catalog of spices/herbs (1000+ rows via base names × preparation variants).
/// Content is general wellness-style education — not medical advice, dosing guidance, or treatment.
/// </summary>
public static class SpiceCatalog
{
    private static readonly string[] Variants = { "", " (whole)", " (ground)", " (toasted)" };

    /// <summary>Total entries including variants (≥1000 when spice_names.txt has ≥250 lines).</summary>
    public static int TotalCount => All.Count;

    private static readonly Lazy<IReadOnlyList<SpiceEntry>> Cache = new(Build);
    private static readonly Lazy<IReadOnlyDictionary<string, SpiceEntry>> ById = new(() =>
        Cache.Value.ToDictionary(s => s.Id, StringComparer.OrdinalIgnoreCase));

    public static IReadOnlyList<SpiceEntry> All => Cache.Value;

    public static SpiceEntry? FindById(string id) =>
        ById.Value.TryGetValue(id, out var s) ? s : null;

    public static IReadOnlyList<SpiceEntry> Search(string? query, int maxResults = 400)
    {
        if (string.IsNullOrWhiteSpace(query))
            return All.Take(maxResults).ToList();
        var q = query.Trim();
        var ql = q.ToLowerInvariant();
        var tokens = ql.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var scored = new List<(SpiceEntry s, int score)>();
        foreach (var s in All)
        {
            var text = s.SearchText;
            var score = 0;
            if (text.Contains(ql, StringComparison.Ordinal))
                score += 100;
            foreach (var t in tokens)
            {
                if (t.Length < 2)
                    continue;
                if (text.Contains(t, StringComparison.Ordinal))
                    score += 12;
            }

            if (score > 0)
                scored.Add((s, score));
        }

        return scored
            .OrderByDescending(x => x.score)
            .ThenBy(x => x.s.Name, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.s)
            .Take(maxResults)
            .ToList();
    }

    private static IReadOnlyList<SpiceEntry> Build()
    {
        var asm = typeof(SpiceCatalog).Assembly;
        var stream = asm.GetManifestResourceStream("MealPannerModifiedApp.Data.spice_names.txt")
                     ?? throw new InvalidOperationException(
                         "spice_names.txt not embedded. Expected MealPannerModifiedApp.Data.spice_names.txt. Found: "
                         + string.Join(", ", asm.GetManifestResourceNames()));
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var baseNames = new List<string>();
        while (reader.ReadLine() is { } line)
        {
            var t = line.Trim();
            if (t.Length > 0)
                baseNames.Add(t);
        }

        var distinct = baseNames.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var list = new List<SpiceEntry>(distinct.Count * Variants.Length);
        var n = 0;
        foreach (var baseName in distinct)
        {
            var category = ClassifyCategory(baseName);
            foreach (var suffix in Variants)
            {
                var display = baseName + suffix;
                var seed = HashCode.Combine(display, category, suffix);
                list.Add(new SpiceEntry
                {
                    Id = $"spice-{n++:D6}",
                    Name = display,
                    Category = category,
                    SearchText = BuildSearchText(display, baseName, category),
                    Benefits = PickBenefits(display, seed),
                    HealthUses = PickUses(display, seed + 1),
                    EvidenceNote =
                        "Educational summary only. Strong clinical evidence for specific disease outcomes is rare for culinary spices; ask a clinician or pharmacist if you use medicines, are pregnant, or have chronic conditions — especially for concentrated extracts."
                });
            }
        }

        return list;
    }

    private static string ClassifyCategory(string baseName)
    {
        var l = baseName.ToLowerInvariant();
        if (l.Contains("blend", StringComparison.Ordinal) || l.Contains("masala", StringComparison.Ordinal)
                                                         || l.Contains("mix", StringComparison.Ordinal)
                                                         || l.Contains("spice note", StringComparison.Ordinal))
            return "Blend / mix";
        if (l.Contains("leaf", StringComparison.Ordinal) || l.Contains("basil", StringComparison.Ordinal)
                                                          || l.Contains("mint", StringComparison.Ordinal)
                                                          || l.Contains("thyme", StringComparison.Ordinal)
                                                          || l.Contains("oregano", StringComparison.Ordinal)
                                                          || l.Contains("sage", StringComparison.Ordinal)
                                                          || l.Contains("cilantro", StringComparison.Ordinal)
                                                          || l.Contains("parsley", StringComparison.Ordinal)
                                                          || l.Contains("curry leaf", StringComparison.Ordinal)
                                                          || l.Contains("bay", StringComparison.Ordinal)
                                                          || l.Contains("kaffir", StringComparison.Ordinal)
                                                          || l.Contains("makrut", StringComparison.Ordinal)
                                                          || l.Contains("perilla leaf", StringComparison.Ordinal)
                                                          || l.Contains("shiso", StringComparison.Ordinal))
            return "Leaf herb";
        if (l.Contains("seed", StringComparison.Ordinal) || l.Contains("sesame", StringComparison.Ordinal)
                                                          || l.Contains("nigella", StringComparison.Ordinal)
                                                          || l.Contains("poppy", StringComparison.Ordinal)
                                                          || l.Contains("chia", StringComparison.Ordinal)
                                                          || l.Contains("mustard", StringComparison.Ordinal)
                                                          || l.Contains("lotus seed", StringComparison.Ordinal))
            return "Seed spice";
        if (l.Contains("pepper", StringComparison.Ordinal) || l.Contains("chili", StringComparison.Ordinal)
                                                          || l.Contains("chile", StringComparison.Ordinal)
                                                          || l.Contains("paprika", StringComparison.Ordinal)
                                                          || l.Contains("cayenne", StringComparison.Ordinal)
                                                          || l.Contains("gochugar", StringComparison.Ordinal)
                                                          || l.Contains("urfa", StringComparison.Ordinal)
                                                          || l.Contains("chipotle", StringComparison.Ordinal))
            return "Pepper & heat";
        if (l.Contains("root", StringComparison.Ordinal) || l.Contains("ginger", StringComparison.Ordinal)
                                                          || l.Contains("turmeric", StringComparison.Ordinal)
                                                          || l.Contains("galangal", StringComparison.Ordinal)
                                                          || l.Contains("horseradish", StringComparison.Ordinal)
                                                          || l.Contains("wasabi", StringComparison.Ordinal)
                                                          || l.Contains("licorice", StringComparison.Ordinal))
            return "Root & rhizome";
        if (l.Contains("bark", StringComparison.Ordinal) || l.Contains("cinnamon", StringComparison.Ordinal)
                                                        || l.Contains("cassia", StringComparison.Ordinal)
                                                        || l.Contains("mace", StringComparison.Ordinal))
            return "Bark & floral parts";
        if (l.Contains("berry", StringComparison.Ordinal) || l.Contains("juniper", StringComparison.Ordinal)
                                                          || l.Contains("allspice", StringComparison.Ordinal)
                                                          || l.Contains("schinus", StringComparison.Ordinal))
            return "Berry & fruit spice";
        if (l.Contains("flower", StringComparison.Ordinal) || l.Contains("saffron", StringComparison.Ordinal)
                                                           || l.Contains("elderflower", StringComparison.Ordinal)
                                                           || l.Contains("chamomile", StringComparison.Ordinal)
                                                           || l.Contains("hibiscus", StringComparison.Ordinal)
                                                           || l.Contains("jasmine", StringComparison.Ordinal)
                                                           || l.Contains("clove", StringComparison.Ordinal))
            return "Flower / bud";
        if (l.Contains("salt", StringComparison.Ordinal))
            return "Salt / mineral note";
        return "Culinary spice";
    }

    private static string BuildSearchText(string display, string baseName, string category)
    {
        var sb = new StringBuilder();
        sb.Append(display.ToLowerInvariant()).Append(' ');
        sb.Append(baseName.ToLowerInvariant()).Append(' ');
        sb.Append(category.ToLowerInvariant()).Append(' ');

        // A few searchable cross-language hints (expand as needed)
        var l = baseName.ToLowerInvariant();
        if (l.Contains("turmeric", StringComparison.Ordinal)) sb.Append("haldi curcumin ");
        if (l.Contains("cilantro", StringComparison.Ordinal) || l.Contains("coriander leaf", StringComparison.Ordinal))
            sb.Append("dhania ");
        if (l.Contains("asafoetida", StringComparison.Ordinal)) sb.Append("hing ");
        if (l.Contains("nigella", StringComparison.Ordinal)) sb.Append("kalonji ");
        if (l.Contains("cumin", StringComparison.Ordinal)) sb.Append("jeera ");
        if (l.Contains("fennel", StringComparison.Ordinal)) sb.Append("saunf ");
        if (l.Contains("green cardamom", StringComparison.Ordinal)) sb.Append("elaichi ");
        if (l.Contains("holy basil", StringComparison.Ordinal)) sb.Append("tulsi ");

        sb.Append("digestion antioxidant aromatic cooking culinary wellness");

        return sb.ToString();
    }

    private static IReadOnlyList<string> PickBenefits(string display, int seed)
    {
        var rng = new Random(seed);
        var picks = new List<string>(6);
        var used = new HashSet<int>();
        while (picks.Count < 6 && used.Count < BenefitPool.Length)
        {
            var i = rng.Next(BenefitPool.Length);
            if (!used.Add(i))
                continue;
            picks.Add(BenefitPool[i].Replace("{n}", display, StringComparison.Ordinal));
        }

        return picks;
    }

    private static IReadOnlyList<string> PickUses(string display, int seed)
    {
        var rng = new Random(seed);
        var picks = new List<string>(6);
        var used = new HashSet<int>();
        while (picks.Count < 6 && used.Count < UsePool.Length)
        {
            var i = rng.Next(UsePool.Length);
            if (!used.Add(i))
                continue;
            picks.Add(UsePool[i].Replace("{n}", display, StringComparison.Ordinal));
        }

        return picks;
    }

    private static readonly string[] BenefitPool =
    {
        "{n} adds bold aroma so you can use less salt while still enjoying satisfying flavor.",
        "Plant-forward diets often lean on spices like {n} to keep vegetables, beans, and grains interesting day after day.",
        "Many culinary spices provide small amounts of polyphenols — lab studies are common; real-world benefits depend on diet pattern and dose.",
        "Traditional food cultures pair {n} with slow-cooked or high-fiber meals that support steady energy.",
        "Flavor density from {n} can make smaller portions feel more rewarding — useful when balancing calories mindfully.",
        "Swapping ultraprocessed sauces for whole ingredients plus {n} can reduce added sugar and saturated fat in everyday cooking.",
        "Aromatic compounds in {n} can make low-sodium broths, soups, and stews taste fuller.",
        "When used in cooking rather than supersized supplement doses, {n} is generally approached like food — still discuss concentrates with a clinician.",
        "Colorful spices often sit alongside other plants in Mediterranean, South Asian, and Latin patterns linked with longevity in population research.",
        "Heat-forward spices may encourage slower eating and water with meals — helpful habits for some people.",
        "Mildly pungent spices can brighten bland therapeutic diets when appetite is low (as part of recipes you tolerate).",
        "Layering {n} with acid (citrus, tomato, yogurt) balances plates in ways many cuisines use for both taste and comfort.",
        "Whole spices you toast at home keep volatile oils fresher than pre-ground jars left open for months.",
        "Some people track spices when troubleshooting heartburn or allergies — keep a simple food log if symptoms fluctuate.",
        "Using {n} in bean and lentil dishes is a classic way to improve flavor while increasing fiber and plant protein.",
        "Ground or toasted forms of {n} distribute flavor quickly in marinades; whole forms suit slow infusions.",
    };

    private static readonly string[] UsePool =
    {
        "Bloom {n} briefly in a little oil or ghee, then add vegetables or protein so the fat carries flavor evenly.",
        "Add early in a braise or soup when you want depth; finish with a pinch of fresh herbs if the dish allows.",
        "Rub onto lean proteins with a yogurt or citrus marinade for oven or grill recipes (follow safe cooking temperatures).",
        "Toast {n} in a dry skillet until fragrant — 30–90 seconds — cool, then grind for fresher aroma.",
        "Steep in hot (not boiling) water for a mild liquor, strain; skip medicinal mega-doses unless a professional guides you.",
        "Fold {n} into whole-grain dough or batter for savory baking; keep amounts modest so other flavors still shine.",
        "Stir into plain yogurt or skyr for a quick sauce or dip base with vegetables on the side.",
        "Combine {n} with garlic, ginger, or onion powders sparingly to build aroma without excess sodium.",
        "Pair with legume or tofu dishes to complement earthy notes and reduce reliance on bottled sauces.",
        "Mix into roasted nuts or seeds for crunchy salads; watch oven temperature so small seeds do not scorch.",
        "Layer {n} into grain bowls with pickled vegetables for probiotic-friendly, high-fiber meals.",
        "Whisk into vinaigrettes with mustard or honey emulsions for stable, flavorful dressings.",
        "Stir into simmered tomato sauces toward the end to preserve volatile top notes when possible.",
        "Blend {n} with stock and starchy vegetables for pureed soups that feel creamy with less dairy.",
        "Sprinkle over cut fruit with lime in cultures that enjoy sweet–spicy contrast; adjust for your comfort.",
        "Use {n} in homemade spice kits for travel or camping so you rely less on single-use packets.",
    };
}
