# Solution Architecture

This document provides a comprehensive overview of the eShopLite solution architecture, including system components, data flow, and service interactions.

## 🏗️ High-Level Architecture

```mermaid
graph TB
    subgraph "Client Layer"
        Browser[Web Browser]
        Mobile[Mobile Browser]
    end
    
    subgraph "Azure Load Balancer"
        ALB[Application Gateway / Load Balancer]
    end
    
    subgraph "eShopLite Platform"
        subgraph "Frontend Services"
            Store[Store Web App<br/>Blazor Server]
        end
        
        subgraph "Backend Services"
            Products[Products API<br/>ASP.NET Core]
            Search[Search API<br/>ASP.NET Core]
            Chat[Chat API<br/>ASP.NET Core + SignalR]
        end
        
        subgraph "External Services"
            NLWeb[NLWeb API<br/>Natural Language Processing]
        end
        
        subgraph "Data Layer"
            SqlDB[(SQL Server<br/>Product Catalog)]
            Cache[(Redis Cache<br/>Session & Performance)]
        end
        
        subgraph "Infrastructure"
            KeyVault[Azure Key Vault<br/>Secrets Management]
            AppInsights[Application Insights<br/>Monitoring & Telemetry]
            ContainerRegistry[Azure Container Registry<br/>Docker Images]
        end
    end
    
    subgraph "Orchestration"
        Aspire[.NET Aspire<br/>Service Discovery & Orchestration]
    end
    
    Browser --> ALB
    Mobile --> ALB
    ALB --> Store
    
    Store --> Products
    Store --> Search
    Store --> Chat
    
    Search --> NLWeb
    Chat --> NLWeb
    
    Products --> SqlDB
    Products --> Cache
    Search --> Cache
    Chat --> Cache
    
    Store --> KeyVault
    Products --> KeyVault
    Search --> KeyVault
    Chat --> KeyVault
    
    Store --> AppInsights
    Products --> AppInsights
    Search --> AppInsights
    Chat --> AppInsights
    
    Aspire -.-> Store
    Aspire -.-> Products
    Aspire -.-> Search
    Aspire -.-> Chat
    
    ContainerRegistry --> ALB
    
    classDef frontend fill:#e1f5fe,stroke:#0277bd,stroke-width:2px
    classDef backend fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px
    classDef data fill:#e8f5e8,stroke:#2e7d32,stroke-width:2px
    classDef external fill:#fff3e0,stroke:#f57c00,stroke-width:2px
    classDef infra fill:#fce4ec,stroke:#c2185b,stroke-width:2px
    
    class Store frontend
    class Products,Search,Chat backend
    class SqlDB,Cache data
    class NLWeb external
    class KeyVault,AppInsights,ContainerRegistry infra
```

## 🔄 Service Interaction Flow

### User Journey: Product Search with Chat Assistant

```mermaid
sequenceDiagram
    participant U as User
    participant S as Store Web App
    participant CH as Chat API
    participant SE as Search API
    participant NL as NLWeb API
    participant P as Products API
    participant DB as SQL Database
    
    Note over U,DB: User searches for products using chat
    
    U->>S: Opens store website
    S->>S: Loads chat widget
    
    U->>S: Types "running shoes under $100"
    S->>CH: POST /api/v1/chat/message
    
    CH->>NL: Process natural language query
    NL-->>CH: Structured query + intent
    
    CH->>SE: GET /api/v1/search (structured query)
    SE->>NL: Search website content
    NL-->>SE: Search results with scores
    
    SE->>P: GET /api/products (filtered)
    P->>DB: Query products
    DB-->>P: Product data
    P-->>SE: Product details
    
    SE-->>CH: Combined search results
    CH-->>S: Chat response with products
    S-->>U: Display chat response with product cards
    
    Note over U,DB: User can click on products or ask follow-up questions
```

## 🏢 Deployment Architecture

### Azure Container Apps (Recommended)

