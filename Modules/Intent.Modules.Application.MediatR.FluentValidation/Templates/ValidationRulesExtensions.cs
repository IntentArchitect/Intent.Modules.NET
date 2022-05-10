using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Application.MediatR.FluentValidation.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Utils;

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates
{
    public static class ValidationRulesExtensions
    {
        public static IEnumerable<string> GetValidationRules<TModel>(this CSharpTemplateBase<TModel> template, IEnumerable<DTOFieldModel> properties)
        {
            foreach (var property in properties)
            {
                var validations = new List<string>();
                if (!template.Types.Get(property.TypeReference).IsPrimitive && !property.TypeReference.IsNullable)
                {
                    validations.Add(".NotNull()");
                }
                if (property.TypeReference.Element.IsEnumModel())
                {
                    validations.Add(".IsInEnum()");
                }

                if (property.HasValidations())
                {
                    if (property.GetValidations().NotEmpty())
                    {
                        validations.Add(".NotEmpty()");
                    }

                    if (!string.IsNullOrWhiteSpace(property.GetValidations().Equal()))
                    {
                        validations.Add($".Equal({property.GetValidations().Equal()})");
                    }
                    if (!string.IsNullOrWhiteSpace(property.GetValidations().NotEqual()))
                    {
                        validations.Add($".NotEqual({property.GetValidations().NotEqual()})");
                    }

                    if (property.GetValidations().MinLength() != null && property.GetValidations().MaxLength() != null)
                    {
                        validations.Add($".Length({property.GetValidations().MinLength()}, {property.GetValidations().MaxLength()})");
                    }
                    else if (property.GetValidations().MinLength() != null)
                    {
                        validations.Add($".MinimumLength({property.GetValidations().MinLength()})");
                    }
                    else if (property.GetValidations().MaxLength() != null)
                    {
                        validations.Add($".MaximumLength({property.GetValidations().MaxLength()})");
                    }

                    if (property.GetValidations().Min() != null && property.GetValidations().Max() != null && 
                        int.TryParse(property.GetValidations().Min(), out var min) && int.TryParse(property.GetValidations().Max(), out var max))
                    {
                        validations.Add($".InclusiveBetween({min}, {max})");
                    }
                    else if (!string.IsNullOrWhiteSpace(property.GetValidations().Min()))
                    {
                        validations.Add($".GreaterThanOrEqualTo({property.GetValidations().Min()})");
                    }
                    else if (!string.IsNullOrWhiteSpace(property.GetValidations().Max()))
                    {
                        validations.Add($".LessThanOrEqualTo({property.GetValidations().Max()})");
                    }

                    if (!string.IsNullOrWhiteSpace(property.GetValidations().Predicate()))
                    {
                        var message = !string.IsNullOrWhiteSpace(property.GetValidations().PredicateMessage()) ? $".WithMessage(\"{property.GetValidations().PredicateMessage()}\")" : string.Empty;
                        validations.Add($".Must({property.GetValidations().Predicate()}){message}");
                    }
                    if (property.GetValidations().HasCustomValidation())
                    {
                        validations.Add($".Must(Validate{property.Name})");
                    }
                }
                if (!validations.Any(x => x.StartsWith(".MaximumLength")) && property.InternalElement.IsMapped)
                {
                    try
                    {
                        var attribute = property.InternalElement.MappedElement.Element.AsAttributeModel();
                        if (attribute.HasStereotype("Text Constraints") &&
                            attribute.GetStereotypeProperty<int?>("Text Constraints", "MaxLength") > 0 &&
                            property.GetValidations()?.MaxLength() == null)
                        {
                            validations.Add($".MaximumLength({attribute.GetStereotypeProperty<int>("Text Constraints", "MaxLength")})");
                        }
                    }
                    catch (Exception e)
                    {
                        Logging.Log.Debug("Could not resolve [Text Constraints] stereotype for Domain attribute: " + e.Message);
                    }
                }

                if (!validations.Any())
                {
                    continue;
                }

                yield return $@"RuleFor(v => v.{property.Name.ToPascalCase()})
                {string.Join($"{Environment.NewLine}                ", validations)};";
            }
        }

    }
}