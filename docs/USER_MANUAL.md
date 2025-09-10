# User Manual: Web Page Indexing and Enhanced Search

## Overview

RealTimeStore now supports indexing web pages to enhance your AI chat and search experience. You can add up to 5 web page URLs, and their content will be made available for semantic search alongside product information.

## Getting Started

### Prerequisites
- Access to RealTimeStore application
- Valid web page URLs (HTTP/HTTPS)
- Internet connectivity for web crawling

### Accessing the Settings Page

1. **Navigate to Settings**
   - Open RealTimeStore in your web browser
   - Click on **"Settings"** in the main navigation menu
   - The Settings page will load with the URL indexing form

## Managing Web Pages

### Adding URLs for Indexing

1. **Enter Web Page URLs**
   - In the "Index Web Pages" section, you'll see 5 numbered input fields
   - Enter valid web page URLs (must start with `http://` or `https://`)
   - You can enter between 1 and 5 URLs
   - Empty fields will be ignored

2. **Example URLs**
   ```
   https://example.com/article
   https://docs.microsoft.com/azure
   https://github.com/project/readme
   ```

3. **Click "Index Web Pages"**
   - The system will validate your URLs
   - A spinner will appear showing indexing progress
   - Wait for the process to complete (typically 10-30 seconds per URL)

### Understanding Indexing Results

After indexing completes, you'll see a results table with:

- **URL**: The web page address that was processed
- **Title**: The extracted page title
- **Chunks**: Number of content pieces created for search
- **Status**: Success (green) or Failed (red)
- **Indexed At**: When the page was processed

#### Success Indicators
- ✅ **Green badge**: Page successfully indexed
- **Chunk count > 0**: Content was extracted and processed
- **Recent timestamp**: Processing completed

#### Error Indicators
- ❌ **Red badge**: Page failed to index
- **Chunk count = 0**: No content could be extracted
- **Error message**: Details about what went wrong

### Viewing Currently Indexed Pages

The right sidebar shows:
- **Page titles** and **source domains**
- **Number of chunks** per page
- **Indexing timestamps**
- Links to **visit original pages**

### Common Issues and Solutions

#### URL Validation Errors
- **Problem**: "Invalid URL format" error
- **Solution**: Ensure URLs start with `http://` or `https://`
- **Example**: Use `https://example.com` not `example.com`

#### Failed Crawling
- **Problem**: Page shows "Failed to crawl"
- **Possible causes**:
  - Website blocks automated crawlers
  - Page requires authentication
  - Network connectivity issues
  - Page doesn't exist (404 error)
- **Solution**: Try different URLs or contact the website owner

#### No Content Extracted
- **Problem**: Page indexed but 0 chunks created
- **Possible causes**:
  - Page is mostly images/videos
  - Content is behind JavaScript (dynamic loading)
  - Page structure prevents content extraction
- **Solution**: Try pages with more text content

## Using Enhanced Search

### Accessing Enhanced Search

1. **Navigate to Search Page**
   - Click **"Search"** in the main navigation
   - Enter your search query in the text field

2. **Enable Web Page Search**
   - Toggle **"Include Web Pages"** option
   - This will search both products and indexed web pages
   - Keep it off to search only products

### Understanding Search Results

When web page search is enabled, you'll see:

#### Product Results (Existing)
- Traditional product search results
- Product cards with images, names, descriptions, prices

#### Web Page Insights (New)
- **Green alert box** with "Web Page Insights" header
- AI-generated response based on web page content
- **Source URLs** mentioned in the response
- Relevant excerpts from indexed pages

### Search Best Practices

#### Effective Query Types
- **Factual questions**: "What is Azure Functions?"
- **How-to queries**: "How to deploy containers?"
- **Comparison requests**: "Difference between React and Vue?"
- **Feature questions**: "What features does X have?"

#### Query Tips
- Use **specific terms** rather than generic ones
- Ask **complete questions** for better AI responses
- Try **different phrasings** if results aren't relevant
- **Combine product and web searches** for comprehensive results

## Advanced Usage

### Content Types That Work Best
- **Documentation pages**: Technical guides, API docs
- **Blog articles**: How-to posts, tutorials
- **News articles**: Recent information and updates
- **Reference materials**: Specifications, standards
- **Educational content**: Learning materials, courses

### Content Types to Avoid
- **Dynamic content**: Pages that load content via JavaScript
- **Authentication-required**: Pages behind login walls
- **Multimedia-heavy**: Pages with mostly videos/images
- **Download pages**: File repositories without descriptions

### Managing Indexed Content

#### Refreshing Content
- **Re-index URLs**: Enter the same URL again to update content
- **Clear old content**: New indexing replaces previous content for the same URL
- **Update regularly**: Re-index pages that change frequently

#### Storage Considerations
- **In-memory storage**: Content persists while the service is running
- **Service restart**: All indexed content is lost on restart
- **Capacity limits**: System optimized for up to 5 URLs with moderate content

## Troubleshooting

### Common Error Messages

#### "Please enter at least one valid URL"
- **Cause**: All URL fields are empty or contain invalid URLs
- **Solution**: Enter at least one properly formatted URL

#### "Maximum 5 URLs allowed"
- **Cause**: System limitation to prevent overload
- **Solution**: Choose your 5 most important URLs

#### "Failed to index URLs. Please try again."
- **Cause**: Network or service error
- **Solution**: Wait a moment and retry, check URL validity

#### "No web pages have been indexed yet"
- **Cause**: Attempting search before indexing any URLs
- **Solution**: Add some URLs in Settings first

### Performance Tips

#### Optimal URL Selection
- Choose pages with **substantial text content**
- Prefer **well-structured HTML** over complex layouts
- Select **stable content** that doesn't change frequently
- Include **authoritative sources** for better quality

#### Search Optimization
- **Start broad**, then narrow your queries
- **Use keywords** from the content you indexed
- **Try different phrasings** of the same question
- **Combine searches** with both toggles for comprehensive results

## Best Practices

### Content Strategy
1. **Curate quality sources** - Choose authoritative, well-written content
2. **Diverse topics** - Index different types of content for variety
3. **Regular updates** - Re-index pages that change frequently
4. **Relevant content** - Choose pages related to your search needs

### Search Strategy
1. **Test different queries** - Experiment with various phrasings
2. **Use specific terms** - More specific queries yield better results
3. **Leverage both modes** - Compare results with/without web pages
4. **Read full responses** - AI responses often contain valuable context

### Maintenance Tips
1. **Monitor results quality** - Check if indexed content is helpful
2. **Update as needed** - Re-index when source content changes
3. **Remove irrelevant URLs** - Replace URLs that don't add value
4. **Document your URLs** - Keep track of what you've indexed and why

## Support and Feedback

### Getting Help
- Check this manual for common issues
- Review error messages for specific guidance
- Test with different URLs if crawling fails
- Verify URL accessibility in a web browser first

### Feature Limitations
- **Maximum 5 URLs** per configuration
- **In-memory storage** (content lost on restart)
- **HTTP/HTTPS only** (no other protocols)
- **Public pages only** (no authentication support)

Remember: The web page indexing feature is designed to enhance your search and chat experience by providing additional context from external sources. Choose your URLs thoughtfully to maximize the benefit!