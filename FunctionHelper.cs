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
