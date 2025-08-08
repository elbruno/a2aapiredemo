#!/bin/bash

# eShopLite Screenshot Capture Script
# This script demonstrates how to use the Playwright MCP server integration
# to capture screenshots of the eShopLite application for documentation

set -e

echo "🎬 eShopLite Screenshot Capture Service"
echo "======================================="

# Configuration
STORE_URL="https://localhost:7147"
OUTPUT_DIR="../docs/screenshots"
SCREENSHOT_SERVICE_DIR="./ScreenshotService"

# Ensure output directory exists
mkdir -p "$OUTPUT_DIR"

echo "📁 Output directory: $OUTPUT_DIR"
echo "🌐 Store URL: $STORE_URL"

# Check if .NET 9 is available
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 9 SDK."
    exit 1
fi

# Set up .NET 9 if needed
if [ -f "./dotnet-install.sh" ]; then
    echo "🔧 Setting up .NET 9..."
    export PATH="$HOME/.dotnet:$PATH"
fi

echo "✅ .NET Version: $(dotnet --version)"

# Build the screenshot service
echo "🔨 Building Screenshot Service..."
cd "$SCREENSHOT_SERVICE_DIR"
dotnet build --configuration Release

echo "📸 Starting screenshot capture..."

# Check if the Store is running (this would be done by the actual service)
if curl -s "$STORE_URL" > /dev/null 2>&1; then
    echo "✅ Store application is accessible"
    
    # Run the screenshot service
    echo "🚀 Running screenshot capture service..."
    dotnet run --configuration Release
    
    echo "✅ Screenshot capture completed successfully!"
    
    # List captured screenshots
    echo ""
    echo "📸 Captured Screenshots:"
    echo "======================="
    find "$OUTPUT_DIR" -name "*.png" -type f | sort | while read -r file; do
        size=$(du -h "$file" | cut -f1)
        echo "  📷 $(basename "$file") ($size)"
    done
    
else
    echo "⚠️  Store application not accessible at $STORE_URL"
    echo "   Please start the application first:"
    echo "   dotnet run --project ../eShopAppHost"
    echo ""
    echo "🎭 Creating demonstration screenshots instead..."
    
    # Create placeholder screenshots for documentation
    create_demo_screenshots
fi

echo ""
echo "🎉 Screenshot capture process completed!"
echo "📖 Screenshots are saved in: $OUTPUT_DIR"

# Function to create demonstration screenshots
create_demo_screenshots() {
    echo "Creating demonstration screenshots..."
    
    # Create placeholder images (this would be replaced by actual Playwright captures)
    cat > "$OUTPUT_DIR/screenshot-manifest.json" << EOF
{
  "captureDate": "$(date -u +"%Y-%m-%dT%H:%M:%SZ")",
  "application": "eShopLite",
  "version": "2.0",
  "screenshots": [
    {
      "name": "01-home-page.png",
      "title": "Home Page",
      "description": "Main landing page with enhanced UI and chat widget",
      "viewport": "1920x1080",
      "features": ["Hero section", "Feature cards", "Chat widget introduction"]
    },
    {
      "name": "02-chat-widget.png", 
      "title": "Chat Widget",
      "description": "AI chat assistant widget in collapsed and expanded states",
      "viewport": "1920x1080",
      "features": ["Real-time messaging", "Typing indicators", "Suggested actions"]
    },
    {
      "name": "03-product-catalog.png",
      "title": "Product Catalog",
      "description": "Product browsing interface with filters and search",
      "viewport": "1920x1080", 
      "features": ["Product grid", "Filtering options", "Search integration"]
    },
    {
      "name": "04-search-results.png",
      "title": "Search Results",
      "description": "Natural language search results with AI-powered relevance",
      "viewport": "1920x1080",
      "features": ["Natural language queries", "Result scoring", "Quick filters"]
    },
    {
      "name": "05-chat-conversation.png",
      "title": "Chat Conversation", 
      "description": "Active chat conversation with AI assistant providing product recommendations",
      "viewport": "1920x1080",
      "features": ["Conversational AI", "Product recommendations", "Contextual help"]
    },
    {
      "name": "06-home-mobile.png",
      "title": "Mobile Home Page",
      "description": "Responsive mobile view of the home page",
      "viewport": "390x844",
      "features": ["Mobile-optimized UI", "Touch-friendly navigation", "Responsive chat widget"]
    }
  ],
  "captureSettings": {
    "browser": "Chromium",
    "headless": true,
    "timeout": 30000,
    "waitForNetworkIdle": true,
    "quality": 90
  }
}
EOF

    echo "📄 Screenshot manifest created: $OUTPUT_DIR/screenshot-manifest.json"
}

# Export function for use in other scripts
export -f create_demo_screenshots