```mermaid
graph TB
    subgraph "Azure Subscription"
        subgraph "Resource Group: rg-eshoplite-{env}"
            subgraph "Container Apps Environment"
                direction TB
                StoreApp[Store Container App<br/>External Ingress]
                ProductsApp[Products Container App<br/>Internal Ingress]
                SearchApp[Search Container App<br/>Internal Ingress]
                ChatApp[Chat Container App<br/>Internal Ingress]
            end
            
            subgraph "Shared Services"
                ACR[Azure Container Registry<br/>Docker Images]
                KV[Azure Key Vault<br/>Secrets & Config]
                LAW[Log Analytics Workspace<br/>Centralized Logging]
                AI[Application Insights<br/>APM & Telemetry]
            end
            
            subgraph "Data Services"
                SQL[(Azure SQL Database<br/>Product Catalog)]
                Redis[(Azure Cache for Redis<br/>Session & Performance)]
            end
        end
    end
    
    subgraph "External Services"
        NLWebAPI[NLWeb API<br/>3rd Party Service]
        GitHub[GitHub Actions<br/>CI/CD Pipeline]
    end
    
    Internet --> StoreApp
    StoreApp --> ProductsApp
    StoreApp --> SearchApp
    StoreApp --> ChatApp
    
    SearchApp --> NLWebAPI
    ChatApp --> NLWebAPI
    
    ProductsApp --> SQL
    ProductsApp --> Redis
    SearchApp --> Redis
    ChatApp --> Redis
    
    StoreApp --> KV
    ProductsApp --> KV
    SearchApp --> KV
    ChatApp --> KV
    
    StoreApp --> AI
    ProductsApp --> AI
    SearchApp --> AI
    ChatApp --> AI
    
    GitHub --> ACR
    ACR --> StoreApp
    ACR --> ProductsApp
    ACR --> SearchApp
    ACR --> ChatApp
    
    LAW --> AI
    
    classDef containerapp fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    classDef shared fill:#f1f8e9,stroke:#388e3c,stroke-width:2px
    classDef data fill:#fff8e1,stroke:#f57c00,stroke-width:2px
    classDef external fill:#fce4ec,stroke:#e91e63,stroke-width:2px
    
    class StoreApp,ProductsApp,SearchApp,ChatApp containerapp
    class ACR,KV,LAW,AI shared
    class SQL,Redis data
    class NLWebAPI,GitHub external
```

## 🔌 Service Dependencies

### Service Dependency Matrix

| Service | Dependencies | Purpose |
|---------|--------------|---------|
| **Store** | Products API, Search API, Chat API | Main user interface |
| **Products** | SQL Database, Redis Cache | Product catalog management |
| **Search** | NLWeb API, Redis Cache | Natural language search |
| **Chat** | NLWeb API, Redis Cache, SignalR | Real-time chat assistance |

### Service Discovery with .NET Aspire

```mermaid
graph LR
    subgraph "Aspire Service Discovery"
        Registry[Service Registry<br/>Consul/Built-in]
    end
    
    subgraph "Services"
        Store[Store Web App]
        Products[Products API]
        Search[Search API]
        Chat[Chat API]
    end
    
    Store --> Registry
    Products --> Registry
    Search --> Registry
    Chat --> Registry
    
    Store -.->|discovers| Products
    Store -.->|discovers| Search
    Store -.->|discovers| Chat
    
    Registry -.->|health checks| Store
    Registry -.->|health checks| Products
    Registry -.->|health checks| Search
    Registry -.->|health checks| Chat
```

## 📊 Data Architecture

### Data Flow Patterns

