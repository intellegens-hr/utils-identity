# API using Identity Server 4 Demo

This guide presumes you already have Identity Server 4 instance up & running after following [Identity Server 4 Demo](../IdentityUtils.Demos.IdentityServer4.MultiTenant/README.md) guide. This guide will assume IS4 instance is running on `https://localhost:5010`.

Once again, create empty web project and name it `IdentityUtils.Demos.Api`. In project settings, set this web app to run on `https://localhost:5015`.

## Add host to tenant

Before we do anything we must make sure that the new API host is added to one of tenants defined on IS4 side.

First, let's make sure this host doesn't already exist. Navigate to CLI folder and make sure you've setup config file correctly:
```
HOSTNAME: https://localhost:5010
CLIENT ID: is4management
CLIENT SECRET: 511536EF-F270-4058-80CA-1C89C192F69A
SCOPE: demo-is4-management-api
```

List all tenants: <br/>
```cli.exe tenants list```

Output should be similar to:
```
Info: Authorization configuration not set in arguments
Info: Using configuration file 'C:\sources\intellegens-hr\dotnetcore-identity-utils\IdentityUtils.Api.Extensions.Cli\bin\Debug\netcoreapp3.1\identityutils-cli.config'
        ID                      NAME            HOSTNAME
--------------------------------------------
2a0e2d0a-feae-40cd-236e-08d7d225dbda            Intellegens Exams               https://localhost:5015;https://localhost:5010;https://localhost:5020
66d2a733-bd69-47b3-236f-08d7d225dbda            Intellegens Administration              https://localhost:5011
e505c636-63bf-46c2-2370-08d7d225dbda            Cmars Exams             https://localhost:5012
```

As you may notice, this API address is already assigned to tenant "Intellegens Exams". In case it wasn't listed here, we would simply add it to one of tenants. Please reference [CLI docs](../IdentityUtils.Api.Extensions.Cli/README.md) if needed.

## Configuration
Next (large) step is to define needed configuration, inlcuding:
- IApiExtensionsConfig
- IApiExtensionsIs4Config

Your appsettings should contain following settings (key and sample value):
- `"Is4Host": "https://localhost:5010"` - Identity Server app url
- `"Is4JsClientId": "jsapp"` - Client ID which Javascript apps will use
- `"ApiAuthenticationAudience": "demo-core-api"` - Which target audience to use when authenticating calls between Javascript apps and this API
- `"Is4ManagementApiClientId": "is4management"` - which client id to use when authenticating requests between API and IS4 management API
- `"Is4ManagementApiClientSecret": "511536EF-F270-4058-80CA-1C89C192F69A"` - Secret for `Is4ManagementApiClientId` client 
- `"Is4ManagementApiClientScope": "demo-is4-management-api"` - scope to use when authenticating calls with `Is4ManagementApiClientId` client
- `"UserManagementBaseRoute": "/api/management/users"` - route for IS4 user management API
- `"RoleManagementBaseRoute": "/api/management/roles"` - route for IS4 roles management API
- `"TenantManagementBaseRoute": "/api/management/tenants"` - route for IS4 tenant management API

## Authentication controller
Authentication controller should at least contain one method for user authentication. This method will call IS4 endpoint for token generation and retun it's response data.

This method must be public (hence, AllowAnonymous attribute) and receive model with username and password.

```csharp
[HttpPost("token/id")]
[AllowAnonymous]
public async Task<JsonResult> GetIdToken([FromBody]LoginModel loginModel)
{
    using var client = new HttpClient();
    var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
    {
        Address = $"{appSettings.Is4Host}/connect/token",
        ClientId = appSettings.Is4JsClientId,
        UserName = loginModel.Username,
        Password = loginModel.Password
    });

    return new JsonResult(new { tokenData = tokenResponse.Raw });
}
```

JavaScript apps or other client apps will use this token to authorize further calls to this API. Token refreshing is handled between client app and IS4 directly.

## Service config
In order for authentication to work, authentication must be specified in Services configuration method by using settings we defined earlier:
```csharp

...

services.AddMvcCore()
    .AddAuthorization();

---

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = AppSettings.Is4Host;
        options.Audience = AppSettings.ApiAuthenticationAudience;
    });

...
```

## Api user

