using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IVisualStudioProjectTemplate _template;
        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;
        private readonly IChanges _changes;

        public CoreProjectSyncProcessor(
            IVisualStudioProjectTemplate template,
            ISoftwareFactoryEventDispatcher sfEventDispatcher,
            IChanges changes)
        {
            _template = template;
            _sfEventDispatcher = sfEventDispatcher;
            _changes = changes;
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

            ProcessEvents(xml,events);

            var updatedContent = xml.Document.ToString();
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

        private string ProjectPath => _template.FilePath;

        private void ProcessEvents(
            ProjectFileXml xml,
            List<SoftwareFactoryEvent> events)
        {
            foreach (var @event in events)
            {
                switch (@event.EventIdentifier)
                {
                    case SoftwareFactoryEvents.FileAddedEvent:
                        {
                            var data = ProjectSyncProcessorBase.GetFileAddedData(@event.AdditionalInfo);
                            ProcessAddProjectItem(xml, @event.GetValue("Path"), data);
                        }
                        break;
                    case CsProjectEvents.AddContentFile:
                        {
                            var data = ProjectSyncProcessorBase.GetFileAddedData(@event.AdditionalInfo);

                            data.ItemType = "Content";
                            data.Attributes.Add("Update", null);
                            data.Attributes.Add("Link", Path.Combine(Path.GetDirectoryName(ProjectPath)!, @event.GetValue("Link")));
                            data.Attributes.Add("Include", @event.GetValue("Include"));

                            ProcessAddProjectItem(xml, @event.GetValue("Path"), data);
                        }
                        break;
                    case SoftwareFactoryEvents.FileRemovedEvent:
                        ProcessRemoveProjectItem(xml, @event.GetValue("Path"));
                        break;
                    case CsProjectEvents.AddCompileDependsOn:
                        ProcessCompileDependsOn(xml, @event.GetValue("TargetName"));
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

        private static XElement GetOrCreateItemGroupFor(
            ProjectFileXml xml,
            string type)
        {
            var itemGroupElement = xml.Document.XPathSelectElement($"/ns:Project/ns:ItemGroup[ns:{type}]", xml.Namespaces);
            if (itemGroupElement != null)
            {
                return itemGroupElement;
            }

            xml.Document.Root!.Add(
                "  ",
                itemGroupElement = new XElement(XName.Get("ItemGroup", xml.Namespace.NamespaceName)),
                Environment.NewLine,
                Environment.NewLine);

            return itemGroupElement;
        }

        private void ProcessCompileDependsOn(
            ProjectFileXml xml,
            string targetName)
        {
            /*
              <PropertyGroup>
                <CompileDependsOn>$(CompileDependsOn);GulpBuild;</CompileDependsOn>
              </PropertyGroup>
            */

            var element = xml.Document.XPathSelectElement($"/ns:Project/ns:PropertyGroup/ns:CompileDependsOn[contains(text(),'$(CompileDependsOn);') and contains(text(),'{targetName};')]", xml.Namespaces);
            if (element == null)
            {
                xml.ProjectElement.Add(CreateElement(
                    xml: xml,
                    name: "PropertyGroup",
                    subElements: new[]
                    {
                        CreateElement(
                            xml: xml,
                            name: "CompileDependsOn",
                            value: $"$(CompileDependsOn);{targetName};")
                    }));
            }
        }

        private static XElement GetFileItem(
            ProjectFileXml xml,
            string fileName)
        {
            var projectItem = xml.Document.XPathSelectElement($"/ns:Project/ns:ItemGroup/*[@Include='{fileName}' or @Update='{fileName}' or @Link='{fileName}']", xml.Namespaces);

            return projectItem;
        }

        private void ProcessRemoveProjectItem(
            ProjectFileXml xml,
            string path)
        {
            var relativeFileName = NormalizePath(Path.GetRelativePath(Path.GetDirectoryName(ProjectPath)!, path));

            var projectItem = GetFileItem(xml, relativeFileName);
            projectItem?.Remove();
        }

        private void ProcessAddProjectItem(
            ProjectFileXml xml,
            string itemPath,
            FileAddedData data)
        {
            var relativeFileName = NormalizePath(Path.GetRelativePath(Path.GetDirectoryName(ProjectPath)!, itemPath));

            if (string.IsNullOrWhiteSpace(relativeFileName))
            {
                throw new Exception($"{nameof(relativeFileName)} is null");
            }

            var itemElement = GetFileItem(xml, relativeFileName);
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
                var targetElement = GetOrCreateItemGroupFor(xml, data.ItemType);
                var content = new object[]
                {
                    $"{Environment.NewLine}    ",
                    itemElement = new XElement(XName.Get(data.ItemType, xml.Namespace.NamespaceName))
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

            itemElement.SetAttributeValue(XName.Get("Update", xml.Namespace.NamespaceName), relativeFileName);

            foreach (var (name, value) in data.Attributes)
            {
                itemElement.SetAttributeValue(XName.Get(name, xml.Namespace.NamespaceName), value);
            }

            foreach (var (name, value) in data.Elements)
            {
                var subElement = itemElement.Elements().SingleOrDefault(x => x.Name == XName.Get(name, xml.Namespace.NamespaceName));
                if (subElement == null)
                {
                    var content = new object[]
                    {
                        $"{Environment.NewLine}      ",
                        subElement = new XElement(XName.Get(name, xml.Namespace.NamespaceName))
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

        private static XElement CreateElement(
            ProjectFileXml xml,
            string name,
            string value = null,
            IEnumerable<XAttribute> attributes = null,
            IEnumerable<XElement> subElements = null)
        {
            attributes ??= Enumerable.Empty<XAttribute>();
            subElements ??= Enumerable.Empty<XElement>();

            var newElement = new XElement(XName.Get(name, xml.Namespace.NamespaceName));
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
