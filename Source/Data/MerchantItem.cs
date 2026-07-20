using System.Collections.Generic;
using Verse;
using BuyAnything.Source.Utilities;

namespace BuyAnything.Source.Data
{
    public class MerchantItem
    {
        // Item definition
        public ThingDef Thing;

        // Total quantity available (used for selling)
        public int Quantity;

        // Actual map objects represented by this entry
        public List<Thing> SourceThings = new List<Thing>();

        // Display name
        public string Name
        {
            get
            {
                if (Thing == null)
                    return "";

                return Thing.LabelCap.ToString();
            }
        }

        // Unit price
        public float Price
        {
            get
            {
                return Thing != null ? Thing.BaseMarketValue : 0f;
            }
        }

        // Description
        public string Description
        {
            get
            {
                return Thing != null ? Thing.description : "";
            }
        }

        // Maximum stack size
        public int StackLimit
        {
            get
            {
                return Thing != null ? Thing.stackLimit : 1;
            }
        }

        // Store category
        public string Category
        {
            get
            {
                return CategoryUtility.GetStoreCategory(Thing);
            }
        }

        // Constructor for BUY items
        public MerchantItem()
        {
            Quantity = 1;
        }

        // Constructor for SELL items
        public MerchantItem(ThingDef thing, int quantity, List<Thing> sourceThings)
        {
            Thing = thing;
            Quantity = quantity;
            SourceThings = sourceThings ?? new List<Thing>();
        }
    }
}