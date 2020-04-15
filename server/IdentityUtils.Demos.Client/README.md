# Javascript client using API and Identity Server 4 Demo

This guide presumes you already have API instance and Identity Server 4 instance up & running after following 
- [Identity Server 4 Demo](../IdentityUtils.Demos.IdentityServer4/README.md)
- [API Demo](../IdentityUtils.Demos.Api/README.md)

This guide will assume IS4 instance is running on `https://localhost:5010` and API on `https://localhost:5015`

Once again, create empty web project and name it `IdentityUtils.Demos.Client`. In project settings, set this web app to run on `https://localhost:5020`.

## Configuration - server side
Content server in this app will be static, so it's enough to add following to `Startup.cs`
```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}
```

## Configuration - Client side
First, prepare js configuration file which will contain ():
```javascript
//Client configuration
const ConfigAuthorizationAuthority = "https://localhost:5010";
const ConfigAuthorizationClientId = "jsapp";
const ConfigAuthorizationClientScope = "openid demo-core-api";
const ConfigIs4AuthorizationEndpoint = "https://localhost:5010";

//IS4 endpoint used to programmatically request tokens
const ConfigIs4TokenEndpoint = "https://localhost:5010/connect/token";

//IS4 endpoint used to programmatically revoke tokens
const ConfigIs4EndSessionEndpoint = "https://localhost:5010/connect/endsession";

//IS4 openid-configuration endpoint
const ConfigIs4JwksEndpoint = "https://localhost:5010/.well-known/openid-configuration/jwks";

//API login endpoint - return access tokens
const ConfigApiLoginEndpoint = "https://localhost:5015/api/authentication/token/id";

//API logout endpoint - revokes access tokens
const ConfigApiLogoutEndpoint = "https://localhost:5015/api/authentication/token/revoke";
```

## Ajax helper method
This function is used to wrap XHR requests and make them easier to use:
```typescript
function AjaxCall<T>(url, method: "GET" | "POST", payload: object, authToken?) {
    return new Promise<T>(function (resolve, reject) {
        const xhr = new XMLHttpRequest();
        xhr.onload = () => {
            const result = xhr.response;
            resolve(JSON.parse(result));
        };
        xhr.open(method, url);

        xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8");

        if (authToken)
            xhr.setRequestHeader("Authorization", "Bearer " + authToken);

        if (payload)
            xhr.send(JSON.stringify(payload));
        else
            xhr.send();
    })
}
```

## Login
To login user it's enough to submit login data (username and password) to API login endpoint:
```typescript
const loginData = {
    "username": "alice"
    "password": "Pass123$"
}

const tokenResponse = AjaxCall(ConfigApiLoginEndpoint, "POST", loginData); 
```

If authorization was successful, response data will contain user token. 

Next we need to setup OIDC client. This is library to provide OpenID Connect (OIDC) and OAuth2 protocol support for client-side, browser-based JavaScript client applications. Also included is support for user session and access token management. More info on their [GitHub page](https://github.com/IdentityModel/oidc-client-js).

### Oidc wrapper
To make user management easier, wrapper class for OIDC is used. Most of the parameters defined in js configuration file from start of this guide is used here. Basicly, we need to tell the client everything related to authentication process:
- Identity server host
- Client to use (id, scope, ...)
- Authentication endpoints
- ...

```typescript
class OidcWrapper {
    private usermanagerConfig = {
        authority: ConfigAuthorizationAuthority,
        client_id: ConfigAuthorizationClientId,
        response_type: 'code',
        scope: ConfigAuthorizationClientScope,
        automaticSilentRenew: true,
        loadUserInfo: false,
        metadata: {
            issuer: ConfigAuthorizationAuthority,
            authorization_endpoint: ConfigIs4AuthorizationEndpoint,
            token_endpoint: ConfigIs4TokenEndpoint,
            end_session_endpoint: ConfigIs4EndSessionEndpoint,
            jwks_uri: ConfigIs4JwksEndpoint,
        }
    };

    ...

    public UserManager;

    ...
}
```

### Add user to OIDC client
To set user data to OIDC client, we need to add new user with token response:

```typescript
const token = JSON.parse(tokenResponse);
const user = new Oidc.User(token);
user.expires_in = token.expires_in;

const oidcWrapper = new OidcWrapper();
oidcWrapper.UserManager.storeUser(user);
```

Now, regardless if user reloads window or closes and reopens browser, his info will be stored in session storage. In token response, two tokens were actually received:
- authentication token
- refresh token

Refresh token has longer lifespan, and when authentication token expires, OIDC client will automatically request new one.

## Ajax calls which require authorization
`AjaxCall` function accepts authentication token as argument. To call API which requires authorization, all we need to do is:
```typescript
const user = await oidcWrapper.UserManager.getUser();
const accessToken = user.access_token;
const data = await AjaxCall<any>("/api/authenticatedendpoint", "GET", null, accessToken);
```