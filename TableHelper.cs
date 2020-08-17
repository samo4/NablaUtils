using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DoubleUpFxApi.NablaUtils
{
    public static class TableHelper
    {
        public static async Task<T> GetSingleAsync<T>(this CloudTable _table, string partitionKey, string rowKey) where T : ITableEntity, new()
        {
            var result = await _table.GetSingleTableResultAsync<T>(partitionKey, rowKey);
            if (result == null)
            {
                return default;
            } 
            else
            {
                return (T)result.Result;
            }
        }

        public static async Task<TableResult> GetSingleTableResultAsync<T>(this CloudTable _table, string partitionKey, string rowKey) where T: ITableEntity, new()
        {
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var tableResult = await _table.ExecuteAsync(retrieveOperation);
            return tableResult;
        }

        public static async Task<T> UpdateAsync<T>(this CloudTable _table, T tableEntityData) where T : ITableEntity, new()
        {
            var updateCallistConfig = await _table.GetSingleAsync<T>(tableEntityData.PartitionKey, tableEntityData.RowKey);
            if (updateCallistConfig != null)
            {
                var updateOperation = TableOperation.InsertOrMerge(tableEntityData);
                var tableResult = await _table.ExecuteAsync(updateOperation);
                return (T)tableResult.Result;
            }
            return default;
        }

        public static async Task<T> InsertAsync<T>(this CloudTable _table, T tableEntityData) where T : ITableEntity, new()
        {
            var operation = TableOperation.Insert(tableEntityData);
            var result = await _table.ExecuteAsync(operation);
            return (T)result.Result;
        }

        public static async Task<List<T>> GetPartitionAsync<T>(this CloudTable _table, string partitionKey, int takeCount = 1000) where T : ITableEntity, new()
        {
            var result = new List<T>();
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            query.TakeCount = takeCount;
            TableContinuationToken continuationToken = null;
            do
            {
                var response = await _table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = response.ContinuationToken;
                result.AddRange(response.Results);
            } while (continuationToken != null);
            return result;
        }
    }
}
