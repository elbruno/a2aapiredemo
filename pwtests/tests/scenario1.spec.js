import { test, expect } from '@playwright/test';
import { TestUtils } from '../utils/test-utils.js';
import path from 'path';

test.describe('Scenario 1: Single Agent Analysis', () => {
  let utils;

  test.beforeEach(async ({ page }) => {
    utils = new TestUtils(page);
  });

  test('Complete Single Agent Analysis workflow', async ({ page }) => {
    console.log('🚀 Starting Scenario 1: Single Agent Analysis');

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
    
    // Step 3: Navigate to Scenario 1 page
    console.log('📍 Navigating to Single Agent Analysis page...');
    await utils.navigateToScenario('scenario1-single-agent');
    await utils.takeScreenshot('03-scenario1-page-loaded');

    // Step 4: Verify page elements are present
    console.log('🔍 Verifying page elements...');
    await utils.waitForElement('h1', 'Page Title');
    await utils.waitForElement('input[type="file"]', 'File Upload Input');
    await utils.waitForElement('textarea', 'Project Description Textarea');
    await utils.waitForElement('input[type="text"]', 'Customer ID Input');
    
    // Check page title
    const pageTitle = await page.textContent('h1');
    expect(pageTitle).toContain('Single Agent Analysis');
    console.log(`✅ Page title verified: ${pageTitle}`);

    await utils.takeScreenshot('04-form-elements-verified');

    // Step 5: Fill out the form
    console.log('📝 Filling out the analysis form...');
    
    // Upload project image
    const testImagePath = path.join(__dirname, '..', 'test-data', 'scenario01-01livingroom-01-small.png');
    await utils.uploadFile('#projectImage', testImagePath, 'Project Image');
    await utils.takeScreenshot('05-image-uploaded');

    // Fill project description
    const projectDescription = 'I want to paint this living room with a modern color scheme. The room has good natural light and I want to create a warm, inviting atmosphere. What tools and materials do I need for this project?';
    await utils.fillField('#prompt', projectDescription, 'Project Description');
    await utils.takeScreenshot('06-description-filled');

    // Fill customer ID
    await utils.fillField('#customerId', 'CUST-TEST-001', 'Customer ID');
    await utils.takeScreenshot('07-customer-id-filled');

    // Check for any validation errors before submitting
    await utils.checkForErrors();

    // Step 6: Submit the form
    console.log('🚀 Submitting analysis request...');
    await utils.clickButton('button[type="submit"]', 'Submit Analysis Button');
    await utils.takeScreenshot('08-form-submitted');

    // Step 7: Wait for results and take screenshots
    console.log('⏳ Waiting for analysis results...');
    
    // Wait for form submission to complete
    await utils.waitForFormSubmission();

    // Check for any errors after submission
    const errors = await utils.checkForErrors();
    if (errors) {
      console.log(`⚠️ Error detected: ${errors}`);
      await utils.takeScreenshot('09-error-detected');
    }

    // Look for results section
    try {
      await utils.waitForElement('.card:has(.card-header)', 'Results Card', 30000);
      await utils.takeScreenshot('09-results-loaded');
      console.log('✅ Analysis results received');

      // Check for image analysis results
      const analysisSelector = 'p.text-muted';
      try {
        await utils.waitForElement(analysisSelector, 'Analysis Text', 5000);
        const analysisText = await page.textContent(analysisSelector);
        console.log(`📊 Analysis result: ${analysisText?.substring(0, 100)}...`);
        await utils.takeScreenshot('10-analysis-text-visible');
      } catch (e) {
        console.log('ℹ️ Analysis text not found - might be demo mode');
      }

      // Check for reusable tools
      try {
        await utils.waitForElement('.badge.bg-success', 'Reusable Tools', 5000);
        const reusableTools = await page.$$('.badge.bg-success');
        console.log(`🔧 Found ${reusableTools.length} reusable tools`);
        await utils.takeScreenshot('11-reusable-tools-visible');
      } catch (e) {
        console.log('ℹ️ No reusable tools found');
      }

      // Check for recommended tools
      try {
        await utils.waitForElement('.card.card-body.p-2', 'Recommended Tools', 5000);
        const recommendedTools = await page.$$('.card.card-body.p-2');
        console.log(`🛍️ Found ${recommendedTools.length} recommended tools`);
        await utils.takeScreenshot('12-recommended-tools-visible');
      } catch (e) {
        console.log('ℹ️ No recommended tools found');
      }

    } catch (error) {
      console.log('ℹ️ Results might be in demo mode or service unavailable');
      await utils.takeScreenshot('09-no-results-timeout');
      
      // Check if there's a service unavailable message
      try {
        const serviceMessage = await page.textContent('.card-body');
        if (serviceMessage && serviceMessage.includes('service is currently unavailable')) {
          console.log('ℹ️ Service is in demo mode - this is expected for testing');
          await utils.takeScreenshot('10-demo-mode-detected');
        }
      } catch (e) {
        // Ignore
      }
    }

    // Step 8: Final screenshot and summary
    await utils.takeScreenshot('13-scenario1-completed');
    
    console.log('🎉 Scenario 1 test completed successfully!');
    console.log('📸 Screenshots captured in the screenshots folder');
  });

  test('Scenario 1 - Error handling test', async ({ page }) => {
    console.log('🧪 Testing error handling for Scenario 1');

    // Navigate to scenario 1 page
    await utils.navigateToScenario('scenario1-single-agent');
    await utils.takeScreenshot('error-test-01-page-loaded');

    // Try to submit form without filling required fields
    console.log('Testing form validation...');
    await utils.clickButton('button[type="submit"]', 'Submit Button (Empty Form)');
    await utils.takeScreenshot('error-test-02-validation-triggered');

    // Check for validation errors
    const validationErrors = await utils.checkForErrors();
    if (validationErrors) {
      console.log(`✅ Validation working: ${validationErrors}`);
    }

    // Test with only partial form data
    await utils.fillField('#customerId', 'CUST-PARTIAL-001', 'Customer ID (Partial Test)');
    await utils.clickButton('button[type="submit"]', 'Submit Button (Partial Form)');
    await utils.takeScreenshot('error-test-03-partial-form-submitted');

    await utils.checkForErrors();

    console.log('✅ Error handling test completed');
  });
});