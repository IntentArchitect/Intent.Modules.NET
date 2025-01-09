using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.CosmosDB.Api
{
    public static class DomainPackageModelStereotypeExtensions
    {
        public static Container GetContainer(this DomainPackageModel model)
        {
            var stereotype = model.GetStereotype(Container.DefinitionId);
            return stereotype != null ? new Container(stereotype) : null;
        }


        public static bool HasContainer(this DomainPackageModel model)
        {
            return model.HasStereotype(Container.DefinitionId);
        }

        public static bool TryGetContainer(this DomainPackageModel model, out Container stereotype)
        {
            if (!HasContainer(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Container(model.GetStereotype(Container.DefinitionId));
            return true;
        }

        public class Container
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "ef9b1772-18e1-44ad-b606-66406221c805";

            public Container(IStereotype stereotype)
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

            public ThroughputTypeOptions ThroughputType()
            {
                return new ThroughputTypeOptions(_stereotype.GetProperty<string>("Throughput Type"));
            }

            public int? MaximumThroughputRUs()
            {
                return _stereotype.GetProperty<int?>("Maximum Throughput (RUs)");
            }

            public int? ManualThroughputRUs()
            {
                return _stereotype.GetProperty<int?>("Manual Throughput (RUs)");
            }

            public class ThroughputTypeOptions
            {
                public readonly string Value;

                public ThroughputTypeOptions(string value)
                {
                    Value = value;
                }

                public ThroughputTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Autoscale":
                            return ThroughputTypeOptionsEnum.Autoscale;
                        case "Manual":
                            return ThroughputTypeOptionsEnum.Manual;
                        case "Serverless":
                            return ThroughputTypeOptionsEnum.Serverless;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsAutoscale()
                {
                    return Value == "Autoscale";
                }
                public bool IsManual()
                {
                    return Value == "Manual";
                }
                public bool IsServerless()
                {
                    return Value == "Serverless";
                }
            }

            public enum ThroughputTypeOptionsEnum
            {
                Autoscale,
                Manual,
                Serverless
            }

        }

    }
}