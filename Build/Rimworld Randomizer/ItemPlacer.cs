using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Diagnostics;
using System.Xml;
namespace Rimworld_Randomizer
{
    [HarmonyPatch (typeof(GenSpawn), "Spawn", new[] {typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool), typeof(bool)})]
    class ItemPlacer
    {
        [HarmonyPrefix]
        public static bool replaceItems(ref Thing newThing, IntVec3 loc, Map map, Rot4 rot, WipeMode wipeMode = WipeMode.Vanish, bool respawningAfterLoad = false)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackList = stackTrace.GetFrames();
            if (SavePatch.lastSaveLoadTick > GenTicks.TicksAbs)
            {
                SavePatch.lastSaveLoadTick = GenTicks.TicksAbs;
            }
            if (!Controller.settings.randomizeStartingItems && GenTicks.TicksGame < 600)
            {
                return true;
            }
            else if ((GenTicks.TicksAbs - SavePatch.lastSaveLoadTick < 5 && GenTicks.TicksGame > 5))
            {
                return true;
            }
            //Log.Message("GenTicks.TicksAbs = " + GenTicks.TicksAbs.ToString());
            //Log.Message("lastSaveLoadTick " + SavePatch.lastSaveLoadTick.ToString());
            foreach (StackFrame sf in stackList)
            {
                //Log.Message(sf.GetMethod().Name);
                String name = sf.GetMethod().Name;
                if (name == "TryDrop" || name == "Kill" || name.Contains("FinishRecipeAndStartStoringProduct") || name == "<UninstallIfMinifiable>b__1" /*|| name == "CheckMoveItemsAside"*/) {
                    return true;
                }
                else if(name == "GiveSoldThingToPlayer" && !Controller.settings.randomizeTraderDrops)
                {
                    return true;
                }
            }
            if (newThing.def == null)
            {
                return true;
            }

            Thing tempThing = RandomizeItem(newThing, false);
            if (tempThing == null)
            {
                return true;
            }
            int tempCount = tempThing.stackCount;
            if (tempCount > tempThing.def.stackLimit)
            {
                int times = tempCount / tempThing.def.stackLimit;
                int leftover = tempCount % tempThing.def.stackLimit;
                for (int i = 0; i < times; i++)
                {
                    Thing thing3 = RandomList.makeThingSubstitute(tempThing.def);
                    thing3.Rotation = rot;
                    thing3.Position = loc;
                    thing3.stackCount = tempThing.def.stackLimit;
                    if (tempThing.holdingOwner != null)
                    {
                        tempThing.holdingOwner.Remove(newThing); //idk when this would be useful
                    }
                    thing3.SpawnSetup(map, respawningAfterLoad);
                }
                if (leftover > 0)
                {
                    Thing thing4 = tempThing;
                    thing4.Rotation = rot;
                    thing4.Position = loc;
                    thing4.stackCount = leftover;
                    if (tempThing.holdingOwner != null)
                    {
                        tempThing.holdingOwner.Remove(newThing);
                    }
                    thing4.SpawnSetup(map, respawningAfterLoad);
                }
                return false;
            }
            newThing = tempThing;
            newThing.stackCount = tempCount;
            return true;
        }

        public static Thing RandomizeItem(Thing original, bool reversed = false)
        {
            Thing tempThing = null;
            ThingDef tempDef;
            int stackCount = original.stackCount;
            Dictionary<ThingDef, ThingDef> gear = reversed ? RandomList.reverseGear : RandomList.gear;
            Dictionary<ThingDef, ThingDef> items = reversed ? RandomList.reverseItems : RandomList.items;
            Dictionary<ThingDef, ThingDef> resources = reversed ? RandomList.reverseResources : RandomList.resources;
            Dictionary<ThingDef, ThingDef> food = reversed ? RandomList.reverseFood : RandomList.food;
            if (gear.TryGetValue(original.def, out tempDef))
            {
                tempThing = RandomList.makeThingSubstitute(tempDef);
                if (tempThing.def.MadeFromStuff)
                {
                    IEnumerable<ThingDef> source = new List<ThingDef>();
                    foreach (var category in tempThing.def.stuffCategories)
                    {
                        IEnumerable<ThingDef> temp = DefDatabase<ThingDef>.AllDefs.Where<ThingDef>(
                        (Func<ThingDef, bool>)(def => def.stuffProps != null && def.stuffProps.categories != null &&
                        def.stuffProps.categories.Contains(category)));
                        source = source.ConcatIfNotNull(temp);
                    }
                    ThingDef stuff = source.RandomElement<ThingDef>();
                    tempThing = ThingMaker.MakeThing(tempThing.def, stuff);
                }
                var quality = tempThing.TryGetComp<CompQuality>();
                if (quality != null)
                {
                    tempThing.TryGetComp<CompQuality>().SetQuality(RandomList.RandQuality(), ArtGenerationContext.Colony);
                }
            }
            else if (items.TryGetValue(original.def, out tempDef))
            {
                tempThing = RandomList.makeThingSubstitute(tempDef);
            }
            else if (resources.TryGetValue(original.def, out tempDef))
            {
                tempThing = RandomList.makeThingSubstitute(tempDef);
                if (Controller.settings.resourcesBasedOnMV)
                {
                    float totalValue = original.def.BaseMarketValue * stackCount;
                    stackCount = (int)Math.Round(totalValue / tempDef.BaseMarketValue);
                }
            }
            else if (food.TryGetValue(original.def, out tempDef))
            {
                tempThing = RandomList.makeThingSubstitute(tempDef);
                float totalNutrition = original.GetStatValue(StatDefOf.Nutrition) * stackCount;
                stackCount = (int)Math.Round(totalNutrition / (float) tempDef.GetStatValueAbstract(StatDefOf.Nutrition));
            }
            if (tempDef == null)
            {
                return null;
            }
            if (tempThing != null && tempThing.GetType() == typeof(Verse.Book))
            {
                tempThing = BookUtility.MakeBook(ArtGenerationContext.Outsider);
            }
            stackCount = Math.Max(stackCount, 1);
            tempThing.stackCount = stackCount;
            if (tempThing != null && tempThing.GetType() == typeof(RimWorld.Xenogerm))
            {
                tempThing.Notify_DebugSpawned();
            }
            return tempThing;
        }
    }
}
