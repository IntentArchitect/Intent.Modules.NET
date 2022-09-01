using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    internal class FrameworkProjectSyncProcessor
    {
        private readonly IVisualStudioProjectTemplate _template;
        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;
        private readonly IChanges _changes;

        public FrameworkProjectSyncProcessor(
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

            SyncAssemblyReferences(xml);
            SyncProjectReferences(xml);

            ProcessEvents(xml, events);

            var updatedContent = xml.Document.ToStringUTF8();
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

        public void ProcessEvents(
            ProjectFileXml xml,
            List<SoftwareFactoryEvent> events)
        {
            foreach (var @event in events)
            {
                switch (@event.EventIdentifier)
                {
                    case SoftwareFactoryEvents.FileAddedEvent:
                        ProcessAddProjectItem(
                            xml: xml,
                            path: @event.GetValue("Path"),
                            itemType: @event.TryGetValue("ItemType"),
                            dependsOn: @event.TryGetValue("Depends On"),
                            copyToOutputDirectory: @event.TryGetValue("CopyToOutputDirectory"));
                        break;
                    case SoftwareFactoryEvents.FileRemovedEvent:
                        ProcessRemoveProjectItem(
                            xml: xml,
                            path: @event.GetValue("Path"));
                        break;
                    case SoftwareFactoryEvents.AddTargetEvent:
                        ProcessTargetElement(
                            xml: xml,
                            name: @event.GetValue("Name"),
                            inputXml: @event.GetValue("Xml"));
                        break;
                    case SoftwareFactoryEvents.AddTaskEvent:
                        ProcessUsingTask(
                            xml: xml,
                            taskName: @event.GetValue("TaskName"),
                            assemblyFile: @event.GetValue("AssemblyFile"));
                        break;
                    case SoftwareFactoryEvents.ChangeProjectItemTypeEvent:
                        ProcessChangeProjectItemType(
                            xml: xml,
                            relativeFileName: @event.GetValue("RelativeFileName"),
                            itemType: @event.GetValue("ItemType"));
                        break;
                    case CsProjectEvents.AddCompileDependsOn:
                        ProcessCompileDependsOn(
                            xml: xml,
                            targetName: @event.GetValue("TargetName"));
                        break;
                    case CsProjectEvents.AddImport:
                        ProcessImport(
                            xml: xml,
                            project: @event.GetValue("Project"),
                            condition: @event.GetValue("Condition"));
                        break;
                    case CsProjectEvents.AddBeforeBuild:
                        ProcessBeforeBuild(
                            xml: xml,
                            inputXml: @event.GetValue("Xml"));
                        break;
                    case CsProjectEvents.AddContentFile:
                        // This and SoftwareFactoryEvents.AddProjectItemEvent can potentially be merged, this one is just
                        // a more explicit version, the other one tends to "work it out".
                        ProcessContent(
                            xml: xml,
                            include: @event.GetValue("Include"),
                            link: @event.GetValue("Link"),
                            copyToOutputDirectory: @event.GetValue("CopyToOutputDirectory"));
                        break;
                    default:
                        Logging.Log.Warning($"VSProject Sync not handling {@event.EventIdentifier}");
                        break;
                }
            }
        }

        private void SyncProjectReferences(ProjectFileXml xml)
        {
            if (_template.OutputTarget.Dependencies().Count <= 0)
            {
                return;
            }

            var itemGroupElement = FindOrCreateProjectReferenceItemGroup(xml);

            foreach (var dependency in _template.OutputTarget.Dependencies())
            {
                var projectUrl = string.Format("..\\{0}\\{0}.csproj", dependency.Name);
                var projectReferenceItem = xml.Document.XPathSelectElement($"/ns:Project/ns:ItemGroup/ns:ProjectReference[@Include='{projectUrl}']", xml.Namespaces);
                if (projectReferenceItem != null)
                {
                    continue;
                }

                /*
                <ProjectReference Include="..\Intent.SoftwareFactory\Intent.SoftwareFactory.csproj"/>
                */

                var item = new XElement(XName.Get("ProjectReference", xml.Namespace.NamespaceName));
                item.Add(new XAttribute("Include", projectUrl));

                var projectIdElement = new XElement(XName.Get("Project", xml.Namespace.NamespaceName))
                {
                    Value = $"{{{dependency.Id}}}"
                };
                item.Add(projectIdElement);

                var projectNameElement = new XElement(XName.Get("Name", xml.Namespace.NamespaceName))
                {
                    Value = $"{dependency.Name}"
                };
                item.Add(projectNameElement);

                itemGroupElement.Add(item);
            }
        }

        private void SyncAssemblyReferences(ProjectFileXml xml)
        {
            if (_template.OutputTarget.References().Count == 0)
            {
                return;
            }

            var aReferenceElement = xml.Document.XPathSelectElement("/ns:Project/ns:ItemGroup/ns:Reference", xml.Namespaces);
            if (aReferenceElement == null)
            {
                throw new Exception("aReferenceElement is null");
            }

            var itemGroupElement = aReferenceElement.Parent;
            if (itemGroupElement == null)
            {
                throw new Exception("itemGroupElement is null");
            }
            foreach (var reference in _template.OutputTarget.References())
            {
                var projectReferenceItem = xml.Document.XPathSelectElement($"/ns:Project/ns:ItemGroup/ns:Reference[@Include='{reference.Library}']", xml.Namespaces);
                if (projectReferenceItem == null)
                {
                    /*
    <Reference Include="Microsoft.Build">
      <HintPath>..\..\..\..\Program Files (x86)\Reference Assemblies\Microsoft\MSBuild\v14.0\Microsoft.Build.dll</HintPath>
    </Reference>
    <Reference Include="Microsoft.CodeAnalysis, Version=1.3.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
      <HintPath>..\packages\Microsoft.CodeAnalysis.Common.1.3.2\lib\net45\Microsoft.CodeAnalysis.dll</HintPath>
      <Private>True</Private>
    </Reference>                   
                    */
                    var item = new XElement(XName.Get("Reference", xml.Namespace.NamespaceName));
                    item.Add(new XAttribute("Include", reference.Library));
                    if (reference.HasHintPath())
                    {
                        var projectIdElement = new XElement(XName.Get("HintPath", xml.Namespace.NamespaceName))
                        {
                            Value = reference.HintPath
                        };
                        item.Add(projectIdElement);
                    }

                    itemGroupElement.Add(item);
                }
            }
        }

        private static XElement FindOrCreateProjectReferenceItemGroup(ProjectFileXml xml)
        {
            var aProjectReferenceElement = xml.Document.XPathSelectElement("/ns:Project/ns:ItemGroup/ns:ProjectReference", xml.Namespaces);

            if (aProjectReferenceElement != null)
            {
                return aProjectReferenceElement.Parent;
            }

            var result = new XElement(XName.Get("ItemGroup", xml.Namespace.NamespaceName));
            xml.ProjectElement.Add(result);
            return result;
        }

        private static XElement FindItemGroupForCodeFiles(ProjectFileXml xml)
        {
            var codeItems = xml.Document.XPathSelectElement("/ns:Project/ns:ItemGroup[ns:Compile or ns:Content or ns:None]", xml.Namespaces);

            return codeItems;
        }

        private static XElement AddItemGroupForCodeFiles(ProjectFileXml xml)
        {
            var codeItems = new XElement(XName.Get("ItemGroup", xml.Namespace.NamespaceName));

            var lastItemGroup = xml.Document.XPathSelectElements("/ns:Project/ns:ItemGroup", xml.Namespaces).LastOrDefault();

            if (lastItemGroup == null)
            {
                xml.ProjectElement.Add(codeItems);
            }
            else
            {
                if (!lastItemGroup.Elements().Any())
                {
                    codeItems = lastItemGroup;
                }
                else
                {
                    lastItemGroup.AddAfterSelf(codeItems);
                }
            }

            return codeItems;
        }

        private static void ProcessCompileDependsOn(
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

        private static void ProcessImport(
            ProjectFileXml xml,
            string project,
            string condition)
        {
            project = NormalizePath(project);

            var element = xml.Document.XPathSelectElement($"/ns:Project/ns:Import[@Project=\"{project}\"]", xml.Namespaces);
            if (element == null)
            {
                xml.ProjectElement.Add(element = CreateElement(
                    xml: xml,
                    name: "Import",
                    attributes: new[]
                    {
                        new XAttribute("Project", project),
                        new XAttribute("Condition", condition),
                    }));
            }

            SetOrClearAttribute(attributeName: "Condition", value: condition, xElement: element);
        }

        private static void ProcessUsingTask(
            ProjectFileXml xml,
            string taskName,
            string assemblyFile)
        {
            assemblyFile = NormalizePath(assemblyFile);

            var element = xml.Document.XPathSelectElement($"/ns:Project/ns:UsingTask[@TaskName=\"{taskName}\"]", xml.Namespaces);
            if (element == null)
            {
                xml.ProjectElement.Add(element = CreateElement(
                    xml: xml,
                    name: "UsingTask",
                    attributes: new[]
                    {
                        new XAttribute("TaskName", taskName),
                        new XAttribute("AssemblyFile", assemblyFile),
                    }));
            }

            SetOrClearAttribute(attributeName: "AssemblyFile", value: assemblyFile, xElement: element);
        }

        private static XElement GetProjectItem(
            ProjectFileXml xml,
            string fileName)
        {
            var projectItem = xml.Document.XPathSelectElement($"/ns:Project/ns:ItemGroup/*[@Include='{fileName}']", xml.Namespaces);
            return projectItem;
        }

        private void ProcessRemoveProjectItem(
            ProjectFileXml xml,
            string path)
        {
            var relativeFileName = NormalizePath(Path.GetRelativePath(Path.GetDirectoryName(ProjectPath)!, path));

            var projectItem = GetProjectItem(xml, relativeFileName);
            projectItem?.Remove();
        }

        private void ProcessAddProjectItem(
            ProjectFileXml xml,
            string path,
            string itemType,
            string dependsOn,
            string copyToOutputDirectory)
        {
            var relativePath = NormalizePath(Path.GetRelativePath(Path.GetDirectoryName(ProjectPath)!, path));
            dependsOn = NormalizePath(dependsOn);

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new Exception("relativeFileName is null");
            }

            if (Path.GetExtension(relativePath).Equals(".config", StringComparison.InvariantCultureIgnoreCase))
            {
                copyToOutputDirectory = "PreserveNewest";
            }

            if (itemType == null)
            {
                var fileExtension = !string.IsNullOrEmpty(Path.GetExtension(relativePath)) ? Path.GetExtension(relativePath).Substring(1) : null; //remove the '.'
                switch (fileExtension)
                {
                    case "cs":
                        itemType = "Compile";
                        break;
                    case "csproj":
                        return;
                    default:
                        itemType = "Content";
                        break;
                }
            }

            var metadata = new Dictionary<string, string>();
            if (copyToOutputDirectory != null)
                metadata.Add("CopyToOutputDirectory", copyToOutputDirectory);
            if (dependsOn != null)
                metadata.Add("DependentUpon", dependsOn);

            // GCB - not the most extensible / flexible way of doing this...
            if (Path.GetExtension(relativePath).Equals(".tt", StringComparison.InvariantCultureIgnoreCase))
            {
                metadata.Add("Generator", "TextTemplatingFilePreprocessor");
                metadata.Add("LastGenOutput", Path.GetFileNameWithoutExtension(relativePath) + ".cs");
            }

            var codeItems = FindItemGroupForCodeFiles(xml) ?? AddItemGroupForCodeFiles(xml);

            var projectItem = GetProjectItem(xml, relativePath);
            if (projectItem == null)
            {
                var item = new XElement(XName.Get(itemType, xml.Namespace.NamespaceName));
                item.Add(new XAttribute("Include", relativePath));
                foreach (var data in metadata)
                {
                    var child = new XElement(XName.Get(data.Key, xml.Namespace.NamespaceName))
                    {
                        Value = data.Value
                    };
                    item.Add(child);
                }
                codeItems.Add(item);
            }
            else if (projectItem.Attribute("IntentIgnore")?.Value.ToLower() != "true")
            {
                if (projectItem.Name.LocalName != itemType)
                {
                    projectItem.Name = XName.Get(itemType, projectItem.Name.NamespaceName);
                }
                var children = projectItem.Elements().ToList();
                projectItem.RemoveNodes();
                foreach (var data in metadata)
                {
                    var child = new XElement(XName.Get(data.Key, xml.Namespace.NamespaceName)) { Value = data.Value };
                    projectItem.Add(child);
                }
                foreach (var userAddedMetadata in children.Where(x => metadata.All(y => XName.Get(y.Key, xml.Namespace.NamespaceName) != x.Name)))
                {
                    var child = new XElement(userAddedMetadata.Name) { Value = userAddedMetadata.Value };
                    projectItem.Add(child);
                }
            }
        }

        private static void ProcessBeforeBuild(
            ProjectFileXml xml,
            string inputXml)
        {
            var parsedXml = GetParsedXml(xml, inputXml);

            var beforeBuildElement = xml.Document.XPathSelectElement("/ns:Project/ns:Target[@Name=\"BeforeBuild\"]", xml.Namespaces);
            if (beforeBuildElement == null)
            {
                xml.ProjectElement.Add(beforeBuildElement = CreateElement(
                    xml: xml,
                    name: "Target",
                    attributes: new[]
                    {
                        new XAttribute("Name", "BeforeBuild"),
                    }));
            }

            if (beforeBuildElement.Elements().All(x => x.ToString() != parsedXml.ToString()))
            {
                beforeBuildElement.Add(parsedXml);
            }
        }

        private static void ProcessContent(
            ProjectFileXml xml,
            string include,
            string link,
            string copyToOutputDirectory)
        {
            include = NormalizePath(include);
            link = NormalizePath(link);

            var subElements = new List<XElement>();
            if (!string.IsNullOrWhiteSpace(link))
            {
                subElements.Add(CreateElement(
                    xml: xml,
                    name: "Link",
                    value: link));
            }

            if (!string.IsNullOrWhiteSpace(copyToOutputDirectory))
            {
                subElements.Add(CreateElement(
                    xml: xml,
                    name: "CopyToOutputDirectory",
                    value: copyToOutputDirectory));
            }

            var desiredElement = CreateElement(
                xml: xml,
                name: "Content",
                attributes: new[]
                {
                    new XAttribute("Include", include),
                },
                subElements: subElements);

            var element = GetProjectItem(xml, include);
            if (element == null)
            {
                var codeItems = FindItemGroupForCodeFiles(xml) ?? AddItemGroupForCodeFiles(xml);
                codeItems.Add(desiredElement);
                return;
            }

            ReplaceElementIfNotMatch(element, desiredElement);
        }

        private static void ProcessTargetElement(
            ProjectFileXml xml,
            string name,
            string inputXml)
        {
            var desiredContent = GetParsedXml(xml, inputXml);

            var targetElement = xml.Document.XPathSelectElement($"/ns:Project/ns:Target[@Name=\"{name}\"]", xml.Namespaces);
            if (targetElement == null)
            {
                xml.ProjectElement.Add(desiredContent);
                return;
            }

            ReplaceElementIfNotMatch(targetElement, desiredContent);
        }

        private void ProcessChangeProjectItemType(
            ProjectFileXml xml,
            string relativeFileName,
            string itemType)
        {
            relativeFileName = NormalizePath(relativeFileName);

            var projectItem = GetProjectItem(xml, relativeFileName);
            if (projectItem == null)
            {
                //WTF
                throw new Exception($"Cant from config file {relativeFileName} in project file {ProjectPath}");
            }

            //Make sure it's None not Content
            if (projectItem.Name.LocalName != itemType)
            {
                projectItem.Name = XName.Get(itemType, projectItem.Name.NamespaceName);
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

        private static XElement GetParsedXml(
            ProjectFileXml xml,
            string inputXml)
        {
            var parsedXml = XElement.Parse(inputXml);
            foreach (var xElement in parsedXml.DescendantsAndSelf())
            {
                xElement.Name = xml.Namespace + xElement.Name.LocalName;
            }

            return parsedXml;
        }

        private static void ReplaceElementIfNotMatch(XNode current, XElement desired)
        {
            if (desired.ToString() == current.ToString())
            {
                return;
            }

            var parentElement = current.Parent;
            if (parentElement == null)
            {
                throw new Exception("parentElement is null");
            }

            var previousElement = current.PreviousNode;
            current.Remove();

            if (previousElement != null)
            {
                previousElement.AddAfterSelf(desired);
            }
            else
            {
                parentElement.AddFirst(desired);
            }

        }

        private static void SetOrClearAttribute(string attributeName, string value, XElement xElement)
        {
            var attribute = xElement.Attributes().SingleOrDefault(x => x.Name == attributeName);
            if (value == null)
            {
                attribute?.Remove();
            }

            if (value != null && attribute == null)
            {
                xElement.Add(attribute = new XAttribute(attributeName, value));
            }

            if (value != null && attribute.Value != value)
            {
                attribute.Value = value;
            }
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

