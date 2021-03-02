using AutoMapper;
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
using System.Threading.Tasks;

namespace IdentityUtils.Core.Services
{
    public class IdentityManagerUserService<TUser, TRole, TUserDto> :
        IdentityManagerUserServiceBase<TUser, TUserDto>,
        IIdentityManagerUserService<TUser, TUserDto>
        where TUser : IdentityManagerUser
        where TRole : IdentityManagerRole
        where TUserDto : class, IIdentityManagerUserDto
    {
        private readonly IdentityManagerDbContext<TUser, TRole> dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<TUser> userManager;

        public IdentityManagerUserService(
            UserManager<TUser> userManager,
            IdentityManagerDbContext<TUser, TRole> dbContext,
            IMapper mapper) : base(userManager, mapper)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<IdentityUtilsResult> AddToRoleAsync(Guid userId, Guid roleId)
        {
            if (!await IsInRoleAsync(userId, roleId))
            {
                dbContext.UserRoles.Add(new IdentityUserRole<Guid>() { RoleId = roleId, UserId = userId });
                await dbContext.SaveChangesAsync();
            }

            return IdentityUtilsResult.SuccessResult;
        }

        public async Task<IdentityUtilsResult> AddToRolesAsync(Guid userId, IEnumerable<Guid> roles)
        {
            var result = IdentityUtilsResult.SuccessResult;
            foreach (var role in roles)
            {
                if (result.Success)
                    result = await AddToRoleAsync(userId, role);
                else
                    break;
            };

            return result;
        }

        public async Task<IEnumerable<RoleBasicData>> GetRolesAsync(Guid userId)
        {
            var userRoles = dbContext
                .UserRoles
                .Where(x => x.UserId == userId)
                .Select(x => x.RoleId);

            return await dbContext
                .Roles
                .Where(x => userRoles.Contains(x.Id))
                .Select(x => new RoleBasicData(x.Id, x.NormalizedName))
                .ToListAsync();
        }

        public Task<bool> IsInRoleAsync(Guid userId, Guid role)
        {
            return dbContext.UserRoles
                .Where(x => x.UserId == userId && x.RoleId == role)
                .AnyAsync();
        }

        public async Task<IdentityUtilsResult> RemoveFromRoleAsync(Guid userId, Guid roleId)
        {
            var roleToRemoveQuery = dbContext.UserRoles.Where(x => x.UserId == userId && x.RoleId == roleId);
            dbContext.UserRoles.RemoveRange(roleToRemoveQuery);
            await dbContext.SaveChangesAsync();

            return IdentityUtilsResult.SuccessResult;
        }

        public async Task<IEnumerable<TUserDto>> Search(UsersSearch searchModel)
        {
            var usersQuery = dbContext.Users.AsQueryable();

            if (searchModel.RoleId != null)
            {
                var rolesSubQuery = dbContext.UserRoles
                    .Where(x => x.RoleId == searchModel.RoleId)
                    .Select(x => x.UserId);

                usersQuery = usersQuery.Where(x => rolesSubQuery.Contains(x.Id));
            }

            var users = await usersQuery.ToListAsync();
            return mapper.Map<IEnumerable<TUserDto>>(users);
        }
    }
}