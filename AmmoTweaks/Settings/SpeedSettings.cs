using Mutagen.Bethesda.Synthesis.Settings;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Skyrim;

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

        [SynthesisOrder] public HashSet<IFormLinkGetter<IProjectileGetter>> Exclusions = new()
        {
            Dragonborn.Projectile.DLC2ArrowRieklingSpearProjectile,
            Skyrim.Projectile.MQ101ArrowSteelProjectile
        };
    }
}
