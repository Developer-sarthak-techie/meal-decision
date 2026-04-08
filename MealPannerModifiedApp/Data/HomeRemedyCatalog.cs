using MealPannerModifiedApp.Models;

namespace MealPannerModifiedApp.Data;

/// <summary>
/// Large catalog of conservative self-care summaries. Content follows patterns from WHO, NIH MedlinePlus,
/// and similar patient-education pages for mild, short-term issues — it is not copied verbatim and must
/// not replace professional care. See in-app disclaimers.
/// </summary>
public static class HomeRemedyCatalog
{
    public static int TotalRemedyCount => All.Count;

    private static readonly Lazy<IReadOnlyList<HomeRemedy>> Cache = new(Build);
    private static readonly Lazy<IReadOnlyDictionary<string, HomeRemedy>> ById = new(() =>
        Cache.Value.ToDictionary(r => r.Id, StringComparer.OrdinalIgnoreCase));

    public static IReadOnlyList<HomeRemedy> All => Cache.Value;

    public static HomeRemedy? FindById(string id) =>
        ById.Value.TryGetValue(id, out var r) ? r : null;

    public static IReadOnlyList<HomeRemedy> Search(string? query, int maxResults = 200)
    {
        if (string.IsNullOrWhiteSpace(query))
            return All.Take(maxResults).ToList();
        var q = query.Trim();
        var ql = q.ToLowerInvariant();
        var tokens = ql.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var scored = new List<(HomeRemedy r, int score)>();
        foreach (var r in All)
        {
            var s = r.SearchText;
            var score = 0;
            if (s.Contains(ql, StringComparison.Ordinal))
                score += 100;
            foreach (var t in tokens)
            {
                if (t.Length < 2)
                    continue;
                if (s.Contains(t, StringComparison.Ordinal))
                    score += 15;
            }

            if (score > 0)
                scored.Add((r, score));
        }

        return scored
            .OrderByDescending(x => x.score)
            .ThenBy(x => x.r.Title, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.r)
            .Take(maxResults)
            .ToList();
    }

    private static IReadOnlyList<HomeRemedy> Build()
    {
        var audiences = new (string Key, string Label)[]
        {
            ("adults", "Adults"),
            ("older", "Older adults"),
            ("children", "Children — caregiver reads first")
        };

        var bases = HomeRemedyBaseConditions.All;
        var list = new List<HomeRemedy>(bases.Count * audiences.Length);
        var n = 0;
        foreach (var baseName in bases)
        {
            var cat = HomeRemedyBaseConditions.CategoryFor(baseName);
            var tmpl = ClassifyTemplate(baseName);
            foreach (var (audKey, audLabel) in audiences)
            {
                var id = $"remedy-{n:D5}";
                var title = $"{baseName} ({audLabel})";
                var search = BuildSearchText(baseName, cat, audKey);
                var body = RemedyBodyBuilder.Build(baseName, cat, tmpl, audKey);
                list.Add(new HomeRemedy
                {
                    Id = id,
                    Title = title,
                    AudienceLabel = audLabel,
                    Category = cat,
                    SearchText = search,
                    Overview = body.Overview,
                    SelfCare = body.SelfCare,
                    HydrationAndFood = body.Hydration,
                    ComfortMeasures = body.Comfort,
                    WhatToAvoid = body.Avoid,
                    SeeDoctorPromptly = body.RedFlags,
                    SourceAlignmentNote = body.SourceNote
                });
                n++;
            }
        }

        return list;
    }

    private static string BuildSearchText(string baseName, string category, string audience)
    {
        var parts = new List<string>
        {
            baseName.ToLowerInvariant(),
            category.ToLowerInvariant(),
            audience.ToLowerInvariant()
        };
        foreach (var syn in Synonyms.Expand(baseName))
            parts.Add(syn);
        return string.Join(" • ", parts.Distinct(StringComparer.OrdinalIgnoreCase));
    }

