using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NablaUtils
{
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
    }
}
