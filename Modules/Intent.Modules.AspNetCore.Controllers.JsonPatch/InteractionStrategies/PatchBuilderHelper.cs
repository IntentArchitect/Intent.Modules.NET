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

internal static class PatchBuilderHelper
{
    public static void EnsurePatchHelperMethods(
        ICSharpClassMethodDeclaration handleMethod,
        UpdateEntityActionTargetEndModel updateAction,
        ClassModel foundEntity,
        Func<ICSharpClassMethodDeclaration, ICSharpParameter> getRequestParameter)
    {
        ArgumentNullException.ThrowIfNull(getRequestParameter);
        
        var requestParameter = getRequestParameter(handleMethod);
        if (requestParameter is null)
        {
            throw new Exception($@"{handleMethod.Class.Name}.{handleMethod.Name}:  ""{nameof(getRequestParameter)}"" cannot return `null` for request parameter");
        }

        var entityTypeName = handleMethod.File.Template.GetTypeName(TemplateRoles.Domain.Entity.Primary, foundEntity)!;
        var parameterType = requestParameter.Type;
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
            method.AddParameter(parameterType, requestParameter.Name);
                    
            method.AddStatement("ArgumentNullException.ThrowIfNull(entity);");
            method.AddStatement($"ArgumentNullException.ThrowIfNull({requestParameter.Name});");

            var generator = new JsonPatchLoadOriginalStateGenerator(handleMethod, updateMapping);
            foreach (var statement in generator.Generate())
            {
                if (statement is CSharpAssignmentStatement)
                {
                    statement.WithSemicolon();
                }
                method.AddStatement(statement);
            }

            method.AddStatement($"return {requestParameter.Name};");
        });

        @class.AddMethod(entityTypeName, "ApplyChangesTo", method =>
        {
            method.Static();
            method.Private();
            method.AddParameter(parameterType, requestParameter.Name);
            method.AddParameter(entityTypeName, "entity");

            method.AddStatement("ArgumentNullException.ThrowIfNull(command);");
            method.AddStatement("ArgumentNullException.ThrowIfNull(entity);");

            var mappingManager = handleMethod.GetMappingManager();

            // Ensure mapping statements target the helper method parameters.
            mappingManager.SetFromReplacement((IElement)updateAction.OtherEnd().Element, requestParameter.Name);
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