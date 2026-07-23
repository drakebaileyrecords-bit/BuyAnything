using RimWorld;
using System.Linq;
using Verse;

namespace WorkbenchMerchant2.Source.Trading
{
    internal static class AutoHaulService
    {
        public static bool Execute(Map map)
        {
            if (map == null)
            {
                Messages.Message(
                    "No active map found.",
                    MessageTypeDefOf.RejectInput,
                    false);

                return false;
            }

            int itemsMoved = 0;

            foreach (Thing thing in map.listerThings.AllThings.ToList())
            {
                if (thing.Destroyed)
                    continue;

                if (!thing.def.EverHaulable)
                    continue;

                if (thing.IsForbidden(Faction.OfPlayer))
                    continue;

                if (thing.PositionHeld == IntVec3.Invalid)
                    continue;

                IntVec3 storeCell;

                bool foundStore =
                    StoreUtility.TryFindBestBetterStoreCellFor(
                        thing,
                        null,
                        map,
                        StoragePriority.Unstored,
                        Faction.OfPlayer,
                        out storeCell,
                        false);

                if (!foundStore)
                    continue;

                ThingOwner owner = thing.holdingOwner;

                if (owner != null)
                {
                    owner.Remove(thing);
                }

                thing.DeSpawn();

                GenPlace.TryPlaceThing(
                    thing,
                    storeCell,
                    map,
                    ThingPlaceMode.Direct);

                itemsMoved++;
            }

            Messages.Message(
                            $"Freight crew hauled {itemsMoved} items.",
                MessageTypeDefOf.PositiveEvent,
                false);

            return true;
        }
    }
}