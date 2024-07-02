using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainApi = Intent.Modelers.Domain.Api;

namespace Intent.Modules.AspNetCore.IntegrationTests.CRUD.FactoryExtensions.TestImplementations
{
    internal class CrudMapHelper
    {
        internal static List<CrudMap> LoadCrudMaps(ICSharpFileBuilderTemplate template, IMetadataManager _metadataManager, IApplication application, out List<CrudMap> cantImplement)
        {
            var crudtests = new List<CrudMap>();
            var testableProxies = _metadataManager.GetServicesAsProxyModels(application);
            foreach (var testableProxy in testableProxies)
            {
                var crudMaps = ExtractCrudMap(template, testableProxy);
                if (crudMaps.Any())
                {
                    crudtests.AddRange(crudMaps);
                }
            }
            //This ensures that the Dependencies list is ordered e.g. Customer -> Order (depends on Customer) -> OrderItem (depends on Order)
            DependencySortingHelper.SortDependencies(crudtests);

            cantImplement = crudtests.Where(ct => ct.Dependencies.Any(d => d.CrudMap is null)).ToList();
            //Removing implementations which can't work because they have dependencies on Enities which don't have CRUD Services.
            var result = crudtests.Except(cantImplement).ToList();
            return result;
        }

        private static IEnumerable<CrudMap> ExtractCrudMap(ICSharpFileBuilderTemplate template, IServiceProxyModel testableProxy)
        {
            var operations = testableProxy.GetMappedEndpoints();
            var creates = operations.Where(o => o.Name.StartsWith("Create", StringComparison.OrdinalIgnoreCase) && o.ReturnType?.Element != null ).ToList();
            if (!creates.Any())
                yield break;

            foreach (var create in creates)
            {
                string potentialEntityName = create.Name.Substring("Create".Length);
                var getById = operations.FirstOrDefault(o => o.Name.StartsWith($"Get{potentialEntityName}ById", StringComparison.OrdinalIgnoreCase));
                if (getById != null)
                {
                    if (ValidateCreateReturnType(template, create, potentialEntityName, out var responseDtoIdField))
                    {
                        var result = CreateCrudMap(testableProxy, potentialEntityName, "Get", responseDtoIdField);
                        if (result != null)
                            yield return result;
                    }
                }
                else
                {
                    var findById = operations.FirstOrDefault(o => o.Name.StartsWith($"Find{potentialEntityName}ById", StringComparison.OrdinalIgnoreCase));
                    if (findById != null)
                    {
                        if (ValidateCreateReturnType(template, create, potentialEntityName, out var responseDtoIdField))
                        {
                            var result = CreateCrudMap(testableProxy, potentialEntityName, "Find", responseDtoIdField);
                            if (result != null)
                                yield return result;
                        }
                    }
                }
            }
        }

        private static bool ValidateCreateReturnType(ICSharpFileBuilderTemplate template, IHttpEndpointModel create, string entityName, out string? idFieldOnDto)
        {
            idFieldOnDto = null;
            if (create.ReturnType?.Element == null)
                return false;
            if (template.GetTypeInfo(create.ReturnType).IsPrimitive || create.ReturnType?.Element?.Name == "string")
                return true;
            var dto = create.ReturnType?.Element?.AsDTOModel();
            if (dto != null)
            {
                if (dto.Mapping != null && dto.Mapping.Element.Name == entityName)
                {
                    var pk = dto.Fields.FirstOrDefault( f => f.Mapping.Element.HasStereotype("Primary Key"));
                    if (pk != null)
                    {
                        idFieldOnDto = pk.Name;
                        return true;
                    }
                }
            }

            return false;
        }

        private static IElementToElementMapping? GetCreateMapping(IElement mappedElement)
        {
            return mappedElement.AssociatedElements.FirstOrDefault(a => a.Association.SpecializationTypeId == "7a3f0474-3cf8-4249-baac-8c07c49465e0")//Create Entity Association
                   ?.Mappings.FirstOrDefault(mappingModel => mappingModel.TypeId == "5f172141-fdba-426b-980e-163e782ff53e");// Command to Class Creation Mapping;
        }


        private static CrudMap? CreateCrudMap(IServiceProxyModel testableProxy, string entityName, string getPrefix, string? responseDtoIdField)
        {
            var operations = testableProxy.GetMappedEndpoints();
            var createOperation = operations.First(o => string.Compare(o.Name, $"Create{entityName}", ignoreCase: true) == 0);

            var mapping = GetCreateMapping(createOperation.InternalElement);
            if (mapping == null) // We only doing Test for services on new mapping system
            {
                return null;
            }

            var entity = mapping.TargetElement.AsClassModel();
            if (entity == null) return null;

            var owningAggregate = GetOwningAggregate(entity);

            var dependencies = DetermineDependencies(createOperation, mapping);
            if (entity.Attributes.Count(a => a.HasStereotype("Primary Key")) != 1) return null;

            var createOperaion = operations.FirstOrDefault(o => string.Compare(o.Name, $"Create{entityName}", ignoreCase: true) == 0 && ExpectedCreateParameters(o));
            if (createOperaion == null) return null;
            var getByIdOperation = operations.FirstOrDefault(o => string.Compare(o.Name, $"{getPrefix}{entityName}ById", ignoreCase: true) == 0 && ExpectedGetByIdParameters(o, entity, owningAggregate));
            if (getByIdOperation == null) return null;

            return new CrudMap(
                testableProxy,
                entity,
                dependencies,
                createOperaion, 
                operations.FirstOrDefault(o => string.Compare(o.Name, $"Update{entityName}", ignoreCase: true) == 0 && ExpectedUpdateParameters(o, entity)),
                operations.FirstOrDefault(o => string.Compare(o.Name, $"Delete{entityName}", ignoreCase: true) == 0 && ExpectedDeleteParameters(o, entity, owningAggregate)),
                getByIdOperation,
                operations.FirstOrDefault(o => string.Compare(o.Name, $"{getPrefix}{entityName.Pluralize(false)}", ignoreCase: true) == 0 && ExpectedGetAllParameters(o, owningAggregate)),
                responseDtoIdField,
                owningAggregate
                );
        }

