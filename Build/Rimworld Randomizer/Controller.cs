using System;
using RimWorld;
using Verse;
using UnityEngine;


namespace Rimworld_Randomizer
{
    public class Controller : Mod
    {
        public static Settings settings;

        public Controller(ModContentPack content) : base(content)
        {

            settings = GetSettings<Settings>();

            //LongEventHandler.QueueLongEvent(Patches.Init, "CE_LongEvent_CompatibilityPatches", false, null);

            //This is uncommented in the repository's assembly, so players that download the repo without compiling it are warned about potential issues.
            //LongEventHandler.QueueLongEvent(ShowUncompiledBuildWarning, "CE_LongEvent_ShowUncompiledBuildWarning", false, null);
        }
        public override string SettingsCategory()
        {
            return "Rimworld Randomizer";
        }
        public override void DoSettingsWindowContents(Rect canvas)
        {
            Listing_Standard list = new Listing_Standard();
            list.ColumnWidth = (canvas.width - 17) / 2; // Subtract 17 for gap between columns
            list.Begin(canvas);

            // Do general settings
            Text.Font = GameFont.Medium;
            list.Label("Randomizer Settings");
            Text.Font = GameFont.Small;
            list.Gap();

            list.CheckboxLabeled("Randomize Starting Items", ref settings.randomizeStartingItems, "If disabled, items will not be randomized for a few seconds after starting a new game.");
            list.CheckboxLabeled("Randomize Trader Drops", ref settings.randomizeTraderDrops, "If enabled, Items dropped by traders will also be randomized");
            list.CheckboxLabeled("Randomize Resources Based on Market Value", ref settings.resourcesBasedOnMV, "If enabled, the amount of RAW RESOURCES that drop in a stack will be determined by its relative marnet value compared to the item it replaces.");
            list.CheckboxLabeled("Randomize Crafting Recipes", ref settings.randomizeRecipes, "Determines whether or not items dropped by workstations will be randomized");

            if (Current.Game == null)
            {
                //list.CheckboxLabeled("Store Randomized Lists", ref settings.storeRandomLists, "Makes it so item randomization is tied with the save file, this option may be changed in game to prevent the game from breaking");

                //list.CheckboxLabeled("CE_Settings_ShowBackpacks_Title".Translate(), ref showBackpacks, "CE_Settings_ShowBackpacks_Desc".Translate());
                //list.CheckboxLabeled("CE_Settings_ShowWebbing_Title".Translate(), ref showTacticalVests, "CE_Settings_ShowWebbing_Desc".Translate());
            }
            else
            {
                // tell the user that he can only change these settings from main menu
                /*list.GapLine();
                Text.Font = GameFont.Medium;
                list.Label("CE_Settings_MainMenuOnly_Title".Translate(), tooltip: "CE_Settings_MainMenuOnly_Desc".Translate());
                Text.Font = GameFont.Small;

                list.Gap();
                list.Label("CE_Settings_ShowBackpacks_Title".Translate(), tooltip: "CE_Settings_ShowBackpacks_Desc".Translate());
                list.Label("CE_Settings_ShowWebbing_Title".Translate(), tooltip: "CE_Settings_ShowWebbing_Desc".Translate());
                list.Gap();*/
            }
            list.End();
        }
        }
    }
