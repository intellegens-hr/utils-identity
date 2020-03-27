# IdentityUtils

IdentityUtils are set of libraries intended to solve problem with `ASP.NET Identity` management in multitenant environments. It extends `ASP.NET Identity` with new models and features.

## Project structure

Project has 5 logic parts:
- IdentityUtils Identity management:
    - IdentityUtils.Core.Contracts
    - IdentityUtils.Core.Services
- IdentityUtils Identity management API
    - IdentityUtils.Api.Models
    - IdentityUtils.Api.Controllers
- IdentityUtils API Extension library
- IdentityUtils API Extension CLI
- IdentityUtils IS4 Extensions

![Dependency graph](./Docs/Images/DependencyGraph.PNG)

### IdentityUtils Identity management
Idea behind IdentityUtils services is to have separate database and DTO (domain-transfer) objects. `Contracts` project defines entities which can be overriden if needed, required interfaces for DTO objects and DbContext.

`Services` project uses `ASP.NET Identity` stores and DbContext. Every service must return `IdentityUtilsResult` or `IdentityUtilsResult<T>` if DTO object (or list of objects) need to be returned.

These services can be used separately in any project which needs to extend `ASP.NET Identity` with multitenancy feature. 

### IdentityUtils Identity management API
Management API is set of abstract API controllers which call services inside `IdentityUtils.Core.Services`:
- `TenantControllerApiAbstract` for tenant
- `RolesControllerApiAbstract` for role management
- `UsersControllerApiAbstract` for user management

If any project needs to implement management API, it can simply create new controllers which inherit these abstract controllers.

## Per project guides
[IdentityUtils.Commons](./IdentityUtils.Commons/README.md)

[IdentityUtils.Core.Contracts](./IdentityUtils.Core.Contracts/README.md)

[IdentityUtils.Core.Services](./IdentityUtils.Core.Services/README.md)

[IdentityUtils.Api.Controllers](./IdentityUtils.Api.Controllers/README.md)

[IdentityUtils.Api.Models](./IdentityUtils.Api.Models/README.md)

[IdentityUtils.Api.Extensions](./IdentityUtils.Api.Extensions/README.md)

[IdentityUtils.Api.Extensions.Cli](./IdentityUtils.Api.Extensions.Cli/README.md)

[IdentityUtils.Demos.IdentityServer4](./IdentityUtils.Demos.IdentityServer4/README.md)

[IdentityUtils.Demos.Api](./IdentityUtils.Demos.Api/README.md)

[IdentityUtils.Demos.Client](./IdentityUtils.Demos.Client/README.md)