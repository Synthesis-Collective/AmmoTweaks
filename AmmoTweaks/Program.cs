using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using System.Threading.Tasks;
using System.IO;

namespace AmmoTweaks
{
    public class Program
    {
        private static bool damageRescaling;
        private static float damageMult;
        private static float minDamage;
        private static float maxDamage;
        private static float lootMult;
        private static bool renaming;
        private static string separator = "";
        private static bool speedChanges;
        private static float speedArrow;
        private static float speedBolt;
        private static float gravity;

        private static List<FormKey> blacklist = new List<FormKey>(){
            Dragonborn.Projectile.DLC2ArrowRieklingSpearProjectile,
            Skyrim.Projectile.MQ101ArrowSteelProjectile
        };

        private static List<IAmmunitionGetter> patchammo = new List<IAmmunitionGetter>();

        private static List<String> overpowered = new List<String>();

        private static List<IPerkGetter> perks = new List<IPerkGetter>();
        public static Task<int> Main(string[] args)
        {
            return SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .Run(args, new RunPreferences()
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "AmmoTweaks.esp",
                        TargetRelease = GameRelease.SkyrimSE,
                        BlockAutomaticExit = true,
                    }
                });
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            string configFilePath = Path.Combine(state.ExtraSettingsDataPath, "config.json");

            if (!File.Exists(configFilePath))
            {
                Console.WriteLine("\"config.json\" cannot be found in the users Data folder.");
                return;
            }

            JObject config = JObject.Parse(File.ReadAllText(configFilePath));


            if (config.TryGetValue("damageRescaling", out var jRescale))
                damageRescaling = jRescale.Value<bool?>() ?? false;
            if (config.TryGetValue("TweakToNonPlayable", out var jNonPlayable))
                tweakNonPlayable = jNonPlayable.Value<bool?>() ?? false;
            if (config.TryGetValue("damageMult", out var jDamageMult))
                damageMult = jDamageMult.Value<float?>() ?? 1;
            if (config.TryGetValue("minDamage", out var jMin))
                minDamage = jMin.Value<float?>() ?? 4;
            if (config.TryGetValue("maxDamage", out var jMax))
                maxDamage = jMax.Value<float?>() ?? 25;
            if (config.TryGetValue("lootMult", out var jLoot))
                lootMult = jLoot.Value<float?>() ?? 1;
            if (config.TryGetValue("renaming", out var jRename))
                renaming = jRename.Value<bool?>() ?? false;
            if (config.TryGetValue("separator", out var jSeparator))
                separator = jSeparator.Value<string?>() ?? " - ";
            if (config.TryGetValue("speedChanges", out var jSpeedChanges))
                speedChanges = jSpeedChanges.Value<bool?>() ?? true;
            if (config.TryGetValue("speedArrow", out var jSspeedArrow))
                speedArrow = jSspeedArrow.Value<float?>() ?? 5400;
            if (config.TryGetValue("speedBolt", out var jSpeedBolt))
                speedBolt = jSpeedBolt.Value<float?>() ?? 8100;
            if (config.TryGetValue("gravity", out var jGravity))
                gravity = jGravity.Value<float?>() ?? (float)0.2;
        
            
            foreach (var ammogetter in state.LoadOrder.PriorityOrder.WinningOverrides<IAmmunitionGetter>())
            {
                if (tweakNonPlayable or !ammogetter.Flags.HasFlag(Ammunition.Flag.NonPlayable))
                {
                    patchammo.Add(ammogetter);                   
                }
            }

            foreach (var ammogetter in patchammo)
            {
                var ammo = state.PatchMod.Ammunitions.GetOrAddAsOverride(ammogetter);
                ammo.Weight = 0;

                if (damageRescaling && ammo.Damage != 0)
                {
                    var newDmg = ammo.Damage * damageMult;                   
                    if (maxDamage >= 0  && newDmg > maxDamage)
                    {
                        newDmg = maxDamage;  
                        if (ammogetter.Name?.String is string name) overpowered.Add(name);
                    }                                              
                    if (minDamage >= 0  && newDmg < minDamage) 
                    {
                        newDmg = minDamage;    
                    }
                    if (ammo.Damage != newDmg) Console.WriteLine($"Changing {ammo.Name} damage from {ammo.Damage} to {newDmg}.");
                    ammo.Damage =  newDmg;
                }

                if (speedChanges && ammo.Projectile.TryResolve<IProjectileGetter>(state.LinkCache, out var proj) && !blacklist.Contains(proj.FormKey)
                        && (proj.Gravity != gravity
                        || (proj.Speed != speedArrow && ammo.Flags.HasFlag(Ammunition.Flag.NonBolt))
                        || (proj.Speed != speedBolt && !ammo.Flags.HasFlag(Ammunition.Flag.NonBolt))))
                {
                    var projectile = state.PatchMod.Projectiles.GetOrAddAsOverride(proj);
                    Console.WriteLine($"Adjusting {proj.Name} projectile.");
                    projectile.Gravity = gravity;
                    if (ammo.Flags.HasFlag(Ammunition.Flag.NonBolt))
                    {
                        projectile.Speed = speedArrow;
                    }
                    else
                    {
                        projectile.Speed = speedBolt;
                    }

                }

                if (renaming) ammo.Name = RenameAmmo(ammo);
            }

            if (lootMult != 1)
            {
                if (state.LinkCache.TryResolve<IGameSettingGetter>(Skyrim.GameSetting.iArrowInventoryChance, out var gmst))
                {
                    var modifiedGmst = state.PatchMod.GameSettings.GetOrAddAsOverride(gmst);

                    int data = ((GameSettingInt)modifiedGmst).Data.GetValueOrDefault();
                    int newData = (int)Math.Round(data * lootMult);
                    ((GameSettingInt)modifiedGmst).Data = newData < 100 ? newData : 100;
                    Console.WriteLine($"Setting iArrowInventoryChance from {data} to {(newData < 100 ? newData : 100)}");
                }
                if (state.LinkCache.TryResolve<IPerkGetter>(Skyrim.Perk.HuntersDiscipline, out var perk))
                {
                    var modifiedPerk = state.PatchMod.Perks.GetOrAddAsOverride(perk);

                    foreach (var effect in modifiedPerk.Effects)
                    {
                        if (!(effect is PerkEntryPointModifyValue modValue)) continue;
                        if (modValue.EntryPoint == APerkEntryPointEffect.EntryType.ModRecoverArrowChance)
                        {
                            var value = modValue.Value;
                            var newValue = (float)Math.Round(value * lootMult);
                            modValue.Value = newValue < 100 ? newValue : 100;
                            Console.WriteLine($"Setting {modifiedPerk.Name} chance from {value} to {(newValue < 100 ? newValue : 100)}");
                        }
                    }
                }

            }
            if (overpowered.Count == 0) return;
            Console.WriteLine("Warning: The following ammunitions were above the upper damage limit. They have been reduced to the maximum.");
            foreach (var item in overpowered)
            {
                Console.WriteLine(item);
            }
        }

        private static String RenameAmmo(IAmmunitionGetter ammo)
        {
            if (!(ammo.Name?.String is string name)) return "";
            string oldname = name;
            string prefix = "";
            string pattern = "Arrow$|Bolt$";

            if (name.Contains("Arrow"))
            {
                prefix = "Arrow";
            }
            else if (name.Contains("Bolt"))
            {
                prefix = "Bolt";
            }
            else
            {
                return name;
            }
            name = prefix + separator + Regex.Replace(name, pattern, String.Empty);
            name = name.Trim(' ');
            Console.WriteLine($"Renaming {oldname} to {name}.");
            return name;
        }
    }
}