    private static RemedyTemplateKind ClassifyTemplate(string name)
    {
        var n = name.ToLowerInvariant();
        if (n.Contains("cold") || n.Contains("cough") || n.Contains("throat") || n.Contains("nose") ||
            n.Contains("sinus") || n.Contains("congestion") || n.Contains("sneez") || n.Contains("hoarse") ||
            n.Contains("voice") || n.Contains("flu-like") || n.Contains("post-nasal"))
            return RemedyTemplateKind.Respiratory;
        if (n.Contains("nausea") || n.Contains("vomit") || n.Contains("diarr") || n.Contains("constip") ||
            n.Contains("bloat") || n.Contains("gas") || n.Contains("indigest") || n.Contains("heartburn") ||
            n.Contains("reflux") || n.Contains("stomach") || n.Contains("bowel") || n.Contains("appetite") ||
            n.Contains("burp") || n.Contains("retch") || n.Contains("hemorrhoid") || n.Contains("rectal"))
            return RemedyTemplateKind.Digestive;
        if (n.Contains("headache") || n.Contains("migraine") || n.Contains("cramp") || n.Contains("ache") ||
            n.Contains("pain") || n.Contains("strain") || n.Contains("stiff") || n.Contains("tension") ||
            n.Contains("menstrual") || n.Contains("period"))
            return RemedyTemplateKind.Pain;
        if (n.Contains("skin") || n.Contains("sunburn") || n.Contains("itch") || n.Contains("bite") ||
            n.Contains("rash") || n.Contains("bruise") || n.Contains("scrape") || n.Contains("lip") ||
            n.Contains("burn") && !n.Contains("heartburn"))
            return RemedyTemplateKind.Skin;
        if (n.Contains("sleep") || n.Contains("insomnia") || n.Contains("stress") || n.Contains("anxiety") ||
            n.Contains("jet lag") || n.Contains("fatigue") || n.Contains("restless"))
            return RemedyTemplateKind.SleepStress;
        if (n.Contains("ear") || n.Contains("tooth") || n.Contains("dental") || n.Contains("gum"))
            return RemedyTemplateKind.Ent;
        if (n.Contains("eye") || n.Contains("vision strain"))
            return RemedyTemplateKind.Eye;
        if (n.Contains("heat") || n.Contains("dehydrat") || n.Contains("prickly") || n.Contains("sun") && n.Contains("stroke"))
            return RemedyTemplateKind.HeatFluid;
        if (n.Contains("allergy") || n.Contains("hay fever") || n.Contains("pollen") || n.Contains("sneeze"))
            return RemedyTemplateKind.Allergy;
        if (n.Contains("motion") || n.Contains("travel") || n.Contains("altitude") || n.Contains("hangover"))
            return RemedyTemplateKind.TravelMotion;
        return RemedyTemplateKind.General;
    }

    private enum RemedyTemplateKind
    {
        Respiratory,
        Digestive,
        Pain,
        Skin,
        SleepStress,
        Ent,
        Eye,
        HeatFluid,
        Allergy,
        TravelMotion,
        General
    }

    private static class RemedyBodyBuilder
    {
        internal static Body Build(string baseName, string category, RemedyTemplateKind tmpl, string audKey)
        {
            var isChild = audKey == "children";
            var isOlder = audKey == "older";
            var overview = TemplateOverview(baseName, tmpl, isChild);
            var selfCare = TemplateSelfCare(tmpl, isChild, isOlder);
            var hydration = TemplateHydration(tmpl, isChild);
            var comfort = TemplateComfort(tmpl);
            var avoid = TemplateAvoid(tmpl, isChild);
            var red = TemplateRedFlags(tmpl, isChild, isOlder, baseName);
            var source =
                "Educational patterns follow public patient guides (e.g., WHO home-care basics, NIH MedlinePlus, NHS conditions A–Z) for mild, short-lived issues. This text is not a quote from those sites; confirm with official pages and your clinician.";
            return new Body(overview, selfCare, hydration, comfort, avoid, red, source);
        }

        private static IReadOnlyList<string> TemplateOverview(string title, RemedyTemplateKind k, bool child)
        {
            var who =
                "These suggestions describe common, low-risk comfort measures for short-lived symptoms. They do not replace diagnosis or treatment from a qualified health professional.";
            if (child)
                return
                [
                    who,
                    $"Caregiver focus: {title}. Young children can worsen quickly — watch hydration, breathing, and responsiveness; seek urgent care for any red-flag signs listed below.",
                    "When in doubt, call your pediatrician or local nurse helpline the same day."
                ];
            return
            [
                who,
                $"Topic: {title}. Duration matters: if symptoms are new today, self-care may be reasonable; if they persist beyond what is typical for a common cold or upset stomach, get medical advice.",
                "Medicines (including OTC), supplements, and doses are not specified here on purpose — ask a pharmacist or clinician what is safe for you and your other conditions."
            ];
        }

