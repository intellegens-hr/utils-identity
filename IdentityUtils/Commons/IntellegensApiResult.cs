namespace Commons
{
    /// <summary>
    /// Basic API result
    /// </summary>
    public class IntellegensApiResult
    {
        public IntellegensApiResult()
        {
        }

        public IntellegensApiResult(int statusCode, string statusMessage)
        {
            StatusCode = statusCode;
            StatusMessage = statusMessage;
        }

        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }

    /// <summary>
    /// API result with payload
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntellegensApiResult<T> : IntellegensApiResult
    {
        public IntellegensApiResult() : base()
        {
        }

        public IntellegensApiResult(int statusCode, string statusMessage, T payload) : base(statusCode, statusMessage)
        {
            Payload = payload;
        }

        public T Payload { get; set; }
    }
}