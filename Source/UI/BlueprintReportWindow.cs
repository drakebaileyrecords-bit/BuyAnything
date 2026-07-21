using BuyAnything.Source.Data;
using BuyAnything.Source.Trading;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BuyAnything.Source.UI
{
    public class BlueprintReportWindow : Window
    {
        private readonly List<BlueprintMaterial> materials;

        private Vector2 scrollPosition = Vector2.zero;

        private const float RowHeight = 34f;

        private const float HeaderHeight = 36f;

        private const float SummaryHeight = 90f;

        private const float FooterHeight = 120f;

        public override Vector2 InitialSize =>
            new Vector2(1120f, 760f);

        public BlueprintReportWindow(
            List<BlueprintMaterial> materials)
        {
            this.materials = materials;

            forcePause = true;
            absorbInputAroundWindow = true;
            draggable = true;
            doCloseButton = false;
            doCloseX = true;
            closeOnClickedOutside = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(
                new Rect(
                    0f,
                    8f,
                    inRect.width,
                    40f),
                "Construction Shopping Report");

            Text.Font = GameFont.Small;

            float y = 72f;

            DrawSummarySection(
                new Rect(
                    0f,
                    y,
                    inRect.width,
                    SummaryHeight));

            y += SummaryHeight + 10f;

            DrawTableHeader(
                new Rect(
                    0f,
                    y,
                    inRect.width,
                    HeaderHeight));

            y += HeaderHeight;

            Rect listRect =
                new Rect(
                    0f,
                    y,
                    inRect.width,
                    inRect.height -
                    y -
                    FooterHeight);

            DrawMaterialList(listRect);

            Rect footerRect =
                new Rect(
                    0f,
                    inRect.height -
                    FooterHeight,
                    inRect.width,
                    FooterHeight);

            DrawFooter(footerRect);
        }

        private void DrawSummarySection(Rect rect)
        {
            Widgets.DrawMenuSection(rect);

            Rect left =
                new Rect(
                    rect.x + 12f,
                    rect.y + 8f,
                    rect.width * 0.45f,
                    rect.height - 16f);

            Rect right =
                new Rect(
                    rect.width * 0.52f,
                    rect.y + 8f,
                    rect.width * 0.46f,
                    rect.height - 16f);

            Text.Font = GameFont.Small;

            Widgets.Label(
                new Rect(
                    left.x,
                    left.y,
                    left.width,
                    24f),
                "Construction Summary");

            Widgets.Label(
                new Rect(
                    left.x,
                    left.y + 24f,
                    left.width,
                    24f),
                $"Missing Materials: {GetMissingItemCount()}");

            Widgets.Label(
                new Rect(
                    left.x,
                    left.y + 48f,
                    left.width,
                    24f),
                $"Material Types: {materials.Count}");

            Widgets.Label(
                new Rect(
                    right.x,
                    right.y,
                    right.width,
                    24f),
                "Pricing");

            Widgets.Label(
                new Rect(
                    right.x,
                    right.y + 24f,
                    right.width,
                    24f),
                $"Subtotal: ${GetSubtotal():0.00}");

            Widgets.Label(
                new Rect(
                    right.x,
                    right.y + 48f,
                    right.width,
                    24f),
                $"Estimated Tax: ${GetEstimatedTax():0.00}");
        }

        private void DrawTableHeader(Rect rect)
        {
            Widgets.DrawMenuSection(rect);

            float x = 8f;

            DrawHeader(rect, ref x, "", 36f);
            DrawHeader(rect, ref x, "Material", 220f);
            DrawHeader(rect, ref x, "Required", 90f);
            DrawHeader(rect, ref x, "Owned", 90f);
            DrawHeader(rect, ref x, "Missing", 90f);
            DrawHeader(rect, ref x, "Unit", 100f);
            DrawHeader(rect, ref x, "Tax", 90f);
            DrawHeader(rect, ref x, "Total", 110f);
            DrawHeader(rect, ref x, "", 90f);
        }

        private void DrawHeader(
    Rect rect,
    ref float x,
    string label,
    float width)
        {
            Widgets.Label(
    new Rect(
        x,
        rect.y + 8f,
        width,
        24f),
    label);

            x += width;
        }

        private void DrawMaterialList(Rect rect)
        {
            float viewHeight =
                materials.Count * RowHeight;

            Rect viewRect =
                new Rect(
                    0f,
                    0f,
                    rect.width - 16f,
                    viewHeight);

            Widgets.BeginScrollView(
                rect,
                ref scrollPosition,
                viewRect);

            float y = 0f;

            for (int i = 0; i < materials.Count; i++)
            {
                DrawMaterialRow(
                    new Rect(
                        0f,
                        y,
                        viewRect.width,
                        RowHeight),
                    materials[i]);

                y += RowHeight;
            }

            Widgets.EndScrollView();
        }
        private void DrawMaterialRow(
    Rect rect,
    BlueprintMaterial material)
        {
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }

            float x = 8f;

            Widgets.ThingIcon(
                new Rect(
                    x,
                    rect.y + 2f,
                    28f,
                    28f),
                material.Thing);

            x += 36f;

            Widgets.Label(
                new Rect(
                    x,
                    rect.y + 6f,
                    220f,
                    24f),
                material.Thing.LabelCap);

            x += 220f;

            DrawValue(
                ref x,
                rect.y,
                material.Required.ToString(),
                90f);

            DrawValue(
                ref x,
                rect.y,
                material.Owned.ToString(),
                90f);

            GUI.color = Color.yellow;

            DrawValue(
                ref x,
                rect.y,
                material.Missing.ToString(),
                90f);

            GUI.color = Color.white;

            DrawValue(
                ref x,
                rect.y,
                $"${material.UnitPrice:0.00}",
                100f);

            float lineTax =
                GetTaxForMaterial(material);

            DrawValue(
                ref x,
                rect.y,
                $"${lineTax:0.00}",
                90f);

            DrawValue(
                ref x,
                rect.y,
                $"${material.TotalPrice:0.00}",
                110f);

            Rect buttonRect =
                new Rect(
                    x,
                    rect.y + 2f,
                    80f,
                    28f);

            if (Widgets.ButtonText(
                buttonRect,
                "Buy"))
            {
                CartManager.AddItem(
                    material.Thing,
                    material.Missing,
                    material.UnitPrice);
            }
        }

        private static void DrawValue(
            ref float x,
            float y,
            string value,
            float width)
        {
            Widgets.Label(
                new Rect(
                    x,
                    y + 6f,
                    width,
                    24f),
                value);

            x += width;
        }

        private void DrawFooter(Rect rect)
        {
            Widgets.DrawMenuSection(rect);

            float leftWidth =
                rect.width * 0.55f;

            Rect left =
                new Rect(
                    rect.x + 12f,
                    rect.y + 10f,
                    leftWidth,
                    rect.height - 20f);

            Rect right =
                new Rect(
                    rect.x + leftWidth,
                    rect.y + 10f,
                    rect.width - leftWidth - 12f,
                    rect.height - 20f);

            Widgets.Label(
                new Rect(
                    left.x,
                    left.y,
                    left.width,
                    24f),
                $"Subtotal: ${GetSubtotal():0.00}");

            Widgets.Label(
                new Rect(
                    left.x,
                    left.y + 24f,
                    left.width,
                    24f),
                $"Sales Tax: ${GetEstimatedTax():0.00}");

            Widgets.Label(
                new Rect(
                    left.x,
                    left.y + 48f,
                    left.width,
                    24f),
                $"Grand Total: ${GetGrandTotal():0.00}");

            Rect cancelRect =
                new Rect(
                    right.x,
                    right.y + 36f,
                    180f,
                    36f);

            if (Widgets.ButtonText(
                cancelRect,
                "Cancel"))
            {
                Close();
            }

            Rect addRect =
                new Rect(
                    right.x + 190f,
                    right.y + 36f,
                    220f,
                    36f);

            if (Widgets.ButtonText(
                addRect,
                "Add Everything"))
            {
                foreach (BlueprintMaterial material in materials)
                {
                    CartManager.AddItem(
                        material.Thing,
                        material.Missing,
                        material.UnitPrice);
                }

                Close();
            }
        }
                private int GetMissingItemCount()
        {
            int total = 0;

            foreach (BlueprintMaterial material in materials)
            {
                total += material.Missing;
            }

            return total;
        }

        private float GetSubtotal()
        {
            float total = 0f;

            foreach (BlueprintMaterial material in materials)
            {
                total += material.TotalPrice;
            }

            return total;
        }

        private float GetEstimatedTax()
        {
            float total = 0f;

            foreach (BlueprintMaterial material in materials)
            {
                total += GetTaxForMaterial(material);
            }

            return total;
        }

        private float GetGrandTotal()
        {
            return GetSubtotal() + GetEstimatedTax();
        }

        private float GetTaxForMaterial(
            BlueprintMaterial material)
        {
            // Match this to your merchant tax calculation later.
            const float TaxRate = 0.08f;

            return material.TotalPrice * TaxRate;
        }

        private bool HasAnythingToBuy()
        {
            foreach (BlueprintMaterial material in materials)
            {
                if (material.Missing > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public override void PostClose()
        {
            base.PostClose();

            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}