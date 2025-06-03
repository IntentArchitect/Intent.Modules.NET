using System;
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
        private ICSharpFileBuilderTemplate _template;
        private CSharpClassMappingManager _csharpMapping;

        public bool IsMatch(IAssociationEnd interaction)
        {
            return interaction.IsServiceInvocationTargetEndModel() && (interaction.TypeReference.Element.IsCommandModel() || interaction.TypeReference.Element.IsQueryModel());
        }

        public IEnumerable<CSharpStatement> GetStatements(CSharpClass handlerClass, IAssociationEnd interaction, CSharpClassMappingManager csharpMapping)
        {
            _template = (ICSharpFileBuilderTemplate)handlerClass.File.Template;
            _csharpMapping = csharpMapping;
            var @class = handlerClass;
            var ctor = @class.Constructors.First();
            var template = handlerClass.File.Template;
            if (ctor.Parameters.All(x => x.Type != template.UseType("MediatR.ISender")))
            {
                ctor.AddParameter(template.UseType("MediatR.ISender"), "mediator", param =>
                {
                    param.IntroduceReadonlyField((_, s) => s.ThrowArgumentNullException());
                });
            }

            var requestName = interaction.TypeReference.Element.Name.ToLocalVariableName();
            var statements = new List<CSharpStatement>();
            statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(requestName), _csharpMapping.GenerateCreationStatement(interaction.Mappings.Single())).WithSemicolon().SeparatedFromPrevious());
            var response = interaction.TypeReference.Element?.TypeReference?.Element;
            if (response != null)
            {
                statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(response.Name.ToLocalVariableName()), new CSharpInvocationStatement("await _mediator.Send").AddArgument(requestName).AddArgument("cancellationToken")));
            }
            else
            {
                statements.Add(new CSharpInvocationStatement("await _mediator.Send").AddArgument(requestName).AddArgument("cancellationToken"));
            }
            return statements;
        }
    }
}
