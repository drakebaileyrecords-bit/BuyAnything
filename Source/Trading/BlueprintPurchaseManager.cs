using System.Collections.Generic;
using BuyAnything.Source.Data;
using BuyAnything.Source.UI;
using RimWorld;
using Verse;

namespace BuyAnything.Source.Trading
{
    public static class BlueprintPurchaseManager
    {
        public static void AddMissingMaterialsToCart()
        {
            List<BlueprintMaterial> report =
                GetShoppingReport();

            if (report.Count == 0)
            {
                Messages.Message(
                    "No missing construction materials found.",
                    MessageTypeDefOf.NeutralEvent,
                    false);

                return;
            }

            Find.WindowStack.Add(
                new BlueprintReportWindow(report));
        }

        public static List<BlueprintMaterial> GetShoppingReport()
        {
            Map map = Find.CurrentMap;

            List<BlueprintMaterial> report =
                new List<BlueprintMaterial>();

            if (map == null)
                return report;

            Dictionary<ThingDef, int> required =
                GetRequiredMaterials(map);

            if (required.Count == 0)
            {
                Log.Message(
                    "[BuyAnything] No blueprint materials detected.");
                return report;
            }

            SubtractDeliveredFrameMaterials(
                map,
                required);

            Dictionary<ThingDef, int> available =
                GetAvailableMaterials(map);

            foreach (KeyValuePair<ThingDef, int> pair in required)
            {
                available.TryGetValue(
                    pair.Key,
                    out int owned);

                int missing = pair.Value - owned;

                if (missing <= 0)
                    continue;

                report.Add(new BlueprintMaterial
                {
                    Thing = pair.Key,
                    Required = pair.Value,
                    Owned = owned,
                    Missing = missing,
                    UnitPrice = PriceCalculator.GetBuyPrice(pair.Key)
                });
            }

            return report;
        }

        private static Dictionary<ThingDef, int> GetRequiredMaterials(Map map)
        {
            Dictionary<ThingDef, int> materials =
                new Dictionary<ThingDef, int>();

            foreach (Thing thing in map.listerThings.AllThings)
            {
                BuildableDef buildable = null;
                ThingDef stuff = null;

                if (thing is Blueprint blueprint)
                {
                    buildable = blueprint.EntityToBuild();
                    stuff = blueprint.EntityToBuildStuff();
                }
                else if (thing is Frame frame)
                {
                    buildable = frame.def.entityDefToBuild;
                    stuff = frame.EntityToBuildStuff();
                }
                else
                {
                    continue;
                }

                if (buildable == null)
                    continue;

                if (buildable is TerrainDef terrain)
                {
                    if (terrain.costList != null)
                    {
                        foreach (ThingDefCountClass cost in terrain.costList)
                        {
                            if (!materials.ContainsKey(cost.thingDef))
                                materials[cost.thingDef] = 0;

                            materials[cost.thingDef] += cost.count;
                        }
                    }

                    continue;
                }

                if (!(buildable is ThingDef building))
                    continue;

                if (stuff != null &&
                    building.CostStuffCount > 0)
                {
                    if (!materials.ContainsKey(stuff))
                    {
                        materials[stuff] = 0;
                    }

                    materials[stuff] +=
                        building.CostStuffCount;
                }
                if (building.costList != null)
                {
                    foreach (ThingDefCountClass cost in building.costList)
                    {
                        if (!materials.ContainsKey(cost.thingDef))
                        {
                            materials[cost.thingDef] = 0;
                        }

                        materials[cost.thingDef] += cost.count;
                    }
                }
            }

            if (Prefs.DevMode)
            {
                Log.Message(
                    $"[BuyAnything] Found {materials.Count} required material types.");
            }

            return materials;
        }

        private static Dictionary<ThingDef, int> GetAvailableMaterials(Map map)
        {
            Dictionary<ThingDef, int> inventory =
                new Dictionary<ThingDef, int>();

            foreach (Thing thing in map.listerThings.AllThings)
            {
                if (thing.Destroyed)
                    continue;

                if (!thing.Spawned)
                    continue;

                if (thing.Position.Fogged(map))
                    continue;

                if (!thing.def.EverHaulable)
                    continue;

                if (!thing.IsInAnyStorage())
                    continue;

                if (thing.Faction != null &&
                    thing.Faction != Faction.OfPlayer)
                    continue;

                if (!inventory.ContainsKey(thing.def))
                {
                    inventory[thing.def] = 0;
                }

                inventory[thing.def] += thing.stackCount;
            }

            if (Prefs.DevMode)
            {
                Log.Message(
                    $"[BuyAnything] Found {inventory.Count} available material types.");
            }

            return inventory;
        }

        private static void SubtractDeliveredFrameMaterials(
            Map map,
            Dictionary<ThingDef, int> requiredMaterials)
        {
            foreach (Thing thing in map.listerThings.AllThings)
            {
                if (!(thing is Frame frame))
                    continue;

                if (frame.resourceContainer == null)
                    continue;

                foreach (Thing resource in frame.resourceContainer)
                {
                    if (!requiredMaterials.ContainsKey(resource.def))
                        continue;

                    requiredMaterials[resource.def] -= resource.stackCount;

                    if (requiredMaterials[resource.def] < 0)
                    {
                        requiredMaterials[resource.def] = 0;
                    }

                    if (Prefs.DevMode)
                    {
                        Log.Message(
                            $"[BuyAnything] Frame already contains {resource.stackCount}x {resource.def.defName}");
                    }
                }
            }

            if (Prefs.DevMode)
            {
                foreach (KeyValuePair<ThingDef, int> pair in requiredMaterials)
                {
                    Log.Message(
                        $"[BuyAnything] Remaining Required: {pair.Key.defName} = {pair.Value}");
                }
            }
        }
    }
}