# 🚀 Minimal API ASP.NET Project

A lightweight ASP.NET Core Minimal API for managing administrators and vehicles, featuring JWT authentication, MySQL database integration, and comprehensive unit testing.

## 🛠️ Technologies

- **Framework**: ASP.NET Core 9.0 (Preview)
- **Language**: C#
- **Database**: MySQL with Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **Testing**: MSTest with Moq for mocking
- **Serialization**: System.Text.Json

## 📁 Project Structure

```
minimal-api-asp-dotnet/
├── Api/                          # Main API application
│   ├── Program.cs               # Application entry point
│   ├── Startup.cs               # Services and middleware configuration
│   ├── Domain/                  # Business logic layer
│   │   ├── DTOs/                # Data Transfer Objects
│   │   ├── Entities/            # Database entities
│   │   ├── Enums/               # Enumerations (e.g., Profile)
│   │   ├── Interfaces/          # Service contracts
│   │   ├── Services/            # Business logic implementations
│   │   └── ModelViews/          # Response models
│   └── Infrastructure/
│       └── Db/                  # Database context and configurations
├── Migrations/                  # EF Core database migrations
└── Test/                        # Unit tests
    ├── Domain/                  # Domain layer tests
    ├── Requests/                # API endpoint integration tests
    ├── Mocks/                   # Mock implementations
    └── Helpers/                 # Test setup utilities
```

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK (Preview)
- MySQL Server
- Git

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/giovanesouza/minimal-api-asp-dotnet.git
   
   cd minimal-api-asp-dotnet
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Update database connection in `Api/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "MySql": "Server=localhost;Database=minimalapi;User=root;Password=yourpassword;"
     },
     "Jwt": {
       "Key": "your-jwt-secret-key"
     }
   }
   ```

4. Run migrations:
   ```bash
   dotnet ef database update --project Api
   ```

5. Run the application:
   ```bash
   dotnet run --project Api
   ```

The API will be available at `https://localhost:5066`. Swagger documentation is available at `https://localhost:5066/swagger`.

## 🔧 API Endpoints

### Authentication
- `POST /administrators/login` - Authenticate and get JWT token

### Administrators (Requires Admin role)
- `GET /administrators` - List all administrators
- `GET /administrators/{id}` - Get administrator by ID
- `POST /administrators` - Create new administrator

### Vehicles
- `GET /vehicles` - List vehicles (Admin only)
- `GET /vehicles/{id}` - Get vehicle by ID (Admin or Editor)
- `POST /vehicles` - Create vehicle (Admin or Editor)
- `PUT /vehicles/{id}` - Update vehicle (Admin only)
- `DELETE /vehicles/{id}` - Delete vehicle (Admin only)

All endpoints except login require JWT authentication in the Authorization header.

## 🧪 Testing

Run tests with:
```bash
dotnet test
```

Tests include:
- Unit tests for services and domain logic
- Integration tests for API endpoints
- Mocked dependencies for isolated testing
- Parallel execution support (with state isolation)

## 📝 Notes

- Uses BCrypt for password hashing
- JWT tokens expire in 2 hours
- Supports Admin and Editor roles
- Database migrations handle schema updates
- CI/CD pipeline configured in `.github/workflows/api-check.yml`

For more details, check the source code and individual test files.
