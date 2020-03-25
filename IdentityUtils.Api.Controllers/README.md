# IdentityUtils.Api.Controllers

This project provides base classes for API controllers for entire identity management (CRUD operations, etc.). It depends on 
- `IdentityUtils.Core.Services` to have access to management services and models
- `IdentityUtils.Api.Models` where all helper models can be found (eg. models expected in request body). Since this project relies on entire .Net Core MVC, models are in separate project to avoid unnecessary dependencies for libraries which use these models. 

## Management Api Controller

Three separate API controllers can be found, each covering CRUD operations for one model:
- `RolesControllerApiAbstract`
- `TenantControllerApiAbstract`
- `UsersControllerApiAbstract`

Typical API controller has following declaration
```csharp 
public abstract class UsersControllerApiAbstract<TUser, TRole, TUserDto> : ControllerBase
    where TUser : IdentityManagerUser
    where TRole : IdentityManagerRole
    where TUserDto : class, IIdentityManagerUserDto
```

Controller which inherits management API controller, needs to specify concrete types in order to work. Also, authorization and route should be specified:

```csharp
[ApiController]
[Route("/api/management/users")]
[Authorize(Policy = "Is4ManagementApi")]
public class UsersControllerApi : UsersControllerApiAbstract<UserImplementation, RoleImplementation, UserDto>
    
```