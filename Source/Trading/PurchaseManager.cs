using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using BuyAnything.Source.Data;
using BuyAnything.Source.Settings;

namespace BuyAnything.Source.Trading
{
    public static class PurchaseManager
    {
        public static void Purchase()
        {
            // Empty cart?
            if (CartManager.Items.Count == 0)
            {
                Messages.Message(
                    "Your shopping cart is empty.",
                    MessageTypeDefOf.RejectInput,
                    false);
                return;
            }

            float totalCost = CartManager.GetTotalCost();
            int availableSilver = GetAvailableSilver();

            // Purchase limit
            if (ModMain.Settings.silverPurchaseLimit > 0 &&
                totalCost > ModMain.Settings.silverPurchaseLimit)
            {
                Messages.Message(
                    $"This purchase exceeds the configured limit of ${ModMain.Settings.silverPurchaseLimit}.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return;
            }

            // Enough silver?
            if (!ModMain.Settings.allowNegativeSilver &&
                availableSilver < totalCost)
            {
                Messages.Message(
                    $"Not enough silver.\nNeed: ${totalCost:F2}\nAvailable: ${availableSilver}",
                    MessageTypeDefOf.RejectInput,
                    false);

                return;
            }

            // Pay
            DeductSilver((int)totalCost);

            // Deliver purchased items
            if (ModMain.Settings.instantDelivery)
            {
                DeliverPurchasedItemsInstantly();
            }
            else
            {
                DeliverPurchasedItemsByCargoPods();
            }

            // Empty cart
            CartManager.Clear();

            Messages.Message(
                "Purchase complete!",
                MessageTypeDefOf.TaskCompletion,
                false);
        }

        private static int GetAvailableSilver()
        {
            Map map = Find.CurrentMap;

            if (map == null)
                return 0;

            return map.listerThings
                .ThingsOfDef(ThingDefOf.Silver)
                .Where(t => t.Spawned)
                .Sum(t => t.stackCount);
        }

        private static void DeductSilver(int amount)
        {
            Map map = Find.CurrentMap;

            if (map == null)
                return;

            var silverStacks = map.listerThings
                .ThingsOfDef(ThingDefOf.Silver)
                .Where(t => t.Spawned)
                .OrderBy(t => t.stackCount)
                .ToList();

            foreach (Thing silver in silverStacks)
            {
                if (amount <= 0)
                    break;

                if (silver.stackCount <= amount)
                {
                    amount -= silver.stackCount;
                    silver.Destroy(DestroyMode.Vanish);
                }
                else
                {
                    silver.stackCount -= amount;
                    amount = 0;
                }
            }
        }
        private static void DeliverPurchasedItemsInstantly()
        {
            Map map = Find.CurrentMap;

            if (map == null)
                return;

            IntVec3 dropSpot = FindSafeDropSpot(map);

            foreach (CartItem item in CartManager.Items)
            {
                int remaining = item.Quantity;
                int stackLimit = item.Def.stackLimit;

                while (remaining > 0)
                {
                    int stackSize = remaining > stackLimit
                        ? stackLimit
                        : remaining;

                    Thing thing;

                    if (item.Def.Minifiable)
                    {
                        Building building = (Building)ThingMaker.MakeThing(item.Def);
                        thing = building.MakeMinified();
                    }
                    else
                    {
                        thing = ThingMaker.MakeThing(item.Def);
                    }

                    thing.stackCount = stackSize;

                    GenPlace.TryPlaceThing(
                        thing,
                        dropSpot,
                        map,
                        ThingPlaceMode.Near);

                    remaining -= stackSize;
                }
            }
        }
        private static void DeliverPurchasedItemsByCargoPods()
        {
            Map map = Find.CurrentMap;

            if (map == null)
                return;

            List<Thing> thingsToDrop = new List<Thing>();

            foreach (CartItem item in CartManager.Items)
            {
                int remaining = item.Quantity;
                int stackLimit = item.Def.stackLimit;

                while (remaining > 0)
                {
                    int stackSize = remaining > stackLimit
                        ? stackLimit
                        : remaining;

                    Thing thing;

                    if (item.Def.Minifiable)
                    {
                        Building building = (Building)ThingMaker.MakeThing(item.Def);
                        thing = building.MakeMinified();
                    }
                    else
                    {
                        thing = ThingMaker.MakeThing(item.Def);
                        thing.stackCount = stackSize;
                    }

                    thingsToDrop.Add(thing);
                    thing.stackCount = stackSize;



                    remaining -= stackSize;
                }
            }

            IntVec3 dropCenter = FindSafeDropSpot(map);

            DropPodUtility.DropThingsNear(
                dropCenter,
                map,
                thingsToDrop,
                openDelay: 110,
                canInstaDropDuringInit: false,
                leaveSlag: false,
                canRoofPunch: true,
                forbid: false,
                allowFogged: false,
                faction: Faction.OfPlayer);
        }
    private static IntVec3 FindSafeDropSpot(Map map)
        {
            IntVec3 center = DropCellFinder.TradeDropSpot(map);

            if (IsSafeDropCell(center, map))
                return center;

            foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, 30f, true))
            {
                if (!cell.InBounds(map))
                    continue;

                if (IsSafeDropCell(cell, map))
                    return cell;
            }

            return center;
        }

        private static bool IsSafeDropCell(IntVec3 cell, Map map)
        {
            if (!cell.Standable(map))
                return false;

            if (cell.Roofed(map))
                return false;

            if (cell.GetEdifice(map) != null)
                return false;

            return true;
        }
    }
}