using System.Collections.Generic;

namespace Intent.Modules.SqlServerImporter.Tasks.Models;

public class RepositoryImportModel
{
    public string ApplicationId { get; set; } = null!;
    public string DesignerId { get; set; } = null!;
    public string PackageId { get; set; } = null!;
    
    // SQL Extractor Contract BEGIN

    public string EntityNameConvention { get; set; } = null!;
    public string TableStereotype { get; set; } = null!;
    public List<string> TypesToExport { get; set; } = [];
    public List<string> SchemaFilter { get; set; } = [];
    
    public List<string> StoredProcNames { get; set; } = [];
    public string? StoredProcedureType { get; set; }
    
    public string? RepositoryElementId { get; set; }
    
    
    public string ConnectionString { get; set; } = null!;
    public string? PackageFileName { get; set; }
    
    // SQL Extractor Contract END
    
    public RepositorySettingPersistence SettingPersistence { get; set; } = RepositorySettingPersistence.None;
}

public enum RepositorySettingPersistence
{
    None,
    InheritDb,
    AllSanitisedConnectionString,
    AllWithoutConnectionString,
    All
}