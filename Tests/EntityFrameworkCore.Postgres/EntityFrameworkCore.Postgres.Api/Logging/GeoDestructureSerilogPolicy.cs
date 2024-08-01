using Intent.RoslynWeaver.Attributes;
using NetTopologySuite.Geometries;
using Serilog.Core;
using Serilog.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.NetTopologySuite.GeoDestructureSerilogPolicy", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Api.Logging
{
    /// <summary>
    /// NetTopologySuite.Geometries by default when serialized will cause circular referencing which results in infinite logging.
    /// This Destructure solves this problem by overriding the serializing behaviour to write out simpler and easy to read representations.
    /// </summary>
    public class GeoDestructureSerilogPolicy : IDestructuringPolicy
    {
        public bool TryDestructure(
            object value,
            ILogEventPropertyValueFactory propertyValueFactory,
            out LogEventPropertyValue? result)
        {
            result = value switch
            {
                Point point => new ScalarValue($"Point({point.X}, {point.Y})"),
                _ => null
            };
            return result is not null;
        }
    }
}