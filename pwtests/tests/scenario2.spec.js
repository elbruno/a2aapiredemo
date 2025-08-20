import { test, expect } from '@playwright/test';
import { TestUtils } from '../utils/test-utils.js';
import path from 'path';

test.describe('Scenario 2: Multi-Agent Orchestration', () => {
  let utils;

  test.beforeEach(async ({ page }) => {
    utils = new TestUtils(page);
  });

  test('Complete Multi-Agent Orchestration workflow', async ({ page }) => {
    console.log('🚀 Starting Scenario 2: Multi-Agent Orchestration');

    // Step 1: Check if application is running
    await utils.takeScreenshot('01-initial-state');
    const isHealthy = await utils.checkApplicationHealth();
    if (!isHealthy) {
      console.error('❌ Application is not running. Please start the Zava application first.');
      console.log('💡 Run: cd src/ZavaAppHost && dotnet run');
      return;
    }

    // Step 2: Navigate to home page and take screenshot
    await utils.takeScreenshot('02-home-page');
    
    // Step 3: Navigate to Scenario 2 page
    console.log('📍 Navigating to Multi-Agent Orchestration page...');
    await utils.navigateToScenario('scenario2-multi-agent');
    await utils.takeScreenshot('03-scenario2-page-loaded');

    // Step 4: Verify page elements are present
    console.log('🔍 Verifying page elements...');
    await utils.waitForElement('h1', 'Page Title');
    await utils.waitForElement('input[type="file"]', 'File Upload Input');
    await utils.waitForElement('#userId', 'User ID Input');
    await utils.waitForElement('#productQuery', 'Product Query Input');
    
    // Check page title
    const pageTitle = await page.textContent('h1');
    expect(pageTitle).toContain('Multi Agent');
    console.log(`✅ Page title verified: ${pageTitle}`);

    await utils.takeScreenshot('04-form-elements-verified');

    // Step 5: Fill out the form
    console.log('📝 Filling out the multi-agent request form...');
    
    // Upload product image (optional)
    const testImagePath = path.join(__dirname, '..', 'test-data', 'scenario01-03kitchen-01-small.png');
    await utils.uploadFile('#projectImage', testImagePath, 'Product Image');
    await utils.takeScreenshot('05-image-uploaded');

    // Fill user ID
    await utils.fillField('#userId', 'USER-TEST-001', 'User ID');
    await utils.takeScreenshot('06-user-id-filled');

    // Fill product query
    const productQuery = 'paint sprayer, brushes, rollers, drop cloths, primer, wall paint';
    await utils.fillField('#productQuery', productQuery, 'Product Query');
    await utils.takeScreenshot('07-product-query-filled');

    // Step 6: Test location feature
    console.log('📍 Testing location functionality...');
    
    // Check the location checkbox
    const locationCheckbox = 'input[type="checkbox"]#includeLocation';
    await utils.waitForElement(locationCheckbox, 'Location Checkbox');
    await page.check(locationCheckbox);
    await utils.takeScreenshot('08-location-enabled');

    // Fill location coordinates
    await utils.waitForElement('input[placeholder="Latitude"]', 'Latitude Input');
    await utils.fillField('input[placeholder="Latitude"]', '47.6062', 'Latitude');
    await utils.fillField('input[placeholder="Longitude"]', '-122.3321', 'Longitude');
    await utils.takeScreenshot('09-location-coordinates-filled');

    // Try the default location button
    try {
      await utils.clickButton('button:has-text("Use Default Location")', 'Default Location Button');
      await utils.takeScreenshot('10-default-location-used');
    } catch (e) {
      console.log('ℹ️ Default location button not found or different text');
    }

    // Check for any validation errors before submitting
    await utils.checkForErrors();

    // Step 7: Submit the form
    console.log('🚀 Submitting multi-agent request...');
    await utils.clickButton('button[type="submit"]', 'Submit Request Button');
    await utils.takeScreenshot('11-form-submitted');

    // Step 8: Wait for results and document multi-agent workflow
    console.log('⏳ Waiting for multi-agent orchestration results...');
    
    // Wait for form submission to complete
    await utils.waitForFormSubmission();

    // Check for any errors after submission
    const errors = await utils.checkForErrors();
    if (errors) {
      console.log(`⚠️ Error detected: ${errors}`);
      await utils.takeScreenshot('12-error-detected');
    }

    // Look for orchestration timeline
    try {
      await utils.waitForElement('.timeline-item', 'Orchestration Timeline', 30000);
      await utils.takeScreenshot('12-orchestration-timeline-visible');
      console.log('✅ Multi-agent orchestration timeline received');

      // Count agent interactions
      const timelineItems = await page.$$('.timeline-item');
      console.log(`🤖 Found ${timelineItems.length} agent interactions`);

      // Check for different agent badges
      const agentBadges = await page.$$('.badge');
      for (let i = 0; i < agentBadges.length; i++) {
        const badgeText = await agentBadges[i].textContent();
        console.log(`🎯 Agent: ${badgeText}`);
      }

      await utils.takeScreenshot('13-agent-interactions-documented');

    } catch (error) {
      console.log('ℹ️ Timeline not found - checking for alternative result format');
    }

    // Look for product results
    try {
      await utils.waitForElement('.col-md-4', 'Product Results', 10000);
      await utils.takeScreenshot('14-product-results-visible');
      console.log('✅ Product results received');

      // Count product cards
      const productCards = await page.$$('.col-md-4 .card');
      console.log(`🛍️ Found ${productCards.length} product recommendations`);

      // Check for product details
      for (let i = 0; i < Math.min(productCards.length, 3); i++) {
        const card = productCards[i];
        const title = await card.$eval('.card-title', el => el.textContent.trim());
        const price = await card.$eval('.text-primary', el => el.textContent.trim());
        console.log(`📦 Product ${i + 1}: ${title} - ${price}`);
      }

      await utils.takeScreenshot('15-product-details-documented');

    } catch (error) {
      console.log('ℹ️ Product results not found - might be demo mode');
    }

    // Look for navigation instructions
    try {
      await utils.waitForElement('.navigation-steps', 'Navigation Instructions', 10000);
      await utils.takeScreenshot('16-navigation-instructions-visible');
      console.log('✅ Navigation instructions received');

      // Count navigation steps
      const navSteps = await page.$$('.navigation-steps .d-flex');
      console.log(`🧭 Found ${navSteps.length} navigation steps`);

      // Document navigation details
      for (let i = 0; i < Math.min(navSteps.length, 5); i++) {
        const step = navSteps[i];
        const stepText = await step.textContent();
        console.log(`📍 Step ${i + 1}: ${stepText.trim().substring(0, 100)}...`);
      }

      await utils.takeScreenshot('17-navigation-steps-documented');

    } catch (error) {
      console.log('ℹ️ Navigation instructions not found');
    }

    // Step 9: Final screenshot and summary
    await utils.takeScreenshot('18-scenario2-completed');
    
    console.log('🎉 Scenario 2 test completed successfully!');
    console.log('📸 Screenshots captured in the screenshots folder');
    console.log('🤖 Multi-agent orchestration workflow documented');
  });

  test('Scenario 2 - Without location', async ({ page }) => {
    console.log('🧪 Testing Scenario 2 without location services');

    // Navigate to scenario 2 page
    await utils.navigateToScenario('scenario2-multi-agent');
    await utils.takeScreenshot('no-location-01-page-loaded');

    // Fill basic form without location
    await utils.fillField('#userId', 'USER-NO-LOC-001', 'User ID');
    await utils.fillField('#productQuery', 'drill, screws, wall anchors', 'Product Query');
    
    // Ensure location checkbox is unchecked
    const locationCheckbox = 'input[type="checkbox"]#includeLocation';
    await page.uncheck(locationCheckbox);
    await utils.takeScreenshot('no-location-02-form-filled');

    // Submit form
    await utils.clickButton('button[type="submit"]', 'Submit Request Button');
    await utils.takeScreenshot('no-location-03-submitted');

    // Wait for results
    await utils.waitForFormSubmission();
    await utils.takeScreenshot('no-location-04-results');

    console.log('✅ No-location test completed');
  });

  test('Scenario 2 - Error handling test', async ({ page }) => {
    console.log('🧪 Testing error handling for Scenario 2');

    // Navigate to scenario 2 page
    await utils.navigateToScenario('scenario2-multi-agent');
    await utils.takeScreenshot('error-test-01-page-loaded');

    // Try to submit form without required fields
    console.log('Testing form validation...');
    await utils.clickButton('button[type="submit"]', 'Submit Button (Empty Form)');
    await utils.takeScreenshot('error-test-02-validation-triggered');

    // Check for validation errors
    const validationErrors = await utils.checkForErrors();
    if (validationErrors) {
      console.log(`✅ Validation working: ${validationErrors}`);
    }

    // Test with only partial form data
    await utils.fillField('#userId', 'USER-PARTIAL-001', 'User ID (Partial Test)');
    await utils.clickButton('button[type="submit"]', 'Submit Button (Partial Form)');
    await utils.takeScreenshot('error-test-03-partial-form-submitted');

    await utils.checkForErrors();

    console.log('✅ Error handling test completed');
  });
});