using System.Collections.Generic;
using RimWorld;
using Verse;
using BuyAnything.Source.Data;
using BuyAnything.Source.Settings;

namespace BuyAnything.Source.Trading
{
    public static class SellManager
    {
        public static void Sell()
        {
            if (!CartManager.HasSellItems())
            {
                Messages.Message(
                    "Your sell cart is empty.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return;
            }

            Map map = Find.CurrentMap;

            if (map == null)
                return;

            int totalSilver = 0;

            foreach (CartItem item in CartManager.SellItems())
            {
                int available = 0;

                foreach (Thing thing in item.SourceThings)
                {
                    if (thing == null)
                        continue;

                    if (thing.Destroyed)
                        continue;

                    available += thing.stackCount;
                }

                int amountToSell = item.Quantity;

                if (amountToSell > available)
                    amountToSell = available;

                if (amountToSell <= 0)
                    continue;

                totalSilver += (int)(
    amountToSell *
    item.UnitPrice *
    ModMain.Settings.sellPriceMultiplier);

                CartItem actualSale = new CartItem(
                    item.Def,
                    amountToSell,
                    item.UnitPrice,
                    item.SourceThings);

                RemoveItems(actualSale);
            }

            GiveSilver(map, totalSilver);

            CartManager.Clear();

            Messages.Message(
                "Sale complete!",
                MessageTypeDefOf.TaskCompletion,
                false);
        }

        private static void RemoveItems(CartItem cartItem)
        {
            int remaining = cartItem.Quantity;

            foreach (Thing thing in cartItem.SourceThings)
            {
                if (thing == null)
                    continue;

                if (thing.Destroyed)
                    continue;

                if (remaining <= 0)
                    break;

                if (thing.stackCount <= remaining)
                {
                    remaining -= thing.stackCount;
                    thing.Destroy(DestroyMode.Vanish);
                }
                else
                {
                    Thing split = thing.SplitOff(remaining);
                    split.Destroy(DestroyMode.Vanish);
                    remaining = 0;
                }
            }
        }

        private static void GiveSilver(Map map, int amount)
        {
            List<Thing> things = new List<Thing>();

            while (amount > 0)
            {
                int stackSize = amount > 750 ? 750 : amount;

                Thing silver = ThingMaker.MakeThing(ThingDefOf.Silver);
                silver.stackCount = stackSize;

                things.Add(silver);

                amount -= stackSize;
            }

            DropPodUtility.DropThingsNear(
                DropCellFinder.TradeDropSpot(map),
                map,
                things,
                110,
                false,
                true,
                true);
        }
    }
}