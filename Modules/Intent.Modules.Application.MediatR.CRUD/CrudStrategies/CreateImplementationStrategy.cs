using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
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
        private ClassModel _foundEntity;
        private RequiredService _repository;

        public CreateImplementationStrategy(CommandHandlerTemplate template, IApplication application, IMetadataManager metadataManager)
        {
            _template = template;
            _application = application;
            _metadataManager = metadataManager;
        }

        public bool IsMatch()
        {
            var matchingEntities = _metadataManager.Domain(_application).GetClassModels().Where(x => new[]
            {
                $"add{x.Name.ToLower()}",
                $"addnew{x.Name.ToLower()}",
                $"create{x.Name.ToLower()}",
                $"createnew{x.Name.ToLower()}",
            }.Contains(_template.Model.Name.ToLower().RemoveSuffix("command"))).ToList();

            if (matchingEntities.Count() == 1)
            {
                _foundEntity = matchingEntities.Single();
                var repositoryInterface = _template.GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, _foundEntity, new TemplateDiscoveryOptions() { ThrowIfNotFound = false });
                if (repositoryInterface == null)
                {
                    return false;
                }
                _repository = new RequiredService(type: repositoryInterface, name: repositoryInterface.Substring(1).ToCamelCase());
                return true;
            }

            return false;
        }

        public IEnumerable<RequiredService> GetRequiredServices()
        {
            return new[]
            {
                _repository
            };
        }

        public string GetImplementation()
        {
            var entityName = _template.GetTypeName("Domain.Entities", _foundEntity, new TemplateDiscoveryOptions() { ThrowIfNotFound = false });
            var impl = $@"var new{_foundEntity.Name} = new {entityName ?? _foundEntity.Name}
                {{
{GetPropertyAssignments(_foundEntity, _template.Model)}
                }};
                
                {_repository.FieldName}.Add(new{_foundEntity.Name});";

            if (_template.Model.TypeReference.Element != null)
            {
                impl += $@"
                await {_repository.FieldName}.SaveChangesAsync(cancellationToken);
                return new{_foundEntity.Name}.Id;";
            }
            else
            {
                impl += @"
                return Unit.Value;";
            }

            return impl;
        }

        private string GetPropertyAssignments(ClassModel domainModel, CommandModel command)
        {
            var sb = new StringBuilder();
            foreach (var property in command.Properties)
            {
                var attribute = property.Mapping?.Element != null
                    ? property.Mapping.Element.AsAttributeModel()
                    : domainModel.Attributes.FirstOrDefault(p => p.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                if (attribute == null)
                {
                    sb.AppendLine($"                    #warning No matching field found for {property.Name}");
                    continue;
                }
                if (attribute.Type.Element.Id != property.TypeReference.Element.Id)
                {
                    sb.AppendLine($"                    #warning No matching type for Domain: {attribute.Name} and DTO: {property.Name}");
                    continue;
                }
                sb.AppendLine($"                    {attribute.Name.ToPascalCase()} = request.{property.Name.ToPascalCase()},");
            }

            return sb.ToString().Trim();
        }
    }
}