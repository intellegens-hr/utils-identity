﻿using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Controllers
{
    /// <summary>
    /// Basic controller which provides all needed API endpoints for role management.
    /// </summary>
    /// <typeparam name="TUser">User database model</typeparam>
    /// <typeparam name="TUserDto">User DTO model</typeparam>
    /// <typeparam name="TRole">Role database model</typeparam>
    /// <typeparam name="TRoleDto">Role DTO model</typeparam>
    [ApiController]
    public abstract class RolesControllerApiAbstract<TUser, TUserDto, TRole, TRoleDto> : ControllerBase
        where TUser : IdentityManagerUser
        where TUserDto : class, IIdentityManagerUserDto
        where TRole : IdentityManagerRole
        where TRoleDto : class, IIdentityManagerRoleDto
    {
        private readonly IdentityManagerRolesService<TUser, TRole, TRoleDto> rolesService;

        protected RolesControllerApiAbstract(IdentityManagerRolesService<TUser, TRole, TRoleDto> rolesService)
        {
            this.rolesService = rolesService;
        }

        [HttpGet]
        public virtual async Task<IList<TRoleDto>> GetAllRoles()
            => await rolesService.GetAllRoles();

        [HttpPost]
        public virtual async Task<IdentityUtilsResult<TRoleDto>> AddRole(TRoleDto roleDto)
        {
            return await rolesService.AddRole(roleDto);
        }

        [HttpGet("{roleId}")]
        public virtual async Task<IdentityUtilsResult<TRoleDto>> GetRoleById([FromRoute]Guid roleId)
            => await rolesService.GetRole(roleId);

        [HttpDelete("{roleId}")]
        public virtual async Task<IdentityUtilsResult> DeleteRole([FromRoute]Guid roleId)
            => await rolesService.DeleteRole(roleId);

        [HttpGet("rolename/{roleName}")]
        public virtual async Task<IdentityUtilsResult<TRoleDto>> GetRoleByNormalizedName([FromRoute]string roleName)
        {
            roleName = WebUtility.UrlDecode(roleName);
            return await rolesService.GetRole(roleName);
        }
    }
}