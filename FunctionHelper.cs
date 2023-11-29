using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sentry;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace NablaUtils
{
    public record UpdateResponse
    {
        public string Status { get; init; }
    }

    public record DeleteResponse
    {
        public string Status { get; init; }
    }

    public record InsertResponse
    {
        public string Status { get; init; }
        public Guid InsertedId { get; init; }
        public int InsertedCount { get; init; }
        public int FailedCount { get; init; } = 0;
    }

    public record ExceptionResponse
    {
        public string Status { get; init; }
        public string Message { get; init; }
        public string StackTrace { get; init; }
    }

    public record ValidationResponse
    {
        public string Status { get; init; }
        public string Message { get; init; }
        public IEnumerable<ValidationResult> InvalidProperties { get; init; }
    }

    public static class FunctionHelper
    {

        /*
        Original source https://github.com/Azure/Azure-Functions/issues/717#issuecomment-400098791
        https://blog.torib.io/2020/03/03/getting-azure-function-connection-strings-from-configurations/
        */

        public static string GetSqlConnectionString(string name)
        {
            string conStr = System.Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable($"SQLCONNSTR_{name}", EnvironmentVariableTarget.Process);

            if (string.IsNullOrWhiteSpace(conStr))
            {
                throw new Exception("Empty connection string from Environment!");
            }
            return conStr;
        }
        public static string GetSqlAzureConnectionString(string name)
        {
            string conStr = System.Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable($"SQLAZURECONNSTR_{name}", EnvironmentVariableTarget.Process);
            return conStr;
        }
        public static string GetMySqlConnectionString(string name)
        {
            string conStr = System.Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable($"MYSQLCONNSTR_{name}", EnvironmentVariableTarget.Process);
            return conStr;
        }
        public static string GetCustomConnectionString(string name)
        {
            string conStr = System.Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable($"CUSTOMCONNSTR_{name}", EnvironmentVariableTarget.Process);
            return conStr;
        }
        /* end */



        public static string GetAutoUsersConnectionString(string name)
        {
            string conStr = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr))
                 conStr = System.Environment.GetEnvironmentVariable($"CUSTOMCONNSTR_{name}", EnvironmentVariableTarget.Process);

            if (string.IsNullOrWhiteSpace(conStr))
            {
                throw new Exception("Empty connection string from Environment!");
            }
            return conStr;
        }

        public static async Task<dynamic> EnsureBodyAsync(HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic body = JsonConvert.DeserializeObject(requestBody);
            if (body == null) { throw new ArgumentNullException(nameof(body)); }
            return body;
        }

        public static void EnsureProperty(object obj, string name)
        {
            var jObject = obj as JObject;
            if (jObject == null)
            {
                throw new Exception("unknown object type.. only jObject supported  ");
            }
            if (!jObject.ContainsKey(name))
            {
                throw new ArgumentNullException(name);
            }
        }

        public static bool IsSuccess(this TableResult result)
        {
            return result.HttpStatusCode >= 200 && result.HttpStatusCode < 300;
        }

        public static bool IsSuccess(this HttpStatusCode result)
        {
            return (int) result >= 200 && (int) result < 300;
        }

		public static long GetCurrentTimeStamp(int roundtoMinutes)
        {
            var tsNow = (double)new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            return (long)Math.Floor(tsNow / (roundtoMinutes * 60)) * (roundtoMinutes * 60);
        }

        public static ObjectResult ReturnValidationResponse(IEnumerable<ValidationResult> validationResults)
        {
            return new ObjectResult(new ValidationResponse { Status = "error", Message = "Body validation failed.", InvalidProperties = validationResults.ToArray() }) { StatusCode = 422 };
        }

        public static ObjectResult ReturnErrorResponse(Exception ex, ILogger log = null)
        {
            if (log != null)
            {
                log.LogError(ex, "ReturnErrorResponse");
            }
            SentrySdk.CaptureException(ex);
            var result = new BadRequestObjectResult(new ExceptionResponse { Status = "error", Message = ex.Message, StackTrace = ex.StackTrace });
            if (ex is SecurityException)
            {
                result.StatusCode = 403;
            }
            return result;
        }

        public static DateTimeOffset GetDateTimeOffsetFromRequest(this HttpRequest req, string name)
        {
            var result = DateTimeOffset.MinValue;
            if (!req.Query.ContainsKey(name))
            {
                return result;
            }
            if (DateTimeOffset.TryParseExact(req.Query[name], "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out result))
            {
                return result;
            }
            if (DateTimeOffset.TryParseExact(req.Query[name], "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out result))
            {
                return result;
            }
            throw new ArgumentException(name);
        }

        public static bool TryGetParameterFromRequest(this HttpRequest req, string name, out DateTimeOffset result)
        {
            result = DateTimeOffset.MinValue;
            if (!req.Query.ContainsKey(name))
            {
                return false;
            }
            if (DateTimeOffset.TryParseExact(req.Query[name], "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out result))
            {
                return true;
            }
            if (DateTimeOffset.TryParseExact(req.Query[name], "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out result))
            {
                return true;
            }
            return false;
        }

        public static bool TryGetParameterFromRequest(this HttpRequest req, string name, out Guid result)
        {
            result = Guid.Empty;
            if (!req.Query.ContainsKey(name))
            {
                return false;
            }
            return Guid.TryParse(req.Query[name], out result);
        }

        public static bool GetBoolFromRequest(this HttpRequest req, string name, out bool result)
        {
            result = false;
            if (!req.Query.ContainsKey(name))
            {
                return false;
            }
            return bool.TryParse(req.Query[name], out result);
        }
    }
}
