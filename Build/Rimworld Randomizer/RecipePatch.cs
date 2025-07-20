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
    [HarmonyPatch(typeof(GenRecipe), "MakeRecipeProducts")]
    internal class RecipePatch
    {
        //public const float RECIPE_LOSS_FACTOR = 0.8f;
        public static HashSet<string> recipeDefs = new HashSet<string> {"RandomizeItem", "RandomizeItemX4", "RandomItemFusion"};
        [HarmonyPostfix]
        public static IEnumerable<Thing> MakeRecipeProducts(IEnumerable<Thing> __result, RecipeDef recipeDef, List<Thing> ingredients)
        {
            List<Thing> newList = new List<Thing>();
            if (recipeDefs.Contains(recipeDef.defName))
            {
                Dictionary<ThingDef, ThingDef> resources = RandomList.resources;
                Thing item = ingredients[0];
                float totalValue = item.MarketValue * item.stackCount;
                for (int i = 1; i < ingredients.Count; i++)
                {
                    totalValue += ingredients[i].MarketValue * ingredients[i].stackCount;
                }
                //float finalValue = totalValue * RECIPE_LOSS_FACTOR;
                float finalValue = totalValue;
                ThingDef newDef;
                if (!resources.TryGetValue(item.def, out newDef))
                {
                    Log.Error("Unable to randomize ingredient: " + item.def.ToString());
                    return __result;
                }
                Thing newItem = new Thing();
                if (recipeDef.defName == "RandomizeItem")
                {
                    // newItem = RandomList.makeThingSubstitute(item.def);
                    newItem = ItemPlacer.RandomizeItem(item, false);
                    finalValue *= 0.7f;
                }
                else if (recipeDef.defName == "RandomizeItemX4")
                {
                    newItem = ItemPlacer.RandomizeItem(item, false);
                    finalValue *= 0.8f;
                }
                else if (recipeDef.defName == "RandomItemFusion")
                {
                    List<ThingDef> defList = new List<ThingDef>(RandomList.resources.Keys);
                    newDef = defList[RandomList.random.Next(defList.Count)];
                    newItem = RandomList.makeThingSubstitute(newDef);
                    finalValue *= 0.8f;
                }
                newItem.stackCount = Math.Max(1, (int) Math.Round(finalValue / newItem.MarketValue));
                newList.Add(newItem);
                return newList;
            }
            if (Controller.settings.randomizeRecipes)
            {
                foreach (Thing thing in __result)
                {
                    Thing tempThing = ItemPlacer.RandomizeItem(thing, false);
                    if (tempThing != null)
                    {
                        newList.Add(tempThing);
                    }
                    else
                    {
                        newList.Add(thing);
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
    /*
    [HarmonyPatch(typeof(WealthWatcher), "CalculateWealthItems")]
    internal class WealthWatcherPatch
    {
        [HarmonyPostfix]
        public static float CalculateWealthItems(float __result, WealthWatcher __instance)
        {
            Map map = Traverse.Create(__instance).Field("map").GetValue() as Map;
            List<Thing> tmpThings = Traverse.Create(__instance).Field("tmpThings").GetValue() as List<Thing>;
            tmpThings.Clear();
            Dictionary<ThingDef, float> marketValues = new Dictionary<ThingDef, float>();
            ThingOwnerUtility.GetAllThingsRecursively<Thing>(map, ThingRequest.ForGroup(ThingRequestGroup.HaulableEver), tmpThings, false, new Predicate<IThingHolder>(WealthWatcher.WealthItemsFilter));
            for (int index = 0; index < tmpThings.Count; ++index)
            {
                Thing tempThing = tmpThings[index];
                if (tempThing.SpawnedOrAnyParentSpawned && !tempThing.PositionHeld.Fogged(map))
                {
                    if (!marketValues.ContainsKey(tempThing.def))
                    {
                        marketValues.Add(tempThing.def, 0f);
                    }
                    marketValues[tempThing.def] += tempThing.stackCount;
                }
            }
            foreach (KeyValuePair<ThingDef, float> pair in marketValues)
            {
                Log.Message(pair);
            }
            return __result;
        }
    }
    */
}
