using Verse;

namespace BuyAnything.Source.Data
{
    public class BlueprintMaterial
    {
        public ThingDef Thing;

        public int Required;

        public int Owned;

        public int Missing;

        public float UnitPrice;

        public float TotalPrice
        {
            get
            {
                return Missing * UnitPrice;
            }
        }
    }
}