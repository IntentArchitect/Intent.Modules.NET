using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Application.FluentValidation.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using static Intent.Application.FluentValidation.Api.DTOFieldModelStereotypeExtensions;

namespace Intent.Modules.Application.FluentValidation.Templates
{
    public static class ValidationRulesExtensions
    {
        public static IEnumerable<string> GetValidationRules(IEnumerable<DTOFieldModel> fields)
        {
            return GetValidationRules<object>(null, fields);
        }

        public static IEnumerable<string> GetValidationRules<TModel>(this CSharpTemplateBase<TModel> template, IEnumerable<DTOFieldModel> fields)
        {
            var statements = GetValidationRulesStatements<TModel>(template, fields);
            foreach (var statement in statements)
            {
                yield return statement.ToString();
            }
        }

        public static IEnumerable<CSharpMethodChainStatement> GetValidationRulesStatements(IEnumerable<DTOFieldModel> fields)
        {
            return GetValidationRulesStatements<object>(null, fields);
        }

        public static IEnumerable<CSharpMethodChainStatement> GetValidationRulesStatements<TModel>(this CSharpTemplateBase<TModel> template, IEnumerable<DTOFieldModel> fields)
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
                    if (property.GetValidations().HasCustomValidation())
                    {
                        validations.AddChainStatement($"MustAsync(Validate{property.Name}Async)");
                    }
                }
                if (!validations.Statements.Any(x => x.GetText("").StartsWith("MaximumLength")) && property.InternalElement.MappedElement?.Element.IsAttributeModel() == true)
                {
                    try
                    {
                        var attribute = property.InternalElement.MappedElement.Element.AsAttributeModel();
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
    }
}