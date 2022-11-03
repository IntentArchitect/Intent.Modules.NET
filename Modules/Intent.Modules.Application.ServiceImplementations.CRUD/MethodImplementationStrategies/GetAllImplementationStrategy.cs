using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.Decorators;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies
{
    public class GetAllImplementationStrategy : IImplementationStrategy
    {
        private readonly CrudConventionDecorator _decorator;

        public GetAllImplementationStrategy(CrudConventionDecorator decorator)
        {
            _decorator = decorator;
        }

        public bool Match(ClassModel domainModel, OperationModel operationModel)
        {
            if (operationModel.Parameters.Any())
            {
                return false;
            }

            if (!(operationModel?.TypeReference?.IsCollection ?? false))
            {
                return false;
            }

            if (!_decorator.HasEntityRepositoryInterfaceName(domainModel)
                || !_decorator.HasDtoName(operationModel.TypeReference?.Element))
            {
                return false;
            }

            var lowerDomainName = domainModel.Name.ToLower();
            var pluralLowerDomainName = lowerDomainName.Pluralize();
            var lowerOperationName = operationModel.Name.ToLower();
            return new[]
                {
                    $"get",
                    $"get{lowerDomainName}",
                    $"get{pluralLowerDomainName}",
                    $"get{pluralLowerDomainName}list",
                    $"getall",
                    $"getall{pluralLowerDomainName}",
                    $"find",
                    $"find{lowerDomainName}",
                    $"find{pluralLowerDomainName}",
                    "findall"
                }
                .Contains(lowerOperationName);
        }

        public string GetImplementation(ClassModel domainModel, OperationModel operationModel)
        {
            return
                $@"var elements ={(operationModel.IsAsync() ? " await" : string.Empty)} {domainModel.Name.ToPrivateMemberName()}Repository.FindAll{(operationModel.IsAsync() ? "Async" : "")}();
            return elements.MapTo{_decorator.GetDtoName(operationModel.TypeReference.Element)}List(_mapper);";
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