using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using System.Diagnostics;
using System.Xml;

namespace Rimworld_Randomizer
{
    [HarmonyPatch(typeof(GameConditionManager), "ExposeData")]
    internal class SavePatch
    {
        public static int lastSaveLoadTick = 0;
        [HarmonyPostfix]
        [HarmonyPriority(69)]
        public static void ExposeData()
        {
            lastSaveLoadTick = GenTicks.TicksAbs;
            Scribe_Values.Look(ref lastSaveLoadTick, "lastSaveLoadTick");
            Scribe_Collections.Look(ref RandomList.gear, "gear", LookMode.Def, LookMode.Def);
            Scribe_Collections.Look(ref RandomList.items, "items", LookMode.Def, LookMode.Def);
            Scribe_Collections.Look(ref RandomList.resources, "resources", LookMode.Def, LookMode.Def);
            Scribe_Collections.Look(ref RandomList.food, "food", LookMode.Def, LookMode.Def);
            Scribe_Collections.Look(ref RandomList.reverseGear, "reverseGear", LookMode.Def, LookMode.Def);
            Scribe_Collections.Look(ref RandomList.reverseItems, "reverseItems", LookMode.Def, LookMode.Def);
            Scribe_Collections.Look(ref RandomList.reverseResources, "reverseResources", LookMode.Def, LookMode.Def);
            Scribe_Collections.Look(ref RandomList.reverseFood, "reverseFood", LookMode.Def, LookMode.Def);

            if (Scribe.mode == LoadSaveMode.LoadingVars) //shouldn't ever happen unless a save is broken
            {
                if (RandomList.gear == null || RandomList.items == null || RandomList.resources == null || RandomList.food == null)
                {
                    Log.Warning("If you are reading this, your file was corrupted. Things should still work fine, but the item randomization will be different from before");
                    ListScrambler.randAll();
                    ListScrambler.reverseAll();
                    Scribe.mode = LoadSaveMode.Saving;
                    ExposeData();
                    Scribe.mode = LoadSaveMode.LoadingVars;
                }
                else if(RandomList.reverseGear == null || RandomList.reverseItems == null || RandomList.reverseResources == null || RandomList.reverseFood == null)
                {
                    Log.Warning("If you are reading this, your file was corrupted. Things should still (mostly) work fine though");
                    ListScrambler.reverseAll();
                    Scribe.mode = LoadSaveMode.Saving;
                    ExposeData();
                    Scribe.mode = LoadSaveMode.LoadingVars;
                }
            }
            
        }
    }

    [HarmonyPatch(typeof(WorldGenerator), "GenerateWorld")]
    internal static class GenerateWorldPatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(420)]
        public static void Postfix()
        {
            ListScrambler.randAll();
            ListScrambler.reverseAll();
            SavePatch.lastSaveLoadTick = 0;
        }
    }
}
