using System.Collections.Generic;
using System.Data;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Repositories.ExtensionMethods.Dbo
{
    internal static class BrandTypeExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<BrandType> brandTypes)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("IsActive", typeof(bool));

            foreach (var item in brandTypes)
            {
                dataTable.Rows.Add(item.Name, item.IsActive);
            }

            return dataTable;
        }
    }
}