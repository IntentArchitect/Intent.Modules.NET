using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    public class CoreProjectSyncProcessor
    {
        private readonly IXmlFileCache _xmlFileCache;
        private readonly IChanges _changeManager;
        private readonly string _projectPath;
        private readonly ISoftwareFactoryEventDispatcher _softwareFactoryEventDispatcher;
        private readonly IOutputTarget _project;

        private Action<string, string> _syncProjectFile;
        private XDocument _doc;
        private XmlNamespaceManager _namespaces;
        private XNamespace _namespace;
        private XElement _projectElement;

        public CoreProjectSyncProcessor(
            string projectPath,
            ISoftwareFactoryEventDispatcher softwareFactoryEventDispatcher,
            IXmlFileCache xmlFileCache,
            IChanges changeManager,
            IOutputTarget project)
        {
            _projectPath = projectPath;
            _softwareFactoryEventDispatcher = softwareFactoryEventDispatcher;
            _xmlFileCache = xmlFileCache;
            _changeManager = changeManager;
            _project = project;
            _syncProjectFile = UpdateFileOnHdd;
        }


        public void Process(List<SoftwareFactoryEvent> events)
        {
            var filename = LoadProjectFile();

            if (string.IsNullOrWhiteSpace(filename))
                return;

            filename = Path.GetFullPath(filename);

            _project.SyncProjectReferences(_doc);
            _project.SyncFrameworkReferences(_doc);

            //run events
            ProcessEvents(events);

            var currentProjectFileContent = "";
            if (File.Exists(filename))
            {
                var currentProjectFile = XDocument.Parse(File.ReadAllText(filename), LoadOptions.PreserveWhitespace);
                currentProjectFileContent = currentProjectFile.ToString();
            }
            var outputContent = _doc.ToString();

            // Perform a semantic comparision as VS does inconsistent formatting 
            if (currentProjectFileContent != outputContent)
            {
                Logging.Log.Debug($"Syncing changes to Project File {filename}");
                //string content = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + outputContent;
                _syncProjectFile(filename, outputContent);
            }
        }

        public void ProcessEvents(List<SoftwareFactoryEvent> events)
        {
            foreach (var @event in events)
            {
                switch (@event.EventIdentifier)
                {
                    case SoftwareFactoryEvents.FileAddedEvent:
                        {
                            var data = ProjectSyncProcessorBase.GetFileAddedData(@event.AdditionalInfo);
                            ProcessAddProjectItem(@event.GetValue("Path"), data);
                        }
                        break;
                    case CsProjectEvents.AddContentFile:
                        {
                            var data = ProjectSyncProcessorBase.GetFileAddedData(@event.AdditionalInfo);

                            data.ItemType = "Content";
                            data.Attributes.Add("Update", null);
                            data.Attributes.Add("Link", Path.Combine(Path.GetDirectoryName(_projectPath)!, @event.GetValue("Link")));
                            data.Attributes.Add("Include", @event.GetValue("Include"));

                            ProcessAddProjectItem(@event.GetValue("Path"), data);
                        }
                        break;
                    case SoftwareFactoryEvents.FileRemovedEvent:
                        ProcessRemoveProjectItem(
                            path: @event.GetValue("Path"));
                        break;
                    case CsProjectEvents.AddCompileDependsOn:
                        ProcessCompileDependsOn(
                            targetName: @event.GetValue("TargetName"));
                        break;
                    case CsProjectEvents.AddBeforeBuild:
                    case SoftwareFactoryEvents.AddTargetEvent:
                    case SoftwareFactoryEvents.AddTaskEvent:
                    case SoftwareFactoryEvents.ChangeProjectItemTypeEvent:
                    case CsProjectEvents.AddImport:
                        // NOP
                        break;
                    default:
                        Logging.Log.Warning($"Core VSProject Sync not handling {@event.EventIdentifier}");
                        break;
                }
            }
        }

        private void UpdateFileOnHdd(string filename, string outputContent)
        {
            var se = new SoftwareFactoryEvent(SoftwareFactoryEvents.OverwriteFileCommand, new Dictionary<string, string>
                                        {
                                            {"FullFileName", filename},
                                            {"Content", outputContent},
                                        });
            _softwareFactoryEventDispatcher.Publish(se);
        }

        private string LoadProjectFile()
        {
            if (string.IsNullOrWhiteSpace(_projectPath))
                return null;

            var change = _changeManager.FindChange(_projectPath);
            if (change == null)
            {
                _doc = _xmlFileCache.GetFile(_projectPath);
                if (_doc == null)
                {
                    throw new Exception($"Trying to sync project file, but unable to find csproj content. {_projectPath}");
                }
            }
            else
            {
                _doc = XDocument.Parse(change.Content, LoadOptions.PreserveWhitespace);
                _syncProjectFile = (_, c) => change.ChangeContent(c);
            }

            if (_doc.Root == null)
            {
                throw new Exception("_doc.Root is null");
            }

            _namespaces = new XmlNamespaceManager(new NameTable());
            _namespace = _doc.Root.GetDefaultNamespace();
            _namespaces.AddNamespace("ns", _namespace.NamespaceName);

            _projectElement = _doc.XPathSelectElement("/ns:Project", _namespaces);

            return _projectPath;
        }

        private XElement GetOrCreateItemGroupFor(string type)
        {
            var itemGroupElement = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup[ns:{type}]", _namespaces);
            if (itemGroupElement == null)
            {
                if (_doc.Root == null) throw new Exception("_doc.Root is null.");
                _doc.Root.Add(
                    "  ",
                    itemGroupElement = new XElement(XName.Get("ItemGroup", _namespace.NamespaceName)),
                    Environment.NewLine,
                    Environment.NewLine);
            }

            return itemGroupElement;
        }

        private void ProcessCompileDependsOn(string targetName)
        {
            /*
              <PropertyGroup>
                <CompileDependsOn>$(CompileDependsOn);GulpBuild;</CompileDependsOn>
              </PropertyGroup>
            */

            var element = _doc.XPathSelectElement($"/ns:Project/ns:PropertyGroup/ns:CompileDependsOn[contains(text(),'$(CompileDependsOn);') and contains(text(),'{targetName};')]", _namespaces);
            if (element == null)
            {
                _projectElement.Add(CreateElement(
                    name: "PropertyGroup",
                    subElements: new[]
                    {
                        CreateElement(
                            name: "CompileDependsOn",
                            value: $"$(CompileDependsOn);{targetName};")
                    }));
            }
        }

        private XElement GetFileItem(string fileName)
        {
            var projectItem = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/*[@Include='{fileName}' or @Update='{fileName}' or @Link='{fileName}']", _namespaces);

            return projectItem;
        }

        private XElement GetProjectItem(string fileName)
        {
            var projectItem = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/*[@Include='{fileName}']", _namespaces);
            return projectItem;
        }

        private void ProcessRemoveProjectItem(string path)
        {
            var relativeFileName = NormalizePath(Path.GetRelativePath(Path.GetDirectoryName(_projectPath)!, path));

            var projectItem = GetProjectItem(relativeFileName);
            projectItem?.Remove();
        }

        private void ProcessAddProjectItem(string path, FileAddedData data)
        {
            var relativeFileName = NormalizePath(Path.GetRelativePath(Path.GetDirectoryName(_projectPath)!, path));

            if (string.IsNullOrWhiteSpace(relativeFileName))
            {
                throw new Exception($"{nameof(relativeFileName)} is null");
            }

            var itemElement = GetFileItem(relativeFileName);
            if (itemElement?.Attribute("IntentIgnore")?.Value.Equals(true.ToString(), StringComparison.OrdinalIgnoreCase) == true)
            {
                return;
            }

            if (!data.AlwaysGenerateProjectItem &&
                data.Attributes.Count == 0 &&
                data.Elements.Count == 0)
            {
                itemElement?.Remove();
                return;
            }

            if (itemElement == null)
            {
                var targetElement = GetOrCreateItemGroupFor(data.ItemType);
                var content = new object[]
                {
                    $"{Environment.NewLine}    ",
                    itemElement = new XElement(XName.Get(data.ItemType, _namespace.NamespaceName))
                };

                var lastElement = targetElement.Elements().LastOrDefault();
                if (lastElement != null)
                {
                    lastElement.AddAfterSelf(content);
                }
                else
                {
                    targetElement.AddFirst(content);
                }
            }

            if (itemElement.Name.LocalName != data.ItemType)
            {
                itemElement.Name = XName.Get(data.ItemType, itemElement.Name.NamespaceName);
            }

            itemElement.SetAttributeValue(XName.Get("Update", _namespace.NamespaceName), relativeFileName);

            foreach (var (name, value) in data.Attributes)
            {
                itemElement.SetAttributeValue(XName.Get(name, _namespace.NamespaceName), value);
            }

            foreach (var (name, value) in data.Elements)
            {
                var subElement = itemElement.Elements().SingleOrDefault(x => x.Name == XName.Get(name, _namespace.NamespaceName));
                if (subElement == null)
                {
                    var content = new object[]
                    {
                        $"{Environment.NewLine}      ",
                        subElement = new XElement(XName.Get(name, _namespace.NamespaceName))
                    };

                    var lastElement = itemElement.Elements().LastOrDefault();
                    if (lastElement != null)
                    {
                        lastElement.AddAfterSelf(content);
                    }
                    else
                    {
                        itemElement.AddFirst(content);
                        itemElement.Add($"{Environment.NewLine}    ");
                    }
                }

                subElement.Value = value;
            }

            foreach (var element in itemElement.Elements())
            {
                if (!data.Elements.ContainsKey(element.Name.LocalName))
                {
                    element.Remove();
                }
            }
        }

        private XElement CreateElement(string name, string value = null, IEnumerable<XAttribute> attributes = null, IEnumerable<XElement> subElements = null)
        {
            attributes ??= Enumerable.Empty<XAttribute>();
            subElements ??= Enumerable.Empty<XElement>();

            var newElement = new XElement(XName.Get(name, _namespace.NamespaceName));
            if (value != null)
            {
                newElement.Value = value;
            }

            foreach (var attribute in attributes)
            {
                newElement.Add(attribute);
            }

            foreach (var element in subElements)
            {
                newElement.Add(element);
            }

            return newElement;
        }

        private static string NormalizePath(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            // .csproj and solution files use backslashes even on Mac
            value = value.Replace("/", @"\");

            // Replace double occurrences of folder separators with single separator. IE, turn a path like Dev\\Folder to Dev\Folder
            while (value.Contains(@"\\"))
                value = value.Replace(@"\\", @"\");

            return value;
        }

    }
}

