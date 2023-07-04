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
        [HarmonyPostfix]
        public static IEnumerable<Thing> MakeRecipeProducts(IEnumerable<Thing> __result)
        {
            if (Controller.settings.randomizeRecipes)
            {
                List<Thing> newList = new List<Thing>();
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
