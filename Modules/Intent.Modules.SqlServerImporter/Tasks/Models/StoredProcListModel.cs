using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.SqlServerImporter.Tasks.Models;

public class StoredProcListInputModel
{
    public string ConnectionString { get; set; } = null!;
}

public class StoredProcListResultModel
{
    public Dictionary<string, string[]> StoredProcs { get; set; }
}