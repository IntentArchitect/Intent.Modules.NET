using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Metadata.Security.Api;
using Intent.Modules.AspNetCore.Grpc.Templates.CommonTypesProtoFile;
using Intent.Modules.AspNetCore.Grpc.Templates.MessageProtoFile;
using Intent.Modules.AspNetCore.Grpc.Templates.PagedResultProtoFile;
using Intent.Modules.AspNetCore.Grpc.Templates.ServiceProtoFile;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.Security.Models;
using Intent.Templates;

namespace Intent.Modules.AspNetCore.Grpc.Templates;

internal static class GeneralHelpers
{
    public static (string[] Service, Dictionary<IElement, string[]> ByEndpoint) GetAuthorizationAttributes(
        this ICSharpTemplate template,
        IElement container,
        IEnumerable<IElement> endpointElements)
    {
        var byEndpoint = endpointElements
            .ToDictionary(childElement => childElement, childElement => SecurityModelHelpers.GetSecurityModels(childElement, checkParents: false).ToArray());

        var securedByDefault = template.ExecutionContext.GetSettings().GetSetting(MetadataIds.ApiSettingsGroupId, MetadataIds.ApiDefaultSecuritySettingsId)?.Value == "secured";
        var serviceAttributes = SecurityModelHelpers.GetSecurityModels(container, checkParents: true).Select(AsAttribute).ToArray();
        var serviceIsExplicitlyUnsecured = IsExplicitlyUnsecured(container, checkParents: true);

        if (serviceAttributes.Length == 0 &&
            byEndpoint.Keys.All(x => IsExplicitlyUnsecured(x)) ||
            (serviceIsExplicitlyUnsecured && byEndpoint.Values.All(x => x.Length == 0)))
        {
            return (Service: [UseAllowAnonymous()], ByEndpoint: byEndpoint.Keys.ToDictionary(x => x, _ => Array.Empty<string>()));
        }

        var first = byEndpoint.First().Value;
        if (serviceAttributes.Length == 0 &&
            !serviceIsExplicitlyUnsecured &&
            first.Length > 0 &&
            byEndpoint.All(x => AreSame(first, x.Value)))
        {
            return (Service: first.Select(AsAttribute).ToArray(), ByEndpoint: byEndpoint.Keys.ToDictionary(x => x, _ => Array.Empty<string>()));
        }

        if (serviceIsExplicitlyUnsecured)
        {
            serviceAttributes = [UseAllowAnonymous()];
        }
        else if (serviceAttributes.Length == 0 &&
                 securedByDefault &&
                 byEndpoint.Any(x => x.Value.Length == 0 && !IsExplicitlyUnsecured(x.Key)))
        {
            serviceAttributes = [UseAuthorize()];
        }

        return (Service: serviceAttributes,
                ByEndpoint: byEndpoint.ToDictionary(
                    x => x.Key,
                    x => IsExplicitlyUnsecured(x.Key)
                        ? [UseAllowAnonymous()]
                        : x.Value.Select(AsAttribute).ToArray()));

        string UseAuthorize() => template.UseType("Microsoft.AspNetCore.Authorization.Authorize");
        string UseAllowAnonymous() => template.UseType("Microsoft.AspNetCore.Authorization.AllowAnonymous");

        static bool AreSame(IEnumerable<ISecurityModel> first, IEnumerable<ISecurityModel> second)
        {
            var firstArrayed = first.OrderBy(x => x.EquatablePolicies).ThenBy(x => x.EquatableRoles).ToArray();
            var secondArrayed = second.OrderBy(x => x.EquatablePolicies).ThenBy(x => x.EquatableRoles).ToArray();

            if (firstArrayed.Length != secondArrayed.Length)
            {
                return false;
            }

            return firstArrayed.Zip(secondArrayed).All(x =>
                x.First.EquatablePolicies == x.Second.EquatablePolicies &&
                x.First.EquatableRoles == x.Second.EquatableRoles);
        }

        string AsAttribute(ISecurityModel model)
        {
            var authorize = UseAuthorize();
            var properties = new List<string>(2);

            if (model.Roles.Count > 0)
            {
                properties.Add($"Roles = \"{string.Join(',', model.Roles)}\"");
            }

            if (model.Policies.Count > 0)
            {
                properties.Add($"Policy = \"{string.Join(',', model.Policies)}\"");
            }

            return properties.Count == 0
                ? authorize
                : $"{authorize}({string.Join(", ", properties)})";
        }

        bool IsExplicitlyUnsecured(IHasStereotypes hasStereotypes, bool checkParents = false)
        {
            while (hasStereotypes != null)
            {
                if (hasStereotypes.HasStereotype(OperationModelStereotypeExtensions.Unsecured.DefinitionId))
                {
                    return true;
                }

                if (hasStereotypes.HasStereotype(OperationModelStereotypeExtensions.Secured.DefinitionId) ||
                    hasStereotypes is not IElement element)
                {
                    return false;
                }

                if (!checkParents)
                {
                    return false;
                }

                hasStereotypes = element.ParentElement != null
                    ? element.ParentElement
                    : element.Package;
            }

            return false;
        }
    }

