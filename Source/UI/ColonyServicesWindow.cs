using RimWorld;
using UnityEngine;
using Verse;
using WorkbenchMerchant2.Source.Trading;
using BuyAnything.Source;
using BuyAnything.Source.Settings;

namespace WorkbenchMerchant2.Source.UI
{
    public class ColonyServicesWindow : Window

    {
        private Vector2 scrollPosition = Vector2.zero;
        public override Vector2 InitialSize => new Vector2(1150f, 820f);

        public ColonyServicesWindow()
        {

            draggable = true;
            doCloseButton = true;
            doCloseX = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;

            Widgets.Label(
                new Rect(20f, 15f, 400f, 35f),
                "Colony Services");

            Text.Anchor = TextAnchor.UpperRight;

            Widgets.Label(
                new Rect(
                    inRect.width - 270f,
                    15f,
                    250f,
                    35f),
                "Silver: " + Find.CurrentMap.resourceCounter.Silver);

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            Widgets.Label(
                new Rect(
                    20f,
                    50f,
                    700f,
                    24f),
                "Hire professional contractors to instantly perform colony-wide services.");

            Rect panelRect = new Rect(
                15f,
                80f,
                inRect.width - 30f,
                inRect.height - 95f);

            Widgets.DrawMenuSection(panelRect);

            Rect outRect = new Rect(
                panelRect.x + 8f,
                panelRect.y + 8f,
                panelRect.width - 16f,
                panelRect.height - 16f);

            Rect viewRect = new Rect(
                0f,
                0f,
                outRect.width - 20f,
                1050f);

            Widgets.BeginScrollView(
                outRect,
                ref scrollPosition,
                viewRect);

            float y = 10f;

            //==================================================
            // SANITATION
            //==================================================

            Widgets.DrawLineHorizontal(0f, y, viewRect.width);
            y += 10f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(5f, y, 300f, 30f), "Sanitation");
            Text.Font = GameFont.Small;

            y += 35f;

            if (Widgets.ButtonText(
                new Rect(10f, y, 220f, 30f),
                "Clean Colony"))
            {
                ColonyServiceManager.CleanColony(Find.CurrentMap);
            }

            Widgets.Label(
                new Rect(250f, y + 1f, 180f, 20f),
                $"{ModMain.Settings.cleanColonyCost} Silver");

            Widgets.Label(
                new Rect(250f, y + 17f, 650f, 20f),
                "Removes all filth from the colony.");

            y += 45f;

            //==================================================
            // CLEANUP
            //==================================================

            Widgets.DrawLineHorizontal(0f, y, viewRect.width);
            y += 10f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(5f, y, 300f, 30f), "Cleanup");
            Text.Font = GameFont.Small;

            y += 35f;

            if (Widgets.ButtonText(
                new Rect(10f, y, 220f, 30f),
                "Remove Bodies"))
            {
                ColonyServiceManager.RemoveBodies(Find.CurrentMap);
            }

            Widgets.Label(
                new Rect(250f, y + 1f, 180f, 20f),
                $"{ModMain.Settings.removeBodiesCost} Silver");

            Widgets.Label(
                new Rect(250f, y + 17f, 650f, 20f),
                "Removes every corpse on the map.");

            y += 40f;

            if (Widgets.ButtonText(
                new Rect(10f, y, 220f, 30f),
                "Remove Chunks"))
            {
                ColonyServiceManager.RemoveChunks(Find.CurrentMap);
            }

            Widgets.Label(
                new Rect(250f, y + 1f, 180f, 20f),
                $"{ModMain.Settings.removeChunksCost} Silver");

            Widgets.Label(
                new Rect(250f, y + 17f, 650f, 20f),
                "Removes all stone chunks and slag.");

            y += 50f;

            //==================================================
            // LOGISTICS
            //==================================================

            Widgets.DrawLineHorizontal(0f, y, viewRect.width);
            y += 10f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(5f, y, 300f, 30f), "Logistics");
            Text.Font = GameFont.Small;

            y += 35f;

            if (Widgets.ButtonText(
                new Rect(10f, y, 220f, 30f),
                "Auto Haul"))
            {
                ColonyServiceManager.AutoHaul(Find.CurrentMap);
            }

            Widgets.Label(
                new Rect(250f, y + 1f, 180f, 20f),
                $"{ModMain.Settings.autoHaulCost} Silver");

            Widgets.Label(
                new Rect(250f, y + 17f, 650f, 20f),
                "Instantly hauls all haulable items into storage.");

            y += 50f;

            //==================================================
            // AGRICULTURE
            //==================================================

            Widgets.DrawLineHorizontal(0f, y, viewRect.width);
            y += 10f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(5f, y, 300f, 30f), "Agriculture");
            Text.Font = GameFont.Small;

            y += 35f;

            if (Widgets.ButtonText(
                new Rect(10f, y, 220f, 30f),
                "Harvest Trees"))
            {
                ColonyServiceManager.HarvestTrees(Find.CurrentMap);
            }

            Widgets.Label(
                new Rect(250f, y + 1f, 180f, 20f),
                $"{ModMain.Settings.harvestTreesCost} Silver");

            Widgets.Label(
                new Rect(250f, y + 17f, 650f, 20f),
                "Harvests all designated trees.");

            y += 40f;

            if (Widgets.ButtonText(
                new Rect(10f, y, 220f, 30f),
                "Harvest Crops"))
            {
                ColonyServiceManager.HarvestCrops(Find.CurrentMap);
            }

            Widgets.Label(
                new Rect(250f, y + 1f, 180f, 20f),
                $"{ModMain.Settings.harvestCropsCost} Silver");

            Widgets.Label(
                new Rect(250f, y + 17f, 650f, 20f),
                "Harvests every mature crop.");

            y += 50f;
            //==================================================
            // CONSTRUCTION
            //==================================================

            Widgets.DrawLineHorizontal(10f, y, viewRect.width - 20f);
            y += 10f;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(15f, y, 300f, 30f), "Construction");
            Text.Font = GameFont.Small;
            y += 35f;

            if (Widgets.ButtonText(new Rect(20f, y, 220f, 30f), "Mine Designated"))
            {
                ColonyServiceManager.MineDesignated(Find.CurrentMap);
            }

            Widgets.Label(
                new Rect(260f, y + 2f, 220f, 22f),
                $"{ModMain.Settings.mineDesignatedCost} Silver");

            Widgets.Label(
                new Rect(260f, y + 18f, 650f, 22f),
                "Mines every designated rock.");

            y += 45f;

            if (Widgets.ButtonText(new Rect(20f, y, 220f, 30f), "Finish Construction"))
            {
                ColonyServiceManager.FinishConstruction(Find.CurrentMap);
            }

            Widgets.Label(
                new Rect(260f, y + 2f, 220f, 22f),
                $"{ModMain.Settings.finishConstructionCost} Silver");

            Widgets.Label(
                new Rect(260f, y + 18f, 650f, 22f),
                "Instantly finishes every construction project.");

            y += 45f;

            if (Widgets.ButtonText(new Rect(20f, y, 220f, 30f), "Repair Buildings"))
            {
                ColonyServiceManager.RepairBuildings(Find.CurrentMap);
            }

            Widgets.Label(
                new Rect(260f, y + 2f, 220f, 22f),
                $"{ModMain.Settings.repairBuildingsCost} Silver");

            Widgets.Label(
                new Rect(260f, y + 18f, 650f, 22f),
                "Repairs every damaged building.");

            y += 55f;

            viewRect.height = y + 20f;

            Widgets.EndScrollView();
        }
    }
}