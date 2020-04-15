namespace IdentityUtils.Api.Extensions
{
    public interface IApiExtensionsConfig
    {
        /// <summary>
        /// API address, eg. https://127.0.0.1:5003
        /// </summary>
        string Hostname { get; }

        /// <summary>
        /// Base route for user management, eg. "/api/management/users"
        /// </summary>
        string UserManagementBaseRoute { get; }

        /// <summary>
        /// Base route for role management, eg. "/api/management/roles"
        /// </summary>
        string RoleManagementBaseRoute { get; }

        /// <summary>
        /// Base route for tenant management, eg. "/api/management/tenants"
        /// </summary>
        string TenantManagementBaseRoute { get; }
    }
}