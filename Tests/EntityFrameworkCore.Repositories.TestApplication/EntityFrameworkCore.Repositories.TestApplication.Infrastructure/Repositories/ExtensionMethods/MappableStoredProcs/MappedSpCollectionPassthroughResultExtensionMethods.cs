using System.Collections.Generic;
using System.Data;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.ExtensionMethods.MappableStoredProcs
{
    internal static class MappedSpCollectionPassthroughResultExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<MappedSpCollectionPassthroughResult> mappedSpCollectionPassthroughResults)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Attribute", typeof(string));

            foreach (var item in mappedSpCollectionPassthroughResults)
            {
                dataTable.Rows.Add(item.Attribute);
            }

            return dataTable;
        }
    }
}