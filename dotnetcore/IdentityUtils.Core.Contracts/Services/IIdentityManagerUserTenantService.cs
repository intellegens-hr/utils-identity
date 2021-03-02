using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Contracts.Users;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityUtils.Core.Contracts.Services
{
    public interface IIdentityManagerUserTenantService<TUser, TUserDto> : IIdentityManagerUserServiceCommon<TUser, TUserDto>
        where TUser : IdentityManagerUser
        where TUserDto : class, IIdentityManagerUserDto
    {
        Task<IdentityUtilsResult> AddToRoleAsync(Guid userId, Guid tenantId, Guid roleId);

        Task<IdentityUtilsResult> AddToRolesAsync(Guid userId, Guid tenantId, IEnumerable<Guid> roles);

        Task<IEnumerable<RoleBasicData>> GetRolesAsync(Guid userId, Guid tenantId);

        Task<IEnumerable<Claim>> GetUserTenantRolesClaims(Guid userId);

        Task<bool> IsInRoleAsync(Guid userId, Guid tenantId, Guid role);

        Task<IdentityUtilsResult> RemoveFromRoleAsync(Guid userId, Guid tenantId, Guid roleId);

        Task<IEnumerable<TUserDto>> Search(UsersTenantSearch searchModel);
    }
}