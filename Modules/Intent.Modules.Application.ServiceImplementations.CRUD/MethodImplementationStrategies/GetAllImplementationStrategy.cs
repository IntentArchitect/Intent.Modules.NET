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
            var dto = _decorator.FindDTOModel(operationModel.TypeReference.Element.Id);
            return $@"var elements ={ (operationModel.IsAsync() ? "await" : "") } {domainModel.Name.ToPrivateMember()}Repository.FindAll{ (operationModel.IsAsync() ? "Async" : "") }();
            return elements.MapTo{_decorator.Template.GetTypeName(DtoModelTemplate.TemplateId, dto)}List(_mapper);";
        }

        public IEnumerable<ConstructorParameter> GetRequiredServices(ClassModel targetEntity)
        {
            var repo = _decorator.Template.GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, targetEntity);
            return new[]
            {
                new ConstructorParameter(repo, repo.Substring(1).ToCamelCase()),
                new ConstructorParameter(_decorator.Template.UseType("AutoMapper.IMapper"), "mapper"), 
            };
        }   
    }
}
