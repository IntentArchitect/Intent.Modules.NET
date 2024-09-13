using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Blazor.FluentValidation.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.FluentValidation.Shared;

public static class CreateCustomValidationsExtensions
{
    public static void AddCustomValidations(
        this CSharpMethodChainStatement validationRuleChain,
        IFluentValidationTemplate template,
        IElement field)
    {
        if (!field.HasValidations())
        {
            return;
        }
        var validations = field.GetValidations();
        var @class = template.CSharpFile.Classes.First();

        var toValidateTypeName = template.GetTypeName(template.ToValidateTemplateId, (IMetadataModel)((ITemplateWithModel)template).Model);
        if (validations.Custom())
        {
            validationRuleChain.AddChainStatement($"CustomAsync(Validate{field.Name.ToPascalCase()}Async)");
            @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}", $"Validate{field.Name.ToPascalCase()}Async", method =>
            {
                method
                    .AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored())
                    .Private()
                    .Async();
                method.AddParameter(template.GetTypeName(field.TypeReference), "value");
                method.AddParameter($"ValidationContext<{toValidateTypeName}>", "validationContext");
                method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                method.AddStatement($"throw new {template.UseType("System.NotImplementedException")}(\"Your custom validation rules here...\");");
            });
        }

        if (validations.HasCustomValidation() ||
            validations.Must())
        {
            validationRuleChain.AddChainStatement($"MustAsync(Validate{field.Name.ToPascalCase()}Async)");
            @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}<bool>", $"Validate{field.Name.ToPascalCase()}Async", method =>
            {
                method
                    .AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored())
                    .Private()
                    .Async();
                method.AddParameter(toValidateTypeName, "model");
                method.AddParameter(template.GetTypeName(field.TypeReference), "value");
                method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                method.AddStatement($"throw new {template.UseType("System.NotImplementedException")}(\"Your custom validation rules here...\");");
            });
        }
    }
}

public static class ValidateDomainConstraintsExtensions
{
    public static void AddValidatorsFromMappedDomain(
        this CSharpMethodChainStatement validationRuleChain,
        IElement model,
        IElement field)
    {
        var hasMappedAttribute = TryGetMappedAttribute(field, out var mappedAttribute) || TryGetAdvancedMappedAttribute(model, field, out mappedAttribute);
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


    private static bool TryGetMappedAttribute(IElement field, out IElement attribute)
    {
        var mappedElement = field.MappedElement?.Element as IElement;
        // The loop is not needed on the service side where a Command/Query/DTO is mapped
        // to an Attribute but it is needed when mapping from a Service Proxy to a DTO Field and then to an Attribute.
        while (mappedElement is not null)
        {
            if (mappedElement.SpecializationTypeId == "0090fb93-483e-41af-a11d-5ad2dc796adf") // Domain Class Attribute
            {
                attribute = mappedElement;
                return true;
            }

            mappedElement = mappedElement.MappedElement?.Element as IElement;
        }

        attribute = default;
        return false;
    }

    private static bool TryGetAdvancedMappedAttribute(IElement model, IElement field, out IElement attribute)
    {
        var mappedEnd = field.MappedToElements.FirstOrDefault(p => p.MappingType == "Data Mapping"
            && p.TargetElement?.SpecializationTypeId == "0090fb93-483e-41af-a11d-5ad2dc796adf"); // Domain Class Attribute
        if (mappedEnd != null)
        {
            attribute = mappedEnd.TargetElement as IElement;
            return true;
        }

        attribute = null;
        return false;
    }
}