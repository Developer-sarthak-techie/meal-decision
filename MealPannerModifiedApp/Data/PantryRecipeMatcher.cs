using System.Text;
using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Data;

/// <summary>
/// Matches free-text pantry lists to recipe ingredient lines (substring + synonym groups).
/// </summary>
public static class PantryRecipeMatcher
{
    private static readonly string[][] SynonymGroups =
    [
        ["atta", "wheat", "flour", "whole wheat", "dough"],
        ["jeera", "cumin", "cumin seed"],
        ["haldi", "turmeric"],
        ["besan", "gram flour", "chickpea flour"],
        ["coriander", "dhania", "cilantro"],
        ["hing", "asafoetida"],
        ["amchur", "mango powder"],
        ["moong", "mung", "moong dal"],
        ["toor", "toor dal", "arhar", "pigeon pea"],
        ["masoor", "masoor dal", "red lentil"],
        ["urad", "urad dal", "black gram"],
        ["chana", "chana dal", "chickpea", "chole"],
        ["suji", "sooji", "rava", "semolina"],
        ["poha", "aval", "beaten rice"],
        ["ghee", "butter", "clarified butter"],
        ["chili", "chilli", "mirchi", "red chili"],
        ["onion", "pyaz"],
        ["tomato", "tamatar"],
        ["ginger", "adrak"],
        ["garlic", "lasun"],
        ["salt", "namak"],
        ["oil", "cooking oil", "mustard oil"],
        ["rice", "chawal", "biryani rice"],
        ["paneer", "cottage cheese"],
        ["dahi", "yogurt", "yoghurt", "curd"],
        ["bread", "toast", "slice"],
        ["egg", "anda"],
        ["fish", "mach", "meen"],
        ["chicken", "murgh"],
        ["tamarind", "imli"],
        ["coconut", "nariyal"],
        ["mustard seed", "rai", "sarson"],
        ["fenugreek", "methi"],
        ["curry leaf", "karuvepillai"],
        ["bay leaf", "tej patta"],
        ["cardamom", "elaichi"],
        ["clove", "laung"],
        ["cinnamon", "dalchini"],
        ["pepper", "black pepper", "kali mirch"],
    ];

    public static IReadOnlyList<PantryMatchResult> Match(
        RecipeCategoryKind mealType,
        MealDietPreference diet,
        string pantryRaw,
        int maxResults = 100,
        double minimumMatchFraction = 0.22)
    {
        var pantryTokens = NormalizePantryTokens(pantryRaw);
        if (pantryTokens.Count == 0)
            return [];

        var candidates = RecipeCatalog.GetRecipes(mealType, diet);
        var scored = new List<PantryMatchResult>();

        foreach (var recipe in candidates)
        {
            var lines = recipe.Ingredients;
            if (lines.Count == 0)
                continue;

            var matchedLines = 0;
            var missing = new List<string>();
            foreach (var line in lines)
            {
                if (LineMatchedByPantry(line, pantryTokens))
                    matchedLines++;
                else if (missing.Count < 4)
                    missing.Add(line.Length > 72 ? string.Concat(line.AsSpan(0, 69), "…") : line);
            }

            var fraction = (double)matchedLines / lines.Count;
            if (fraction < minimumMatchFraction)
                continue;

            scored.Add(new PantryMatchResult
            {
                Recipe = recipe,
                MatchPercent = Math.Round(fraction * 100, 1),
                IngredientLinesMatched = matchedLines,
                IngredientLineCount = lines.Count,
                MissingIngredientHints = missing.AsReadOnly()
            });
        }

        return scored
            .OrderByDescending(r => r.MatchPercent)
            .ThenByDescending(r => r.IngredientLinesMatched)
            .ThenBy(r => r.Recipe.Title, StringComparer.OrdinalIgnoreCase)
            .Take(maxResults)
            .ToList();
    }

    private static HashSet<string> NormalizePantryTokens(string raw)
    {
        var tokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var sb = new StringBuilder(raw.Length);
        foreach (var c in raw)
        {
            if (char.IsLetterOrDigit(c) || c is ' ' or '-' or '&')
                sb.Append(char.ToLowerInvariant(c));
            else
                sb.Append(' ');
        }

        foreach (var word in sb.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            var w = word.Trim();
            if (w.Length < 2)
                continue;
            tokens.Add(w);
            ExpandSynonyms(tokens, w);
        }

        return tokens;
    }

    private static void ExpandSynonyms(HashSet<string> tokens, string word)
    {
        foreach (var group in SynonymGroups)
        {
            if (!group.Any(g => g.Equals(word, StringComparison.OrdinalIgnoreCase)))
                continue;
            foreach (var g in group)
                tokens.Add(g);
        }
    }

    private static bool LineMatchedByPantry(string ingredientLine, HashSet<string> pantryTokens)
    {
        var line = ingredientLine.ToLowerInvariant();
        foreach (var t in pantryTokens)
        {
            if (t.Length < 2)
                continue;
            if (line.Contains(t, StringComparison.Ordinal))
                return true;
        }

        return false;
    }
}
