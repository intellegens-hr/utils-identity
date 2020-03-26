# IdentityUtils.Api.Extensions.Cli

This CLI tools is used to administer users, roles and tenants on IdentityServer which implements all controllers specified in [IdentityUtils.Api.Controllers](./IdentityUtils.Api.Controllers/README.md).

CLI tool uses [API extension utils](./IdentityUtils.Api.Extensions/README.md) to communicate with IdentityServer via REST API.

1. [ Authentication ](#auth)
2. [ Commands ](#commands)
    1. [ Tenants ](#tenants)
    2. [ Roles ](#roles)
    3. [ Users ](#users)

<a name="auth"></a>
## Authentication

IdentityServer API is protected and requires authentication. Authentication parameters are:
- Hostname - base url for IdentityServer instance (eg. https://127.0.0.1:5022)
- Client ID
- Client secret
- Client scope

This parameters can be specified as one of CLI arguments or in configuration file located in executable directory or execution directory.

Authentication parameters are searched with following priority:
- arguments
- execution directory
- executable directory

### Via arguments

To specify authentication parameters with arguments, all parameters must be set, otherwise CLI will look for alternative authentication parameters (in config files).

Authentication parameters can be specified in following way:
```IdentityUtils.Api.Extensions.Cli.exe --Auth.Hostname <hostname> --Auth.ClientId <client-id> --Auth.ClientSecret <client-secret> --Auth.Scope <scope> ...rest of commands```

### Via configuration file

If authentication arguments are not set (or are partially set), CLI tool will look for configuration file named `identityutils-cli.config` in following directories:
- executing directory - directory from which the exe is called
- executable directory - directory where CLI .exe file is located

Configuration file must have following content:
```
HOSTNAME: <hostname>
CLIENT ID: <client-id>
CLIENT SECRET: <client-secret>
SCOPE: <scope>
```

<a name="commands"></a>
## Commands

Similar to authentication parameters, all commands have optional endpoint switch `-r` or `--api-base-route` which specifies base route for management API (eg. `/api/management/users`).

These arguments are optional and default to:
- Tenants - `/api/management/tenants`
- Roles - `/api/management/roles`
- Users - `/api/management/users`

<a name="tenants"></a>
### Tenants
Available commands:
- `list` - List all tenants
- `add` - Add tenant
- `update` - Update tenant
- `delete` - Delete tenant

#### Examples
- List tenants: <br/>
`Cli.exe tenants list`<br/>
Optionally, `--id` switch can be used to list only tenant with specified ID

- Add tenant: <br/>
`Cli.exe tenants add --name "New tenant" --hostname https://localhost:5002`

- Update tenant<br/>
`Cli.exe tenants update --name "New tenant" --hostname https://localhost:5002 --id 8c46fde9-d305-425f-8fee-8360b09de2cf`

- Delete tenant: <br/>
`Cli.exe tenants delete --id 8c46fde9-d305-425f-8fee-8360b09de2cf`

<a name="roles"></a>
### Roles
Available commands:
- `list` - List all roles
- `add` - Add role
- `delete` - Delete role

#### Examples
- List roles: <br/>
`Cli.exe roles list`<br/>
Optionally, `--id` switch can be used to list only role with specified ID, or `--name` switch to list only role with specified normalized name

- Add role: <br/>
`Cli.exe roles add --name "New role"`

- Delete role: <br/>
`Cli.exe roles delete --id f613404e-f23d-48e2-ae4b-a63bab3d4abe`

<a name="users"></a>
### Users
Available commands:
- `list` - List all users
- `list-per-role` - List all users per role
- `add` - Add user
- `update` - Update user
- `delete` - Delete user
- `add-to-role` - Add user to role
- `remove-from-role` - Remove user from role

#### Examples
- List users: <br/>
`Cli.exe Cli.exe users list`<br/>
Optionally, `--id` switch can be used to list only users with specified ID

- List users per role: <br/>
`Cli.exe Cli.exe users list-per-role`<br/>
List all tenants and roles, for each tenant-role pair lists all users which have that role in tenant. Optionally, `--tenant-id` and `--role-id` switch can be used to filter wanted tenant and/or role

- Add user: <br/>
`Cli.exe Cli.exe users add --username testuser001 --email test@email.com --password notsostrongpassword`<br/>

- Update user: <br/>
`Cli.exe users update --email test2@email.com --id bdac63ea-d84f-432f-bc81-dabc88bb0c6d`

- Delete user: <br/>
`Cli.exe users delete --id bdac63ea-d84f-432f-bc81-dabc88bb0c6d`

- Add user to role: <br/>
`Cli.exe users add-to-role --role-id bdac63ea-d84f-432f-bc81-dabc88bb0c6d --user-id 718c39b2-d7de-49e2-9268-e22245b364e6 --tenant-id 174f052f-7489-4793-a652-5e384d27b8ad` <br />

- Remove user from role: <br/>
`Cli.exe users remove-from-role --role-id bdac63ea-d84f-432f-bc81-dabc88bb0c6d --user-id 718c39b2-d7de-49e2-9268-e22245b364e6 --tenant-id 174f052f-7489-4793-a652-5e384d27b8ad` <br />

