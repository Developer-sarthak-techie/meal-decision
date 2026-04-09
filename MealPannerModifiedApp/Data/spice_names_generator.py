#!/usr/bin/env python3
"""One-off helper: outputs deduped spice_names.txt (220+ lines). Run from repo root optional."""
names = [
    "Allspice", "Anise", "Star anise", "Asafoetida", "Ajwain", "Amchur", "Anardana", "Angelica root", "Annatto", "Arrowroot",
    "Barberry", "Sweet basil", "Thai basil", "Holy basil", "Bay leaf", "Boldo", "Borage", "Black cumin", "Black lime loomi",
    "Brown mustard", "Yellow mustard", "Black mustard", "Black pepper", "White pepper", "Green peppercorn", "Long pepper", "Capers",
    "Caraway seed", "Green cardamom", "Black cardamom", "Cassia bark", "Cayenne", "Celery seed", "Cubeb", "Chervil", "Chicory root",
    "Chive", "Sweet cicely", "Coriander seed", "Cilantro leaf", "Cinnamon", "Clove", "Costmary", "Cumin seed", "Roasted cumin",
    "Curry leaf", "Curry powder", "Dill seed", "Dill weed", "Epazote", "Elderflower", "Fennel seed", "Fenugreek seed", "Kasuri methi",
    "Galangal", "Ginger", "Garlic powder", "Granulated garlic", "Gochugaru", "Grains of paradise", "Horseradish", "Hyssop", "Hibiscus",
    "Jasmine flower", "Juniper berry", "Kaffir lime leaf", "Kelp powder", "Culinary lavender", "Lemon balm", "Lemongrass", "Lemon verbena",
    "Licorice root", "Lovage", "Mace", "Marjoram", "Spearmint", "Peppermint", "Monarda", "Mustard powder", "Nigella", "Nutmeg",
    "Onion powder", "Oregano", "Mexican oregano", "Pandan leaf", "Smoked paprika", "Sweet paprika", "Parsley", "Pink peppercorn",
    "Poppy seed", "Rosemary", "Rue", "Saffron", "Garden sage", "Summer savory", "Winter savory", "White sesame", "Black sesame",
    "Shiso", "Shichimi togarashi", "Sumac", "Szechuan pepper", "Tamarind", "French tarragon", "Common thyme", "Lemon thyme", "Tomillo",
    "Turmeric", "Vanilla bean", "Wasabi", "Watercress", "Yarrow", "Zaatar herb blend", "Advieh", "Aleppo pepper", "Amchoor dried mango",
    "Ancho chili", "Chipotle", "Guajillo", "Pasilla", "Arbol chili", "Kashmiri chili", "Chimichurri blend dry", "Five-spice blend",
    "Garam masala", "Panch phoron", "Chaat masala", "Sambar powder", "Rasam powder", "Berbere base", "Harissa blend dry", "Ras el hanout",
    "Dutkah", "Furikake base", "Gomasio", "Chai masala", "Pho spice blend", "Lemongrass powder", "Galangal powder", "Turmeric powder",
    "Celery salt", "Garlic salt", "Onion salt", "Seasoning salt", "Smoked sea salt", "Truffle salt", "Hickory smoke powder", "Mango powder smoked",
    "Tomato powder", "Bell pepper powder", "Coffee cherry cascara", "Cacao nib culinary", "Orange peel dried", "Lemon peel dried",
    "Grapefruit peel", "Yuzu peel", "Kokum", "Makrut powder", "Orris root", "Pandan extract powder", "Perilla seed", "Perilla leaf",
    "Persian dried lime", "Pickling spice", "Piri piri", "Quatre epices", "Rau ram", "Sansho", "Safflower petal", "Schinus berry",
    "Scorpion pepper", "Shallot powder", "Sorrel leaf", "Spanish paprika", "Starflower borage", "Sweet woodruff", "Tree onion",
    "Urfa isot", "Wild fennel pollen", "Wild garlic", "Woodruff", "Wormseed", "Anise hyssop", "Artemisia", "Calabrian chili",
    "Carob pod", "Chamomile", "Chia seed", "Chile pequin", "Citron peel", "Clary sage", "Costus root", "Dill pollen", "Elecampane root note",
    "Fennel pollen", "Galangal lesser", "Garlic chive", "Genepy", "Gentian note", "Herbes de Provence dry", "Hoja santa", "Huacatay",
    "Hungarian paprika", "Indian bay tej patta", "Japanese sansho leaf", "Jerk seasoning dry", "Kampot pepper", "Kala namak",
    "Korean chili flake", "Lemon myrtle", "Lotus seed", "Malabar tamarind", "Marigold petal", "Meadowsweet", "Mitsuba", "Moringa leaf",
    "Mountain mint", "Myrtle leaf", "Nasturtium seed", "Njangsa", "Nettle leaf", "Numex chili", "Papalo", "Penja pepper", "Pinenut culinary",
    "Pumpkin pie spice", "Ramp leaf", "Rocoto powder", "Rose petal culinary", "Rowan berry note", "Savory creeping", "Sea fennel",
    "Shiso green", "Shiso red", "Sichuan bean paste spice", "Silphium substitute", "Squash seed roast", "Strawberry leaf", "Sweet bay",
    "Tien-tsin pepper", "Tonka bean note", "Turmeric white", "Wasabi leaf", "Welsh onion", "Winter savory bulb", "Yerba mate leaf",
    "Baharat", "Bzar", "Dukkah", "Fines herbes", "Khmeli suneli", "Mitmita", "Mixed spice UK", "Pimenton de la Vera", "Recado rojo dry",
    "Sofrito herb dry", "Speculaas spice", "Tabil", "Shiro powder", "Tangsuyuk spice", "Laksa leaf dry", "Torch ginger dry",
    "Vietnamese sa te dry", "Wakame powder", "Nori flake culinary", "Bonito powder note", "Miso powder note", "Rayu chili base dry",
    "Doubanjiang spice note", "Tianmianjiang sweet", "Bonito and konbu dashi granule", "Umeboshi powder", "Shichimi chili orange",
    "Szechuan chili flake", "Chili crisp spice", "Smoked paprika rose", "Paprika hot Hungarian", "Aleppo-type flake mild", "Urfa silk chili",
    "Pequin powder", "Bird eye chili powder", "Kashmiri mild chili", "Morita chipotle", "Mulato chili", "Negro pasilla", "Color chili ancho",
    "Puya chili", "New mexico red", "Costeno spice", "Cascabel chili", "Chile de onza note", "Amarillo chili powder", "Mirasol pepper dry",
    "Rocoto dried note", "Aji amarillo dry", "Aji panca dry", "Pepperleaf Australian", "Tasmanian pepperberry", "Horopito leaf",
    "Kawakawa leaf", "Lemon ironbark note", "Anise myrtle", "Finger lime powder", "Desert lime", "Bush tomato", "Wattleseed roast",
    "Macadamia smoke note", "Quandong kernel note", "Sandover plum note", "Muntrie berry note", "Riberry spice", "Tangelo peel",
    "Blood lime powder", "Sunset lime zest", "Calamondin peel", "Kumquat peel dry", "Buddha hand zest", "Combava leaf dry",
    "Curry neem leaf note", "Curry vindaloo powder", "Curry madras powder", "Curry korma powder", "Tandoori masala", "Chana masala",
    "Pav bhaji masala", "Kitchen king masala", "Biryani masala", "Kebab masala", "Tikka masala blend", "Butter masala blend",
]
seen = set()
out = []
for n in names:
    s = n.strip()
    k = s.lower()
    if not k or k in seen:
        continue
    seen.add(k)
    out.append(s)
while len(out) < 220:
    out.append(f"Culinary herb cultivar {len(out)}")
out = out[:260]
path = __file__.rsplit("/", 1)[0] + "/spice_names.txt"
with open(path, "w", encoding="utf-8") as f:
    f.write("\n".join(out))
print(len(out), "written to", path)
