using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NablaUtils.SimpleInBlue
{
    internal record EmailsApiError
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class EmailsApiException : Exception
    {
        public string ErrorCode { get; private set; }
        public dynamic ErrorContent { get; private set; }

        public EmailsApiException(string errorCode, string message, dynamic errorContent = null) : base(message)
        {
            ErrorCode = errorCode;
            ErrorContent = errorContent;
        }
    }
}
