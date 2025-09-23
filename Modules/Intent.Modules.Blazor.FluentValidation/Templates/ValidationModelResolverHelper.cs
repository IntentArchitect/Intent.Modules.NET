using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.FluentValidation.Templates.ModelDefinitionValidator;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.FluentValidation.Shared;

#nullable enable

namespace Intent.Modules.Blazor.FluentValidation.Templates;

internal static class ValidationModelResolverHelper
{
    public static bool TryGetDtoTypeName(IFluentValidationTemplate template, IElement dtoModel, out string typeName)
    {
        if (dtoModel.ParentElement.IsComponentModel())
        {
            if (template.TryGetTypeName(RazorComponentCodeBehindTemplate.TemplateId, dtoModel.ParentElement, out var parentTypeName))
            {
                typeName = $"{parentTypeName}.{dtoModel.Name}";
                return true;
            }
        }
        else if (template.TryGetTypeName(template.ToValidateTemplateId, dtoModel, out var dtoTypeName))
        {
            typeName = dtoTypeName;
            return true;
        }

        typeName = string.Empty;
        return false;
    }

    public static IEnumerable<DtoFieldMetadata> GetDtoFields(IFluentValidationTemplate template, IElement dtoModel)
    {
        CSharpClass targetClass;
        if (dtoModel.ParentElement.IsComponentModel())
        {
            var componentTemplate = template.GetTemplate<ICSharpFileBuilderTemplate>(RazorComponentCodeBehindTemplate.TemplateId, dtoModel.ParentElement);
            targetClass = componentTemplate.CSharpFile.Classes.First().NestedClasses.First(p => p.Name == dtoModel.Name);
        }
        else
        {
            var dtoModelTemplate = template.GetTemplate<ICSharpFileBuilderTemplate>(template.ToValidateTemplateId, dtoModel);
            targetClass = dtoModelTemplate.CSharpFile.Classes.First();
        }
        
        var results = dtoModel.ChildElements.Select(field =>
            {
                if (!targetClass.TryGetReferenceForModel(field, out var reference) || reference is not CSharpProperty)
                {
                    return null;
                }

                return new DtoFieldMetadata(field, reference);
            })
            .Where(field => field is not null)
            .Cast<DtoFieldMetadata>();
        return results;
    }

    public static string[] GetAdditionalFolders<TModel>(TModel model) where TModel : IElementWrapper
    {
        if (model.InternalElement.ParentElement.IsComponentModel())
        {
            return model.InternalElement.ParentElement.AsComponentModel().GetParentFolderNames().ToArray();
        }
        else
        {
            return Enumerable.Empty<string>().ToArray();
        }

    }

    public static string GetFolderPath<TModel>(IIntentTemplate<TModel> template, TModel model) where TModel : IElementWrapper
    {
        string folderPath;
        if (model.InternalElement.ParentElement.IsComponentModel())
        {
            folderPath = string.Join("/", model.InternalElement.ParentElement.AsComponentModel().GetParentFolderNames());
        }
        else
        {
            folderPath = template.GetFolderPath();
        }

        return folderPath;
    }
}

internal record DtoFieldMetadata(IElement Field, IHasCSharpName Reference);