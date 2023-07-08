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
        [HarmonyPostfix]
        public static IEnumerable<Thing> MakeRecipeProducts(IEnumerable<Thing> __result, RecipeDef recipeDef, List<Thing> ingredients)
        {
            List<Thing> newList = new List<Thing>();
            if (recipeDef.defName == "RandomizeItem")
            {
                Dictionary<ThingDef, ThingDef> resources = RandomList.resources;
                if (ingredients.Count > 1)
                {
                    Log.Error("Randomize Item recipe shouldn't allow more than one ingredient at a time");
                }
                Thing item = ingredients[0];
                float totalValue = item.MarketValue * item.stackCount;
                float finalValue = totalValue * RECIPE_LOSS_FACTOR;
                ThingDef newDef;
                if (resources.TryGetValue(item.def, out newDef))
                {
                    Thing newItem = ThingMaker.MakeThing(newDef);
                    newItem.stackCount = Math.Max(1, (int)Math.Round(finalValue / newItem.MarketValue));
                    newList.Add(newItem);
                    __result = newList;
                    return __result;
                }
                else
                {
                    Log.Error("Unable to randomize ingredient: " + item.def.ToString());
                }
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
