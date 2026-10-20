using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace ValueObjects.Record.Domain
{
    public record Waypoint(decimal Latitude, decimal Longitude);

}