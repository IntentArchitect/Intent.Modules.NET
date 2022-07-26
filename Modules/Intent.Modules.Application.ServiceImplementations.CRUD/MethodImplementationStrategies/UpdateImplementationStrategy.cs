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
    public class UpdateImplementationStrategy : IImplementationStrategy
    {
        private readonly CrudConventionDecorator _decorator;

        public UpdateImplementationStrategy(CrudConventionDecorator decorator)
        {
            _decorator = decorator;
        }

        public bool Match(ClassModel domainModel, OperationModel operationModel)
        {
            var lowerDomainName = domainModel.Name.ToLower();
            var lowerOperationName = operationModel.Name.ToLower();
            if (operationModel.Parameters.Count() != 2)
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

            if (operationModel.TypeReference?.Element != null)
            {
                return false;
            }

            if (!_decorator.HasEntityRepositoryInterfaceName(domainModel))
            {
                return false;
            }

            return new[]
                {
                    "put",
                    $"put{lowerDomainName}",
                    "update",
                    $"update{lowerDomainName}",
                }
                .Contains(lowerOperationName);
        }

        public string GetImplementation(ClassModel domainModel, OperationModel operationModel)
        {
            var idParam =
                operationModel.Parameters.First(p => p.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase));
            var dtoParam =
                operationModel.Parameters.First(p => !p.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase));

            return
                $@"var existing{domainModel.Name} ={(operationModel.IsAsync() ? " await" : "")} {domainModel.Name.ToPrivateMemberName()}Repository.FindById{(operationModel.IsAsync() ? "Async" : "")}({idParam.Name});
                {GetPropertyAssignments(domainModel, "existing" + domainModel.Name, dtoParam)}";
        }

        public IEnumerable<ConstructorParameter> GetRequiredServices(ClassModel domainModel)
        {
            var repo = _decorator.GetEntityRepositoryInterfaceName(domainModel);
            return new[]
            {
                new ConstructorParameter(repo, repo.Substring(1).ToCamelCase()),
            };
        }

        private string GetPropertyAssignments(ClassModel domainModel, string domainVarName,
            ParameterModel operationParameterModel)
        {
            var sb = new StringBuilder();
            var dto = _decorator.FindDTOModel(operationParameterModel.TypeReference.Element.Id);
            foreach (var dtoField in dto.Fields)
            {
                var domainAttribute = domainModel.Attributes.FirstOrDefault(p =>
                    p.Name.Equals(dtoField.Name, StringComparison.OrdinalIgnoreCase));
                if (domainAttribute == null)
                {
                    sb.AppendLine($"                    #warning No matching field found for {dtoField.Name}");
                    continue;
                }

                if (domainAttribute.Type.Element.Id != dtoField.TypeReference.Element.Id)
                {
                    sb.AppendLine(
                        $"                    #warning No matching type for Domain: {domainAttribute.Name} and DTO: {dtoField.Name}");
                    continue;
                }

                sb.AppendLine(
                    $"                    {domainVarName}.{domainAttribute.Name.ToPascalCase()} = {operationParameterModel.Name}.{dtoField.Name.ToPascalCase()};");
            }

            return sb.ToString().Trim();
        }
    }
}