using BuyAnything.Source;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WorkbenchMerchant2.Source.Trading
{
    public static class ColonyServiceManager
    {
        public const int CleanColonyCost = 400;
        public const int RemoveBodiesCost = 600;
        public const int RemoveChunksCost = 1000;
        public const int AutoHaulCost = 2500;

        /// <summary>
        /// Cleans all filth from the current map for a fixed silver cost.
        /// </summary>
        /// <param name="map">The map to clean.</param>
        /// <returns>True if the service completed successfully.</returns>
        public static bool CleanColony(Map map)
        {
            if (map == null)
            {
                Messages.Message(
                    "No active map found.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            var silverStacks = map.listerThings.ThingsOfDef(ThingDefOf.Silver);

            int totalSilver = 0;

            foreach (Thing silver in silverStacks)
            {
                if (silver.Faction == Faction.OfPlayer || silver.Faction == null)
                {
                    totalSilver += silver.stackCount;
                }
            }

            if (totalSilver < CleanColonyCost)
            {
                Messages.Message(
                    $"You need {CleanColonyCost} silver to hire a cleaning crew.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            int silverToRemove = CleanColonyCost;

            foreach (Thing silver in silverStacks.ToList())
            {
                if (silver.Faction != Faction.OfPlayer && silver.Faction != null)
                    continue;

                if (silver.stackCount <= silverToRemove)
                {
                    silverToRemove -= silver.stackCount;
                    silver.Destroy();
                }
                else
                {
                    silver.SplitOff(silverToRemove);
                    silverToRemove = 0;
                }

                if (silverToRemove <= 0)
                    break;
            }

            var filthList = map.listerThings.ThingsInGroup(ThingRequestGroup.Filth);

            int filthRemoved = 0;

            // Copy the list first since we'll be modifying it.
            foreach (Thing filth in filthList.ToList())
            {
                filth.Destroy(DestroyMode.Vanish);
                filthRemoved++;
            }

            Messages.Message(
                $"Professional cleaning crew removed {filthRemoved} filth.",
                MessageTypeDefOf.PositiveEvent,
                false);

            return true;
        }

        public static bool RemoveBodies(Map map)
        {
            if (map == null)
            {
                Messages.Message(
                    "No active map found.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            var silverStacks = map.listerThings.ThingsOfDef(ThingDefOf.Silver);

            int totalSilver = 0;

            foreach (Thing silver in silverStacks)
            {
                if (silver.Faction == Faction.OfPlayer || silver.Faction == null)
                {
                    totalSilver += silver.stackCount;
                }
            }

            if (totalSilver < RemoveBodiesCost)
            {
                Messages.Message(
                    $"You need {RemoveBodiesCost} silver to hire a cleanup crew.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            int silverToRemove = RemoveBodiesCost;

            foreach (Thing silver in silverStacks.ToList())
            {
                if (silver.Faction != Faction.OfPlayer && silver.Faction != null)
                    continue;

                if (silver.stackCount <= silverToRemove)
                {
                    silverToRemove -= silver.stackCount;
                    silver.Destroy();
                }
                else
                {
                    silver.SplitOff(silverToRemove);
                    silverToRemove = 0;
                }

                if (silverToRemove <= 0)
                    break;
            }

            int bodiesRemoved = 0;

            foreach (Thing thing in map.listerThings.AllThings.ToList())
            {
                if (thing is Corpse)
                {
                    thing.Destroy(DestroyMode.Vanish);
                    bodiesRemoved++;
                }
            }

            Messages.Message(
                $"Cleanup crew removed {bodiesRemoved} bodies.",
                MessageTypeDefOf.PositiveEvent,
                false);

            return true;
        }

        public static bool RemoveChunks(Map map)
        {
            if (map == null)
            {
                Messages.Message(
                    "No active map found.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            var silverStacks = map.listerThings.ThingsOfDef(ThingDefOf.Silver);

            int totalSilver = 0;

            foreach (Thing silver in silverStacks)
            {
                if (silver.Faction == Faction.OfPlayer || silver.Faction == null)
                {
                    totalSilver += silver.stackCount;
                }
            }

            if (totalSilver < RemoveChunksCost)
            {
                Messages.Message(
                    $"You need {RemoveChunksCost} silver to hire a heavy equipment crew.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            int silverToRemove = RemoveChunksCost;

            foreach (Thing silver in silverStacks.ToList())
            {
                if (silver.Faction != Faction.OfPlayer && silver.Faction != null)
                    continue;

                if (silver.stackCount <= silverToRemove)
                {
                    silverToRemove -= silver.stackCount;
                    silver.Destroy();
                }
                else
                {
                    silver.SplitOff(silverToRemove);
                    silverToRemove = 0;
                }

                if (silverToRemove <= 0)
                    break;
            }

            int chunksRemoved = 0;

            foreach (Thing thing in map.listerThings.AllThings.ToList())
            {
                if ((thing.def.thingCategories != null &&
     thing.def.thingCategories.Contains(ThingCategoryDefOf.StoneChunks))
    || thing.def == ThingDefOf.ChunkSlagSteel)
                {
                    thing.Destroy(DestroyMode.Vanish);
                    chunksRemoved++;
                }
            }

            Messages.Message(
                $"Heavy equipment crew removed {chunksRemoved} chunks.",
                MessageTypeDefOf.PositiveEvent,
                false);

            return true;
        }

        public static bool AutoHaul(Map map)
        {
            return AutoHaulService.Execute(map);
        }
        public static bool HarvestTrees(Map map)
        {
            if (map == null)
            {
                Messages.Message(
                    "No active map found.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            const int HarvestTreesCost = 2000;

            List<Thing> silverStacks = map.listerThings.ThingsOfDef(ThingDefOf.Silver);

            int totalSilver = silverStacks
                .Where(s => s.Faction == Faction.OfPlayer || s.Faction == null)
                .Sum(s => s.stackCount);

            if (totalSilver < HarvestTreesCost)
            {
                Messages.Message(
                    $"You need {HarvestTreesCost} silver to hire a logging crew.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            int silverToRemove = HarvestTreesCost;

            foreach (Thing silver in silverStacks.ToList())
            {
                if (silver.Faction != Faction.OfPlayer && silver.Faction != null)
                    continue;

                if (silver.stackCount <= silverToRemove)
                {
                    silverToRemove -= silver.stackCount;
                    silver.Destroy();
                }
                else
                {
                    silver.SplitOff(silverToRemove);
                    silverToRemove = 0;
                }

                if (silverToRemove <= 0)
                    break;
            }

            int treesCut = 0;

            List<Designation> designations = map.designationManager.AllDesignations.ToList();

            foreach (Designation designation in designations)
            {
                Plant plant = designation.target.Thing as Plant;

                if (plant == null)
                    continue;

                if (!plant.def.plant.IsTree)
                    continue;

                if (designation.def != DesignationDefOf.CutPlant &&
                    designation.def != DesignationDefOf.HarvestPlant)
                    continue;

                // Spawn wood.
                Thing wood = ThingMaker.MakeThing(ThingDefOf.WoodLog);
                wood.stackCount = 60;

                GenPlace.TryPlaceThing(
                    wood,
                    plant.Position,
                    map,
                    ThingPlaceMode.Near);

                // Remove the designation.
                map.designationManager.RemoveDesignation(designation);

                // Destroy the tree.
                plant.Destroy(DestroyMode.Vanish);

                treesCut++;
            }

            Messages.Message(
                $"Logging crew cut {treesCut} trees.",
                MessageTypeDefOf.PositiveEvent,
                false);

            return true;
        }

        public static bool HarvestCrops(Map map)
        {
            if (map == null)
            {
                Messages.Message(
                    "No active map found.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            const int HarvestCropsCost = 1500;

            var silverStacks = map.listerThings.ThingsOfDef(ThingDefOf.Silver);

            int totalSilver = 0;

            foreach (Thing silver in silverStacks)
            {
                if (silver.Faction == Faction.OfPlayer || silver.Faction == null)
                    totalSilver += silver.stackCount;
            }

            if (totalSilver < HarvestCropsCost)
            {
                Messages.Message(
                    $"You need {HarvestCropsCost} silver to hire a harvesting crew.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            int silverToRemove = HarvestCropsCost;

            foreach (Thing silver in silverStacks.ToList())
            {
                if (silver.Faction != Faction.OfPlayer && silver.Faction != null)
                    continue;

                if (silver.stackCount <= silverToRemove)
                {
                    silverToRemove -= silver.stackCount;
                    silver.Destroy();
                }
                else
                {
                    silver.SplitOff(silverToRemove);
                    silverToRemove = 0;
                }

                if (silverToRemove <= 0)
                    break;
            }

            int cropsHarvested = 0;

            foreach (Thing thing in map.listerThings.AllThings.ToList())
            {
                Plant plant = thing as Plant;

                if (plant == null)
                    continue;

                if (plant.def.plant.IsTree)
                    continue;

                if (!plant.HarvestableNow)
                    continue;

                bool designated =
                    map.designationManager.DesignationOn(
                        plant,
                        DesignationDefOf.HarvestPlant) != null;

                bool growingZone =
                    map.zoneManager.ZoneAt(plant.Position) is Zone_Growing;

                if (!designated && !growingZone)
                    continue;

                int yield = plant.YieldNow();

                if (yield > 0 && plant.def.plant.harvestedThingDef != null)
                {
                    Thing harvestedThing =
                        ThingMaker.MakeThing(
                            plant.def.plant.harvestedThingDef);

                    harvestedThing.stackCount = yield;

                    GenPlace.TryPlaceThing(
                        harvestedThing,
                        plant.Position,
                        map,
                        ThingPlaceMode.Near);
                }

                Pawn worker = map.mapPawns.FreeColonistsSpawned.FirstOrDefault();

                if (worker == null)
                    continue;

                plant.PlantCollected(worker, PlantDestructionMode.Cut);

                cropsHarvested++;
            }

            Messages.Message(
                $"Harvest crew harvested {cropsHarvested} crops.",
                MessageTypeDefOf.PositiveEvent,
                false);

            return true;
        }

        public static bool MineDesignated(Map map)
        {
            if (map == null)
            {
                Messages.Message(
                    "No active map found.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            const int MineCost = 3000;

            var silverStacks = map.listerThings.ThingsOfDef(ThingDefOf.Silver);

            int totalSilver = 0;

            foreach (Thing silver in silverStacks)
            {
                if (silver.Faction == Faction.OfPlayer || silver.Faction == null)
                    totalSilver += silver.stackCount;
            }

            if (totalSilver < MineCost)
            {
                Messages.Message(
                    $"You need {MineCost} silver to hire a mining crew.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            int silverToRemove = MineCost;

            foreach (Thing silver in silverStacks.ToList())
            {
                if (silver.Faction != Faction.OfPlayer && silver.Faction != null)
                    continue;

                if (silver.stackCount <= silverToRemove)
                {
                    silverToRemove -= silver.stackCount;
                    silver.Destroy();
                }
                else
                {
                    silver.SplitOff(silverToRemove);
                    silverToRemove = 0;
                }

                if (silverToRemove <= 0)
                    break;
            }

            int mined = 0;

            foreach (Thing thing in map.listerThings.AllThings.ToList())
            {
                Mineable mineable = thing as Mineable;

                if (mineable == null)
                    continue;

                Designation designation =
    map.designationManager.DesignationAt(
        mineable.Position,
        DesignationDefOf.Mine);

                if (designation == null)
                    continue;

                mineable.DestroyMined(null);

                mined++;
            }

            Messages.Message(
                $"Mining crew mined {mined} rocks.",
                MessageTypeDefOf.PositiveEvent,
                false);

            return true;
        }

        public static bool FinishConstruction(Map map)
        {
            if (!ModMain.Settings.enableFinishConstruction)
            {
                Messages.Message(
                    "Finish Construction is disabled.",
                    MessageTypeDefOf.RejectInput,
                    false);
                return false;
            }

            if (map == null)
                return false;

            int serviceCost = ModMain.Settings.finishConstructionCost;

            //----------------------------------------------------
            // Verify silver
            //----------------------------------------------------

            List<Thing> silverStacks = map.listerThings
                .ThingsOfDef(ThingDefOf.Silver)
                .Where(x => x.Faction == Faction.OfPlayer || x.Faction == null)
                .ToList();

            int totalSilver = silverStacks.Sum(x => x.stackCount);

            if (totalSilver < serviceCost)
            {
                Messages.Message(
                    $"You need {serviceCost} silver.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            int completed = 0;

            //----------------------------------------------------
            // FINISH BLUEPRINTS
            //----------------------------------------------------

            List<Blueprint> blueprints = map.listerThings.AllThings
                .OfType<Blueprint>()
                .ToList();

            foreach (Blueprint blueprint in blueprints)
            {
                if (blueprint.Destroyed)
                    continue;

                //------------------------------------------------
                // TERRAIN (Floors)
                //------------------------------------------------

                if (blueprint.def.entityDefToBuild is TerrainDef terrain)
                {
                    map.terrainGrid.SetTerrain(
                        blueprint.Position,
                        terrain);

                    blueprint.Destroy(DestroyMode.Vanish);

                    completed++;
                    continue;
                }

                //------------------------------------------------
                // BUILDINGS / FURNITURE
                //------------------------------------------------

                ThingDef buildDef =
                    blueprint.def.entityDefToBuild as ThingDef;

                if (buildDef == null)
                    continue;

                RemoveBlockingThings(map, blueprint);

                ConsumeMaterials(map, blueprint.TotalMaterialCost());

                blueprint.Destroy(DestroyMode.Vanish);

                ThingDef stuff = blueprint.EntityToBuildStuff();

                Thing building = ThingMaker.MakeThing(buildDef, stuff);

                building.SetFactionDirect(Faction.OfPlayer);

                GenSpawn.Spawn(
                    building,
                    blueprint.Position,
                    map,
                    blueprint.Rotation);

                completed++;
            }

            //----------------------------------------------------
            // FINISH FRAMES
            //----------------------------------------------------

            List<Frame> frames = map.listerThings.AllThings
                .OfType<Frame>()
                .ToList();

            foreach (Frame frame in frames)
            {
                if (frame.Destroyed)
                    continue;

                ThingDef buildDef = frame.def.entityDefToBuild as ThingDef;

                if (buildDef == null)
                    continue;

                RemoveBlockingThings(map, frame);

                if (buildDef.CostList != null)
                {
                    foreach (ThingDefCountClass cost in buildDef.CostList)
                    {
                        int stillNeeded = frame.ThingCountNeeded(cost.thingDef);

                        if (stillNeeded > 0)
                        {
                            RemoveMaterial(map, cost.thingDef, stillNeeded);
                        }
                    }
                }

                frame.Destroy(DestroyMode.Vanish);

                ThingDef stuff = frame.EntityToBuildStuff();

                Thing building = ThingMaker.MakeThing(buildDef, stuff);

                GenSpawn.Spawn(
                    building,
                    frame.Position,
                    map,
                    frame.Rotation);

                completed++;
            }

            //----------------------------------------------------
            // REMOVE SILVER
            //----------------------------------------------------

            int remaining = serviceCost;

            foreach (Thing stack in silverStacks)
            {
                if (remaining <= 0)
                    break;

                if (stack.stackCount <= remaining)
                {
                    remaining -= stack.stackCount;
                    stack.Destroy(DestroyMode.Vanish);
                }
                else
                {
                    stack.SplitOff(remaining)
                        .Destroy(DestroyMode.Vanish);

                    remaining = 0;
                }
            }

            Messages.Message(
                $"Construction crew completed {completed} projects.",
                MessageTypeDefOf.PositiveEvent,
                false);

            return true;
        }
        private static void RemoveBlockingThings(Map map, Thing thing)
        {
            foreach (IntVec3 cell in thing.OccupiedRect())
            {
                foreach (Thing other in cell.GetThingList(map).ToList())
                {
                    if (other == thing)
                        continue;

                    if (other is Plant)
                    {
                        other.Destroy(DestroyMode.Vanish);
                        continue;
                    }

                    if (other.def.EverHaulable)
                    {
                        other.Destroy(DestroyMode.Vanish);
                    }
                }
            }
        }
        private static void ConsumeMaterials(
    Map map,
    List<ThingDefCountClass> costs)
        {
            if (costs == null)
                return;

            foreach (ThingDefCountClass cost in costs)
            {
                int remaining = cost.count;

                foreach (Thing stack in map.listerThings
                    .ThingsOfDef(cost.thingDef)
                    .Where(t => t.Faction == Faction.OfPlayer || t.Faction == null)
                    .ToList())
                {
                    if (remaining <= 0)
                        break;

                    if (stack.stackCount <= remaining)
                    {
                        remaining -= stack.stackCount;
                        stack.Destroy(DestroyMode.Vanish);
                    }
                    else
                    {
                        stack.SplitOff(remaining)
                            .Destroy(DestroyMode.Vanish);

                        remaining = 0;
                    }
                }
            }
        }
        private static void RemoveMaterial(
    Map map,
    ThingDef thingDef,
    int amount)
        {
            foreach (Thing stack in map.listerThings
                .ThingsOfDef(thingDef)
                .Where(t => t.Faction == Faction.OfPlayer || t.Faction == null)
                .ToList())
            {
                if (amount <= 0)
                    break;

                if (stack.stackCount <= amount)
                {
                    amount -= stack.stackCount;
                    stack.Destroy(DestroyMode.Vanish);
                }
                else
                {
                    stack.SplitOff(amount).Destroy(DestroyMode.Vanish);
                    amount = 0;
                }
            }
        }
        public static bool RepairBuildings(Map map)
        {
            if (map == null)
            {
                Messages.Message(
                    "No active map found.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            const int RepairCost = 2500;

            List<Thing> silverStacks = map.listerThings.ThingsOfDef(ThingDefOf.Silver);

            int totalSilver = silverStacks
                .Where(s => s.Faction == Faction.OfPlayer || s.Faction == null)
                .Sum(s => s.stackCount);

            if (totalSilver < RepairCost)
            {
                Messages.Message(
                    $"You need {RepairCost} silver to hire a repair crew.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            int silverToRemove = RepairCost;

            foreach (Thing silver in silverStacks.ToList())
            {
                if (silver.Faction != Faction.OfPlayer && silver.Faction != null)
                    continue;

                if (silver.stackCount <= silverToRemove)
                {
                    silverToRemove -= silver.stackCount;
                    silver.Destroy();
                }
                else
                {
                    silver.SplitOff(silverToRemove);
                    silverToRemove = 0;
                }

                if (silverToRemove <= 0)
                    break;
            }

            int repaired = 0;

            foreach (Thing thing in map.listerThings.AllThings)
            {
                Building building = thing as Building;

                if (building == null)
                    continue;

                if (building is Frame)
                    continue;

                if (building.Faction != Faction.OfPlayer)
                    continue;

                if (building.Destroyed)
                    continue;

                if (building.HitPoints >= building.MaxHitPoints)
                    continue;

                building.HitPoints = building.MaxHitPoints;

                repaired++;
            }

            Messages.Message(
                $"Repair crew repaired {repaired} buildings.",
                MessageTypeDefOf.PositiveEvent,
                false);

            return true;
        }
    }
}