using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Unity;
using UnityEngine;
using Verse;
using LudeonTK;

namespace Rimworld_Randomizer
{
    internal class DebugActions
    {
        [DebugAction("Item Randomizer", "Spawn Without Random", false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static List<DebugActionNode> SpawnWithoutRandom()
        {
            //return DebugThingPlaceHelper.TryPlaceOptionsForStackCount(1, false).OrderBy<DebugActionNode, string>((Func<DebugActionNode, string>)(x => x.label)).ToList<DebugActionNode>();
            List<DebugActionNode> debugActionNodeList = new List<DebugActionNode>();
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs.Where<ThingDef>((Func<ThingDef, bool>)(def => DebugThingPlaceHelper.IsDebugSpawnable(def))))
            {
                ThingDef localDef = thingDef;
                debugActionNodeList.Add(new DebugActionNode(localDef.defName, DebugActionType.ToolMap, (Action) (() => {
                    if (RandomList.reverseGear.TryGetValue(thingDef, out localDef))
                    {
                    }
                    else if (RandomList.reverseItems.TryGetValue(thingDef, out localDef))
                    {
                    }
                    else if (RandomList.reverseResources.TryGetValue(thingDef, out localDef))
                    {
                    }
                    else if (RandomList.reverseFood.TryGetValue(thingDef, out localDef))
                    {
                    }
                    else
                    {
                        localDef = thingDef;
                    }
                    DebugThingPlaceHelper.DebugSpawn(localDef, UI.MouseCell(), 1);
                })));
            }
            return debugActionNodeList;
        }

        [DebugAction("Item Randomizer", "Spawn Reversed Random", false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static List<DebugActionNode> SpawnReversedRandom()
        {
            //return DebugThingPlaceHelper.TryPlaceOptionsForStackCount(1, false).OrderBy<DebugActionNode, string>((Func<DebugActionNode, string>)(x => x.label)).ToList<DebugActionNode>();
            List<DebugActionNode> debugActionNodeList = new List<DebugActionNode>();
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs.Where<ThingDef>((Func<ThingDef, bool>)(def => DebugThingPlaceHelper.IsDebugSpawnable(def))))
            {
                ThingDef localDef = thingDef;
                debugActionNodeList.Add(new DebugActionNode(localDef.defName, DebugActionType.ToolMap, (Action)(() => {
                    if (RandomList.reverseGear.TryGetValue(thingDef, out localDef))
                    {
                        RandomList.reverseGear.TryGetValue(localDef, out localDef);
                    }
                    else if (RandomList.reverseItems.TryGetValue(thingDef, out localDef))
                    {
                        RandomList.reverseItems.TryGetValue(localDef, out localDef);
                    }
                    else if (RandomList.reverseResources.TryGetValue(thingDef, out localDef))
                    {
                        RandomList.reverseResources.TryGetValue(localDef, out localDef);
                    }
                    else if (RandomList.reverseFood.TryGetValue(thingDef, out localDef))
                    {
                        RandomList.reverseFood.TryGetValue(localDef, out localDef);
                    }
                    else
                    {
                        localDef = thingDef;
                    }
                    DebugThingPlaceHelper.DebugSpawn(localDef, UI.MouseCell(), 1);
                })));
            }
            return debugActionNodeList;
        }
    }
}
