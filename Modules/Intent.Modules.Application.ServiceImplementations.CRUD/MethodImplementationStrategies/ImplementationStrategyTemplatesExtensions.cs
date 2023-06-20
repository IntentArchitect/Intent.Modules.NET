using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies;

public static class ImplementationStrategyTemplatesExtensions
{
    public static string GetNotFoundExceptionName(this ICSharpTemplate template)
    {
        var exceptionName = template
            .GetTypeName("Domain.NotFoundException", TemplateDiscoveryOptions.DoNotThrow);
        return exceptionName;
    }
    
    public record EntityIdAttribute(string IdName);

    public static EntityIdAttribute GetEntityIdAttribute(this ClassModel entity)
    {
        var explicitKeyField = GetExplicitEntityIdField(entity);
        if (explicitKeyField != null) return explicitKeyField;
        return new EntityIdAttribute("Id");

        EntityIdAttribute GetExplicitEntityIdField(ClassModel entity)
        {
            return entity.Attributes.Where(p => p.HasPrimaryKey()).Select(s => new EntityIdAttribute(s.Name)).FirstOrDefault();
        }
    }

    public static DTOFieldModel GetEntityIdField(this IEnumerable<DTOFieldModel> properties, ClassModel entity)
    {
        var explicitKeyField = GetExplicitEntityIdField(properties, entity);
        if (explicitKeyField != null) return explicitKeyField;
        var implicitKeyField = GetImplicitEntityIdField(properties, entity);
        return implicitKeyField;
        
        DTOFieldModel GetExplicitEntityIdField(IEnumerable<DTOFieldModel> properties, ClassModel entity)
        {
            var idField = properties
                .FirstOrDefault(field =>
                {
                    var attr = field.Mapping?.Element.AsAttributeModel();
                    if (attr == null)
                    {
                        return false;
                    }

                    return attr.HasPrimaryKey();
                });
            return idField;
        }

        DTOFieldModel GetImplicitEntityIdField(IEnumerable<DTOFieldModel> properties, ClassModel entity)
        {
            var idField = properties.FirstOrDefault(p =>
                string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(p.Name, $"{entity.Name}Id", StringComparison.InvariantCultureIgnoreCase));
            return idField;
        }
    }

    public record ModelPair(DTOModel DtoModel, ClassModel DomainModel);

    public static ModelPair GetCreateModelPair(this OperationModel operationModel)
    {
        var dtoModel = operationModel.Parameters.First().TypeReference.Element.AsDTOModel();
        var domainModel = dtoModel.Mapping.Element.AsClassModel();
        return new ModelPair(dtoModel, domainModel);
    }

    public static ModelPair GetUpdateModelPair(this OperationModel operationModel)
    {
        var dtoModel = operationModel.Parameters.FirstOrDefault(x => x.TypeReference.Element.IsDTOModel()).TypeReference.Element.AsDTOModel();
        var domainModel = dtoModel.Mapping.Element.AsClassModel();
        return new ModelPair(dtoModel, domainModel);
    }

    public static ModelPair GetDeleteModelPair(this OperationModel operationModel)
    {
        var dtoModel = operationModel.TypeReference.Element.AsDTOModel();
        var domainModel = dtoModel.Mapping.Element.AsClassModel();
        return new ModelPair(dtoModel, domainModel);
    }

    public static ClassModel GetLegacyDeleteDomainModel(this OperationModel operationModel, IApplication application)
    {
        var domainModel = GetDomainForService(operationModel.ParentService, application);
        return domainModel;
    }
    
    private static ClassModel GetDomainForService(ServiceModel service, IApplication application)
    {
        var serviceIdentifier = service.Name.RemoveSuffix("RestController", "Controller", "Service", "Manager").ToLower();
        var entities = application.MetadataManager.Domain(application).GetClassModels();
        return entities.SingleOrDefault(e => e.Name.Equals(serviceIdentifier, StringComparison.InvariantCultureIgnoreCase) ||
                                             e.Name.Pluralize().Equals(serviceIdentifier, StringComparison.InvariantCultureIgnoreCase));
    }
}