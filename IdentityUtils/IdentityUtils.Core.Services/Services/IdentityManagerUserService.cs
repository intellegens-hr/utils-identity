using AutoMapper;
using AutoMapper.QueryableExtensions;
using Commons.Validation;
using IdentityManagement.Services;
using IdentityUtils.Core.Contracts;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services.Extensions;
using IdentityUtils.Core.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityUtils.Core.Services
{
    public class IdentityManagerUserService<TUser, TUserDto, TRole>
        where TUser : IdentityManagerUser
        where TRole : IdentityManagerRole
        where TUserDto : class, IIdentityManagerUserDto
    {
        private readonly UserManager<TUser> userManager;
        private readonly RoleManager<TRole> roleManager;
        private readonly IIdentityManagerUserContext<TUser> dbContext;
        private readonly IMapper mapper;

        public IdentityManagerUserService(
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
            IIdentityManagerUserContext<TUser> dbContext,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.roleManager = roleManager;
        }

        public async Task<List<TUser>> GetAllUsers()
        {
            return await userManager.Users.ToListAsync();
        }

        /// <summary>
        /// Retrieves user by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IdentityManagementResult<TUser>> FindByIdAsync(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return IdentityManagementResult<TUser>.ErrorResult("User with specified ID not found");
            else
                return IdentityManagementResult<TUser>.SuccessResult(user);
        }

        /// <summary>
        /// Uses AutoMapper to map retrieved user to speciefied type
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IdentityManagementResult<TDto>> FindByIdAsync<TDto>(Guid id) where TDto : class
        {
            var user = await FindByIdAsync(id);

            return user.Success
                ? IdentityManagementResult<TDto>.SuccessResult(mapper.Map<TDto>(user.Payload))
                : IdentityManagementResult<TDto>.ErrorResult(user.ErrorMessages);
        }

        public async Task<IdentityManagementResult<TUser>> FindByNameAsync(string name)
        {
            var user = await userManager.FindByNameAsync(name);
            if (user == null)
                return IdentityManagementResult<TUser>.ErrorResult("User with specified name not found");

            return IdentityManagementResult<TUser>.SuccessResult(user);
        }

        public async Task<IdentityManagementResult<TUserDto>> FindByNameAsync<TUserDto>(string name) where TUserDto : class
        {
            var userResult = await FindByNameAsync(name);

            if (!userResult.Success)
                return IdentityManagementResult<TUserDto>.ErrorResult(userResult.ErrorMessages);

            return IdentityManagementResult<TUserDto>.SuccessResult(mapper.Map<TUserDto>(userResult.Payload));
        }

        public async Task<IdentityManagementResult> CreateUser(TUserDto user)
        {
            var userDb = mapper.Map<TUser>(user);

            var result = ModelValidator.ValidateDataAnnotations(userDb).ToIdentityManagementResult();
            if (!result.Success)
                return result;

            result = (await userManager.CreateAsync(userDb, user.Password)).ToIdentityManagementResult();

            if (result.Success)
                mapper.Map(userDb, user);

            return result;
        }

        public async Task<IdentityManagementResult> UpdateUser(TUserDto user)
        {
            var userDbResult = await FindByIdAsync(user.Id);

            if (!userDbResult.Success)
                return IdentityManagementResult.ErrorResult(userDbResult.ErrorMessages);

            mapper.Map(user, userDbResult.Payload);
            var result = await userManager.UpdateAsync(userDbResult.Payload);

            if (!result.Succeeded)
                return result.ToIdentityManagementResult();

            mapper.Map(userDbResult.Payload, user);
            return IdentityManagementResult.SuccessResult;
        }

        public async Task<IdentityManagementResult> DeleteUser(Guid userId)
        {
            var userResult = await FindByIdAsync(userId);
            if (!userResult.Success)
                return userResult;

            var identityResult = await userManager.DeleteAsync(userResult.Payload);
            return identityResult.ToIdentityManagementResult();
        }

        public Task<string> GeneratePasswordResetTokenAsync(TUser user)
            => userManager.GeneratePasswordResetTokenAsync(user);

        public async Task<IdentityManagementResult> ResetPasswordAsync(TUser user, string token, string newPassword)
        {
            var result = await userManager.ResetPasswordAsync(user, token, newPassword);
            return result.ToIdentityManagementResult();
        }

        public async Task<IEnumerable<Claim>> GetUserTenantRolesClaims(Guid userId)
        {
            var tenantClaims = await dbContext
                .UserClaims
                .Where(x => x.UserId == userId)
                .Where(x => x.ClaimType == TenantClaimsSchema.TenantRolesData)
                .ToListAsync();

            return tenantClaims.Select(x => x.ToClaim());
        }

        public async Task<IdentityManagementResult> AddClaimAsync(Guid userId, Claim claim)
            => await AddClaimsAsync(userId, new List<Claim> { claim });

        public async Task<IdentityManagementResult> AddClaimsAsync(Guid userId, IEnumerable<Claim> claims)
        {
            var userResult = await FindByIdAsync(userId);
            if (!userResult.Success)
                return userResult;

            var result = await userManager.AddClaimsAsync(userResult.Payload, claims);
            return result.ToIdentityManagementResult();
        }

        public async Task<Claim> GetUserTenantRolesClaims(Guid userId, Guid tenantId)
        {
            var claims = await GetUserTenantRolesClaims(userId);
            return claims
                .Where(x => x.Value.DeserializeToTenantRolesClaimData().TenantId == tenantId)
                .FirstOrDefault();
        }

        private async Task<IdentityManagementResult> AddOrUpdateTenantRolesClaim(Guid userId, Guid tenantId, TenantRolesClaimData tenantClaimData)
        {
            var result = IdentityResult.Success;
            var newTenantRolesClaim = new Claim(TenantClaimsSchema.TenantRolesData, tenantClaimData.Serialize());
            var oldTenantRolesClaim = await GetUserTenantRolesClaims(userId, tenantId);

            var userResult = await FindByIdAsync(userId);
            if (!userResult.Success)
                return userResult;

            if (oldTenantRolesClaim != null)
                result = await userManager.RemoveClaimAsync(userResult.Payload, oldTenantRolesClaim);

            if (result.Succeeded)
                result = await userManager.AddClaimAsync(userResult.Payload, newTenantRolesClaim);

            return result.ToIdentityManagementResult();
        }

        public async Task<IEnumerable<TenantRolesClaimData>> GetTenantRolesListAsync(Guid userId)
        {
            var claims = await GetUserTenantRolesClaims(userId);
            return claims.Select(x => x.Value.DeserializeToTenantRolesClaimData());
        }

        public async Task<IdentityManagementResult> AddToRoleAsync(Guid userId, Guid tenantId, Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            TenantRolesClaimData tenantClaimData = new TenantRolesClaimData(tenantId, role.NormalizedName);

            var tenantRolesClaim = (await GetUserTenantRolesClaims(userId, tenantId));
            if (tenantRolesClaim != null)
            {
                tenantClaimData = tenantRolesClaim.Value.DeserializeToTenantRolesClaimData();
                if (!tenantClaimData.Roles.Contains(role.NormalizedName))
                    tenantClaimData.Roles.Add(role.NormalizedName);
            }

            var result = await AddOrUpdateTenantRolesClaim(userId, tenantId, tenantClaimData);
            return result;
        }

        public async Task<IdentityManagementResult> AddToRolesAsync(Guid userId, Guid tenantId, IEnumerable<Guid> roles)
        {
            var result = IdentityManagementResult.SuccessResult;
            foreach (var role in roles)
            {
                if (result.Success)
                    result = await AddToRoleAsync(userId, tenantId, role);
                else
                    break;
            };

            return result;
        }

        public async Task<IList<string>> GetRolesAsync(Guid userId, Guid tenantId)
        {
            var data = await GetUserTenantRolesClaims(userId, tenantId);
            return data.Value.DeserializeToTenantRolesClaimData().Roles;
        }

        public async Task<bool> IsInRoleAsync(Guid userId, Guid tenantId, string role)
        {
            var roles = await GetRolesAsync(userId, tenantId);
            return roles.Contains(role);
        }

        public async Task<IdentityManagementResult> RemoveFromRoleAsync(Guid userId, Guid tenantId, Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            TenantRolesClaimData tenantClaimData = new TenantRolesClaimData(tenantId, role.NormalizedName);

            var tenantRolesClaim = (await GetUserTenantRolesClaims(userId, tenantId));
            if (tenantRolesClaim != null)
            {
                tenantClaimData = tenantRolesClaim.Value.DeserializeToTenantRolesClaimData();
                if (tenantClaimData.Roles.Contains(role.NormalizedName))
                    tenantClaimData.Roles.Remove(role.NormalizedName);
            }

            var result = await AddOrUpdateTenantRolesClaim(userId, tenantId, tenantClaimData);
            return result;
        }

        public async Task<IdentityManagementResult<List<TUserDto>>> RoleUsersPerTenant(Guid roleId, Guid tenantId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            string roleNormalizedName = role.NormalizedName;

            var tenantClaims = await dbContext
                .UserClaims
                .Where(x => x.ClaimType == TenantClaimsSchema.TenantRolesData)
                .ToListAsync();

            var userIds = tenantClaims
                .Select(x => new
                {
                    userClaim = x,
                    tenantRoleClaim = x.ToClaim().Value.DeserializeToTenantRolesClaimData()
                })
                .Where(x => x.tenantRoleClaim.TenantId == tenantId && x.tenantRoleClaim.Roles.Contains(roleNormalizedName))
                .Select(x => x.userClaim.UserId);

            var usersDto = await dbContext
                .Users
                .Where(x => userIds.Contains(x.Id))
                .ProjectTo<TUserDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            return IdentityManagementResult<List<TUserDto>>.SuccessResult(usersDto);
        }
    }
}