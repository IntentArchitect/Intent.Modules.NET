using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Constants;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    internal abstract class ProjectSyncProcessorBase
    {
        private readonly IXmlFileCache _xmlFileCache;
        private readonly IChanges _changeManager;
        private readonly string _relativeProjectPath;
        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;

        private Action<string, string> _commitChanges;
        private XDocument _doc;
        private XmlNamespaceManager _namespaces;
        private XNamespace _namespace;
        private XElement _projectElement;

        protected ProjectSyncProcessorBase(
            string relativeProjectPath,
            ISoftwareFactoryEventDispatcher sfEventDispatcher,
            IXmlFileCache xmlFileCache,
            IChanges changeManager)
        {
            if (string.IsNullOrWhiteSpace(relativeProjectPath))
            {
                throw new ArgumentNullException(nameof(relativeProjectPath));
            }

            _relativeProjectPath = relativeProjectPath;
            _sfEventDispatcher = sfEventDispatcher;
            _xmlFileCache = xmlFileCache;
            _changeManager = changeManager;
            _commitChanges = SendOverwriteFileCommand;
        }

        public void Process(List<SoftwareFactoryEvent> events)
        {
            LoadProjectFile();

            ProcessEvents(events);

            CommitChanges();
        }

        protected virtual void ProcessEvents(List<SoftwareFactoryEvent> events)
        {
            foreach (var @event in events)
            {
                var additionalData = @event.AdditionalInfo is Dictionary<string, string> dictionary
                    ? dictionary
                    : @event.AdditionalInfo.ToDictionary(
                        x => x.Key,
                        x => x.Value);

                switch (@event.EventIdentifier)
                {
                    case SoftwareFactoryEvents.FileAddedEvent:
                        AddProjectItem(
                            path: @event.GetValue("Path"),
                            additionalData: additionalData);
                        break;
                    case SoftwareFactoryEvents.FileRemovedEvent:
                        RemoveProjectItem(
                            path: @event.GetValue("Path"));
                        break;
                    default:
                        Logging.Log.Warning($"{GetType()} not handling {@event.EventIdentifier}");
                        break;
                }
            }
        }

        protected abstract void AddProjectItem(string path, Dictionary<string, string> additionalData);

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

        private void CommitChanges()
        {
            var filename = Path.GetFullPath(_relativeProjectPath);

            var normalizedExistingContent = File.Exists(filename)
                ? XDocument.Parse(File.ReadAllText(filename)).ToString()
                : string.Empty;

            var normalizedOutput = XDocument.Parse(_doc.ToString()).ToString();

            if (normalizedExistingContent == normalizedOutput)
            {
                return;
            }

            Logging.Log.Debug($"Syncing changes to Project File {filename}");
            _commitChanges(filename, $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}{normalizedOutput}");
        }

        private void SendOverwriteFileCommand(string fullFilePath, string fileContent)
        {
            var @event = new SoftwareFactoryEvent(
                SoftwareFactoryEvents.OverwriteFileCommand,
                new Dictionary<string, string>
                {
                    ["FullFileName"] = fullFilePath,
                    ["Content"] = fileContent,
                });

            _sfEventDispatcher.Publish(@event);
        }

        private void LoadProjectFile()
        {
            var change = _changeManager.FindChange(_relativeProjectPath);
            if (change == null)
            {
                _doc = _xmlFileCache.GetFile(_relativeProjectPath);
                if (_doc == null)
                {
                    throw new Exception($"Trying to sync project file, but unable to find file content. {_relativeProjectPath}");
                }
            }
            else
            {
                _doc = XDocument.Parse(change.Content, LoadOptions.PreserveWhitespace);
                _commitChanges = (_, content) => change.ChangeContent(content);
            }

            if (_doc.Root == null)
            {
                throw new Exception("_doc.Root is null");
            }

            _namespaces = new XmlNamespaceManager(new NameTable());
            _namespace = _doc.Root.GetDefaultNamespace();
            _namespaces.AddNamespace("ns", _namespace.NamespaceName);

            _projectElement = _doc.XPathSelectElement("/ns:Project", _namespaces);
        }

        private XElement FindItemGroup(string itemType)
        {
            var itemGroup = _doc.XPathSelectElements($"/ns:Project/ns:ItemGroup[ns:{itemType}]", _namespaces);

            return itemGroup.FirstOrDefault();
        }

        private XElement AddItemGroup()
        {
            var newItemGroup = new XElement(XName.Get("ItemGroup", _namespace.NamespaceName));

            var lastItemGroup = _doc.XPathSelectElements("/ns:Project/ns:ItemGroup", _namespaces).LastOrDefault();
            if (lastItemGroup == null)
            {
                _projectElement.Add(newItemGroup);
                return newItemGroup;
            }

            if (!lastItemGroup.Elements().Any())
            {
                return lastItemGroup;
            }

            lastItemGroup.AddAfterSelf(newItemGroup);
            return newItemGroup;
        }

        private XElement GetProjectItem(string fileName)
        {
            var projectItem = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/*[@Include='{fileName}']", _namespaces);
            return projectItem;
        }

        private void RemoveProjectItem(string path)
        {
            var relativeFileName = NormalizePath(Path.GetRelativePath(Path.GetDirectoryName(_relativeProjectPath), path));

            var projectItem = GetProjectItem(relativeFileName);
            projectItem?.Remove();
        }

        protected void AddProjectItem(string path, string itemType, Dictionary<string, string> children)
        {
            if (string.IsNullOrWhiteSpace(itemType))
            {
                throw new ArgumentOutOfRangeException(nameof(itemType), itemType, "Cannot be null, empty or whitespace");
            }

            var relativePath = NormalizePath(Path.GetRelativePath(Path.GetDirectoryName(_relativeProjectPath), path));

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new Exception("relativeFileName is null");
            }

            var itemGroup = FindItemGroup(itemType) ?? AddItemGroup();

            var projectItem = GetProjectItem(relativePath);
            if (projectItem?.Attribute("IntentIgnore")?.Value.ToLower() == "true")
            {
                return;
            }

            if (projectItem == null)
            {
                projectItem = new XElement(
                    name: XName.Get(itemType, _namespace.NamespaceName),
                    content: new XAttribute("Include", relativePath));
                itemGroup.Add(projectItem);
            }

            if (projectItem.Name.LocalName != itemType)
            {
                projectItem.Name = XName.Get(itemType, projectItem.Name.NamespaceName);
            }

            var toRemove = projectItem
                .Elements()
                .Where(x => !children.ContainsKey(x.Name.LocalName))
                .ToArray();

            foreach (var item in toRemove)
            {
                item.Remove();
            }

            foreach (var (name, value) in children)
            {
                var existing = projectItem.Elements().SingleOrDefault(x => x.Name.LocalName == name);
                if (existing == null)
                {
                    existing = new XElement(XName.Get(name, _namespace.NamespaceName), value);
                    projectItem.Add(existing);
                }

                if (existing.Value != value)
                {
                    existing.SetValue(value);
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
