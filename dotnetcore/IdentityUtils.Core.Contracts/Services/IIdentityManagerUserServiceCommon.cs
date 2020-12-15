using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Users;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityUtils.Core.Contracts.Services
{
    public interface IIdentityManagerUserServiceCommon<TUser, TUserDto>
        where TUser : IdentityManagerUser
        where TUserDto : class, IIdentityManagerUserDto
    {
        Task<IdentityUtilsResult> AddClaimAsync(Guid userId, Claim claim);

        Task<IdentityUtilsResult> AddClaimsAsync(Guid userId, IEnumerable<Claim> claims);

        Task<IdentityUtilsResult<TUserDto>> CreateUser(TUserDto user);

        Task<IdentityUtilsResult> DeleteUser(Guid userId);

        Task<IdentityUtilsResult<TUser>> FindByIdAsync(Guid id);

        Task<IdentityUtilsResult<TDto>> FindByIdAsync<TDto>(Guid id) where TDto : class;

        Task<IdentityUtilsResult<TUser>> FindByNameAsync(string name);

        Task<IdentityUtilsResult<TUserDto>> FindByNameAsync<TUserDto>(string name) where TUserDto : class;

        Task<IdentityUtilsResult<string>> GeneratePasswordResetTokenAsync(string username);

        Task<List<TUser>> GetAllUsers();

        Task<IdentityUtilsResult> ResetPasswordAsync(string username, string token, string newPassword);

        Task<IdentityUtilsResult> UpdateUser(TUserDto user);
    }
}