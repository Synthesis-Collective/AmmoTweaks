using Mutagen.Bethesda.Synthesis.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmoTweaks.Settings
{
    public class SpeedSettings
    {
        [SynthesisOrder]
        public bool DoSpeedChanges = true;

        [SynthesisOrder]
        public float ArrowSpeed = 5400;

        [SynthesisOrder]
        public float BoltSpeed = 8100;

        [SynthesisOrder]
        public float Gravity = 0.2f;
    }
}
