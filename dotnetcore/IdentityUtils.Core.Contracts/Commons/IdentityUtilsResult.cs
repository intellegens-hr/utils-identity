using System.Collections.Generic;
using System.Linq;

namespace IdentityUtils.Core.Contracts.Commons
{
    /// <summary>
    /// Common result all services should return
    /// </summary>
    public class IdentityUtilsResult
    {
        public IdentityUtilsResult()
        {
        }

        public static IdentityUtilsResult SuccessResult
            => new IdentityUtilsResult { Success = true };

        public IEnumerable<string> ErrorMessages { get; set; } = Enumerable.Empty<string>();

        public bool Success { get; set; }

        public static IdentityUtilsResult ErrorResult(string errorMessage)
            => new IdentityUtilsResult()
            {
                Success = false,
                ErrorMessages = new string[] { errorMessage }
            };

        public static IdentityUtilsResult ErrorResult(IEnumerable<string> errorMessages)
            => new IdentityUtilsResult()
            {
                Success = false,
                ErrorMessages = errorMessages
            };
    }

    /// <summary>
    /// Common result with payload
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IdentityUtilsResult<T> : IdentityUtilsResult
    {
        public IdentityUtilsResult() : base()
        {
        }

        public IdentityUtilsResult(T data) : base()
        {
            Data = new T[] { data };
        }

        public IdentityUtilsResult(IEnumerable<T> data) : base()
        {
            Data = data;
        }

        public IEnumerable<T> Data { get; set; }

        public static IdentityUtilsResult<T> ErrorResult(string errorMessage)
            => new IdentityUtilsResult<T>()
            {
                Success = false,
                ErrorMessages = new string[] { errorMessage }
            };

        public static IdentityUtilsResult<T> ErrorResult(IEnumerable<string> errorMessages)
            => new IdentityUtilsResult<T>()
            {
                Success = false,
                ErrorMessages = errorMessages
            };

        public static IdentityUtilsResult<T> FromNonTypedResult(IdentityUtilsResult result)
            => new IdentityUtilsResult<T>
            {
                Success = result.Success,
                ErrorMessages = result.ErrorMessages
            };

        public static IdentityUtilsResult<T> FromNonTypedResult(IdentityUtilsResult result, T data)
            => new IdentityUtilsResult<T>
            {
                Success = result.Success,
                ErrorMessages = result.ErrorMessages,
                Data = new T[] { data }
            };

        public static IdentityUtilsResult<T> FromNonTypedResult(IdentityUtilsResult result, IEnumerable<T> data)
            => new IdentityUtilsResult<T>
            {
                Success = result.Success,
                ErrorMessages = result.ErrorMessages,
                Data = data
            };

        public static IdentityUtilsResult<T> SuccessResult(T data)
            => new IdentityUtilsResult<T>(data) { Success = true };

        public static IdentityUtilsResult<T> SuccessResult(IEnumerable<T> data)
            => new IdentityUtilsResult<T>(data) { Success = true };
    }
}