using AutoMapper;
using AutoMapper.QueryableExtensions;
using IdentityUtils.Core.Contracts.Claims;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Contracts.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityUtils.Core.Services
{
    public class IdentityManagerUserTenantService<TUser, TUserDto, TRole> :
        IdentityManagerUserServiceBase<TUser, TUserDto>,
        IIdentityManagerUserTenantService<TUser, TUserDto>
        where TUser : IdentityManagerUser
        where TRole : IdentityManagerRole
        where TUserDto : class, IIdentityManagerUserDto
    {
        private readonly IIdentityManagerUserContext<TUser> dbContext;
        private readonly IMapper mapper;
        private readonly RoleManager<TRole> roleManager;
        private readonly UserManager<TUser> userManager;

        public IdentityManagerUserTenantService(
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
            IIdentityManagerUserContext<TUser> dbContext,
            IMapper mapper) : base(userManager, mapper)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.roleManager = roleManager;
        }

        public async Task<IdentityUtilsResult> AddToRoleAsync(Guid userId, Guid tenantId, Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            TenantRolesClaimData tenantClaimData = new TenantRolesClaimData(tenantId);

            var tenantRolesClaim = (await GetUserTenantRolesClaims(userId, tenantId));
            if (tenantRolesClaim != null)
                tenantClaimData = tenantRolesClaim
                    .Value
                    .DeserializeToTenantRolesClaimData();

            tenantClaimData.Roles = tenantClaimData
                .Roles
                .Append(new RoleBasicData(role.Id, role.NormalizedName))
                .Distinct();

            var result = await AddOrUpdateTenantRolesClaim(userId, tenantId, tenantClaimData);
            return result;
        }

        public async Task<IdentityUtilsResult> AddToRolesAsync(Guid userId, Guid tenantId, IEnumerable<Guid> roles)
        {
            var result = IdentityUtilsResult.SuccessResult;
            foreach (var role in roles)
            {
                if (result.Success)
                    result = await AddToRoleAsync(userId, tenantId, role);
                else
                    break;
            };

            return result;
        }

        public async Task<IEnumerable<RoleBasicData>> GetRolesAsync(Guid userId, Guid tenantId)
        {
            var data = await GetUserTenantRolesClaims(userId, tenantId);
            return data.Value
                .DeserializeToTenantRolesClaimData()
                .Roles;
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

        public async Task<bool> IsInRoleAsync(Guid userId, Guid tenantId, Guid role)
        {
            var roles = await GetRolesAsync(userId, tenantId);
            return roles.Select(x => x.Id).Contains(role);
        }

        public async Task<IdentityUtilsResult> RemoveFromRoleAsync(Guid userId, Guid tenantId, Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            TenantRolesClaimData tenantClaimData = new TenantRolesClaimData(tenantId);

            var tenantRolesClaim = (await GetUserTenantRolesClaims(userId, tenantId));
            if (tenantRolesClaim != null)
            {
                tenantClaimData.Roles = tenantClaimData.Roles
                    .Where(x => x.Id != role.Id);
            }

            var result = await AddOrUpdateTenantRolesClaim(userId, tenantId, tenantClaimData);
            return result;
        }

        public async Task<IEnumerable<TUserDto>> Search(UsersTenantSearch searchModel)
        {
            var usersQuery = dbContext.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.Username))
                usersQuery = usersQuery
                    .Where(x => x.NormalizedUserName == searchModel.Username.ToUpperInvariant());

            // If tenant claims must be processed
            if (searchModel.RoleId != null || searchModel.TenantId != null)
            {
                var tenantClaims = await dbContext
                    .UserClaims
                    .Where(x => x.ClaimType == TenantClaimsSchema.TenantRolesData)
                    .Where(x => usersQuery.Select(y => y.Id).Contains(x.UserId))
                    .ToListAsync();

                var usersClaimQuery = tenantClaims
                    .Select(x => new
                    {
                        userClaim = x,
                        tenantRoleClaim = x.ToClaim().Value.DeserializeToTenantRolesClaimData()
                    });

                if (searchModel.RoleId != null)
                    usersClaimQuery = usersClaimQuery.Where(x => x.tenantRoleClaim.Roles.Where(x => x.Id == searchModel.RoleId).Any());

                if (searchModel.TenantId != null)
                    usersClaimQuery = usersClaimQuery.Where(x => x.tenantRoleClaim.TenantId == searchModel.TenantId);

                var userIdsQuery = usersClaimQuery.Select(x => x.userClaim.UserId).Distinct();

                usersQuery = usersQuery
                    .Where(x => userIdsQuery.Contains(x.Id));
            }

            return await usersQuery
                .ProjectTo<TUserDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        private async Task<IdentityUtilsResult> AddOrUpdateTenantRolesClaim(Guid userId, Guid tenantId, TenantRolesClaimData tenantClaimData)
        {
            var result = IdentityResult.Success;
            var newTenantRolesClaim = new Claim(TenantClaimsSchema.TenantRolesData, tenantClaimData.Serialize());
            var oldTenantRolesClaim = await GetUserTenantRolesClaims(userId, tenantId);

            var userResult = await FindByIdAsync(userId);
            if (!userResult.Success)
                return userResult;

            if (oldTenantRolesClaim != null)
                result = await userManager.RemoveClaimAsync(userResult.Data, oldTenantRolesClaim);

            if (result.Succeeded)
                result = await userManager.AddClaimAsync(userResult.Data, newTenantRolesClaim);

            return result.ToIdentityUtilsResult();
        }

        private async Task<Claim> GetUserTenantRolesClaims(Guid userId, Guid tenantId)
        {
            var claims = await GetUserTenantRolesClaims(userId);
            return claims
                .Where(x => x.Value.DeserializeToTenantRolesClaimData().TenantId == tenantId)
                .FirstOrDefault();
        }
    }
}