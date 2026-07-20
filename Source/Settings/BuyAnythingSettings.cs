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
        }
    }
}