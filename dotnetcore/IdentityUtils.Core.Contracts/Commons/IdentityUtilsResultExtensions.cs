using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
                 ErrorMessages = result.Errors.Select(x => $"{x.Code}: {x.Description}")
             };

        public static IdentityUtilsResult ToIdentityUtilsResult(this (bool isValid, IEnumerable<ValidationResult> validationResults) result)
        {
            if (result.isValid)
                return IdentityUtilsResult.SuccessResult;
            else
            {
                var flatResults = result
                    .validationResults
                    .Select(x => $"{x.ErrorMessage}: {string.Join(',', x.MemberNames)}");

                return IdentityUtilsResult.ErrorResult(flatResults);
            }
        }

        public static IdentityUtilsResult<T> ToTypedResult<T>(this IdentityUtilsResult result, T payload = null) where T : class
            => IdentityUtilsResult<T>.FromNonTypedResult(result, payload);

        public static async Task<(IdentityUtilsResult<T> result, T data)> UnpackSingleOrDefault<T>(this Task<IdentityUtilsResult<T>> task)
        {
            var result = await task;
            return (result, (result.Data ?? Array.Empty<T>()).SingleOrDefault());
        }
    }
}