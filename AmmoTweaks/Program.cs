using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Alphaleonis.Win32.Filesystem;
using Newtonsoft.Json.Linq;


namespace BoltAndArrowPatcher
{
    public class Program
    {
        private static bool rescaling;
        private static float minDamage;
        private static float maxDamage;
        private static float lootMult;
        private static bool renaming;
        private static string separator = "";

        private static List<IAmmunitionGetter> patchammo = new List<IAmmunitionGetter>();

        public static int Main(string[] args)
        {
            return SynthesisPipeline.Instance.Patch<ISkyrimMod, ISkyrimModGetter>(
                args: args,
                patcher: RunPatch,
                userPreferences: new UserPreferences()
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "AmmoTweaks.esp",
                        TargetRelease = GameRelease.SkyrimSE,
                        BlockAutomaticExit = true,
                    }
                });
        }

        public static void RunPatch(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {
            string configFilePath = Path.Combine(state.ExtraSettingsDataPath, "config.json");

            if (!File.Exists(configFilePath))
            {
                Console.WriteLine("\"config.json\" cannot be found in the users Data folder.");
                return;
            }

            JObject config = JObject.Parse(File.ReadAllText(configFilePath));


            if (config.TryGetValue("minDamage", out var jRescale))
                rescaling = jRescale.Value<bool?>() ?? false;
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

            float vmin = maxDamage;
            float vmax = minDamage;
            foreach (var ammogetter in state.LoadOrder.PriorityOrder.WinningOverrides<IAmmunitionGetter>())
            {
                if (!ammogetter.Flags.HasFlag(Ammunition.Flag.NonPlayable))
                {
                    patchammo.Add(ammogetter);
                    var dmg = ammogetter.Damage;
                    if (ammogetter.Damage == 0) continue;
                    if (dmg < vmin) vmin = dmg;
                    if (dmg > vmax && (dmg < maxDamage)) vmax = dmg;
                }
            }

            foreach (var ammogetter in patchammo)
            {
                var ammo = state.PatchMod.Ammunitions.GetOrAddAsOverride(ammogetter);           
                 ammo.Weight = 0;
                
                if (rescaling && ammo.Damage != 0)
                {
                    var dmg = ammo.Damage;
                    ammo.Damage = (float)Math.Round(((ammo.Damage - vmin) / (vmax - vmin)) * (maxDamage - minDamage) + minDamage);
                    Console.WriteLine($"Changing {ammo.Name} damage from {dmg} to {ammo.Damage}");
                }

                if (renaming) ammo.Name = RenameAmmo(ammo);
            }

            if (lootMult != 1)
            {
                foreach (var gmst in state.LoadOrder.PriorityOrder.WinningOverrides<IGameSettingGetter>())
                {
                    if (gmst.EditorID?.Contains("iArrowInventoryChance") == true)
                    {
                        var modifiedGmst = state.PatchMod.GameSettings.GetOrAddAsOverride(gmst);

                        int data = ((GameSettingInt)modifiedGmst).Data.GetValueOrDefault();
                        int newdata = (int)Math.Round(data * lootMult);
                        ((GameSettingInt)modifiedGmst).Data = newdata < 100 ? newdata : 100;
                        Console.WriteLine($"Setting iArrowInventoryChance from {data} to {(newdata < 100 ? newdata : 100)}");
                    }
                }
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
            Console.WriteLine($"Renaming {oldname} to {name}");
            return name;
        }
    }
}
