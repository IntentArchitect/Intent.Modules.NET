using Intent.Metadata.Models;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Repositories.Api;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates;

#nullable enable
public static class RepositoryOperationHelper
{
    public static void ApplyMethods(ICSharpFileBuilderTemplate template, CSharpClass @class, RepositoryModel repositoryModel)
    {
        template.AddDomainTypeSources();

        foreach (var operationModel in repositoryModel.Operations)
        {
            if (operationModel.TryGetStoredProcedure(out _))
            {
                continue;
            }

            var isAsync = operationModel.Name.EndsWith("Async") || operationModel.HasStereotype("Asynchronous");
            @class.AddMethod(GetReturnType(template, operationModel.ReturnType), operationModel.Name, method =>
            {
                method.AddMetadata("model", operationModel);
                method.RepresentsModel(operationModel);
                if (isAsync)
                {
                    method.Async();
                }

                method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());

                foreach (var parameterModel in operationModel.Parameters)
                {
                    method.AddParameter(template.GetTypeName(parameterModel.TypeReference), parameterModel.Name);
                }

                if (isAsync)
                {
                    method.AddOptionalCancellationTokenParameter();
                }
            });
        }
    }

    public static void ApplyMethods(ICSharpFileBuilderTemplate template, CSharpInterface @interface, RepositoryModel repositoryModel)
    {
        template.AddDomainTypeSources();

        foreach (var operationModel in repositoryModel.Operations)
        {
            if (operationModel.TryGetStoredProcedure(out _))
            {
                continue;
            }

            var isAsync = operationModel.Name.EndsWith("Async") || operationModel.HasStereotype("Asynchronous");
            @interface.AddMethod(GetReturnType(template, operationModel.ReturnType), operationModel.Name, method =>
            {
                method.TryAddXmlDocComments(operationModel.InternalElement);
                method.AddMetadata("model", operationModel);
                method.RepresentsModel(operationModel);
                if (isAsync)
                {
                    method.Async();
                }

                foreach (var parameterModel in operationModel.Parameters)
                {
                    method.AddParameter(template.GetTypeName(parameterModel.TypeReference), parameterModel.Name);
                }

                if (isAsync)
                {
                    method.AddOptionalCancellationTokenParameter();
                }
            });
        }
    }

    public static void AddDomainTypeSources(this IIntentTemplate template)
    {
        template.AddTypeSource(TemplateRoles.Domain.Enum);
        template.AddTypeSource(TemplateRoles.Domain.Entity.Interface);
        template.AddTypeSource(TemplateRoles.Domain.DataContract);
    }

    private static string GetReturnType(ICSharpFileBuilderTemplate template, ITypeReference? returnType)
    {
        if (returnType is null)
        {
            return "void";
        }

        return template.GetTypeName(returnType, "System.Collections.Generic.List<{0}>");
    }
}