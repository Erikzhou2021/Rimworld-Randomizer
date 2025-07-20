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
using System.Reflection;

namespace Rimworld_Randomizer
{
    //[HarmonyPatch(typeof(ITrader), "GiveSoldThingToPlayer")]
    // can't patch interfaces
    //[HarmonyPatch(typeof(Caravan_TraderTracker), "GiveSoldThingToPlayer")]
    //[HarmonyPatch("GiveSoldThingToPlayer")]
    [HarmonyPatch]
    internal class TradePatch
    {

        [HarmonyTargetMethods]
        static IEnumerable<MethodBase> CalculateMethods()
        {
            yield return AccessTools.Method(typeof(Caravan_TraderTracker), "GiveSoldThingToPlayer");
            yield return AccessTools.Method(typeof(Settlement_TraderTracker), "GiveSoldThingToPlayer");
            // yield return AccessTools.Method(typeof(Pawn_TraderTracker), "GiveSoldThingToPlayer");
            // yield return AccessTools.Method(typeof(TradeShip), "GiveSoldThingToPlayer");
        }

        [HarmonyPrefix]
        public static bool GiveSoldThingToPlayer(ref Thing toGive, ref int countToGive, Pawn playerNegotiator)
        {
            if (!Controller.settings.randomizeTraderDrops)
            {
                return true;
            }
            toGive.stackCount = countToGive;
            toGive = ItemPlacer.RandomizeItem(toGive);
            countToGive = toGive.stackCount;
            return true;
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
                Thing tempThing = ItemPlacer.RandomizeItem(t, true);
                if (tempThing == null)
                {
                    return true;
                }
                t = tempThing;

                ActiveTransporterInfo activeDropPodInfo = new ActiveTransporterInfo();
                activeDropPodInfo.SingleContainedThing = t;
                activeDropPodInfo.leaveSlag = false;
                DropPodUtility.MakeDropPodAt(dropSpot, map, activeDropPodInfo);
                return false;
            }
            return true;
        }
    }
}
