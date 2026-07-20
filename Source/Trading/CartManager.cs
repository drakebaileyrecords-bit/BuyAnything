using System.Collections.Generic;
using System.Linq;
using BuyAnything.Source.Data;
using Verse;

namespace BuyAnything.Source.Trading
{
    public static class CartManager
    {
        private static readonly List<CartItem> items = new List<CartItem>();

        public static IReadOnlyList<CartItem> Items => items;

        public static void AddItem(ThingDef def, int quantity, float unitPrice)
        {
            CartItem existing = items.FirstOrDefault(i => i.Def == def);

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                items.Add(new CartItem(def, quantity, unitPrice));
            }
        }
        public static void AddSellItem(
    ThingDef def,
    int quantity,
    float unitPrice,
    List<Thing> sourceThings)
        {
            CartItem existing = items.FirstOrDefault(i =>
                i.IsSellItem &&
                i.Def == def);

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                items.Add(new CartItem(
                    def,
                    quantity,
                    unitPrice,
                    sourceThings));
            }
        }

        public static IEnumerable<CartItem> SellItems()
        {
            return items.Where(i => i.IsSellItem);
        }

        public static bool HasSellItems()
        {
            return items.Any(i => i.IsSellItem);
        }

        public static void RemoveItem(CartItem item)
        {
            items.Remove(item);
        }

        public static void Clear()
        {
            items.Clear();
        }

        public static float GetTotalCost()
        {
            return items.Sum(i => i.TotalPrice);
        }

        public static int GetTotalItems()
        {
            return items.Sum(i => i.Quantity);
        }
    }
}