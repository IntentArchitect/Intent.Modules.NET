using System.Collections.Generic;
using System.Data;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Repositories.ExtensionMethods
{
    internal static class ProductInMemoryExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<ProductInMemory> productInMemories)
        {
            var dataTable = new DataTable();

            foreach (var item in productInMemories)
            {
                dataTable.Rows.Add();
            }

            return dataTable;
        }
    }
}