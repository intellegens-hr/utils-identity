using System.Collections.Generic;

namespace Commons
{
    public class Result
    {
        public static Result SuccessResult => new Result { Success = true };

        public static Result ErrorResult(string errorMessage)
            => new Result()
            {
                Success = false,
                ErrorMessages = new List<string> { errorMessage }
            };

        public static Result ErrorResult(List<string> errorMessages)
            => new Result()
            {
                Success = false,
                ErrorMessages = errorMessages
            };

        public Result()
        {
        }

        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; }
    }

    /// <summary>
    /// API result with payload
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T> : Result where T : class
    {
        public static Result<T> FromNonTypedResult(Result result, T payload = null)
            => new Result<T>
            {
                Success = result.Success,
                ErrorMessages = result.ErrorMessages,
                Payload = payload
            };

        public static Result<T> SuccessResult()
            => new Result<T>() { Success = true };

        public static Result<T> SuccessResult(T payload)
            => new Result<T>(payload) { Success = true };

        public static Result<T> ErrorResult(string errorMessage)
            => new Result<T>()
            {
                Success = false,
                ErrorMessages = new List<string> { errorMessage }
            };

        public static Result<T> ErrorResult(string errorMessage, T payload)
            => new Result<T>()
            {
                Success = false,
                ErrorMessages = new List<string> { errorMessage },
                Payload = payload
            };

        public static Result<T> ErrorResult(List<string> errorMessages)
            => new Result<T>()
            {
                Success = false,
                ErrorMessages = errorMessages
            };

        public Result() : base()
        {
        }

        public Result(T payload) : base()
        {
            Payload = payload;
        }

        public T Payload { get; set; }
    }
}