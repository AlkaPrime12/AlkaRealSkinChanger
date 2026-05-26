# -*- coding: utf-8 -*-
"""Generate MenuLocalization.Catalog.cs with EN/ES item labels."""
import re, os

ROOT = os.path.join(os.path.dirname(__file__), '..', 'StickFightColorCustomizer', 'Core')
OUT = os.path.join(ROOT, 'MenuLocalization.Catalog.cs')

# id -> Spanish label (English taken from catalog)
ES = {
    'cap': 'Gorra', 'beanie': 'Gorro', 'tophat': 'Sombrero de copa', 'cowboy': 'Vaquero',
    'cone': 'Fiesta', 'crown': 'Corona', 'wizard': 'Mago', 'hardhat': 'Casco obra',
    'bandana': 'Bandana', 'propeller': 'Hélice', 'horns': 'Cuernos',
    'mario_cap': 'Gorra fontanero', 'link_cap': 'Gorra héroe', 'ash_cap': 'Gorra entrenador',
    'ac_hood': 'Capucha asesino', 'toad_cap': 'Gorra hongo', 'chef': 'Gorro chef',
    'dunce': 'Gorro tonto',
    'img_mario': 'Gorra Mario (HD)', 'img_luigi': 'Gorra Luigi (HD)',
    'img_link': 'Gorra Link (HD)', 'img_ash': 'Gorra Ash (HD)',
    'img_bison': 'Gorra M. Bison (HD)', 'img_wizard': 'Capucha mago (HD)',
    'img_samus': 'Casco Samus (HD)', 'img_metroid': 'Casco Metroid (HD)',
    'img_tacticalops': 'Casco táctico (HD)', 'img_masterchief': 'Casco Master Chief (HD)',
    'sneakers': 'Zapatillas', 'boots': 'Botas', 'combat': 'Combate', 'dress': 'Vestido',
    'sandals': 'Sandalias', 'cleats': 'Tacos', 'slippers': 'Pantuflas',
    'steel_toe': 'Punta acero', 'rollers': 'Patines', 'spikes': 'Botas con pinchos',
    'high_top': 'Zapatillas botín', 'running': 'Running', 'neon_kicks': 'Neon kicks',
    'biker': 'Botas moto', 'cowboy_b': 'Botas vaqueras',
    'slim_black': 'Bota negra slim', 'slim_brown': 'Bota cuero slim',
    'slim_combat': 'Bota combate slim', 'slim_ninja': 'Bota ninja slim',
    'slim_formal': 'Bota formal slim', 'slim_hiker': 'Bota montaña slim',
    'slim_punk': 'Bota punk slim', 'slim_arctic': 'Bota ártica slim',
    'slim_rider': 'Bota jinete slim', 'slim_chelsea': 'Chelsea slim',
    'sh_loafer': 'Mocasín', 'sh_oxford': 'Oxford', 'sh_moccasin': 'Mocasín clásico',
    'sh_court': 'Zapatilla court', 'sh_kicks_red': 'Kicks rojas',
    'sh_kicks_blue': 'Kicks azules', 'sh_kicks_pink': 'Kicks rosas',
    'sh_chelsea': 'Chelsea slip-on', 'sh_ballet': 'Bailarinas',
    'sh_geta': 'Geta', 'sh_iceskate': 'Patines hielo', 'sh_snowboot': 'Botas nieve',
    'sh_rainboot': 'Botas lluvia', 'sh_flipflop': 'Chancletas', 'sh_clog': 'Zuecos',
    'sh_platform': 'Plataformas', 'sh_skater': 'Skate', 'sh_canvas': 'Lona',
    'sh_geta_blue': 'Geta azul', 'sh_winged': 'Bota alada',
    'tshirt': 'Remera', 'hoodie': 'Buzo', 'jacket': 'Campera', 'tank': 'Musculosa',
    'dress_shirt': 'Camisa', 'jersey': 'Camiseta deportiva', 'vest': 'Chaleco',
    'armor_gold': 'Armadura dorada', 'tuxedo': 'Esmoquin', 'clown': 'Payaso',
    'neon': 'Cyber neón', 'varsity': 'Bomber varsity',
    'tx_lava': 'Remera lava', 'tx_galaxy': 'Galaxia', 'tx_camo': 'Camuflaje',
    'tx_pirate': 'Abrigo pirata', 'tx_knight': 'Placas caballero', 'tx_ninja': 'Gi ninja',
    'tx_pharaoh': 'Túnica faraón', 'tx_robot': 'Chasis robot', 'tx_skeleton': 'Esqueleto',
    'tx_lab': 'Bata laboratorio', 'tx_track': 'Chándal', 'tx_kimono': 'Kimono',
    'tx_punk': 'Chaleco punk', 'tx_gradient': 'Gradiente pop', 'tx_streetwear': 'Streetwear',
    'tx_holiday': 'Suéter navideño', 'tx_racer': 'Mono carreras', 'tx_priest': 'Túnica clérigo',
    'tx_chef': 'Delantal chef', 'tx_diver': 'Traje neopreno',
}

