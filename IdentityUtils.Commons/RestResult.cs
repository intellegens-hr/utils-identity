using System.Collections.Generic;

namespace IdentityUtils.Commons
{
    public class RestResult
    {
        public static RestResult SuccessResult => new RestResult { Success = true };

        public static RestResult ErrorResult(string errorMessage)
            => new RestResult()
            {
                Success = false,
                ErrorMessages = new List<string> { errorMessage }
            };

        public static RestResult ErrorResult(List<string> errorMessages)
            => new RestResult()
            {
                Success = false,
                ErrorMessages = errorMessages
            };

        public RestResult()
        {
        }

        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; }
    }

    /// <summary>
    /// API result with payload
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RestResult<T> : RestResult
    {
        public static RestResult<T> FromNonTypedResult(RestResult result)
            => new RestResult<T>
            {
                Success = result.Success,
                ErrorMessages = result.ErrorMessages
            };

        public static RestResult<T> FromNonTypedResult(RestResult result, T data)
            => new RestResult<T>
            {
                Success = result.Success,
                ErrorMessages = result.ErrorMessages,
                Data = data
            };

        public static RestResult<T> SuccessResult()
            => new RestResult<T>() { Success = true };

        public static RestResult<T> SuccessResult(T data)
            => new RestResult<T>(data) { Success = true };

        public static RestResult<T> ErrorResult(string errorMessage)
            => new RestResult<T>()
            {
                Success = false,
                ErrorMessages = new List<string> { errorMessage }
            };

        public static RestResult<T> ErrorResult(string errorMessage, T data)
            => new RestResult<T>()
            {
                Success = false,
                ErrorMessages = new List<string> { errorMessage },
                Data = data
            };

        public static RestResult<T> ErrorResult(List<string> errorMessages)
            => new RestResult<T>()
            {
                Success = false,
                ErrorMessages = errorMessages
            };

        public RestResult() : base()
        {
        }

        public RestResult(T data) : base()
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}