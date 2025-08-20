#!/bin/bash

# Simple test validation script to check test syntax and structure

echo "🔍 Validating Playwright test files..."

# Check if Node.js is available
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is not installed"
    exit 1
fi

echo "✅ Node.js is available"

# Check if test files exist
if [ ! -f "tests/scenario1.spec.js" ]; then
    echo "❌ Scenario 1 test file not found"
    exit 1
fi

if [ ! -f "tests/scenario2.spec.js" ]; then
    echo "❌ Scenario 2 test file not found"
    exit 1
fi

echo "✅ Test files found"

# Check if utility files exist
if [ ! -f "utils/test-utils.js" ]; then
    echo "❌ Test utilities file not found"
    exit 1
fi

echo "✅ Utility files found"

# Validate JavaScript syntax
echo "🔍 Checking JavaScript syntax..."

node -c tests/scenario1.spec.js
if [ $? -eq 0 ]; then
    echo "✅ Scenario 1 test syntax is valid"
else
    echo "❌ Scenario 1 test has syntax errors"
    exit 1
fi

node -c tests/scenario2.spec.js
if [ $? -eq 0 ]; then
    echo "✅ Scenario 2 test syntax is valid"
else
    echo "❌ Scenario 2 test has syntax errors"
    exit 1
fi

node -c utils/test-utils.js
if [ $? -eq 0 ]; then
    echo "✅ Test utilities syntax is valid"
else
    echo "❌ Test utilities have syntax errors"
    exit 1
fi

# Check if configuration is valid
node -c playwright.config.js
if [ $? -eq 0 ]; then
    echo "✅ Playwright configuration is valid"
else
    echo "❌ Playwright configuration has syntax errors"
    exit 1
fi

# Check if test data exists
if [ ! -d "test-data" ] || [ -z "$(ls -A test-data)" ]; then
    echo "⚠️ Test data directory is empty or missing"
else
    echo "✅ Test data directory contains files"
fi

echo "🎉 All validation checks passed!"
echo ""
echo "Next steps:"
echo "1. Start the Zava application: cd ../src/ZavaAppHost && dotnet run"
echo "2. Install Playwright browsers: npm run install-browsers"
echo "3. Run tests: ./run-tests.sh"