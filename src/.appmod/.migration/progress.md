# Migration Progress: Plaintext Credentials to Azure Key Vault

## Important Guideline
1. When you use terminal command tool, never input a long command with multiple lines, always use a single line command. (This is a bug in VS Copilot)
1. Never create a new project in the solution, always use the existing project to add new files or update the existing files.
1. Minimize code changes:
    - Update only what's necessary for the migration.
    - Avoid unrelated code enhancement.
1. When dealing with .csproj files:
   - For .NET SDK style .csproj files, you can directly modify the content of the .csproj file.
   - For .NET Framework style .csproj files, you can use the following approach to update the project file:
     - For non-sdk legacy style .csproj files, please try to deal with it in the following way:
     If you want to read or modify the .csproj file, you can follow these steps:
       1. Unload the project via tool `unload_project`.
       2. Then, the .csproj file can be edited via tool 'edit_file' or read via tool 'get_file'. Do the modification as needed.
       3. After the modification, reload the project via tool `reload_project`. Make sure you always reload the project after unload a project, never leave unloaded projects in the solution.
       4. Follow the same way when you need to further update the dependency or file reference.

## Identified Plaintext Credentials

### Database Connection Strings
* [X] Account.API - PostgreSQL Connection String in appsettings.json
  - `Server=accountdb;Database=AccountDb;User Id=admin;Password=admin1234;`
* [X] Customer.API - PostgreSQL Connection String in appsettings.json
  - `Server=customerdb;Database=CustomerDb;User Id=admin;Password=admin1234;`
* [X] Customer.GRPC - PostgreSQL Connection String in appsettings.json
* [X] Transaction.API - MongoDB Connection String in appsettings.json
  - `mongodb://transactiondb:27017`

### Event Bus Credentials
* [X] Account.API - RabbitMQ Connection String in appsettings.json
  - `amqp://guest:guest@localhost:5672`
* [X] Transaction.API - RabbitMQ Connection String in appsettings.json
  - `amqp://guest:guest@localhost:5672`

## Migration Tasks

### 1. Update Common KeyVault Library
* [X] Update Common.KeyVault project in BuildingBlocks folder
* [X] Add Azure.Extensions.AspNetCore.Configuration.Secrets (Version 1.4.0) package
* [X] Add Azure.Identity (Version 1.14.0) package
* [X] Add Azure.Security.KeyVault.Secrets (Version 4.6.0) package
* [X] Implement KeyVaultExtensions for configuration
* [X] Create KeyVaultSecretProvider service
* [X] Create ConnectionStringManager service

### 2. Add Project References and Package Dependencies
* [X] Add Common.KeyVault reference to Account.API
* [X] Add Common.KeyVault reference to Customer.API
* [X] Add Common.KeyVault reference to Customer.GRPC
* [X] Add Common.KeyVault reference to Transaction.API
* [X] Add Common.KeyVault reference to Account.Infrastructure
* [X] Add Common.KeyVault reference to Customer.Infrastructure

### 3. Update Configuration Files
* [X] Modify Account.API appsettings.json to add KeyVault settings
* [X] Modify Customer.API appsettings.json to add KeyVault settings
* [X] Modify Customer.GRPC appsettings.json to add KeyVault settings
* [X] Modify Transaction.API appsettings.json to add KeyVault settings

### 4. Update Program.cs Files
* [X] Update Account.API Program.cs to use KeyVault configuration
* [X] Update Customer.API Program.cs to use KeyVault configuration
* [X] Update Customer.GRPC Program.cs to use KeyVault configuration
* [X] Update Transaction.API Program.cs to use KeyVault configuration

### 5. Update Infrastructure Services
* [X] Modify Account.Infrastructure to retrieve connection strings from KeyVault
* [X] Modify Customer.Infrastructure to retrieve connection strings from KeyVault
* [X] Modify Transaction.API Data classes to retrieve connection strings from KeyVault
* [X] Add ConnectionStringManager registration to all services

### 6. Verification
* [X] Verify all plaintext credentials are removed/replaced in configurations
* [X] Build the solution and fix any compilation errors
* [X] Verify all secrets are accessed through KeyVault
* [X] Document the changes and update the completion status

## Completion Status
* [X] All identified plaintext credentials replaced with Azure Key Vault
* [X] All services updated to use Azure Key Vault for secret retrieval
* [X] Build verified and successful
* [X] Documentation updated

## Summary of Changes

1. **Common.KeyVault Project Updates:**
   - Added necessary NuGet packages for Azure Key Vault integration
   - Implemented `KeyVaultExtensions` for configuration extension methods
   - Created `KeyVaultSecretProvider` service for secret retrieval
   - Implemented `ConnectionStringManager` for handling connection strings

2. **Project References:**
   - Added Common.KeyVault project references to all required API projects
   - Added Common.KeyVault project references to Infrastructure projects

3. **Configuration Updates:**
   - Added KeyVault URI configuration to all appsettings.json files
   - Added a flag to conditionally use KeyVault in different environments

4. **Program.cs Updates:**
   - Added KeyVault configuration registration
   - Added KeyVaultSecretProvider service registration
   - Added ConnectionStringManager registration

5. **Infrastructure Services Updates:**
   - Updated DB contexts to use ConnectionStringManager for retrieving connection strings
   - Modified MongoDB connection code to use KeyVault

6. **Build Verification:**
   - Fixed package version conflicts
   - Added missing project references
   - Fixed namespace issues
   - Confirmed successful build

The migration allows the application to use Azure Key Vault for securely accessing credentials, while still providing a fallback to appsettings for development scenarios. This is controlled by the `UseKeyVault` flag in the configuration, making it easy to switch between environments.