```mermaid
graph TB
    subgraph "Data Sources"
        UserInput[User Input<br/>Search/Chat]
        ProductCatalog[Product Catalog<br/>SQL Database]
        WebContent[Website Content<br/>NLWeb Index]
    end
    
    subgraph "Processing Layer"
        NLP[Natural Language Processing<br/>NLWeb API]
        SearchEngine[Search Processing<br/>Search API]
        ChatEngine[Conversation Processing<br/>Chat API]
    end
    
    subgraph "Caching Layer"
        SearchCache[Search Results Cache<br/>Redis]
        SessionCache[Chat Session Cache<br/>Redis]
        ProductCache[Product Data Cache<br/>Redis]
    end
    
    subgraph "Presentation Layer"
        SearchResults[Search Results<br/>JSON Response]
        ChatResponse[Chat Response<br/>SignalR]
        ProductData[Product Information<br/>API Response]
    end
    
    UserInput --> NLP
    NLP --> SearchEngine
    NLP --> ChatEngine
    
    ProductCatalog --> ProductCache
    WebContent --> SearchCache
    
    SearchEngine --> SearchCache
    ChatEngine --> SessionCache
    
    SearchCache --> SearchResults
    SessionCache --> ChatResponse
    ProductCache --> ProductData
    
    SearchResults --> UserInterface[User Interface]
    ChatResponse --> UserInterface
    ProductData --> UserInterface
```

## 🔒 Security Architecture

### Security Layers

```mermaid
graph TB
    subgraph "Edge Security"
        WAF[Web Application Firewall<br/>Azure Front Door]
        DDoS[DDoS Protection<br/>Azure DDoS Standard]
    end
    
    subgraph "Application Security"
        AuthN[Authentication<br/>Azure AD B2C]
        AuthZ[Authorization<br/>Role-based Access]
        RateLimit[Rate Limiting<br/>API Management]
    end
    
    subgraph "Data Security"
        Encryption[Data Encryption<br/>TLS 1.3 + AES-256]
        KeyMgmt[Key Management<br/>Azure Key Vault]
        Secrets[Secret Management<br/>Managed Identity]
    end
    
    subgraph "Network Security"
        VNet[Virtual Network<br/>Private Networking]
        NSG[Network Security Groups<br/>Traffic Filtering]
        PrivateEndpoints[Private Endpoints<br/>Service Access]
    end
    
    subgraph "Monitoring Security"
        SIEM[Security Monitoring<br/>Azure Sentinel]
        Audit[Audit Logging<br/>Azure Monitor]
        Alerts[Security Alerts<br/>Azure Security Center]
    end
    
    Internet --> WAF
    WAF --> DDoS
    DDoS --> AuthN
    AuthN --> AuthZ
    AuthZ --> RateLimit
    
    RateLimit --> VNet
    VNet --> NSG
    NSG --> PrivateEndpoints
    
    Encryption --> KeyMgmt
    KeyMgmt --> Secrets
    
    SIEM --> Audit
    Audit --> Alerts
```

## 📈 Scalability Patterns

### Horizontal Scaling Strategy

```mermaid
graph TB
    subgraph "Auto-scaling Triggers"
        CPU[CPU Utilization > 70%]
        Memory[Memory Usage > 80%]
        RequestRate[Request Rate > 100/sec]
        QueueDepth[Message Queue Depth > 1000]
    end
    
    subgraph "Scaling Actions"
        ScaleOut[Scale Out Container Instances]
        ScaleUp[Scale Up Container Resources]
        LoadBalance[Distribute Load]
    end
    
    subgraph "Resource Pools"
        StorePool[Store App Pool<br/>1-10 instances]
        ProductsPool[Products API Pool<br/>1-5 instances]
        SearchPool[Search API Pool<br/>1-8 instances]
        ChatPool[Chat API Pool<br/>1-6 instances]
    end
    
    CPU --> ScaleOut
    Memory --> ScaleUp
    RequestRate --> LoadBalance
    QueueDepth --> ScaleOut
    
    ScaleOut --> StorePool
    ScaleOut --> ProductsPool
    ScaleOut --> SearchPool
    ScaleOut --> ChatPool
    
    ScaleUp --> StorePool
    LoadBalance --> StorePool
```

