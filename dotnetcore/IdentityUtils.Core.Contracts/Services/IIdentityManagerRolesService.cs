using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.Core.Contracts.Services
{
    public interface IIdentityManagerRolesService<TRoleDto>
        where TRoleDto : class, IIdentityManagerRoleDto
    {
        Task<IdentityUtilsResult<TRoleDto>> AddRole(TRoleDto roleDto);

        Task<IdentityUtilsResult> DeleteRole(Guid id);

        Task<IEnumerable<TRoleDto>> GetAllRoles();

        Task<IdentityUtilsResult<TRoleDto>> GetRole(Guid roleId);

        Task<IEnumerable<TRoleDto>> Search(RoleSearch searchModel);
    }
}