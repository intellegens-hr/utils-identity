using IdentityUtils.Commons;
using IdentityUtils.Core.Contracts.Commons;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Extensions
{
    internal static class RestResultExtensions
    {
        internal static IdentityUtilsResult<T> ToIdentityResult<T>(this RestResult<T> restResult)
        {
            return new IdentityUtilsResult<T>
            {
                Success = restResult.Success,
                Payload = restResult.Data,
                ErrorMessages = restResult.ErrorMessages
            };
        }

        internal async static Task<IdentityUtilsResult> ParseRestResultTask(this Task<RestResult<IdentityUtilsResult>> restResultTask)
        {
            var restResult = await restResultTask;

            if (!restResult.Success)
                return IdentityUtilsResult.ErrorResult(restResult.ErrorMessages);

            return restResult.Data;
        }

        internal async static Task<IdentityUtilsResult<T>> ParseRestResultTask<T>(this Task<RestResult<IdentityUtilsResult<T>>> restResultTask)
        {
            var restResult = await restResultTask;

            if (!restResult.Success)
                return IdentityUtilsResult<T>.ErrorResult(restResult.ErrorMessages);

            return restResult.Data;
        }
    }
}