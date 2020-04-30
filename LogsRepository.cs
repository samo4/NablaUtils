using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NablaUtils
{
    public static class LogRepository
    {
        public static CloudTableClient _tableClient;
        public static CloudTable LogTable { get; set; }

        static LogRepository()
        {
            var AzureAutoStorageConnectionString = FunctionHelper.GetAutoUsersConnectionString("AzureAutoStorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(AzureAutoStorageConnectionString);
            _tableClient = storageAccount.CreateCloudTableClient();
            LogTable = _tableClient.GetTableReference("log");
        }

        public static async Task EnsureDatabaseSchemaAsync()
        {
            await LogTable.CreateIfNotExistsAsync();
        }
		
		public static Task<TableResult> AddLogMessageAsync(string sub, Microsoft.Extensions.Logging.LogLevel level, Exception ex)
        {   
            return AddLogMessageAsync(sub, level, $"[{ex.Message}] {ex.StackTrace}");
        }

        public static async Task<TableResult> AddLogMessageAsync(string sub, Microsoft.Extensions.Logging.LogLevel level, string message)
        {
            try
            {
                DynamicTableEntity dte = new DynamicTableEntity { PartitionKey = sub, RowKey = DateTime.UtcNow.Ticks.ToString() };
                dte.Properties.Add("Level", EntityProperty.GeneratePropertyForString(level.ToString()));
                dte.Properties.Add("Message", EntityProperty.GeneratePropertyForString(message));
                return await LogTable.ExecuteAsync(TableOperation.Insert(dte));
            }
            catch 
            {
                return new TableResult { HttpStatusCode = 500 };
            }
        }
    }
}
