import { expect } from '@playwright/test';
import fs from 'fs';
import path from 'path';

/**
 * Utility class for Playwright test helpers
 */
export class TestUtils {
  constructor(page) {
    this.page = page;
    this.screenshotCounter = 0;
    this.screenshotDir = path.join(__dirname, '..', 'screenshots');
    
    // Ensure screenshots directory exists
    if (!fs.existsSync(this.screenshotDir)) {
      fs.mkdirSync(this.screenshotDir, { recursive: true });
    }
  }

  /**
   * Take a screenshot with automatic naming
   * @param {string} description - Description for the screenshot
   * @param {Object} options - Screenshot options
   */
  async takeScreenshot(description, options = {}) {
    this.screenshotCounter++;
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    const filename = `${this.screenshotCounter.toString().padStart(2, '0')}-${description.replace(/[^a-zA-Z0-9]/g, '-')}-${timestamp}.png`;
    const filePath = path.join(this.screenshotDir, filename);
    
    await this.page.screenshot({
      path: filePath,
      fullPage: true,
      ...options
    });
    
    console.log(`📷 Screenshot taken: ${filename}`);
    return { filename, filePath };
  }

  /**
   * Wait for page to load completely
   */
  async waitForPageLoad() {
    await this.page.waitForLoadState('networkidle');
    await this.page.waitForLoadState('domcontentloaded');
  }

  /**
   * Fill form field with error handling
   * @param {string} selector - Element selector
   * @param {string} value - Value to fill
   * @param {string} fieldName - Field name for error reporting
   */
  async fillField(selector, value, fieldName) {
    try {
      await this.page.waitForSelector(selector, { timeout: 5000 });
      await this.page.fill(selector, value);
      console.log(`✅ Filled ${fieldName}: ${value}`);
    } catch (error) {
      console.error(`❌ Failed to fill ${fieldName}:`, error.message);
      await this.takeScreenshot(`error-filling-${fieldName}`);
      throw error;
    }
  }

  /**
   * Click button with error handling
   * @param {string} selector - Button selector
   * @param {string} buttonName - Button name for error reporting
   */
  async clickButton(selector, buttonName) {
    try {
      await this.page.waitForSelector(selector, { timeout: 5000 });
      await this.page.click(selector);
      console.log(`✅ Clicked ${buttonName}`);
    } catch (error) {
      console.error(`❌ Failed to click ${buttonName}:`, error.message);
      await this.takeScreenshot(`error-clicking-${buttonName}`);
      throw error;
    }
  }

  /**
   * Upload file with error handling
   * @param {string} selector - File input selector
   * @param {string} filePath - Path to file to upload
   * @param {string} fieldName - Field name for error reporting
   */
  async uploadFile(selector, filePath, fieldName) {
    try {
      await this.page.waitForSelector(selector, { timeout: 5000 });
      await this.page.setInputFiles(selector, filePath);
      console.log(`✅ Uploaded file to ${fieldName}: ${path.basename(filePath)}`);
    } catch (error) {
      console.error(`❌ Failed to upload file to ${fieldName}:`, error.message);
      await this.takeScreenshot(`error-uploading-${fieldName}`);
      throw error;
    }
  }

  /**
   * Wait for element to be visible
   * @param {string} selector - Element selector
   * @param {string} elementName - Element name for error reporting
   * @param {number} timeout - Timeout in milliseconds
   */
  async waitForElement(selector, elementName, timeout = 10000) {
    try {
      await this.page.waitForSelector(selector, { state: 'visible', timeout });
      console.log(`✅ Element visible: ${elementName}`);
    } catch (error) {
      console.error(`❌ Element not found: ${elementName}`, error.message);
      await this.takeScreenshot(`error-waiting-for-${elementName}`);
      throw error;
    }
  }

  /**
   * Check if application is running
   */
  async checkApplicationHealth() {
    try {
      await this.page.goto('/', { waitUntil: 'networkidle' });
      await this.page.waitForSelector('body', { timeout: 10000 });
      console.log('✅ Application is running');
      return true;
    } catch (error) {
      console.error('❌ Application is not running or not accessible:', error.message);
      await this.takeScreenshot('application-not-running');
      return false;
    }
  }

  /**
   * Navigate to a specific scenario page
   * @param {string} scenario - Scenario path (scenario1-single-agent or scenario2-multi-agent)
   */
  async navigateToScenario(scenario) {
    try {
      await this.page.goto(`/${scenario}`, { waitUntil: 'networkidle' });
      await this.waitForPageLoad();
      console.log(`✅ Navigated to ${scenario}`);
    } catch (error) {
      console.error(`❌ Failed to navigate to ${scenario}:`, error.message);
      await this.takeScreenshot(`error-navigating-to-${scenario}`);
      throw error;
    }
  }

  /**
   * Check for error messages on the page
   */
  async checkForErrors() {
    const errorSelectors = [
      '.alert-danger',
      '.error',
      '.validation-message',
      '[role="alert"]'
    ];

    for (const selector of errorSelectors) {
      try {
        const errorElement = await this.page.$(selector);
        if (errorElement) {
          const errorText = await errorElement.textContent();
          console.warn(`⚠️ Error found on page: ${errorText}`);
          await this.takeScreenshot('page-error-detected');
          return errorText;
        }
      } catch (e) {
        // Ignore - selector not found
      }
    }
    return null;
  }

  /**
   * Wait for form submission to complete
   */
  async waitForFormSubmission() {
    try {
      // Wait for any loading indicators to appear and disappear
      await this.page.waitForFunction(() => {
        const loadingElements = document.querySelectorAll('[disabled], .spinner, .loading');
        return loadingElements.length === 0;
      }, { timeout: 30000 });
      
      // Wait for network to be idle
      await this.page.waitForLoadState('networkidle');
      console.log('✅ Form submission completed');
    } catch (error) {
      console.error('❌ Form submission timeout:', error.message);
      await this.takeScreenshot('form-submission-timeout');
      throw error;
    }
  }
}