using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Blazor.FluentValidation.Templates;
using Intent.Modules.Blazor.FluentValidation.Templates.ModelDefinitionValidator;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using Intent.Templates;

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
        if (!template.TryGetModel<IElementWrapper>(out var componentModel))
        {
            return;
        }
        
        var validations = field.GetValidations();
        var @class = template.CSharpFile.Classes.First();
        
        if (!ValidationModelResolverHelper.TryGetDtoTypeName(template, componentModel.InternalElement, out var toValidateTypeName))
        {
            return;
        }
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
                method.AddStatement("// IntentInitialGen");
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
                    .AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge())
                    .Private()
                    .Async();
                method.AddParameter(toValidateTypeName, "model");
                method.AddParameter(template.GetTypeName(field.TypeReference), "value");
                method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                method.AddStatement("// IntentInitialGen");
                method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                method.AddStatement($"throw new {template.UseType("System.NotImplementedException")}(\"Your custom validation rules here...\");");
            });
        }
    }
}