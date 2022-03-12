using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.Decorators;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies
{
    public class CreateImplementationStrategy : IImplementationStrategy
    {
        private readonly CrudConventionDecorator _decorator;

        public CreateImplementationStrategy(CrudConventionDecorator decorator)
        {
            _decorator = decorator;
        }

        public bool Match(ClassModel domainModel, OperationModel operationModel)
        {
            if (operationModel.Parameters.Count() != 1)
            {
                return false;
            }

            if (operationModel.TypeReference.Element != null && !_decorator.Template.GetTypeInfo(operationModel.TypeReference).IsPrimitive)
            {
                return false;
            }

            var lowerDomainName = domainModel.Name.ToLower();
            var lowerOperationName = operationModel.Name.ToLower();
            return new[]
            {
                "post",
                $"post{lowerDomainName}",
                "create",
                $"create{lowerDomainName}",
                $"add{lowerDomainName}",
            }
            .Contains(lowerOperationName);
        }

        public string GetImplementation(ClassModel domainModel, OperationModel operationModel)
        {
            var entityName = _decorator.Template.GetTypeName("Domain.Entities", domainModel, new TemplateDiscoveryOptions() { ThrowIfNotFound = false });
            var impl = $@"var new{domainModel.Name} = new {entityName ?? domainModel.Name}
                {{
{GetPropertyAssignments(domainModel, operationModel.Parameters.First())}
                }};
                
                {domainModel.Name.ToPrivateMember()}Repository.Add(new{domainModel.Name});";

            if (operationModel.TypeReference.Element != null)
            {
                impl += $@"
                await {domainModel.Name.ToPrivateMember()}Repository.SaveChangesAsync();
                return new{domainModel.Name}.Id;";
            }

            return impl;
        }

        public IEnumerable<ConstructorParameter> GetRequiredServices(ClassModel targetEntity)
        {
            var repo = _decorator.Template.GetTypeName(EntityRepositoryInterfaceTemplate.Identifier, targetEntity);
            return new[]
            {
                new ConstructorParameter(repo, repo.Substring(1).ToCamelCase()),
            };
        }

        private string GetPropertyAssignments(ClassModel domainModel, ParameterModel operationParameterModel)
        {
            var sb = new StringBuilder();
            var dto = _decorator.FindDTOModel(operationParameterModel.TypeReference.Element.Id);
            foreach (var dtoField in dto.Fields)
            {
                var domainAttribute = domainModel.Attributes.FirstOrDefault(p => p.Name.Equals(dtoField.Name, StringComparison.OrdinalIgnoreCase));
                if (domainAttribute == null)
                {
                    sb.AppendLine($"                    #warning No matching field found for {dtoField.Name}");
                    continue;
                }
                if (domainAttribute.Type.Element.Id != dtoField.TypeReference.Element.Id)
                {
                    sb.AppendLine($"                    #warning No matching type for Domain: {domainAttribute.Name} and DTO: {dtoField.Name}");
                    continue;
                }
                sb.AppendLine($"                    {domainAttribute.Name.ToPascalCase()} = {operationParameterModel.Name}.{dtoField.Name.ToPascalCase()},");
            }

            return sb.ToString().Trim();
        }
    }
}
