using System.Collections.Generic;

namespace IdentityUtils.Core.Contracts
{
    /// <summary>
    /// Basic API result
    /// </summary>
    public class IdentityManagementResult
    {
        public static IdentityManagementResult SuccessResult => new IdentityManagementResult { Success = true };

        public static IdentityManagementResult ErrorResult(string errorMessage)
            => new IdentityManagementResult()
            {
                Success = false,
                ErrorMessages = new List<string> { errorMessage }
            };

        public static IdentityManagementResult ErrorResult(List<string> errorMessages)
            => new IdentityManagementResult()
            {
                Success = false,
                ErrorMessages = errorMessages
            };

        public IdentityManagementResult()
        {
        }

        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; }
    }

    /// <summary>
    /// API result with payload
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IdentityManagementResult<T> : IdentityManagementResult where T : class
    {
        public static IdentityManagementResult<T> FromNonTypedResult(IdentityManagementResult result, T payload = null)
            => new IdentityManagementResult<T>
            {
                Success = result.Success,
                ErrorMessages = result.ErrorMessages,
                Payload = payload
            };

        public static IdentityManagementResult<T> SuccessResult()
            => new IdentityManagementResult<T>() { Success = true };

        public static IdentityManagementResult<T> SuccessResult(T payload)
            => new IdentityManagementResult<T>(payload) { Success = true };

        public static IdentityManagementResult<T> ErrorResult(string errorMessage)
            => new IdentityManagementResult<T>()
            {
                Success = false,
                ErrorMessages = new List<string> { errorMessage }
            };

        public static IdentityManagementResult<T> ErrorResult(string errorMessage, T payload)
            => new IdentityManagementResult<T>()
            {
                Success = false,
                ErrorMessages = new List<string> { errorMessage },
                Payload = payload
            };

        public static IdentityManagementResult<T> ErrorResult(List<string> errorMessages)
            => new IdentityManagementResult<T>()
            {
                Success = false,
                ErrorMessages = errorMessages
            };

        public IdentityManagementResult() : base()
        {
        }

        public IdentityManagementResult(T payload) : base()
        {
            Payload = payload;
        }

        public T Payload { get; set; }
    }
}