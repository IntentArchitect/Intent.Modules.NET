﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Exceptions;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.InteractionStrategies
{
    public class SendOnMediatorInteractionStrategy : IInteractionStrategy
    {
        private CSharpClassMappingManager _csharpMapping;

        public bool IsMatch(IElement interaction)
        {
            return interaction.IsPerformInvocationTargetEndModel() && (interaction.TypeReference.Element.IsCommandModel() || interaction.TypeReference.Element.IsQueryModel());
        }

        public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
        {
            var interaction = (IAssociationEnd)interactionElement;
            var handlerClass = method.Class;
            var template = (ICSharpFileBuilderTemplate)handlerClass.File.Template;
            template.AddTypeSource(TemplateRoles.Application.Query);
            template.AddTypeSource(TemplateRoles.Application.Command);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            _csharpMapping = method.GetMappingManager();
            _csharpMapping.AddMappingResolver(new CommandQueryMappingResolver(template));
            var @class = handlerClass;
            var ctor = @class.Constructors.First();
            if (ctor.Parameters.All(x => x.Type != template.UseType("MediatR.ISender")))
            {
                ctor.AddParameter(template.UseType("MediatR.ISender"), "mediator", param =>
                {
                    param.IntroduceReadonlyField((_, s) => s.ThrowArgumentNullException());
                });
            }

            var requestName = interaction.TypeReference.Element.IsCommandModel() ? "command" : "query";// ? interaction.TypeReference.Element.Name.ToLocalVariableName();

            var statements = new List<CSharpStatement>();
            statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(requestName), _csharpMapping.GenerateCreationStatement(interaction.Mappings.Single())).WithSemicolon().SeparatedFromPrevious());
            var response = interaction.TypeReference.Element?.TypeReference?.Element;
            if (response != null && interaction.TypeReference.Element.IsQueryModel())
            {
                var responseStaticElementId = "9acdd519-a45a-469d-89f1-00896a31ca61";
                _csharpMapping.SetFromReplacement(interaction, response.Name.ToLocalVariableName());
                _csharpMapping.SetToReplacement(interaction, response.Name.ToLocalVariableName());
                _csharpMapping.SetFromReplacement(new StaticMetadata(responseStaticElementId), "");
                _csharpMapping.SetToReplacement(new StaticMetadata(responseStaticElementId), "");
                statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(response.Name.ToLocalVariableName()), new CSharpInvocationStatement("await _mediator.Send").AddArgument(requestName).AddArgument("cancellationToken")));
                method.TrackedEntities().Add(response.Id, new EntityDetails((IElement)response, response.Name.ToLocalVariableName(), null, false));
            }
            else
            {
                statements.Add(new CSharpInvocationStatement("await _mediator.Send").AddArgument(requestName).AddArgument("cancellationToken"));
            }
            method.AddStatements(ExecutionPhases.BusinessLogic, statements);
        }
    }
}

public class CommandQueryMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public CommandQueryMappingResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.Model.SpecializationType == "Command" || mappingModel.Model.SpecializationType == "Query")
        {
            return new ConstructorMapping(mappingModel, _template);
        }
        if (mappingModel.Model.TypeReference?.Element?.SpecializationType == "DTO")
        {
            return new ObjectInitializationMapping(mappingModel, _template);
        }
        //if (mappingModel.Model.TypeReference?.Element?.IsTypeDefinitionModel() == true
        //    || mappingModel.Model.TypeReference?.Element?.IsEnumModel() == true)
        //{
        //    return new TypeConvertingCSharpMapping(mappingModel, _template);
        //}
        return null;
    }
}

public record StaticMetadata(string id) : IMetadataModel
{
    public string Id { get; } = id;
}
