namespace StickFightColorCustomizer.Core

{

    public struct HatCatalogEntry

    {

        public string Id;

        public string Label;

    }



    public static class HatCatalog

    {

        public static readonly HatCatalogEntry[] ClassicEntries =

        {

            new HatCatalogEntry { Id = "none",      Label = "None" },

            new HatCatalogEntry { Id = "cap",       Label = "Cap" },

            new HatCatalogEntry { Id = "beanie",    Label = "Beanie" },

            new HatCatalogEntry { Id = "tophat",    Label = "Top Hat" },

            new HatCatalogEntry { Id = "cowboy",    Label = "Cowboy" },

            new HatCatalogEntry { Id = "cone",      Label = "Party" },

            new HatCatalogEntry { Id = "crown",     Label = "Crown" },

            new HatCatalogEntry { Id = "wizard",    Label = "Wizard" },

            new HatCatalogEntry { Id = "hardhat",   Label = "Hard Hat" },

            new HatCatalogEntry { Id = "bandana",   Label = "Bandana" },

            new HatCatalogEntry { Id = "propeller", Label = "Propeller" },

            new HatCatalogEntry { Id = "horns",     Label = "Horns" },

            new HatCatalogEntry { Id = "mario_cap", Label = "Plumber Cap" },

            new HatCatalogEntry { Id = "link_cap",  Label = "Hero Cap" },

            new HatCatalogEntry { Id = "ash_cap",   Label = "Trainer Cap" },

            new HatCatalogEntry { Id = "ac_hood",   Label = "Assassin Hood" },

            new HatCatalogEntry { Id = "toad_cap",  Label = "Mushroom Cap" },
            new HatCatalogEntry { Id = "chef",      Label = "Chef Toque" },
            new HatCatalogEntry { Id = "dunce",     Label = "Dunce Cap" },
            // ── HD image hats (PNG files in AlkaSkin_Images) ──
            new HatCatalogEntry { Id = "img_mario",        Label = "Mario Cap (HD)" },
            new HatCatalogEntry { Id = "img_luigi",        Label = "Luigi Cap (HD)" },
            new HatCatalogEntry { Id = "img_link",         Label = "Link Cap (HD)" },
            new HatCatalogEntry { Id = "img_ash",          Label = "Ash Cap (HD)" },
            new HatCatalogEntry { Id = "img_bison",        Label = "M. Bison Cap (HD)" },
            new HatCatalogEntry { Id = "img_wizard",       Label = "Wizard Hood (HD)" },
            new HatCatalogEntry { Id = "img_samus",        Label = "Samus Helmet (HD)" },
            new HatCatalogEntry { Id = "img_metroid",      Label = "Metroid Helmet (HD)" },
            new HatCatalogEntry { Id = "img_tacticalops",  Label = "Tactical Ops (HD)" },
            new HatCatalogEntry { Id = "img_masterchief",  Label = "Master Chief (HD)" }
        };



        public static readonly HatCatalogEntry[] Entries = ClassicEntries;



        public static bool IsValid(string hatId)

        {

            if (string.IsNullOrEmpty(hatId))

            {

                return false;

            }



            for (int i = 0; i < ClassicEntries.Length; i++)

            {

                if (ClassicEntries[i].Id == hatId)

                {

                    return true;

                }

            }



            return HatCategoryCatalog.IsVariant(hatId);

        }



        public static string Normalize(string hatId)

        {

            return IsValid(hatId) ? hatId : "none";

        }

    }

}


