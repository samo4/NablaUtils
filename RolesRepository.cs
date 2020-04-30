using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NablaUtils
{
    public static class RolesRepository
    {
        public static CloudTableClient _tableClient;
        public static CloudTable RolesTable { get; set; }

        static RolesRepository()
        {
            var AzureAutoStorageConnectionString = FunctionHelper.GetAutoUsersConnectionString("AzureAutoStorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(AzureAutoStorageConnectionString);
            _tableClient = storageAccount.CreateCloudTableClient();
            RolesTable = _tableClient.GetTableReference("roles");
        }

        public static async Task EnsureDatabaseSchemaAsync()
        {
            await RolesTable.CreateIfNotExistsAsync();
        }

        public static async Task<bool> IsUserAdmin(string sub)
        {
            var result = await RolesTable.ExecuteAsync(TableOperation.Retrieve("admin", sub));
            if (result.HttpStatusCode == 404)
                return false;
            return result.Result != null;
        }
    }
}
