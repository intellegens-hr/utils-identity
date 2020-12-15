using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Contracts.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.Core.Contracts.Services
{
    public interface IIdentityManagerUserService<TUser, TUserDto> : IIdentityManagerUserServiceCommon<TUser, TUserDto>
        where TUser : IdentityManagerUser
        where TUserDto : class, IIdentityManagerUserDto
    {
        Task<IdentityUtilsResult> AddToRoleAsync(Guid userId, Guid roleId);

        Task<IdentityUtilsResult> AddToRolesAsync(Guid userId, IEnumerable<Guid> roles);

        Task<IEnumerable<RoleBasicData>> GetRolesAsync(Guid userId);

        Task<bool> IsInRoleAsync(Guid userId, Guid role);

        Task<IdentityUtilsResult> RemoveFromRoleAsync(Guid userId, Guid roleId);

        Task<IEnumerable<TUserDto>> Search(UsersSearch searchModel);
    }
}