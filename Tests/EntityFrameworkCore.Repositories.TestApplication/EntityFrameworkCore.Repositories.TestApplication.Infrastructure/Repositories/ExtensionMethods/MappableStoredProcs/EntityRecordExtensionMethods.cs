using System;
using System.Collections.Generic;
using System.Data;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.ExtensionMethods.MappableStoredProcs
{
    internal static class EntityRecordExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<EntityRecord> entityRecords)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(Guid));
            dataTable.Columns.Add("Name", typeof(string));

            foreach (var item in entityRecords)
            {
                dataTable.Rows.Add(item.Id, item.Name);
            }

            return dataTable;
        }
    }
}