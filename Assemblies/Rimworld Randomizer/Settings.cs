using System;
using RimWorld;
using Verse;
namespace Rimworld_Randomizer
{
    public class Settings : ModSettings
    {
        public bool randomizeStartingItems = true;
        public bool randomizeTraderDrops = false;
        public bool randomizeRecipes = true;
        public bool resourcesBasedOnMV = false;
        public Settings()
        {
        }
        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref randomizeStartingItems, "randomizeStartingItems");
            Scribe_Values.Look<bool>(ref randomizeTraderDrops, "randomizeTraderDrops");
            Scribe_Values.Look<bool>(ref randomizeRecipes, "randomizeRecipes");
            Scribe_Values.Look<bool>(ref resourcesBasedOnMV, "resourcesBasedOnMV");
        }

    }
}
