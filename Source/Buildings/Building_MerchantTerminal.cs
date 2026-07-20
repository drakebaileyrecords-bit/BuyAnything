using System.Collections.Generic;
using RimWorld;
using Verse;
using BuyAnything.Source.UI;
using BuyAnything.Source.Settings;

namespace BuyAnything.Source.Buildings
{
    public class Building_MerchantTerminal : Building
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
                yield return gizmo;

            yield return new Command_Action
            {
                defaultLabel = "Open Merchant",
                defaultDesc = "Browse and purchase items.",

                action = () =>
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

                    if (ModMain.Settings.requirePower)
                    {
                        CompPowerTrader power = GetComp<CompPowerTrader>();

                        if (power != null && !power.PowerOn)
                        {
                            Messages.Message(
                                "This Merchant Terminal has no power.",
                                MessageTypeDefOf.RejectInput,
                                false);

                            return;
                        }
                    }

                    Find.WindowStack.Add(new MerchantWindow());
                }
            };
        }

        public override string GetInspectString()
        {
            string inspect = base.GetInspectString();

            if (ModMain.Settings.requireResearch)
            {
                ResearchProjectDef research =
                    DefDatabase<ResearchProjectDef>.GetNamedSilentFail("MerchantTerminalResearch");

                if (research != null)
                {
                    inspect += "\nResearch: " +
                        (research.IsFinished ? "Complete" : "Required");
                }
            }

            if (ModMain.Settings.requirePower)
            {
                CompPowerTrader power = GetComp<CompPowerTrader>();

                if (power != null)
                {
                    inspect += "\nPower: " +
                        (power.PowerOn ? "Online" : "Offline");
                }
            }

            return inspect.TrimEnd();
        }
    }
}