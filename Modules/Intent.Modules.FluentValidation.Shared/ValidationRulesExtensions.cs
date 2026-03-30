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
        var indexFields = GetUniqueConstraintFields(dtoModel, sourceElementAdvancedMappings, uniqueConstraintValidationEnabled);

        var hasNativeValidationRules = GetValidationRulesStatements<object>(
            template: null,
            dtoModel: dtoModel,
            dtoTemplateId: dtoTemplateId,
            dtoValidatorTemplateId: dtoValidatorTemplateId,
            indexFields: indexFields,
            customValidationEnabled: customValidationEnabled,
            sourceElementAdvancedMappings: sourceElementAdvancedMappings).Any();

        if (hasNativeValidationRules)
        {
            return true;
        }

        return HasDomainConstraintRules(dtoModel, sourceElementAdvancedMappings);
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

                    ((ICSharpFileBuilderTemplate)template).CSharpFile.AfterBuild(file =>
                    {
                        var validationRuleStatements = template.GetValidationRulesStatements(
                                dtoModel: dtoModel,
                                dtoTemplateId: dtoTemplateId,
                                dtoValidatorTemplateId: dtoValidatorTemplateId,
                                indexFields: indexFields,
                                customValidationEnabled: customValidationEnabled,
                                sourceElementAdvancedMappings: associationedElements)
                            .ToList();

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

                        if (!validationRuleStatements.Any())
                        {
                            method.AddStatement("// Implement custom validation logic here if required");
                        }
                    }, 0);
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
            }).AfterBuild(file =>
            {
                var @class = file.Classes.FirstOrDefault();

                // if sonar qube module is installed, then add the appropriate attribute to suppress the warning
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

    private static IEnumerable<CSharpMethodChainStatement> GetValidationRulesStatements<TModel>(this CSharpTemplateBase<TModel>? template,
        DTOModel dtoModel,
        string dtoTemplateId,
        string dtoValidatorTemplateId,
        IReadOnlyCollection<ConstraintField> indexFields,
        bool customValidationEnabled,
        IEnumerable<IAssociationEnd>? sourceElementAdvancedMappings)
    {
        // If no template is present, we still need a way to determine what
        // type is primitive.
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

            if (field.HasValidations())
            {
                AddValidatorsFromFluentValidationStereotype(field, validationRuleChain, customValidationEnabled, template);
            }


            AddValidatorsFromMappedDomain(validationRuleChain, field, indexFields, sourceElementAdvancedMappings, dtoModel);

            AddValidatorsBasedOnTypeReference(template, validationRuleChain, dtoTemplateId, dtoValidatorTemplateId, dtoModel, field);

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

    private static void AddValidatorsFromFluentValidationStereotype<TModel>(
        DTOFieldModel field,
        CSharpMethodChainStatement validationRuleChain,
        bool customValidationEnabled,
        CSharpTemplateBase<TModel> template)
    {
        var validations = field.GetValidations();

        if (validations.NotEmpty())
        {
            validationRuleChain.AddChainStatement("NotEmpty()",
                stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Required));
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
            validationRuleChain.AddChainStatement($"Length({validations.MinLength()}, {validations.MaxLength()})",
                stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Length));
        }
        else if (validations.MinLength() != null)
        {
            validationRuleChain.AddChainStatement($"MinimumLength({validations.MinLength()})",
                stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.LengthMin));
        }
        else if (validations.MaxLength() != null)
        {
            validationRuleChain.AddChainStatement($"MaximumLength({validations.MaxLength()})",
                stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.LengthMax));
        }

        if (validations.Min() != null && validations.Max() != null &&
            int.TryParse(validations.Min(), out var min) && int.TryParse(validations.Max(), out var max))
        {
            validationRuleChain.AddChainStatement($"InclusiveBetween({min}, {max})",
                stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Numeric));
        }
        else if (!string.IsNullOrWhiteSpace(validations.Min()))
        {
            validationRuleChain.AddChainStatement($"GreaterThanOrEqualTo({validations.Min()})",
                stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.NumericMin));
        }
        else if (!string.IsNullOrWhiteSpace(validations.Max()))
        {
            validationRuleChain.AddChainStatement($"LessThanOrEqualTo({validations.Max()})",
                stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.NumericMax));
        }

        if (!string.IsNullOrWhiteSpace(validations.RegularExpression()))
        {
            var invocation = new CSharpInvocationStatement($"new {template?.UseType("System.Text.RegularExpressions.Regex") ?? "System.Text.RegularExpressions.Regex"}")
                            .AddArgument($"@\"{validations.RegularExpression()}\"")
                            .AddArgument("RegexOptions.Compiled")
                            .AddArgument($"{template?.UseType("System.TimeSpan") ?? "System.TimeSpan"}.FromSeconds({validations.RegularExpressionTimeout() ?? 1})")
                            .WithoutSemicolon();

            // if the template is null for use the less efficient method of putting the Regex declaration in the Matches call
            if (template is null || template is not ICSharpFileBuilderTemplate)
            {
                validationRuleChain.AddChainStatement($@"Matches({invocation})",
                    stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Regex));
            }
            else
            {
                if (template is ICSharpFileBuilderTemplate builderTemplate)
                {
                    // really should always be a class, just double checking
                    if (builderTemplate.CSharpFile.Classes.Any())
                    {
                        var regexName = $"{field.Name}Regex";

                        var @class = builderTemplate.CSharpFile.Classes.First();
                        if (!@class.Fields.Any(f => f.Name == regexName))
                        {
                            @class.AddField(builderTemplate.UseType("System.Text.RegularExpressions.Regex"), regexName, @field =>
                            {
                                field.Static().PrivateReadOnly();
                                field.WithAssignment(invocation);
                            });

                            validationRuleChain.AddChainStatement($@"Matches({regexName})",
                                stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Regex));
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(validations.RegularExpressionMessage()))
            {
                validationRuleChain.AddChainStatement($@"WithMessage(""{validations.RegularExpressionMessage()}"")");
            }
        }

        if (validations.EmailAddress())
        {
            validationRuleChain.AddChainStatement("EmailAddress()",
                stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Email));
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

                if (!string.IsNullOrWhiteSpace(validations.MustMessage()))
                {
                    validationRuleChain.AddChainStatement($@"WithMessage(""{validations.MustMessage()}"")");
                }
            }
        }
    }

    private static void AddValidatorsFromMappedDomain(
        CSharpMethodChainStatement validationRuleChain,
        DTOFieldModel field,
        IReadOnlyCollection<ConstraintField> indexFields,
        IEnumerable<IAssociationEnd>? associationedElements,
        DTOModel dtoModel)
    {
        var hasMappedAttribute = TryGetMappedAttribute(field, out var mappedAttribute) ||
                                 TryGetAdvancedMappedAttribute(field, out mappedAttribute) ||
                                 TryGetAssociationMappedAttribute(field, associationedElements, out mappedAttribute);
        if (!HasRuleForSpace(validationRuleChain, RuleSpace.LengthMax) && hasMappedAttribute)
        {
            try
            {
                if (mappedAttribute.HasStereotype("Text Constraints") &&
                    mappedAttribute.GetStereotypeProperty<int?>("Text Constraints", "MaxLength") > 0 &&
                    field.GetValidations()?.MaxLength() == null)
                {
                    validationRuleChain.AddChainStatement(
                        $"MaximumLength({mappedAttribute.GetStereotypeProperty<int>("Text Constraints", "MaxLength")})",
                        stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.LengthMax));
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

        if (hasMappedAttribute)
        {
            AddDomainConstraintValidators(validationRuleChain, field, mappedAttribute!, dtoModel);
        }
    }

    private static bool TryGetAssociationMappedAttribute(
        DTOFieldModel field,
        IEnumerable<IAssociationEnd>? associationedElements,
        out AttributeModel attribute)
    {
        var parentAssociations = (field.InternalElement.ParentElement as IElement)?.AssociatedElements;
        var associations = associationedElements?.Any() == true
            ? associationedElements
            : parentAssociations;
        if (associations is null)
        {
            attribute = null;
            return false;
        }

        foreach (var associationEnd in associations)
        {
            foreach (var mapping in associationEnd.Mappings)
            {
                var mappedEnd = mapping.MappedEnds.FirstOrDefault(p =>
                    p.MappingType == "Data Mapping" &&
                    (p.SourceElement as IElement)?.Id == field.Id);

                if (mappedEnd?.TargetElement is IElement targetElement &&
                    targetElement.IsAttributeModel())
                {
                    attribute = targetElement.AsAttributeModel();
                    return true;
                }
            }
        }

        attribute = null;
        return false;
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
            // We have a self-referencing DTO
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
            (!TryGetMappedClass(dtoModel, out var classModel) && !TryGetAdvancedMappedClass(dtoModel, associationedElements, out classModel)) ||
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
        IEnumerable<IAssociationEnd>? sourceElementAdvancedMappings,
        bool enabled)
    {
        if (!enabled || (!IsCreateDto(dtoModel) && !IsUpdateDto(dtoModel)))
        {
            return ArraySegment<ConstraintField>.Empty;
        }

        var hasMappedClass = TryGetMappedClass(dtoModel, out var mappedClass) || TryGetAdvancedMappedClass(dtoModel, sourceElementAdvancedMappings, out mappedClass);
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

    private static bool TryGetAdvancedMappedClass(DTOModel dtoModel, IEnumerable<IAssociationEnd>? associationedElements, out ClassModel? classModel)
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
                var mappedEnd = mapping.MappedEnds.FirstOrDefault(p =>
                {
                    var possibleDto = (p.SourceElement as IElement)?.ParentElement;
                    return p.MappingType == "Data Mapping" && possibleDto is not null && possibleDto.Id == dtoModel.Id;
                });
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
    private static bool TryGetMappedAttribute(IElement field, out AttributeModel attribute, bool checkAdvancedMappings = true)
    {
        var mappedElement = field?.MappedElement?.Element as IElement;
        // The loop is not needed on the service side where a Command/Query/DTO is mapped
        // to an Attribute but it is needed when mapping from a Service Proxy to a DTO Field and then to an Attribute.
        while (mappedElement is not null)
        {
            if (mappedElement.IsAttributeModel())
            {
                attribute = mappedElement.AsAttributeModel();
                return true;
            }

            if (checkAdvancedMappings && mappedElement.MappedElement?.Element is null)
            {
                return TryGetAdvancedMappedAttribute(mappedElement, out attribute, checkBasicMappings: false);
            }
            mappedElement = mappedElement.MappedElement?.Element as IElement;
        }

        attribute = default;
        return false;
    }

    private static bool TryGetAdvancedMappedAttribute(DTOFieldModel field, out AttributeModel attribute) => TryGetAdvancedMappedAttribute(field.InternalElement, out attribute);
    private static bool TryGetAdvancedMappedAttribute(IElement field, out AttributeModel attribute, bool checkBasicMappings = true)
    {
        var mappedEnd = field.MappedToElements.FirstOrDefault(p => p.MappingType == "Data Mapping");
        if (mappedEnd != null)
        {
            if (mappedEnd.TargetElement.IsAttributeModel())
            {
                attribute = mappedEnd.TargetElement.AsAttributeModel();
                return true;
            }

            if (checkBasicMappings && TryGetMappedAttribute((IElement)mappedEnd.TargetElement, out attribute, checkAdvancedMappings: false))
            {
                return true;
            }
        }

        attribute = null;
        return false;
    }

    // Domain Constraints stereotype DefinitionIds (package Intent.Metadata.Domain.Constraints).
    // Using DefinitionIds instead of display names avoids any compile-time coupling to the
    // Intent.Metadata.Domain.Constraints assembly and prevents false positives from
    // identically-named stereotypes in other packages.
    private const string DcRequired          = "14680476-e24a-490f-ba44-75eb8dc6fb46";
    private const string DcTextLimits        = "13649b19-4dfe-43ec-967f-0b85a5801dd6";
    private const string DcNumericLimits     = "cb14e47d-672c-4244-8950-7c4ebf8cf8ed";
    private const string DcCollectionLimits  = "06daef0d-5be0-43e0-9cc6-2bb8ea35dc86";
    private const string DcRegularExpression = "3dd144bc-374b-4acd-841a-7323210df66d";
    private const string DcEmail             = "9fb8d1b1-39b3-4f16-88e0-34d24a4e9bf6";

    // Numeric type DefinitionIds from the domain model — used for proper literal suffixes.
    private const string TypeIdDecimal = "675c7b84-997a-44e0-82b9-cd724c07c9e6";
    private const string TypeIdFloat   = "341929e9-e3e7-46aa-acb3-b0438421f4c4";

    /// <summary>
    /// Returns true when any field in <paramref name="dtoModel"/> maps to a domain attribute
    /// that carries at least one Domain Constraints stereotype. Detection is purely metadata-driven
    /// via DefinitionId — no reference to Intent.Metadata.Domain.Constraints is required.
    /// </summary>
    private static bool HasDomainConstraintRules(
        DTOModel dtoModel,
        IEnumerable<IAssociationEnd>? sourceElementAdvancedMappings)
    {
        foreach (var field in dtoModel.Fields)
        {
            if (!TryGetMappedAttribute(field, out var attr) &&
                !TryGetAdvancedMappedAttribute(field, out attr) &&
                !TryGetAssociationMappedAttribute(field, sourceElementAdvancedMappings, out attr))
            {
                continue;
            }

            if (attr.HasStereotype(DcRequired) ||
                attr.HasStereotype(DcTextLimits) ||
                attr.HasStereotype(DcNumericLimits) ||
                attr.HasStereotype(DcCollectionLimits) ||
                attr.HasStereotype(DcRegularExpression) ||
                attr.HasStereotype(DcEmail))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Appends FluentValidation chain statements derived from Domain Constraints stereotypes
    /// on <paramref name="mappedAttribute"/>. Uses generic stereotype access via DefinitionId
    /// so there is no compile-time reference to Intent.Metadata.Domain.Constraints.
    /// Conflict detection is performed via <see cref="HasRuleForSpace"/> — a rule is skipped
    /// if the same rule-space is already occupied by a higher-priority statement (e.g. an
    /// explicit FluentValidation stereotype rule on the DTO field).
    /// </summary>
    private static void AddDomainConstraintValidators(
        CSharpMethodChainStatement validationRuleChain,
        DTOFieldModel field,
        AttributeModel mappedAttribute,
        DTOModel dtoModel)
    {
        // Required → NotEmpty
        if (mappedAttribute.HasStereotype(DcRequired) && !HasRuleForSpace(validationRuleChain, RuleSpace.Required))
        {
            validationRuleChain.AddChainStatement("NotEmpty()",
                stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Required));
        }

        // Text Limits → MinimumLength / MaximumLength / Length(min, max)
        if (mappedAttribute.HasStereotype(DcTextLimits))
        {
            var textLimits   = mappedAttribute.GetStereotype(DcTextLimits);
            var minLengthStr = textLimits?.GetProperty("Min Length")?.Value;
            var maxLengthStr = textLimits?.GetProperty("Max Length")?.Value;
            var hasMin       = int.TryParse(minLengthStr, out var minLength);
            var hasMax       = int.TryParse(maxLengthStr, out var maxLength);

            if (hasMin && hasMax &&
                !HasRuleForSpace(validationRuleChain, RuleSpace.LengthMin) &&
                !HasRuleForSpace(validationRuleChain, RuleSpace.LengthMax))
            {
                validationRuleChain.AddChainStatement($"Length({minLength}, {maxLength})",
                    stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Length));
            }
            else if (hasMin && !HasRuleForSpace(validationRuleChain, RuleSpace.LengthMin))
            {
                validationRuleChain.AddChainStatement($"MinimumLength({minLength})",
                    stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.LengthMin));
            }
            else if (hasMax && !HasRuleForSpace(validationRuleChain, RuleSpace.LengthMax))
            {
                validationRuleChain.AddChainStatement($"MaximumLength({maxLength})",
                    stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.LengthMax));
            }
        }

        // Numeric Limits → GreaterThanOrEqualTo / LessThanOrEqualTo / InclusiveBetween
        if (mappedAttribute.HasStereotype(DcNumericLimits))
        {
            var numericLimits = mappedAttribute.GetStereotype(DcNumericLimits);
            var minValStr     = numericLimits?.GetProperty("Min Value")?.Value;
            var maxValStr     = numericLimits?.GetProperty("Max Value")?.Value;
            var hasMin        = !string.IsNullOrWhiteSpace(minValStr);
            var hasMax        = !string.IsNullOrWhiteSpace(maxValStr);

            if (hasMin && hasMax &&
                !HasRuleForSpace(validationRuleChain, RuleSpace.NumericMin) &&
                !HasRuleForSpace(validationRuleChain, RuleSpace.NumericMax))
            {
                var minLit = FormatNumericLiteral(minValStr!, mappedAttribute);
                var maxLit = FormatNumericLiteral(maxValStr!, mappedAttribute);
                validationRuleChain.AddChainStatement($"InclusiveBetween({minLit}, {maxLit})",
                    stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Numeric));
            }
            else if (hasMin && !HasRuleForSpace(validationRuleChain, RuleSpace.NumericMin))
            {
                validationRuleChain.AddChainStatement(
                    $"GreaterThanOrEqualTo({FormatNumericLiteral(minValStr!, mappedAttribute)})",
                    stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.NumericMin));
            }
            else if (hasMax && !HasRuleForSpace(validationRuleChain, RuleSpace.NumericMax))
            {
                validationRuleChain.AddChainStatement(
                    $"LessThanOrEqualTo({FormatNumericLiteral(maxValStr!, mappedAttribute)})",
                    stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.NumericMax));
            }
        }

        // Collection Limits → Must(c => c.Count >= min && c.Count <= max)
        if (mappedAttribute.HasStereotype(DcCollectionLimits))
        {
            var collectionLimits = mappedAttribute.GetStereotype(DcCollectionLimits);
            var minLengthStr     = collectionLimits?.GetProperty("Min Length")?.Value;
            var maxLengthStr     = collectionLimits?.GetProperty("Max Length")?.Value;
            var hasMin           = int.TryParse(minLengthStr, out var minLength);
            var hasMax           = int.TryParse(maxLengthStr, out var maxLength);

            if (hasMin && hasMax &&
                !HasRuleForSpace(validationRuleChain, RuleSpace.CollectionMin) &&
                !HasRuleForSpace(validationRuleChain, RuleSpace.CollectionMax))
            {
                validationRuleChain.AddChainStatement(
                    $"Must(c => c?.Count >= {minLength} && c?.Count <= {maxLength})",
                    stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Collection));
                validationRuleChain.AddChainStatement(
                    $@"WithMessage(""'{{PropertyName}}' must contain between {minLength} and {maxLength} items."")");
            }
            else if (hasMin && !HasRuleForSpace(validationRuleChain, RuleSpace.CollectionMin))
            {
                validationRuleChain.AddChainStatement($"Must(c => c?.Count >= {minLength})",
                    stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.CollectionMin));
                validationRuleChain.AddChainStatement(
                    $@"WithMessage(""'{{PropertyName}}' must contain at least {minLength} items."")");
            }
            else if (hasMax && !HasRuleForSpace(validationRuleChain, RuleSpace.CollectionMax))
            {
                validationRuleChain.AddChainStatement($"Must(c => c?.Count <= {maxLength})",
                    stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.CollectionMax));
                validationRuleChain.AddChainStatement(
                    $@"WithMessage(""'{{PropertyName}}' must contain at most {maxLength} items."")");
            }
        }

        // Regular Expression → Matches
        if (mappedAttribute.HasStereotype(DcRegularExpression) && !HasRuleForSpace(validationRuleChain, RuleSpace.Regex))
        {
            var pattern = mappedAttribute.GetStereotype(DcRegularExpression)?.GetProperty("Pattern")?.Value;
            if (!string.IsNullOrWhiteSpace(pattern))
            {
                var escaped = pattern!.Replace("\"", "\"\"");
                validationRuleChain.AddChainStatement($@"Matches(@""{escaped}"")",
                    stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Regex));
                var message = mappedAttribute.GetStereotype(DcRegularExpression)?.GetProperty("Message")?.Value;
                if (!string.IsNullOrWhiteSpace(message))
                {
                    validationRuleChain.AddChainStatement($@"WithMessage(@""{message}"")",
                        stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Regex));
                }
            }
        }

        // Email → EmailAddress
        if (mappedAttribute.HasStereotype(DcEmail) && !HasRuleForSpace(validationRuleChain, RuleSpace.Email))
        {
            validationRuleChain.AddChainStatement("EmailAddress()",
                stmt => stmt.AddMetadata(RuleSpaceMetadataKey, RuleSpace.Email));
        }
    }

    // Metadata key used to tag each validation chain statement with the rule-space it occupies.
    // Downstream conflict detection (e.g. domain constraints vs. explicit stereotype rules) is
    // done through HasRuleForSpace() rather than fragile string-matching on statement text.
    private const string RuleSpaceMetadataKey = "rule-space";

    private static class RuleSpace
    {
        public const string Required      = "required";
        public const string LengthMax     = "length.max";
        public const string LengthMin     = "length.min";
        /// <summary>Covers both <see cref="LengthMin"/> and <see cref="LengthMax"/>.</summary>
        public const string Length        = "length";
        public const string NumericMin    = "numeric.min";
        public const string NumericMax    = "numeric.max";
        /// <summary>Covers both <see cref="NumericMin"/> and <see cref="NumericMax"/>.</summary>
        public const string Numeric       = "numeric";
        public const string CollectionMin = "collection.min";
        public const string CollectionMax = "collection.max";
        /// <summary>Covers both <see cref="CollectionMin"/> and <see cref="CollectionMax"/>.</summary>
        public const string Collection    = "collection";
        public const string Regex         = "pattern.regex";
        public const string Email         = "format.email";
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="chain"/> already contains a statement
    /// that occupies <paramref name="space"/>. Combined spaces (e.g. <c>"length"</c>) are treated
    /// as covering their constituent parts (<c>"length.min"</c> and <c>"length.max"</c>), so asking
    /// for <c>"length.max"</c> returns <see langword="true"/> when a <c>"length"</c> statement is
    /// present — preventing a redundant MaximumLength from being added alongside a Length(min,max).
    /// </summary>
    private static bool HasRuleForSpace(CSharpMethodChainStatement chain, string space)
    {
        return chain.Statements.Any(s =>
        {
            if (!s.TryGetMetadata<string>(RuleSpaceMetadataKey, out var occupied))
                return false;

            if (string.Equals(occupied, space, StringComparison.Ordinal))
                return true;

            // Combined spaces cover their constituent parts
            if (occupied == RuleSpace.Length &&
                (space == RuleSpace.LengthMin || space == RuleSpace.LengthMax))
                return true;

            if (occupied == RuleSpace.Numeric &&
                (space == RuleSpace.NumericMin || space == RuleSpace.NumericMax))
                return true;

            if (occupied == RuleSpace.Collection &&
                (space == RuleSpace.CollectionMin || space == RuleSpace.CollectionMax))
                return true;

            return false;
        });
    }

    /// <summary>
    /// Returns the C# numeric literal string for <paramref name="value"/> with the correct
    /// suffix for the type of <paramref name="attribute"/> (e.g. <c>m</c> for decimal, <c>f</c>
    /// for float). Integer and double types need no suffix.
    /// </summary>
    private static string FormatNumericLiteral(string value, AttributeModel attribute)
    {
        var typeId = attribute.TypeReference?.Element?.Id ?? string.Empty;

        if (string.Equals(typeId, TypeIdDecimal, StringComparison.OrdinalIgnoreCase))
            return value + "m";

        if (string.Equals(typeId, TypeIdFloat, StringComparison.OrdinalIgnoreCase))
            return value + "f";

        return value;
    }
}