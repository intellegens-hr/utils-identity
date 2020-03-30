# Identity Server 4 Demo

This guide will show you how to build IdentityServer 4 app from scratch including:
- setting authorization
- building data models
- setting up management endpoints

## Basics
First thing to do is create new empty web project inside new or existing solution:
- (optionally) Create new solution
- Inside your solution, create new ASP.NET Core web application. Don't use any template - select empty template. Name this project `IdentityUtils.Demos.IdentityServer4`
- add following Nuget packages to new project
     - Microsoft.EntityFrameworkCore.Sqlite
     - Microsoft.EntityFrameworkCore.Design
- add reference to projects:
     - `IdentityUtils.Api.Controllers`
     - `IdentityUtils.IS4Extensions`

## Data model

### DbContext
Now it's time to create DbContext. `IdentityUtils` expect implementation of `IdentityUtils.Core.Contracts.Context.IdentityManagerDbContext`. Since this is demo, we'll use default Db models for users, tenants and roles provided in `IdentityUtils.Core.Contracts` project. This demo will use SQLite to store data.

Create new folder DbContext in your demo project. Inside, create new .cs file `Is4DemoDbContext.cs` with following content:
```csharp
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using Microsoft.EntityFrameworkCore;

namespace IdentityUtils.Demos.IdentityServer4.DbContext
{
    public class Is4DemoDbContext : IdentityManagerDbContext<IdentityManagerUser, IdentityManagerRole, IdentityManagerTenant>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(@"Data Source=Is4Demo.db;");
        }
    }
}
```

#### Migrations
We created new DbContext and specified types, now we need to generate migration scripts. Open your project in command line and run <br/>
`dotnet ef migrations add InitialCreate -o DbContext/Migrations`. This will generate migration scripts and place them in new folder called `Migrations` inside `DbContext` folder.

### DTO Models
After preparing DbContext, DTO models need to be prepared. Inside the project, create new folder `Models`. Inside, place three files listed below.

#### TenantDto.cs
```csharp
using IdentityUtils.Core.Contracts.Tenants;
using System;

namespace IdentityUtils.Demos.IdentityServer4.Models
{
    public class TenantDto : IIdentityManagerTenantDto
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public List<string> Hostnames { get; set; }
    }
}
```

#### RoleDto.cs
```csharp
using IdentityUtils.Core.Contracts.Roles;
using System;

namespace IdentityUtils.Demos.IdentityServer4.Models
{
    public class RoleDto : IIdentityManagerRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}
```

#### UserDto.cs
```csharp
using IdentityUtils.Core.Contracts.Users;
using System;

namespace IdentityUtils.Demos.IdentityServer4.Models
{
    public class UserDto : IIdentityManagerUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AdditionalDataJson { get; set; }
    }
}
```

### AutoMapper
`IdentityUtils` use AutoMapper to convert between Dto and database models. For that reason, we need to configure AutoMapper for types we specified.

Create new file inside `Models` folder called `_MapperConfig.cs` and add following content:
```csharp
using AutoMapper;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using System.Collections.Generic;

namespace IdentityUtils.Demos.IdentityServer4.Models
{
    public class Is4ModelsMapperProfile : Profile
    {
        public Is4ModelsMapperProfile()
        {
            CreateMap<IdentityManagerUser, UserDto>();
            CreateMap<UserDto, IdentityManagerUser>();
            CreateMap<List<IdentityManagerUser>, List<UserDto>>();

            CreateMap<IdentityManagerRole, RoleDto>();
            CreateMap<RoleDto, IdentityManagerRole>();
            CreateMap<List<IdentityManagerRole>, List<RoleDto>>();

            CreateMap<IdentityManagerTenant, TenantDto>();
            CreateMap<TenantDto, IdentityManagerTenant>();
            CreateMap<List<IdentityManagerTenant>, List<TenantDto>>();
        }
    }
}
```

## Management controllers
Next, we need to implement management controller which use database and dto models we want.

`IdentityUtils.Api.Controllers` project has abstract controller classes for managing roles, tenants and users. All methods are implemented, but this classes need to be inherited to specify which types will controller use as database and which types as DTO model. It's only simple matter of creating new controller classes, inheriting base controllers with defined types and you're good to go.

Create new folder called `ControllersApi` inside the project and add all files listed below to this folder.

### TenantManagementControllerApi.cs
```csharp
using IdentityUtils.Api.Controllers;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Services;
using IdentityUtils.Demos.IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUtils.Demos.IdentityServer4.ControllersApi
{
    [Authorize(Policy = "Is4ManagementApi")]
    [Route("/api/management/tenants")]
    public class TenantManagementControllerApi
        : TenantControllerApiAbstract<IdentityManagerTenant, TenantDto>
    {
        public TenantManagementControllerApi(IdentityManagerTenantService<IdentityManagerTenant, TenantDto> tenantService)
            : base(tenantService)
        {
        }
    }
}
```

