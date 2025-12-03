using System;
using System.Xml;
using Intent.Modules.VisualStudio.Projects.Events.ServiceFabric;

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric
{
    public static class Helpers
    {
        public static XmlElement CreateServiceElement(
            ServiceRegistrationRequiredBase @event,
            IXmlDocumentTemplate template)
        {
            var service = template.Document.CreateElement("Service", template.Namespace);
            service.SetAttribute("Name", @event.Name);
            service.SetAttribute("ServicePackageActivationMode", "ExclusiveProcess");
            service.AppendChild(template.Document.CreateWhitespace($"{Environment.NewLine}      "));

            switch (@event)
            {
                case StatefulServiceRegistrationRequired stateful:
                    {
                        var statefulService = template.Document.CreateElement("StatefulService", template.Namespace);
                        statefulService.SetAttribute("ServiceTypeName", @event.ServiceTypeName);
                        statefulService.SetAttribute("TargetReplicaSetSize", stateful.TargetReplicaSetSize);
                        template.AddParameterMaybe(stateful.TargetReplicaSetSize, stateful.TargetReplicaSetSizeParameterDefaultValue);

                        statefulService.SetAttribute("MinReplicaSetSize", stateful.MinReplicaSetSize);
                        template.AddParameterMaybe(stateful.MinReplicaSetSize, stateful.MinReplicaSetSizeParameterDefaultValue);

                        var partition = template.Document.CreateElement("UniformInt64Partition", template.Namespace);
                        partition.SetAttribute("PartitionCount", stateful.PartitionCount);
                        template.AddParameterMaybe(stateful.PartitionCount, stateful.PartitionCountParameterDefaultValue);

                        partition.SetAttribute("LowKey", "-9223372036854775808");
                        partition.SetAttribute("HighKey", "9223372036854775807");

                        statefulService.AppendChild(template.Document.CreateWhitespace($"{Environment.NewLine}        "));
                        statefulService.AppendChild(partition);
                        statefulService.AppendChild(template.Document.CreateWhitespace($"{Environment.NewLine}      "));

                        service.AppendChild(statefulService);
                    }
                    break;
                case StatelessServiceRegistrationRequired stateless:
                    {
                        var statelessService = template.Document.CreateElement("StatelessService", template.Namespace);
                        statelessService.SetAttribute("ServiceTypeName", @event.ServiceTypeName);
                        statelessService.SetAttribute("InstanceCount", stateless.InstanceCount);
                        template.AddParameterMaybe(stateless.InstanceCount, stateless.InstanceCountParameterDefaultValue);

                        var singlePartition = template.Document.CreateElement("SinglePartition", template.Namespace);

                        statelessService.AppendChild(template.Document.CreateWhitespace($"{Environment.NewLine}        "));
                        statelessService.AppendChild(singlePartition);
                        statelessService.AppendChild(template.Document.CreateWhitespace($"{Environment.NewLine}      "));

                        service.AppendChild(statelessService);
                    }
                    break;
            }

            return service;
        }

        public static void AddParameterMaybe(
            this IXmlDocumentTemplate template,
            string value,
            object defaultParameterValue)
        {
            if (value[0] != '[' ||
                value[^1] != ']')
            {
                return;
            }

            template.AddParameter(value[1..^1], defaultParameterValue);
        }

        private static void AddParameter(
            this IXmlDocumentTemplate template,
            string name,
            object defaultValue)
        {
            var parametersNode = template.Document.SelectSingleNode($"/f:{template.Document.DocumentElement!.LocalName}/f:Parameters", template.NamespaceManager);
            if (parametersNode == null)
            {
                parametersNode = template.Document.CreateElement("Parameters", template.Namespace);
                parametersNode.AppendChild(template.Document.CreateWhitespace($"{Environment.NewLine}  "));

                var firstChild = template.Document.DocumentElement!.FirstChild;
                template.Document.DocumentElement.InsertBefore(template.Document.CreateWhitespace($"{Environment.NewLine}  "), firstChild);
                template.Document.DocumentElement.InsertBefore(parametersNode, firstChild);
            }

            var parameter = template.Document.CreateElement("Parameter", template.Namespace);
            parameter.SetAttribute("Name", name);
            parameter.SetAttribute("DefaultValue", defaultValue?.ToString() ?? "");
            parametersNode.AppendChild(template.Document.CreateWhitespace("  "));
            parametersNode.AppendChild(parameter);
            parametersNode.AppendChild(template.Document.CreateWhitespace($"{Environment.NewLine}  "));
        }
    }
}