RULES = [
    ('Truth Orbs', 'Orbes verdad'), ('Battle Rings', 'Anillos batalla'),
    ('White Orbs', 'Orbes blancos'), ('Cyan Gems', 'Gemas cian'),
    ('Gold Orbs', 'Orbes dorados'), ('Purple Orbs', 'Orbes violeta'),
    ('Floating Halo', 'Halo flotante'), ('Sapphire Gems', 'Gemas zafiro'),
    ('Ruby Gems', 'Gemas rubí'), ('Emerald Gems', 'Gemas esmeralda'),
    ('Amethyst Gems', 'Gemas amatista'), ('Golden Stars', 'Estrellas doradas'),
    ('White Stars', 'Estrellas blancas'), ('Blue Plasma', 'Plasma azul'),
    ('Purple Plasma', 'Plasma violeta'), ('Cyan Rings', 'Anillos cian'),
    ('Steel Knives', 'Cuchillos acero'), ('Gold Knives', 'Cuchillos oro'),
    ('Shurikens', 'Shurikens'), ('Red Shurikens', 'Shurikens rojos'),
    ('Steel Swords', 'Espadas acero'), ('Fire Swords', 'Espadas fuego'),
    ('Red Kanji', 'Kanji rojo'), ('Gold Kanji', 'Kanji dorado'),
    ('Black Kanji', 'Kanji negro'), ('Skulls', 'Calaveras'),
    ('Cursed Skulls', 'Calaveras malditas'), ('Hearts', 'Corazones'),
    ('Dark Hearts', 'Corazones oscuros'), ('Lightning', 'Rayos'),
    ('Cyan Bolts', 'Rayos cian'), ('Snowflakes', 'Copos nieve'),
    ('Green Leaves', 'Hojas verdes'), ('Autumn Leaves', 'Hojas otoño'),
    ('Yin-Yang', 'Yin-Yang'), ('Gold Crosses', 'Cruces doradas'),
    ('Silver Moons', 'Lunas plateadas'),
    ('Inverted Crosses', 'Cruces invertidas'), ('Red Inverted Cross', 'Cruz invertida roja'),
    ('White Inverted Cross', 'Cruz invertida blanca'),
    ('Glow Letter X', 'Letra X brillante'), ('Glow Letter O', 'Letra O brillante'),
    ('Glow Letter Z', 'Letra Z brillante'),
    ('Red Han', 'Han rojo'), ('Gold Han', 'Han dorado'), ('Cyan Han', 'Han cian'),
    ('Void Han', 'Han vacío'), ('Infinity', 'Infinito'), ('Omega', 'Omega'),
    ('Pentagram', 'Pentagrama'), ('Wi-Fi Arc', 'Arco Wi-Fi'),
    ('Hashtag', 'Hashtag'), ('Meme Smile', 'Sonrisa meme'),
    ('Spiral Eyes', 'Ojos espiral'), ('Norse Rune', 'Runa nórdica'),
    ('Soul Flame', 'Llama alma'), ('Mini Wings', 'Mini alas'),
    ('Battle Axes', 'Hachas batalla'), ('Arrow Spiral', 'Espiral flechas'),
    ('DNA Helix', 'Hélice ADN'), ('Atom', 'Átomo'), ('Flame Ring', 'Anillo llamas'),
    ('Ice Shards', 'Fragmentos hielo'), ('Water Drops', 'Gotas agua'),
    ('Pocket Clocks', 'Relojes bolsillo'), ('Watching Eyes', 'Ojos vigilantes'),
    ('Chain Links', 'Eslabones cadena'), ('Gold Coins', 'Monedas oro'),
    ('Dice', 'Dados'), ('Playing Cards', 'Cartas'),
]

def to_spanish(item_id, en):
    if item_id in ES:
        return ES[item_id]
    s = en
    for a, b in RULES:
        s = s.replace(a, b)
    return s

def collect_items():
    items = {}
    for fn in ['HatCatalog.cs', 'ShoeCatalog.cs', 'TopsCatalog.cs', 'ObjectsCatalog.cs']:
        text = open(os.path.join(ROOT, fn), encoding='utf-8').read()
        for m in re.finditer(r'Id\s*=\s*"([^"]+)"[^;\n]{0,400}?Label\s*=\s*"([^"]+)"', text, re.DOTALL):
            i, l = m.group(1), m.group(2)
            if i != 'none':
                items[i] = l
        for m in re.finditer(r'Id\s*=\s*"([^"]+)"\s*,\s*\n\s*Label\s*=\s*"([^"]+)"', text):
            i, l = m.group(1), m.group(2)
            if i != 'none':
                items[i] = l
    return items

items = collect_items()
lines = []
lines.append('// Generated by tools/gen_catalog_i18n.py — do not edit by hand.')
lines.append('namespace StickFightColorCustomizer.Core')
lines.append('{')
lines.append('    public static partial class MenuLocalization')
lines.append('    {')
lines.append('        static partial void RegisterCatalogItems()')
lines.append('        {')
for iid in sorted(items.keys()):
    en = items[iid].replace('\\', '\\\\').replace('"', '\\"')
    es = to_spanish(iid, items[iid]).replace('\\', '\\\\').replace('"', '\\"')
    lines.append('            Register("item_%s", "%s", "%s");' % (iid, en, es))
lines.append('        }')
lines.append('    }')
lines.append('}')
open(OUT, 'w', encoding='utf-8').write('\n'.join(lines) + '\n')
print('Wrote %d items to %s' % (len(items), OUT))
