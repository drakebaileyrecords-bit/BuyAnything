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
        private float GetDisplayPrice(MerchantItem item)
        {
            return currentMode == MerchantMode.Buy
                ? PriceCalculator.GetBuyPrice(item.Thing)
                : PriceCalculator.GetSellPrice(item.Thing);
        }
        private void DrawInspectorRow(float x, float y, float width, string label, string value)
        {
            if (!string.IsNullOrEmpty(label))
            {
                Widgets.Label(
                    new Rect(x, y, width * 0.55f, 22f),
                    label);

                Text.Anchor = TextAnchor.UpperRight;

                Widgets.Label(
                    new Rect(x + width * 0.55f, y, width * 0.45f, 22f),
                    value);

                Text.Anchor = TextAnchor.UpperLeft;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {



            Rect topBarRect = new Rect(20f, 42f, inRect.width - 40f, 42f);

            Rect categoryRect = new Rect(20f, 95f, 220f, 610f);

            Rect itemPanelRect = new Rect(255f, 95f, 330f, 610f);

            Rect detailsRect = new Rect(600f, 95f, 470f, 360f);

            Rect cartRect = new Rect(600f, 465f, 470f, 240f);

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
            // Sort button
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

            // Buy Blueprints button
            if (Widgets.ButtonText(
                new Rect(405f, 49f, 165f, 28f),
                "Buy Blueprints"))
            {
                BlueprintPurchaseManager.AddMissingMaterialsToCart();
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
                        .OrderBy(i => GetDisplayPrice(i))
                        .ToList();
                    break;

                case SortMode.PriceHighToLow:
                    visibleItems = visibleItems
                        .OrderByDescending(i => GetDisplayPrice(i))
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

                float displayPrice = GetDisplayPrice(item);

                string label =
                    (FavoritesManager.IsFavorite(item.Thing) ? "★ " : "") +
                    item.Name +
                    " ($" +
                    displayPrice.ToString("F2") +
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
                const float LineHeight = 22f;
                const float SectionGap = 12f;

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

                y += 60f;

                float marketValue = selectedItem.Thing.BaseMarketValue;
                float finalPrice = GetDisplayPrice(selectedItem);

                DrawInspectorRow(
                    left,
                    y,
                    285f,
                    "Market Value",
                    "$" + marketValue.ToString("F2"));

                y += LineHeight;

                DrawInspectorRow(
                    left,
                    y,
                    285f,
                    currentMode == MerchantMode.Buy
                        ? "Buy Price"
                        : "Sell Price",
                    "$" + finalPrice.ToString("F2"));

                y += LineHeight + SectionGap;

                DrawInspectorRow(
                    left,
                    y,
                    285f,
                    currentMode == MerchantMode.Buy
                        ? "Buy Multiplier"
                        : "Sell Multiplier",
                    "x" + (
                        currentMode == MerchantMode.Buy
                            ? ModMain.Settings.buyPriceMultiplier
                            : ModMain.Settings.sellPriceMultiplier
                    ).ToString("F2"));

                y += LineHeight;

                DrawInspectorRow(
                    left,
                    y,
                    285f,
                    "Trade Tax",
                    ModMain.Settings.tradeTax.ToString("F0") + "%");

                y += LineHeight + SectionGap;

                DrawInspectorRow(
                    left,
                    y,
                    285f,
                    "Category",
                    selectedItem.Category);

                y += LineHeight;

                int inCart = 0;

                if (currentMode == MerchantMode.Sell)
                {
                    CartItem cartItem =
                        CartManager.Items.FirstOrDefault(i =>
                            i.IsSellItem &&
                            i.Def == selectedItem.Thing);

                    if (cartItem != null)
                        inCart = cartItem.Quantity;
                }

                DrawInspectorRow(
                    left,
                    y,
                    285f,
                    currentMode == MerchantMode.Buy ? "Stack Limit" : "Owned",
                    currentMode == MerchantMode.Buy
                        ? selectedItem.StackLimit.ToString()
                        : selectedItem.Quantity.ToString());

                y += LineHeight;

                DrawInspectorRow(
                    left,
                    y,
                    285f,
                    currentMode == MerchantMode.Buy ? "" : "In Cart",
                    currentMode == MerchantMode.Buy ? "" : inCart.ToString());

                y += LineHeight;

                DrawInspectorRow(
                    left,
                    y,
                    285f,
                    currentMode == MerchantMode.Buy ? "" : "Remaining",
                    currentMode == MerchantMode.Buy
                        ? ""
                        : (selectedItem.Quantity - inCart).ToString());

                y += LineHeight;

                float controlsX = detailsRect.x + 320f;
                float controlsY = detailsRect.y + 105f;

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

                purchaseQuantity = Mathf.Clamp(
                    purchaseQuantity,
                    1,
                    maxQuantity);

                // Quantity title
                Widgets.Label(
                    new Rect(
                        controlsX,
                        controlsY,
                        80f,
                        22f),
                    "Qty");

                controlsY += 22f;

                // Minus button
                if (Widgets.ButtonText(
                    new Rect(controlsX, controlsY, 24f, 24f),
                    "-"))
                {
                    purchaseQuantity = Mathf.Max(1, purchaseQuantity - 1);
                }

                // Quantity number
                Widgets.Label(
                    new Rect(
                        controlsX + 30f,
                        controlsY,
                        35f,
                        24f),
                    purchaseQuantity.ToString());

                // Plus button
                if (Widgets.ButtonText(
                    new Rect(controlsX + 70f, controlsY, 24f, 24f),
                    "+"))
                {
                    purchaseQuantity = Mathf.Min(maxQuantity, purchaseQuantity + 1);
                }

                controlsY += 32f;

                // Slider
                purchaseQuantity = Mathf.RoundToInt(
                    Widgets.HorizontalSlider(
                        new Rect(
                            controlsX,
                            controlsY,
                            120f,
                            22f),
                        purchaseQuantity,
                        1,
                        maxQuantity));

                controlsY += 24f;

                // Max button
                if (Widgets.ButtonText(
                    new Rect(
                        controlsX,
                        controlsY,
                        120f,
                        24f),
                    "MAX"))
                {
                    purchaseQuantity = maxQuantity;
                }

                bool favorite = FavoritesManager.IsFavorite(selectedItem.Thing);

                // Footer buttons
                float footerY = detailsRect.yMax - 45f;

                if (Widgets.ButtonText(
                    new Rect(
                        detailsRect.x + 20f,
                        footerY,
                        170f,
                        30f),
                    favorite ? "★ Favorited" : "☆ Favorite"))
                {
                    FavoritesManager.ToggleFavorite(selectedItem.Thing);
                }

                if (Widgets.ButtonText(
    new Rect(
        detailsRect.x + 205f,
        footerY,
        170f,
        30f),
    "Add to Cart"))
                
                {
                    if (currentMode == MerchantMode.Buy)
                    {
                        float displayPrice = PriceCalculator.GetBuyPrice(selectedItem.Thing);

                        CartManager.AddItem(
                            selectedItem.Thing,
                            purchaseQuantity,
                            displayPrice);
                    }
                    else
                    {
                        float displayPrice = PriceCalculator.GetSellPrice(selectedItem.Thing);

                        CartManager.AddSellItem(
                            selectedItem.Thing,
                            purchaseQuantity,
                            displayPrice,
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
    currentMode == MerchantMode.Buy
        ? "Shopping Cart"
        : "Selling Cart");

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

            Text.Anchor = TextAnchor.MiddleRight;

            Widgets.Label(
                new Rect(
                    cartRect.x + cartRect.width - 220f,
                    cartRect.y + cartRect.height - 30f,
                    210f,
                    25f),
                "Total: $" + CartManager.GetTotalCost().ToString("N2"));

            Text.Anchor = TextAnchor.UpperLeft;
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