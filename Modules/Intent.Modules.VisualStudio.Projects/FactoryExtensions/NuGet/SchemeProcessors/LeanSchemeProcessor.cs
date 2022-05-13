using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes;
using NuGet.Versioning;

namespace Intent.Modules.VisualStudio.Projects.NuGet.SchemeProcessors
{
    internal class LeanSchemeProcessor : INuGetSchemeProcessor
    {
        public Dictionary<string, NuGetPackage> GetInstalledPackages(string projectPath, XNode xNode)
        {
            var packageReferenceElements = xNode.XPathSelectElements("Project/ItemGroup/PackageReference");

            return packageReferenceElements.ToDictionary(
                element => element.Attribute("Include")?.Value,
                element =>
                {
                    var version = GetAttributeOrElementValue(element, "Version");

                    var privateAssetsElement = element.XPathSelectElement("PrivateAssets");
                    var privateAssets = privateAssetsElement != null
                        ? privateAssetsElement.Value
                            .Split(';')
                            .Select(y => y.Trim())
                        : new string[0];

                    var includeAssetsElement = element.XPathSelectElement("IncludeAssets");
                    var includeAssets = includeAssetsElement != null
                        ? includeAssetsElement.Value
                            .Split(';')
                            .Select(y => y.Trim())
                        : new string[0];

                    return NuGetPackage.Create(version, includeAssets, privateAssets);
                });
        }

        public string InstallPackages(
            string projectContent,
            Dictionary<string, NuGetPackage> requestedPackages,
            Dictionary<string, NuGetPackage> installedPackages,
            string projectName,
            ITracing tracing)
        {
            var document = XDocument.Parse(projectContent);

            var packageReferenceItemGroup = document.XPathSelectElement("Project/ItemGroup[PackageReference]");
            if (packageReferenceItemGroup == null)
            {
                var projectElement = document.XPathSelectElement("Project");
                if (projectElement == null)
                {
                    throw new Exception("Project element not found.");
                }

                projectElement.Add(packageReferenceItemGroup = new XElement("ItemGroup"));
            }

            foreach (var requestedPackage in requestedPackages)
            {
                var packageId = requestedPackage.Key;
                var package = requestedPackage.Value;

                var existingProjectReference = document.XPathSelectElement(
                    $"Project/ItemGroup/ProjectReference[substring(@Include,string-length(@Include) -string-length('{packageId}.csproj') +1) = '{packageId}.csproj']");
                if (existingProjectReference != null)
                {
                    continue;
                }

                var packageReferenceElement =
                    packageReferenceItemGroup.XPathSelectElement($"PackageReference[@Include='{packageId}']");
                if (packageReferenceElement == null)
                {
                    tracing.Info($"Installing {packageId} {package.Version} into project {projectName}");
                    packageReferenceItemGroup.Add(packageReferenceElement = new XElement("PackageReference",
                        new XAttribute("Include", packageId),
                        new XAttribute("Version", package.Version.OriginalString)));
                }

                if (package.PrivateAssets.Any())
                {
                    var privateAssetsElement = packageReferenceElement.XPathSelectElement("PrivateAssets");
                    if (privateAssetsElement == null)
                    {
                        packageReferenceElement.Add(privateAssetsElement = new XElement("PrivateAssets"));
                    }

                    privateAssetsElement.SetValue(package.PrivateAssets.Aggregate((x, y) => x + "; " + y));
                }


                if (package.IncludeAssets.Any())
                {
                    var includeAssetsElement = packageReferenceElement.XPathSelectElement("IncludeAssets");
                    if (includeAssetsElement == null)
                    {
                        packageReferenceElement.Add(includeAssetsElement = new XElement("IncludeAssets"));
                    }

                    includeAssetsElement.SetValue(package.IncludeAssets.Aggregate((x, y) => x + "; " + y));
                }

                var packageVersion = GetAttributeOrElementValue(packageReferenceElement, "Version");
                if (packageVersion == null)
                {
                    throw new Exception("Missing version attribute from PackageReference element.");
                }

                if (VersionRange.Parse(packageVersion).MinVersion >= package.Version.MinVersion)
                {
                    continue;
                }

                tracing.Info(
                    $"Upgrading {packageId} from {packageVersion} to {package.Version} in project {projectName}");
                SetAttributeOrElementValue(packageReferenceElement, "Version", package.Version.OriginalString);
            }

            SortAlphabetically(packageReferenceItemGroup);
            return document.ToFormattedProjectString();
        }

        private static void SortAlphabetically(XContainer packageReferenceItemGroup)
        {
            // Remove and re-add the elements in alphabetical order
            var packageReferenceElements = packageReferenceItemGroup
                .Elements()
                .OrderBy(x => x.Attributes("Include").Single().Value);

            foreach (var packageReferenceElement in packageReferenceElements)
            {
                packageReferenceElement.Remove();
                packageReferenceItemGroup.Add(packageReferenceElement);
            }
        }

        private static string GetAttributeOrElementValue(XElement targetElement, string name)
        {
            return targetElement.Attribute(name)?.Value
                   ?? targetElement.Elements(name).SingleOrDefault()?.Value;
        }

        private void SetAttributeOrElementValue(XElement targetElement, string name, string value)
        {
            var nestedElement = targetElement.Elements(name).SingleOrDefault();
            if (nestedElement?.Value != null)
            {
                nestedElement.Value = value;
                return;
            }

            targetElement.SetAttributeValue(name, value);
        }
    }
}