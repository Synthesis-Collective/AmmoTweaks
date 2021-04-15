using Mutagen.Bethesda.Synthesis.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmoTweaks.Settings
{
    public class AmmoTweaksSettings
    {
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
