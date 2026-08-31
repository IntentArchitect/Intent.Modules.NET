using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Wolverine;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Constants;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.InteractionStrategies;

public class SendOnWolverineInteractionStrategy : IInteractionStrategy
{
    public bool IsMatch(IElement interaction)
    {
        return interaction.IsPerformInvocationTargetEndModel() &&
            (interaction.TypeReference.Element.IsCommandModel() || interaction.TypeReference.Element.IsQueryModel()) &&
            (interaction as IAssociationEnd)?.Mappings.Count() == 1;
    }

    public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
    {
        var interaction = (IAssociationEnd)interactionElement;
        var handlerClass = method.Class;
        var template = (ICSharpFileBuilderTemplate)handlerClass.File.Template;
        template.AddNugetDependency(NugetPackages.WolverineFx(template.OutputTarget));
        template.AddTypeSource(TemplateRoles.Application.Query);
        template.AddTypeSource(TemplateRoles.Application.Command);
        template.AddTypeSource(TemplateRoles.Application.Contracts.Dto);

        var csharpMapping = method.GetMappingManager();
        csharpMapping.AddMappingResolver(new CommandQueryMappingResolver(template));
        var @class = handlerClass;
        var ctor = @class.Constructors.First();
        // Hardcoded literal, not routed through UseType: UseType decides short-vs-qualified from
        // whatever it already knows about this file's OTHER using directives AT THE MOMENT it is
        // called - and that moment is racy relative to WHEN the eventing contracts' own IMessageBus
        // gets added to this same constructor by a different interaction strategy. If this call
        // runs first, UseType sees no clash yet and returns the short "IMessageBus", silently
        // importing `using Wolverine;` - which then collides with the contracts IMessageBus exactly
        // like the CS0104 this module's Point 1 fix removed. The type is well-known and never needs
        // shortening for correctness, so it is spelled out unconditionally instead.
        const string wolverineBusType = "Wolverine.IMessageBus";
        if (ctor.Parameters.All(x => x.Type != wolverineBusType))
        {
            ctor.AddParameter(wolverineBusType, "sender",
                param => { param.IntroduceReadonlyField((_, s) => s.ThrowArgumentNullException()); });
        }

        var requestName =
            interaction.TypeReference.Element.IsCommandModel() ? "command" : "query";

        var statements = new List<CSharpStatement>();
        statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(requestName),
            csharpMapping.GenerateCreationStatement(interaction.Mappings.Single())).WithSemicolon().SeparatedFromPrevious());
        var response = interaction.TypeReference.Element?.TypeReference?.Element;
        var cancellationToken = method.Parameters.FirstOrDefault(x => x.Type == "CancellationToken");

        if (response != null && interaction.TypeReference.Element.IsQueryModel())
        {
            var responseStaticElementId = "9acdd519-a45a-469d-89f1-00896a31ca61";
            csharpMapping.SetFromReplacement(interaction, response.Name.ToLocalVariableName());
            csharpMapping.SetToReplacement(interaction, response.Name.ToLocalVariableName());
            csharpMapping.SetFromReplacement(new StaticMetadata(responseStaticElementId), "");
            csharpMapping.SetToReplacement(new StaticMetadata(responseStaticElementId), "");

            var invokeCall = new CSharpInvocationStatement($"await _sender.InvokeAsync<{template.GetTypeName(response.AsTypeReference())}>")
                .AddArgument(requestName);
            if (cancellationToken != null)
            {
                invokeCall.AddArgument(cancellationToken.Name);
            }

            statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(response.Name.ToLocalVariableName()), invokeCall));
            method.TrackedEntities().Add(response.Id, new EntityDetails((IElement)response, response.Name.ToLocalVariableName(), null, false));
        }
        else if (response != null && interaction.TypeReference.Element.IsCommandModel())
        {
            var variableName = interaction.Name.ToLocalVariableName();
            if (string.IsNullOrWhiteSpace(variableName))
            {
                variableName = interaction.TypeReference.Element!.Name.ToLocalVariableName() + "Result";
            }

            csharpMapping.SetFromReplacement(interaction, variableName);
            csharpMapping.SetToReplacement(interaction, variableName);

            var invokeCall = new CSharpInvocationStatement($"await _sender.InvokeAsync<{template.GetTypeName(response.AsTypeReference())}>")
                .AddArgument(requestName);
            if (cancellationToken != null)
            {
                invokeCall.AddArgument(cancellationToken.Name);
            }

            statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(variableName), invokeCall));
            method.TrackedEntities().Add(response.Id, new EntityDetails((IElement)response, variableName, null, false));
        }
        else
        {
            var invokeCall = new CSharpInvocationStatement("await _sender.InvokeAsync")
                .AddArgument(requestName);
            if (cancellationToken != null)
            {
                invokeCall.AddArgument(cancellationToken.Name);
            }

            statements.Add(invokeCall);
        }

        method.AddStatements(ExecutionPhases.BusinessLogic, statements);
    }
}

internal class CommandQueryMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public CommandQueryMappingResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping? ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.Model.SpecializationType == "Command" || mappingModel.Model.SpecializationType == "Query")
        {
            return new ConstructorMapping(mappingModel, _template);
        }

        if (mappingModel.Model.TypeReference?.Element?.SpecializationType == "DTO")
        {
            return new ObjectInitializationMapping(mappingModel, _template);
        }

        return null;
    }
}

internal record StaticMetadata(string Id) : IMetadataModel;
