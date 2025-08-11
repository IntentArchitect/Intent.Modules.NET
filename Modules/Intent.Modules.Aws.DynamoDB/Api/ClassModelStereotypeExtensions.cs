using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Aws.DynamoDB.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static Table GetTable(this ClassModel model)
        {
            var stereotype = model.GetStereotype(Table.DefinitionId);
            return stereotype != null ? new Table(stereotype) : null;
        }


        public static bool HasTable(this ClassModel model)
        {
            return model.HasStereotype(Table.DefinitionId);
        }

        public static bool TryGetTable(this ClassModel model, out Table stereotype)
        {
            if (!HasTable(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Table(model.GetStereotype(Table.DefinitionId));
            return true;
        }

        public class Table
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "6f32b3ab-384d-4696-8427-86156189346b";

            public Table(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

            public string PartitionKey()
            {
                return _stereotype.GetProperty<string>("Partition Key");
            }

            public ThroughputModeOptions ThroughputMode()
            {
                return new ThroughputModeOptions(_stereotype.GetProperty<string>("Throughput Mode"));
            }

            public int? MaximumReadThroughputUnits()
            {
                return _stereotype.GetProperty<int?>("Maximum Read Throughput (Units)");
            }

            public int? MaximumWriteThroughputUnits()
            {
                return _stereotype.GetProperty<int?>("Maximum Write Throughput (Units)");
            }

            public int? ReadThroughputUnits()
            {
                return _stereotype.GetProperty<int?>("Read Throughput (Units)");
            }

            public int? WriteThroughputUnits()
            {
                return _stereotype.GetProperty<int?>("Write Throughput (Units)");
            }

            public class ThroughputModeOptions
            {
                public readonly string Value;

                public ThroughputModeOptions(string value)
                {
                    Value = value;
                }

                public ThroughputModeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "On-demand":
                            return ThroughputModeOptionsEnum.OnDemand;
                        case "Provisioned":
                            return ThroughputModeOptionsEnum.Provisioned;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsOnDemand()
                {
                    return Value == "On-demand";
                }
                public bool IsProvisioned()
                {
                    return Value == "Provisioned";
                }
            }

            public enum ThroughputModeOptionsEnum
            {
                OnDemand,
                Provisioned
            }
        }

    }
}