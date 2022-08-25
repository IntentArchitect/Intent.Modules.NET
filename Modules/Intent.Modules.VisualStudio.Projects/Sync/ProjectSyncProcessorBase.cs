using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.SdkEvolutionHelpers;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    internal abstract class ProjectSyncProcessorBase
    {
        private readonly IVisualStudioProjectTemplate _template;
        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;
        private readonly IChanges _changes;
        private readonly bool _includeXmlDeclaration;

        protected ProjectSyncProcessorBase(
            IVisualStudioProjectTemplate template,
            ISoftwareFactoryEventDispatcher sfEventDispatcher,
            IChanges changes,
            bool includeXmlDeclaration)
        {
            _template = template;
            _sfEventDispatcher = sfEventDispatcher;
            _changes = changes;
            _includeXmlDeclaration = includeXmlDeclaration;
        }

        public void Process(List<SoftwareFactoryEvent> events)
        {
            var change = _changes.FindChange(_template.FilePath);

            var content = change?.Content;
            if (content == null &&
                !_template.TryGetExistingFileContent(out content))
            {
                throw new Exception($"Could not find content for {_template.FilePath}");
            }

            var xml = new ProjectFileXml(content);

            _template.OutputTarget.SyncProjectReferences(xml.Document);
            _template.OutputTarget.SyncFrameworkReferences(xml.Document);

            ProcessEvents(xml, events);

            var updatedContent = _includeXmlDeclaration
                ? xml.Document.ToStringUTF8()
                : xml.Document.ToString();
            if (XmlHelper.IsSemanticallyTheSame(content, updatedContent))
            {
                return;
            }

            Logging.Log.Debug($"Syncing changes to Project File {ProjectPath}");
            if (change == null)
            {
                _sfEventDispatcher.Publish(new SoftwareFactoryEvent(
                    eventIdentifier: SoftwareFactoryEvents.OverwriteFileCommand,
                    additionalInfo: new Dictionary<string, string>
                    {
                        ["FullFileName"] = ProjectPath,
                        ["Context"] = _template.ToString(),
                        ["Content"] = string.Empty
                    }));

                change = _changes.FindChange(ProjectPath);
            }

            change.ChangeContent(updatedContent);
        }

        protected string ProjectPath => _template.FilePath;

        protected virtual void ProcessEvents(
            ProjectFileXml xml,
            List<SoftwareFactoryEvent> events)
        {
            foreach (var @event in events)
            {
                switch (@event.EventIdentifier)
                {
                    case SoftwareFactoryEvents.FileAddedEvent:
                        AddProjectItem(
                            xml: xml,
                            path: @event.GetValue("Path"),
                            data: GetFileAddedDataP(@event.AdditionalInfo));
                        break;
                    case SoftwareFactoryEvents.FileRemovedEvent:
                        RemoveProjectItem(
                            xml: xml,
                            path: @event.GetValue("Path"));
                        break;
                    default:
                        Logging.Log.Warning($"{GetType()} not handling {@event.EventIdentifier}");
                        break;
                }
            }
        }

        protected string DetermineItemType(
            string path,
            Dictionary<string, string> additionalData,
            IReadOnlyCollection<(string FileExtension, string ItemType)> fallbacks)
        {
            if ((
                    additionalData.TryGetValue("ItemType", out var itemType) ||
                    additionalData.TryGetValue("BuildAction", out itemType) ||
                    additionalData.TryGetValue("Build Action", out itemType)
                 ) &&
                !string.IsNullOrWhiteSpace(itemType))
            {
                return itemType;
            }

            var extension = Path.GetExtension(path);
            foreach (var fallback in fallbacks)
            {
                if (fallback.FileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                {
                    return fallback.ItemType;
                }
            }

            return null;
        }

        private static XElement FindItemGroup(
            ProjectFileXml xml,
            string itemType)
        {
            var itemGroup = xml.Document.XPathSelectElements($"/ns:Project/ns:ItemGroup[ns:{itemType}]", xml.Namespaces);

            return itemGroup.FirstOrDefault();
        }

        private static XElement AddItemGroup(
            ProjectFileXml xml)
        {
            var newItemGroup = new XElement(XName.Get("ItemGroup", xml.Namespace.NamespaceName));

            var lastItemGroup = xml.Document.XPathSelectElements("/ns:Project/ns:ItemGroup", xml.Namespaces).LastOrDefault();
            if (lastItemGroup == null)
            {
                xml.ProjectElement.Add(newItemGroup);
                return newItemGroup;
            }

            if (!lastItemGroup.Elements().Any())
            {
                return lastItemGroup;
            }

            lastItemGroup.AddAfterSelf(newItemGroup);
            return newItemGroup;
        }

        private static XElement GetProjectItem(
            ProjectFileXml xml,
            string fileName)
        {
            var projectItem = xml.Document.XPathSelectElement($"/ns:Project/ns:ItemGroup/*[@Include='{fileName}']", xml.Namespaces);
            return projectItem;
        }

        protected void RemoveProjectItem(
            ProjectFileXml xml,
            string path)
        {
            var relativeFileName = GetRelativeFileName(path);

            var projectItem = GetProjectItem(xml, relativeFileName);
            if (projectItem == null)
            {
                return;
            }

            var container = projectItem.Parent;
            projectItem.Remove();

            if (container is { HasElements: false, HasAttributes: false })
            {
                container.Remove();
            }
        }

        protected string GetRelativeFileName(string path)
        {
            return NormalizePath(Path.GetRelativePath(Path.GetDirectoryName(ProjectPath)!, path));
        }

        protected virtual void AddProjectItem(
            ProjectFileXml xml,
            string path,
            FileAddedData data)
        {
            var relativeFileName = GetRelativeFileName(path);
            if (string.IsNullOrWhiteSpace(relativeFileName))
            {
                throw new Exception($"{nameof(relativeFileName)} is null");
            }

            var itemGroup = FindItemGroup(xml, data.ItemType) ?? AddItemGroup(xml);

            var itemElement = GetProjectItem(xml, relativeFileName);
            if (itemElement?.Attribute("IntentIgnore")?.Value.ToLower() == "true")
            {
                return;
            }

            if (itemElement == null)
            {
                itemElement = new XElement(
                    name: XName.Get(data.ItemType, xml.Namespace.NamespaceName),
                    content: new XAttribute("Include", relativeFileName));
                itemGroup.Add(itemElement);
            }

            if (itemElement.Name.LocalName != data.ItemType)
            {
                itemElement.Name = XName.Get(data.ItemType, itemElement.Name.NamespaceName);
            }

            foreach (var (name, value) in data.Elements)
            {
                var subElement = itemElement.Elements().SingleOrDefault(x => x.Name.LocalName == name);
                if (subElement == null)
                {
                    subElement = new XElement(XName.Get(name, xml.Namespace.NamespaceName), value);
                    itemElement.Add(subElement);
                }

                subElement.SetValue(value);
            }

            foreach (var element in itemElement.Elements())
            {
                if (!data.Elements.ContainsKey(element.Name.LocalName))
                {
                    element.Remove();
                }
            }
        }

        private static string NormalizePath(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            // MSBuild and solution files use backslashes regardless of the OS
            value = value.Replace("/", @"\");

            // Replace double occurrences of folder separators with single separator. IE, turn a path like Dev\\Folder to Dev\Folder
            while (value.Contains(@"\\"))
            {
                value = value.Replace(@"\\", @"\");
            }

            return value;
        }

        protected virtual FileAddedData GetFileAddedDataP(IDictionary<string, string> input)
        {
            return GetFileAddedData(input);
        }

        /// <remarks>
        /// Ultimately all sync processors should derive from this class, hence putting it here
        /// but as internal static for now. It can later be changed to private.
        /// </remarks>
        internal static FileAddedData GetFileAddedData(IDictionary<string, string> input)
        {
            var data = new FileAddedData();

            foreach (var (key, value) in input)
            {
                if (key.StartsWith(CustomMetadataKeys.ElementPrefix))
                {
                    data.Elements.Add(key[CustomMetadataKeys.ElementPrefix.Length..], value);
                }

                if (key.StartsWith(CustomMetadataKeys.AttributePrefix))
                {
                    data.Attributes.Add(key[CustomMetadataKeys.AttributePrefix.Length..], value);
                }

                if (key == CustomMetadataKeys.ItemType)
                {
                    data.ItemType = value;
                }

                if (key == CustomMetadataKeys.AlwaysGenerateProjectItem)
                {
                    data.AlwaysGenerateProjectItem = true.ToString().Equals(value, StringComparison.OrdinalIgnoreCase);
                }
            }

            ApplyLegacyCompatibility(data, input);

            // Fallback value
            data.ItemType ??= "Content";

            return data;
        }

        [FixFor_Version4("Remove this method and its uses")]
        private static void ApplyLegacyCompatibility(FileAddedData data, IDictionary<string, string> input)
        {
            foreach (var (key, value) in input)
            {
                switch (key)
                {
                    case "CopyToOutputDirectory":
                    case "CopyToPublishDirectory":
                        data.Elements.Add(key, value);
                        break;
                    case "Depends On":
                        // Automatically adding "DesignTime" and "AutoGen" just because "Depends
                        // On" was set is very wrong, but it's the way it used to work prior to
                        // 3.3.1 and module builder .tt files relied on this.
                        data.Elements.Add("DesignTime", "True");
                        data.Elements.Add("AutoGen", "True");
                        data.Elements.Add("DependentUpon", value);
                        break;
                    case "ItemType":
                    case "BuildAction":
                    case "Build Action":
                        data.ItemType ??= value;
                        break;
                    default:
                        continue;
                }
            }

            var path = input["Path"];
            switch (Path.GetExtension(path))
            {
                case ".cs":
                    data.ItemType ??= "Compile";
                    break;
                case ".tt":
                    data.ItemType ??= "None";
                    data.AlwaysGenerateProjectItem = true;
                    data.Elements.Add("Generator", "TextTemplatingFilePreprocessor");
                    data.Elements.Add("LastGenOutput", $"{Path.GetFileNameWithoutExtension(path)}.cs");
                    break;
            }
        }
    }
}
