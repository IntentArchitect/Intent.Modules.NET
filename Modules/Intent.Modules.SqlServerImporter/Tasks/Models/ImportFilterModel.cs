using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Intent.Modules.SqlServerImporter.Tasks.Models;

public class ImportFilterModel
{
    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = [];

    [JsonPropertyName("include_tables")]
    public List<FilterTableModel> IncludeTables { get; set; } = [];

    [JsonPropertyName("include_views")]
    public List<FilterViewModel> IncludeViews { get; set; } = [];

    [JsonPropertyName("include_stored_procedures")]
    public List<string> IncludeStoredProcedures { get; set; } = [];

    [JsonPropertyName("exclude_tables")]
    public List<string> ExcludeTables { get; set; } = [];

    [JsonPropertyName("exclude_views")]
    public List<string> ExcludeViews { get; set; } = [];

    [JsonPropertyName("exclude_stored_procedures")]
    public List<string> ExcludeStoredProcedures { get; set; } = [];
}

public class FilterTableModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("exclude_columns")]
    public List<string> ExcludeColumns { get; set; } = [];
}

public class FilterViewModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("exclude_columns")]
    public List<string> ExcludeColumns { get; set; } = [];
}

public class FilterLoadInputModel
{
    public string ImportFilterFilePath { get; set; } = null!;
    public string PackageId { get; set; }
    public string ApplicationId { get; set; }
}

public class FilterSaveInputModel
{
    public string ImportFilterFilePath { get; set; } = null!;
    public string PackageId { get; set; }
    public string ApplicationId { get; set; }
    public ImportFilterModel FilterData { get; set; } = null!;
}

public class PathResolutionInputModel
{
    public string SelectedFilePath { get; set; } = null!;
    public string PackageId { get; set; }
    public string ApplicationId { get; set; }
} 