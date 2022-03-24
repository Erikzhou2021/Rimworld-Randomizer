using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Diagnostics;
using System.Xml;

namespace Rimworld_Randomizer
{
    [HarmonyPatch(typeof(GameConditionManager), "ExposeData")]
    internal class SavePatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(69)]
        public static void ExposeData()
        {
            Scribe_Collections.Look(ref RandomList.resources, "resources", LookMode.Def, LookMode.Def);
            Scribe_Collections.Look(ref RandomList.gear, "gear", LookMode.Def, LookMode.Def);
            Scribe_Collections.Look(ref RandomList.food, "food", LookMode.Def, LookMode.Def);
            Scribe_Collections.Look(ref RandomList.items, "items", LookMode.Def, LookMode.Def);
            if (Scribe.mode == LoadSaveMode.LoadingVars && RandomList.resources == null)
            {
                RandomList.resources = ListScrambler.randResources();
                RandomList.gear = ListScrambler.randGear();
                RandomList.food = ListScrambler.randFood();
                RandomList.items = ListScrambler.randItems();
                Scribe.mode = LoadSaveMode.Saving;
                Scribe_Collections.Look(ref RandomList.resources, "resources", LookMode.Def, LookMode.Def);
                Scribe_Collections.Look(ref RandomList.gear, "gear", LookMode.Def, LookMode.Def);
                Scribe_Collections.Look(ref RandomList.food, "food", LookMode.Def, LookMode.Def);
                Scribe_Collections.Look(ref RandomList.items, "items", LookMode.Def, LookMode.Def);
                Scribe.mode = LoadSaveMode.LoadingVars;

            }

        }
    }
    [HarmonyPatch(typeof(GenRecipe), "MakeRecipeProducts")]
    internal class RecipePatch
    {
        [HarmonyPostfix]
        public static IEnumerable<Thing> MakeRecipeProducts(IEnumerable<Thing> __result)
        {
            if (Controller.settings.randomizeRecipes)
            {
                List<Thing> newList = new List<Thing>();
                foreach (Thing thing in __result)
                {
                    if (thing.def.IsWithinCategory(ThingCategoryDefOf.Apparel) || thing.def.IsWithinCategory(ThingCategoryDefOf.Weapons))
                    {
                        Thing tempThing = RandomList.makeThingSubstitute(RandomList.gear[thing.def]);
                        if (tempThing.def.MadeFromStuff)
                        {
                            //Log.Message(newThing.def.ToString());
                            IEnumerable<ThingDef> source = DefDatabase<ThingDef>.AllDefs.Where<ThingDef>((Func<ThingDef, bool>)(def => def.stuffProps != null && def.stuffProps.categories != null && def.stuffProps.categories.Contains(tempThing.def.stuffCategories.RandomElement())));
                            ThingDef stuff = source.RandomElement<ThingDef>();
                            tempThing = ThingMaker.MakeThing(tempThing.def, stuff);
                            //tempThing.Destroy();
                        }
                        newList.Add(tempThing);
                    }
                    if (thing.def.IsWithinCategory(ThingCategoryDefOf.ResourcesRaw) || thing.def.IsWithinCategory(ThingCategoryDefOf.Manufactured))
                    {
                        Thing tempThing = RandomList.makeThingSubstitute(RandomList.resources[thing.def]);
                        tempThing.stackCount = thing.stackCount;
                        newList.Add(tempThing);
                    }
                    if (thing.def.IsWithinCategory(ThingCategoryDefOf.Items) && !thing.def.isUnfinishedThing || thing.def.IsWithinCategory(ThingCategoryDefOf.Artifacts))
                    {
                        Thing tempThing = RandomList.makeThingSubstitute(RandomList.items[thing.def]);
                        newList.Add(tempThing);
                    }
                    if (thing.def.IsWithinCategory(ThingCategoryDefOf.Foods))
                    {
                        int count = thing.stackCount;
                        Thing tempThing = RandomList.makeThingSubstitute(RandomList.food[thing.def]);
                        float baseNutrition = thing.GetStatValue(StatDefOf.Nutrition) * (float)count;
                        float newCount = baseNutrition / tempThing.def.GetStatValueAbstract(StatDefOf.Nutrition);
                        tempThing.stackCount = (int)newCount;
                        newList.Add(tempThing);
                    }

                }
                return newList;
            }
            else
            {
                return __result;
            }
        }
    }
    [HarmonyPatch(typeof(TradeUtility), "SpawnDropPod")]
    internal class orbitalTraderPatch
    {
        [HarmonyPrefix]
        public static bool SpawnDropPod(IntVec3 dropSpot, Map map, Thing t)
        {

            if (!Controller.settings.randomizeTraderDrops)
            {
                ThingDef tempDef;
                Dictionary<ThingDef, ThingDef> reverseSearch = null;
                if (RandomList.items.TryGetValue(t.def, out tempDef))
                {
                    reverseSearch = ListScrambler.reverseDict(RandomList.items);
                }
                else if (RandomList.resources.TryGetValue(t.def, out tempDef))
                {
                    reverseSearch = ListScrambler.reverseDict(RandomList.resources);
                }
                else if (RandomList.food.TryGetValue(t.def, out tempDef))
                {
                    reverseSearch = ListScrambler.reverseDict(RandomList.food);
                }
                else if (RandomList.gear.TryGetValue(t.def, out tempDef))
                {
                    reverseSearch = ListScrambler.reverseDict(RandomList.gear);
                }
                if (reverseSearch != null)
                {
                    int stackCount = t.stackCount;
                    Thing tempThing = RandomList.makeThingSubstitute(reverseSearch[t.def]);
                    t = tempThing;
                    t.stackCount = stackCount;
                }
                ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
                activeDropPodInfo.SingleContainedThing = t;
                activeDropPodInfo.leaveSlag = false;
                DropPodUtility.MakeDropPodAt(dropSpot, map, activeDropPodInfo);

            }
            return false;
        }
    }
}
