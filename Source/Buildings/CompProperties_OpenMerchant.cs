using RimWorld;
using Verse;

namespace BuyAnything.Source.Buildings
{
    public class CompProperties_OpenMerchant : CompProperties_UseEffect
    {
        public CompProperties_OpenMerchant()
        {
            compClass = typeof(CompUseEffect_OpenMerchant);
        }
    }
}