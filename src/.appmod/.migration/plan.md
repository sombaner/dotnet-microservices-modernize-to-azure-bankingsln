# .NET App Migration Prompt Template: Plaintext Credentials to Azure Key Vault

## Migration Request
Migrate this codebase from Plaintext Credentials to Azure Key Vault, focusing **exclusively** on code-level changes required for successful compilation.

## Tools use
You **MUST** use the tools from AzureKeyVaultKnowledgeBase for more detailed SDK specs and Azure Key Vault samples.
- Serve as a knowledge base for you when you make a plan.
- Serve as a knowledge base for you whenever you make code changes.
- Refer to the code samples for better code quality and keep the same code styles.

## Scope
* DO - Collect the framework used (.NET or .NET Framework) and keep the original project framework
* DO - Code modification to replace Plaintext Credentials dependencies with Azure Key Vault equivalents
* DO - Configuration file updates necessary for compilation
* DO - Dependency management changes
* DO - Update the function references to use the new generated functions.
* DO NOT - No infrastructure setup (assumed to be handled separately)
* DO NOT - No testing beyond compilation verification
* DO NOT - No deployment considerations

## Success Criteria
1. Codebase compiles successfully with Azure Key Vault
2. All Plaintext Credentials dependencies and imports are replaced
3. All old Plaintext Credentials code files and project configurations are cleaned from the solution
4. All migration tasks are tracked and completed

## Execution Process
1. Read .NET related knowledge base from AzureKeyVaultKnowledgeBase from tools provided for more Azure Key Vault details.
   - Save the dependency versions info to the migration plan
2. Analyze the codebase to identify all Plaintext Credentials usages as well as those places using the old Plaintext Credentials API.
   - Identify all files that need to be modified.
   - Identify all dependencies that need to be updated.
   - Identify all configuration files that need to be updated.
   - Identify all project files that need to be updated.
3. Create a 'progress.md' file to track changes with checkmarks
4. Execute migration tasks systematically:
   - Update dependency management files, put version information to Directory.Packages.props or package.config if it exists.
   - Update .csproj files to reference the dependency defined in above step. Follow the approach defined in section 'Important Notice' below.
   - Replace API calls and imports
   - Update configuration files
5. Mark tasks as completed: [] - [X]
6. Continue until all tasks are complete
7. Identify and clean up old Plaintext Credentials files that have been replaced by new Azure Key Vault equivalents.
8. Verify compilation for the entire project after all steps, refer to the AzureKeyVaultKnowledgeBase when you meet compile errors related to api and sdk, keep fixing until the project compiles successfully.
9. Stick to the plan and progress files, finish all the tasks and do not deviate from the plan unless absolutely necessary.

## Progress Tracking
Maintain a 'progress.md' file with:
* [ ] Task description (with files modified)
* [ ] Verification step
* [ ] Success criteria status
* [ ] Completion status

## Important Guideline (put this notice into the progress.md file)
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

## Build Verification
After all steps you are REQUIRED to:# Add newly created projects to the solution if applicable
# Make sure all the projects are reloaded before triggering the build process
# Run the appropriate build command for project type
# Report success/failure
# Fix any compilation errors and re-verify
## Migration Temporary Files

- Save the migration plan to file plan.md under folder .appmod/.migration.
- Save the progress tracking to file progress.md under folder .appmod/.migration.

If create files failed, you can try the following steps:

    1. Use command line to create the empty files plan.md and progress.md under .appmod/.migration folder.
    1. Use `edit_file` tool to edit the empty files plan.md and progress.md.

NOTE: **DO NOT** add files under .appmod/ to the .csproj project files, they are temporary files for migration purpose only.

## Technical Analysis and Migration Strategy

### Framework Analysis
- The project is using .NET 8.0 (all projects have TargetFramework=net8.0)
- The application has a microservice architecture with multiple services (Account, Customer, Transaction)
- All services use plaintext credentials in configuration files

### Identified Plaintext Credentials
1. **Database Connection Strings:**
   - PostgreSQL connection strings in Account.API and Customer.API/GRPC:
     - `Server=accountdb;Database=AccountDb;User Id=admin;Password=admin1234;`
     - `Server=customerdb;Database=CustomerDb;User Id=admin;Password=admin1234;`
   - MongoDB connection string in Transaction.API:
     - `mongodb://transactiondb:27017`

2. **Event Bus Credentials:**
   - RabbitMQ connection strings in Account.API and Transaction.API:
     - `amqp://guest:guest@localhost:5672`

### Required NuGet Packages
For .NET 8.0 applications:
- Azure.Extensions.AspNetCore.Configuration.Secrets (Version 1.4.0)
- Azure.Identity (Version 1.14.0)
- Azure.Security.KeyVault.Secrets (Version 4.5.0)

### Implementation Strategy
1. Update the existing Common.KeyVault project in the BuildingBlocks folder
   - Implement KeyVault configuration extension methods
   - Create a KeyVaultSecretProvider service

2. Modify configuration files:
   - Add KeyVault URL configuration
   - Keep plaintext credentials for development but implement a mechanism to use KeyVault in production

3. Update Program.cs files to:
   - Load KeyVault configuration
   - Use Managed Identity for authentication

4. Update Infrastructure services to retrieve connection strings from KeyVault

### Migration Steps
1. Update Common.KeyVault project and add necessary dependencies
2. Implement KeyVault configuration providers
3. Update all relevant project files with references to the updated Common.KeyVault project
4. Update configuration files
5. Modify Program.cs files to use KeyVault
6. Update infrastructure services to use KeyVault for retrieving connection strings
7. Verify the build works successfully

## Next

After creating the plan.md and progress.md, stop the session and wait for user's review and continue signal.