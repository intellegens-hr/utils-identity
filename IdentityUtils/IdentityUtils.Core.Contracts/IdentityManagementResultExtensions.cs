using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IdentityUtils.Core.Contracts
{
    /// <summary>
    /// Basic API result
    /// </summary>
    public static class IdentityManagementResultExtensions
    {
        public static IdentityManagementResult ToIdentityManagementResult(this IdentityResult result)
             => new IdentityManagementResult
             {
                 Success = result.Succeeded,
                 ErrorMessages = result.Errors.Select(x => $"{x.Code}: {x.Description}").ToList()
             };

        public static IdentityManagementResult<T> ToTypedResult<T>(this IdentityManagementResult result, T payload = null) where T : class
           => IdentityManagementResult<T>.FromNonTypedResult(result, payload);

        public static IdentityManagementResult ToIdentityManagementResult(this (bool isValid, List<ValidationResult> validationResults) result)
        {
            if (result.isValid)
                return IdentityManagementResult.SuccessResult;
            else
            {
                var flatResults = result
                    .validationResults
                    .Select(x => $"{x.ErrorMessage}: {string.Join(',', x.MemberNames)}")
                    .ToList();

                return IdentityManagementResult.ErrorResult(flatResults);
            }
        }
    }
}