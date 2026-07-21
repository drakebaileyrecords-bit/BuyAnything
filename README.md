# BuyAnything
### A complete buy & sell station for RimWorld

BuyAnything adds a fully featured shopping system to RimWorld, allowing colonists to purchase resources, equipment, and building materials through an intuitive in-game merchant interface. The mod also includes an intelligent Blueprint Purchasing system that automatically analyzes your construction projects and generates a shopping list for every missing resource.

---

# Version 0.5.0

## Major Features

### 🛒 Blueprint Purchasing Report
A complete redesign of the blueprint purchasing workflow.

Instead of instantly adding resources to the cart, BuyAnything now generates a detailed shopping report before purchase.

The report includes:

- Complete material breakdown
- Required quantity
- Currently owned quantity
- Missing quantity
- Unit price
- Estimated sales tax
- Total cost per material
- Grand total
- Individual material purchasing
- One-click **Add Everything** button

Blueprints and construction frames are now scanned correctly before generating the report.

---

### 🏗 Improved Blueprint Detection

The blueprint scanner has been rewritten to properly detect:

- Building blueprints
- Construction frames
- Stuff-based buildings
- Material substitutions
- Existing owned resources

This greatly improves shopping accuracy for large construction projects.

---

### 🧾 Shopping Report UI Redesign

The Blueprint Report window has been completely rebuilt.

New layout includes:

- Construction Summary
- Pricing Summary
- Scrollable material list
- Fixed footer
- Improved spacing
- Larger window
- Cleaner RimWorld-style interface

---

### 💰 Pricing Improvements

Added pricing breakdown including:

- Subtotal
- Estimated Sales Tax
- Grand Total

Large currency values now display with thousands separators for easier reading.

Example:

```
$8,069.19
```

instead of

```
$8069.19
```

---

### 🛍 Shopping Cart Improvements

Merchant cart layout has been cleaned up.

Changes include:

- Cleaner footer layout
- Total moved to the right side of the cart
- Purchase controls grouped together
- Improved readability

---

### 🎨 UI Polish

Numerous interface improvements including:

- Better spacing
- Improved alignment
- Larger report window
- Cleaner scrolling behavior
- Better visual hierarchy
- Highlighted missing materials
- Improved pricing display

---

## Internal Changes

- Rebuilt BlueprintReportWindow
- Refactored BlueprintPurchaseManager
- Improved material aggregation
- Cleaner UI rendering methods
- Better separation between business logic and UI
- Added reusable pricing helper methods

---

## Fixed

- Fixed blueprint report not opening
- Fixed blueprint material detection
- Fixed frame detection
- Fixed stuff-based building support
- Fixed duplicate material calculations
- Fixed incorrect total calculations
- Fixed report layout overlap
- Fixed header rendering
- Fixed footer alignment
- Fixed shopping cart footer layout

---

## Performance

Large blueprint projects now generate a single consolidated material report instead of repeatedly adding items directly into the shopping cart, providing a much smoother purchasing workflow.

---

## Next Planned Features

- Merchant information panel redesign
- Better item pricing breakdown
- Purchase history
- Search improvements
- Additional item categories
- Improved trading statistics
- Optional merchant discounts
- Better tax configuration

---

Thank you for using BuyAnything!

Feedback, bug reports, and feature suggestions are always appreciated.