        private static IReadOnlyList<string> TemplateSelfCare(RemedyTemplateKind k, bool child, bool older)
        {
            var rest = "Rest in a calm environment; avoid strenuous activity until you feel clearly better.";
            var track =
                "Note start time, temperature (if febrile), fluid intake, and symptom changes — useful if you need telehealth or urgent care.";

            return k switch
            {
                RemedyTemplateKind.Respiratory => new[]
                {
                    rest,
                    "Wash hands often; cover coughs/sneezes; ventilate the room gently.",
                    "Saline nasal rinse or steam inhalation (not scalding) can ease congestion — stop if it hurts ears or causes dizziness.",
                    "Honey may soothe throat in adults and children over 1 year; infants under 1 year must not have honey.",
                    "Throat lozenges only for ages safe to avoid choking; supervise children.",
                    track
                },
                RemedyTemplateKind.Digestive => new[]
                {
                    "Pause heavy, greasy, very spicy, and alcoholic foods until settled.",
                    "Eat small, bland portions as tolerated; stop if vomiting worsens.",
                    "For mild diarrhea, focus on oral rehydration strategy per clinician/pharmacist advice; avoid anti-diarrheal meds unless appropriate for your situation.",
                    rest,
                    track
                },
                RemedyTemplateKind.Pain => new[]
                {
                    rest,
                    "Gentle stretching, posture reset, and short walks may help tension — avoid forcing painful ranges.",
                    "Cool pack 10–15 min or warm pack (if no acute injury swelling) per usual first-aid principles.",
                    older ? "Older adults: falls are a major risk — use stable seating; avoid sedating remedies without medical review." : rest,
                    track
                },
                RemedyTemplateKind.Skin => new[]
                {
                    "Cleanse gently with mild soap and cool/lukewarm water; pat dry.",
                    "Protect area from friction; avoid scratching — trim nails, consider cotton gloves at night for kids if appropriate.",
                    "Sunburn: shade, fluids, and bland moisturizers; seek care for blistering large areas or systemic symptoms.",
                    track
                },
                RemedyTemplateKind.SleepStress => new[]
                {
                    "Dim lights 60 minutes before bed; reduce screens or use night modes.",
                    "Consistent sleep and wake times, even on weekends, help reset rhythm.",
                    "Brief relaxation: slow breathing 4–6 counts in/out for several minutes.",
                    "If anxiety is severe, persistent, or includes panic or self-harm thoughts, seek immediate professional help — hotlines exist in most countries."
                },
                RemedyTemplateKind.Ent => new[]
                {
                    "Avoid inserting cotton swabs deep into ear canals.",
                    "Warm compress outer ear only if clinician has said it is appropriate for your situation.",
                    "Dental pain often needs a dentist within days — temporary measures do not fix decay or abscess.",
                    track
                },
                RemedyTemplateKind.Eye => new[]
                {
                    "Do not rub eyes; wash hands before touching face.",
                    "Artificial tears (if previously tolerated) may soothe dryness — redness with pain or vision change needs urgent eye care.",
                    "Contact lens wearers: remove lenses on irritation unless an optometrist/ophthalmologist advises otherwise.",
                    track
                },
                RemedyTemplateKind.HeatFluid => new[]
                {
                    "Move to a cooler place, loosen clothing, sip water or oral rehydration fluids as directed.",
                    "Cool (not ice-cold) cloths on neck/armpits; fan air gently.",
                    "Heat stroke signs (confusion, hot dry skin, collapse) are emergencies — call emergency services.",
                    child ? "Children dehydrate quickly; offer frequent small sips; watch for listlessness or sunken eyes." : track
                },
                RemedyTemplateKind.Allergy => new[]
                {
                    "Reduce exposure: keep windows closed on high pollen days; shower after outdoor time; wash bedding weekly in hot water if dust-sensitive.",
                    "Rinse nose with saline; sunglasses reduce pollen eye contact outdoors.",
                    "OTC antihistamine choice is individual — pharmacist/clinician guidance if pregnant, nursing, driving, or on other meds.",
                    track
                },
                RemedyTemplateKind.TravelMotion => new[]
                {
                    "Face forward, fix gaze on horizon in motion sickness; fresh air when safe.",
                    "Small bland snacks; avoid strong odors; break long trips.",
                    "Altitude: ascend gradually when possible; recognize symptoms of altitude illness needing descent and medical care.",
                    "Alcohol hangover: time, hydration, food — repeated heavy drinking needs medical support, not home tips alone."
                },
                _ => new[]
                {
                    rest,
                    "Symptom diary for 24–48 hours helps decide if self-care is working.",
                    "Avoid starting multiple new supplements at once — hard to know what helped or harmed.",
                    track
                }
            };
        }

