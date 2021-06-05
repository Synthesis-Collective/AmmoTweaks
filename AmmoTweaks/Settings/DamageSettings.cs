using Mutagen.Bethesda.Synthesis.Settings;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;

namespace AmmoTweaks.Settings
{
    public class DamageSettings
    {
        [SynthesisOrder]
        public bool DoRescaling = true;

        [SynthesisOrder]
        public float MinDamage = 8;

        [SynthesisOrder]
        public float MaxDamage = 35;

        [SynthesisOrder]
        public HashSet<IFormLinkGetter<IAmmunitionGetter>> Exclusions = new();
    }
}
