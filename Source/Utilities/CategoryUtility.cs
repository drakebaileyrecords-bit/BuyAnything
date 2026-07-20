using RimWorld;
using Verse;

namespace BuyAnything.Source.Utilities
{
    public static class CategoryUtility
    {
        public static string GetStoreCategory(ThingDef thing)
        {
            Log.Warning("[BuyAnything] CategoryUtility v2 loaded");

            if (thing == null)
                return "Misc";

            // Weapons
            if (thing.IsWeapon)
            {
                Log.Warning($"WEAPON: {thing.defName}");
                return "Weapons";
            }

            // Apparel (includes armor)
            if (thing.IsApparel)
                return "Apparel";

            // Food
            if (thing.IsIngestible)
                return "Food";

            // Medicine & drugs
            if (thing.IsMedicine || thing.IsDrug)
                return "Medicine";

            // Resources
            if (thing.category == ThingCategory.Item)
            {
                if (thing.IsStuff)
                    return "Resources";

                if (thing.defName.Contains("Block"))
                    return "Resources";

                switch (thing.defName)
                {
                    case "Steel":
                    case "Silver":
                    case "Gold":
                    case "Plasteel":
                    case "Uranium":
                    case "Jade":
                    case "WoodLog":
                    case "ComponentIndustrial":
                    case "ComponentSpacer":
                        return "Resources";
                }
            }

            // Buildings
            if (thing.category == ThingCategory.Building)
                return "Buildings";

            Log.Message(
                $"[BuyAnything] {thing.defName} | " +
                $"ThingCategory={thing.category} | " +
                $"FirstCategory={thing.FirstThingCategory?.LabelCap}");

            return "Misc";
        }
    }
}