        private static bool ExpectedGetAllParameters(IHttpEndpointModel o, ClassModel? owningAggregate)
        {
            if (owningAggregate == null)
                return !o.Inputs.Any();
            return o.Inputs.Count == 1 && 
                o.Inputs.First().TypeReference?.Element?.Id == owningAggregate.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference?.Element?.Id;
        }

        private static bool ExpectedGetByIdParameters(IHttpEndpointModel o, ClassModel entity, ClassModel? owningAggregate)
        {
            if (owningAggregate == null)
                return o.Inputs.Count == 1 && 
                    o.Inputs.First().TypeReference?.Element?.Id == entity.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference?.Element?.Id;
            return o.Inputs.Count == 2 && 
                o.Inputs.First().TypeReference?.Element?.Id == owningAggregate.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference?.Element?.Id &&
                o.Inputs.ElementAt(1).TypeReference?.Element?.Id == entity.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference?.Element?.Id ;
        }

        private static bool ExpectedDeleteParameters(IHttpEndpointModel o, ClassModel entity, ClassModel? owningAggregate)
        {
            if (owningAggregate == null)
                return o.Inputs.Count == 1 &&
                    o.Inputs.First().TypeReference?.Element?.Id == entity.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference?.Element?.Id;
            return o.Inputs.Count == 2 &&
                o.Inputs.First().TypeReference?.Element?.Id == owningAggregate.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference?.Element?.Id &&
                o.Inputs.ElementAt(1).TypeReference?.Element?.Id == entity.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference?.Element?.Id;
        }

        private static bool ExpectedUpdateParameters(IHttpEndpointModel o, ClassModel entity)
        {
            return o.Inputs.Count == 2 &&                
                o.Inputs.First().TypeReference?.Element?.Id == entity.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference?.Element?.Id;
        }

        private static bool ExpectedCreateParameters(IHttpEndpointModel o)
        {
            return o.Inputs.Count == 1;
        }

        private static ClassModel? GetOwningAggregate(ClassModel entity)
        {
            if (!entity.IsAggregateRoot())
            {
                var aggregateAssociations = entity.AssociatedClasses
                    .Where(p => p.TypeReference?.Element?.AsClassModel()?.IsAggregateRoot() == true &&
                                p.IsSourceEnd() && !p.IsCollection && !p.IsNullable)
                    .Distinct()
                    .ToList();
                if (aggregateAssociations.Count != 1)
                {
                    return null;
                }
                return aggregateAssociations.Single().Class;
            }
            return null;
        }

        private static List<Dependency> DetermineDependencies(IHttpEndpointModel createOperation, IElementToElementMapping mapping)
        {
            var deDupe = new Dictionary<string, List<IElementToElementMappedEnd>>();
            foreach (var mappedEnd in mapping.MappedEnds.Where(me => me.MappingType == "Data Mapping"))
            {
                var attributeModel = (mappedEnd.TargetElement as IElement)?.AsAttributeModel();
                if (attributeModel == null && mappedEnd.TargetElement as IElement != null) 
                {
                    var parameterModel = DomainApi.ParameterModelExtensions.AsParameterModel(mappedEnd.TargetElement as IElement);
                    if (parameterModel != null && parameterModel.InternalElement.MappedElement != null && parameterModel.InternalElement.MappedElement.Element.IsAttributeModel())
                    {
                        attributeModel = parameterModel.InternalElement.MappedElement.Element.AsAttributeModel();
                    }
                }
                if (attributeModel is not null && attributeModel.HasStereotype("Foreign Key"))
                {
                    var ae = attributeModel.GetStereotype("Foreign Key")?.GetProperty<IElement>("Association")?.AsAssociationTargetEndModel();
                    if (ae != null)
                    {
                        var cm = ae.Class;
                        if (ae.Class.Id == attributeModel.Class.Id)
                        {
                            cm = ae.OtherEnd().Class;
                        }
                        if (!deDupe.TryGetValue(cm.Name, out var mappings))
                        {
                            mappings = new List<IElementToElementMappedEnd>();
                            deDupe.Add(cm.Name, mappings);
                        }
                        mappings.Add(mappedEnd);
                    }
                }
            }
            return deDupe.Select(x => new Dependency(x.Key, x.Value)).OrderBy(d => d.EntityName).ToList();
        }
    }
}
