using Mutagen.Bethesda.Synthesis.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmoTweaks.Settings
{
    public class RenamingSettings
    {
        [SynthesisOrder]
        public bool DoRenaming;

        [SynthesisOrder]
        public string Separator = " - ";
    }
}
