using System;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Constants;
using Intent.Modules.Common.CSharp.Interactions;

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.InteractionStrategies;

internal record PayloadInfo(string VarName, string Type);

internal static class PatchBuilderHelper
{
    public static void EnsurePatchHelperMethods(
        ICSharpClassMethodDeclaration handleMethod,
        UpdateEntityActionTargetEndModel updateAction,
        ClassModel foundEntity,
        Func<ICSharpClassMethodDeclaration, PayloadInfo> getPayloadInfo)
    {
        ArgumentNullException.ThrowIfNull(getPayloadInfo);

        var payloadInfo = getPayloadInfo(handleMethod);
        if (payloadInfo is null)
        {
            throw new Exception($@"{handleMethod.Class.Name}.{handleMethod.Name}:  ""{nameof(payloadInfo)}"" cannot return `null`.");
        }

        var entityTypeName = handleMethod.File.Template.GetTypeName(TemplateRoles.Domain.Entity.Primary, foundEntity)!;
        var parameterType = payloadInfo.Type;
        var updateMapping = updateAction.Mappings.GetUpdateEntityMapping();
        if (updateMapping == null)
        {
            throw new ElementException(updateAction.InternalAssociationEnd, "No Update Entity mapping was found for PATCH update interaction.");
        }

        var @class = handleMethod.Class;

        @class.AddMethod(parameterType, "LoadOriginalState", method =>
        {
            method.Static();
            method.Private();
            method.AddParameter(entityTypeName, "entity");
            method.AddParameter(parameterType, payloadInfo.VarName);
                    
            method.AddStatement("ArgumentNullException.ThrowIfNull(entity);");
            method.AddStatement($"ArgumentNullException.ThrowIfNull({payloadInfo.VarName});");

            var generator = new JsonPatchLoadOriginalStateGenerator(handleMethod, updateMapping);
            foreach (var statement in generator.Generate())
            {
                if (statement is CSharpAssignmentStatement)
                {
                    statement.WithSemicolon();
                }
                method.AddStatement(statement);
            }

            method.AddStatement($"return {payloadInfo.VarName};");
        });

        @class.AddMethod(entityTypeName, "ApplyChangesTo", method =>
        {
            method.Static();
            method.Private();
            method.AddParameter(parameterType, payloadInfo.VarName);
            method.AddParameter(entityTypeName, "entity");

            method.AddStatement($"ArgumentNullException.ThrowIfNull({payloadInfo.VarName});");
            method.AddStatement("ArgumentNullException.ThrowIfNull(entity);");

            var mappingManager = handleMethod.GetMappingManager();

            // Ensure mapping statements target the helper method parameters.
            mappingManager.SetFromReplacement((IElement)updateAction.OtherEnd().Element, payloadInfo.VarName);
            mappingManager.SetToReplacement(foundEntity, "entity");
            mappingManager.SetToReplacement(updateAction, "entity");

            var updateStatements = mappingManager.GenerateUpdateStatements(updateMapping);

            foreach (var statement in updateStatements)
            {
                if (statement is CSharpAssignmentStatement)
                {
                    statement.WithSemicolon();
                }
                method.AddStatement(statement);
            }

            method.AddStatement("return entity;");
        });
    }
}