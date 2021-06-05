using Mutagen.Bethesda.Synthesis.Settings;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;

namespace AmmoTweaks.Settings
{
    public class AmmoTweaksSettings
    {
        [SynthesisOrder]
        public HashSet<IFormLinkGetter<IAmmunitionGetter>> GlobalExclusions = new();
        
        [SynthesisOrder]
        public DamageSettings Damage = new();

        [SynthesisOrder]
        public RenamingSettings Renaming = new();

        [SynthesisOrder]
        public SpeedSettings Speed = new();

        [SynthesisOrder]
        public LootSettings Loot = new();
    }
}
