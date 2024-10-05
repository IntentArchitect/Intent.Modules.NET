using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.VisualStudio.Projects.Templates
{
    public static class NetCoreProjectExtensions
    {
        public static void InstallNugetPackages(this IProject project, XDocument doc)
        {
            var nugetPackages = project
                .NugetPackages()
                .Distinct()
                .GroupBy(x => x.Name)
                .ToDictionary(x => x.Key, x => x)
                .Select(x => x.Value.OrderByDescending(y => y.Version).First())
                .OrderBy(x => x.Name)
                .ToArray();

            if (!nugetPackages.Any())
            {
                return;
            }

            var packageReferenceItemGroup = doc.XPathSelectElement("Project/ItemGroup[PackageReference]");
            if (packageReferenceItemGroup == null)
            {
                packageReferenceItemGroup = new XElement("ItemGroup");
                packageReferenceItemGroup.Add(Environment.NewLine);
                packageReferenceItemGroup.Add("  ");

                var projectElement = doc.XPathSelectElement("Project");

                projectElement.Add("  ");
                projectElement.Add(packageReferenceItemGroup);
                projectElement.Add(Environment.NewLine);
                projectElement.Add(Environment.NewLine);
            }

            foreach (var package in nugetPackages)
            {
                var existingReference =
                    packageReferenceItemGroup.XPathSelectElement($"PackageReference[@Include='{package.Name}']");


                // TODO: It would be nice if we inserted these alphatically into existing items
                if (existingReference == null)
                {
                    //tracing.Info($"{TracingOutputPrefix}Installing {nugetPackageInfo.Name} {nugetPackageInfo} into project {netCoreProject.Name}");

                    packageReferenceItemGroup.Add("  ");
                    var xElement = new XElement("PackageReference",
                        new XAttribute("Include", package.Name),
                        new XAttribute("Version", package.Version));

                    var subElementAdded = false;
                    if (package.PrivateAssets != null && package.PrivateAssets.Any())
                    {
                        xElement.Add($"{Environment.NewLine}      ");
                        xElement.Add(new XElement("PrivateAssets", package.PrivateAssets.Aggregate((x, y) => x + "; " + y)));
                        subElementAdded = true;
                    }

                    if (package.IncludeAssets != null && package.IncludeAssets.Any())
                    {
                        xElement.Add($"{Environment.NewLine}      ");
                        xElement.Add(new XElement("PrivateAssets", package.IncludeAssets.Aggregate((x, y) => x + "; " + y)));
                        subElementAdded = true;
                    }

                    if (subElementAdded)
                    {
                        xElement.Add($"{Environment.NewLine}    ");
                    }

                    packageReferenceItemGroup.Add(xElement);
                    packageReferenceItemGroup.Add(Environment.NewLine);
                    packageReferenceItemGroup.Add("  ");
                }
            }
        }

        public static void SyncFrameworkReferences(this IOutputTarget project, XDocument doc)
        {
            var frameworkDependencies = project.FrameworkDependencies().ToArray();
            if (frameworkDependencies.Length == 0)
            {
                return;
            }

            var itemGroupElement = doc.XPathSelectElement("Project/ItemGroup[FrameworkReference]");
            if (itemGroupElement == null)
            {
                itemGroupElement = new XElement("ItemGroup");
                itemGroupElement.Add(Environment.NewLine);
                itemGroupElement.Add("  ");

                var projectElement = doc.XPathSelectElement("Project");

                projectElement.Add("  ");
                projectElement.Add(itemGroupElement);
                projectElement.Add(Environment.NewLine);
                projectElement.Add("  ");
            }

            foreach (var dependency in frameworkDependencies)
            {
                var existingItem = doc.XPathSelectElement($"/Project/ItemGroup/FrameworkReference[@Include='{dependency}']");
                if (existingItem != null)
                {
                    continue;
                }

                /*
                <FrameworkReference Include="Microsoft.AspNetCore.App" />
                */

                var item = new XElement(XName.Get("FrameworkReference"));
                item.Add(new XAttribute("Include", dependency));

                itemGroupElement.Add("  ");
                itemGroupElement.Add(item);
                itemGroupElement.Add(Environment.NewLine);
                itemGroupElement.Add("  ");
            }
        }

        public static void SyncProjectReferences(this IOutputTarget project, XDocument doc)
        {
            SyncActualProjectReferences(project, doc);

            if (project.Dependencies().Count <= 0)
            {
                return;
            }

            var itemGroupElement = doc.XPathSelectElement("Project/ItemGroup[ProjectReference]");
            if (itemGroupElement == null)
            {
                itemGroupElement = new XElement("ItemGroup");
                itemGroupElement.Add(Environment.NewLine);
                itemGroupElement.Add("  ");

                var projectElement = doc.XPathSelectElement("Project");

                projectElement.Add("  ");
                projectElement.Add(itemGroupElement);
                projectElement.Add(Environment.NewLine);
                projectElement.Add("  ");
            }

            foreach (var dependency in project.Dependencies())
            {
                var projectUrl = Path
                    .Join(
                        path1: Path.GetRelativePath(project.Location, dependency.Location),
                        path2: $"{dependency.Name}.csproj")
                    .Replace('/', '\\'); // .csproj files always use backslash as a path separator

                var projectReferenceItem = doc.XPathSelectElement($"/Project/ItemGroup/ProjectReference[@Include='{projectUrl}']");
                if (projectReferenceItem != null)
                {
                    continue;
                }

                /*
                <ProjectReference Include="..\Intent.SoftwareFactory\Intent.SoftwareFactory.csproj"/>
                */

                var item = new XElement(XName.Get("ProjectReference"));
                item.Add(new XAttribute("Include", projectUrl));

                itemGroupElement.Add("  ");
                itemGroupElement.Add(item);
                itemGroupElement.Add(Environment.NewLine);
                itemGroupElement.Add("  ");
            }
        }

        private static void SyncActualProjectReferences(IOutputTarget project, XDocument doc)
        {
            // Temporary hack to address an issue in the current Application.Dtos module doing this (which needs to be removed)
            // AddAssemblyReference(new GacAssemblyReference("System.Runtime.Serialization"));
            var references = project.References().Where(r => r.Library != "System.Runtime.Serialization").ToList();
            if (references.Count <= 0)
            {
                return;
            }

            var itemGroupElement = doc.XPathSelectElement("Project/ItemGroup[ProjectReference]");
            if (itemGroupElement == null)
            {
                itemGroupElement = new XElement("ItemGroup");
                itemGroupElement.Add(Environment.NewLine);
                itemGroupElement.Add("  ");

                var projectElement = doc.XPathSelectElement("Project");

                projectElement.Add("  ");
                projectElement.Add(itemGroupElement);
                projectElement.Add(Environment.NewLine);
                projectElement.Add("  ");
            }

            foreach (var reference in references)
            {

                var projectUrl = reference.Library.Replace('/', '\\'); 
                    
                var projectReferenceItem = doc.XPathSelectElement($"/Project/ItemGroup/ProjectReference[@Include='{projectUrl}']");
                if (projectReferenceItem != null)
                {
                    continue;
                }

                /*
                <ProjectReference Include="..\Intent.SoftwareFactory\Intent.SoftwareFactory.csproj"/>
                */

                var item = new XElement(XName.Get("ProjectReference"));
                item.Add(new XAttribute("Include", projectUrl));
                if (reference.HintPath != null) 
                {
                    item.Add(new XAttribute("HintPath", reference.HintPath));
                }
                itemGroupElement.Add("  ");
                itemGroupElement.Add(item);
                itemGroupElement.Add(Environment.NewLine);
                itemGroupElement.Add("  ");
            }
        }
    }
}