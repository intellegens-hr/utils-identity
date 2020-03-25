﻿using System.Collections.Generic;

namespace IdentityUtils.Core.Contracts.Commons
{
    /// <summary>
    /// Common result all services should return
    /// </summary>
    public class IdentityUtilsResult
    {
        public static IdentityUtilsResult SuccessResult => new IdentityUtilsResult { Success = true };

        public static IdentityUtilsResult ErrorResult(string errorMessage)
            => new IdentityUtilsResult()
            {
                Success = false,
                ErrorMessages = new List<string> { errorMessage }
            };

        public static IdentityUtilsResult ErrorResult(List<string> errorMessages)
            => new IdentityUtilsResult()
            {
                Success = false,
                ErrorMessages = errorMessages
            };

        public IdentityUtilsResult()
        {
        }

        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; }
    }

    /// <summary>
    /// Common result with payload
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IdentityUtilsResult<T> : IdentityUtilsResult where T : class
    {
        public static IdentityUtilsResult<T> FromNonTypedResult(IdentityUtilsResult result, T payload = null)
            => new IdentityUtilsResult<T>
            {
                Success = result.Success,
                ErrorMessages = result.ErrorMessages,
                Payload = payload
            };

        public static IdentityUtilsResult<T> SuccessResult()
            => new IdentityUtilsResult<T>() { Success = true };

        public static IdentityUtilsResult<T> SuccessResult(T payload)
            => new IdentityUtilsResult<T>(payload) { Success = true };

        public static IdentityUtilsResult<T> ErrorResult(string errorMessage)
            => new IdentityUtilsResult<T>()
            {
                Success = false,
                ErrorMessages = new List<string> { errorMessage }
            };

        public static IdentityUtilsResult<T> ErrorResult(string errorMessage, T payload)
            => new IdentityUtilsResult<T>()
            {
                Success = false,
                ErrorMessages = new List<string> { errorMessage },
                Payload = payload
            };

        public static IdentityUtilsResult<T> ErrorResult(List<string> errorMessages)
            => new IdentityUtilsResult<T>()
            {
                Success = false,
                ErrorMessages = errorMessages
            };

        public IdentityUtilsResult() : base()
        {
        }

        public IdentityUtilsResult(T payload) : base()
        {
            Payload = payload;
        }

        public T Payload { get; set; }
    }
}