using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Rimworld_Randomizer
{
    static class ListScrambler
    {
        public static Dictionary<ThingDef, ThingDef> randResources()
        {
            IEnumerable<ThingDef> rawResources = DefDatabase<ThingDef>.AllDefs.Where<ThingDef>((Func<ThingDef, bool>)(def => def.IsWithinCategory(ThingCategoryDefOf.ResourcesRaw) || def.IsWithinCategory(ThingCategoryDefOf.Manufactured)));
            return scramble(rawResources);
        }
        public static Dictionary<ThingDef, ThingDef> randItems()
        {
            IEnumerable<ThingDef> items = DefDatabase<ThingDef>.AllDefs.Where<ThingDef>((Func<ThingDef, bool>)(def => def.IsWithinCategory(ThingCategoryDefOf.Items) && !def.isUnfinishedThing || def.IsWithinCategory(ThingCategoryDefOf.Artifacts)));
            return scramble(items);
        }
        public static Dictionary<ThingDef, ThingDef> randGear()
        {
            IEnumerable<ThingDef> items = DefDatabase<ThingDef>.AllDefs.Where<ThingDef>((Func<ThingDef, bool>)(def => def.IsWithinCategory(ThingCategoryDefOf.Weapons) || def.IsWithinCategory(ThingCategoryDefOf.Apparel)));
            return scramble(items);
        }
        public static Dictionary<ThingDef, ThingDef> randFood()
        {
            IEnumerable<ThingDef> items = DefDatabase<ThingDef>.AllDefs.Where<ThingDef>((Func<ThingDef, bool>)(def => def.IsWithinCategory(ThingCategoryDefOf.Foods)));
            return scramble(items);
        }
        public static Dictionary<ThingDef, ThingDef> randCorpse()
        {
            IEnumerable<ThingDef> items = DefDatabase<ThingDef>.AllDefs.Where<ThingDef>((Func<ThingDef, bool>)(def => def.IsWithinCategory(ThingCategoryDefOf.Corpses)));
            return scramble(items);
        }
        public static Dictionary<ThingDef, ThingDef> scramble(IEnumerable<ThingDef> rawResources)
        {
            var rand = new Random();
            int removedAt;
            var tempList = rawResources.ToList();
            Dictionary<ThingDef, ThingDef> switchedList = new Dictionary<ThingDef, ThingDef>();

            for (int i = 0; i < rawResources.Count() - 1; i++)
            {
                removedAt = rand.Next(tempList.Count() - 1);
                switchedList.Add(rawResources.ElementAt(i), tempList[removedAt]);
                tempList.RemoveAt(removedAt);
                //Log.Message(switchedList.ElementAt(i).ToString());
            }
            return switchedList;
        }
        public static Dictionary<ThingDef, ThingDef> reverseDict(Dictionary<ThingDef, ThingDef> original)
        {
            Dictionary<ThingDef, ThingDef> reversed = new Dictionary<ThingDef, ThingDef>();
            foreach (KeyValuePair<ThingDef, ThingDef> pair in original)
            {
                reversed.Add(pair.Value, pair.Key);
            }
            return reversed;
        }
        public static List<ThingDef> splitDict(Dictionary<ThingDef, ThingDef> dict, bool getKeys)
        {
            List<ThingDef> newList = new List<ThingDef>();
            foreach (KeyValuePair<ThingDef, ThingDef> pair in dict)
            {
                if (getKeys)
                {
                    newList.Add(pair.Key);
                }
                else
                {
                    newList.Add(pair.Value);
                }
            }
            return newList;
        }
    }
}
