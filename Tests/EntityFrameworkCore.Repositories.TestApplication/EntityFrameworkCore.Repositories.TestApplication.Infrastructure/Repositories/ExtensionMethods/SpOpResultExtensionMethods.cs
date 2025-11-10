using System.Collections.Generic;
using System.Data;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.ExtensionMethods
{
    internal static class SpOpResultExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<SpOpResult> spOpResults)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Data", typeof(string));

            foreach (var item in spOpResults)
            {
                dataTable.Rows.Add(item.Data);
            }

            return dataTable;
        }
    }
}