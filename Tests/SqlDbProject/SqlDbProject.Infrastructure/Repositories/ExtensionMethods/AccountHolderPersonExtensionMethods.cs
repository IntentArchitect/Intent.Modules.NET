using System.Data;
using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Contracts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Repositories.ExtensionMethods
{
    internal static class AccountHolderPersonExtensionMethods
    {
        public static DataTable ToDataTable(this IEnumerable<AccountHolderPerson> accountHolderPeople)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("StakeholderId", typeof(long));
            dataTable.Columns.Add("Birthdate", typeof(DateTime));
            dataTable.Columns.Add("SexTypeId", typeof(int));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("Height", typeof(decimal));

            foreach (var item in accountHolderPeople)
            {
                dataTable.Rows.Add(item.StakeholderId, item.Birthdate, item.SexTypeId, item.Description, item.Height);
            }

            return dataTable;
        }
    }
}