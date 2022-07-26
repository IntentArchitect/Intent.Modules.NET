using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.Decorators;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies
{
    public class DeleteImplementationStrategy : IImplementationStrategy
    {
        private readonly CrudConventionDecorator _decorator;

        public DeleteImplementationStrategy(CrudConventionDecorator decorator)
        {
            _decorator = decorator;
        }

        public bool Match(ClassModel domainModel, OperationModel operationModel)
        {
            var lowerDomainName = domainModel.Name.ToLower();
            var lowerOperationName = operationModel.Name.ToLower();
            if (operationModel.Parameters.Count() != 1)
            {
                return false;
            }

            if (!operationModel.Parameters
                    .Any(p => string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase)
                              || string.Equals(p.Name, $"{lowerDomainName}Id",
                                  StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }

            if (operationModel.TypeReference.Element != null)
            {
                return false;
            }
            
            if (!_decorator.HasEntityRepositoryInterfaceName(domainModel))
            {
                return false;
            }

            return new[]
                {
                    "delete",
                    $"delete{lowerDomainName}"
                }
                .Contains(lowerOperationName);
        }

        public string GetImplementation(ClassModel domainModel, OperationModel operationModel)
        {
            return
                $@"var existing{domainModel.Name} ={(operationModel.IsAsync() ? " await" : "")} {domainModel.Name.ToPrivateMemberName()}Repository.FindById{(operationModel.IsAsync() ? "Async" : "")}({operationModel.Parameters.Single().Name.ToCamelCase()});
                {domainModel.Name.ToPrivateMemberName()}Repository.Remove(existing{domainModel.Name});";
        }

        public IEnumerable<ConstructorParameter> GetRequiredServices(ClassModel domainModel)
        {
            var repo = _decorator.GetEntityRepositoryInterfaceName(domainModel);
            return new[]
            {
                new ConstructorParameter(repo, repo.Substring(1).ToCamelCase()),
            };
        }
    }
}