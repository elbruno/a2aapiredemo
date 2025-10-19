# Demo Instructions: Web Page Indexing Feature

## Quick Start Demo

### Prerequisites
1. **.NET 9 SDK** installed
2. **Azure OpenAI** connection configured (or OpenAI API key)
3. **Environment variables** set:
   - `OPENAI_API_KEY` or Azure OpenAI connection string
   - `AI_ChatDeploymentName` (e.g., "gpt-4o-mini")
   - `AI_embeddingsDeploymentName` (e.g., "text-embedding-ada-002")

### Running the Application

1. **Start the Aspire AppHost**:
   ```bash
   cd src/eShopAppHost
   dotnet run
   ```

2. **Access the Aspire Dashboard**: 
   - Open browser to displayed dashboard URL (typically http://localhost:15888)
   - Verify all services are running:
     - ✅ **products** - Product service
     - ✅ **datasources** - Web page indexing service  
     - ✅ **store** - Frontend application
     - ✅ **realtimestore** - Real-time chat (optional)

3. **Open the Store Application**:
   - Click on the **store** service endpoint
   - Should open at http://localhost:5000 (or similar)

## Demo Walkthrough

### Part 1: Index Web Pages

1. **Navigate to Settings**:
   - Click **"Settings"** in the main navigation
   - You'll see the URL indexing form

2. **Add Sample URLs** (suggested):
   ```
   https://docs.microsoft.com/en-us/azure/
   https://github.com/microsoft/semantic-kernel
   https://learn.microsoft.com/en-us/dotnet/core/
   ```

3. **Index the URLs**:
   - Click **"Index Web Pages"**
   - Watch the progress spinner
   - View results table showing:
     - ✅ Success status for each URL
     - Number of content chunks created
     - Page titles extracted
   - Check the sidebar for indexed pages list

### Part 2: Enhanced Search

1. **Navigate to Search**:
   - Click **"Search"** in the main navigation

2. **Test Product Search** (baseline):
   - Enter: `"camping gear"`
   - Keep "Include Web Pages" **OFF**
   - Click **Search**
   - See traditional product results

3. **Test Combined Search**:
   - Enter: `"What is Azure?"`
   - Toggle **"Include Web Pages"** ON
   - Click **Search**
   - Observe two result sections:
     - **AI Response**: Traditional product context
     - **Web Page Insights**: Information from indexed Microsoft docs

4. **Test Semantic Understanding**:
   - Try: `"How to use .NET Core?"`
   - Should get relevant information from indexed .NET documentation
   - Try: `"Semantic Kernel features"`
   - Should reference the GitHub repository content

### Part 3: Content Management

1. **View Indexed Content**:
   - Return to **Settings** page
   - Check "Currently Indexed Pages" sidebar
   - See all URLs with metadata:
     - Page titles
     - Domain names  
     - Chunk counts
     - Indexing timestamps

2. **Re-index Content**:
   - Enter the same URL again
   - System will replace old content with fresh crawl
   - Useful for updating dynamic content

## API Testing (Optional)

If you want to test the DataSources API directly:

### Check Service Health
```bash
curl http://localhost:5001/api/datasources/health
```

### Index URLs Programmatically
```bash
curl -X POST http://localhost:5001/api/datasources/index \
  -H "Content-Type: application/json" \
  -d '{
    "urls": [
      "https://docs.microsoft.com/en-us/azure/cognitive-services/",
      "https://github.com/microsoft/semantic-kernel/blob/main/README.md"
    ]
  }'
```

### Search Web Content
```bash
curl "http://localhost:5001/api/datasources/search/What%20is%20Azure%20Cognitive%20Services"
```

### List Indexed URLs
```bash
curl http://localhost:5001/api/datasources/indexed
```

## Expected Results

### Successful Indexing
- **Status**: Green "Success" badges
- **Chunks**: Numbers > 0 (typically 5-20 per page)
- **Titles**: Meaningful page titles extracted
- **Timestamps**: Recent indexing times

### Quality Search Results
- **Relevant Responses**: AI should provide contextual answers
- **Source Attribution**: Responses mention source URLs
- **Combined Context**: Both product and web page information
- **Fast Performance**: Sub-second response times

### Error Scenarios to Test
1. **Invalid URLs**: Try `"not-a-url"` - should show validation error
2. **Unreachable Pages**: Try `"https://this-domain-does-not-exist.invalid"` - should show crawl failure
3. **Empty Search**: Search without indexing any URLs - should show "no pages indexed" message

## Troubleshooting

### Common Issues

#### Services Not Starting
- **Check logs** in Aspire dashboard
- **Verify .NET 9** is installed: `dotnet --version`
- **Check environment variables** for OpenAI configuration

#### Indexing Failures  
- **Network connectivity**: Test URLs in browser first
- **Content extraction**: Some sites block crawlers
- **JavaScript-heavy sites**: May not extract content properly

#### Search Not Working
- **OpenAI configuration**: Verify API keys and deployment names
- **Service communication**: Check Aspire service discovery
- **Empty results**: Ensure URLs were successfully indexed first

### Performance Notes
- **First search may be slow**: OpenAI embeddings generation
- **Subsequent searches faster**: Vector similarity calculations
- **Memory usage**: Content stored in RAM, lost on restart
- **Concurrent users**: Service handles multiple simultaneous requests

## Demo Script

### 5-Minute Demo
1. **Show Settings page** (30 seconds)
   - "Users can add up to 5 web page URLs"
   - Add sample Microsoft documentation URLs
   
2. **Index URLs** (2 minutes)
   - Click "Index Web Pages"
   - Explain crawling and chunking process
   - Show successful results table

3. **Demonstrate Search** (2 minutes)
   - Search for ".NET Core features"
   - Show both product and web page results
   - Highlight source URL attribution

4. **Show Architecture** (30 seconds)
   - Quick view of Aspire dashboard
   - Explain microservices and vector storage

### Key Talking Points
- **Semantic Search**: Vector embeddings find conceptually similar content
- **Real-time Integration**: Immediate availability in search results
- **Source Attribution**: Always shows where information came from
- **Scalable Architecture**: Aspire orchestration, service discovery
- **User Control**: Users choose which content to index

This demo showcases a practical implementation of RAG (Retrieval-Augmented Generation) patterns using .NET 9, Aspire, and modern LLM capabilities.