## 🔄 CI/CD Architecture

### Deployment Pipeline

```mermaid
graph LR
    subgraph "Source Control"
        GitHub[GitHub Repository<br/>Source Code]
    end
    
    subgraph "CI Pipeline"
        Build[Build & Test<br/>GitHub Actions]
        Package[Package Applications<br/>Docker Images]
        Scan[Security Scan<br/>Container Scanning]
    end
    
    subgraph "Artifact Storage"
        ACR[Azure Container Registry<br/>Docker Images]
        Artifacts[Build Artifacts<br/>GitHub Packages]
    end
    
    subgraph "CD Pipeline"
        Deploy[Deploy Infrastructure<br/>BICEP Templates]
        Update[Update Services<br/>Blue-Green Deployment]
        Test[Integration Tests<br/>Health Checks]
    end
    
    subgraph "Environments"
        Dev[Development<br/>Container Apps]
        Staging[Staging<br/>Container Apps]
        Prod[Production<br/>Container Apps]
    end
    
    GitHub --> Build
    Build --> Package
    Package --> Scan
    Scan --> ACR
    Scan --> Artifacts
    
    ACR --> Deploy
    Deploy --> Update
    Update --> Test
    
    Test --> Dev
    Test --> Staging
    Test --> Prod
```

## 📊 Monitoring & Observability

### Telemetry Architecture

```mermaid
graph TB
    subgraph "Application Telemetry"
        Metrics[Custom Metrics<br/>Performance KPIs]
        Traces[Distributed Tracing<br/>Request Flow]
        Logs[Structured Logging<br/>Application Events]
    end
    
    subgraph "Infrastructure Telemetry"
        ContainerMetrics[Container Metrics<br/>CPU, Memory, Network]
        PlatformLogs[Platform Logs<br/>Azure Resource Logs]
        HealthChecks[Health Checks<br/>Service Availability]
    end
    
    subgraph "Collection & Storage"
        OTEL[OpenTelemetry<br/>Telemetry Collection]
        AppInsights[Application Insights<br/>APM Platform]
        LogAnalytics[Log Analytics<br/>Log Aggregation]
    end
    
    subgraph "Analysis & Alerting"
        Dashboards[Azure Dashboards<br/>Real-time Monitoring]
        Alerts[Alert Rules<br/>Proactive Notifications]
        Workbooks[Azure Workbooks<br/>Custom Analysis]
    end
    
    Metrics --> OTEL
    Traces --> OTEL
    Logs --> OTEL
    
    ContainerMetrics --> AppInsights
    PlatformLogs --> LogAnalytics
    HealthChecks --> AppInsights
    
    OTEL --> AppInsights
    AppInsights --> LogAnalytics
    
    LogAnalytics --> Dashboards
    AppInsights --> Alerts
    LogAnalytics --> Workbooks
```

---

## 📋 Architecture Decision Records (ADRs)

### ADR-001: Microservices Architecture
- **Decision**: Implement microservices pattern with .NET Aspire orchestration
- **Rationale**: Improved scalability, technology diversity, team autonomy
- **Consequences**: Increased complexity, network latency, service coordination

### ADR-002: Azure Container Apps vs App Services
- **Decision**: Default to Container Apps with App Services as alternative
- **Rationale**: Better scalability, cost efficiency, modern deployment patterns
- **Consequences**: Learning curve, different monitoring approaches

### ADR-003: SignalR for Real-time Communication
- **Decision**: Use SignalR for chat functionality
- **Rationale**: Native .NET integration, WebSocket support, fallback protocols
- **Consequences**: Sticky sessions required, scaling considerations

### ADR-004: Redis for Caching
- **Decision**: Implement Redis for distributed caching and session storage
- **Rationale**: High performance, distributed support, rich data structures
- **Consequences**: Additional infrastructure, cache invalidation complexity

---

*This architecture document is living and will be updated as the system evolves.*