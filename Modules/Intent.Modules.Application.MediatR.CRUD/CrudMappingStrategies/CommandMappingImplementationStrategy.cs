using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.MediatR.CRUD.CrudMappingStrategies
{
    public class CommandMappingImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly ICSharpFileBuilderTemplate _template;
        private readonly CommandModel _model;


        public CommandMappingImplementationStrategy(CSharpTemplateBase<CommandModel> template)
        {
            _template = (ICSharpFileBuilderTemplate)template;
            _model = template.Model;
        }

        public bool IsMatch()
        {
            return _model.HasDomainInteractions();
        }

        public void ApplyStrategy()
        {

            _template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateRoles.Domain.ValueObject);

            var @class = _template.CSharpFile.Classes.First();
            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
            var csharpMapping = new CSharpClassMappingManager(_template);
            csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(_template));
            var domainInteractionManager = new DomainInteractionsManager(_template, csharpMapping);

            csharpMapping.SetFromReplacement(_model, "request");
            handleMethod.AddMetadata("mapping-manager", csharpMapping);

            handleMethod.AddStatements(domainInteractionManager.CreateInteractionStatements(_model));

            if (_model.TypeReference.Element != null)
            {
                var returnStatement = domainInteractionManager.GetReturnStatements(_model.TypeReference);
                handleMethod.AddStatements(returnStatement);
            }
        }
    }
}