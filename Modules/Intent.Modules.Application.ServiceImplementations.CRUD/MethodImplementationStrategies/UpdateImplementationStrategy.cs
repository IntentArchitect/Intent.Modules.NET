using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies
{
    public class UpdateImplementationStrategy : IImplementationStrategy
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public UpdateImplementationStrategy(ICSharpFileBuilderTemplate template, IApplication application)
        {
            _template = template;
        }

        public bool IsMatch(OperationModel operationModel)
        {
            if (operationModel.CreateEntityActions().Any()
                || operationModel.UpdateEntityActions().Any()
                || operationModel.DeleteEntityActions().Any()
                || operationModel.QueryEntityActions().Any())
            {
                return false;
            }
            
            if (_template.CSharpFile.Classes.First()
                    .FindMethod(m => m.TryGetMetadata<OperationModel>("model", out var model) && model.Id == operationModel.Id) is null)
            {
                return false;
            }

            if (operationModel.Parameters.Count > 2)
            {
                return false;
            }

            var dtoModel = operationModel.Parameters.FirstOrDefault(x => x.TypeReference?.Element?.IsDTOModel() == true)?.TypeReference?.Element?.AsDTOModel();
            if (dtoModel == null)
            {
                return false;
            }

            var domainModel = dtoModel.Mapping?.Element?.AsClassModel();
            if (domainModel == null)
            {
                return false;
            }

            if (!operationModel.Parameters.Any(p => p.Name.Contains("id", StringComparison.OrdinalIgnoreCase)) &&
                dtoModel.Fields.GetEntityIdField(domainModel) == null)
            {
                return false;
            }

            if (!_template.TryGetTemplate<ITemplate>(TemplateRoles.Repository.Interface.Entity, domainModel, out _))
            {
                return false;
            }

            var lowerOperationName = operationModel.Name.ToLower();
            return new[] { "put", "update" }.Any(x => lowerOperationName.Contains(x));
        }
        public void BindToTemplate(ICSharpFileBuilderTemplate template, OperationModel operationModel)
        {
            template.CSharpFile.AfterBuild(_ => ApplyStrategy(operationModel));
        }

        public void ApplyStrategy(OperationModel operationModel)
        {
            _template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");

            var (dtoModel, domainModel) = operationModel.GetUpdateModelPair();
            var repositoryTypeName = _template.GetTypeName(TemplateRoles.Repository.Interface.Entity, domainModel);
            var repositoryParameterName = repositoryTypeName.Split('.').Last()[1..].ToLocalVariableName();
            var repositoryFieldName = repositoryParameterName.ToPrivateMemberName();
            var dtoParam = operationModel.Parameters.First(p => !p.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase));
            var entityVariableName = domainModel.GetExistingVariableName();

            var codeLines = new List<CSharpStatement>();
            var idParam = operationModel.Parameters.FirstOrDefault(p => p.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase))?.Name
                ?? $"{dtoParam.Name}.{dtoModel.Fields.GetEntityIdField(domainModel).Name}";
            codeLines.Add($"var {entityVariableName} = await {repositoryFieldName}.FindByIdAsync({idParam}, cancellationToken);");
            codeLines.Add(new CSharpIfStatement($"{entityVariableName} is null")
                .AddStatement($@"throw new {_template.GetNotFoundExceptionName()}($""Could not find {domainModel.Name.ToPascalCase()} {{{idParam}}}"");"));
            codeLines.AddRange(GetDTOPropertyAssignments($"{entityVariableName}", dtoParam.Name, domainModel.InternalElement, dtoModel.Fields, true));

            if (RepositoryRequiresExplicitUpdate(domainModel))
            {
                codeLines.Add($"{repositoryFieldName}.Update({entityVariableName});");
            }

            if (operationModel.TypeReference.Element.IsDTOModel())
            {
                codeLines.Add($"await {repositoryFieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);");
                codeLines.Add($"return {entityVariableName}.MapTo{_template.GetTypeName((IElement)operationModel.TypeReference.Element)}(_mapper);");
            }

            var @class = _template.CSharpFile.Classes.First();
            var method = @class.FindMethod(m => m.TryGetMetadata<OperationModel>("model", out var model) && model.Id == operationModel.Id);
            var attr = method.Attributes.OfType<CSharpIntentManagedAttribute>().FirstOrDefault();
            if (attr == null)
            {
                attr = CSharpIntentManagedAttribute.Fully();
                method.AddAttribute(attr);
            }
            attr.WithBodyFully();
            method.Statements.Clear();
            method.AddStatements(codeLines);

            var ctor = @class.Constructors.First();
            if (ctor.Parameters.All(p => p.Name != repositoryParameterName))
            {
                ctor.AddParameter(repositoryTypeName, repositoryParameterName, parameter => parameter.IntroduceReadonlyField());
            }
            if (operationModel.TypeReference.Element?.IsDTOModel() == true && ctor.Parameters.All(p => p.Name != "mapper"))
            {
                ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", parameter => parameter.IntroduceReadonlyField());
            }
        }

        private bool RepositoryRequiresExplicitUpdate(ClassModel domainModel)
        {
            return _template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                       TemplateRoles.Repository.Interface.Entity,
                       domainModel,
                       out var repositoryInterfaceTemplate) &&
                   repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var requiresUpdate) &&
                   requiresUpdate;
        }


        private IList<CSharpStatement> GetDTOPropertyAssignments(string entityVarName, string dtoVarName, IElement domainModel, IList<DTOFieldModel> dtoFields, bool skipIdField)
        {
            var codeLines = new CSharpStatementAggregator();
            foreach (var field in dtoFields)
            {
                if (skipIdField && field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (field.Mapping?.Element == null
                    && domainModel.ChildElements.All(p => p.Name != field.Name))
                {
                    codeLines.Add($"#warning No matching field found for {field.Name}");
                    continue;
                }

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
                            var property = $"{entityVarName}{attribute.Name.ToPascalCase()}";
                            var updateMethodName = $"Create{attribute.TypeReference.Element.Name.ToPascalCase()}";
                            if (attribute.TypeReference.IsCollection)
                            {
                                codeLines.Add($"{property} = {dtoVarName}.{field.Name.ToPascalCase()}.Select(x => {updateMethodName}(x)).ToList());");
                            }
                            else
                            {
                                codeLines.Add($"{property} = {updateMethodName}({dtoVarName}.{field.Name.ToPascalCase()});");
                            }
                            AddValueObjectFactoryMethod(updateMethodName, (IElement)attribute.TypeReference.Element, field);
                            break;
                        }

                        if (field.TypeReference.IsCollection)
                        {
                            codeLines.Add($"{entityVarName}.{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()}.ToList();");
                            break;
                        }
                        codeLines.Add($"{entityVarName}.{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()};");
                        break;
                    case AssociationTargetEndModel.SpecializationTypeId:
                        {
                            var association = field.Mapping.Element.AsAssociationTargetEndModel();
                            var targetEntityElement = (IElement)association.Element;
                            var attributeName = association.Name.ToPascalCase();
                            if (association.Association.AssociationType == AssociationType.Aggregation)
                            {
                                codeLines.Add($@"#warning Field not a composite association: {field.Name.ToPascalCase()}");
                                break;
                            }

                            var property = $"{entityVarName}.{attributeName}";

                            if (association.Element.SpecializationType == "Value Object")
                            {
                                var targetValueObject = (IElement)association.Element;
                                var factoryMethodName = $"Create{targetValueObject.Name.ToPascalCase()}";
                                if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                                {
                                    codeLines.Add($"{property} = {factoryMethodName}({dtoVarName}.{field.Name.ToPascalCase()});");
                                }
                                else
                                {
                                    codeLines.Add($"{property} = {dtoVarName}.{field.Name.ToPascalCase()}.Select(x => {factoryMethodName}(x)).ToList());");
                                }
                                AddValueObjectFactoryMethod(factoryMethodName, targetValueObject, field);
                                break;
                            }

                            var updateMethodName = $"CreateOrUpdate{targetEntityElement.Name.ToPascalCase()}";

                            if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                            {
                                codeLines.Add($"{property} = {updateMethodName}({property}, {dtoVarName}.{field.Name.ToPascalCase()});");
                            }
                            else
                            {
                                var targetClass = targetEntityElement.AsClassModel();
                                var targetDto = field.TypeReference.Element.AsDTOModel();
                                var dtoEntityIdField = targetDto.Fields.GetEntityIdField(targetClass);
                                if (dtoEntityIdField == null)
                                {
                                    codeLines.Add($@"#warning Unable to find Identifier on {targetDto.Name}(Dto) for {targetClass.Name}(Class).");
                                    break;
                                }
                                codeLines.Add($"{property} = {_template.GetTypeName("Domain.Common.UpdateHelper")}.CreateOrUpdateCollection({property}, {dtoVarName}.{field.Name.ToPascalCase()}, (e, d) => e.{targetClass.GetEntityIdAttribute().IdName} == d.{dtoEntityIdField.Name.ToPascalCase()}, {updateMethodName});");
                            }

                            var createEntityInterfaces = _template.ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
                            var implementationName = _template.GetTypeName(TemplateRoles.Domain.Entity.EntityImplementation, targetEntityElement);
                            var interfaceName = createEntityInterfaces ? _template.GetTypeName(TemplateRoles.Domain.Entity.Interface, targetEntityElement) : implementationName;
                            string nullableChar = _template.OutputTarget.GetProject().NullableEnabled ? "?" : "";

                            var fieldIsNullable = field.TypeReference.IsNullable;
                            var nullable = fieldIsNullable ? "?" : string.Empty;

                            var @class = _template.CSharpFile.Classes.First();
                            @class.AddMethod($"{interfaceName}{nullable}",
                                updateMethodName,
                                method =>
                                {
                                    method.Private()
                                        .Static()
                                        .AddAttribute(CSharpIntentManagedAttribute.Fully())
                                        .AddParameter($"{interfaceName}{nullableChar}", "entity")
                                        .AddParameter($"{_template.GetTypeName((IElement)field.TypeReference.Element)}{nullable}",
                                            "dto")
                                        .AddStatementBlock("if (dto == null)", s => s
                                            .AddStatement("return null;")
                                        )
                                        .AddStatement($"entity ??= new {implementationName}();", s => s.SeparatedFromPrevious())
                                        .AddStatements(GetDTOPropertyAssignments("entity", "dto", targetEntityElement,
                                            ((IElement)field.TypeReference.Element).ChildElements
                                            .Where(x => x.IsDTOFieldModel()).Select(x => x.AsDTOFieldModel()).ToList(),
                                            true))
                                        .AddStatement("return entity;", s => s.SeparatedFromPrevious());
                                });
                        }
                        break;
                }
            }

            return codeLines.ToList();
        }

        private void AddValueObjectFactoryMethod(string mappingMethodName, IElement domain, DTOFieldModel field)
        {
            var @class = _template.CSharpFile.Classes.First();
            var targetDto = field.TypeReference.Element.AsDTOModel();
            if (!MethodExists(mappingMethodName, @class, targetDto))
            {
                var createEntityInterfaces = _template.ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
                if (!_template.TryGetTypeName(TemplateRoles.Domain.Entity.EntityImplementation, domain, out var implementationName))
                {
                    if (!_template.TryGetTypeName("Intent.ValueObjects.ValueObject", domain, out implementationName))
                    {
                        throw new Exception("unexpected attribute type : " + domain.GetType().Name);
                    }
				}
                var interfaceName = createEntityInterfaces ? _template.GetTypeName(TemplateRoles.Domain.Entity.Interface, domain) : implementationName;

                @class.AddMethod(interfaceName, mappingMethodName, method =>
                {
                    method.Static()
                        .AddAttribute(CSharpIntentManagedAttribute.Fully())
                        .AddParameter(_template.GetTypeName(targetDto.InternalElement), "dto");

                    var attributeModels = GetDomainAttributeModels(domain);

                    var attributeMap = attributeModels.Select(a => (Domain: a, Dto: targetDto.Fields.FirstOrDefault(f => f.Mapping?.Element.Id == a.Id))).ToArray();
                    if (attributeMap.Any(x => x.Dto == null))
                    {
                        method.AddStatement(@"#warning Not all fields specified for ValueObject.");
                    }
                    var ctorParameters = string.Join(",", attributeMap.Select(m => $"{m.Domain.Name.ToParameterName()}: {(m.Dto == null ? $"default({_template.GetTypeName(m.Domain.TypeReference)})" : $"dto.{m.Dto.Name.ToPascalCase()}")}"));
                    method.AddStatement($"return new {implementationName}({ctorParameters});");
                });
            }
        }

        private IList<AttributeModel> GetDomainAttributeModels(IElement element)
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

    }
}