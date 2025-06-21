# ExternalUserService

## Build Instructions

1. **Prerequisites:**
   - [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) or later installed.
   - Git installed (for cloning and version control).

2. **Clone the repository:**
   ```sh
   git clone git@github-personal:vaishnaviithamraju/external-user-service.git
   cd ExternalUserService
   ```

3. **Restore dependencies:**
   ```sh
   dotnet restore
   ```

4. **Build the solution:**
   ```sh
   dotnet build
   ```

## Test Instructions

1. **Navigate to the test project directory:**
   ```sh
   cd UserApiIntegration.Tests
   ```

2. **Run the tests:**
   ```sh
   dotnet test
   ```

## Design Decisions

- **Project Structure:**
  - `UserApiIntegration.Console/`: Console application for interacting with the external user API.
  - `UserApiIntegration.Core/`: Core library containing business logic, DTOs, and API client abstractions.
  - `UserApiIntegration.Tests/`: Unit tests for the core library and integration points.
- **Separation of Concerns:**
  - Core logic and DTOs are separated from the console app for better testability and reusability.
- **.NET 9.0:**
  - Chosen for latest language features and performance improvements.
- **Testing:**
  - Unit tests are provided for core logic to ensure reliability and facilitate future changes.