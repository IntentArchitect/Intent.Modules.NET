using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Metadata.Models;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.SchemeProcessors;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Modules.VisualStudio.Projects.Templates.DirectoryPackagesProps;
using Intent.Templates;
using NSubstitute;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers;

internal static class TestFixtureHelper
{
    public static IVisualStudioProjectTemplate CreateProject(VisualStudioProjectScheme? scheme, TestVersion testVersion, TestPackage testPackage, IDictionary<string, string> nugetPackagesToInstall)
    {
        var package = Substitute.For<IPackage>();
        package.Id.Returns(string.Empty);
        package.Stereotypes.Returns(Enumerable.Empty<IStereotype>());
        package.SpecializationTypeId.Returns(VisualStudioSolutionModel.SpecializationTypeId);

        var solution = new VisualStudioSolutionModel(package);

        var project = Substitute.For<IVisualStudioProject>();
        project.Solution.Returns(solution);

        return new ProjectImplementation(
            scheme: scheme,
            testVersion: testVersion,
            testPackage: testPackage,
            nugetPackagesToInstall: nugetPackagesToInstall,
            project: project);
    }



    public static SdkSchemeProcessor CreateSdkProcessor()
    {
        return new SdkSchemeProcessor(
            application: CreateApplicationWithCpmTemplate(),
            softwareFactoryEventDispatcher: Substitute.For<ISoftwareFactoryEventDispatcher>());
    }

    public static NuGetProject CreateNuGetProject(
        VisualStudioProjectScheme? scheme,
        INuGetSchemeProcessor processor,
        TestVersion testVersion,
        TestPackage testPackage,
        IDictionary<string, string> nugetPackagesToInstall)
    {
        var processors = new Dictionary<VisualStudioProjectScheme, INuGetSchemeProcessor>();

        foreach (var enumeratedScheme in Enum.GetValues<VisualStudioProjectScheme>())
        {
            processors[enumeratedScheme] = processor;
        }

        return NugetInstallerFactoryExtension.DetermineProjectNugetPackageInfo(
            nuGetProjectSchemeProcessors: processors,
            template: CreateProject(scheme, testVersion, testPackage, nugetPackagesToInstall));
    }

    public static IApplication CreateApplicationWithCpmTemplate()
    {
        var cpmTemplate = Substitute.For<ICpmTemplate>();
        cpmTemplate.CanRunTemplate().Returns(false);

        var application = Substitute.For<IApplication>();
        application.GetApplicationTemplates().Returns(_ => new ITemplate[] { cpmTemplate });
        return application;
    }

    public class ProjectImplementation : IVisualStudioProjectTemplate
    {
        private readonly IDictionary<string, string> _nugetPackagesToInstall;

        public ProjectImplementation(
            VisualStudioProjectScheme? scheme,
            TestVersion testVersion,
            TestPackage testPackage,
            IDictionary<string, string> nugetPackagesToInstall,
            IVisualStudioProject project)
        {
            _nugetPackagesToInstall = nugetPackagesToInstall;

            Project = project;
            Name = $"{(scheme.HasValue ? scheme.Value.ToString() : "null")}_{testVersion}_{(int)testPackage}";
            FilePath = GetPath(scheme, testVersion, (int)testPackage);
            OutputTarget = new VSProjectOutputTarget();
        }

        public string ProjectId => Name;

        public string Name { get; }

        public string FilePath { get; }

        public string LoadContent()
        {
            return File.ReadAllText(FilePath);
        }

        public void UpdateContent(string content, ISoftwareFactoryEventDispatcher sfEventDispatcher)
        {
            File.WriteAllText(FilePath, content);
        }

        public IEnumerable<INugetPackageInfo> RequestedNugetPackages()
        {
            return _nugetPackagesToInstall.Select(x => new NuGetPackages(x));
        }

        public IEnumerable<string> GetTargetFrameworks()
        {
            return new List<string>();
        }

        public IVisualStudioProject Project { get; }
        public IOutputTarget OutputTarget { get; }
        public bool TryGetExistingFileContent(out string content) => throw new NotImplementedException();

        private class VSProjectOutputTarget : IOutputTarget
        {
            public bool Equals(IOutputTarget other)
            {
                throw new NotImplementedException();
            }

            public IList<IOutputTarget> GetTargetPath()
            {
                throw new NotImplementedException();
            }

            public bool HasRole(string role)
            {
                throw new NotImplementedException();
            }

            public bool OutputsTemplate(string templateId)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<string> GetSupportedFrameworks()
            {
                throw new NotImplementedException();
            }

            public bool HasTemplateInstances(string templateId)
            {
                throw new NotImplementedException();
            }

            public bool HasTemplateInstances(string templateId, Func<ITemplate, bool> predicate)
            {
                throw new NotImplementedException();
            }

            public string Id { get; } = Guid.NewGuid().ToString();
            public string Name { get; }
            public string Location { get; }
            public string RelativeLocation { get; }
            public string Type { get; }
            public IOutputTarget Parent { get; }
            public IEnumerable<ITemplate> TemplateInstances { get; }
            public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>() { ["VS.Dependencies"] = new List<IOutputTarget>() };
            public IApplication Application { get; }
            public ISoftwareFactoryExecutionContext ExecutionContext { get; }
        }

        private class NuGetPackages : INugetPackageInfo
        {
            public NuGetPackages(KeyValuePair<string, string> package)
            {
                Name = package.Key;
                Version = package.Value;
            }

