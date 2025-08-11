namespace Intent.Modules.Aws.DynamoDB.Templates;

internal class TableSettings
{
    public string? Name { get; set; }
    public string? PartitionKey { get; set; }
    public TableThroughputMode? ThroughputMode { get; set; }
    public int? MaximumReadThroughput { get; set; }
    public int? MaximumWriteThroughput { get; set; }
    public int? ReadThroughput { get; set; }
    public int? WriteThroughput { get; set; }
}