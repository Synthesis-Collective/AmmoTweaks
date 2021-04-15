using Mutagen.Bethesda.Synthesis.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
