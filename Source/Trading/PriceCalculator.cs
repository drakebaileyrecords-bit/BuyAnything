using Verse;
using BuyAnything.Source.Settings;

namespace BuyAnything.Source.Trading
{
    public static class PriceCalculator
    {
        public static float GetMarketValue(ThingDef thing)
        {
            return thing?.BaseMarketValue ?? 0f;
        }

        public static float GetBuyPrice(ThingDef thing)
        {
            float price = GetMarketValue(thing);

            price *= ModMain.Settings.buyPriceMultiplier;
            price *= 1f + (ModMain.Settings.tradeTax / 100f);

            return price;
        }

        public static float GetSellPrice(ThingDef thing)
        {
            float price = GetMarketValue(thing);

            price *= ModMain.Settings.sellPriceMultiplier;
            price *= 1f - (ModMain.Settings.tradeTax / 100f);

            return price;
        }
    }
}