using AutoMapper;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.Core.Services
{
    public class IdentityManagerRolesService<TUser, TRole, TRoleDto> where TUser : IdentityUser<Guid>
        where TRole : IdentityManagerRole
        where TRoleDto : class, IIdentityManagerRoleDto
    {
        private readonly RoleManager<TRole> roleManager;
        private readonly IMapper mapper;

        public IdentityManagerRolesService(RoleManager<TRole> roleManager, IMapper mapper)
        {
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        public async Task<IList<TRoleDto>> GetAllRoles()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return mapper.Map<IList<TRoleDto>>(roles);
        }

        private async Task<IdentityUtilsResult<TRole>> GetRoleById(Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return IdentityUtilsResult<TRole>.ErrorResult("Role with specified ID not found");

            return IdentityUtilsResult<TRole>.SuccessResult(role);
        }

        public async Task<IdentityUtilsResult<TRoleDto>> GetRole(Guid roleId)
        {
            var roleResult = await GetRoleById(roleId);
            return roleResult.ToTypedResult<TRoleDto>(mapper.Map<TRoleDto>(roleResult.Payload));
        }

        public async Task<IdentityUtilsResult<TRoleDto>> GetRole(string roleNameNormalized)
        {
            var role = await roleManager.FindByNameAsync(roleNameNormalized);
            if (role == null)
                return IdentityUtilsResult<TRoleDto>.ErrorResult("Role with specified name not found");

            return IdentityUtilsResult<TRoleDto>.SuccessResult(mapper.Map<TRoleDto>(role));
        }

        public async Task<IdentityUtilsResult<TRoleDto>> AddRole(TRoleDto roleDto)
        {
            roleDto.Name = roleDto.Name.Trim();
            var role = mapper.Map<TRole>(roleDto);

            var roleResult = await roleManager.CreateAsync(role);
            if (!roleResult.Succeeded)
                return roleResult.ToIdentityUtilsResult().ToTypedResult<TRoleDto>();

            mapper.Map(role, roleDto);
            return IdentityUtilsResult<TRoleDto>.SuccessResult(roleDto);
        }

        public async Task<IdentityUtilsResult> DeleteRole(Guid id)
        {
            var roleResult = await GetRoleById(id);
            if (!roleResult.Success)
                return roleResult;

            var deleteResult = await roleManager.DeleteAsync(roleResult.Payload);
            return deleteResult.ToIdentityUtilsResult();
        }
    }
}