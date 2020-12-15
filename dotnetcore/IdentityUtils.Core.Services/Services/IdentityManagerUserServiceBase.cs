using AutoMapper;
using IdentityUtils.Commons.Validation;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityUtils.Core.Services
{
    public abstract class IdentityManagerUserServiceBase<TUser, TUserDto> : IIdentityManagerUserServiceCommon<TUser, TUserDto>
        where TUser : IdentityManagerUser
        where TUserDto : class, IIdentityManagerUserDto
    {
        private readonly IMapper mapper;
        private readonly UserManager<TUser> userManager;

        public IdentityManagerUserServiceBase(
            UserManager<TUser> userManager,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<IdentityUtilsResult> AddClaimAsync(Guid userId, Claim claim)
            => await AddClaimsAsync(userId, new List<Claim> { claim });

        public async Task<IdentityUtilsResult> AddClaimsAsync(Guid userId, IEnumerable<Claim> claims)
        {
            var userResult = await FindByIdAsync(userId);
            if (!userResult.Success)
                return userResult;

            var result = await userManager.AddClaimsAsync(userResult.Data, claims);
            return result.ToIdentityUtilsResult();
        }

        public async Task<IdentityUtilsResult<TUserDto>> CreateUser(TUserDto user)
        {
            var userDb = mapper.Map<TUser>(user);

            var result = ModelValidator.ValidateDataAnnotations(userDb).ToIdentityUtilsResult();
            if (!result.Success)
                return result.ToTypedResult<TUserDto>();

            var managerValidationResult = await new UserValidator<TUser>().ValidateAsync(userManager, userDb);
            if (!managerValidationResult.Succeeded)
                return managerValidationResult.ToIdentityUtilsResult().ToTypedResult<TUserDto>();

            result = (await userManager.CreateAsync(userDb, user.Password ?? "")).ToIdentityUtilsResult();

            if (result.Success)
            {
                user.Password = null;
                mapper.Map(userDb, user);
            }

            return result.ToTypedResult(user);
        }

        public async Task<IdentityUtilsResult> DeleteUser(Guid userId)
        {
            var userResult = await FindByIdAsync(userId);
            if (!userResult.Success)
                return userResult;

            var identityResult = await userManager.DeleteAsync(userResult.Data);
            return identityResult.ToIdentityUtilsResult();
        }

        /// <summary>
        /// Retrieves user by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IdentityUtilsResult<TUser>> FindByIdAsync(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return IdentityUtilsResult<TUser>.ErrorResult("User with specified ID not found");
            else
                return IdentityUtilsResult<TUser>.SuccessResult(user);
        }

        /// <summary>
        /// Uses AutoMapper to map retrieved user to speciefied type
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IdentityUtilsResult<TDto>> FindByIdAsync<TDto>(Guid id) where TDto : class
        {
            var user = await FindByIdAsync(id);

            return user.Success
                ? IdentityUtilsResult<TDto>.SuccessResult(mapper.Map<TDto>(user.Data))
                : IdentityUtilsResult<TDto>.ErrorResult(user.ErrorMessages);
        }

        public async Task<IdentityUtilsResult<TUser>> FindByNameAsync(string name)
        {
            var user = await userManager.FindByNameAsync(name);
            if (user == null)
                return IdentityUtilsResult<TUser>.ErrorResult("User with specified name not found");

            return IdentityUtilsResult<TUser>.SuccessResult(user);
        }

        public async Task<IdentityUtilsResult<TUserDto>> FindByNameAsync<TUserDto>(string name) where TUserDto : class
        {
            var userResult = await FindByNameAsync(name);

            if (!userResult.Success)
                return IdentityUtilsResult<TUserDto>.ErrorResult(userResult.ErrorMessages);

            return IdentityUtilsResult<TUserDto>.SuccessResult(mapper.Map<TUserDto>(userResult.Data));
        }

        public async Task<IdentityUtilsResult<string>> GeneratePasswordResetTokenAsync(string username)
        {
            var userResult = await FindByNameAsync(username);
            var result = userResult.ToTypedResult<string>();

            if (result.Success)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(userResult.Data);
                result.Data = token;
            }

            return result;
        }

        public async Task<List<TUser>> GetAllUsers()
        {
            return await userManager.Users.ToListAsync();
        }

        public async Task<IdentityUtilsResult> ResetPasswordAsync(string username, string token, string newPassword)
        {
            var userResult = await FindByNameAsync(username);

            if (!userResult.Success)
                return userResult;

            var result = await userManager.ResetPasswordAsync(userResult.Data, token, newPassword);
            return result.ToIdentityUtilsResult();
        }

        public async Task<IdentityUtilsResult> UpdateUser(TUserDto user)
        {
            var userDbResult = await FindByIdAsync(user.Id);

            if (!userDbResult.Success)
                return IdentityUtilsResult.ErrorResult(userDbResult.ErrorMessages);

            var userDb = userDbResult.Data;

            mapper.Map(user, userDb);

            var result = ModelValidator.ValidateDataAnnotations(userDb).ToIdentityUtilsResult();
            if (!result.Success)
                return result.ToTypedResult<TUserDto>();

            var managerValidationResult = await new UserValidator<TUser>().ValidateAsync(userManager, userDb);
            if (!managerValidationResult.Succeeded)
                return managerValidationResult.ToIdentityUtilsResult().ToTypedResult<TUserDto>();

            var identityResult = await userManager.UpdateAsync(userDb);

            if (!identityResult.Succeeded)
                return identityResult.ToIdentityUtilsResult();

            mapper.Map(userDbResult.Data, user);
            return IdentityUtilsResult.SuccessResult;
        }
    }
}