using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using BuyAnything.Source.Data;

namespace BuyAnything.Source.Trading
{
    public static class MerchantDatabase
    {
        public static List<MerchantItem> GetAllItems()
        {
            return DefDatabase<ThingDef>.AllDefs
                .Where(IsPurchasable)
                .Select(t => new MerchantItem
                {
                    Thing = t
                })
                .OrderBy(t => t.Name)
                .ToList();
        }

        private static bool IsPurchasable(ThingDef t)
        {
            if (t == null)
                return false;

            if (t.BaseMarketValue <= 0f)
                return false;

            if (t.destroyOnDrop)
                return false;

            if (t.IsCorpse)
                return false;

            if (t.IsBlueprint || t.IsFrame)
                return false;

            // Hide all chunks from the Buy menu
            if (t.thingCategories != null &&
                t.thingCategories.Contains(ThingCategoryDefOf.Chunks))
                return false;

            // Hide all Ancient furniture/buildings/items
            if (t.defName.StartsWith("Ancient"))
                return false;

            // Hide ship chunks and ship parts
            if (t.defName.StartsWith("ShipChunk") ||
                t.defName.StartsWith("ShipPart"))
                return false;

            // Hide natural rock
            if (t.building != null && t.building.isNaturalRock)
                return false;

            return true;
        }
    }
}