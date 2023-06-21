using System;
using System.Collections.Generic;
using System.Data;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.ExtensionMethods
{
    internal static class SpParameterExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<SpParameter> spParameters)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("AttributeBinary", typeof(byte[]));
            dataTable.Columns.Add("AttributeBool", typeof(bool));
            dataTable.Columns.Add("AttributeByte", typeof(byte));
            dataTable.Columns.Add("AttributeDate", typeof(DateTime));
            dataTable.Columns.Add("AttributeDateTime", typeof(DateTime));
            dataTable.Columns.Add("AttributeDateTimeOffset", typeof(DateTimeOffset));
            dataTable.Columns.Add("AttributeDecimal", typeof(decimal));
            dataTable.Columns.Add("AttributeDouble", typeof(double));
            dataTable.Columns.Add("AttributeFloat", typeof(float));
            dataTable.Columns.Add("AttributeGuid", typeof(Guid));
            dataTable.Columns.Add("AttributeInt", typeof(int));
            dataTable.Columns.Add("AttributeLong", typeof(long));
            dataTable.Columns.Add("AttributeShort", typeof(short));
            dataTable.Columns.Add("AttributeString", typeof(string));

            foreach (var item in spParameters)
            {
                dataTable.Rows.Add(item.AttributeBinary, item.AttributeBool, item.AttributeByte, item.AttributeDate, item.AttributeDateTime, item.AttributeDateTimeOffset, item.AttributeDecimal, item.AttributeDouble, item.AttributeFloat, item.AttributeGuid, item.AttributeInt, item.AttributeLong, item.AttributeShort, item.AttributeString);
            }

            return dataTable;
        }
    }
}