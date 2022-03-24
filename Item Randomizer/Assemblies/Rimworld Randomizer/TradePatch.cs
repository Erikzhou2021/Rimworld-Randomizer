using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Threading.Tasks;

namespace Rimworld_Randomizer
{
    [HarmonyPatch(typeof(RimWorld.Planet.Settlement_TraderTracker), "GiveSoldThingToPlayer")]
    internal class SettlementTradePatch
    {
        [HarmonyPrefix]
        public static void GiveSoldThingToPlayer(ref Thing toGive, ref int countToGive, ref Pawn playerNegotiator)
        {
            if (Controller.settings.randomizeTraderDrops && toGive.def != null)
            {
                if (toGive.def.IsWithinCategory(ThingCategoryDefOf.Apparel) || toGive.def.IsWithinCategory(ThingCategoryDefOf.Weapons))
                {
                    Thing tempThing = RandomList.makeThingSubstitute(RandomList.gear[toGive.def]);
                    if (tempThing.def.MadeFromStuff)
                    {
                        //Log.Message(toGive.def.ToString());
                        IEnumerable<ThingDef> source = DefDatabase<ThingDef>.AllDefs.Where<ThingDef>((Func<ThingDef, bool>)(def => def.stuffProps != null && def.stuffProps.categories != null && def.stuffProps.categories.Contains(tempThing.def.stuffCategories.RandomElement())));
                        ThingDef stuff = source.RandomElement<ThingDef>();
                        toGive = ThingMaker.MakeThing(tempThing.def, stuff);
                    }
                    else
                    {
                        toGive = tempThing;
                        toGive.stackCount = countToGive;
                        //tempThing.Destroy();
                    }
                    var quality = toGive.TryGetComp<CompQuality>();
                    if (quality != null)
                        toGive.TryGetComp<CompQuality>().SetQuality(RandomList.RandQuality(), ArtGenerationContext.Colony);
                }
                else if (toGive.def.IsWithinCategory(ThingCategoryDefOf.Items) && !toGive.def.isUnfinishedThing || toGive.def.IsWithinCategory(ThingCategoryDefOf.Artifacts))
                {
                    Thing tempThing = RandomList.makeThingSubstitute(RandomList.items[toGive.def]);
                    toGive = tempThing;
                    toGive.stackCount = countToGive;
                }
                else if (toGive.def.IsWithinCategory(ThingCategoryDefOf.Foods))
                {
                    float count = countToGive;
                    float baseNutrition = toGive.GetStatValue(StatDefOf.Nutrition) * count;
                    Thing tempThing = RandomList.makeThingSubstitute(RandomList.food[toGive.def]);
                    int newCount = (int)(baseNutrition / (float)tempThing.def.GetStatValueAbstract(StatDefOf.Nutrition));
                    if (newCount < 1)
                    {
                        newCount = 1;
                    }
                    toGive = tempThing;
                    toGive.stackCount = newCount;
                    countToGive = (int)newCount;
                }
                else if (toGive.def.IsWithinCategory(ThingCategoryDefOf.ResourcesRaw) || toGive.def.IsWithinCategory(ThingCategoryDefOf.Manufactured))
                {
                    Thing tempThing = RandomList.makeThingSubstitute(RandomList.resources[toGive.def]);
                    int count = countToGive;
                    if (Controller.settings.resourcesBasedOnMV)
                    {
                        float baseValue = toGive.GetStatValue(StatDefOf.MarketValue) * (float)count;
                        count = (int)(baseValue / tempThing.def.GetStatValueAbstract(StatDefOf.MarketValue));
                    }
                    if (count < 1)
                    {
                        count = 1;
                    }
                    toGive = tempThing;
                    toGive.stackCount = count;
                    countToGive = count;
                }
            }
        }
    }
    [HarmonyPatch(typeof(RimWorld.Planet.Caravan_TraderTracker), "GiveSoldThingToPlayer")]
    internal class CaravanTradePatch
    {
        [HarmonyPrefix]
        public static void GiveSoldThingToPlayer(ref Thing toGive, ref int countToGive, ref Pawn playerNegotiator)
        {
            SettlementTradePatch.GiveSoldThingToPlayer(ref toGive, ref countToGive, ref playerNegotiator);
        }
    }
}
