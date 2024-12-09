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
        public const float RECIPE_LOSS_FACTOR = 0.8f;
        public static HashSet<string> recipeDefs = new HashSet<string> {"RandomizeItem", "ReverseRandomizeItem", "RandomItemFusion"};
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
                float finalValue = totalValue * RECIPE_LOSS_FACTOR;
                ThingDef newDef;
                if (!resources.TryGetValue(item.def, out newDef))
                {
                    Log.Error("Unable to randomize ingredient: " + item.def.ToString());
                    return __result;
                }
                Thing newItem = new Thing();
                if (recipeDef.defName == "RandomizeItem")
                {
                    newItem = RandomList.makeThingSubstitute(item.def);
                }
                else if (recipeDef.defName == "ReverseRandomizeItem")
                {
                    RandomList.reverseResources.TryGetValue(item.def, out newDef);
                    RandomList.reverseResources.TryGetValue(newDef, out newDef);
                    newItem = RandomList.makeThingSubstitute(newDef);
                }
                else if (recipeDef.defName == "RandomItemFusion")
                {
                    List<ThingDef> defList = new List<ThingDef>(RandomList.resources.Keys);
                    newDef = defList[RandomList.random.Next(defList.Count)];
                    newItem = RandomList.makeThingSubstitute(newDef);
                }
                newItem.stackCount = Math.Max(1, (int)Math.Round(finalValue / newItem.MarketValue));
                newList.Add(newItem);
                __result = newList;
                return __result;
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
}
