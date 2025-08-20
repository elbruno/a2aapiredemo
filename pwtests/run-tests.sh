#!/bin/bash

# Zava Application - Playwright Test Runner Script
# This script helps run Playwright tests with proper setup and error handling

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
BASE_URL=${BASE_URL:-"http://localhost:5000"}
TEST_MODE=${1:-"all"}
HEADED=${HEADED:-false}

echo -e "${BLUE}🎭 Zava Playwright Test Runner${NC}"
echo "=================================="

# Function to check if application is running
check_application() {
    echo -e "${YELLOW}🔍 Checking if Zava application is running...${NC}"
    
    if curl -s --connect-timeout 5 "$BASE_URL" > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Application is running at $BASE_URL${NC}"
        return 0
    else
        echo -e "${RED}❌ Application is not running at $BASE_URL${NC}"
        echo -e "${YELLOW}💡 To start the application:${NC}"
        echo "   cd ../src/ZavaAppHost"
        echo "   dotnet run"
        echo ""
        echo -e "${YELLOW}⏳ Waiting 30 seconds for you to start the application...${NC}"
        
        # Wait and check again
        for i in {30..1}; do
            echo -ne "${YELLOW}Waiting... $i seconds remaining\r${NC}"
            sleep 1
            if curl -s --connect-timeout 2 "$BASE_URL" > /dev/null 2>&1; then
                echo -e "\n${GREEN}✅ Application is now running!${NC}"
                return 0
            fi
        done
        
        echo -e "\n${RED}❌ Application is still not running. Exiting...${NC}"
        return 1
    fi
}

# Function to install dependencies
install_dependencies() {
    echo -e "${YELLOW}📦 Installing dependencies...${NC}"
    
    if [ ! -d "node_modules" ]; then
        npm install --ignore-scripts
    fi
    
    # Check if browsers are installed
    if ! npx playwright --version > /dev/null 2>&1; then
        echo -e "${YELLOW}🌐 Installing Playwright browsers...${NC}"
        npx playwright install
    fi
}

# Function to run tests
run_tests() {
    local test_type=$1
    local headed_flag=""
    
    if [ "$HEADED" = "true" ]; then
        headed_flag="--headed"
    fi
    
    echo -e "${BLUE}🧪 Running tests: $test_type${NC}"
    
    case $test_type in
        "scenario1")
            npx playwright test scenario1.spec.js $headed_flag
            ;;
        "scenario2")
            npx playwright test scenario2.spec.js $headed_flag
            ;;
        "all")
            npx playwright test $headed_flag
            ;;
        *)
            echo -e "${RED}❌ Unknown test type: $test_type${NC}"
            echo "Available options: scenario1, scenario2, all"
            exit 1
            ;;
    esac
}

# Function to generate report
generate_report() {
    echo -e "${BLUE}📊 Generating test report...${NC}"
    
    if [ -d "test-results" ]; then
        npx playwright show-report --host 0.0.0.0 --port 9323 &
        REPORT_PID=$!
        
        echo -e "${GREEN}✅ Test report available at: http://localhost:9323${NC}"
        echo -e "${YELLOW}Press Ctrl+C to stop the report server${NC}"
        
        # Wait for interrupt
        trap "kill $REPORT_PID 2>/dev/null; exit 0" INT
        wait $REPORT_PID
    else
        echo -e "${YELLOW}⚠️ No test results found${NC}"
    fi
}

# Function to show usage
show_usage() {
    echo "Usage: $0 [test_type] [options]"
    echo ""
    echo "Test Types:"
    echo "  scenario1  - Run only Scenario 1 tests"
    echo "  scenario2  - Run only Scenario 2 tests"  
    echo "  all        - Run all tests (default)"
    echo ""
    echo "Environment Variables:"
    echo "  BASE_URL   - Application URL (default: http://localhost:5000)"
    echo "  HEADED     - Run tests in headed mode (default: false)"
    echo ""
    echo "Examples:"
    echo "  $0                           # Run all tests"
    echo "  $0 scenario1                 # Run only Scenario 1"
    echo "  HEADED=true $0 scenario2     # Run Scenario 2 in headed mode"
    echo "  BASE_URL=http://localhost:3000 $0  # Use different URL"
}

# Main execution
main() {
    # Handle help flag
    if [ "$1" = "-h" ] || [ "$1" = "--help" ]; then
        show_usage
        exit 0
    fi
    
    # Check if we're in the right directory
    if [ ! -f "package.json" ]; then
        echo -e "${RED}❌ Please run this script from the pwtests directory${NC}"
        exit 1
    fi
    
    # Install dependencies
    install_dependencies
    
    # Check if application is running
    if ! check_application; then
        exit 1
    fi
    
    # Clean previous results
    echo -e "${YELLOW}🧹 Cleaning previous test results...${NC}"
    rm -rf test-results/html-report test-results/artifacts screenshots/* 2>/dev/null || true
    mkdir -p screenshots
    
    # Run tests
    echo -e "${GREEN}🚀 Starting test execution...${NC}"
    run_tests "$TEST_MODE"
    
    # Show results
    echo -e "${GREEN}🎉 Test execution completed!${NC}"
    echo ""
    echo -e "${BLUE}📸 Screenshots saved in: screenshots/${NC}"
    echo -e "${BLUE}📊 Test results saved in: test-results/${NC}"
    
    # Ask if user wants to see the report
    read -p "Do you want to view the test report? (y/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        generate_report
    fi
}

# Run main function
main "$@"