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
            IEnumerable<ThingDef> items = DefDatabase<ThingDef>.AllDefs.Where<ThingDef>((Func<ThingDef, bool>)(def => def.IsWithinCategory(ThingCategoryDefOf.Items) && !def.isUnfinishedThing || def.IsWithinCategory(ThingCategoryDefOf.Neurotrainers)));
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
        public static void randAll()
        {
            RandomList.items = randItems();
            RandomList.gear = randGear();
            RandomList.resources = randResources();
            RandomList.food = randFood();
        }
        public static void reverseAll()
        {
            RandomList.reverseItems = reverseDict(RandomList.items);
            RandomList.reverseResources = reverseDict(RandomList.resources);
            RandomList.reverseFood = reverseDict(RandomList.food);
            RandomList.reverseGear = reverseDict(RandomList.gear);
        }
        public static Dictionary<ThingDef, ThingDef> scramble(IEnumerable<ThingDef> things)
        {
            var rand = new Random();
            Dictionary<ThingDef, ThingDef> result = new Dictionary<ThingDef, ThingDef>();
            List<ThingDef> tempList = new List<ThingDef>();
            int count = 0;

            foreach (ThingDef thing in things)
            {
                if (!thing.destroyOnDrop)
                {
                    tempList.Add(thing);
                    count++;
                }
            }
            int end = count;
            List<ThingDef> copy = new List<ThingDef>(tempList);
            for (int i = 0; i < count; i++)
            {
                int j = rand.Next(end - 1);
                ThingDef toSwap = copy[j];
                copy[j] = copy[end -1];
                copy[end - 1] = toSwap;
                result.Add(tempList[i], toSwap);
                end--;
            }
            return result;
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