    public static string GetClosedGenericTypeName(this ITypeReference typeReference)
    {
        var sb = new StringBuilder();
        ResolveRecursively(typeReference, sb);
        return sb.ToString();

        static void ResolveRecursively(ITypeReference typeReference, StringBuilder sb)
        {
            var genericTypeParameters = typeReference.GenericTypeParameters.ToArray();

            sb.Append(typeReference.Element?.Name.ToPascalCase());
            if (genericTypeParameters.Length == 0)
            {
                return;
            }

            sb.Append("Of");
            foreach (var genericTypeParameter in genericTypeParameters)
            {
                ResolveRecursively(genericTypeParameter, sb);
                sb.Append("And");
            }

            // Remove the last "And"
            sb.Length -= "And".Length;
        }
    }

    public static string GetProtoFolderPath<TModel>(this IIntentTemplate<TModel> template)
    {
        if (template.Model is IHasFolder)
        {
            return template.GetFolderPath();
        }

        if (template.Model is not IElement element)
        {
            return string.Empty;
        }

        return string.Join('/', GetProtoFolderParts(element));
    }

    public static string GetProtoNamespace<TModel>(this IIntentTemplate<TModel> template)
    {
        var @namespace = template.GetNamespace();

        if (template.Model is IHasFolder ||
            template.Model is not IElement element)
        {
            return @namespace;
        }

        return string.Join('.', Enumerable.Empty<string>().Append(@namespace).Concat(GetProtoFolderParts(element)));
    }

    public static IEnumerable<string> GetProtoFolderParts(IElement element)
    {
        while (element != null)
        {
            if (element.SpecializationTypeId is not MetadataIds.FolderTypeId)
            {
                element = element.ParentElement;
                continue;
            }

            var stereotype = element.GetStereotype(MetadataIds.FolderOptionsStereotypeId);
            if (stereotype?.TryGetProperty(MetadataIds.FolderOptionsStereotypePropertyId, out var a) == true &&
                !string.Equals(a.Value, true.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                element = element.ParentElement;
                continue;
            }

            yield return element.Name;
            element = element.ParentElement;
        }
    }

    private static string[] _protoRoot;

    public static string[] GetProtoRootParts(this IIntentTemplate template)
    {
        if (_protoRoot != null)
        {
            return _protoRoot;
        }

        var vsDesigner = template.ExecutionContext.MetadataManager.GetDesigner(template.ExecutionContext.GetApplicationConfig().Id, MetadataIds.VisualStudioDesignerId);
        var templateOutputs = vsDesigner.GetElementsOfType(MetadataIds.TemplateOutputTypeId)
            .Where(x => x.Name is
                CommonTypesProtoFileTemplate.TemplateId or
                MessageProtoFileTemplate.TemplateId or
                PagedResultProtoFileTemplate.TemplateId or
                ServiceProtoFileTemplate.TemplateId)
            .ToArray();

        if (templateOutputs.Length == 0)
        {
            return [];
        }

        var templateOutputFolders = templateOutputs
            .Select(x => GetFolders(x).Reverse().ToArray())
            .ToArray();

        var commonPartCount = 0;

        while (true)
        {
            if (templateOutputFolders.All(x => x.Length > commonPartCount && x[commonPartCount] == templateOutputFolders[0][commonPartCount]))
            {
                commonPartCount++;
                continue;
            }

            break;
        }

        _protoRoot = templateOutputFolders[0]
            .Take(commonPartCount)
            .ToArray();

        return _protoRoot;

        static IEnumerable<string> GetFolders(IElement element)
        {
            element = element.ParentElement;

            while (element?.SpecializationTypeId is MetadataIds.VisualStudioFolderTypeId)
            {
                yield return element.Name;
                element = element.ParentElement;
            }
        }
    }

    public static string[] GetProtoRelativePathParts(this IIntentTemplate template)
    {
        var element = (template as ITemplateWithModel)?.Model as IElement;

        return template.OutputTarget.GetTargetPath()
            .Skip(template.GetProtoRootParts().Length + 1)
            .Select(x => x.Name)
            .Concat(element != null ? GetProtoFolderParts(element) : [])
            .ToArray();
    }
}