using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Application.FluentValidation.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Utils;

namespace Intent.Modules.FluentValidation.Shared;

public static class ValidationRulesExtensions
{
    public static bool HasValidationRules(IEnumerable<DTOFieldModel> fields)
    {
        return GetValidationRulesStatements<object>(default, fields).Any();
    }

    public static void ConfigureForValidation<TModel>(
        this CSharpTemplateBase<TModel> template,
        CSharpClass @class,
        IList<DTOFieldModel> properties,
        string modelTypeName,
        string modelParameterName)
    {
        @class.WithBaseType($"AbstractValidator <{modelTypeName}>");
        @class.AddConstructor(ctor =>
        {
            ctor.AddStatement("ConfigureValidationRules();");
            ctor.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored().WithSignatureMerge());

        });

        @class.AddMethod("void", "ConfigureValidationRules", method =>
        {
            method.Private();
            method.AddAttribute(CSharpIntentManagedAttribute.Fully());

            foreach (var propertyStatement in template.GetValidationRulesStatements(properties))
            {
                method.AddStatement(propertyStatement);
            }
        });

        foreach (var property in properties)
        {
            var validations = property.GetValidations();
            if (validations == null)
            {
                continue;
            }

            if (validations.Custom())
            {
                @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}", $"Validate{property.Name.ToPascalCase()}Async", method =>
                {
                    method
                        .AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored())
                        .Private()
                        .Async();
                    method.AddParameter(template.GetTypeName(property), "value");
                    method.AddParameter($"ValidationContext<{modelTypeName}>", "validationContext");
                    method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                    method.AddStatement("throw new NotImplementedException(\"Your custom validation rules here...\");");
                });
            }

            if (validations.HasCustomValidation() ||
                validations.Must())
            {
                @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}<bool>", $"Validate{property.Name.ToPascalCase()}Async", method =>
                {
                    method
                        .AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored())
                        .Private()
                        .Async();
                    method.AddParameter(modelTypeName, modelParameterName);
                    method.AddParameter(template.GetTypeName(property), "value");
                    method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                    method.AddStatement("throw new NotImplementedException(\"Your custom validation rules here...\");");
                });
            }
        }
    }

    private static IEnumerable<CSharpMethodChainStatement> GetValidationRulesStatements<TModel>(this CSharpTemplateBase<TModel> template, IEnumerable<DTOFieldModel> fields)
    {
        // If no template is present we still need a way to determine what
        // type is primitive.
        var resolvedTypeInfo = template is null
            ? new CSharpTypeResolver(
                defaultCollectionFormatter: CSharpCollectionFormatter.Create("System.Collections.Generic.IEnumerable<{0}>"),
                defaultNullableFormatter: null)
            : template.Types;

        foreach (var property in fields)
        {
            var validations = new CSharpMethodChainStatement($"RuleFor(v => v.{property.Name.ToPascalCase()})");
            validations.AddMetadata("model", property);

            if (!resolvedTypeInfo.Get(property.TypeReference).IsPrimitive &&
                !property.TypeReference.IsNullable)
            {
                validations.AddChainStatement("NotNull()");
            }
            if (property.TypeReference.Element.IsEnumModel())
            {
                validations.AddChainStatement(property.TypeReference.IsCollection
                    ? "ForEach(x => x.IsInEnum())"
                    : "IsInEnum()");
            }

            if (property.HasValidations())
            {
                if (property.GetValidations().NotEmpty())
                {
                    validations.AddChainStatement("NotEmpty()");
                }

                if (!string.IsNullOrWhiteSpace(property.GetValidations().Equal()))
                {
                    validations.AddChainStatement($"Equal({property.GetValidations().Equal()})");
                }
                if (!string.IsNullOrWhiteSpace(property.GetValidations().NotEqual()))
                {
                    validations.AddChainStatement($"NotEqual({property.GetValidations().NotEqual()})");
                }

                if (property.GetValidations().MinLength() != null && property.GetValidations().MaxLength() != null)
                {
                    validations.AddChainStatement($"Length({property.GetValidations().MinLength()}, {property.GetValidations().MaxLength()})");
                }
                else if (property.GetValidations().MinLength() != null)
                {
                    validations.AddChainStatement($"MinimumLength({property.GetValidations().MinLength()})");
                }
                else if (property.GetValidations().MaxLength() != null)
                {
                    validations.AddChainStatement($"MaximumLength({property.GetValidations().MaxLength()})");
                }

                if (property.GetValidations().Min() != null && property.GetValidations().Max() != null &&
                    int.TryParse(property.GetValidations().Min(), out var min) && int.TryParse(property.GetValidations().Max(), out var max))
                {
                    validations.AddChainStatement($"InclusiveBetween({min}, {max})");
                }
                else if (!string.IsNullOrWhiteSpace(property.GetValidations().Min()))
                {
                    validations.AddChainStatement($"GreaterThanOrEqualTo({property.GetValidations().Min()})");
                }
                else if (!string.IsNullOrWhiteSpace(property.GetValidations().Max()))
                {
                    validations.AddChainStatement($"LessThanOrEqualTo({property.GetValidations().Max()})");
                }

                if (!string.IsNullOrWhiteSpace(property.GetValidations().Predicate()))
                {
                    var message = !string.IsNullOrWhiteSpace(property.GetValidations().PredicateMessage()) ? $".WithMessage(\"{property.GetValidations().PredicateMessage()}\")" : string.Empty;
                    validations.AddChainStatement($"Must({property.GetValidations().Predicate()}){message}");
                }

                if (property.GetValidations().Custom())
                {
                    validations.AddChainStatement($"CustomAsync(Validate{property.Name.ToPascalCase()}Async)");
                }

                if (property.GetValidations().HasCustomValidation() ||
                    property.GetValidations().Must())
                {
                    validations.AddChainStatement($"MustAsync(Validate{property.Name.ToPascalCase()}Async)");
                }
            }

            if (!validations.Statements.Any(x => x.GetText("").StartsWith("MaximumLength")) &&
                TryGetMappedAttribute(property, out var attribute))
            {
                try
                {
                    if (attribute.HasStereotype("Text Constraints") &&
                        attribute.GetStereotypeProperty<int?>("Text Constraints", "MaxLength") > 0 &&
                        property.GetValidations()?.MaxLength() == null)
                    {
                        validations.AddChainStatement($"MaximumLength({attribute.GetStereotypeProperty<int>("Text Constraints", "MaxLength")})");
                    }
                }
                catch (Exception e)
                {
                    Logging.Log.Debug("Could not resolve [Text Constraints] stereotype for Domain attribute: " + e.Message);
                }
            }

            if (!validations.Statements.Any())
            {
                continue;
            }

            yield return validations;
        }
    }

    private static bool TryGetMappedAttribute(DTOFieldModel field, out AttributeModel attribute)
    {
        var mappedElement = field.InternalElement.MappedElement?.Element as IElement;
        while (mappedElement != null)
        {
            if (mappedElement.IsAttributeModel())
            {
                attribute = mappedElement.AsAttributeModel();
                return true;
            }

            mappedElement = mappedElement.MappedElement?.Element as IElement;
        }

        attribute = default;
        return false;
    }
}