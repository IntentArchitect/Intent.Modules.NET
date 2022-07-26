using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.Decorators;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies
{
    public class GetByIdImplementationStrategy : IImplementationStrategy
    {
        private readonly CrudConventionDecorator _decorator;

        public GetByIdImplementationStrategy(CrudConventionDecorator decorator)
        {
            _decorator = decorator;
        }

        public bool Match(ClassModel domainModel, OperationModel operationModel)
        {
            var entityName = domainModel.Name.ToLower();
            var lowerOperationName = operationModel.Name.ToLower();

            if (operationModel.Parameters.Count() != 1)
            {
                return false;
            }

            if (!operationModel.Parameters.Any(p =>
                    string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(p.Name, $"{entityName}Id", StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }

            if (operationModel?.TypeReference?.IsCollection ?? false)
            {
                return false;
            }

            if (!_decorator.HasEntityRepositoryInterfaceName(domainModel)
                || !_decorator.HasDtoName(operationModel.TypeReference?.Element))
            {
                return false;
            }

            return new[]
                {
                    "get",
                    $"get{entityName}",
                    "find",
                    "findbyid",
                    $"find{entityName}",
                    $"find{entityName}byid",
                    entityName
                }
                .Contains(lowerOperationName);
        }

        public string GetImplementation(ClassModel domainModel, OperationModel operationModel)
        {
            return
                $@"var element ={(operationModel.IsAsync() ? " await" : "")} {domainModel.Name.ToPrivateMemberName()}Repository.FindById{(operationModel.IsAsync() ? "Async" : "")}({operationModel.Parameters.First().Name.ToCamelCase()});
            return element.MapTo{_decorator.GetDtoName(operationModel.TypeReference.Element)}(_mapper);";
        }

        public IEnumerable<ConstructorParameter> GetRequiredServices(ClassModel domainModel)
        {
            var repo = _decorator.GetEntityRepositoryInterfaceName(domainModel);
            return new[]
            {
                new ConstructorParameter(repo, repo.Substring(1).ToCamelCase()),
                new ConstructorParameter(_decorator.Template.UseType("AutoMapper.IMapper"), "mapper"),
            };
        }
    }
}