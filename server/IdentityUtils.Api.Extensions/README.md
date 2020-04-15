# IdentityUtils.Api.Extensions

`IdentityUtils.Api.Extensions` serve as Http wrapper around projects using `IdentityUtils.Api.Controllers`. This helps to avoid:
- using and configuring HttpClient calls
- storing management API routes as magic strings
- building Poco objects to send/receive data from API
- handling common API errors

Dependencies for this project are:
- `IndentityUtils.Api.Models` to use required Poco objects to send/receive data from API
- `IdentityUtils.Core.Contracts` since all classes need to specify desired DTO objects with constraints specified in `Contracts` project
- `IdentityUtils.Common` since it relies on `RestClient`

Currently supported management APIs:
- Tenant management
- Role management
- User management

## Configuration


## Example
Eg. to create new user, following steps are required:
- create (or inject) instance of UserManagementApi with desired DTO type. DTO type should be the same as the one specified when configuring `IdentityUtils.Core.Services`
```csharp
UserManagementApi<UserDto> userManagement = ...
```
- build your DTO object
```csharp
var user = new UserDto{
    Email = "ex@ample.com"
    Username = "testuser"
}
```
- call method for user creation
```csharp
var result = userManagement.CreateUser(user);
```

Variable `result` will contain status, error messages (if any) and new DTO object for created user.