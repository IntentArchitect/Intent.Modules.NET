using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Constants;
using System.Linq;
using System.Threading;
using Intent.Configuration;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System;

public static class ProxyUrlHelper
{
    public static string GetProxyApplicationtUrl(IElement element, ISoftwareFactoryExecutionContext executionContext)
    {
        var url = string.Empty;

        var package = element.MappedElement?.Element?.Package;
        package ??= element.Package;

        if (package == null)
        {
            return url;
        }

        var proxyUrl = GetProxyApplicationtUrl(package, executionContext);
        return string.IsNullOrWhiteSpace(proxyUrl) ? "https://localhost:{app_port}/" : proxyUrl;
    }

    public static string GetProxyApplicationtUrl(IPackage package, ISoftwareFactoryExecutionContext executionContext)
    {
        // "Endpoint Settings => Service URL" defined in Intent.Metadata.WebApi
        var serviceUrl = package.GetStereotypeProperty("c06e9978-c271-49fc-b5c9-09833b6b8992", "2164bf84-1db8-42d0-94a6-255d2908b9b5", string.Empty);
        if (!string.IsNullOrWhiteSpace(serviceUrl))
        {
            return serviceUrl;
        }

        // if the Endpoint Settings is not defined, then we need to try get the base URL from the 
        // project in the VS Designer, in the "source" application
        var sourceAppConfig = executionContext.GetSolutionConfig()
            .GetApplicationReferences()
            .Where(a => a.Id == package.ApplicationId)
            .Select(app => executionContext.GetSolutionConfig().GetApplicationConfig(app.Id))
            .FirstOrDefault();

        if (sourceAppConfig is not null)
        {
            var vsDesigner = executionContext.MetadataManager.GetDesigner(sourceAppConfig.Id, Designers.VisualStudioId);

            IEnumerable<IPackage> vsPackages = [];

            try
            {
                vsPackages = vsDesigner.Packages;
            }
            // not the best way to handle this, but this occurs if there is an problem loading all of the packages
            // in the VS designer
            catch(Exception ex)
            {
                return string.Empty;
            }

            if (vsPackages.Count() == 1)
            {
                IPackage vsPackage = vsPackages.First();
                var launchProject = FindFirstMatchingElementRecursive(vsPackage);

                if (launchProject is not null)
                {
                    var applicationUrl = launchProject.GetStereotypeProperty(ProjectStereotypes.LaunchSettings.Id, ProjectStereotypes.LaunchSettings.BaseUrlId, "");

                    if (!string.IsNullOrEmpty(applicationUrl))
                    {
                        return applicationUrl;
                    }
                }
            }
        }

        return string.Empty;
    }

    // These two methods are duplicated as there is no common interface with ChildElements
    // between IPackage and IElement (that I can see)
    private static IElement? FindFirstMatchingElementRecursive(IPackage element)
    {
        // Check current level first
        var match = element.ChildElements
            .FirstOrDefault(c =>
                (c.SpecializationTypeId == "FFD54A85-9362-48AC-B646-C93AB9AC63D2" ||
                 c.SpecializationTypeId == "8e9e6693-2888-4f48-a0d6-0f163baab740") &&
                c.Stereotypes.Any(s => s.DefinitionId == ProjectStereotypes.LaunchSettings.Id));

        if (match != null)
        {
            return match; // Found, stop here
        }

        // Recurse into children
        foreach (var child in element.ChildElements)
        {
            var nestedMatch = FindFirstMatchingElementRecursive(child);
            if (nestedMatch != null)
            {
                return nestedMatch; // Propagate first match up
            }
        }

        return null; // No match found
    }

    private static IElement? FindFirstMatchingElementRecursive(IElement element)
    {
        // Check current level first
        var match = element.ChildElements
            .FirstOrDefault(c =>
                (c.SpecializationTypeId == "FFD54A85-9362-48AC-B646-C93AB9AC63D2" ||
                 c.SpecializationTypeId == "8e9e6693-2888-4f48-a0d6-0f163baab740") &&
                c.Stereotypes.Any(s => s.DefinitionId == ProjectStereotypes.LaunchSettings.Id));

        if (match != null)
        {
            return match; // Found, stop here
        }

        // Recurse into children
        foreach (var child in element.ChildElements)
        {
            var nestedMatch = FindFirstMatchingElementRecursive(child);
            if (nestedMatch != null)
            {
                return nestedMatch; // Propagate first match up
            }
        }

        return null; // No match found
    }
}