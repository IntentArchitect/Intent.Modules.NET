using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.SchemeProcessors;
using Intent.Modules.VisualStudio.Projects.NuGet;
using Intent.Modules.VisualStudio.Projects.Settings;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Templates;
using Intent.Utils;
using Microsoft.Build.Evaluation;
using NuGet.Versioning;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions
{
    public class NugetInstallerFactoryExtension : FactoryExtensionBase, IExecutionLifeCycle
    {
        public const string Identifier = "Intent.VisualStudio.NuGet.Installer";
        private const string SettingKeyForConsolidatePackageVersions = "Consolidate Package Versions"; // Must match the config entry in the .imodspec
        private const string SettingKeyForWarnOnMultipleVersionsOfSamePackage = "Warn On Multiple Versions of Same Package"; // Must match the config entry in the .imodspec

        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;
        private readonly IChanges _changeManager;
        private readonly IDictionary<string, IVisualStudioProjectTemplate> _projectRegistry = new Dictionary<string, IVisualStudioProjectTemplate>();
        private readonly IDictionary<string, List<string>> _projectPackageRemoveRequests = new Dictionary<string, List<string>>();

        private bool _settingConsolidatePackageVersions;
        private bool _settingWarnOnMultipleVersionsOfSamePackage;
        private IDictionary<VisualStudioProjectScheme, INuGetSchemeProcessor> _nuGetProjectSchemeProcessors;

        public NugetInstallerFactoryExtension(ISoftwareFactoryEventDispatcher sfEventDispatcher, IChanges changeManager)
        {
            _sfEventDispatcher = sfEventDispatcher;
            _changeManager = changeManager;
        }

        public override int Order => 200;

        public override string Id => Identifier;

        public override void Configure(IDictionary<string, string> settings)
        {
            settings.SetIfSupplied(
                name: SettingKeyForConsolidatePackageVersions,
                setSetting: parsedValue => _settingConsolidatePackageVersions = parsedValue,
                convert: rawValue => true.ToString().Equals(rawValue, StringComparison.OrdinalIgnoreCase));
            settings.SetIfSupplied(
                name: SettingKeyForWarnOnMultipleVersionsOfSamePackage,
                setSetting: parsedValue => _settingWarnOnMultipleVersionsOfSamePackage = parsedValue,
                convert: rawValue => true.ToString().Equals(rawValue, StringComparison.OrdinalIgnoreCase));

            base.Configure(settings);
        }

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            _nuGetProjectSchemeProcessors = new Dictionary<VisualStudioProjectScheme, INuGetSchemeProcessor>
            {
                { VisualStudioProjectScheme.Sdk, new SdkSchemeProcessor(application, _sfEventDispatcher) },
                { VisualStudioProjectScheme.Unsupported, new UnsupportedSchemeProcessor() },
                { VisualStudioProjectScheme.FrameworkWithPackageReference, new NetFrameworkPackageReferencesSchemeProcessor() },
                { VisualStudioProjectScheme.FrameworkWithPackagesDotConfig, new NetFrameworkPackagesDotConfigSchemeProcessor() }
            };

            base.OnAfterTemplateRegistrations(application);
        }

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            application.EventDispatcher.Subscribe<VisualStudioProjectCreatedEvent>(HandleEvent);
            application.EventDispatcher.Subscribe<RemoveNugetPackageEvent>(HandleEvent);
            base.OnBeforeTemplateRegistrations(application);
        }

        protected override void OnAfterTemplateExecution(IApplication application)
        {
            var tracing = new TracingWithPrefix(Logging.Log, "NuGet - ");

            tracing.Info("Start processing packages");

            var depVerOverBehOption = application.Settings.DependencyVersionOverwriteBehavior().Value == null
                ? DependencyVersionOverwriteBehaviorOption.IfNewer
                : application.Settings.DependencyVersionOverwriteBehavior().AsEnum();

            // Call a separate method to do the actual execution which is internally accessible and more easily unit testable.
            Execute(
                nuGetProjectSchemeProcessors: _nuGetProjectSchemeProcessors,
                applicationProjects: _projectRegistry.Values,
                tracing: tracing,
                saveProjectDelegate: SaveProject,
                dependencyVersionOverwriteBehavior: depVerOverBehOption);

            tracing.Info("Package processing complete");
            base.OnAfterTemplateExecution(application);
        }

        private void HandleEvent(RemoveNugetPackageEvent @event)
        {
            if (!_projectPackageRemoveRequests.TryGetValue(@event.Target.GetProject().Id, out var removeEvents))
            {
                removeEvents = new List<string>();
                _projectPackageRemoveRequests.Add(@event.Target.GetProject().Id, removeEvents);
            }
            removeEvents.Add(@event.NugetPackageName);
        }

        private void HandleEvent(VisualStudioProjectCreatedEvent @event)
        {
            if (_projectRegistry.ContainsKey(@event.TemplateInstance.ProjectId))
            {
                throw new Exception($"Attempted to add project with same project Id [{@event.TemplateInstance.ProjectId}] (location: {@event.TemplateInstance.FilePath})");
            }
            _projectRegistry.Add(@event.TemplateInstance.ProjectId, @event.TemplateInstance);
        }

        private void SaveProject(string filePath, string content)
        {
            var change = _changeManager.FindChange(filePath);
            content = content.ReplaceLineEndings();

            // Normalize the content of both by parsing with no whitespace and calling .ToString()
            var targetContent = XDocument.Parse(content).ToString();
            var existingContent = change != null
                ? XDocument.Parse(change.Content).ToString().ReplaceLineEndings()
                : XDocument.Load(filePath).ToString().ReplaceLineEndings();

            if (existingContent == targetContent)
            {
                return;
            }

            if (change != null)
            {
                change.ChangeContent(content);
                return;
            }

            _sfEventDispatcher.Publish(new SoftwareFactoryEvent(SoftwareFactoryEvents.OverwriteFileCommand, new Dictionary<string, string>
            {
                { "FullFileName", filePath },
                { "Content", content },
            }));
        }

        /// <param name="applicationProjects"></param>
        /// <param name="tracing"></param>
        /// <param name="saveProjectDelegate">T1 = path, T2 = content</param>
        /// <param name="dependencyVersionOverwriteBehavior"></param>
        internal void Execute(
            IDictionary<VisualStudioProjectScheme, INuGetSchemeProcessor> nuGetProjectSchemeProcessors,
            IEnumerable<IVisualStudioProjectTemplate> applicationProjects,
            ITracing tracing,
            Action<string, string> saveProjectDelegate,
            DependencyVersionOverwriteBehaviorOption dependencyVersionOverwriteBehavior)
        {
            string report;
            var (projectPackages, highestVersions) = DeterminePackages(nuGetProjectSchemeProcessors, applicationProjects);

            ConsolidateRequestedPackages(projectPackages);

            if (_settingConsolidatePackageVersions &&
                !string.IsNullOrWhiteSpace(report = GetPackagesWithMultipleVersionsReport(projectPackages)))
            {
                tracing.Info(
                    "Multiple versions exist for one or more NuGet packages within the solution. Intent will now automatically " +
                           "upgrade any lower versions to the highest installed version within the solution. To disable this behaviour " +
                           $"change the 'Consolidate Package Versions' option in Intent in the {Identifier} module configuration." +
                           $"{Environment.NewLine}" +
                           $"{Environment.NewLine}" +
                           $"{report}");

                ConsolidatePackageVersions(projectPackages, highestVersions);
            }

            foreach (var projectPackage in projectPackages)
            {
                if (!_projectPackageRemoveRequests.TryGetValue(projectPackage.ProjectId, out var packagesToRemove))
                {
                    packagesToRemove = new List<string>();
                }

                var updatedProjectContent = projectPackage.Processor.InstallPackages(
                    solutionModelId: projectPackage.SolutionModelId,
                    projectStereotypes: projectPackage.ProjectStereotypes,
                    projectPath: projectPackage.FilePath,
                    projectContent: projectPackage.Content,
                    requestedPackages: projectPackage.RequestedPackages,
                    installedPackages: projectPackage.InstalledPackages,
                    packagesToRemove: packagesToRemove,
                    projectName: projectPackage.Name,
                    tracing: tracing,
                    dependencyVersionOverwriteBehavior: dependencyVersionOverwriteBehavior);

                if (XDocument.Parse(updatedProjectContent).ToString() == XDocument.Parse(projectPackage.Content).ToString())
                {
                    continue;
                }

                saveProjectDelegate(projectPackage.FilePath, updatedProjectContent);
            }

            if (_settingWarnOnMultipleVersionsOfSamePackage &&
                !_settingConsolidatePackageVersions &&
                !string.IsNullOrWhiteSpace(report = GetPackagesWithMultipleVersionsReport(projectPackages)))
            {
                tracing.Warning(
                    "Multiple versions exist for one or more NuGet packages within the solution. You should consider " +
                           "consolidating these package versions within Visual Studio or alternatively enable the 'Consolidate " +
                           $"Package Versions' option in Intent in the {Identifier} module configuration." +
                           $"{Environment.NewLine}" +
                           $"{Environment.NewLine}" +
                           $"{report}");
            }
        }

        /// <summary>
        /// This method does the  following
        /// 1.) Consolidates version numbers across requested packages if required (including Implicit dependencies)
        /// 2.) Removes Requested Nuget packages if they already present from transative dependencies (including Implicit dependencies)
        /// </summary>
        /// <param name="projectPackages"></param>
        internal void ConsolidateRequestedPackages(IEnumerable<NuGetProject> projectPackages)
        {
            ConsolidateVersionsUpfront(projectPackages);
            RemoveTransativeDependencies(projectPackages);
        }

        private void ConsolidateVersionsUpfront(IEnumerable<NuGetProject> projectPackages)
        {
            var highestRequestedVersions = new Dictionary<string, VersionRange>();
            foreach (var projectPackage in projectPackages)
            {
                foreach (var kvp in projectPackage.RequestedPackages)
                {
                    DetermineHighest(highestRequestedVersions, kvp.Key, kvp.Value.Version);
                    if (kvp.Value.RequestedPackage?.Dependencies?.Any() == true)
                    {
                        foreach (var dependant in kvp.Value.RequestedPackage?.Dependencies)
                        {
                            var dependantVersion = VersionRange.Parse(dependant.Version);
                            DetermineHighest(highestRequestedVersions, dependant.Name, dependantVersion);
                        }
                    }
                }
            }

            foreach (var projectPackage in projectPackages)
            {
                foreach (var kvp in projectPackage.RequestedPackages)
                {
                    var highestRequested = highestRequestedVersions[kvp.Key];
                    if (kvp.Value.Version.MinVersion < highestRequested.MinVersion)
                    {
                        kvp.Value.Update(highestRequested, null);
                    }
                }
            }


            static void DetermineHighest(Dictionary<string, VersionRange> highestRequestedVersions, string packageName, VersionRange version)
            {
                if (!highestRequestedVersions.TryGetValue(packageName, out var current))
                {
                    highestRequestedVersions.Add(packageName, version);
                }
                else
                {
                    if (version.MinVersion > current.MinVersion)
                    {
                        highestRequestedVersions[packageName] = version;
                    }
                }
            }
        }

        private void RemoveTransativeDependencies(IEnumerable<NuGetProject> projectPackages)
        {
            var projectToNugetPackageMap = new Dictionary<string, Dictionary<string, VersionRange>>();
            foreach (var projectPackage in projectPackages)
            {
                var requestedPackages = new Dictionary<string, VersionRange>();
                foreach (var kvp in projectPackage.RequestedPackages)
                {
                    AddUpdatePackageVersion(requestedPackages, kvp.Key, kvp.Value.Version);
                    if (kvp.Value.RequestedPackage?.Dependencies?.Any() == true)
                    {
                        foreach (var dependant in kvp.Value.RequestedPackage?.Dependencies)
                        {
                            var dependentVersion = VersionRange.Parse(dependant.Version);
                            AddUpdatePackageVersion(requestedPackages, dependant.Name, dependentVersion);
                        }
                    }
                }
                projectToNugetPackageMap[projectPackage.OutputTarget.Id] = requestedPackages;
            }

            foreach (var projectPackage in projectPackages)
            {
                var dependantPackages = GetDependantPackages(projectPackage.OutputTarget, projectToNugetPackageMap);
                foreach (var dependantPackage in dependantPackages)
                {
                    if (projectPackage.RequestedPackages.TryGetValue(dependantPackage.Key, out var packge))
                    {
                        //If the package is already installed, don't remove it, so that the request can still update the installed package if required
                        if (projectPackage.InstalledPackages.ContainsKey(dependantPackage.Key))
                        {
                            continue;
                        }
                        //The requested package is higher than the dependent so install the higher version, 
                        //This scenario happens if an implicit version is lower than requested as the explicits are consolodated.
                        if (projectPackage.RequestedPackages[dependantPackage.Key].Version.MinVersion > dependantPackage.Value.MinVersion)
                        {
                            continue;
                        }

                        projectPackage.RequestedPackages.Remove(dependantPackage.Key);
                    }
                }

                List<string> toRemove = new List<string>();
                foreach (var requestedPackage in projectPackage.RequestedPackages)
                {
                    if (requestedPackage.Value.RequestedPackage?.Dependencies?.Any() == true)
                    {
                        foreach (var dependant in requestedPackage.Value.RequestedPackage?.Dependencies)
                        {
                            if (projectPackage.RequestedPackages.ContainsKey(dependant.Name))
                            {
                                var dependentVersion = VersionRange.Parse(dependant.Version);
                                if (projectPackage.RequestedPackages[dependant.Name].Version.MinVersion <= dependentVersion.MinVersion)
                                {
                                    toRemove.Add(dependant.Name);
                                }
                            }
                        }
                    }
                }
                foreach (var name in toRemove)
                {
                    projectPackage.RequestedPackages.Remove(name);
                }
            }
        }

        private Dictionary<string, VersionRange> GetDependantPackages(IOutputTarget project, Dictionary<string, Dictionary<string, VersionRange>> projectToNugetPackageMap)
        {
            var collectedPackages = new Dictionary<string, VersionRange>();

            if (!project.Dependencies().Any())
            {
                return collectedPackages;
            }

            foreach (var dependentProject in project.Dependencies())
            {
                if (projectToNugetPackageMap.TryGetValue(dependentProject.Id, out var packages))
                {
                    foreach (var package in packages)
                    {
                        AddUpdatePackageVersion(collectedPackages, package.Key, package.Value);
                    }
                }

                // Recursively collect packages from the dependent projects of this dependent project
                var dependentPackages = GetDependantPackages(dependentProject, projectToNugetPackageMap);
                foreach (var package in dependentPackages)
                {
                    AddUpdatePackageVersion(collectedPackages, package.Key, package.Value);
                }
            }

            return collectedPackages;
        }

        private static void AddUpdatePackageVersion(Dictionary<string, VersionRange> packages, string packageName, VersionRange version)
        {
            if (packages.TryGetValue(packageName, out var current))
            {
                if (current.MinVersion < version.MinVersion)
                {
                    packages[packageName] = version;
                }
            }
            else
            {
                packages[packageName] = version;
            }
        }

        /// <summary>
        /// Internal so available to unit tests
        /// </summary>
        internal (IReadOnlyCollection<NuGetProject> Projects, Dictionary<string, VersionRange> HighestVersions) DeterminePackages(
            IDictionary<VisualStudioProjectScheme, INuGetSchemeProcessor> nuGetProjectSchemeProcessors,
            IEnumerable<IVisualStudioProjectTemplate> applicationProjectTemplates)
        {
            var projects = new List<NuGetProject>();

            var highestVersions = new Dictionary<string, VersionRange>();
            foreach (var template in applicationProjectTemplates.OrderBy(x => x.Name))
            {
                var projectNugetInfo = DetermineProjectNugetPackageInfo(
                    nuGetProjectSchemeProcessors,
                     template);
                foreach (var nuGetVersion in projectNugetInfo.HighestVersions)
                {
                    if (!highestVersions.TryGetValue(nuGetVersion.Key, out var highestVersion) ||
                        (highestVersion != null &&
                         highestVersion.MinVersion < nuGetVersion.Value.MinVersion))
                    {
                        highestVersions[nuGetVersion.Key] = nuGetVersion.Value;
                    }
                }
                projects.Add(projectNugetInfo);
            }

            return (projects, highestVersions);
        }

        internal static NuGetProject DetermineProjectNugetPackageInfo(
            IDictionary<VisualStudioProjectScheme, INuGetSchemeProcessor> nuGetProjectSchemeProcessors,
            IVisualStudioProjectTemplate template)
        {
            var projectContent = template.LoadContent();
            var document = XDocument.Parse(projectContent);

            var projectType = document.ResolveProjectScheme();
            if (!nuGetProjectSchemeProcessors.TryGetValue(projectType, out var processor))
                throw new ArgumentOutOfRangeException(nameof(projectType), $"No scheme registered for type {projectType}.");

            var installedPackages = processor.GetInstalledPackages(template.Project.Solution.Id, template.FilePath, document);

            var highestVersionsInProject = new Dictionary<string, VersionRange>();
            foreach (var installedPackage in installedPackages)
            {
                var packageId = installedPackage.Key;

                if (!highestVersionsInProject.TryGetValue(packageId, out var highestVersion) ||
                    highestVersion.MinVersion < installedPackage.Value.Version.MinVersion)
                {
                    highestVersionsInProject[packageId] = installedPackage.Value.Version;
                }
            }

            var requestedPackages = new Dictionary<string, NuGetPackage>();

            foreach (var package in template.RequestedNugetPackages())
            {
                if (!VersionRange.TryParse(package.Version, out var semanticVersion))
                {
                    throw new Exception(
                        $"Could not parse '{package.Version}' from Intent metadata for package '{package.Name}' in project '{template.Name}' as a valid Semantic Version 2.0 'version' value.");
                }

                if (!highestVersionsInProject.TryGetValue(package.Name, out var highestVersion) ||
                    (highestVersion != null &&
                     highestVersion.MinVersion < semanticVersion.MinVersion))
                {
                    highestVersionsInProject[package.Name] = highestVersion = semanticVersion;
                }

                if (requestedPackages.TryGetValue(package.Name, out var requestedPackage))
                {
                    if (requestedPackage.Version.MinVersion < semanticVersion.MinVersion)
                    {
                        requestedPackage.Update(semanticVersion, package);
                    }
                }
                else
                {
                    requestedPackages.Add(package.Name, NuGetPackage.Create(template.FilePath, package, semanticVersion));
                }
            }

            return new NuGetProject
            {
                SolutionModelId = template.Project.Solution.Id,
                ProjectId = template.ProjectId,
                ProjectStereotypes = template.Project.Stereotypes,
                Content = projectContent,
                RequestedPackages = requestedPackages,
                InstalledPackages = installedPackages,
                HighestVersions = highestVersionsInProject,
                Name = template.Name,
                FilePath = template.FilePath,
                Processor = processor,
                OutputTarget = template.OutputTarget,
            };
        }

        private static void ConsolidatePackageVersions(IReadOnlyCollection<NuGetProject> projectPackages, IDictionary<string, VersionRange> highestVersions)
        {
            foreach (var highestVersion in highestVersions)
            {
                foreach (var projectPackage in projectPackages)
                {
                    if (projectPackage.RequestedPackages.TryGetValue(highestVersion.Key, out var requestedPackage))
                    {
                        if (requestedPackage.Version.MinVersion < highestVersion.Value.MinVersion)
                        {
                            requestedPackage.Version = highestVersion.Value;
                        }
                        
                        continue;
                    }
                    
                    if (projectPackage.InstalledPackages.TryGetValue(highestVersion.Key, out var installedPackage) &&
                        installedPackage.Version.MinVersion < highestVersion.Value.MinVersion)
                    {
                        projectPackage.RequestedPackages.Add(highestVersion.Key, installedPackage.Clone(highestVersion.Value));
                    }
                }
            }
        }

        private static string GetPackagesWithMultipleVersionsReport(IEnumerable<NuGetProject> nuGetProjects)
        {
            var resolvedProjects = nuGetProjects
                .Select(x => new
                {
                    ConsolidatedPackageVersions = x.GetConsolidatedPackages(),
                    ProjectName = x.Name
                })
                .ToArray();

            var packagesWithMultipleVersions = resolvedProjects
                .SelectMany(x => x.ConsolidatedPackageVersions)
                .GroupBy(x => x.Key, x => x.Value.Version)
                .Where(x => x.Distinct().Count() > 1)
                .Select(x => x.Key)
                .ToArray();

            if (!packagesWithMultipleVersions.Any())
            {
                return null;
            }

            return packagesWithMultipleVersions
                .Select(packageId => new
                {
                    PackageId = packageId,
                    VersionReport = resolvedProjects
                        .Where(x => x.ConsolidatedPackageVersions.ContainsKey(packageId))
                        .Select(x => new
                        {
                            x.ProjectName,
                            x.ConsolidatedPackageVersions[packageId].Version
                        })
                        .OrderByDescending(x => x.Version.ToString())
                        .ThenBy(x => x.ProjectName)
                        .Select(x => $"    Version {x.Version} in '{x.ProjectName}'")
                        .Aggregate((x, y) => x + Environment.NewLine + y)
                })
                .Select(x => $"{x.PackageId} has the following versions installed:{Environment.NewLine}{x.VersionReport}")
                .Aggregate((x, y) => x + Environment.NewLine + y);
        }
    }

    internal static class VsProjectExtensions
    {
        internal static VisualStudioProjectScheme ResolveProjectScheme(this XDocument xNode)
        {
            if (xNode == null)
            {
                return VisualStudioProjectScheme.Unsupported;
            }

            var (prefix, namespaceManager, _) = xNode.Document.GetNamespaceManager();

            if (xNode.XPathSelectElement($"/{prefix}:Project[@Sdk]", namespaceManager) != null)
            {
                return VisualStudioProjectScheme.Sdk;
            }

            if (xNode.XPathSelectElement($"/{prefix}:Project/{prefix}:ItemGroup/{prefix}:None[@Include='packages.config']", namespaceManager) != null)
            {
                return VisualStudioProjectScheme.FrameworkWithPackagesDotConfig;
            }

            if (xNode.XPathSelectElement($"/{prefix}:Project", namespaceManager) != null)
            {
                // Even if there is no PackageReference element, so long as there is no packages.config, then we are free to to use
                // PackageReferences going forward. In the event there is PackageReference element, then we are of course already
                // using the PackageReference scheme.
                return VisualStudioProjectScheme.FrameworkWithPackageReference;
            }

            return VisualStudioProjectScheme.Unsupported;
        }

        internal static string ToFormattedProjectString(this XDocument document)
        {
            // Changes the XML from:

            // <Project Sdk="Microsoft.NET.Sdk">
            //   <PropertyGroup>
            //     ...
            //   </PropertyGroup>
            //   <PropertyGroup>
            //     ...
            //   </PropertyGroup>
            // </Project>

            // To:

            // <Project Sdk="Microsoft.NET.Sdk">
            //
            //   <PropertyGroup>
            //     ...
            //   </PropertyGroup>
            //
            //   <PropertyGroup>
            //     ...
            //   </PropertyGroup>
            //
            // </Project>

            if (document.ResolveProjectScheme() != VisualStudioProjectScheme.Sdk)
            {
                return document.ToString();
            }

            document = XDocument.Parse(document.ToString(), LoadOptions.PreserveWhitespace);
            if (document == null)
                throw new Exception("document is null");
            if (document.Root == null)
                throw new Exception("document.Root is null");

            foreach (var node in document.Root.Nodes())
            {
                if (node is XText xText)
                {
                    xText.Value = $"\n{xText.Value}";
                }
            }

            return document.ToString();
        }
    }
}
