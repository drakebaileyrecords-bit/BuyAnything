using RimWorld;
using Verse;
using BuyAnything.Source.UI;
using BuyAnything.Source.Settings;

namespace BuyAnything.Source.Buildings
{
    public class CompUseEffect_OpenMerchant : CompUseEffect
    {
        public override void DoEffect(Pawn usedBy)
        {
            if (ModMain.Settings.requireResearch)
            {
                ResearchProjectDef research =
                    DefDatabase<ResearchProjectDef>.GetNamedSilentFail("MerchantTerminalResearch");

                if (research != null && !research.IsFinished)
                {
                    Messages.Message(
                        "You must complete Merchant Terminal research before using this building.",
                        MessageTypeDefOf.RejectInput,
                        false);

                    return;
                }
            }

            Find.WindowStack.Add(new MerchantWindow());
        }
    }
}