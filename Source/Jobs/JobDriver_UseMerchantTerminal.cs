using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace BuyAnything.Source.Jobs
{
    public class JobDriver_UseMerchantTerminal : JobDriver
    {
        private Building TargetBuilding => job.targetA.Thing as Building;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetBuilding, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

            yield return Toils_General.Do(delegate
            {
                CompUsable usable = TargetBuilding.GetComp<CompUsable>();

                if (usable != null)
                {
                    usable.UsedBy(pawn);
                }
            });
        }
    }
}