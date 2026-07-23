using Verse;
using UnityEngine;
using BuyAnything.Source.Settings;

namespace BuyAnything.Source
{
    public class ModMain : Mod
    {
        public static ModMain Instance { get; private set; }
        public static BuyAnythingSettings Settings;
        private Vector2 scrollPosition = Vector2.zero;
        private float scrollViewHeight = 2200f;

        public ModMain(ModContentPack content) : base(content)
        {
            Instance = this;
            Settings = GetSettings<BuyAnythingSettings>();

            Log.Message("[BuyAnything] Mod loaded successfully!");
        }

        public override string SettingsCategory()
        {
            return "BuyAnything";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, scrollViewHeight);

            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            // =====================================================
            // General
            // =====================================================

            listing.Label("General");
            listing.GapLine();

            listing.CheckboxLabeled(
                "Enable BuyAnything",
                ref Settings.modEnabled,
                "Enable or disable the BuyAnything mod.");

            listing.CheckboxLabeled(
                "Instant Delivery",
                ref Settings.instantDelivery,
                "Purchased items appear instantly instead of arriving by transport.");

            listing.CheckboxLabeled(
                "Require Research",
                ref Settings.requireResearch,
                "Require the Merchant Terminal research project.");

            listing.CheckboxLabeled(
                "Require Power",
                ref Settings.requirePower,
                "Merchant terminals require power.");

            listing.CheckboxLabeled(
                "Require Pawn Interaction",
                ref Settings.requirePawnInteraction,
                "A colonist must use the terminal.");

            listing.CheckboxLabeled(
                "Pause While Window Is Open",
                ref Settings.pauseWhileOpen,
                "Automatically pause the game while shopping.");

            listing.Gap(18f);

            // =====================================================
            // Economy
            // =====================================================

            listing.Label("Economy");
            listing.GapLine();

            listing.Label($"Buy Price Multiplier: {Settings.buyPriceMultiplier:F2}x");
            Settings.buyPriceMultiplier =
                listing.Slider(Settings.buyPriceMultiplier, 0.10f, 10.00f);

            listing.Gap();

            listing.Label($"Sell Price Multiplier: {Settings.sellPriceMultiplier:F2}x");
            Settings.sellPriceMultiplier =
                listing.Slider(Settings.sellPriceMultiplier, 0.10f, 10.00f);

            listing.Gap();

            listing.Label($"Trade Tax: {Settings.tradeTax:F0}%");
            Settings.tradeTax =
                listing.Slider(Settings.tradeTax, 0f, 100f);

            listing.Gap();

            listing.Label("Silver Purchase Limit (0 = Unlimited)");

            string limitBuffer = Settings.silverPurchaseLimit.ToString();
            limitBuffer = listing.TextEntry(limitBuffer);

            if (int.TryParse(limitBuffer, out int value))
            {
                Settings.silverPurchaseLimit = value;
            }

            listing.Gap();

            listing.CheckboxLabeled(
                "Allow Purchases Without Enough Silver (Cheat)",
                ref Settings.allowNegativeSilver,
                "Allows purchases even if your colony cannot afford them.");
            listing.Gap(18f);

            // =====================================================
            // Colony Services
            // =====================================================

            listing.Label("Colony Services");
            listing.GapLine();

            listing.CheckboxLabeled(
                "Enable Colony Services",
                ref Settings.enableColonyServices,
                "Enable or disable colony service features.");

            listing.Gap();

            // Clean Colony

            listing.CheckboxLabeled(
                "Enable Clean Colony",
                ref Settings.enableCleanColony,
                "Adds the Clean Colony service.");

            listing.Label($"Clean Colony Cost: {Settings.cleanColonyCost} silver");

            Settings.cleanColonyCost = Mathf.RoundToInt(
                listing.Slider(Settings.cleanColonyCost, 0, 5000));

            listing.Gap();

            // Remove Bodies

            listing.CheckboxLabeled(
                "Enable Remove Bodies",
                ref Settings.enableRemoveBodies,
                "Adds the Remove Bodies service.");

            listing.Label($"Remove Bodies Cost: {Settings.removeBodiesCost} silver");

            Settings.removeBodiesCost = Mathf.RoundToInt(
                listing.Slider(Settings.removeBodiesCost, 0, 5000));

            listing.Gap();

            // Remove Chunks

            listing.CheckboxLabeled(
                "Enable Remove Chunks",
                ref Settings.enableRemoveChunks,
                "Adds the Remove Chunks service.");

            listing.Label($"Remove Chunks Cost: {Settings.removeChunksCost} silver");

            Settings.removeChunksCost = Mathf.RoundToInt(
                listing.Slider(Settings.removeChunksCost, 0, 5000));

            listing.Gap();

            // Auto Haul

            listing.CheckboxLabeled(
                "Enable Auto Haul",
                ref Settings.enableAutoHaul,
                "Adds the Auto Haul service.");

            listing.Label($"Auto Haul Cost: {Settings.autoHaulCost} silver");

            Settings.autoHaulCost = Mathf.RoundToInt(
                listing.Slider(Settings.autoHaulCost, 0, 10000));

            listing.Gap();
            // Harvest Trees

            listing.CheckboxLabeled(
                "Enable Harvest Trees",
                ref Settings.enableHarvestTrees,
                "Adds the Harvest Trees service.");

            listing.Label($"Harvest Trees Cost: {Settings.harvestTreesCost} silver");

            Settings.harvestTreesCost = Mathf.RoundToInt(
                listing.Slider(Settings.harvestTreesCost, 0, 10000));

            listing.Gap();

            // Harvest Crops

            listing.CheckboxLabeled(
                "Enable Harvest Crops",
                ref Settings.enableHarvestCrops,
                "Adds the Harvest Crops service.");

            listing.Label($"Harvest Crops Cost: {Settings.harvestCropsCost} silver");

            Settings.harvestCropsCost = Mathf.RoundToInt(
                listing.Slider(Settings.harvestCropsCost, 0, 10000));

            listing.Gap();

            // Mine Designated

            listing.CheckboxLabeled(
                "Enable Mine Designated",
                ref Settings.enableMineDesignated,
                "Adds the Mine Designated service.");

            listing.Label($"Mine Designated Cost: {Settings.mineDesignatedCost} silver");

            Settings.mineDesignatedCost = Mathf.RoundToInt(
                listing.Slider(Settings.mineDesignatedCost, 0, 10000));

            listing.Gap();

            // Finish Construction

            listing.CheckboxLabeled(
                "Enable Finish Construction",
                ref Settings.enableFinishConstruction,
                "Adds the Finish Construction service.");

            listing.Label($"Finish Construction Cost: {Settings.finishConstructionCost} silver");

            Settings.finishConstructionCost = Mathf.RoundToInt(
                listing.Slider(Settings.finishConstructionCost, 0, 10000));

            listing.Gap();

            // Repair Buildings

            listing.CheckboxLabeled(
                "Enable Repair Buildings",
                ref Settings.enableRepairBuildings,
                "Adds the Repair Buildings service.");

            listing.Label($"Repair Buildings Cost: {Settings.repairBuildingsCost} silver");

            Settings.repairBuildingsCost = Mathf.RoundToInt(
                listing.Slider(Settings.repairBuildingsCost, 0, 10000));

            listing.Gap();

            listing.End();

            scrollViewHeight = listing.CurHeight + 30f;

            Widgets.EndScrollView();

            base.DoSettingsWindowContents(inRect);
        }
    }
}