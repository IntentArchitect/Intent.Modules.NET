using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;

namespace Intent.Modules.SqlServerImporter.Tasks.Helpers;

internal static class MetadataManagerExtensions
{
    public static bool TryGetApplicationPackage(
        this IMetadataManager metadataManager,
        string applicationId,
        string packageId,
        [NotNullWhen(true)] out IPackage? applicationPackage,
        [NotNullWhen(false)] out string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(applicationId))
        {
            errorMessage = "Application Id is required";
            applicationPackage = null;
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(packageId))
        {
            errorMessage = "Package Id is required";
            applicationPackage = null;
            return false;
        }
        
        var designer = metadataManager.GetDesigner(applicationId, "Domain");
        if (designer == null)
        {
            errorMessage = $"Unable to find domain designer for application {applicationId}";
            applicationPackage = null;
            return false;
        }

        var package = designer.Packages.FirstOrDefault(p => p.Id == packageId);
        if (package == null)
        {
            errorMessage = $"Unable to find package with Id : {packageId}";
            applicationPackage = null;
            return false;
        }

        applicationPackage = package;
        errorMessage = null;
        return true;
    }
}