### RoleManagementControllerApi.cs
```csharp
using IdentityUtils.Api.Controllers;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services;
using IdentityUtils.Demos.IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUtils.Demos.IdentityServer4.ControllersApi
{
    [Authorize(Policy = "Is4ManagementApi")]
    [Route("/api/management/roles")]
    public class RoleManagementControllerApi
        : RolesControllerApiAbstract<IdentityManagerUser, UserDto, IdentityManagerRole, RoleDto>
    {
        public RoleManagementControllerApi(IdentityManagerRolesService<IdentityManagerUser, IdentityManagerRole, RoleDto> rolesService)
            : base(rolesService)
        {
        }
    }
}
```

### UserManagementControllerApi.cs
```csharp
using AutoMapper;
using IdentityUtils.Api.Controllers;
using IdentityUtils.Commons.Mailing;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services;
using IdentityUtils.Demos.IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUtils.Demos.IdentityServer4.ControllersApi
{
    [Authorize(Policy = "Is4ManagementApi")]
    [Route("/api/management/users")]
    public class UserManagementControllerApi
        : UsersControllerApiAbstract<IdentityManagerUser, IdentityManagerRole, UserDto>
    {
        public UserManagementControllerApi(
            IdentityManagerUserService<IdentityManagerUser, UserDto, IdentityManagerRole> userManager,
            IMailingProvider mailingProvider,
            IMapper mapper)
        : base(userManager, mailingProvider, mapper)
        {
        }
    }
}
```

At this point, your project should be similar to:
![](docs/images/Checkpoint_001.png)

## Identity Server configuration

At this moment, everything we need to setup IdentityServer is ready. This is only matter of configuring app and services in `Startup.cs`

### Services configuration

To actually use Identity Server 4 with everything that we specified, we need to add following to `ConfigureServices` method in `Startup.cs`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...

    //Add context we specified early on and mappings for DTO and database models
    services.AddDbContext<Is4DemoDbContext>();
    services.AddAutoMapper(typeof(Is4ModelsMapperProfile));

    services
        .AddIdentityUtilsIs4Extensions((builder) =>
        {
            builder
                .AddIdentity<IdentityManagerUser, IdentityManagerRole, IdentityManagerTenant, Is4DemoDbContext>()
                //This will add authentication to all API calls, first argument is authority - in this case 
                //it's the URL of this instance. Second parameter is Audience for JWT bearer token
                //This is enough for testing purposes, for dev/prod this should go to appsettings.json
                .AddAuthentication("https://localhost:5000", "demo-is4-management-api")
                .AddTenantStore<IdentityManagerTenant, TenantDto>()
                .AddTenantUserStore<IdentityManagerUser, UserDto, IdentityManagerRole>()
                .AddRolesStore<IdentityManagerUser, IdentityManagerRole, RoleDto>()
                .AddIntellegensProfileClaimsService<IdentityManagerUser, UserDto, IdentityManagerRole>();
        });

    services.AddAuthorization(opt =>
    {
        //Management controllers have defined Authorize attribute
        //This is policy definition they use. Every client calling APIs, needs to have 
        //demo-is4-management-api resource as one of allowed scopes
        opt.AddPolicy("Is4ManagementApi", builder =>
        {
            builder.RequireScope("demo-is4-management-api");
        });
    });

    ...

    var builder = services.AddIdentityServer(options =>
    {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
    })
        .LoadIdentityUtilsIdentityServerSettings((builder) =>
        {
            builder
                //This will add default clients: is4management and jsapp. One will be used to authorize calls to IS4,
                //other one will be used to authorize client apps calls to API apps
                .AddDefaultClientConfiguration()
                //Profile service will properly load roles data per tenant to tokens provided by IS4
                .AddIdentityAndProfileService<IdentityManagerUser, UserDto, IdentityManagerRole>();
        });

    // not recommended for production - you need to store your key material somewhere secure
}
```

## User data



## Wrapping up

This should be enough to get you going with Identity Server 4 and IdentityUtils extensions. Demo project also includes initial data which can be seeded (by defult to SQLite).

Please, keep in mind following notes:
- Client and API definitions in this example are stored in memory. For production environments, this should be moved to permanent store.
- When adding new clients, don't forget to add their hostnames to specific tenant
- Use `IdentityUtils.Api.Extensions` if you need to access management APIs in other projects (APIs).