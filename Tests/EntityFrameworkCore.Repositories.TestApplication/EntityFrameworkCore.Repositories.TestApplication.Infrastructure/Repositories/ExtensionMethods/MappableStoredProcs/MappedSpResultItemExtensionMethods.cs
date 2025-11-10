using System.Collections.Generic;
using System.Data;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.ExtensionMethods.MappableStoredProcs
{
    internal static class MappedSpResultItemExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<MappedSpResultItem> mappedSpResultItems)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("SimpleProp", typeof(string));
            dataTable.Columns.Add("ComplexProp", typeof(MappedSpResultItemProperty));

            foreach (var item in mappedSpResultItems)
            {
                dataTable.Rows.Add(item.SimpleProp, item.ComplexProp);
            }

            return dataTable;
        }
    }
}