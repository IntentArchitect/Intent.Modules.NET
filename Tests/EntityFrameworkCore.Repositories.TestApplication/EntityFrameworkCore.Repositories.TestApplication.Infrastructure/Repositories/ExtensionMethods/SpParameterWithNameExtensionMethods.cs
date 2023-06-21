using System.Collections.Generic;
using System.Data;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.ExtensionMethods
{
    internal static class SpParameterWithNameExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<SpParameterWithName> spParameterWithNames)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Attribute", typeof(string));

            foreach (var item in spParameterWithNames)
            {
                dataTable.Rows.Add(item.Attribute);
            }

            return dataTable;
        }
    }
}