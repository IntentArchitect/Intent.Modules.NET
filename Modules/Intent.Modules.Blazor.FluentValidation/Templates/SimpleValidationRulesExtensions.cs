using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Blazor.FluentValidation.Templates;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;

namespace Intent.Modules.FluentValidation.Shared;

public static class SimpleValidationRulesExtensions
{
    //public static bool HasValidationRules(
    //    DTOModel dtoModel,
    //    string dtoTemplateId,
    //    string dtoValidatorTemplateId,
    //    bool uniqueConstraintValidationEnabled,
    //    bool customValidationEnabled,
    //    List<Action<CSharpMethodChainStatement, IElement>> configureValidations)
    //{
    //    return GetValidationRulesStatements(
    //        template: default,
    //        dtoModel: dtoModel,
    //        dtoTemplateId: dtoTemplateId,
    //        dtoValidatorTemplateId: dtoValidatorTemplateId,
    //        configureValidations: configureValidations).Any();
    //}


    public static void ConfigureForValidation(
        this IFluentValidationTemplate template,
        IElement dtoModel,
        List<Action<CSharpMethodChainStatement, IElement>> configureFieldValidations = default,
        List<Action<CSharpMethodChainStatement>> configureClassValidations = default)
    {
        template.CSharpFile
            .AddUsing("FluentValidation")
            .AddClass($"{dtoModel.Name}Validator", @class =>
            {
                var toValidateTypeName = template.GetTypeName(template.ToValidateTemplateId, dtoModel);

                @class.AddMetadata("validator", true);
                @class.WithBaseType($"AbstractValidator<{toValidateTypeName}>");

                @class.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());

                @class.AddConstructor(ctor =>
                {
                    ctor.AddStatement(new CSharpStatement("ConfigureValidationRules();")
                        .AddMetadata("configure-validation-rules", true));
                    ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                });


                @class.AddMethod("void", "ConfigureValidationRules", method =>
                {
                    method.Private();

                    template.CSharpFile.AfterBuild(file =>
                    {
                        var validationRuleStatements = template.GetValidationRulesStatements(
                            dtoModel: dtoModel,
                            configureFieldValidations: configureFieldValidations ?? [],
                            configureClassValidations: configureClassValidations ?? []);

                        var valids = validationRuleStatements.ToList();

                        foreach (var propertyStatement in valids)
                        {
                            method.AddStatement(propertyStatement);
                        }
                    });
                });
            });
    }

    private static IEnumerable<CSharpMethodChainStatement> GetValidationRulesStatements(this IFluentValidationTemplate template,
        IElement dtoModel,
        List<Action<CSharpMethodChainStatement, IElement>> configureFieldValidations,
        List<Action<CSharpMethodChainStatement>> configureClassValidations)
    {
        var modelTemplate = template.GetTemplate<ICSharpFileBuilderTemplate>(template.ToValidateTemplateId, dtoModel);
        foreach (var field in dtoModel.ChildElements)
        {
            if (!modelTemplate.CSharpFile.Classes.First().TryGetReferenceForModel(field, out var reference) || reference is not CSharpProperty)
            {
                continue;
            }
            var validationRuleChain = new CSharpMethodChainStatement($"RuleFor(v => v.{reference.Name.ToPascalCase()})");
            validationRuleChain.AddMetadata("model", field);

            if (field.HasValidations())
            {
                var validations = field.GetValidations();
                if (validations.CascadeMode().Value != null)
                {
                    validationRuleChain.AddChainStatement($"Cascade(CascadeMode.{validations.CascadeMode().Value})");
                }
            }

            // If no template is present we still need a way to determine what
            // type is primitive.
            var resolvedTypeInfo = template is null
                ? new CSharpTypeResolver(
                    defaultCollectionFormatter: CSharpCollectionFormatter.Create("System.Collections.Generic.IEnumerable<{0}>"),
                    defaultNullableFormatter: null).Get(field.TypeReference)
                : template.GetTypeInfo(field.TypeReference);

            if (!resolvedTypeInfo.IsPrimitive &&
                !field.TypeReference.IsNullable)
            {
                validationRuleChain.AddChainStatement("NotNull()");
            }

            if (field.HasValidations())
            {
                AddValidatorsFromFluentValidationStereotype(validationRuleChain, field);
            }

            AddValidatorsBasedOnTypeReference(validationRuleChain, template, field);

            foreach (var configureValidation in configureFieldValidations)
            {
                configureValidation(validationRuleChain, field);
            }

            if (!validationRuleChain.Statements.Any())
            {
                continue;
            }

            yield return validationRuleChain;
        }
        if (configureClassValidations.Any())
        {
            var validationRuleChain = new CSharpMethodChainStatement("RuleFor(v => v)");

            foreach (var configureClassValidation in configureClassValidations)
            {
                configureClassValidation(validationRuleChain);
            }

            if (!validationRuleChain.Statements.Any())
            {
                yield break;
            }

            yield return validationRuleChain;
        }
    }

    private static void AddValidatorsFromFluentValidationStereotype(
        CSharpMethodChainStatement validationRuleChain,
        IElement field)
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
    }

    private static void AddValidatorsBasedOnTypeReference(
        CSharpMethodChainStatement validationRuleChain,
        IFluentValidationTemplate template,
        IElement property)
    {
        if (property.TypeReference.Element.IsEnumModel())
        {
            validationRuleChain.AddChainStatement(property.TypeReference.IsCollection
                ? "ForEach(x => x.IsInEnum())"
                : "IsInEnum()");
        }
        else if (property.TypeReference?.Element is not null &&
                 property.TypeReference.Element.IsDTOModel() &&
                 template?.TryGetTypeName(
                     templateId: template.Id,
                     model: property.TypeReference.Element.AsDTOModel(),
                     typeName: out _) == true &&
                 template.TryGetTypeName(
                     templateId: template.DtoTemplateId,
                     model: property.TypeReference.Element.AsDTOModel(),
                     typeName: out var dtoTemplateName))
        {
            EnsureValidatorProviderInjected(template);

            validationRuleChain.AddChainStatement(property.TypeReference.IsCollection
                ? $"ForEach(x => x.SetValidator(provider.GetValidator<{dtoTemplateName}>()!))"
                : $"SetValidator(provider.GetValidator<{dtoTemplateName}>()!)");
        }
    }

    // This approach is used for nested DTO Validation scenarios.
    // Nested DTO Validators are required to validate nested DTOs.
    // Instantiating nested DTO Validators will cause problems when dependencies
    // are injected. So instead of overburdening the entire Command / Query / DTO Validator templates
    // with dependency injection complexities, just inject the IValidatorProvider.
    private static void EnsureValidatorProviderInjected(
        IFluentValidationTemplate template)
    {
        var validatorClass = template.CSharpFile.Classes.First();

        var ctor = validatorClass.Constructors.First();

        var validatorProviderInterface = template.GetTypeName(template.ValidatorProviderTemplateId);
        if (ctor.Parameters.Any(p => p.Type.Equals(validatorProviderInterface)))
        {
            return;
        }

        ctor.Parameters.Insert(0, new CSharpConstructorParameter(validatorProviderInterface, "provider", ctor));
        ctor.FindStatements(stmt => stmt.HasMetadata("configure-validation-rules"))
            ?.ToList().ForEach(x => x.Remove());

        ctor.InsertStatement(0, new CSharpStatement("ConfigureValidationRules(provider);")
            .AddMetadata("configure-validation-rules", true));

        validatorClass.FindMethod(p => p.Name == "ConfigureValidationRules")
            ?.AddParameter(validatorProviderInterface, "provider");
    }

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


}