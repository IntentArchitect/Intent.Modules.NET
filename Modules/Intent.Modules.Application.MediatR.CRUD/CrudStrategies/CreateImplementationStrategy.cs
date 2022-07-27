using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    class CreateImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;
        private readonly IApplication _application;
        private readonly IMetadataManager _metadataManager;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public CreateImplementationStrategy(CommandHandlerTemplate template, IApplication application,
            IMetadataManager metadataManager)
        {
            _template = template;
            _application = application;
            _metadataManager = metadataManager;
            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }

        public bool IsMatch()
        {
            return _matchingElementDetails.Value.IsMatch;
        }

        public IEnumerable<RequiredService> GetRequiredServices()
        {
            return new[]
            {
                _matchingElementDetails.Value.Repository
            };
        }

        public string GetImplementation()
        {
            var foundEntity = _matchingElementDetails.Value.FoundEntity;
            var repository = _matchingElementDetails.Value.Repository;
            
            var entityName = _template.GetDomainEntityName(foundEntity);
            var impl = $@"var new{foundEntity.Name} = new {entityName ?? foundEntity.Name}
                {{
{GetPropertyAssignments(foundEntity, _template.Model)}
                }};
                
                {repository.FieldName}.Add(new{foundEntity.Name});";

            if (_template.Model.TypeReference.Element != null)
            {
                impl += $@"
                await {repository.FieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);
                return new{foundEntity.Name}.{foundEntity.Attributes.FirstOrDefault(x => x.HasPrimaryKey())?.Name ?? "Id"};";
            }
            else
            {
                impl += @"
                return Unit.Value;";
            }

            return impl;
        }

        private StrategyData GetMatchingElementDetails()
        {
            var matchingEntities = _metadataManager.Domain(_application)
                .GetClassModels().Where(x => new[]
                {
                    $"add{x.Name.ToLower()}",
                    $"addnew{x.Name.ToLower()}",
                    $"create{x.Name.ToLower()}",
                    $"createnew{x.Name.ToLower()}",
                }.Contains(_template.Model.Name.ToLower().RemoveSuffix("command")))
                .ToList();

            if (matchingEntities.Count() == 1)
            {
                var foundEntity = matchingEntities.Single();
                var repositoryInterface = _template.GetEntityRepositoryInterfaceName(foundEntity);
                if (repositoryInterface == null)
                {
                    return NoMatch;
                }

                var repository = new RequiredService(type: repositoryInterface,
                    name: repositoryInterface.Substring(1).ToCamelCase());
                
                return new StrategyData(true, foundEntity, repository);
            }

            return NoMatch;
        }

        private string GetPropertyAssignments(ClassModel domainModel, CommandModel command)
        {
            var sb = new StringBuilder();
            foreach (var property in command.Properties)
            {
                var attribute = property.Mapping?.Element != null
                    ? property.Mapping.Element.AsAttributeModel()
                    : domainModel.Attributes.FirstOrDefault(p =>
                        p.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                if (attribute == null)
                {
                    sb.AppendLine($"                    #warning No matching field found for {property.Name}");
                    continue;
                }

                if (attribute.Type.Element.Id != property.TypeReference.Element.Id)
                {
                    sb.AppendLine(
                        $"                    #warning No matching type for Domain: {attribute.Name} and DTO: {property.Name}");
                    continue;
                }

                sb.AppendLine(
                    $"                    {attribute.Name.ToPascalCase()} = request.{property.Name.ToPascalCase()},");
            }

            return sb.ToString().Trim();
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null);
        private class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, RequiredService repository)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                Repository = repository;
            }
            
            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public RequiredService Repository { get; }
        }
    }
}