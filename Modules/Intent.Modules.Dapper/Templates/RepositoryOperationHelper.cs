#nullable enable
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Dapper.Templates.StoredProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Dapper.Templates;

public static class RepositoryOperationHelper
{
    public static void ApplyMethods(ICSharpFileBuilderTemplate? template, CSharpClass? @class, RepositoryModel repositoryModel)
    {
        if (template is null || @class is null)
        {
            return;
        }

        AddTypeSources(template);

        foreach (var operationModel in repositoryModel.Operations)
        {
            var isStoredProcedure = operationModel.IsStoredProcedureBacked();
            var isAsync = operationModel.IsAsync();

            @class.AddMethod(GetReturnType(template, operationModel.ReturnType, isAsync), operationModel.Name, method =>
            {
                method.AddMetadata("model", operationModel);
                method.RepresentsModel(operationModel);
                if (isAsync)
                {
                    method.Async();
                }

                method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());

                foreach (var parameterModel in operationModel.Parameters)
                {
                    method.AddParameter(GetParameterType(template, parameterModel.TypeReference), parameterModel.Name);
                }

                if (isAsync)
                {
                    method.AddOptionalCancellationTokenParameter();
                }

                if (isStoredProcedure)
                {
                    StoredProcedureInvocationHelper.ApplyOperationImplementation(template, method, operationModel, isAsync);
                }
            });
        }

        foreach (var storedProcedure in repositoryModel.GetStoredProcedureElements())
        {
            var isAsync = storedProcedure.IsAsync();

            @class.AddMethod(GetReturnType(template, storedProcedure.TypeReference, isAsync), storedProcedure.InternalElement.Name.ToPascalCase(), method =>
            {
                method.AddMetadata("model", storedProcedure.Model);
                method.RepresentsModel(storedProcedure.Model);
                if (isAsync)
                {
                    method.Async();
                }

                method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());

                foreach (var parameter in storedProcedure.Parameters)
                {
                    method.AddParameter(GetParameterType(template, parameter.TypeReference), parameter.InternalElement.Name.ToLocalVariableName());
                }

                if (isAsync)
                {
                    method.AddOptionalCancellationTokenParameter();
                }

                StoredProcedureInvocationHelper.ApplyStoredProcedureElementImplementation(template, method, storedProcedure, isAsync);
            });
        }
    }

    public static void ApplyMethods(ICSharpFileBuilderTemplate? template, CSharpInterface? @interface, RepositoryModel repositoryModel)
    {
        if (template is null || @interface is null)
        {
            return;
        }

        AddTypeSources(template);

        foreach (var operationModel in repositoryModel.Operations)
        {
            var isAsync = operationModel.IsAsync();

            @interface.AddMethod(GetReturnType(template, operationModel.ReturnType, isAsync), operationModel.Name, method =>
            {
                method.AddMetadata("model", operationModel);
                method.RepresentsModel(operationModel);
                if (isAsync)
                {
                    method.Async();
                }

                foreach (var parameterModel in operationModel.Parameters)
                {
                    method.AddParameter(GetParameterType(template, parameterModel.TypeReference), parameterModel.Name);
                }

                if (isAsync)
                {
                    method.AddOptionalCancellationTokenParameter();
                }
            });
        }

        foreach (var storedProcedure in repositoryModel.GetStoredProcedureElements())
        {
            var isAsync = storedProcedure.IsAsync();

            @interface.AddMethod(GetReturnType(template, storedProcedure.TypeReference, isAsync), storedProcedure.InternalElement.Name.ToPascalCase(), method =>
            {
                method.AddMetadata("model", storedProcedure.Model);
                method.RepresentsModel(storedProcedure.Model);
                if (isAsync)
                {
                    method.Async();
                }

                foreach (var parameter in storedProcedure.Parameters)
                {
                    method.AddParameter(GetParameterType(template, parameter.TypeReference), parameter.InternalElement.Name.ToLocalVariableName());
                }

                if (isAsync)
                {
                    method.AddOptionalCancellationTokenParameter();
                }
            });
        }

        if (repositoryModel.HasStoredProcedures())
        {
            template.CSharpFile
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks");
        }
    }

    /// <summary>
    /// Operation and stored procedure signatures can reference any domain type, so the templates which
    /// resolve them need to know where those types are generated. Without this the interface would emit
    /// unresolved type names with no <c>using</c>.
    /// </summary>
    private static void AddTypeSources(ICSharpFileBuilderTemplate template)
    {
        template.AddTypeSource(TemplateRoles.Domain.Enum);
        template.AddTypeSource(TemplateRoles.Domain.ValueObject);
        template.AddTypeSource(TemplateRoles.Domain.DataContract);
        template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
        template.AddTypeSource(TemplateRoles.Domain.Entity.Interface);
    }

    private static string GetParameterType(ICSharpFileBuilderTemplate template, ITypeReference typeReference)
    {
        return template.GetTypeName(typeReference) ?? "object";
    }

    private static string GetReturnType(ICSharpFileBuilderTemplate template, ITypeReference? returnType, bool isAsync = false)
    {
        if (returnType is null || returnType.Element is null)
        {
            return isAsync ? "Task" : "void";
        }

        return template.GetTypeName(returnType, "System.Collections.Generic.List<{0}>") ?? (isAsync ? "Task" : "void");
    }
}
