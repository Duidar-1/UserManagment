# UserManagement API
Description
A lightweight ASP.NET Core Web API for user authentication, role, and user management. Features JWT-based security, role-based authorization, and localization for English (en-US) and Hindi (hi-IN). Includes 25 unit tests. Runs on port 3000 for development and 3001 for testing.

#Technologies and Libraries

.NET 8: Core framework for building the Web API.
ASP.NET Core 8.0: Web API development and middleware.
Entity Framework Core 8.0: ORM for database operations with SQL Server.
Microsoft.AspNetCore.Authentication.JwtBearer 8.0: JWT-based authentication.
System.IdentityModel.Tokens.Jwt 7.0.3: JWT token generation and validation.
BCrypt.Net-Next 4.0.3: Password hashing.
Microsoft.AspNetCore.Mvc.NewtonsoftJson 8.0: JSON serialization.
Swashbuckle.AspNetCore 6.5.0: Swagger for API documentation.
xUnit 2.5.3: Unit testing framework.
Moq 4.18.4: Mocking library for tests.
Microsoft.EntityFrameworkCore.InMemory 8.0: In-memory database for testing.

#Setup

Clone the Repository:
git clone https://github.com/your-username/UserManagement.git
cd UserManagement/UserManagement


Install Dependencies:
dotnet restore


Install EF Core CLI:
dotnet tool install --global dotnet-ef --version 8.0.0


Configure Database:

Edit appsettings.json with your SQL Server connection string:"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=UserManagementDb;Trusted_Connection=True;TrustServerCertificate=True"
}


Apply migrations:dotnet ef migrations add InitialCreate
dotnet ef database update





Running Instructions

Run the API:

Set port 3000 in Properties/launchSettings.json:"applicationUrl": "https://localhost:3000"


Start the API:dotnet run


Access at https://localhost:3000/swagger.


Run Tests:

Tests use port 3001 (in-memory).
Execute:dotnet test





