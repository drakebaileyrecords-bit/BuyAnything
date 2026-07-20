using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using BuyAnything.Source.Data;
using BuyAnything.Source.Trading;
using BuyAnything.Source.Settings;

namespace BuyAnything.Source.UI
{
    public class MerchantWindow : Window
    {
        private enum MerchantMode
        {
            Buy,
            Sell
        }
        private enum SortMode
        {
            Alphabetical,
            PriceLowToHigh,
            PriceHighToLow
        }
        private MerchantMode currentMode = MerchantMode.Buy;
        private SortMode currentSort = SortMode.Alphabetical;

        private string selectedCategory = "All";
        private string searchText = "";
        private bool showFavoritesOnly = false;

        private Vector2 scrollPosition = Vector2.zero;
        private Vector2 cartScrollPosition = Vector2.zero;

        private List<MerchantItem> items = new List<MerchantItem>();

        private MerchantItem selectedItem;

        private int purchaseQuantity = 1;

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1150f, 820f);
            }
        }

        public MerchantWindow()
        {
            draggable = true;
            doCloseX = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = false;

            if (ModMain.Settings.pauseWhileOpen)
            {
                Find.TickManager.Pause();
            }

            RefreshItems();
        }

        private void RefreshItems()
        {
            if (currentMode == MerchantMode.Buy)
            {
                items = MerchantDatabase.GetAllItems();
            }
            else
            {
                items = InventoryScanner.GetSellableItems();
            }

            selectedItem = null;
            scrollPosition = Vector2.zero;
        }

        public override void DoWindowContents(Rect inRect)
        {



            Rect topBarRect = new Rect(20f, 42f, inRect.width - 40f, 42f);

            Rect categoryRect = new Rect(20f, 95f, 220f, 610f);

            Rect itemPanelRect = new Rect(255f, 95f, 330f, 610f);

            Rect detailsRect = new Rect(600f, 95f, 470f, 300f);

            Rect cartRect = new Rect(600f, 405f, 470f, 300f);

            Rect searchLabelRect = new Rect(270f, 108f, 60f, 24f);

            Rect searchBoxRect = new Rect(330f, 105f, 240f, 28f);

            Rect itemListRect = new Rect(265f, 145f, 315f, 550f);



            Text.Font = GameFont.Medium;

            Widgets.Label(
                new Rect(20f, 10f, 400f, 30f),
                "Buy/Sell Station");

            Text.Anchor = TextAnchor.UpperRight;

            Widgets.Label(
                new Rect(
                    inRect.width - 270f,
                    10f,
                    250f,
                    30f),
                "Silver: " + Find.CurrentMap.resourceCounter.Silver);

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            Widgets.DrawMenuSection(topBarRect);
            Widgets.DrawMenuSection(categoryRect);
            Widgets.DrawMenuSection(itemPanelRect);
            Widgets.DrawMenuSection(detailsRect);
            Widgets.DrawMenuSection(cartRect);

            if (Widgets.ButtonText(
    new Rect(30f, 49f, 75f, 28f),
    "Buy"))
            {
                currentMode = MerchantMode.Buy;
                CartManager.Clear();
                RefreshItems();
            }

            if (Widgets.ButtonText(
                new Rect(115f, 49f, 75f, 28f),
                "Sell"))
            {
                currentMode = MerchantMode.Sell;
                CartManager.Clear();
                RefreshItems();
            }

            if (Widgets.ButtonText(
    new Rect(200f, 49f, 90f, 28f),
    showFavoritesOnly ? "All Items" : "Favorites"))
            {
                showFavoritesOnly = !showFavoritesOnly;
            }



            Widgets.Label(
    new Rect(35f, 108f, 150f, 25f),
    "Categories");

            string[] categories =
            {
                "All",
                "Resources",
                "Weapons",
                "Apparel",
                "Food",
                "Medicine",
                "Buildings"
            };

            float categoryY = 160f;

            foreach (string category in categories)
            {
                if (Widgets.ButtonText(
                    new Rect(30f, categoryY, 180f, 30f),
                    category))
                {
                    selectedCategory = category;
                }

                categoryY += 45f;
            }

            Widgets.Label(searchLabelRect, "Search:");

            searchText = Widgets.TextField(
                searchBoxRect,
                searchText);
            if (Widgets.ButtonText(
    new Rect(300f, 49f, 95f, 28f),
    currentSort == SortMode.Alphabetical
    ? "A-Z"
    : currentSort == SortMode.PriceLowToHigh
        ? "$ Low-High"
        : "$ High-Low"))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();

                options.Add(new FloatMenuOption(
    "Alphabetical",
    delegate
    {
        currentSort = SortMode.Alphabetical;
    }));

                options.Add(new FloatMenuOption(
    "Price: Low → High",
    delegate
    {
        currentSort = SortMode.PriceLowToHigh;
    }));

                options.Add(new FloatMenuOption(
    "Price: High → Low",
    delegate
    {
        currentSort = SortMode.PriceHighToLow;
    }));
                Find.WindowStack.Add(new FloatMenu(options));
            }

            List<MerchantItem> visibleItems = items;
            if (showFavoritesOnly)
            {
                visibleItems = visibleItems
                    .Where(i => FavoritesManager.IsFavorite(i.Thing))
                    .ToList();
            }

            if (selectedCategory != "All")
            {
                visibleItems = visibleItems
                    .Where(i => i.Category == selectedCategory)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                visibleItems = visibleItems
                    .Where(i =>
                        i.Name.IndexOf(
                            searchText,
                            StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }
            switch (currentSort)
            {
                case SortMode.Alphabetical:
                    visibleItems = visibleItems
                        .OrderBy(i => i.Name)
                        .ToList();
                    break;

                case SortMode.PriceLowToHigh:
                    visibleItems = visibleItems
                        .OrderBy(i => i.Price)
                        .ToList();
                    break;

                case SortMode.PriceHighToLow:
                    visibleItems = visibleItems
                        .OrderByDescending(i => i.Price)
                        .ToList();
                    break;
            }

            Rect outRect = itemListRect;

            Rect viewRect = new Rect(
                0f,
                0f,
                295f,
                visibleItems.Count * 26f);

            Widgets.BeginScrollView(
                outRect,
                ref scrollPosition,
                viewRect);

            for (int i = 0; i < visibleItems.Count; i++)
            {
                MerchantItem item = visibleItems[i];

                Rect row =
                    new Rect(
                        5f,
                        i * 24f,
                        290f,
                        22f);

                if (selectedItem == item)
                {
                    Widgets.DrawHighlight(row);
                }

                if (Widgets.ButtonInvisible(row))
                {
                    selectedItem = item;
                }

                string label =
    (FavoritesManager.IsFavorite(item.Thing) ? "★ " : "") +
    item.Name +
    " ($" +
    item.Price.ToString("F2") +
    ")";

                if (currentMode == MerchantMode.Sell)
                {
                    label += "   x" + item.Quantity;
                }

                Rect iconRect = new Rect(
    8f,
    i * 24f + 1f,
    20f,
    20f);

                Widgets.ThingIcon(
                    iconRect,
                    item.Thing);

                Widgets.Label(
                    new Rect(
                        34f,
                        i * 24f,
                        245f,
                        22f),
                    label);
            }

            Widgets.EndScrollView();
            if (selectedItem != null)
            {
                float y = detailsRect.y + 12f;
                float left = detailsRect.x + 15f;

                Text.Font = GameFont.Medium;

                Rect bigIcon = new Rect(
    left,
    y,
    48f,
    48f);

                Widgets.ThingIcon(
                    bigIcon,
                    selectedItem.Thing);

                Widgets.Label(
                    new Rect(
                        detailsRect.x + 60f,
                        y + 6f,
                        detailsRect.width - 75f,
                        30f),
                    selectedItem.Name);

                Text.Font = GameFont.Small;

                y += 55f;

                Widgets.Label(
                    new Rect(left, y, 300f, 22f),
                    "Price: $" + selectedItem.Price.ToString("F2"));

                y += 22f;

                Widgets.Label(
                    new Rect(left, y, 300f, 22f),
                    "Category: " + selectedItem.Category);

                y += 22f;

                if (currentMode == MerchantMode.Buy)
                {
                    Widgets.Label(
                        new Rect(left, y, 300f, 22f),
                        "Stack Limit: " + selectedItem.StackLimit);
                }
                else
                {
                    int inCart = 0;

                    CartItem cartItem =
                        CartManager.Items.FirstOrDefault(i =>
                            i.IsSellItem &&
                            i.Def == selectedItem.Thing);

                    if (cartItem != null)
                        inCart = cartItem.Quantity;

                    Widgets.Label(
                        new Rect(left, y, 300f, 22f),
                        "Owned: " + selectedItem.Quantity);

                    y += 22f;

                    Widgets.Label(
                        new Rect(left, y, 300f, 22f),
                        "In Cart: " + inCart);

                    y += 22f;

                    Widgets.Label(
                        new Rect(left, y, 300f, 22f),
                        "Remaining: " + (selectedItem.Quantity - inCart));
                }

                y += 105f;

                Widgets.Label(
     new Rect(
         detailsRect.x + 15f,
         y,
         40f,
         25f),
     "Qty:");

                int maxQuantity;

                if (currentMode == MerchantMode.Buy)
                {
                    maxQuantity = Mathf.Max(1, selectedItem.StackLimit);
                }
                else
                {
                    int alreadyInCart = 0;

                    CartItem existing =
                        CartManager.Items.FirstOrDefault(i =>
                            i.IsSellItem &&
                            i.Def == selectedItem.Thing);

                    if (existing != null)
                        alreadyInCart = existing.Quantity;

                    maxQuantity = Mathf.Max(
                        1,
                        selectedItem.Quantity - alreadyInCart);
                }

                if (purchaseQuantity < 1)
                    purchaseQuantity = 1;

                if (purchaseQuantity > maxQuantity)
                    purchaseQuantity = maxQuantity;

                if (Widgets.ButtonText(
                    new Rect(detailsRect.x + 55f, y, 28f, 25f),
                    "-"))
                {
                    if (purchaseQuantity > 1)
                        purchaseQuantity--;
                }

                Widgets.Label(
                    new Rect(detailsRect.x + 90f, y, 40f, 25f),
                    purchaseQuantity.ToString());

                if (Widgets.ButtonText(
                    new Rect(detailsRect.x + 135f, y, 28f, 25f),
                    "+"))
                {
                    if (purchaseQuantity < maxQuantity)
                        purchaseQuantity++;
                }

                if (Widgets.ButtonText(
                    new Rect(detailsRect.x + 170f, y, 50f, 25f),
                    "MAX"))
                {
                    purchaseQuantity = maxQuantity;
                }

                y += 35f;

                bool favorite = FavoritesManager.IsFavorite(selectedItem.Thing);

                if (Widgets.ButtonText(
                    new Rect(
                        detailsRect.x + 15f,
                        y,
                        140f,
                        35f),
                    favorite ? "★ Favorited" : "☆ Favorite"))
                {
                    FavoritesManager.ToggleFavorite(selectedItem.Thing);
                }

                if (Widgets.ButtonText(
                    new Rect(
                        detailsRect.x + 165f,
                        y,
                        140f,
                        35f),
                    "Add to Cart"))
                {
                    if (currentMode == MerchantMode.Buy)
                    {
                        CartManager.AddItem(
                            selectedItem.Thing,
                            purchaseQuantity,
                            selectedItem.Price);
                    }
                    else
                    {
                        CartManager.AddSellItem(
                            selectedItem.Thing,
                            purchaseQuantity,
                            selectedItem.Price,
                            selectedItem.SourceThings);
                    }
                }
            }

            Widgets.Label(
                new Rect(
                    cartRect.x + 10f,
                    cartRect.y + 10f,
                    200f,
                    25f),
                "Shopping Cart");

            Rect cartOutRect =
                new Rect(
                    cartRect.x + 10f,
                    cartRect.y + 35f,
                    cartRect.width - 20f,
                    cartRect.height - 65f);

            Rect cartViewRect =
                new Rect(
                    0f,
                    0f,
                    cartOutRect.width - 16f,
                    CartManager.Items.Count * 32f);

            Widgets.BeginScrollView(
                cartOutRect,
                ref cartScrollPosition,
                cartViewRect);

            float rowY = 0f;

            foreach (CartItem item in CartManager.Items.ToArray())
            {
                Widgets.ThingIcon(
    new Rect(
        0f,
        rowY + 1f,
        20f,
        20f),
    item.Def);

                Widgets.Label(
                    new Rect(
                        25f,
                        rowY,
                        150f,
                        22f),
                    item.Name);

                Widgets.Label(
                    new Rect(
                        180f,
                        rowY,
                        35f,
                        22f),
                    "x" + item.Quantity);

                Widgets.Label(
                    new Rect(
                        220f,
                        rowY,
                        70f,
                        22f),
                    "$" + item.TotalPrice.ToString("F2"));

                if (Widgets.ButtonText(
                    new Rect(
                        295f,
                        rowY,
                        20f,
                        20f),
                    "-"))
                {
                    item.Quantity--;

                    if (item.Quantity <= 0)
                    {
                        CartManager.RemoveItem(item);
                    }

                    break;
                }

                if (Widgets.ButtonText(
                    new Rect(
                        320f,
                        rowY,
                        20f,
                        20f),
                    "X"))
                {
                    CartManager.RemoveItem(item);
                    break;
                }

                rowY += 32f;
            }

            Widgets.EndScrollView();

            Widgets.Label(
                new Rect(
                    cartRect.x + 10f,
                    cartRect.y + cartRect.height - 55f,
                    250f,
                    25f),
                "Total: $" + CartManager.GetTotalCost().ToString("F2"));

            if (Widgets.ButtonText(
                new Rect(
                    cartRect.x + 10f,
                    cartRect.y + cartRect.height - 30f,
                    120f,
                    25f),
                "Clear Cart"))
            {
                CartManager.Clear();
            }

            string bottomButton =
                currentMode == MerchantMode.Buy
                    ? "Purchase"
                    : "Sell";

            if (Widgets.ButtonText(
                new Rect(
                    cartRect.x + 140f,
                    cartRect.y + cartRect.height - 30f,
                    120f,
                    25f),
                bottomButton))
            {
                if (currentMode == MerchantMode.Buy)
                {
                    PurchaseManager.Purchase();
                }
                else
                {
                    SellManager.Sell();
                }
            }
        }
        public override void PostClose()
        {
            base.PostClose();

            if (ModMain.Settings.pauseWhileOpen)
            {
                Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
            }
        }
    }
}