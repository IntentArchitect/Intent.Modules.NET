using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using NuGet.Versioning;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet;
internal record PackageVersionRange(VersionRange VersionRange, NuGetPackage? Package);
