using AutoMapper;
using IdentityUtils.Commons.Validation;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityUtils.Core.Services
{
    public class IdentityManagerRolesService<TRole, TRoleDto> : IIdentityManagerRolesService<TRoleDto>
        where TRole : IdentityManagerRole
        where TRoleDto : class, IIdentityManagerRoleDto
    {
        private readonly IMapper mapper;
        private readonly RoleManager<TRole> roleManager;

        public IdentityManagerRolesService(RoleManager<TRole> roleManager, IMapper mapper)
        {
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        public async Task<IdentityUtilsResult<TRoleDto>> AddRole(TRoleDto roleDto)
        {
            roleDto.Name = roleDto.Name.Trim();
            var role = mapper.Map<TRole>(roleDto);

            var result = ModelValidator.ValidateDataAnnotations(role).ToIdentityUtilsResult();
            if (!result.Success)
                return result.ToTypedResult<TRoleDto>();

            var managerValidationResult = await new RoleValidator<TRole>().ValidateAsync(roleManager, role);
            if (!managerValidationResult.Succeeded)
                return managerValidationResult.ToIdentityUtilsResult().ToTypedResult<TRoleDto>();

            var roleResult = await roleManager.CreateAsync(role);
            if (!roleResult.Succeeded)
                return roleResult.ToIdentityUtilsResult().ToTypedResult<TRoleDto>();

            mapper.Map(role, roleDto);
            return IdentityUtilsResult<TRoleDto>.SuccessResult(roleDto);
        }

        public async Task<IdentityUtilsResult> DeleteRole(Guid id)
        {
            var (roleResult, role) = await GetRoleById(id).UnpackSingleOrDefault();
            if (!roleResult.Success)
                return roleResult;

            var deleteResult = await roleManager.DeleteAsync(role);
            return deleteResult.ToIdentityUtilsResult();
        }

        public async Task<IEnumerable<TRoleDto>> GetAllRoles()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return mapper.Map<IEnumerable<TRoleDto>>(roles);
        }

        public async Task<IdentityUtilsResult<TRoleDto>> GetRole(Guid roleId)
        {
            var roleResult = await GetRoleById(roleId);
            return roleResult.ToTypedResult<TRoleDto>(mapper.Map<TRoleDto>(roleResult.Data.Single()));
        }

        public async Task<IEnumerable<TRoleDto>> Search(RoleSearch searchModel)
        {
            var rolesQuery = roleManager.Roles;

            if (string.IsNullOrEmpty(searchModel.Name))
                rolesQuery = rolesQuery.Where(x => x.NormalizedName == searchModel.Name.ToUpperInvariant());

            var roles = await rolesQuery.ToListAsync();

            return mapper.Map<IEnumerable<TRoleDto>>(roles);
        }

        private async Task<IdentityUtilsResult<TRole>> GetRoleById(Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return IdentityUtilsResult<TRole>.ErrorResult("Role with specified ID not found");

            return IdentityUtilsResult<TRole>.SuccessResult(role);
        }
    }
}