            public string Name { get; }
            public string Version { get; }
            public string[] PrivateAssets => new string[0];
            public string[] IncludeAssets => new string[0];

            public IList<INugetPackageDependency> Dependencies => new List<INugetPackageDependency>();

            #region throw new NotImplementedException() implementations
            public string TargetFramework => throw new NotImplementedException();
            public bool CanAddFile(string file) => throw new NotImplementedException();
            public IList<AssemblyRedirectInfo> AssemblyRedirects => throw new NotImplementedException();

            #endregion
        }
    }

    internal static ProjectNugetSetupTestData GetCleanArchitectureProjectSetup()
    {
        var apiProjectOutputTarget = Substitute.For<IOutputTarget>();
        var applicationProjectOutputTarget = Substitute.For<IOutputTarget>();
        var domainProjectOutputTarget = Substitute.For<IOutputTarget>();
        var infrastructureProjectOutputTarget = Substitute.For<IOutputTarget>();
        var testProjectOutputTarget = Substitute.For<IOutputTarget>();

        apiProjectOutputTarget.Id.Returns("API");
        applicationProjectOutputTarget.Id.Returns("APPLICATION");
        domainProjectOutputTarget.Id.Returns("DOMAIN");
        infrastructureProjectOutputTarget.Id.Returns("INFRASTRUCTURE");
        testProjectOutputTarget.Id.Returns("TEST");

        apiProjectOutputTarget.Dependencies().Returns(new List<IOutputTarget> { applicationProjectOutputTarget, infrastructureProjectOutputTarget, domainProjectOutputTarget });
        applicationProjectOutputTarget.Dependencies().Returns(new List<IOutputTarget> { domainProjectOutputTarget });
        domainProjectOutputTarget.Dependencies().Returns(new List<IOutputTarget>());
        infrastructureProjectOutputTarget.Dependencies().Returns(new List<IOutputTarget> { applicationProjectOutputTarget, domainProjectOutputTarget });
        testProjectOutputTarget.Dependencies().Returns(new List<IOutputTarget> { apiProjectOutputTarget });

        var projectPackages = new List<NuGetProject>();

        var projectApi = new NuGetProject();
        projectApi.OutputTarget = apiProjectOutputTarget;
        projectPackages.Add(projectApi);

        var projectApplication = new NuGetProject();
        projectApplication.OutputTarget = applicationProjectOutputTarget;
        projectPackages.Add(projectApplication);

        var projectDomain = new NuGetProject();
        projectDomain.OutputTarget = domainProjectOutputTarget;
        projectPackages.Add(projectDomain);

        var projectInfrastructure = new NuGetProject();
        projectInfrastructure.OutputTarget = infrastructureProjectOutputTarget;
        projectPackages.Add(projectInfrastructure);

        var projectTest = new NuGetProject();
        projectTest.OutputTarget = testProjectOutputTarget;
        projectPackages.Add(projectTest);


        return new ProjectNugetSetupTestData { Api = projectApi, Application = projectApplication, Domain = projectDomain, Infrastructure = projectInfrastructure, Test = projectTest };
    }

    internal class ProjectNugetSetupTestData
    {
        internal NuGetProject Api { get; set; }
        internal NuGetProject Application { get; set; }
        internal NuGetProject Domain { get; set; }
        internal NuGetProject Infrastructure { get; set; }
        internal NuGetProject Test { get; set; }

        public IEnumerable<NuGetProject> Projects() => new List<NuGetProject> { Api, Application, Domain, Infrastructure, Test};
    }

    public static string GetPath(VisualStudioProjectScheme? scheme, TestVersion testVersion, int number)
    {
        string path;
        switch (scheme)
        {
            case VisualStudioProjectScheme.Sdk:
                path = $@"{scheme}/{testVersion}Version{number}.xml";
                break;
            case VisualStudioProjectScheme.FrameworkWithPackageReference:
                path = $@"{scheme}/{testVersion}Version{number}.xml";
                break;
            case VisualStudioProjectScheme.FrameworkWithPackagesDotConfig:
                path = $@"{scheme}/{testVersion}Version{number}/csproj.xml";
                break;
            case VisualStudioProjectScheme.Unsupported:
            case null:
                path = $@"Unsupported/csproj.xml";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scheme), scheme, null);
        }

        return $"NuGet/Data/{path}";
    }

    public static NugetInstallerFactoryExtension Configure(
        this NugetInstallerFactoryExtension nugetInstaller,
        bool consolidatePackageVersions,
        bool warnOnMultipleVersionsOfSamePackage)
    {
        nugetInstaller.Configure(new Dictionary<string, string>
        {
            { "Consolidate Package Versions", consolidatePackageVersions.ToString() },
            { "Warn On Multiple Versions of Same Package", warnOnMultipleVersionsOfSamePackage.ToString() }
        });

        return nugetInstaller;
    }

    public static NugetInstallerFactoryExtension GetNuGetInstaller(
        bool consolidatePackageVersions,
        bool warnOnMultipleVersionsOfSamePackage)
    {
        var nugetInstallerFactoryExtension = new NugetInstallerFactoryExtension(Substitute.For<ISoftwareFactoryEventDispatcher>(), GetChangeManager());
        nugetInstallerFactoryExtension.Configure(consolidatePackageVersions, warnOnMultipleVersionsOfSamePackage);

        return nugetInstallerFactoryExtension;
    }

    public static IChanges GetChangeManager()
    {
        var changeManager = Substitute.For<IChanges>();

        changeManager.FindChange(Arg.Any<string>()).Returns(x => null);

        return changeManager;
    }
}