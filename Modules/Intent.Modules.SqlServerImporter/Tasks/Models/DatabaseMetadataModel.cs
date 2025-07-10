using System.Collections.Generic;

namespace Intent.Modules.SqlServerImporter.Tasks.Models;

public class DatabaseMetadataInputModel
{
    public string ConnectionString { get; set; } = null!;
}

public class DatabaseMetadataResultModel
{
    public Dictionary<string, string[]> Tables { get; set; }
    public Dictionary<string, string[]> Views { get; set; }
    public Dictionary<string, string[]> StoredProcedures { get; set; }
} 