Once client app calls API with provided token, Identity should be automatically loaded with claims:
```json
[
   {
      "Type":"address",
      "Value":"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }"
   },
   {
      "Type":"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
      "Value":"AliceSmith@email.com"
   },
   {
      "Type":"/intellegens/tenant/roles",
      "Value":"{\"TenantId\":\"2a0e2d0a-feae-40cd-236e-08d7d225dbda\",\"Roles\":[\"ADMIN\",\"READER\",\"GODMODE\"]}"
   },
   {
      "Type":"/intellegens/tenant/roles",
      "Value":"{\"TenantId\":\"66d2a733-bd69-47b3-236f-08d7d225dbda\",\"Roles\":[\"ADMIN\",\"READER\",\"GODMODE\"]}"
   },
   {
      "Type":"/intellegens/tenant/roles",
      "Value":"{\"TenantId\":\"e505c636-63bf-46c2-2370-08d7d225dbda\",\"Roles\":[\"ADMIN\",\"READER\",\"GODMODE\"]}"
   },
   {
      "Type":"userId",
      "Value":"3cd9ec06-8fa5-4cd9-2374-08d7d225dbda"
   }
]
```

To avoid parsing these claims in each call, helper class called `ApiUser` is created and added to services as scoped. This class will contain parsed claim data so it's more user friendly.

```csharp
public class ApiUser
{
    private readonly HttpContext httpContext;
    private readonly TenantManagementApi<TenantDto> tenantManagementApi;
    private readonly IMemoryCache memoryCache;

    /// <summary>
    /// All calls to this API are cross-domain and should contain Origin header
    /// Tenant ID is found by using client hostname
    /// </summary>
    /// <returns></returns>
    private Guid GetTenantIdByHostname()
    {
        var originHost = httpContext.Request.Headers.First(x => x.Key == "Origin").Value;

        return memoryCache.GetOrCreate(originHost, (entry) =>
        {
            entry.SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddMinutes(5));

            var tenant = tenantManagementApi.GetTenantByHostname(originHost).Result;
            return tenant.TenantId;
        });
    }

    public ApiUser(
        IHttpContextAccessor httpContextAccessor,
        TenantManagementApi<TenantDto> tenantManagementApi,
        IMemoryCache memoryCache
        )
    {
        this.httpContext = httpContextAccessor.HttpContext;
        this.tenantManagementApi = tenantManagementApi;
        this.memoryCache = memoryCache;

        IsAuthenticated = httpContext.User.Identity.IsAuthenticated;

        if (IsAuthenticated)
        {
            var tenantId = GetTenantIdByHostname();

            //parse claims list
            var claims = httpContext
                .User
                .Claims
                .Select(x => new { x.Type, x.Value })
                .ToList();

            UserId = Guid.Parse(claims.First(x => x.Type == "userId").Value);
            TenantId = tenantId;
            TenantRoles = claims
                .Where(x => x.Type == TenantClaimsSchema.TenantRolesData)
                .Select(x => x.Value.DeserializeToTenantRolesClaimData())
                .ToList();

            //Extract roles for current tenant
            Roles = TenantRoles.First(x => x.TenantId == tenantId).Roles;
        };
        
    }

    public bool IsAuthenticated { get; set; }
    public Guid TenantId { get; }
    public Guid UserId { get; }
    public List<string> Roles { get; set; }
    public List<TenantRolesClaimData> TenantRoles { get; set; }
}
```

As you may notice, Identity server returns roles per tenant. For that reason, this helper class also extracts roles for current tenant only. Since user claims don't contain roles, it's impossible to user role based authentication out of the box.

For that reason, we need to add custom middleware which will use `ApiUser` class to check if user is authenticated and fetch roles for current tenant:
```csharp
    app.UseAuthentication();

    app.Use(async (context, next) =>
    {
        //get API User instance which contains all parsed claimdata
        var user = context.RequestServices.GetRequiredService<ApiUser>();

        if (user.IsAuthenticated)
        {
            var identity = context.User.Identity as ClaimsIdentity;
            var currentTenantRoles = user.Roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            identity.AddClaims(currentTenantRoles);
        }

        await next();
    });

    app.UseAuthorization();
```

## Conclusion
This setup should be enough to authenticate and authorize users. For any details, please look at the project source code.