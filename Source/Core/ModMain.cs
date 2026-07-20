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
        private float scrollViewHeight = 0f;

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
            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, 650f);

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

            listing.End();

            scrollViewHeight = listing.CurHeight + 20f;

            Widgets.EndScrollView();
        }
    }
}