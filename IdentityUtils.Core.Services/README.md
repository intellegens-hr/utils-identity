# IdentityUtils.Core.Services

To avoid adding `TenantId` member to all identity models (roles, users), services contained in this project wrap around .Net Core Identity `UserManager`/`RoleManager` and add this feature without altering base identity database models.

Following issues need to be solved with these services:
- roles are common to all tenants. When new role is added it can be used by all tenants
- roles are assigned to a user along with tenant for which it's assigned
- user roles can't be stored in `UserRoles` table since it's missing `TenantId` field

To solve stated issues, user roles are stored as user claims with following structure for each tenant: 
- Claim type: `/intellegens/tenant/roles` 
- Claim value:  `{"tenantId": <tenant-id>, "roles": [<role-1> ... <role-n>]}`

Class used to serialize/deserialize claim value is `TenantRolesClaimData`.