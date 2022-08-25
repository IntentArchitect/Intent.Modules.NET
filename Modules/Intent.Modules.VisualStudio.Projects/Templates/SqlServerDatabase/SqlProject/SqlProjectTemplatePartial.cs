using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Sync;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.SqlServerDatabase.SqlProject
{
    [IntentManaged(Mode.Ignore)]
    public partial class SqlProjectTemplate : VisualStudioProjectTemplateBase<SQLServerDatabaseProjectModel>
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

        protected override string ApplyAdditionalTransforms(string existingFileOrTransformTextContent)
        {
            var xml = new ProjectFileXml(existingFileOrTransformTextContent);

            ApplySqlCmdVariables(xml);

            return xml.Document.ToStringUTF8();
        }

        private void ApplySqlCmdVariables(ProjectFileXml xml)
        {
            var toRemove = xml.Document
                .XPathSelectElements("/ns:Project/ns:ItemGroup/ns:SqlCmdVariable[@IntentReference]", xml.Namespaces)
                .Where(x => Model.SQLCMDVariables.All(s => s.Id != x.Attribute("IntentReference")!.Value))
                .ToArray();
            foreach (var item in toRemove)
            {
                var container = item.Parent;
                item.Remove();

                if (container is { HasAttributes: false, HasElements: false })
                {
                    container.Remove();
                }
            }

            if (!Model.SQLCMDVariables.Any())
            {
                return;
            }

            var itemGroup = xml.Document
                .XPathSelectElements("/ns:Project/ns:ItemGroup[ns:SqlCmdVariable]", xml.Namespaces)
                .FirstOrDefault();
            itemGroup ??= CreateItemGroup(xml);

            var maxValue = xml.Document
                .XPathSelectElements("/ns:Project/ns:ItemGroup/ns:SqlCmdVariable/ns:Value", xml.Namespaces)
                .Select(x => int.TryParse(x.Value.Replace("$(SqlCmdVar__", string.Empty).Replace(")", string.Empty), out var parsed)
                    ? parsed
                    : 0)
                .DefaultIfEmpty(0)
                .Max();

            foreach (var sqlCmdVariable in Model.SQLCMDVariables)
            {
                var element =
                    xml.Document.XPathSelectElement($"/ns:Project/ns:ItemGroup/ns:SqlCmdVariable[@IntentReference='{sqlCmdVariable.Id}']", xml.Namespaces)
                    ??
                    xml.Document.XPathSelectElement($"/ns:Project/ns:ItemGroup/ns:SqlCmdVariable[@Include='{sqlCmdVariable.Name}']", xml.Namespaces);

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

        private static XElement CreateItemGroup(ProjectFileXml xml)
        {
            var newItemGroup = CreateElement("ItemGroup", xml.Namespace);

            var lastItemGroup = xml.Document.XPathSelectElements("/ns:Project/ns:ItemGroup", xml.Namespaces).LastOrDefault();
            if (lastItemGroup == null)
            {
                var projectElement = xml.Document.XPathSelectElement("/ns:Project", xml.Namespaces);
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
    }
}