using System.Collections.Generic;
using Verse;

namespace BuyAnything.Source.Data
{
    public class CartItem
    {
        // Item definition
        public ThingDef Def;

        // Display name
        public string Name;

        // Quantity
        public int Quantity;

        // Price per item
        public float UnitPrice;

        // True when selling instead of buying
        public bool IsSellItem;

        // Actual map objects being sold
        public List<Thing> SourceThings = new List<Thing>();

        public float TotalPrice => UnitPrice * Quantity;

        // Buying constructor
        public CartItem(
            ThingDef def,
            int quantity,
            float unitPrice)
        {
            Def = def;
            Name = def.label.CapitalizeFirst();
            Quantity = quantity;
            UnitPrice = unitPrice;
            IsSellItem = false;
        }

        // Selling constructor
        public CartItem(
            ThingDef def,
            int quantity,
            float unitPrice,
            List<Thing> sourceThings)
        {
            Def = def;
            Name = def.label.CapitalizeFirst();
            Quantity = quantity;
            UnitPrice = unitPrice;
            IsSellItem = true;
            SourceThings = sourceThings ?? new List<Thing>();
        }
    }
}