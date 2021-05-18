using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    class UpdateImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;
        private readonly IApplication _application;
        private readonly IMetadataManager _metadataManager;
        private ClassModel _foundEntity;
        private RequiredService _repository;
        private DTOFieldModel _idProperty;

        public UpdateImplementationStrategy(CommandHandlerTemplate template, IApplication application, IMetadataManager metadataManager)
        {
            _template = template;
            _application = application;
            _metadataManager = metadataManager;
        }

        public bool IsMatch()
        {
            var matchingEntities = _metadataManager.Domain(_application).GetClassModels().Where(x => new[]
            {
                $"update{x.Name.ToLower()}",
                $"update{x.Name.ToLower()}details",
                $"edit{x.Name.ToLower()}",
                $"edit{x.Name.ToLower()}details",
            }.Contains(_template.Model.Name.ToLower().RemoveSuffix("command"))).ToList();


            if (matchingEntities.Count() != 1)
            {
                return false;
            }

            _foundEntity = matchingEntities.Single();

            _idProperty = _template.Model.Properties.FirstOrDefault(p =>
                string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(p.Name, $"{_foundEntity.Name}Id", StringComparison.InvariantCultureIgnoreCase));
            if (_idProperty == null)
            {
                return false;
            }

            var repositoryInterface = _template.GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, _foundEntity, new TemplateDiscoveryOptions() { ThrowIfNotFound = false });
            if (repositoryInterface == null)
            {
                return false;
            }
            _repository = new RequiredService(type: repositoryInterface, name: repositoryInterface.Substring(1).ToCamelCase());
            return true;

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
            return $@"var existing{_foundEntity.Name} = await {_repository.FieldName}.FindByIdAsync(request.{_idProperty.Name.ToPascalCase()}, cancellationToken);
                {GetPropertyAssignments(_foundEntity, _template.Model)}
                return Unit.Value;";
        }

        private string GetPropertyAssignments(ClassModel domainModel, CommandModel command)
        {
            var sb = new StringBuilder();
            foreach (var property in command.Properties.Where(x => !x.Equals(_idProperty)))
            {
                var attribute = domainModel.Attributes.FirstOrDefault(p => p.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                if (attribute == null)
                {
                    sb.AppendLine($"                #warning No matching field found for {property.Name}");
                    continue;
                }
                if (attribute.Type.Element.Id != property.TypeReference.Element.Id)
                {
                    sb.AppendLine($"                #warning No matching type for Domain: {attribute.Name} and DTO: {property.Name}");
                    continue;
                }
                sb.AppendLine($"                existing{_foundEntity.Name}.{attribute.Name.ToPascalCase()} = request.{property.Name.ToPascalCase()};");
            }

            return sb.ToString().Trim();
        }
    }
}