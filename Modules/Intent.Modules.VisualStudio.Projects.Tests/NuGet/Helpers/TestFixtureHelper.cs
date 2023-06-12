using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Templates;
using NSubstitute;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers
{
    internal static class TestFixtureHelper
    {
        internal static ProjectImplementation CreateProject(VisualStudioProjectScheme? scheme, TestVersion testVersion, TestPackage testPackage, IDictionary<string, string> nugetPackagesToInstall)
        {
            return new ProjectImplementation(scheme, testVersion, testPackage, nugetPackagesToInstall);
        }

        internal static NuGetProject CreateNuGetProject(VisualStudioProjectScheme? scheme, TestVersion testVersion, TestPackage testPackage, IDictionary<string, string> nugetPackagesToInstall)
        {
            return NugetInstallerFactoryExtension.DetermineProjectNugetPackageInfo(CreateProject(scheme, testVersion, testPackage, nugetPackagesToInstall));
        }

        internal class ProjectImplementation : IVisualStudioProjectTemplate
        {
            private readonly IDictionary<string, string> _nugetPackagesToInstall;

            public ProjectImplementation(VisualStudioProjectScheme? scheme, TestVersion testVersion, TestPackage testPackage, IDictionary<string, string> nugetPackagesToInstall)
            {
                _nugetPackagesToInstall = nugetPackagesToInstall;

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

                public string Id { get; }
                public string Name { get; }
                public string Location { get; }
                public string RelativeLocation { get; }
                public string Type { get; }
                public IOutputTarget Parent { get; }
                public IEnumerable<ITemplate> TemplateInstances { get; }
                public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();
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

                #region throw new NotImplementedException() implementations
                public string TargetFramework => throw new NotImplementedException();
                public bool CanAddFile(string file) => throw new NotImplementedException();
                public IList<AssemblyRedirectInfo> AssemblyRedirects => throw new NotImplementedException();
                #endregion
            }
        }

        internal static string GetPath(VisualStudioProjectScheme? scheme, TestVersion testVersion, int number)
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
}