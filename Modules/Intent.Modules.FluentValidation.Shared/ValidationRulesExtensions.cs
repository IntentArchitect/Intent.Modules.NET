using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Application.FluentValidation.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Shared;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Utils;

namespace Intent.Modules.FluentValidation.Shared;

public static class ValidationRulesExtensions
{
    public static bool HasValidationRules(
        DTOModel dtoModel,
        string dtoTemplateId,
        string dtoValidatorTemplateId,
        bool uniqueConstraintValidationEnabled,
        bool customValidationEnabled,
        IEnumerable<IAssociationEnd> associationedElements)
    {
        var indexFields = GetUniqueConstraintFields(dtoModel, associationedElements, uniqueConstraintValidationEnabled);

        return GetValidationRulesStatements<object>(
            template: default,
            dtoModel: dtoModel,
            dtoTemplateId: dtoTemplateId,
            dtoValidatorTemplateId: dtoValidatorTemplateId,
            indexFields: indexFields, 
            customValidationEnabled: customValidationEnabled,
            associationedElements: associationedElements).Any();
    }

    public static void ConfigureForValidation<TModel>(
        this CSharpTemplateBase<TModel> template,
        DTOModel dtoModel,
        string toValidateTemplateId,
        string modelParameterName,
        string dtoTemplateId,
        string dtoValidatorTemplateId,
        string validatorProviderInterfaceTemplateId,
        bool uniqueConstraintValidationEnabled,
        bool repositoryInjectionEnabled,
        bool customValidationEnabled,
        IEnumerable<IAssociationEnd> associationedElements)
    {
        ((ICSharpFileBuilderTemplate)template).CSharpFile
            .AddUsing("FluentValidation")
            .AddClass($"{dtoModel.Name}Validator", @class =>
            {
                var toValidateTypeName = template.GetTypeName(toValidateTemplateId, dtoModel);

                @class.AddMetadata("validator", true);
                @class.WithBaseType($"AbstractValidator<{toValidateTypeName}>");

                @class.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());

                @class.AddConstructor(ctor =>
                {
                    ctor.AddStatement(new CSharpStatement("ConfigureValidationRules();")
                        .AddMetadata("configure-validation-rules", true));
                    ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                });

                var indexFields = GetUniqueConstraintFields(dtoModel, associationedElements, uniqueConstraintValidationEnabled);
                string repositoryFieldName = null;

                @class.AddMethod("void", "ConfigureValidationRules", method =>
                {
                    method.Private();

                    var validationRuleStatements = template.GetValidationRulesStatements(
                        dtoModel: dtoModel,
                        dtoTemplateId: dtoTemplateId,
                        dtoValidatorTemplateId: dtoValidatorTemplateId,
                        indexFields: indexFields,
                        customValidationEnabled: customValidationEnabled,
                        associationedElements: associationedElements);

                    foreach (var propertyStatement in validationRuleStatements)
                    {
                        method.AddStatement(propertyStatement);

                        AddValidatorProviderIfRequired(template, @class, propertyStatement, validatorProviderInterfaceTemplateId);
                        if (repositoryInjectionEnabled && AddRepositoryIfRequired(template, dtoModel, @class, propertyStatement, associationedElements, out var possibleRepositoryFieldName) &&
                            string.IsNullOrWhiteSpace(repositoryFieldName))
                        {
                            repositoryFieldName = possibleRepositoryFieldName;
                        }
                    }
                });

                foreach (var field in dtoModel.Fields)
                {
                    var validations = field.GetValidations();
                    if (validations == null)
                    {
                        continue;
                    }

                    if (customValidationEnabled)
                    {
                        if (validations.Custom())
                        {
                            @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}", $"Validate{field.Name.ToPascalCase()}Async", method =>
                            {
                                method
                                    .AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored())
                                    .Private()
                                    .Async();
                                method.AddParameter(template.GetTypeName(field), "value");
                                method.AddParameter($"ValidationContext<{toValidateTypeName}>", "validationContext");
                                method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                                method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                                method.AddStatement($"throw new {template.UseType("System.NotImplementedException")}(\"Your custom validation rules here...\");");
                            });
                        }

                        if (validations.HasCustomValidation() ||
                            validations.Must())
                        {
                            @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}<bool>", $"Validate{field.Name.ToPascalCase()}Async", method =>
                            {
                                method
                                    .AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored())
                                    .Private()
                                    .Async();
                                method.AddParameter(toValidateTypeName, modelParameterName);
                                method.AddParameter(template.GetTypeName(field), "value");
                                method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                                method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                                method.AddStatement($"throw new {template.UseType("System.NotImplementedException")}(\"Your custom validation rules here...\");");
                            });
                        }
                    }

                    if (indexFields.Any(p => p.FieldName == field.Name && p.GroupCount == 1))
                    {
                        if (!TryGetMappedAttribute(field, out var mappedAttribute) && !TryGetAdvancedMappedAttribute(field, out mappedAttribute))
                        { 
                            continue; 
                        }

                        @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}<bool>", $"CheckUniqueConstraint_{field.Name.ToPascalCase()}", method =>
                        {
                            method.Private().Async();
                            if (IsUpdateDto(dtoModel))
                            {
                                method.AddParameter(toValidateTypeName, "model");
                            }
                            method.AddParameter(template.GetTypeName(field.TypeReference), "value");

                            method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");

                            if (!repositoryInjectionEnabled)
                            {
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());
                                method.AddStatement("// Implement custom unique constraint validation here");
                                method.AddStatement("return true;");
                                return;
                            }

                            if (IsCreateDto(dtoModel))
                            {
                                method.AddStatement($"return !await {repositoryFieldName}.AnyAsync(p => p.{mappedAttribute.Name.ToPascalCase()} == value, cancellationToken);");
                            }
                            else
                            {
                                TryGetMappedClass(dtoModel, out var classModel);
                                var fieldIds = dtoModel.Fields.GetEntityIdFields(classModel, template.ExecutionContext);
                                method.AddStatement($"return !await {repositoryFieldName}.AnyAsync(p => {fieldIds.GetAttributeAndFieldComparison("p", "model", false)} && p.{mappedAttribute.Name.ToPascalCase()} == model.{field.Name.ToPascalCase()}, cancellationToken);");
                            }
                        });
                    }
                }

                foreach (var indexGroup in indexFields.Where(p => p.GroupCount > 1).GroupBy(g => g.CompositeGroupName))
                {
                    @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}<bool>", $"CheckUniqueConstraint_{string.Join("_", indexGroup.Select(s => s.FieldName.ToPascalCase()))}", method =>
                    {
                        method.Private().Async();
                        method.AddParameter(toValidateTypeName, "model");
                        method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");

                        if (!repositoryInjectionEnabled)
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());
                            method.AddStatement("// Implement custom unique constraint validation here");
                            method.AddStatement("return true;");
                            return;
                        }

                        CSharpStatement expressionBody;
                        if (IsCreateDto(dtoModel))
                        {
                            expressionBody = GetDtoAndDomainAttributeComparisonExpression("p", "model", dtoModel, indexGroup.ToArray(), associationedElements);
                        }
                        else
                        {
                            TryGetMappedClass(dtoModel, out var classModel);
                            var fieldIds = dtoModel.Fields.GetEntityIdFields(classModel, template.ExecutionContext);
                            expressionBody = $"{fieldIds.GetAttributeAndFieldComparison("p", "model", false)} && " + GetDtoAndDomainAttributeComparisonExpression("p", "model", dtoModel, indexGroup.ToArray(), associationedElements);
                        }

                        method.AddInvocationStatement($"return !await {repositoryFieldName}.AnyAsync", stmt => stmt
                            .AddArgument(new CSharpLambdaBlock("p")
                                .WithExpressionBody(expressionBody))
                            .AddArgument("cancellationToken"));
                    });
                }
            });
    }

    private static CSharpStatement GetDtoAndDomainAttributeComparisonExpression(
        string domainEntityVarName,
        string dtoModelVarName,
        DTOModel dtoModel,
        IReadOnlyCollection<ConstraintField> constraintFields,
        IEnumerable<IAssociationEnd> associationedElements)
    {
        var sb = new StringBuilder();

        foreach (var field in dtoModel.Fields)
        {
            if ((!TryGetMappedAttribute(field, out var mappedAttribute) && !TryGetAdvancedMappedAttribute(field, out mappedAttribute)) || 
                constraintFields.All(p => p.FieldName != mappedAttribute.Name))
            {
                continue;
            }

            if (sb.Length > 0)
            {
                sb.Append(" && ");
            }

            sb.Append($"{domainEntityVarName}.{mappedAttribute.Name} == {dtoModelVarName}.{field.Name}");
        }

        return sb.ToString();
    }

    private static IEnumerable<CSharpMethodChainStatement> GetValidationRulesStatements<TModel>(this CSharpTemplateBase<TModel> template,
        DTOModel dtoModel,
        string dtoTemplateId,
        string dtoValidatorTemplateId,
        IReadOnlyCollection<ConstraintField> indexFields,
        bool customValidationEnabled,
        IEnumerable<IAssociationEnd> associationedElements)
    {
        // If no template is present we still need a way to determine what
        // type is primitive.
        var resolvedTypeInfo = template is null
            ? new CSharpTypeResolver(
                defaultCollectionFormatter: CSharpCollectionFormatter.Create("System.Collections.Generic.IEnumerable<{0}>"),
                defaultNullableFormatter: null)
            : template.Types;

        foreach (var field in dtoModel.Fields)
        {
            var validationRuleChain = new CSharpMethodChainStatement($"RuleFor(v => v.{field.Name.ToPascalCase()})");
            validationRuleChain.AddMetadata("model", field);

            if (field.HasValidations())
            {
                var validations = field.GetValidations();
                if (validations.CascadeMode().Value != null)
                {
                    validationRuleChain.AddChainStatement($"Cascade(CascadeMode.{validations.CascadeMode().Value})");
                }
            }

            if (!resolvedTypeInfo.Get(field.TypeReference).IsPrimitive &&
                !field.TypeReference.IsNullable)
            {
                validationRuleChain.AddChainStatement("NotNull()");
            }

            if (field.HasValidations())
            {
                AddValidatorsFromFluentValidationStereotype(field, validationRuleChain, customValidationEnabled);
            }

            AddValidatorsFromMappedDomain(validationRuleChain, dtoModel, field, indexFields, associationedElements);

            AddValidatorsBasedOnTypeReference(template, dtoTemplateId, dtoValidatorTemplateId, field, validationRuleChain);

            if (!validationRuleChain.Statements.Any())
            {
                continue;
            }

            yield return validationRuleChain;
        }

        var ruleChains = GetValidatorsForDtoLevel(indexFields);
        foreach (var ruleChain in ruleChains)
        {
            yield return ruleChain;
        }
    }

    private static void AddValidatorsFromFluentValidationStereotype(
        DTOFieldModel field,
        CSharpMethodChainStatement validationRuleChain,
        bool customValidationEnabled)
    {
        var validations = field.GetValidations();

        if (validations.NotEmpty())
        {
            validationRuleChain.AddChainStatement("NotEmpty()");
        }

        if (!string.IsNullOrWhiteSpace(validations.Equal()))
        {
            validationRuleChain.AddChainStatement($"Equal({validations.Equal()})");
        }

        if (!string.IsNullOrWhiteSpace(validations.NotEqual()))
        {
            validationRuleChain.AddChainStatement($"NotEqual({validations.NotEqual()})");
        }

        if (validations.MinLength() != null && validations.MaxLength() != null)
        {
            validationRuleChain.AddChainStatement($"Length({validations.MinLength()}, {validations.MaxLength()})");
        }
        else if (validations.MinLength() != null)
        {
            validationRuleChain.AddChainStatement($"MinimumLength({validations.MinLength()})");
        }
        else if (validations.MaxLength() != null)
        {
            validationRuleChain.AddChainStatement($"MaximumLength({validations.MaxLength()})",
                stmt => stmt.AddMetadata("max-length", validations.MaxLength()));
        }

        if (validations.Min() != null && validations.Max() != null &&
            int.TryParse(validations.Min(), out var min) && int.TryParse(validations.Max(), out var max))
        {
            validationRuleChain.AddChainStatement($"InclusiveBetween({min}, {max})");
        }
        else if (!string.IsNullOrWhiteSpace(validations.Min()))
        {
            validationRuleChain.AddChainStatement($"GreaterThanOrEqualTo({validations.Min()})");
        }
        else if (!string.IsNullOrWhiteSpace(validations.Max()))
        {
            validationRuleChain.AddChainStatement($"LessThanOrEqualTo({validations.Max()})");
        }

        if (!string.IsNullOrWhiteSpace(validations.RegularExpression()))
        {
            validationRuleChain.AddChainStatement($@"Matches(@""{validations.RegularExpression()}"")");
        }

        if (validations.EmailAddress())
        {
            validationRuleChain.AddChainStatement("EmailAddress()");
        }

        if (!string.IsNullOrWhiteSpace(validations.Predicate()))
        {
            var message = !string.IsNullOrWhiteSpace(validations.PredicateMessage()) ? $".WithMessage(\"{validations.PredicateMessage()}\")" : string.Empty;
            validationRuleChain.AddChainStatement($"Must({validations.Predicate()}){message}");
        }

        if (customValidationEnabled)
        {
            if (validations.Custom())
            {
                validationRuleChain.AddChainStatement($"CustomAsync(Validate{field.Name.ToPascalCase()}Async)");
            }

            if (validations.HasCustomValidation() ||
                validations.Must())
            {
                validationRuleChain.AddChainStatement($"MustAsync(Validate{field.Name.ToPascalCase()}Async)");
            }
        }
    }

    private static void AddValidatorsFromMappedDomain(
        CSharpMethodChainStatement validationRuleChain,
        DTOModel dtoModel,
        DTOFieldModel field,
        IReadOnlyCollection<ConstraintField> indexFields,
        IEnumerable<IAssociationEnd> associationedElements)
    {
        var hasMappedAttribute = TryGetMappedAttribute(field, out var mappedAttribute) || TryGetAdvancedMappedAttribute(field, out mappedAttribute);
        if (!validationRuleChain.Statements.Any(x => x.HasMetadata("max-length")) && hasMappedAttribute)
        {
            try
            {
                if (mappedAttribute.HasStereotype("Text Constraints") &&
                    mappedAttribute.GetStereotypeProperty<int?>("Text Constraints", "MaxLength") > 0 &&
                    field.GetValidations()?.MaxLength() == null)
                {
                    validationRuleChain.AddChainStatement($"MaximumLength({mappedAttribute.GetStereotypeProperty<int>("Text Constraints", "MaxLength")})");
                }
            }
            catch (Exception e)
            {
                Logging.Log.Debug("Could not resolve [Text Constraints] stereotype for Domain attribute: " + e.Message);
            }
        }

        if (indexFields.Any(p => p.FieldName == field.Name && p.GroupCount == 1))
        {
            validationRuleChain.AddChainStatement($"MustAsync(CheckUniqueConstraint_{field.Name.ToPascalCase()})", stmt => stmt.AddMetadata("requires-repository", true));
            validationRuleChain.AddChainStatement($@"WithMessage(""{field.Name.ToPascalCase()} already exists."")");
        }
    }

    private static void AddValidatorsBasedOnTypeReference<TModel>(CSharpTemplateBase<TModel> template, string dtoTemplateId, string dtoValidatorTemplateId, DTOFieldModel property,
        CSharpMethodChainStatement validationRuleChain)
    {
        if (Common.Types.Api.EnumModelExtensions.IsEnumModel(property.TypeReference.Element))
        {
            validationRuleChain.AddChainStatement(property.TypeReference.IsCollection
                ? "ForEach(x => x.IsInEnum())"
                : "IsInEnum()");
        }
        else if (property.TypeReference?.Element is not null &&
                 property.TypeReference.Element.IsDTOModel() &&
                 template?.TryGetTypeName(
                     templateId: dtoValidatorTemplateId,
                     model: property.TypeReference.Element.AsDTOModel(),
                     typeName: out _) == true &&
                 template.TryGetTypeName(
                     templateId: dtoTemplateId,
                     model: property.TypeReference.Element.AsDTOModel(),
                     typeName: out var dtoTemplateName))
        {
            validationRuleChain.AddChainStatement(property.TypeReference.IsCollection
                ? $"ForEach(x => x.SetValidator(provider.GetValidator<{dtoTemplateName}>()!))"
                : $"SetValidator(provider.GetValidator<{dtoTemplateName}>()!)");

            validationRuleChain.Metadata["requires-validator-provider"] = true;
        }
    }

    private static IEnumerable<CSharpMethodChainStatement> GetValidatorsForDtoLevel(IReadOnlyCollection<ConstraintField> indexFields)
    {
        if (indexFields.Any(p => p.GroupCount > 1))
        {
            var validationRuleChain = new CSharpMethodChainStatement("RuleFor(v => v)");
            var indexGroups = indexFields.Where(p => p.GroupCount > 1).GroupBy(g => g.CompositeGroupName).ToArray();
            foreach (var indexGroup in indexGroups)
            {
                validationRuleChain.AddChainStatement($"MustAsync(CheckUniqueConstraint_{string.Join("_", indexGroup.Select(s => s.FieldName.ToPascalCase()))})", stmt => stmt.AddMetadata("requires-repository", true));
                validationRuleChain.AddChainStatement($@"WithMessage(""The combination of {string.Join(" and ", indexGroup.Select(s => s.FieldName))} already exists."")");
            }

            yield return validationRuleChain;
        }
    }

    // This approach is used for nested DTO Validation scenarios.
    // Nested DTO Validators are required to validate nested DTOs.
    // Instantiating nested DTO Validators will cause problems when dependencies
    // are injected. So instead of overburdening the entire Command / Query / DTO Validator templates
    // with dependency injection complexities, just inject the IServiceProvider.
    private static void AddValidatorProviderIfRequired<TModel>(
        CSharpTemplateBase<TModel> template,
        CSharpClass validatorClass,
        CSharpMethodChainStatement statement,
        string validatorProviderInterfaceTemplateId)
    {
        if (!statement.TryGetMetadata("requires-validator-provider", out bool requiresProvider) || !requiresProvider)
        {
            return;
        }

        var ctor = validatorClass.Constructors.First();

        if (ctor.Parameters.Any(p => p.Type.Contains("IValidatorProvider")))
        {
            return;
        }

        var validatorProviderInter = template.GetTypeName(validatorProviderInterfaceTemplateId);

        ctor.Parameters.Insert(0, new CSharpConstructorParameter(validatorProviderInter, "provider", ctor));
        ctor.FindStatements(stmt => stmt.HasMetadata("configure-validation-rules"))
            ?.ToList().ForEach(x => x.Remove());

        ctor.InsertStatement(0, new CSharpStatement("ConfigureValidationRules(provider);")
            .AddMetadata("configure-validation-rules", true));

        validatorClass.FindMethod(p => p.Name == "ConfigureValidationRules")
            ?.AddParameter(validatorProviderInter, "provider");
    }

    private static bool AddRepositoryIfRequired<TModel>(
        CSharpTemplateBase<TModel> template,
        DTOModel dtoModel,
        CSharpClass validatorClass,
        CSharpMethodChainStatement statement,
        IEnumerable<IAssociationEnd> associationedElements,
        out string repositoryFieldName)
    {
        if (!statement.Statements.Any(x => x.TryGetMetadata("requires-repository", out bool requiresRepository) && requiresRepository) ||
            (!TryGetMappedClass(dtoModel, out var classModel) && !TryGetAdvancedMappedClass(associationedElements, out classModel)) ||
            !template.TryGetTemplate<IClassProvider>(TemplateRoles.Repository.Interface.Entity, classModel, out var repositoryInterface))
        {
            repositoryFieldName = null;
            return false;
        }

        var ctor = validatorClass.Constructors.First();

        if (ctor.Parameters.Any(p => p.Type.Contains(repositoryInterface.ClassName)))
        {
            repositoryFieldName = null;
            return false;
        }

        ctor.AddParameter(
            type: template.UseType(repositoryInterface.FullTypeName()),
            name: repositoryInterface.ClassName.Substring(1).ToParameterName(),
            configure: param => param.IntroduceReadonlyField().AddMetadata("repository", repositoryInterface.FullTypeName()));
        repositoryFieldName = repositoryInterface.ClassName.Substring(1).ToPrivateMemberName();
        return true;
    }

    record ConstraintField(string FieldName, string CompositeGroupName)
    {
        public int GroupCount { get; set; }
    };

    private static bool IsCreateDto(DTOModel dtoModel)
    {
        return dtoModel.Name.StartsWith("create", StringComparison.InvariantCultureIgnoreCase) ||
               dtoModel.Name.StartsWith("add", StringComparison.InvariantCultureIgnoreCase) ||
               dtoModel.Name.StartsWith("new", StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool IsUpdateDto(DTOModel dtoModel)
    {
        return dtoModel.Name.StartsWith("update", StringComparison.InvariantCultureIgnoreCase) ||
               dtoModel.Name.StartsWith("edit", StringComparison.InvariantCultureIgnoreCase);
    }

    private static IReadOnlyCollection<ConstraintField> GetUniqueConstraintFields(
        DTOModel dtoModel,
        IEnumerable<IAssociationEnd> associationedElements,
        bool enabled)
    {
        if (!enabled || (!IsCreateDto(dtoModel) && !IsUpdateDto(dtoModel)))
        {
            return ArraySegment<ConstraintField>.Empty;
        }

        var hasMappedClass = TryGetMappedClass(dtoModel, out var mappedClass) || TryGetAdvancedMappedClass(associationedElements, out mappedClass);
        if (!hasMappedClass)
        {
            return ArraySegment<ConstraintField>.Empty;
        }

        var indexStereotypeAttributes = mappedClass.Attributes
            .Where(p => p.HasStereotype("Index") &&
                        p.GetStereotypeProperty("Index", "IsUnique", false))
            .Select(s => new ConstraintField(s.Name, s.GetStereotypeProperty("Index", "UniqueKey", string.Empty)))
            .ToArray();

        const string indexElementTypeId = "436e3afe-b4ef-481c-b803-0d1e7d263561";
        const string indexColumnTypeId = "c5ba925d-5c08-4809-a848-585a0cd4ddd3";

        var indexElements = mappedClass.InternalElement.ChildElements
            .Where(p => p.SpecializationTypeId == indexElementTypeId && p.GetStereotypeProperty("Settings", "Unique", false))
            .ToArray();

        var indexElementAttributes = new List<ConstraintField>();
        foreach (var indexElement in indexElements)
        {
            foreach (var indexColumnIndex in indexElement.ChildElements)
            {
                if (indexColumnIndex.SpecializationTypeId != indexColumnTypeId)
                {
                    continue;
                }
                var mappedAttribute = indexColumnIndex.MappedElement?.Element?.AsAttributeModel();
                if (mappedAttribute is null)
                {
                    continue;
                }
                if (indexColumnIndex.GetStereotypeProperty<string?>("Settings", "Type")?.ToLower() == "included")
                {
                    continue;
                }

                indexElementAttributes.Add(new ConstraintField(mappedAttribute.Name, indexElement.Name));
            }
        }

        var indexes = indexStereotypeAttributes.Concat(indexElementAttributes).ToArray();

        foreach (var group in indexes.GroupBy(g => g.CompositeGroupName))
        {
            var count = group.Count();

            foreach (var item in indexes)
            {
                if (item.CompositeGroupName == group.Key)
                {
                    item.GroupCount = count;
                }
            }
        }

        return indexes;
    }

    private static bool TryGetMappedClass(DTOModel dtoModel, out ClassModel classModel)
    {
        var mappedElement = dtoModel.InternalElement.MappedElement?.Element as IElement;
        // The loop is not needed on the service side where a Command/Query/DTO is mapped
        // to an Entity but it is needed when mapping from a Service Proxy to a Command/Query/Service Operation
        // and then to an Entity.
        while (mappedElement is not null)
        {
            if (mappedElement.IsClassModel())
            {
                classModel = mappedElement.AsClassModel();
                return true;
            }

            mappedElement = mappedElement.MappedElement?.Element as IElement;
        }

        classModel = default;
        return false;
    }

    private static bool TryGetAdvancedMappedClass(IEnumerable<IAssociationEnd> associationedElements, out ClassModel classModel)
    {
        if (associationedElements is null)
        {
            classModel = null;
            return false;
        }

        foreach (var associationEnd in associationedElements)
        {
            foreach (var mapping in associationEnd.Mappings)
            {
                var mappedEnd = mapping.MappedEnds.FirstOrDefault(p => p.MappingType == "Data Mapping");
                if (mappedEnd is null)
                {
                    continue;
                }

                classModel = (mappedEnd.TargetElement as IElement)?.ParentElement?.AsClassModel();
                if (classModel is not null)
                {
                    return true;
                }
            }
        }

        classModel = null;
        return false;
    }

    private static bool TryGetMappedAttribute(DTOFieldModel field, out AttributeModel attribute) => TryGetMappedAttribute(field.InternalElement, out attribute);
    private static bool TryGetMappedAttribute(IElement field, out AttributeModel attribute)
    {
        var mappedElement = field.MappedElement?.Element as IElement;
        // The loop is not needed on the service side where a Command/Query/DTO is mapped
        // to an Attribute but it is needed when mapping from a Service Proxy to a DTO Field and then to an Attribute.
        while (mappedElement is not null)
        {
            if (mappedElement.IsAttributeModel())
            {
                attribute = mappedElement.AsAttributeModel();
                return true;
            }

            if (mappedElement.MappedElement?.Element is null)
            {
                return TryGetAdvancedMappedAttribute(mappedElement, out attribute);
            }
            mappedElement = mappedElement.MappedElement?.Element as IElement;
        }

        attribute = default;
        return false;
    }

    private static bool TryGetAdvancedMappedAttribute(DTOFieldModel field, out AttributeModel attribute) => TryGetAdvancedMappedAttribute(field.InternalElement, out attribute);
    private static bool TryGetAdvancedMappedAttribute(IElement field, out AttributeModel attribute)
    {
        var mappedEnd = field.MappedToElements.FirstOrDefault(p => p.MappingType == "Data Mapping");
        if (mappedEnd != null)
        {
            if (mappedEnd.TargetElement.IsAttributeModel())
            {
                attribute = mappedEnd.TargetElement.AsAttributeModel();
                return true;
            }

            if (TryGetMappedAttribute((IElement)mappedEnd.TargetElement, out attribute))
            {
                return true;
            }
        }

        attribute = null;
        return false;
    }
}