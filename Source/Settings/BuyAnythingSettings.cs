using Verse;
using System.Collections.Generic;

namespace BuyAnything.Source.Settings
{
    public class BuyAnythingSettings : ModSettings
    {
        // -------------------------
        // General
        // -------------------------

        public bool modEnabled = true;
        public bool instantDelivery = true;
        public bool requireResearch = true;
        public bool requirePower = true;
        public bool requirePawnInteraction = true;
        public bool pauseWhileOpen = false;

        // -------------------------
        // Economy
        // -------------------------

        public float buyPriceMultiplier = 1.0f;
        public float sellPriceMultiplier = 0.50f;
        public float tradeTax = 0f;
        public int silverPurchaseLimit = 0;
        public bool allowNegativeSilver = false;
        public List<string> favoriteItems = new List<string>();

        // -------------------------
        // Colony Services
        // -------------------------

        public bool enableColonyServices = true;

        public bool enableCleanColony = true;
        public int cleanColonyCost = 400;

        public bool enableRemoveBodies = true;
        public int removeBodiesCost = 600;

        public bool enableRemoveChunks = true;
        public int removeChunksCost = 1000;

        public bool enableAutoHaul = true;
        public int autoHaulCost = 2500;

        public bool enableHarvestTrees = true;
        public int harvestTreesCost = 2000;

        public bool enableHarvestCrops = true;
        public int harvestCropsCost = 1500;

        public bool enableMineDesignated = true;
        public int mineDesignatedCost = 3000;

        public bool enableFinishConstruction = true;
        public int finishConstructionCost = 4000;

        public bool enableRepairBuildings = true;
        public int repairBuildingsCost = 2500;

        public override void ExposeData()
        {
            base.ExposeData();

            // General
            Scribe_Values.Look(ref modEnabled, "modEnabled", true);
            Scribe_Values.Look(ref instantDelivery, "instantDelivery", true);
            Scribe_Values.Look(ref requireResearch, "requireResearch", true);
            Scribe_Values.Look(ref requirePower, "requirePower", true);
            Scribe_Values.Look(ref requirePawnInteraction, "requirePawnInteraction", true);
            Scribe_Values.Look(ref pauseWhileOpen, "pauseWhileOpen", false);
            Scribe_Collections.Look(
    ref favoriteItems,
    "favoriteItems",
    LookMode.Value);

            // Economy
            Scribe_Values.Look(ref buyPriceMultiplier, "buyPriceMultiplier", 1.0f);
            Scribe_Values.Look(ref sellPriceMultiplier, "sellPriceMultiplier", 0.50f);
            Scribe_Values.Look(ref tradeTax, "tradeTax", 0f);
            Scribe_Values.Look(ref silverPurchaseLimit, "silverPurchaseLimit", 0);
            Scribe_Values.Look(ref allowNegativeSilver, "allowNegativeSilver", false);

            // Colony Services
            Scribe_Values.Look(ref enableColonyServices, "enableColonyServices", true);

            Scribe_Values.Look(ref enableCleanColony, "enableCleanColony", true);
            Scribe_Values.Look(ref cleanColonyCost, "cleanColonyCost", 400);

            Scribe_Values.Look(ref enableRemoveBodies, "enableRemoveBodies", true);
            Scribe_Values.Look(ref removeBodiesCost, "removeBodiesCost", 600);

            Scribe_Values.Look(ref enableRemoveChunks, "enableRemoveChunks", true);
            Scribe_Values.Look(ref removeChunksCost, "removeChunksCost", 1000);

            Scribe_Values.Look(ref enableAutoHaul, "enableAutoHaul", true);
            Scribe_Values.Look(ref autoHaulCost, "autoHaulCost", 2500);

            Scribe_Values.Look(ref enableHarvestTrees, "enableHarvestTrees", true);
            Scribe_Values.Look(ref harvestTreesCost, "harvestTreesCost", 2000);

            Scribe_Values.Look(ref enableHarvestCrops, "enableHarvestCrops", true);
            Scribe_Values.Look(ref harvestCropsCost, "harvestCropsCost", 1500);

            Scribe_Values.Look(ref enableMineDesignated, "enableMineDesignated", true);
            Scribe_Values.Look(ref mineDesignatedCost, "mineDesignatedCost", 3000);

            Scribe_Values.Look(ref enableFinishConstruction, "enableFinishConstruction", true);
            Scribe_Values.Look(ref finishConstructionCost, "finishConstructionCost", 4000);

            Scribe_Values.Look(ref enableRepairBuildings, "enableRepairBuildings", true);
            Scribe_Values.Look(ref repairBuildingsCost, "repairBuildingsCost", 2500);
        }
    }
}