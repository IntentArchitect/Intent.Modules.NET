namespace Intent.Modules.CosmosDB.Templates;

internal class ContainerSettings
{
    public string? Name { get; set; }
    public string? PartitionKey { get; set; }
    public ContainerThroughputType? ThroughputType { get; set; }
    public int? ManualThroughput { get; set; }
    public int? AutomaticThroughputMax { get; set; }
}