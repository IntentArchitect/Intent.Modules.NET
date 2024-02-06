using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.IntegrationTests.CRUD.FactoryExtensions
{
    internal class CrudMapHelper
    {
        internal static List<CrudMap> LoadCrudMaps(IMetadataManager _metadataManager, IApplication application)
        {
            var crudtests = new List<CrudMap>();
            var testableProxies = _metadataManager.GetServicesAsProxyModels(application);
            foreach (var testableProxy in testableProxies)
            {
                var crudMaps = ExtractCrudMap(testableProxy);
                if (crudMaps.Any())
                {
                    crudtests.AddRange(crudMaps);
                }
            }
            //This ensures that the Dependencies list is ordered e.g. Customer -> Order (depends on Customer) -> OrderItem (depends on Order)
            DependencySortingHelper.SortDependencies(crudtests);
            return crudtests;
        }

        private static IEnumerable<CrudMap> ExtractCrudMap(IServiceProxyModel testableProxy)
        {
            var operations = testableProxy.GetMappedEndpoints();
            var creates = operations.Where(o => o.Name.StartsWith("Create", StringComparison.OrdinalIgnoreCase)).ToList();
            if (!creates.Any())
                yield break;

            foreach (var create in creates)
            {
                string potentialEntityName = create.Name.Substring("Create".Length);
                var getById = operations.FirstOrDefault(o => o.Name.StartsWith($"Get{potentialEntityName}ById", StringComparison.OrdinalIgnoreCase));
                if (getById != null)
                {
                    yield return CreateCrudMap(testableProxy, potentialEntityName, "Get");
                }
                else
                {
                    var findById = operations.FirstOrDefault(o => o.Name.StartsWith($"Find{potentialEntityName}ById", StringComparison.OrdinalIgnoreCase));
                    if (findById != null)
                    {
                        yield return CreateCrudMap(testableProxy, potentialEntityName, "Find");
                    }
                }
            }
        }

        private static IElementToElementMapping? GetCreateMapping(IElement mappedElement)
        {
            return mappedElement.AssociatedElements.FirstOrDefault(a => a.Association.SpecializationTypeId == "7a3f0474-3cf8-4249-baac-8c07c49465e0")//Create Entity Association
                   ?.Mappings.FirstOrDefault(mappingModel => mappingModel.TypeId == "5f172141-fdba-426b-980e-163e782ff53e");// Command to Class Creation Mapping;
        }


        private static CrudMap CreateCrudMap(IServiceProxyModel testableProxy, string entityName, string getPrefix)
        {
            var operations = testableProxy.GetMappedEndpoints();
            var dependencies = DetermineDependencies(operations.First(o => string.Compare(o.Name, $"Create{entityName}", ignoreCase: true) == 0), out var entity);
            return new CrudMap(
                testableProxy,
                entity,
                dependencies,
                operations.First(o => string.Compare(o.Name, $"Create{entityName}", ignoreCase: true) == 0),
                operations.FirstOrDefault(o => string.Compare(o.Name, $"Update{entityName}", ignoreCase: true) == 0),
                operations.FirstOrDefault(o => string.Compare(o.Name, $"Delete{entityName}", ignoreCase: true) == 0),
                operations.First(o => string.Compare(o.Name, $"{getPrefix}{entityName}ById", ignoreCase: true) == 0),
                operations.FirstOrDefault(o => string.Compare(o.Name, $"{getPrefix}{entityName.Pluralize(false)}", ignoreCase: true) == 0)
                );
        }

        private static List<Dependency> DetermineDependencies(IHttpEndpointModel createOperation, out ClassModel? entity)
        {
            entity = null;
            var mapping = GetCreateMapping(createOperation.InternalElement);
            if (mapping != null)
            {
                entity = mapping.TargetElement.AsClassModel();
                var deDupe = new Dictionary<string, List<IElementToElementMappedEnd>>();
                foreach (var mappedEnd in mapping.MappedEnds.Where(me => me.MappingType == "Data Mapping"))
                {
                    var attributeModel = (mappedEnd.TargetElement as IElement)?.AsAttributeModel();
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
            return new List<Dependency>();
        }


    }
}
