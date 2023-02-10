namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes
{
    internal enum VisualStudioProjectScheme
    {
        /// <summary>
        /// New lean format, required for use by .NET Standard and .NET project types.
        /// </summary>
        Sdk,

        /// <summary>
        /// The old verbose format used by .NET Framework projects, set to use newer PackageReference NuGet scheme.
        /// </summary>
        FrameworkWithPackageReference,

        /// <summary>
        /// The old verbose format used by .NET Framework projects, set to use older packages.config NuGet scheme.
        /// </summary>
        FrameworkWithPackagesDotConfig,

        /// <summary>
        /// Unsupported / unknown projected type.
        /// </summary>
        Unsupported
    }
}