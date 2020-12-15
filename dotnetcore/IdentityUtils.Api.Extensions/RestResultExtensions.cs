using IdentityUtils.Commons;
using IdentityUtils.Core.Contracts.Commons;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Extensions
{
    internal static class RestResultExtensions
    {
        internal async static Task<IdentityUtilsResult> ParseRestResultTask(this Task<RestResult<IdentityUtilsResult>> restResultTask)
        {
            var restResult = await restResultTask;

            if (!restResult.Success)
                return IdentityUtilsResult.ErrorResult(restResult.ErrorMessages);

            return restResult.ResponseData;
        }

        internal async static Task<IdentityUtilsResult<T>> ParseRestResultTask<T>(this Task<RestResult<IdentityUtilsResult<T>>> restResultTask)
        {
            var restResult = await restResultTask;

            if (!restResult.Success)
                return IdentityUtilsResult<T>.ErrorResult(restResult.ErrorMessages);

            return restResult.ResponseData;
        }

        internal static IdentityUtilsResult<T> ToIdentityResult<T>(this RestResult<T> restResult)
        {
            return new IdentityUtilsResult<T>
            {
                Success = restResult.Success,
                Data = restResult.ResponseData,
                ErrorMessages = restResult.ErrorMessages
            };
        }
    }
}