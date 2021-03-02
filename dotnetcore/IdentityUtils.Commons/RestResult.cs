using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IdentityUtils.Commons
{
    public class RestResult
    {
        public RestResult()
        {
        }

        public IEnumerable<string> ErrorMessages { get; set; } = Enumerable.Empty<string>();
        public int StatusCode { get; set; }
        public bool Success => StatusCode == 200;
    }

    /// <summary>
    /// API result with payload
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RestResult<T> : RestResult
    {
        public RestResult() : base()
        {
        }

        public RestResult(string rawData, T data) : base()
        {
            ResponseDataRaw = rawData;
            ResponseData = data;
        }

        public T ResponseData { get; set; }
        public string ResponseDataRaw { get; set; }
    }
}