using Mutagen.Bethesda.Synthesis.Settings;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;

namespace AmmoTweaks.Settings
{
    public class RenamingSettings
    {
        [SynthesisOrder]
        public bool DoRenaming;

        [SynthesisOrder]
        public string Separator = " - ";

        [SynthesisOrder]
        public HashSet<IFormLinkGetter<IAmmunitionGetter>> Exclusions = new();
    }
}
