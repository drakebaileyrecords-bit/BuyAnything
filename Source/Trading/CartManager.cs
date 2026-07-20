using BuyAnything.Source.Data;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
            int ownedQuantity = sourceThings.Sum(t => t.stackCount);

            CartItem existing = items.FirstOrDefault(i =>
                i.IsSellItem &&
                i.Def == def);

            if (existing != null)
            {
                int newQuantity = existing.Quantity + quantity;

                existing.Quantity = Mathf.Min(newQuantity, ownedQuantity);

                if (newQuantity > ownedQuantity)
                {
                    Messages.Message(
                        "You cannot sell more than you own.",
                        MessageTypeDefOf.RejectInput,
                        false);
                }
            }
            else
            {
                quantity = Mathf.Min(quantity, ownedQuantity);

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