# Zava Application - Playwright Test Automation & User Manual

This directory contains Playwright automation scripts for testing and documenting the Zava AI-powered eCommerce application scenarios.

## 📁 Directory Structure

```
pwtests/
├── package.json                 # NPM dependencies and scripts
├── playwright.config.js         # Playwright configuration
├── tests/                       # Test scripts
│   ├── scenario1.spec.js        # Single Agent Analysis tests
│   └── scenario2.spec.js        # Multi-Agent Orchestration tests
├── utils/                       # Utility functions
│   └── test-utils.js            # Common test helper functions
├── test-data/                   # Test images and data
│   ├── scenario01-01livingroom-01-small.png
│   ├── scenario01-03kitchen-01-small.png
│   └── ...                     # More test images
├── screenshots/                 # Captured screenshots during tests
├── test-results/                # Test reports and artifacts
└── README.md                   # This file
```

## 🚀 Prerequisites

1. **Start the Zava Application First**

   ```bash
   cd ../src/ZavaAppHost
   dotnet run
   ```

   The application should be running on `http://localhost:5000` (or check the console output for the actual URL)

2. **Install Dependencies**

   ```bash
   npm install
   npm run install-browsers
   ```

## 🎯 Available Scenarios

### Scenario 1: Single Agent Analysis

- **URL**: `/scenario1-single-agent`  
- **Description**: Upload a project image and get AI-powered tool recommendations
- **Features**:
  - Image upload and analysis
  - Project description input
  - Customer identification
  - Tool recommendation engine
  - Existing vs. recommended tools display

### Scenario 2: Multi-Agent Orchestration

- **URL**: `/scenario2-multi-agent`
- **Description**: Coordinated AI agents for product search and navigation
- **Features**:
  - Product image upload (optional)
  - Product search query
  - User identification
  - Location services (optional)
  - Multi-agent workflow visualization
  - Product recommendations
  - In-store navigation instructions

## 🧪 Running Tests

### Run All Tests

```bash
npm test
```

### Run Specific Scenario

```bash
npm run test:scenario1    # Single Agent Analysis
npm run test:scenario2    # Multi-Agent Orchestration
```

### Run with Browser Visible (Headed Mode)

```bash
npm run test:headed
```

### Debug Tests

```bash
npm run test:debug
```

### View Test Report

```bash
npm run report
```

## 📸 Screenshot Capture

The tests automatically capture screenshots at key points:

- Initial page load
- Form filling steps
- Before and after submissions
- Results display
- Error states

Screenshots are saved in the `screenshots/` directory with descriptive filenames including timestamps.

## 🛠️ Configuration

### Environment Variables

- `BASE_URL`: Application URL (default: `http://localhost:5000`)

### Browser Configuration

Tests run on multiple browsers:

- Chromium (Desktop)
- Firefox (Desktop)
- Safari (Desktop)
- Mobile Chrome
- Mobile Safari

### Timeouts

- Action timeout: 10 seconds
- Navigation timeout: 30 seconds
- Form submission timeout: 30 seconds

## 🚨 Error Handling

The tests include comprehensive error handling:

- Application health checks
- Form validation testing
- Network timeout handling
- Service unavailability detection
- Screenshot capture on errors

## 📊 Test Results

Test results are generated in multiple formats:

- HTML report (`test-results/html-report/`)
- JSON results (`test-results/results.json`)
- Console output
- Screenshots and videos on failure

## 🔧 Troubleshooting

### Application Not Running

```
❌ Application is not running. Please start the Zava application first.
💡 Run: cd src/ZavaAppHost && dotnet run
```

### Browser Installation Issues

```bash
npx playwright install
```

### Port Conflicts

Update the `BASE_URL` in playwright.config.js or set environment variable:

```bash
BASE_URL=http://localhost:3000 npm test
```

### Service Unavailability

The tests handle demo mode gracefully when backend services are not configured with AI keys.

## 🎭 Test Utilities

The `TestUtils` class provides:

- Automatic screenshot capture
- Form filling helpers
- Error detection
- Loading state management
- Element waiting functions
- Application health checks

## 📋 Manual Testing Checklist

Use this checklist when testing manually:

### Scenario 1: Single Agent Analysis

- [ ] Navigate to `/scenario1-single-agent`
- [ ] Upload project image
- [ ] Enter project description
- [ ] Enter customer ID
- [ ] Submit form
- [ ] Verify analysis results
- [ ] Check tool recommendations
- [ ] Verify error handling

### Scenario 2: Multi-Agent Orchestration  

- [ ] Navigate to `/scenario2-multi-agent`
- [ ] Upload product image (optional)
- [ ] Enter user ID
- [ ] Enter product search query
- [ ] Enable/disable location services
- [ ] Submit form
- [ ] Verify agent workflow timeline
- [ ] Check product recommendations
- [ ] Review navigation instructions
- [ ] Verify error handling

## 📈 Metrics Tracked

The tests track and report:

- Page load times
- Form submission times
- Number of agent interactions (Scenario 2)
- Number of product recommendations
- Navigation steps provided
- Error rates and types

## 🔄 Continuous Integration

The tests are configured for CI environments with:

- Retry logic for flaky tests
- Headless browser execution
- Artifact collection
- Parallel test execution

---

For more information about the Zava application, see the main README.md in the repository root.

## 🤖 Automated Analysis Prompt

If you need an automated analysis of failing tests and guidance to update the user manual, see `analysis_prompt.md` in this folder. It contains an LLM-friendly prompt describing how to run tests, collect logs, and produce PR-ready fixes and manual updates.
