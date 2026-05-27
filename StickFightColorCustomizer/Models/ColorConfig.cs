namespace StickFightColorCustomizer.Models
{
    public sealed class ColorConfig
    {
        public BodyColors Colors = new BodyColors();
        public GlowSettings Glow = new GlowSettings();
        public HatSettings Hat = new HatSettings();
        public ShoeSettings Shoe = new ShoeSettings();
        public TopsSettings Tops = new TopsSettings();
        public WingSettings Wing = new WingSettings();
        public ObjectSettings Object = new ObjectSettings();
        public WeaponColorSettings Weapon = new WeaponColorSettings();
        public MiscSettings Misc = new MiscSettings();

        /// <summary>Si false, no repinta cuerpo ni mantiene líneas hasta activar en menú.
        /// Default ahora TRUE para que los colores del jugador sean estables en lobby
        /// (antes mostraba colores random vanilla cuando no estaba activado).</summary>
        public bool BodyCustomizationActive = true;

        public bool AnimatedRgb;
        public float RgbSpeed = 1f;
        public string ActivePreset = "Custom";
        public bool UseUniformSkin;

        /// <summary>
        /// Por defecto: no envía paquetes P2P custom ni toca materiales de red (compatible con lobby vanilla).
        /// </summary>
        public bool SafeVanillaMultiplayer = true;
        public MenuLanguage MenuLanguage = MenuLanguage.English;

        public float MenuWindowWidth = 540f;
        public float MenuWindowHeight = 680f;

        public ColorConfig Clone()
        {
            return new ColorConfig
            {
                Colors = Colors != null ? Colors.Clone() : new BodyColors(),
                Glow = Glow != null ? Glow.Clone() : new GlowSettings(),
                Hat = Hat != null ? Hat.Clone() : new HatSettings(),
                Shoe = Shoe != null ? Shoe.Clone() : new ShoeSettings(),
                Tops = Tops != null ? Tops.Clone() : new TopsSettings(),
                Wing = Wing != null ? Wing.Clone() : new WingSettings(),
                Object = Object != null ? Object.Clone() : new ObjectSettings(),
                Weapon = Weapon != null ? Weapon.Clone() : new WeaponColorSettings(),
                Misc = Misc != null ? Misc.Clone() : new MiscSettings(),
                BodyCustomizationActive = BodyCustomizationActive,
                AnimatedRgb = AnimatedRgb,
                RgbSpeed = RgbSpeed,
                ActivePreset = ActivePreset,
                UseUniformSkin = UseUniformSkin,
                SafeVanillaMultiplayer = SafeVanillaMultiplayer,
                MenuLanguage = MenuLanguage,
                MenuWindowWidth = MenuWindowWidth,
                MenuWindowHeight = MenuWindowHeight
            };
        }
    }
}
