using System;
using System.Collections.Generic;
using System.Linq;
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

#nullable enable

namespace Intent.Modules.FluentValidation.Shared;

internal static class ValidationRulesExtensions
{
    public static bool HasValidationRules(
        DTOModel dtoModel,
        string dtoTemplateId,
        string dtoValidatorTemplateId,
        bool uniqueConstraintValidationEnabled,
        bool customValidationEnabled,
        IEnumerable<IAssociationEnd>? sourceElementAdvancedMappings)
    {
        var sourceElementAdvancedMappingsList = sourceElementAdvancedMappings?.ToList();

        var indexFields = UniqueConstraintRules.GetConstraintFields(dtoModel, sourceElementAdvancedMappingsList, uniqueConstraintValidationEnabled);

        var hasNativeValidationRules = GetValidationRulesStatements<object>(
            template: null,
            dtoModel: dtoModel,
            dtoTemplateId: dtoTemplateId,
            dtoValidatorTemplateId: dtoValidatorTemplateId,
            indexFields: indexFields,
            customValidationEnabled: customValidationEnabled,
            sourceElementAdvancedMappings: sourceElementAdvancedMappingsList).Any();

        if (hasNativeValidationRules)
        {
            return true;
        }

        foreach (var field in dtoModel.Fields)
        {
            if (
                (
                    UniqueConstraintRules.TryGetMappedAttribute(field, out var attr) ||
                    UniqueConstraintRules.TryGetAdvancedMappedAttribute(field, out attr) ||
                    UniqueConstraintRules.TryGetAssociationMappedAttribute(field, sourceElementAdvancedMappingsList, out attr)
                ) &&
                DomainConstraintRules.HasAnyRules(attr!))
            {
                return true;
            }
        }

        return false;
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
        IEnumerable<IAssociationEnd>? associationedElements)
    {
        var associationedElementsList = associationedElements?.ToList();

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

                var indexFields = UniqueConstraintRules.GetConstraintFields(dtoModel, associationedElementsList, uniqueConstraintValidationEnabled);
                string? repositoryFieldName = null;
                @class.AddMethod("void", "ConfigureValidationRules", method =>
                {
                    method.Private();

                    var validationRuleStatements = template.GetValidationRulesStatements(
                            dtoModel: dtoModel,
                            dtoTemplateId: dtoTemplateId,
                            dtoValidatorTemplateId: dtoValidatorTemplateId,
                            indexFields: indexFields,
                            customValidationEnabled: customValidationEnabled,
                            sourceElementAdvancedMappings: associationedElementsList)
                        .ToList();

                    foreach (var propertyStatement in validationRuleStatements)
                    {
                        method.AddStatement(propertyStatement);

                        AddValidatorProviderIfRequired(template, @class, propertyStatement, validatorProviderInterfaceTemplateId);
                        if (repositoryInjectionEnabled &&
                            associationedElementsList is not null &&
                            AddRepositoryIfRequired(template, dtoModel, @class, propertyStatement, associationedElementsList, out var possibleRepositoryFieldName) &&
                            string.IsNullOrWhiteSpace(repositoryFieldName))
                        {
                            repositoryFieldName = possibleRepositoryFieldName;
                        }
                    }

                    if (!validationRuleStatements.Any())
                    {
                        method.AddStatement("// Implement custom validation logic here if required");
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
                                    .AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge())
                                    .Private()
                                    .Async();
                                method.AddParameter(template.GetTypeName(field), "value");
                                method.AddParameter($"ValidationContext<{toValidateTypeName}>", "validationContext");
                                method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                                method.AddStatement("// IntentInitialGen");
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
                                    .AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge())
                                    .Private()
                                    .Async();
                                method.AddParameter(toValidateTypeName, modelParameterName);
                                method.AddParameter(template.GetTypeName(field), "value");
                                method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                                method.AddStatement("// IntentInitialGen");
                                method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                                method.AddStatement($"throw new {template.UseType("System.NotImplementedException")}(\"Your custom validation rules here...\");");
                            });
                        }
                    }

                    if (indexFields.Any(p => p.FieldName == field.Name && p.GroupCount == 1))
                    {
                        if (!UniqueConstraintRules.TryGetMappedAttribute(field, out var mappedAttribute) && !UniqueConstraintRules.TryGetAdvancedMappedAttribute(field, out mappedAttribute))
                        {
                            continue;
                        }

                        @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}<bool>", $"CheckUniqueConstraint_{field.Name.ToPascalCase()}", method =>
                        {
                            method.Private().Async();
                            if (UniqueConstraintRules.IsUpdateDto(dtoModel))
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

                            if (UniqueConstraintRules.IsCreateDto(dtoModel))
                            {
                                method.AddStatement($"return !await {repositoryFieldName}.AnyAsync(p => p.{mappedAttribute!.Name.ToPascalCase()} == value, cancellationToken);");
                            }
                            else
                            {
                                UniqueConstraintRules.TryGetMappedClass(dtoModel, out var classModel);
                                var fieldIds = dtoModel.Fields.GetEntityIdFields(classModel, template.ExecutionContext);
                                method.AddStatement($"return !await {repositoryFieldName}.AnyAsync(p => {fieldIds.GetAttributeAndFieldComparison("p", "model", false)} && p.{mappedAttribute!.Name.ToPascalCase()} == model.{field.Name.ToPascalCase()}, cancellationToken);");
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
                        if (UniqueConstraintRules.IsCreateDto(dtoModel))
                        {
                            expressionBody = UniqueConstraintRules.GetDtoAndDomainAttributeComparisonExpression("p", "model", dtoModel, indexGroup.ToArray(), associationedElementsList);
                        }
                        else
                        {
                            UniqueConstraintRules.TryGetMappedClass(dtoModel, out var classModel);
                            var fieldIds = dtoModel.Fields.GetEntityIdFields(classModel, template.ExecutionContext);
                            expressionBody = $"{fieldIds.GetAttributeAndFieldComparison("p", "model", false)} && " + UniqueConstraintRules.GetDtoAndDomainAttributeComparisonExpression("p", "model", dtoModel, indexGroup.ToArray(), associationedElementsList);
                        }

                        method.AddInvocationStatement($"return !await {repositoryFieldName}.AnyAsync", stmt => stmt
                            .AddArgument(new CSharpLambdaBlock("p")
                                .WithExpressionBody(expressionBody))
                            .AddArgument("cancellationToken"));
                    });
                }
            }).AfterBuild(file =>
            {
                var @class = file.Classes.FirstOrDefault();

                if (@class != null && file.Template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.SonarQube"))
                {
                    var method = @class.FindMethod("ConfigureValidationRules");
                    if (method != null)
                    {
                        if (method.Statements.Count == 0 || (method.Statements.Count == 1 && method.Statements.First().ToString().Trim().StartsWith("//")))
                        {
                            method.AddAttribute("System.Diagnostics.CodeAnalysis.SuppressMessage", cfg =>
                            {
                                cfg.AddArgument("\"Performance\"")
                                .AddArgument("\"CA1822:Mark members as static\"")
                                .AddArgument("Justification = \"Depends on user code\"");
                            });
                        }
                    }
                }
            });
    }

    private static IEnumerable<CSharpMethodChainStatement> GetValidationRulesStatements<TModel>(
        this CSharpTemplateBase<TModel>? template,
        DTOModel dtoModel,
        string dtoTemplateId,
        string dtoValidatorTemplateId,
        IReadOnlyCollection<ConstraintField> indexFields,
        bool customValidationEnabled,
        List<IAssociationEnd>? sourceElementAdvancedMappings)
    {
        // If no template is present, we still need a way to determine what type is primitive.
        var resolvedTypeInfo = template is null
            ? new CSharpTypeResolver(
                defaultCollectionFormatter: CSharpCollectionFormatter.Create("System.Collections.Generic.IEnumerable<{0}>"),
                defaultNullableFormatter: null)
            : template.Types;

        if (TryAddValidationFromParent(template, dtoModel, dtoTemplateId, dtoValidatorTemplateId, out var validationStatement))
        {
            yield return validationStatement;
        }

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

            ApplyAttributeValidationRules(
                template,
                validationRuleChain,
                field,
                indexFields,
                sourceElementAdvancedMappings,
                customValidationEnabled);

            AddValidatorsBasedOnTypeReference(template, validationRuleChain, dtoTemplateId, dtoValidatorTemplateId, dtoModel, field);

            if (!validationRuleChain.Statements.Any())
            {
                continue;
            }

            yield return validationRuleChain;
        }

        foreach (var ruleChain in UniqueConstraintRules.GetDtoLevelValidators(indexFields))
        {
            yield return ruleChain;
        }
    }

    private static bool TryAddValidationFromParent<TModel>(CSharpTemplateBase<TModel> template, DTOModel model, string dtoTemplateId,
        string dtoValidatorTemplateId, out CSharpMethodChainStatement? statement)
    {
        if (model.ParentDto is null)
        {
            statement = null;
            return false;
        }

        if (template?.TryGetTypeName(
            templateId: dtoValidatorTemplateId,
            model: model.ParentDto,
            typeName: out _) == true &&
            template.TryGetTypeName(
                templateId: dtoTemplateId,
                model: model.ParentDto,
                typeName: out var dtoTemplateName))
        {
            statement = new CSharpMethodChainStatement($"Include(provider.GetValidator<{dtoTemplateName}>())");
            statement.Metadata["requires-validator-provider"] = true;
            return true;
        }

        statement = null;
        return false;
    }

    /// <summary>
    /// Executes the <b>3-Step Context Pipeline</b> for a single DTO field, appending
    /// FluentValidation chain statements to <paramref name="validationRuleChain"/>:
    /// <list type="number">
    /// <item><description>
    ///   <b>DTO stereotype rules</b> — Explicit rules declared via the FluentValidation
    ///   stereotype on the DTO field (<see cref="DtoValidationRules.ApplyRules"/>).
    ///   These claim rule-spaces in <c>appliedRuleSpaces</c> so later steps cannot override them.
    /// </description></item>
    /// <item><description>
    ///   <b>Unique constraint rules</b> — <c>MustAsync(CheckUniqueConstraint_...)</c> calls derived
    ///   from <c>Index(IsUnique=true)</c> stereotypes on the mapped domain class
    ///   (<see cref="UniqueConstraintRules.ApplyFieldRules"/>).
    /// </description></item>
    /// <item><description>
    ///   <b>Domain constraint fallback rules</b> — Rules derived from Domain Constraints
    ///   stereotypes (TextLimits, NumericLimits, Regex, etc.) on the mapped domain attribute
    ///   (<see cref="DomainConstraintRules.ApplyFallbackRules"/>). Uses the <b>Collect and Join</b>
    ///   pattern to emit a single <c>.ForEach()</c> for collection fields.
    /// </description></item>
    /// </list>
    /// </summary>
    private static void ApplyAttributeValidationRules<TModel>(
        CSharpTemplateBase<TModel>? template,
        CSharpMethodChainStatement validationRuleChain,
        DTOFieldModel field,
        IReadOnlyCollection<ConstraintField> indexFields,
        List<IAssociationEnd>? associationedElements,
        bool customValidationEnabled)
    {
        var appliedRuleSpaces = new HashSet<string>(StringComparer.Ordinal);

        // Step 1: Explicit FluentValidation stereotype rules on the DTO field.
        if (field.HasValidations())
        {
            DtoValidationRules.ApplyRules(field, validationRuleChain, customValidationEnabled, template, appliedRuleSpaces);
        }

        // Step 2: Unique-constraint MustAsync rule for single-field indexes.
        UniqueConstraintRules.ApplyFieldRules(field, validationRuleChain, indexFields);

        // Step 3: Domain constraint fallback rules from the mapped domain attribute.
        var hasMappedAttribute = UniqueConstraintRules.TryGetMappedAttribute(field, out var mappedAttribute) ||
                                 UniqueConstraintRules.TryGetAdvancedMappedAttribute(field, out mappedAttribute) ||
                                 UniqueConstraintRules.TryGetAssociationMappedAttribute(field, associationedElements, out mappedAttribute);

        if (hasMappedAttribute)
        {
            DomainConstraintRules.ApplyFallbackRules(template, validationRuleChain, mappedAttribute!, appliedRuleSpaces);
        }
    }

    private static void AddValidatorsBasedOnTypeReference<TModel>(
        CSharpTemplateBase<TModel> template,
        CSharpMethodChainStatement validationRuleChain,
        string dtoTemplateId,
        string dtoValidatorTemplateId,
        DTOModel dtoModel,
        DTOFieldModel property)
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
            if (property.TypeReference.Element.Id == dtoModel.Id)
            {
                validationRuleChain.AddChainStatement(property.TypeReference.IsCollection
                    ? $"ForEach(x => x.SetValidator(this))"
                    : $"SetValidator(this)");
                return;
            }

            validationRuleChain.AddChainStatement(property.TypeReference.IsCollection
                ? $"ForEach(x => x.SetValidator(provider.GetValidator<{dtoTemplateName}>()!))"
                : $"SetValidator(provider.GetValidator<{dtoTemplateName}>()!)");

            validationRuleChain.Metadata["requires-validator-provider"] = true;
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
            (!UniqueConstraintRules.TryGetMappedClass(dtoModel, out var classModel) && !UniqueConstraintRules.TryGetAdvancedMappedClass(dtoModel, associationedElements, out classModel)) ||
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
}
