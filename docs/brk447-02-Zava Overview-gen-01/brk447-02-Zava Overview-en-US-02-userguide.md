# Video: [brk447-02-Zava Overview.mkv](./REPLACE_WITH_VIDEO_LINK) — 00:00:57

# SAVA E‑commerce Integration — User Manual

This manual documents the workflows demonstrated in the video and provides step‑by‑step instructions for using and validating the e‑commerce features in the SAVA application. Follow the steps below to reproduce the behavior shown, validate issues, and capture evidence for ticketing.

---

## Overview
(00:00:03.720 – 00:00:14.240)

This project involves migrating and integrating e‑commerce sites into the new SAVA experience. The manual covers:

- Adding items to the cart and verifying cart behavior
- Searching for items and validating actions shown in search results
- Checkout behavior and post‑checkout cart clearing
- Basic verification of localization / look‑and‑feel in the SAVA UI

Refer to timestamps in each section to match the video demonstration.

---

## Step‑by‑Step Instructions

### 1. Add products to the cart
(00:00:14.240 – 00:00:27.400)

Follow these steps to select products and add them to the cart.

1. Open the SAVA application and navigate to the target e‑commerce product listing page.
2. Locate a product (example shown: paint).
3. On the product listing, click the **Add to cart** button associated with the product.
4. Repeat for additional items (example shown: stain).
5. Verify the cart count icon updates to show the number of items added — the demonstration shows the cart count as **2**.

Tips:
- If the UI shows product details via a modal or detail page, the **Add to cart** action may be there as well; try both places.
- Use the cart count as a quick check; open the cart to confirm item details.

Warning:
- Perform these actions in a staging/dev environment when validating issues to avoid affecting real orders.

---

### 2. Search for a product and validate search results actions
(00:00:27.400 – 00:00:38.440)

The video demonstrates a search for “paint” where the **Add to cart** action is missing from search results. Use these steps to reproduce and log the issue.

1. In SAVA, focus the **Search field** and enter a product term (e.g., "paint").
2. Submit the search and observe the **Search results list**.
3. Check each result for an **Add to cart** action/button in the list view.
   - Expected: quick actions (like Add to cart) are available directly from search results.
   - Observed: **Add to cart** is missing in the results (issue).
4. Capture evidence:
   - Take a screenshot of the search results showing the missing button.
   - Note the exact search term and any filters applied.
   - Record the timestamp and environment (staging/dev/build/version).
5. Log or flag the issue with the team including:
   - Steps to reproduce
   - Screenshot(s)
   - Environment and version
   - Expected vs. actual behavior

Tip:
- Also check the browser console for JS errors while performing the search; include console logs in the ticket if present.

---

### 3. Review cart and proceed to checkout
(00:00:38.440 – 00:00:47.760)

Use these steps to view the cart contents and run the checkout flow as demonstrated.

1. Click the cart icon or cart link to open the **Cart view**.
2. Review the list of selected items, quantities, and prices.
3. Click **Proceed to checkout** to initiate the checkout flow.
4. After completing checkout (or simulating completion in staging), verify cart state:
   - Expected: cart should be cleared/reset after successful checkout.
   - Observed in demo: the cart clearing/reset action after checkout is **not yet implemented**.
5. If cart does not clear, capture evidence (screenshot) and log the behavior as a defect with steps and environment details.

Workaround:
- Manually remove items from the cart if you need an empty cart for repeated tests until automatic clearing is implemented.

Warning:
- Be cautious when using production systems for testing checkout flows to avoid creating actual orders or affecting inventory/pricing.

---

### 4. Validate localization and UI look-and-feel (SAVA)
(00:00:47.760 – 00:00:55.480)

The SAVA application requires checks for localization and general UI styling. Use these steps to verify locale and appearance.

1. In SAVA, open settings or the UI area that controls localization/locale where available.
2. Confirm locale selections (language, currency, date/time format) reflect expected values for the target market.
3. Browse product listings, search results, cart, and checkout screens:
   - Verify text strings, labels, and buttons are localized correctly.
   - Check styling consistency (fonts, spacing, button appearance).
4. Capture discrepancies:
   - Take screenshots showing mislocalized text or inconsistent styling.
   - Note the screen, component, and expected vs. actual text/styling.
5. Log tickets for localization or look‑and‑feel fixes with clear reproduction steps and evidence.

Tip:
- Validate localization across multiple locales if the site supports them (switch locale and repeat checks).

---

## Reporting Issues — Practical Checklist

When validating features and filing a ticket, include the following to speed resolution:

- Environment (staging/dev/production), build or version
- Exact steps to reproduce
- Timestamps (use the provided video timestamps for reference)
- Expected behavior vs. actual behavior
- Screenshots and console logs if available
- Any filters or special conditions used during testing

---

## Helpful Tips & Warnings

- Tip: Always test in a non‑production environment when possible to avoid creating real orders.
- Tip: Use consistent naming for tickets (e.g., "Search results missing Add to cart — product 'paint' — staging").
- Warning: Do not assume checkout cleared cart behavior — verify manually until automation is implemented.
- Warning: When capturing evidence, ensure sensitive information (user data, payment details) is not included in screenshots or logs.

---

End of manual — total demonstrated duration: 00:00:55.480.