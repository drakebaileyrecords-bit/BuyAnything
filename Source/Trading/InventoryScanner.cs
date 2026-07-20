using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using BuyAnything.Source.Data;

namespace BuyAnything.Source.Trading
{
    public static class InventoryScanner
    {
        public static List<MerchantItem> GetSellableItems()
        {
            Map map = Find.CurrentMap;

            if (map == null)
            {
                return new List<MerchantItem>();
            }

            Dictionary<ThingDef, MerchantItem> items =
                new Dictionary<ThingDef, MerchantItem>();

            foreach (Thing thing in map.listerThings.AllThings)
            {
                if (!IsSellable(thing))
                    continue;

                MerchantItem merchantItem;

                if (!items.TryGetValue(thing.def, out merchantItem))
                {
                    merchantItem = new MerchantItem(
                        thing.def,
                        0,
                        new List<Thing>());

                    items.Add(thing.def, merchantItem);
                }

                merchantItem.Quantity += thing.stackCount;
                merchantItem.SourceThings.Add(thing);
            }

            return items.Values
                .OrderBy(i => i.Name)
                .ToList();
        }

        private static bool IsSellable(Thing thing)

        {
            if (thing is Pawn)
                return false;

            if (thing.def.race != null)
                return false;
            bool isItem = thing.def.category == ThingCategory.Item;
            bool isBuilding = thing.def.category == ThingCategory.Building;

            if (!isItem && !isBuilding)
                return false;

            if (isBuilding && !thing.def.Minifiable)
                return false;

            if (thing == null)
                return false;

            if (!thing.Spawned)
                return false;

            if (thing.Destroyed)
                return false;

            if (thing.Map == null)
                return false;
            if (thing.ParentHolder != null &&
    thing.ParentHolder != thing.Map)
                return false;

            if (thing.Faction != null &&
                thing.Faction != Faction.OfPlayer)
                return false;

            if (thing.def == ThingDefOf.Silver)
                return false;

            if (thing.def.BaseMarketValue <= 0f)
                return false;

            if (thing.def.destroyOnDrop)
                return false;

            if (thing.def.IsCorpse)
                return false;

            return true;
        }
    }
}