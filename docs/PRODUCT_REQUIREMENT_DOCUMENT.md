# Product Requirement Document (PRD): Web Page Indexing with DataSources Service

## Executive Summary

The RealTimeStore application has been enhanced with a new **DataSources service** that enables users to index web pages for enhanced AI chat and search functionality. This feature allows users to define 1-5 URLs whose content will be crawled, indexed, and made available for semantic search alongside existing product data.

## Product Overview

### Background
The RealTimeStore currently provides:
- Real-time GPT chat functionality
- Product search with vector database capabilities
- Blazor-based frontend with interactive features

### Problem Statement
Users needed the ability to reference external web content in AI conversations and searches, but the system was limited to only product-related information.

### Solution
A new DataSources service that:
- Crawls user-defined web pages
- Extracts and chunks content intelligently
- Stores content in an in-memory vector database
- Integrates seamlessly with existing search and chat functionality

## Architecture

### System Components

#### 1. DataSources Service (Backend)
- **Technology**: .NET 9 Web API
- **Hosting**: Aspire orchestration with service discovery
- **Dependencies**: 
  - Microsoft.Extensions.AI for embeddings
  - HtmlAgilityPack for web crawling
  - Microsoft.SemanticKernel for vector operations
  - Azure OpenAI for embeddings and chat

#### 2. Enhanced Store Frontend
- **Technology**: Blazor Server Components (.NET 9)
- **New Features**:
  - Settings page for URL management
  - Enhanced search with web page results
  - Real-time indexing status

#### 3. Integration Points
- **Service Discovery**: Aspire-managed communication between services
- **Vector Storage**: In-memory vector store for performance
- **AI Integration**: Shared OpenAI embeddings and chat models

### Data Flow

1. **URL Submission**: User enters 1-5 URLs via Settings page
2. **Web Crawling**: DataSources service crawls pages using HtmlAgilityPack
3. **Content Processing**: Content is extracted, cleaned, and chunked
4. **Vector Embedding**: Text chunks are embedded using OpenAI embeddings
5. **Storage**: Embeddings stored in in-memory vector database
6. **Search Integration**: Search queries check both product and web page vectors
7. **AI Enhancement**: Chat responses include web page context

## Features

### Core Features

#### URL Management
- **Input Validation**: HTTP/HTTPS URL validation
- **Capacity Limits**: 1-5 URLs per configuration
- **Real-time Status**: Live indexing progress and results
- **Error Handling**: Clear error messages for failed crawls

#### Content Processing
- **Smart Extraction**: Main content detection, script/style removal
- **Intelligent Chunking**: Sentence-aware chunking with overlap
- **Metadata Preservation**: Source URL, title, timestamp tracking

#### Search Enhancement
- **Dual Search**: Combined product and web page results
- **Semantic Similarity**: Vector-based content matching
- **Contextual Responses**: AI responses enriched with web content
- **Source Attribution**: Clear source URL references

### Technical Features

#### Performance
- **In-Memory Storage**: Fast vector operations
- **Efficient Chunking**: Optimized for embedding models
- **Concurrent Processing**: Parallel URL crawling
- **Caching Strategy**: Eliminates redundant API calls

#### Reliability
- **Error Recovery**: Graceful handling of crawl failures
- **Validation Layer**: URL and content validation
- **Logging**: Comprehensive operation logging
- **Health Checks**: Service health monitoring

## User Experience

### Settings Page Flow
1. Navigate to Settings via main navigation
2. Enter up to 5 web page URLs
3. Click "Index Web Pages" button
4. View real-time indexing progress
5. See indexed results and error details
6. Monitor currently indexed pages in sidebar

### Enhanced Search Flow
1. Navigate to Search page
2. Enter search query
3. Optionally enable "Include Web Pages"
4. View combined results:
   - Product information (existing)
   - Web page insights (new)
5. Source URLs provided for web content

## Technical Specifications

### API Endpoints

#### DataSources Service
- `POST /api/datasources/index` - Index URLs
- `GET /api/datasources/search/{query}` - Search web content
- `GET /api/datasources/indexed` - Get indexed URLs
- `GET /api/datasources/health` - Health check

### Data Models

#### IndexUrlsRequest
```csharp
public class IndexUrlsRequest
{
    [Required]
    [MinLength(1), MaxLength(5)]
    public List<string> Urls { get; set; }
}
```

#### WebPageContent
```csharp
public class WebPageContent
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string ChunkContent { get; set; }
    public DateTime IndexedAt { get; set; }
    public int ChunkIndex { get; set; }
    public ReadOnlyMemory<float> Vector { get; set; }
}
```

### Configuration

#### Environment Variables
- `AI_ChatDeploymentName` - OpenAI chat model deployment
- `AI_embeddingsDeploymentName` - OpenAI embeddings model deployment
- OpenAI connection string for authentication

#### Aspire Configuration
- Service discovery registration
- Health check endpoints
- Telemetry and logging integration

## Security Considerations

### Input Validation
- URL format validation (HTTP/HTTPS only)
- Rate limiting on indexing requests
- Content size limits to prevent abuse

### Content Security
- No storage of sensitive information
- Content sanitization during extraction
- Source URL preservation for attribution

### Service Security
- Aspire service discovery for internal communication
- No direct external access to DataSources API
- Standard .NET security practices

## Success Metrics

### Functional Metrics
- Successful URL indexing rate (target: >90%)
- Search result relevance improvement
- Response time for combined searches
- User adoption of web page features

### Technical Metrics
- Service availability (target: 99.9%)
- Average crawling time per URL
- Vector search performance
- Error rate for failed crawls

## Future Enhancements

### Phase 2 Features
- Persistent storage option (database backend)
- Automatic re-crawling of indexed pages
- Content freshness monitoring
- Advanced filtering and search options

### Phase 3 Features
- Support for authenticated pages
- Document format support (PDF, Word)
- Collaborative URL management
- Analytics and usage reporting

## Risk Assessment

### Technical Risks
- **Web Crawling Reliability**: Some sites may block crawlers
  - *Mitigation*: User-agent configuration, retry logic
- **Content Quality**: Extracted content may be poor
  - *Mitigation*: Smart extraction algorithms, user feedback
- **Performance Impact**: Large content volumes may affect performance
  - *Mitigation*: In-memory storage, efficient chunking

### Business Risks
- **Content Ownership**: Legal implications of crawling content
  - *Mitigation*: User responsibility, terms of service
- **Scalability Limits**: In-memory storage has capacity constraints
  - *Mitigation*: Clear usage limits, future persistent storage

## Conclusion

The DataSources service successfully extends RealTimeStore's capabilities by enabling web page indexing and semantic search. The implementation follows .NET 9 and Aspire best practices while providing a seamless user experience. The in-memory approach ensures high performance while keeping the initial implementation simple and focused on core functionality.

The modular architecture allows for future enhancements while maintaining the reliability and performance of the existing system.