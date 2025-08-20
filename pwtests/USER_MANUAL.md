# Zava AI-Powered eCommerce - User Manual

## 📖 Table of Contents
1. [Getting Started](#getting-started)
2. [Scenario 1: Single Agent Analysis](#scenario-1-single-agent-analysis)
3. [Scenario 2: Multi-Agent Orchestration](#scenario-2-multi-agent-orchestration)
4. [Troubleshooting](#troubleshooting)
5. [FAQ](#faq)

---

## 🚀 Getting Started

### Prerequisites
- The Zava application must be running
- Web browser (Chrome, Firefox, Safari, or Edge)
- Internet connection for AI services

### Accessing the Application
1. Open your web browser
2. Navigate to the application URL (typically `http://localhost:5000`)
3. You should see the Zava homepage

---

## 🎯 Scenario 1: Single Agent Analysis

### Overview
The Single Agent Analysis helps you get AI-powered tool recommendations for your home improvement projects. Simply upload an image of your project area and describe what you want to accomplish.

### Step-by-Step Guide

#### Step 1: Navigate to Single Agent Analysis
1. From the homepage, look for navigation menu
2. Click on "Scenario 1: Single Agent Analysis" or navigate to `/scenario1-single-agent`
3. You should see the page title "Scenario 1: Single Agent Analysis"

![Scenario 1 Page](../docs/user-manual-scenario1-page.png)

#### Step 2: Upload Project Image
1. Click on the "Choose File" button under "Project Image"
2. Select an image from your device showing the area you want to work on
3. Supported formats: JPG, PNG, GIF
4. Recommended: Clear, well-lit photos work best

![Image Upload](../docs/user-manual-image-upload.png)

#### Step 3: Describe Your Project
1. In the "Project Description" text area, describe your project
2. Be specific about:
   - What you want to accomplish
   - The scope of the work
   - Any special requirements
3. Example: "I want to paint this living room with a modern color scheme. The room has good natural light and I want to create a warm, inviting atmosphere."

![Project Description](../docs/user-manual-project-description.png)

#### Step 4: Enter Customer Information
1. Fill in your Customer ID in the designated field
2. Use format: CUST-XXX-XXX or any identifier you prefer
3. This helps track your project history

![Customer ID](../docs/user-manual-customer-id.png)

#### Step 5: Submit for Analysis
1. Click the "Analyze Project" button
2. Wait for the AI analysis to complete (this may take a few seconds)
3. The page will update with your results

![Submit Analysis](../docs/user-manual-submit-analysis.png)

#### Step 6: Review Results
The analysis results include:

**Image Analysis**
- AI description of what it sees in your image
- Project recommendations based on the visual analysis

**Your Existing Tools You Can Use**
- Green badges showing tools you already have that can be used
- Helps you avoid unnecessary purchases

**Recommended Tools to Purchase**
- Detailed cards showing suggested tools
- Includes prices and descriptions
- In-stock status indicators

![Analysis Results](../docs/user-manual-analysis-results.png)

### Tips for Best Results
- Use clear, well-lit photos
- Show the specific area you want to work on
- Be detailed in your project description
- Include any constraints or preferences

---

## 🤖 Scenario 2: Multi-Agent Orchestration

### Overview
The Multi-Agent Orchestration demonstrates coordinated AI agents working together to help you find products, check inventory, get location information, and navigate to items in the store.

### Step-by-Step Guide

#### Step 1: Navigate to Multi-Agent Orchestration
1. From the homepage or navigation menu
2. Click on "Scenario 2: Multi-Agent Orchestration" or navigate to `/scenario2-multi-agent`
3. You should see the page title "Scenario 2: Multi Agent Orchestration"

![Scenario 2 Page](../docs/user-manual-scenario2-page.png)

#### Step 2: Upload Product Image (Optional)
1. Click "Choose File" under "Product Image (Optional)"
2. Upload an image of the product you're looking for
3. This helps the AI better understand what you need

![Product Image Upload](../docs/user-manual-product-image.png)

#### Step 3: Enter User Information
1. Fill in your User ID (e.g., USER-001)
2. This identifies you in the system

![User ID](../docs/user-manual-user-id.png)

#### Step 4: Describe What You're Looking For
1. In the "Product Search" field, enter what you need
2. You can list multiple items separated by commas
3. Example: "paint sprayer, brushes, rollers, drop cloths, primer, wall paint"

![Product Search](../docs/user-manual-product-search.png)

#### Step 5: Location Services (Optional)
1. Check "Include my location for navigation" if you want navigation help
2. Enter your coordinates or click "Use Default Location"
3. This enables in-store navigation features

![Location Services](../docs/user-manual-location.png)

#### Step 6: Submit Request
1. Click "Submit Request" button
2. Watch the multi-agent orchestration in action
3. Multiple AI agents will work together to help you

![Submit Request](../docs/user-manual-submit-request.png)

#### Step 7: Review Multi-Agent Results
The orchestration results include:

**Agent Workflow Timeline**
- Shows the sequence of AI agents working on your request
- Different colored badges for different agent types:
  - 🔵 InventoryAgent: Searches for products
  - 🟢 MatchmakingAgent: Finds alternatives
  - 🔵 LocationAgent: Finds store locations
  - 🟡 NavigationAgent: Provides directions

![Agent Timeline](../docs/user-manual-agent-timeline.png)

**Product Recommendations**
- Cards showing found products
- Includes prices, availability, and location
- Stock status indicators (In Stock / Out of Stock)

![Product Results](../docs/user-manual-product-results.png)

**Navigation Instructions**
- Step-by-step directions to products
- Estimated time to reach items
- Landmarks and references to help you navigate

![Navigation Instructions](../docs/user-manual-navigation.png)

### Understanding the Agent Types
- **InventoryAgent**: Searches product database for matches
- **MatchmakingAgent**: Suggests alternative products when items are unavailable
- **LocationAgent**: Determines where products are located in the store
- **NavigationAgent**: Provides turn-by-turn directions

---

## 🚨 Troubleshooting

### Common Issues

#### "Service is currently unavailable"
- This message appears when AI services are not configured
- The application will show demo data instead
- Contact your administrator to configure AI API keys

#### Page Not Loading
- Ensure the application is running
- Check your internet connection
- Try refreshing the page
- Clear browser cache if needed

#### Image Upload Fails
- Check file size (should be under 10MB)
- Ensure file format is supported (JPG, PNG, GIF)
- Try a different image

#### Form Validation Errors
- Ensure all required fields are filled
- Check that Customer ID / User ID follows expected format
- Verify image is uploaded if required

#### Slow Response Times
- AI processing can take several seconds
- Poor internet connection may cause delays
- Large images take longer to process

### Error Messages

| Error Message | Cause | Solution |
|---------------|-------|----------|
| "Please provide a product search query" | Empty search field | Fill in the product search field |
| "Please provide a user ID" | Missing user ID | Enter a valid user ID |
| "Please select an image file" | No image uploaded | Upload an image file |
| "Analysis service is currently unavailable" | AI service not configured | Contact administrator or use demo mode |

---

## ❓ FAQ

### General Questions

**Q: Do I need special software to use Zava?**
A: No, you only need a modern web browser (Chrome, Firefox, Safari, or Edge).

**Q: Is my data stored permanently?**
A: This depends on your organization's configuration. Contact your administrator for data retention policies.

**Q: Can I use this on mobile devices?**
A: Yes, the application is responsive and works on mobile browsers.

### Scenario 1 Questions

**Q: What image formats are supported?**
A: JPG, PNG, and GIF formats are supported. Keep files under 10MB for best performance.

**Q: How accurate is the AI analysis?**
A: The AI analysis is designed to be helpful but should be verified by professionals for important projects.

**Q: Can I save my analysis results?**
A: Use your browser's save or print function to save results. Consider taking screenshots for future reference.

### Scenario 2 Questions

**Q: Why do some agents show "demo data"?**
A: When AI services aren't fully configured, the system shows realistic demo data for testing purposes.

**Q: How does location tracking work?**
A: Location services use coordinates you provide or default locations. No GPS tracking is performed.

**Q: Can I request products not in the inventory?**
A: The MatchmakingAgent will suggest alternatives if your requested items aren't available.

### Technical Questions

**Q: What browsers are supported?**
A: Modern versions of Chrome, Firefox, Safari, and Edge are fully supported.

**Q: Why do some features show demo results?**
A: Demo results appear when backend AI services are not configured with API keys.

**Q: Can I integrate this with other systems?**
A: The application provides REST APIs that can be integrated with other systems. Contact your technical team for details.

---

## 📞 Support

For technical support or questions about the Zava application:
1. Check this user manual first
2. Review the troubleshooting section
3. Contact your system administrator
4. Submit issues through your organization's support channels

---

*This user manual was generated for the Zava AI-Powered eCommerce Platform. Screenshots are captured during automated testing and may vary based on your specific deployment.*