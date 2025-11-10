using System.Collections.Generic;
using System.Data;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.ExtensionMethods
{
    internal static class UdttDataEntityModelExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<UdttDataEntityModel> udttDataEntityModels)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("DataEntityID", typeof(long));

            foreach (var item in udttDataEntityModels)
            {
                dataTable.Rows.Add(item.DataEntityID);
            }

            return dataTable;
        }
    }
}