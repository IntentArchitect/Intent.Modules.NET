using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AspNetCore.IntegrationTests.CRUD.FactoryExtensions.TestImplementations
{
    internal class CrudMap
    {
        public CrudMap(IServiceProxyModel proxy, ClassModel entity, List<Dependency> dependencies, IHttpEndpointModel create, IHttpEndpointModel? update, IHttpEndpointModel? delete, IHttpEndpointModel getById, IHttpEndpointModel? getAll, string? responseDtoIdField = null, ClassModel? owningAggregate = null)
        {
            Proxy = proxy;
            Entity = entity;
            Dependencies = dependencies;
            Create = create;
            Update = update;
            Delete = delete;
            GetById = getById;
            GetAll = getAll;
            ResponseDtoIdField = responseDtoIdField;
            OwningAggregate = owningAggregate;
        }

        public IServiceProxyModel Proxy { get; }
        public ClassModel Entity { get; }
        public ClassModel? OwningAggregate { get; }
        public IHttpEndpointModel Create { get; }
        public IHttpEndpointModel? Update { get; }
        public IHttpEndpointModel? Delete { get; }
        public IHttpEndpointModel GetById { get; }
        public IHttpEndpointModel? GetAll { get; }
        public List<Dependency> Dependencies { get; set; }
        public string? ResponseDtoIdField { get; }
    }

    public class Dependency
    {
        public Dependency(string entityName, List<IElementToElementMappedEnd> mappings)
        {
            EntityName = entityName;
            Mappings = mappings;
            CrudMap = null!;
        }

        public string EntityName { get; set; }
        public List<IElementToElementMappedEnd> Mappings { get; set; }
        internal CrudMap? CrudMap { get; set; }
    }

}