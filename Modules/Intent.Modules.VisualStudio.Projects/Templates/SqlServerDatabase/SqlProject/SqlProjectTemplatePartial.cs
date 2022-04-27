using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Microsoft.Build.Evaluation;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.SqlServerDatabase.SqlProject
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class SqlProjectTemplate : IntentTemplateBase<SQLServerDatabaseProjectModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.VisualStudio.Projects.SqlServerDatabase.SqlProject";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public SqlProjectTemplate(IOutputTarget outputTarget, SQLServerDatabaseProjectModel model) : base(TemplateId, outputTarget, model)
        {
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"{Model.Name}",
                fileExtension: "sqlproj"
            );
        }

        public override string RunTemplate()
        {
            var content = File.Exists(GetExistingFilePath())
                ? File.ReadAllText(GetExistingFilePath())
                : base.RunTemplate();

            var doc = XDocument.Parse(content, LoadOptions.PreserveWhitespace);
            var namespaces = new XmlNamespaceManager(new NameTable());
            var @namespace = doc.Root.GetDefaultNamespace();
            namespaces.AddNamespace("ns", @namespace.NamespaceName);

            ApplySqlCmdVariables(doc, namespaces, @namespace);

            var updated = doc.ToString();
            return IsSemanticallyTheSame(content, updated)
                ? content
                : $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}{updated}";
        }

        private void ApplySqlCmdVariables(XDocument doc, IXmlNamespaceResolver namespaces, XNamespace @namespace)
        {
            var toRemove = doc
                .XPathSelectElements("/ns:Project/ns:ItemGroup/ns:SqlCmdVariable[@IntentReference]", namespaces)
                .Where(x => Model.SQLCMDVariables.All(s => s.Id != x.Attribute("IntentReference")!.Value))
                .ToArray();
            foreach (var item in toRemove)
            {
                item.Remove();
            }

            var itemGroup = doc
                .XPathSelectElements("/ns:Project/ns:ItemGroup[ns:SqlCmdVariable]", namespaces)
                .FirstOrDefault();
            itemGroup ??= CreateItemGroup(doc, namespaces, @namespace);

            var maxValue = doc
                .XPathSelectElements("/ns:Project/ns:ItemGroup/ns:SqlCmdVariable/ns:Value", namespaces)
                .Select(x => int.TryParse(x.Value.Replace("$(SqlCmdVar__", string.Empty).Replace(")", string.Empty), out var parsed)
                    ? parsed
                    : 0)
                .DefaultIfEmpty(0)
                .Max();

            foreach (var sqlCmdVariable in Model.SQLCMDVariables)
            {
                var element =
                    doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/ns:SqlCmdVariable[@IntentReference='{sqlCmdVariable.Id}']", namespaces)
                    ??
                    doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/ns:SqlCmdVariable[@Include='{sqlCmdVariable.Name}']", namespaces);

                if (element == null)
                {
                    element = XElement.Parse(@$"<ItemGroup xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
    <SqlCmdVariable Include="""" IntentReference="""">
      <DefaultValue></DefaultValue>
      <Value>$(SqlCmdVar__{++maxValue:D})</Value>
    </SqlCmdVariable>
  </ItemGroup>", LoadOptions.PreserveWhitespace).Elements().Single();
                    element.Remove();
                    itemGroup.Add(element);
                }

                element.SetAttributeValue("Include", sqlCmdVariable.Name);
                element.SetAttributeValue("IntentReference", sqlCmdVariable.Id);
                element.Nodes().OfType<XElement>().Single(x => x.Name.LocalName == "DefaultValue").Value = sqlCmdVariable.Value;
            }
        }

        private static XElement CreateItemGroup(XDocument doc, IXmlNamespaceResolver namespaces, XNamespace @namespace)
        {
            var newItemGroup = CreateElement("ItemGroup", @namespace);

            var lastItemGroup = doc.XPathSelectElements("/ns:Project/ns:ItemGroup", namespaces).LastOrDefault();
            if (lastItemGroup == null)
            {
                var projectElement = doc.XPathSelectElement("/ns:Project", namespaces);
                projectElement.Add(newItemGroup);
                return newItemGroup;
            }

            if (!lastItemGroup.Elements().Any())
            {
                return lastItemGroup;
            }

            lastItemGroup.AddAfterSelf(newItemGroup);
            return newItemGroup;
        }

        private static XElement CreateElement(string name, XNamespace @namespace, object value = null)
        {
            return new XElement(XName.Get(name, @namespace.NamespaceName), value);
        }

        private static bool IsSemanticallyTheSame(string original, string updated)
        {
            return XDocument.Parse(original).ToString() == XDocument.Parse(updated).ToString();
        }

        public override void OnCreated()
        {
            base.OnCreated();
            ExecutionContext.EventDispatcher.Publish(new VisualStudioSolutionProjectCreatedEvent(OutputTarget.Id, GetMetadata().GetFilePath()));
        }
    }
}