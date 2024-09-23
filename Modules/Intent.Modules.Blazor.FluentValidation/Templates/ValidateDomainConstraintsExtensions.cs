using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Blazor.FluentValidation.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Utils;

namespace Intent.Modules.FluentValidation.Shared;

public static class ValidateDomainConstraintsExtensions
{
    public static void AddMaxLengthValidatorsFromMappedDomain(
        this CSharpMethodChainStatement validationRuleChain,
        IElement model,
        IElement field)
    {
        var hasMappedAttribute = TryGetMappedAttribute(field, out var mappedAttribute) || TryGetAdvancedMappedAttribute(field, out mappedAttribute);
        if (hasMappedAttribute && !validationRuleChain.Statements.Any(x => x.HasMetadata("max-length")))
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
    }


    public static bool TryGetMappedAttribute(this IElement field, out IElement attribute)
    {
        var mappedElement = field.MappedElement?.Element as IElement;
        // The loop is not needed on the service side where a Command/Query/DTO is mapped
        // to an Attribute but it is needed when mapping from a Service Proxy to a DTO Field and then to an Attribute.
        while (mappedElement is not null)
        {
            if (mappedElement.SpecializationTypeId == DomainConstants.Attribute) // Domain Class Attribute
            {
                attribute = mappedElement;
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

    public static bool TryGetAdvancedMappedAttribute(this IElement field, out IElement attribute)
    {
        var mappedEnd = field.MappedToElements.FirstOrDefault(p => p.MappingType == "Data Mapping"); 
        if (mappedEnd != null)
        {
            if (mappedEnd.TargetElement?.SpecializationTypeId == DomainConstants.Attribute) // Domain Class Attribute
            {
                attribute = mappedEnd.TargetElement as IElement;
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