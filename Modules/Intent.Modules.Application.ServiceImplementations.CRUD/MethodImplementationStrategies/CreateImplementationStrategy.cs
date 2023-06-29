using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using JetBrains.Annotations;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies
{
    public class CreateImplementationStrategy : IImplementationStrategy
    {
        private readonly ServiceImplementationTemplate _template;
        private readonly IApplication _application;

        public CreateImplementationStrategy(ServiceImplementationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public bool IsMatch(OperationModel operationModel)
        {
            if (operationModel.Parameters.Count != 1)
            {
                return false;
            }

            var dtoModel = operationModel.Parameters.First().TypeReference.Element.AsDTOModel();
            if (dtoModel == null)
            {
                return false;
            }

            var domainModel = dtoModel.Mapping?.Element?.AsClassModel();
            if (domainModel == null)
            {
                return false;
            }

            var lowerOperationName = operationModel.Name.ToLower();
            return new[] { "post", "create", "add" }.Any(x => lowerOperationName.Contains(x));
        }

        public void ApplyStrategy(OperationModel operationModel)
        {
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");
            
            var (dtoModel, domainModel) = operationModel.GetCreateModelPair();
            var domainType = _template.TryGetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, domainModel, out var result)
                ? result
                : domainModel.Name;
            var domainTypePascalCased = domainType.ToPascalCase();
            var domainTypeCamelCased = domainType.ToCamelCase();
            var repositoryTypeName = _template.GetEntityRepositoryInterfaceName(domainModel);
            var repositoryFieldName = $"{domainTypeCamelCased}Repository";
            var dtoParam = operationModel.Parameters.First(p => !p.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase));
            
            var codeLines = new CSharpStatementAggregator();
            codeLines.Add($"var new{domainTypePascalCased} = new {domainType}");
            codeLines.Add(new CSharpStatementBlock()
                .AddStatements(GetDTOPropertyAssignments("", dtoParam.Name, domainModel.InternalElement, dtoModel.Fields))
                .WithSemicolon());
            codeLines.Add($"{repositoryFieldName.ToPrivateMemberName()}.Add(new{domainTypePascalCased});");
            if (operationModel.TypeReference.Element != null)
            {
                codeLines.Add($@"await {repositoryFieldName.ToPrivateMemberName()}.UnitOfWork.SaveChangesAsync(cancellationToken);");
                var dtoToReturn = operationModel.TypeReference.Element.AsDTOModel();
                codeLines.Add(dtoToReturn != null
                    ? $"return new{domainTypePascalCased}.MapTo{_template.GetTypeName(dtoToReturn.InternalElement)}(_mapper);"
                    : $"return new{domainTypePascalCased}.{(domainModel.Attributes).Concat(domainModel.ParentClass?.Attributes ?? new List<AttributeModel>()).FirstOrDefault(x => x.HasPrimaryKey())?.Name.ToPascalCase() ?? "Id"};");
            }
            
            var @class = _template.CSharpFile.Classes.First();
            var method = @class.FindMethod(m => m.Name.Equals(operationModel.Name, StringComparison.OrdinalIgnoreCase));
            var attr = method.Attributes.OfType<CSharpIntentManagedAttribute>().FirstOrDefault();
            if (attr == null)
            {
                attr = CSharpIntentManagedAttribute.Fully();
                method.AddAttribute(attr);
            }
            attr.WithBodyFully();
            method.Statements.Clear();
            method.AddStatements(codeLines.ToList());

            var ctor = @class.Constructors.First();
            if (ctor.Parameters.All(p => p.Name != repositoryFieldName))
            {
                ctor.AddParameter(repositoryTypeName, repositoryFieldName, parm => parm.IntroduceReadonlyField());
            }
            if (operationModel.TypeReference.Element?.IsDTOModel() == true && ctor.Parameters.All(p => p.Name != "mapper"))
            {
                ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", parm => parm.IntroduceReadonlyField());
            }
        }
        
        private IEnumerable<CSharpStatement> GetDTOPropertyAssignments(string entityVarName, string dtoVarName, IElement domainModel, IList<DTOFieldModel> dtoFields)
        {
            var codeLines = new List<CSharpStatement>();
            foreach (var field in dtoFields)
            {
                if (field.Mapping?.Element == null
                    && domainModel.ChildElements.All(p => p.Name != field.Name))
                {
                    codeLines.Add($"#warning No matching field found for {field.Name}");
                    continue;
                }

                var entityVarExpr = !string.IsNullOrWhiteSpace(entityVarName) ? $"{entityVarName}." : string.Empty;
                switch (field.Mapping?.Element?.SpecializationTypeId)
                {
                    default:
                        var mappedPropertyName = field.Mapping?.Element?.Name ?? "<null>";
                        codeLines.Add($"#warning No matching type for Domain: {mappedPropertyName} and DTO: {field.Name}");
                        break;
                    case null:
                    case AttributeModel.SpecializationTypeId:
                        var attribute = field.Mapping?.Element
                                        ?? domainModel.ChildElements.First(p => p.Name == field.Name);

                        if (attribute.TypeReference?.Element?.SpecializationType == "Value Object")
                        {
                            var property = $"{entityVarExpr}{attribute.Name.ToPascalCase()}";
                            var updateMethodName = $"Create{attribute.TypeReference.Element.Name.ToPascalCase()}";
                            if (attribute.TypeReference.IsCollection)
                            {
                                codeLines.Add($"{property} = {dtoVarName}.{field.Name.ToPascalCase()}.Select(x => {updateMethodName}(x)).ToList()),");
                            }
                            else
                            {
                                codeLines.Add($"{property} = {updateMethodName}({dtoVarName}.{field.Name.ToPascalCase()}),");
                            }
                            AddValueObjectFactoryMethod(updateMethodName, (IElement)attribute.TypeReference.Element, field);
                            break;
                        }                        

                        if (field.TypeReference.IsCollection)
                        {
                            codeLines.Add($"{entityVarExpr}{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()}.ToList(),");
                            break;
                        }
                        if (!attribute.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                        {
                            codeLines.Add($"{entityVarExpr}{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()},");
                            break;
                        }

                        break;
                    case AssociationTargetEndModel.SpecializationTypeId:
                    {
                        var association = field.Mapping.Element.AsAssociationTargetEndModel();
                        var targetType = association.Element as IElement;
                        var attributeName = association.Name.ToPascalCase();

                            
                        if (association.Element.SpecializationType == "Value Object")
                        {
                            var targetValueObject = (IElement)association.Element;
                            var property = $"{entityVarExpr}{attributeName}";
                            var updateMethodName = $"Create{targetValueObject.Name.ToPascalCase()}";
                            if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                            {
                                codeLines.Add($"{property} = {updateMethodName}({dtoVarName}.{field.Name.ToPascalCase()}),");
                            }
                            else
                            {
                                codeLines.Add($"{property} = {dtoVarName}.{field.Name.ToPascalCase()}.Select(x => {updateMethodName}(x)).ToList()),");
                            }
                            AddValueObjectFactoryMethod(updateMethodName, targetValueObject, field);
                            break;
                        }


                        if (association.Association.AssociationType == AssociationType.Aggregation)
                        {
                            codeLines.Add($@"#warning Field not a composite association: {field.Name.ToPascalCase()}");
                            break;
                        }

                        if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                        {
                            if (field.TypeReference.IsNullable)
                            {
                                codeLines.Add(
                                    $"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()} != null ? {GetCreateMethodName(targetType, attributeName)}({dtoVarName}.{field.Name.ToPascalCase()}) : null,");
                            }
                            else
                            {
                                codeLines.Add($"{entityVarExpr}{attributeName} = {GetCreateMethodName(targetType, attributeName)}({dtoVarName}.{field.Name.ToPascalCase()}),");
                            }
                        }
                        else
                        {
                            codeLines.Add(
                                $"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()}{(field.TypeReference.IsNullable ? "?" : "")}.Select({GetCreateMethodName(targetType, attributeName)}).ToList(){(field.TypeReference.IsNullable ? $" ?? new List<{targetType.Name.ToPascalCase()}>()" : "")},");
                        }

                        var @class = _template.CSharpFile.Classes.First();
                        @class.AddMethod(_template.GetTypeName(targetType),
                            GetCreateMethodName(targetType, attributeName),
                            method => method.Private()
                                .AddAttribute(CSharpIntentManagedAttribute.Fully())
                                .AddParameter(_template.GetTypeName((IElement)field.TypeReference.Element), "dto")
                                .AddStatement($"return new {targetType.Name.ToPascalCase()}")
                                .AddStatement(new CSharpStatementBlock()
                                    .AddStatements(GetDTOPropertyAssignments("", $"dto", targetType,
                                        ((IElement)field.TypeReference.Element).ChildElements.Where(x => x.IsDTOFieldModel()).Select(x => x.AsDTOFieldModel()).ToList()))
                                    .WithSemicolon()));
                    }
                        break;
                }
            }

            return codeLines;
        }

        private void AddValueObjectFactoryMethod(string mappingMethodName, IElement domain, DTOFieldModel field)
        {
            var @class = _template.CSharpFile.Classes.First();
            var targetDto = field.TypeReference.Element.AsDTOModel();
            if (!MethodExists(mappingMethodName, @class, targetDto))
            {
                var domainType = _template.GetTypeName(domain);
                @class.AddMethod(domainType, mappingMethodName, method =>
                {
                    method.Static()
                        .AddAttribute(CSharpIntentManagedAttribute.Fully())
                        .AddParameter(_template.GetTypeName(targetDto.InternalElement), "dto");

                    var attributeModels = GetDomainAttibuteModels(domain);

                    var attributeMap = attributeModels.Select(a => (Domain: a, Dto: targetDto.Fields.FirstOrDefault(f => f.Mapping?.Element.Id == a.Id)));
                    if (attributeMap.Any(x => x.Dto == null))
                    {
                        method.AddStatement($@"#warning Not all fields specified for ValueObject.");
                    }
                    var ctorParameters = string.Join(",", attributeMap.Select(m => $"{m.Domain.Name.ToParameterName()}: {(m.Dto == null ? $"default({_template.GetTypeName(m.Domain.TypeReference)})" : $"dto.{m.Dto.Name.ToPascalCase()}")}"));
                    method.AddStatement($"return new {domainType}({ctorParameters});");
                });
            }
        }

        private IList<AttributeModel> GetDomainAttibuteModels(IElement element)
        {
            return element.ChildElements.Where(x => x.IsAttributeModel()).Select(x => x.AsAttributeModel()).ToList();
        }

        private bool MethodExists(string mappingMethodName, CSharpClass @class, DTOModel targetDto)
        {
            return @class.FindMethod((method) =>
                                        method.Name == mappingMethodName
                                        && method.Parameters.Count == 1
                                        && method.Parameters[0].Type == _template.GetTypeName(targetDto.InternalElement)) != null;
        }


        private string GetCreateMethodName(ICanBeReferencedType classModel, [CanBeNull] string attributeName)
        {
            return $"Create{classModel.Name.ToPascalCase()}";
        }
    }
}