using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NablaUtils
{
    public static class DataRecordHasColumn
    {
        //public static bool HasColumn(this IDataRecord r, string columnName)
        //{
        //    try
        //    {
        //        return r.GetOrdinal(columnName) >= 0;
        //    }
        //    catch (IndexOutOfRangeException)
        //    {
        //        return false;
        //    }
        //}

        // which is better?

        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
