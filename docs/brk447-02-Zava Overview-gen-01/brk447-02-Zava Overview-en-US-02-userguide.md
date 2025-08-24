# SAVA Migration — E-commerce Behavior User Manual

Version: 1.0  
Video reference duration: 00:00:55.480

---

## Overview
(References: 00:00:03.720 — 00:00:13.492)

This manual documents the user flows and validation steps demonstrated during migration to the SAVA e-commerce experience. It covers:

- Adding products to the cart
- Searching for products and validating UI behavior
- Reviewing cart and proceeding to checkout
- How to validate and capture issues for the development team

Primary UI elements referenced:
- Product list / product tiles
- Add to cart button
- Cart (cart icon / cart view)
- Search field / search results
- Proceed to checkout button
- SAVA application (project area / localization settings)

Use this guide to reproduce the behaviors shown in the video and to collect consistent diagnostic information when validating or reporting bugs.

---

## Step-by-step instructions

### A. Add products to cart
(Reference: 00:00:14.240 — 00:00:27.400)

Purpose: Verify that products can be added to the cart and that the cart reflects the correct quantity.

Steps:
1. Open the SAVA application and navigate to the product listing page.
2. Locate the product tile for the desired item (example shown: paint products).
3. Click the product tile to view details (if applicable).
4. Click the **Add to cart** button on the product tile or product detail view.
5. Repeat step 4 for a second product (same or different product).
6. Click the cart icon or open the cart view to verify contents.

Expected result:
- The cart should display two products (or the number of products you added).
- Each item should show correct name, quantity, and price.

Tip:
- If the cart count does not update visually, refresh the cart view or check the cart details page to confirm the server-side cart state.

Warning:
- Do not perform tests in production with real user accounts or payment methods. Use a staging/test environment.

---

### B. Search results: verify Add to cart visibility
(Reference: 00:00:27.400 — 00:00:38.440)

Purpose: Confirm the search results display the same add-to-cart option as the main product list.

Steps:
1. In the SAVA application, locate the search field.
2. Enter the product name or keyword (example: "paint") and submit the search.
3. Inspect the search results list for each product tile/item.
4. Look specifically for the **Add to cart** button on the product entries in the search results.

Expected/Observed behavior:
- Expected: Each search result should include an **Add to cart** option, consistent with the product listing.
- Observed in the video: The **Add to cart** option is missing from search results — this should be validated.

Validation action:
- If the button is missing, create a validation ticket describing:
  - Steps to reproduce (include search term, page URL, user role)
  - Environment (staging/branch/build)
  - Screenshot showing search results without the button
  - Browser and device used

Tip:
- Use browser developer tools to check whether the add-to-cart element is present but hidden (CSS/display issues) or absent from DOM.

---

### C. Cart -> Proceed to checkout: validate behavior
(Reference: 00:00:38.440 — 00:00:47.760)

Purpose: Confirm that proceeding to checkout preserves cart contents and triggers the checkout workflow correctly.

Steps:
1. With items in the cart (see Section A), open the cart view.
2. Review items and quantities to ensure they are correct.
3. Click the **Proceed to checkout** button.
4. Observe the result after clicking.

Expected behavior:
- The checkout process should begin and the cart contents should remain intact through the checkout flow.

Observed behavior in the video:
- Clicking **Proceed to checkout** clears the cart unexpectedly. This behavior is not implemented correctly and requires attention.

Validation action:
- Reproduce the issue and collect the following:
  - Exact steps taken (including any navigation between pages)
  - Time and environment (staging/build)
  - Screenshots or screen recording showing items in cart before and after clicking checkout
  - Browser console logs and network requests (especially API calls to cart/checkout endpoints)
  - Any server-side error logs if accessible

Warning:
- If you see this behavior in production, suspend further checkout attempts and notify the development/ops team immediately to avoid data loss or customer impact.

Tip:
- Check whether an authentication/authorization redirect clears session data — try reproducing as both authenticated and unauthenticated users.

---

### D. Localization / General UI "local feel" checks
(Reference: 00:00:47.760 — 00:00:55.480)

Purpose: Ensure UI language, formatting, and local settings feel correct for target locales during SAVA migration.

Steps:
1. In SAVA, switch the UI language or locale setting (if available).
2. Navigate through product listing, search, cart, and checkout screens.
3. Observe translations, date/number formatting, currency display, labels, and button texts (e.g., **Add to cart**, **Proceed to checkout**).
4. Flag any inconsistent or missing translations and any UI phrasing that does not match local expectations.

What to report:
- Screenshots of inconsistent text or formatting
- Exact locale and user settings used during the check
- Specific UI element paths (page and element)

Tip:
- Keep a short checklist of locales to validate and work with localization/UX teams to confirm phrasing.

---

### E. How to document and file a validation/bug report (general)
(Use for any issue found in Sections B–D)

Minimum information to include:
- Title: concise summary (e.g., "Search results missing Add to cart button for 'paint' — staging")
- Steps to reproduce: numbered steps (exact inputs and navigation)
- Expected result vs actual result
- Environment: SAVA app build, branch, staging/production, browser & version, OS/device
- Time of occurrence and video timestamp reference (if recording the reproduction)
- Attachments: screenshots, short screen recording, console logs, network request trace (HAR), server logs if available
- Priority and impact: how this affects user flow (e.g., blocks checkout)

Tip:
- Prefix the ticket with the video timestamp that corresponds to the behavior (e.g., "see 00:00:27 — 00:00:38 for missing Add to cart in search results").

Warning:
- Avoid modifying code or clearing logs while reproducing the issue; keep the environment state as-is until diagnostics are captured.

---

## Quick reference: Video timestamps
- 00:00:03 — 00:00:13: Overview / migration to SAVA
- 00:00:14 — 00:00:27: Add products to cart demonstration
- 00:00:27 — 00:00:38: Search results missing Add to cart (bug/validation)
- 00:00:38 — 00:00:47: Cart -> Proceed to checkout clears cart (bug)
- 00:00:47 — 00:00:55: Localization and SAVA application areas to fix

---

If you need a template for bug tickets or a checklist for locale testing, ask and a ready-to-use template will be provided.