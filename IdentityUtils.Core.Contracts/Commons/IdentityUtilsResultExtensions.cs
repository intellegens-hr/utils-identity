using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IdentityUtils.Core.Contracts.Commons
{
    /// <summary>
    /// Basic API result
    /// </summary>
    public static class IdentityUtilsResultExtensions
    {
        public static IdentityUtilsResult ToIdentityUtilsResult(this IdentityResult result)
             => new IdentityUtilsResult
             {
                 Success = result.Succeeded,
                 ErrorMessages = result.Errors.Select(x => $"{x.Code}: {x.Description}").ToList()
             };

        public static IdentityUtilsResult<T> ToTypedResult<T>(this IdentityUtilsResult result, T payload = null) where T : class
           => IdentityUtilsResult<T>.FromNonTypedResult(result, payload);

        public static IdentityUtilsResult ToIdentityUtilsResult(this (bool isValid, List<ValidationResult> validationResults) result)
        {
            if (result.isValid)
                return IdentityUtilsResult.SuccessResult;
            else
            {
                var flatResults = result
                    .validationResults
                    .Select(x => $"{x.ErrorMessage}: {string.Join(',', x.MemberNames)}")
                    .ToList();

                return IdentityUtilsResult.ErrorResult(flatResults);
            }
        }
    }
}