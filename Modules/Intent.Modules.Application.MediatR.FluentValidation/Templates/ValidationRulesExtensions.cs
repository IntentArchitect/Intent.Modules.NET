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

                if (property.HasStringValidation())
                {
                    if (property.GetStringValidation().NotEmpty())
                    {
                        validations.Add(".NotEmpty()");
                    }
                    if (property.GetStringValidation().MaxLength() != null)
                    {
                        validations.Add($".MaximumLength({property.GetStringValidation().MaxLength()})");
                    }
                    if (property.GetStringValidation().HasCustomValidation())
                    {
                        validations.Add($".Must(Validate{property.Name})");
                    }
                }
                else if (property.InternalElement.IsMapped)
                {
                    try
                    {
                        var attribute = property.InternalElement.MappedElement.Element.AsAttributeModel();
                        if (attribute.HasStereotype("Text Constraints") &&
                            attribute.GetStereotypeProperty<int?>("Text Constraints", "MaxLength") > 0)
                        {
                            validations.Add(
                                $".MaximumLength({attribute.GetStereotypeProperty<int>("Text Constraints", "MaxLength")})");
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