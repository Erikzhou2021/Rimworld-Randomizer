using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Xml;

namespace Rimworld_Randomizer
{
    [StaticConstructorOnStartup]
    public static class RandomList
    {
        public static System.Random random = new System.Random();
        public static Dictionary<ThingDef, ThingDef> resources;
        public static Dictionary<ThingDef, ThingDef> gear;
        public static Dictionary<ThingDef, ThingDef> food;
        public static Dictionary<ThingDef, ThingDef> corpses;
        public static Dictionary<ThingDef, ThingDef> items;
        public static List<ThingDef> Keys = new List<ThingDef>();
        public static List<ThingDef> Values = new List<ThingDef>();
        public static bool flag = false;
        public static bool recursing = false;
        //public static Dictionary<ThingDef, ThingDef> stuffsource;
        
        static RandomList()
        {
            Harmony harmony = new Harmony("rimworld.Rimworld_Randomizer");
            resources = ListScrambler.randResources();
            gear = ListScrambler.randGear();
            food = ListScrambler.randFood();
            items = ListScrambler.randItems();
            //corpses = ListScrambler.randCorpse();
            harmony.PatchAll();
        }
        public static Thing makeThingSubstitute(ThingDef def, ThingDef stuff = null)
        {
            if (stuff != null && !stuff.IsStuff)
            {
                //Log.Error("MakeThing error: Tried to make " + (object)def + " from " + (object)stuff + " which is not a stuff. Assigning default.");
                stuff = GenStuff.DefaultStuffFor((BuildableDef)def);
            }
            if (def.MadeFromStuff && stuff == null)
            {
                //Log.Error("MakeThing error: " + (object)def + " is madeFromStuff but stuff=null. Assigning default.");
                stuff = GenStuff.DefaultStuffFor((BuildableDef)def);
            }
            if (!def.MadeFromStuff && stuff != null)
            {
                //Log.Error("MakeThing error: " + (object)def + " is not madeFromStuff but stuff=" + (object)stuff + ". Setting to null.");
                stuff = (ThingDef)null;
            }
            Thing instance = (Thing)Activator.CreateInstance(def.thingClass);
            instance.def = def;
            instance.SetStuffDirect(stuff);
            instance.PostMake();
            instance.PostPostMake();
            return instance;
        }
        public static QualityCategory RandQuality()
        {
            int num = random.Next(7);
            switch (num)
            {
                case 0:
                    return QualityCategory.Awful;
                case 1:
                    return QualityCategory.Poor;
                case 2:
                    return QualityCategory.Normal;
                case 3: 
                    return QualityCategory.Good;
                case 4:
                    return QualityCategory.Excellent;
                case 5:
                    return QualityCategory.Masterwork;
                case 6:
                    return QualityCategory.Legendary;
            }
            return QualityCategory.Normal;
        }
        
    }
}
