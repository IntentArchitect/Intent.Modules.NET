using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Eventing;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using Intent.Modules.VisualStudio.Projects.Sync;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Modules.VisualStudio.Projects.Templates.SqlServerDatabase.SqlProject.Content;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.SqlServerDatabase.SqlProject
{
    [IntentManaged(Mode.Ignore)]
    public partial class SqlProjectTemplate : IntentFileTemplateBase<SQLServerDatabaseProjectModel>, IVisualStudioProjectTemplate
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.VisualStudio.Projects.SqlServerDatabase.SqlProject";

        private string _fileContent;

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public SqlProjectTemplate(IOutputTarget outputTarget, SQLServerDatabaseProjectModel model) : base(TemplateId, outputTarget, model)
        {
        }

        public override void OnCreated()
        {
            base.OnCreated();
            ExecutionContext.EventDispatcher.Publish(new VisualStudioProjectCreatedEvent(this));
        }

        public string ProjectId => Model.Id;
        public string Name => Model.Name;
        public string FilePath => FileMetadata.GetFilePath();
        IVisualStudioProject IVisualStudioProjectTemplate.Project => Model;

        public string LoadContent()
        {
            var change = ExecutionContext.ChangeManager.FindChange(FilePath);
            if (change != null)
            {
                return change.Content;
            }

            if (_fileContent == null)
            {
                TryGetExistingFileContent(out _fileContent);
            }

            return _fileContent;
        }

        public void UpdateContent(string content, ISoftwareFactoryEventDispatcher sfEventDispatcher)
        {
            var targetContent = XDocument.Parse(content).ToString().ReplaceLineEndings();
            var existingContent = LoadContent().ReplaceLineEndings();

            if (existingContent == targetContent)
            {
                return;
            }

            var change = ExecutionContext.ChangeManager.FindChange(FilePath);
            if (change != null)
            {
                change.ChangeContent(content, content);
                return;
            }

            sfEventDispatcher.Publish(new SoftwareFactoryEvent(SoftwareFactoryEvents.OverwriteFileCommand, new Dictionary<string, string>
            {
                ["FullFileName"] = FilePath,
                ["Context"] = ToString(),
                ["Content"] = content
            }));
        }

        public IEnumerable<NuGetInstall> RequestedNugetPackageInstalls() => [];
        public IEnumerable<string> GetTargetFrameworks() => Model.TargetFrameworkVersion();

        public override string TransformText()
        {
            if (IsSdkStyle())
            {
                return SqlProjectSdkContent.Generate(
                    Model.Name,
                    Model.Id,
                    Model.GetSQLServerDatabaseProject().Version());
            }

            return SqlProjectFrameworkContent.Generate(Model.Name, Model.Id);
        }

        private bool IsSdkStyle()
        {
            var hasSqlServerDatabaseProject = Model.HasSQLServerDatabaseProject();
            var hasNETFrameworkSettings = Model.HasNETFrameworkSettings();

            if (!hasSqlServerDatabaseProject && !hasNETFrameworkSettings)
            {
                throw new ElementException(
                    Model.InternalElement,
                    $"SQL Server Database Project '{Model.Name}' must have either a 'SQL Server Database Project' or '.NET Framework Settings' stereotype applied.");
            }

            if (!hasSqlServerDatabaseProject)
                return false;

            return Model.GetSQLServerDatabaseProject().ProjectType().IsSDK();
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
            var hadExistingContent = TryGetExistingFileContent(out var existingFileContent);

            var content = hadExistingContent && IsMatchingStructure(existingFileContent)
                ? existingFileContent
                : TransformText();

            if (!IsSdkStyle())
            {
                var xml = new ProjectFileXml(content);
                ApplySqlCmdVariables(xml);
                content = xml.Document.ToStringUTF8();
            }

            var doc = XDocument.Parse(content);

            return !hadExistingContent || !XmlHelper.IsSemanticallyTheSame(existingFileContent, doc)
                ? doc.ToFormattedProjectString()
                : existingFileContent;
        }

        private bool IsMatchingStructure(string content)
        {
            var doc = XDocument.Parse(content);
            var isSdkContent = doc.Root?.Name.Namespace == XNamespace.None;
            return IsSdkStyle() == isSdkContent;
        }

        /// <summary>
        /// SQL projects should never have NuGet packages.
        /// </summary>
        public IEnumerable<INugetPackageInfo> RequestedNugetPackages() => [];

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