        private static IReadOnlyList<string> TemplateHydration(RemedyTemplateKind k, bool child)
        {
            var oral =
                "Fluids: frequent small sips of water; oral rehydration solutions may be preferred during vomiting/diarrhea per professional guidance.";
            return k switch
            {
                RemedyTemplateKind.Digestive or RemedyTemplateKind.HeatFluid => new[]
                {
                    oral,
                    child
                        ? "For children: watch wet diapers, tears when crying, and mouth moisture."
                        : "Pale urine roughly every few hours suggests adequate intake in healthy adults under normal conditions.",
                    "Broth, rice water, or diluted fruit juice may be tolerated — avoid excess caffeine/alcohol while recovering."
                },
                RemedyTemplateKind.Respiratory => new[]
                {
                    "Warm herbal teas or broth can soothe throat; caffeine only in moderation.",
                    oral
                },
                _ => new[]
                {
                    "Maintain regular water intake unless your doctor restricts fluids.",
                    oral
                }
            };
        }

        private static IReadOnlyList<string> TemplateComfort(RemedyTemplateKind k)
        {
            return k switch
            {
                RemedyTemplateKind.Respiratory => new[]
                {
                    "Elevate head on an extra pillow if congestion disturbs sleep.",
                    "Humidify air modestly; clean humidifiers to prevent mold."
                },
                RemedyTemplateKind.Pain => new[]
                {
                    "Quiet room, low light for headaches linked to sensory overload.",
                    "Gentle scalp or temple massage if it feels soothing (no pounding pressure)."
                },
                RemedyTemplateKind.SleepStress => new[]
                {
                    "Weighted blanket only if age/health appropriate and comfortable.",
                    "White noise at low volume can mask disruptive sounds."
                },
                _ => new[]
                {
                    "Comfort clothing, temperature layering, and foot elevation if ankles swell from long standing (not a substitute for heart/kidney evaluation if swelling is new)."
                }
            };
        }

        private static IReadOnlyList<string> TemplateAvoid(RemedyTemplateKind k, bool child)
        {
            var meds =
                "Do not combine multiple cold/flu products without reading labels — duplicate ingredients can overdose acetaminophen or cause sedation.";
            return k switch
            {
                RemedyTemplateKind.Digestive => new[]
                {
 "Avoid NSAIDs on an empty stomach if you are nauseated or vomiting — ask a clinician when pain relief is needed.",
                    "Do not force large meals; avoid unpasteurized dairy or questionable street food while symptomatic."
                },
                RemedyTemplateKind.Respiratory => new[]
                {
                    meds,
                    "Avoid smoking and secondhand smoke; vaping can also irritate airways."
                },
                RemedyTemplateKind.Skin => new[]
                {
                    "Avoid heavy fragranced lotions on broken skin.",
                    "Do not pop blisters from burns or bites — infection risk."
                },
                RemedyTemplateKind.Eye => new[]
                {
                    "Avoid cosmetic eye products during acute irritation.",
                    "No home steroid drops unless prescribed."
                },
                _ => new[]
                {
                    meds,
                    child ? "Keep adult medications and batteries out of reach." : "Check drug interactions before adding OTC products."
                }
            };
        }

