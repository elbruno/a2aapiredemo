# PaymentsService Implementation

This document describes the implementation of the PaymentsService according to the PRD specifications in `docs/04-PRD_Add_Payment_Mock_Server.md`.

## Overview

The PaymentsService is a Blazor Server application that provides mock payment processing capabilities for the Zava store. It includes:

- RESTful API endpoints for payment processing
- Web UI for viewing payment records
- Integration with .NET Aspire for service discovery and observability
- SQLite database for local development with EF Core

## Architecture

### Components

1. **PaymentsService** - Blazor Server app with API controllers
2. **PaymentsService.Tests** - Unit and integration tests
3. **Store Integration** - PaymentsClient for service communication
4. **Aspire Integration** - Service registration and database provisioning

### Project Structure

```
src/PaymentsService/
├── Controllers/PaymentsController.cs     # API endpoints
├── Data/PaymentsDbContext.cs             # EF Core context
├── DTOs/                                 # Request/response models
├── Models/PaymentRecord.cs               # Database entity
├── Pages/                                # Blazor pages
├── Services/PaymentRepository.cs         # Data access layer
├── Shared/                               # Blazor layout components
└── Program.cs                            # App configuration
```

## API Endpoints

### POST /api/payments
Creates a new payment record.

**Request:**
```json
{
  "userId": "user-123",
  "currency": "USD", 
  "amount": 99.99,
  "items": [
    {
      "productId": "prod-001",
      "quantity": 2,
      "unitPrice": 49.99
    }
  ],
  "paymentMethod": "Visa ****1234"
}
```

**Response (201 Created):**
```json
{
  "paymentId": "7e9b8f9a-...",
  "status": "Success",
  "processedAt": "2024-01-15T12:34:56Z"
}
```

### GET /api/payments
Retrieves paginated payment records.

**Query Parameters:**
- `page` (default: 1)
- `pageSize` (default: 20, max: 100)  
- `status` (optional filter)

**Response:**
```json
{
  "items": [...],
  "totalCount": 150
}
```

### GET /api/payments/{id}
Retrieves a specific payment by ID.

## Database Schema

**Payments Table:**
- PaymentId (GUID, PK)
- UserId (string, required)
- StoreId (string, optional)
- CartId (string, optional)
- Currency (string, required)
- Amount (decimal, required)
- Status (string, required)
- PaymentMethod (string, required)
- ItemsJson (text, serialized items)
- ProductEnrichmentJson (text, optional)
- CreatedAt (datetime)
- ProcessedAt (datetime)

## Aspire Integration

### Service Registration
The PaymentsService is registered in `ZavaAppHost/Program.cs`:

```csharp
// Add PaymentsDb connection
var paymentsDb = builder.AddConnectionString("PaymentsDb", "Data Source=Data/payments.db");

// Register PaymentsService for service discovery
var paymentsService = builder.AddProject<Projects.PaymentsService>("payments-service")
    .WithReference(paymentsDb);

// Store can discover PaymentsService via Aspire
var store = builder.AddProject<Projects.Store>("store")
    .WithReference(paymentsService);
```

### Features Enabled
- Service discovery between Store and PaymentsService
- Health checks and monitoring
- Centralized logging and telemetry
- Configuration management

## Store Integration

### PaymentsClient
The Store uses a typed HttpClient to communicate with PaymentsService:

```csharp
builder.Services.AddHttpClient<IPaymentsClient, PaymentsClient>(
    client => client.BaseAddress = new("https+http://payments-service"));
```

### Checkout Flow
1. User fills out customer information
2. Mock payment dialog presents payment method options
3. Payment request sent to PaymentsService API
4. Order processed and cart cleared on success
5. User redirected to confirmation page

## Running Locally

### Prerequisites
- .NET 9.0 SDK
- SQLite (for database)

### Steps
1. Start the Aspire AppHost:
   ```bash
   cd src/ZavaAppHost
   dotnet run
   ```

2. The following services will be available:
   - **Store**: Main shopping interface
   - **PaymentsService**: Payment processing and viewing (port 5004)
   - **Products**: Product catalog service
   - **Aspire Dashboard**: Monitoring and logs

3. Test the payment flow:
   - Add items to cart in Store
   - Proceed to checkout
   - Fill customer information
   - Select mock payment method
   - Verify payment appears in PaymentsService UI

## Testing

### Unit Tests
Run PaymentsService tests:
```bash
cd src/PaymentsService.Tests
dotnet test
```

### Integration Tests
Run Store PaymentsClient tests:
```bash  
cd src/Store.Tests
dotnet test
```

### Manual Testing
1. Navigate to Store checkout page
2. Complete payment flow
3. Check PaymentsService `/payments` page for records
4. Verify API endpoints via Swagger at `/swagger`

## Configuration

### Local Development
- **PaymentsService Port**: 5004
- **Database**: SQLite (`Data/payments.db`)
- **Mock Mode**: Always enabled (payments always succeed)

### Environment Variables
- `ConnectionStrings:PaymentsDb` - Database connection string
- `Services:PaymentsService` - Service base URL for Store
- `Payments:MockMode` - Enable/disable mock behavior

## Security Notes

- No real payment processing occurs
- Payment methods are masked (e.g., "Visa ****1234")
- No sensitive card data is logged or stored
- Service-to-service authentication can be enabled for production

## Future Enhancements

1. **Product Enrichment**: Call Products API to display product names
2. **Real Payment Gateways**: Integrate with Stripe, PayPal, etc.
3. **Advanced Filtering**: Search by date range, amount, user
4. **Notifications**: Email/SMS payment confirmations
5. **Reporting**: Payment analytics and dashboards

## Troubleshooting

### Common Issues

1. **Database not created**: Check PaymentsService logs for EF Core errors
2. **Service discovery fails**: Verify Aspire host registration
3. **Payment dialog not showing**: Check browser console for JavaScript errors
4. **API calls failing**: Verify PaymentsService is running and healthy

### Logs
Check Aspire Dashboard for centralized logging from all services.