namespace Intent.Modules.SqlServerImporter.Tasks.Models;

public class TestConnectionInputModel
{
    public string ConnectionString { get; set; }
}

public class TestConnectionResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}