        private static IReadOnlyList<string> TemplateRedFlags(RemedyTemplateKind k, bool child, bool older, string baseName)
        {
            var universal = new List<string>
            {
                "Emergency now: chest pain/pressure, trouble breathing at rest, fainting, confusion, severe sudden headache, weakness on one side, or severe bleeding.",
                "High fever with stiff neck or rash that does not fade under a glass (where meningitis is suspected) needs urgent care.",
                "Severe dehydration: minimal urine, dizziness on standing, very dry mouth, or no tears in infants.",
                "Symptoms that steadily worsen instead of improving over a sensible window for that illness."
            };
            if (child)
            {
                universal.Add(
                    "Child: unusual sleepiness, irritability, working hard to breathe (ribs pulling in), blue lips, or refusing fluids — urgent evaluation.");
                universal.Add("Infants under 3 months with any fever — seek medical advice promptly per local guidelines.");
            }

            if (older)
                universal.Add(
                    "Older adults: new confusion, falls, or reduced mobility during illness — lower threshold to seek care.");

            if (k == RemedyTemplateKind.Digestive)
                universal.Add(
                    "Blood in vomit or stool, black tarry stool, or severe unrelenting abdominal pain — emergency assessment.");
            if (k == RemedyTemplateKind.Respiratory)
                universal.Add("Wheezing not improving, oxygen saturation low if you can measure, or lips turning blue — urgent care.");
            if (k == RemedyTemplateKind.Ent)
                universal.Add("Sudden hearing loss, severe ear pain with fever, or swelling around the jaw — timely medical exam.");
            if (k == RemedyTemplateKind.Eye)
                universal.Add("Eye injury from chemical splash, penetrating object, or sudden vision loss — emergency eye care.");

            return universal;
        }

        internal readonly record struct Body(
            IReadOnlyList<string> Overview,
            IReadOnlyList<string> SelfCare,
            IReadOnlyList<string> Hydration,
            IReadOnlyList<string> Comfort,
            IReadOnlyList<string> Avoid,
            IReadOnlyList<string> RedFlags,
            string SourceNote);
    }

    private static class Synonyms
    {
        public static IEnumerable<string> Expand(string baseName)
        {
            var n = baseName.ToLowerInvariant();
            yield return n;
            if (n.Contains("nausea") || n.Contains("queasy"))
            {
                yield return "sick stomach";
                yield return "queasy";
                yield return "bilious";
            }

            if (n.Contains("diarr"))
            {
                yield return "loose stools";
                yield return "loose motion";
            }

            if (n.Contains("constip"))
            {
                yield return "hard stool";
                yield return "cannot pass stool";
            }

            if (n.Contains("cold") || n.Contains("runny"))
            {
                yield return "sniffles";
                yield return "rhinorrhea";
                yield return "fever cold";
            }

            if (n.Contains("cough"))
            {
                yield return "hacking";
                yield return "chesty cough";
            }

            if (n.Contains("headache") || n.Contains("head ache"))
            {
                yield return "head pain";
                yield return "migraine";
            }

            if (n.Contains("throat"))
            {
                yield return "sore throat";
                yield return "scratchy throat";
            }

            if (n.Contains("fever") || n.Contains("febrile"))
            {
                yield return "temperature";
                yield return "pyrexia";
            }

            if (n.Contains("heartburn") || n.Contains("reflux"))
            {
                yield return "acid";
                yield return "gerd flare";
            }

            if (n.Contains("bloat") || n.Contains("gas"))
            {
                yield return "wind";
                yield return "distension";
            }

            if (n.Contains("sleep") || n.Contains("insomnia"))
            {
                yield return "cannot sleep";
                yield return "sleepless";
            }

            if (n.Contains("anxiety") || n.Contains("stress"))
            {
                yield return "worry";
                yield return "panic";
            }

            if (n.Contains("cramp") && n.Contains("menstrual"))
            {
                yield return "period pain";
                yield return "dysmenorrhea";
            }

            if (n.Contains("sunburn"))
            {
                yield return "sun burn";
                yield return "red skin sun";
            }

            if (n.Contains("bite"))
            {
                yield return "insect bite";
                yield return "mosquito";
            }

            if (n.Contains("allergy") || n.Contains("hay fever"))
            {
                yield return "pollen";
                yield return "sneezing allergy";
            }
        }
    }
}
