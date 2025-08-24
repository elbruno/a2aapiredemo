# SAVA E‑Commerce Migration — User Manual

This manual documents the demonstration steps shown in the analyzed video and provides clear, actionable instructions to reproduce and validate the behaviors seen in the SAVA e‑commerce experience.

- Video total duration: 00:00:55.480  
- Relevant UI: SAVA application (product list, search, cart, checkout, localization)

---

## Overview
(00:00:03.720 — 00:00:13.492)

This project is migrating existing e‑commerce sites into the new SAVA experience. The demo covers basic shopping actions (adding items to cart), a search behavior where the "Add to cart" action is missing, and an unexpected cart clearing when proceeding to checkout. The team will validate these behaviors and apply fixes including localization / "local feel" adjustments.

Key topics:
- Migration to SAVA experience
- Add‑to‑cart functionality
- Search results behavior
- Cart and checkout behavior
- Validation and localization tasks

---

## Step‑by‑Step Instructions

Below are step‑by‑step procedures to reproduce the behaviors demonstrated and to validate or report issues.

### A. Add products to the cart (Demonstration)
(00:00:14.240 — 00:00:27.400)

Purpose: Show how to add products from the product list to the cart and verify items were added.

Steps:
1. Open the SAVA application and navigate to the product list or category page.
2. Locate the product tile for the item you want (example: paint product).
3. Click the "Add to cart" button on the product tile.
   - Tip: Use the product tile action rather than product details if the demo used tile-level add.
4. Repeat step 3 to add a second product.
5. Verify the cart icon (or cart view) shows the correct number of items (the demo confirms two products present).

Expected result:
- The cart icon/count updates to reflect added items.
- Opening the cart shows the two selected products.

Warning:
- If the cart count does not update, do not proceed to checkout — capture screenshots and console logs for debugging.

Inline snapshot:
![Product list with Add to cart buttons (00:00:14.240)](./snapshot_00_00_14_240.png)
Caption: Product tiles and "Add to cart" action (start of add‑to‑cart demo).

Inline snapshot:
![Cart showing two products (00:00:27.400)](./snapshot_00_00_27_400.png)
Caption: Cart view showing two items added (end of add‑to‑cart demo).

---

### B. Search for a product and validate missing "Add to cart"
(00:00:27.400 — 00:00:38.440)

Purpose: Reproduce the search behavior that hides or omits the "Add to cart" option in search results.

Steps:
1. In SAVA, locate the search box at the top of the page.
2. Enter the product name (example: "paint") and run the search.
3. Inspect the search results list for the expected "Add to cart" action on each result.
4. Observe whether the "Add to cart" button is present for each result.

Observed issue (as demonstrated):
- The "Add to cart" option is missing from search results UI for the searched product.

Action for validation/reporting:
- If "Add to cart" is missing:
  - Reproduce the issue with multiple search terms and product types.
  - Note browser, user role, and locale settings.
  - Capture a screenshot of the search results and attach logs.
  - Create a validation ticket referencing this behavior and include the timestamp where demonstrated (00:00:27.400).

Inline snapshot:
![Search results missing Add to cart (00:00:27.400)](./snapshot_00_00_27_400_search.png)
Caption: Search results showing missing "Add to cart" option.

Tip:
- Compare the product tile UI in the main product list (where add works) to search result tiles — differences often indicate a missing action binding or template.

---

### C. Cart → Proceed to Checkout behavior (Bug reproduction)
(00:00:38.440 — 00:00:47.760)

Purpose: Reproduce the behavior where proceeding to checkout clears the cart unexpectedly.

Steps:
1. With items already in the cart (see Section A), open the cart view to review selected items.
2. Click the "Proceed to checkout" button.
3. Observe the cart contents immediately after the action and during the checkout flow.

Observed behavior (as demonstrated):
- Clicking "Proceed to checkout" clears the cart instead of preserving items through the checkout flow.
- This indicates the checkout integration is not implemented or has a bug.

Validation and reporting:
- Reproduce the issue with varied carts (different item counts and SKUs).
- Note whether the issue happens for all users or only certain roles/locales.
- Collect screenshots showing the cart before clicking checkout and after the cart is cleared.
- Capture any network/API failures in developer tools that correspond with the action.

Warning:
- Do not perform live transactions while debugging this issue; it may result in data loss or inconsistent orders.

Inline snapshot:
![Cart cleared after clicking Proceed to checkout (00:00:38.440)](./snapshot_00_00_38_440.png)
Caption: Cart view at the moment the checkout action was performed.

---

### D. Validation checklist and localization / UI "local feel"
(00:00:47.760 — 00:00:55.480)

Purpose: Identify places to fix UI behavior and local feel inconsistencies.

Validation checklist:
- Verify add‑to‑cart behavior across:
  - Product lists / category pages
  - Product detail pages
  - Search results
- Verify cart persistence through the checkout initiation step.
- Confirm localization strings, currency formatting, and numeric/locale UI elements reflect target locales.

Steps to validate localization:
1. Change the application locale (if available) or simulate locale via user profile.
2. Verify UI text, date/number formats, currency symbols, and layout direction are correct.
3. Validate that localized labels for cart and checkout actions are present and consistent.

Inline snapshot:
![SAVA app localization / settings area (00:00:47.760)](./snapshot_00_00_47_760.png)
Caption: Area in SAVA where localization or "local feel" adjustments are identified.

Tip:
- Use automated tests to exercise add‑to‑cart, search, and checkout flows in each locale to catch regressions.

---

## Tips & Troubleshooting (General)

- Capture console logs and network traces when reproducing issues — these are invaluable for developers.
- When filing a bug:
  - Include reproduction steps, observed vs expected behavior, browser/OS, user role, locale, and screenshots.
  - Reference the video timestamps from this manual for quick context.
- If the "Add to cart" button works in product lists but not search results, check template bindings and component reuse between the two views.
- If checkout clears the cart, check session/local storage handling and checkout API calls for failures.

---

## Snapshots

![Snapshot at 00:00:03.720](./images/brk447-02-Zava%20Overview-en-US-snapshot-00-00-03-720.png)  
![Snapshot at 00:00:14.240](./images/brk447-02-Zava%20Overview-en-US-snapshot-00-00-14-240.png)  
![Snapshot at 00:00:27.400](./images/brk447-02-Zava%20Overview-en-US-snapshot-00-00-27-400.png)  
![Snapshot at 00:00:38.440](./images/brk447-02-Zava%20Overview-en-US-snapshot-00-00-38-440.png)  
![Snapshot at 00:00:47.760](./images/brk447-02-Zava%20Overview-en-US-snapshot-00-00-47-760.png)