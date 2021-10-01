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
    [HarmonyPatch (typeof(GenSpawn), "Spawn", new[] {typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool)})]
    class ItemPlacer
    {
        [HarmonyPrefix]
        public static bool replaceItems(ref Thing newThing, IntVec3 loc, Map map, Rot4 rot, WipeMode wipeMode = WipeMode.Vanish, bool respawningAfterLoad = false)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackList = stackTrace.GetFrames();
            //bool recursed = false;
            foreach (StackFrame sf in stackList)
            {
                //Log.Message(sf.GetMethod().Name);
                if (sf.GetMethod().Name == "TryDrop" || sf.GetMethod().Name == "<FinishRecipeAndStartStoringProduct>b__0") { 
                    return true;
                }else if((GenTicks.TicksAbs - LoadSavePatch.x < 5 && GenTicks.TicksGame > 5) || (LoadSavePatch.x == -1 && GenTicks.TicksGame > 600))
                {
                    return true;
                }else if(!Controller.settings.randomizeTraderDrops && sf.GetMethod().Name == "GiveSoldThingToPlayer")
                {
                    return true;
                }
                /*if (RandomList.recursing)
                {
                    return true;
                }*/
            }
            if (!Controller.settings.randomizeStartingItems && GenTicks.TicksGame < 600)
            {
                return true;
            }
            if (newThing.def != null)
            {
                if (newThing.def.IsWithinCategory(ThingCategoryDefOf.Apparel) || newThing.def.IsWithinCategory(ThingCategoryDefOf.Weapons))
                {
                    Thing tempThing = RandomList.makeThingSubstitute(RandomList.gear[newThing.def]);
                    if (tempThing.def.MadeFromStuff)
                    {
                        //Log.Message(newThing.def.ToString());
                        IEnumerable<ThingDef> source = DefDatabase<ThingDef>.AllDefs.Where<ThingDef>((Func<ThingDef, bool>)(def => def.stuffProps != null && def.stuffProps.categories != null && def.stuffProps.categories.Contains(tempThing.def.stuffCategories.RandomElement())));
                        ThingDef stuff = source.RandomElement<ThingDef>();
                        newThing = ThingMaker.MakeThing(tempThing.def, stuff);
                        //tempThing.Destroy();
                    }
                    else
                    {
                        newThing = tempThing;
                        //tempThing.Destroy();
                    }
                    var quality = newThing.TryGetComp<CompQuality>();
                    if (quality != null)
                        newThing.TryGetComp<CompQuality>().SetQuality(RandomList.RandQuality(), ArtGenerationContext.Colony);
                    return true;
                }
                if (newThing.def.IsWithinCategory(ThingCategoryDefOf.ResourcesRaw) || newThing.def.IsWithinCategory(ThingCategoryDefOf.Manufactured))
                {
                    Thing tempThing = RandomList.makeThingSubstitute(RandomList.resources[newThing.def]);
                    int count = newThing.stackCount;
                    if (Controller.settings.resourcesBasedOnMV)
                    {
                        float baseValue = newThing.GetStatValue(StatDefOf.MarketValue) * (float)count;
                        count = (int)(baseValue / tempThing.def.GetStatValueAbstract(StatDefOf.MarketValue));
                    }
                    if (count < 1)
                    {
                        count = 1;
                    }
                    if (count > tempThing.def.stackLimit)
                    {
                        //Dictionary<ThingDef,ThingDef> reverseSearch = ListScrambler.reverseDict(RandomList.resources);
                        //Thing thing2 = RandomList.makeThingSubstitute(reverseSearch[newThing.def]);
                        //thing2.stackCount = tempThing.def.stackLimit;
                        //GenSpawn.Spawn(thing2, loc, map, rot,wipeMode,respawningAfterLoad);
                        int times = count / tempThing.def.stackLimit;
                        int leftover = count % tempThing.def.stackLimit;
                        for (int i = 0; i < times; i++)
                        {
                            Thing thing3 = RandomList.makeThingSubstitute(RandomList.resources[newThing.def]);
                            thing3.Rotation = rot;
                            thing3.Position = loc;
                            thing3.stackCount = tempThing.def.stackLimit;
                            if (tempThing.holdingOwner != null)
                            {
                                tempThing.holdingOwner.Remove(newThing);
                            }
                            thing3.SpawnSetup(map, respawningAfterLoad);
                        }
                        Thing thin4 = tempThing;
                        thin4.Rotation = rot;
                        thin4.Position = loc;
                        thin4.stackCount = leftover;
                        if (tempThing.holdingOwner != null)
                        {
                            tempThing.holdingOwner.Remove(newThing);
                        }
                        thin4.SpawnSetup(map, respawningAfterLoad);

                        //newThing = thing3;
                        //newThing.stackCount = thing3.def.stackLimit;
                        return false;
                        
                    }
                    else
                    {
                        newThing = tempThing;
                        newThing.stackCount = count;
                    }
                }
                if (newThing.def.IsWithinCategory(ThingCategoryDefOf.Items) && !newThing.def.isUnfinishedThing || newThing.def.IsWithinCategory(ThingCategoryDefOf.Artifacts))
                {
                    Thing tempThing = RandomList.makeThingSubstitute(RandomList.items[newThing.def]);
                    newThing = tempThing;
                }
                if (newThing.def.IsWithinCategory(ThingCategoryDefOf.Foods))
                { 
                    float count = newThing.stackCount;
                    float baseNutrition = newThing.GetStatValue(StatDefOf.Nutrition) * count;
                    Thing tempThing = RandomList.makeThingSubstitute(RandomList.food[newThing.def]);
                    int newCount = (int)(baseNutrition / (float) tempThing.def.GetStatValueAbstract(StatDefOf.Nutrition));
                    if (newCount < 1)
                    {
                        newCount = 1;
                    }
                    if (newCount > tempThing.def.stackLimit)
                    {
                        /*Dictionary<ThingDef, ThingDef> reverseSearch = ListScrambler.reverseDict(RandomList.food);
                        Thing thing1 = RandomList.makeThingSubstitute(reverseSearch[newThing.def]);
                        thing1.stackCount = tempThing.def.stackLimit;
                        GenSpawn.Spawn(thing1, loc, map, rot);
                        newThing = tempThing;
                        newThing.stackCount = tempThing.def.stackLimit;*/
                        int times = newCount / tempThing.def.stackLimit;
                        int leftover = newCount % tempThing.def.stackLimit;
                        for (int i = 0; i < times; i++)
                        {
                            Thing thing3 = RandomList.makeThingSubstitute(RandomList.food[newThing.def]);
                            thing3.Rotation = rot;
                            thing3.Position = loc;
                            thing3.stackCount = tempThing.def.stackLimit;
                            if (tempThing.holdingOwner != null)
                            {
                                tempThing.holdingOwner.Remove(newThing);
                            }
                            thing3.SpawnSetup(map, respawningAfterLoad);
                        }
                        Thing thing4 = tempThing;
                        thing4.Rotation = rot;
                        thing4.Position = loc;
                        thing4.stackCount = leftover;
                        if (tempThing.holdingOwner != null)
                        {
                            tempThing.holdingOwner.Remove(newThing);
                        }
                        thing4.SpawnSetup(map, respawningAfterLoad);
                        return false;
                    }
                    else
                    {
                        newThing = tempThing;
                        newThing.stackCount = (int) newCount;
                    }
                    }
            }


            return true;
        }
    }
}
