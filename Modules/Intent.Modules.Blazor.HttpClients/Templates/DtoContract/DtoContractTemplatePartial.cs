using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Application.FluentValidation.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Blazor.HttpClients.Templates.EnumContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Templates.DtoContract
{
    [IntentManaged(Mode.Ignore)]
    public class DtoContractTemplate : DtoContractTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.HttpClients.DtoContract";

        public DtoContractTemplate(IOutputTarget outputTarget, DTOModel model)
            : base(
                templateId: TemplateId,
                outputTarget: outputTarget,
                model: model,
                enumContractTemplateId: EnumContractTemplate.TemplateId)
        {
            CSharpFile.OnBuild(file => 
            {
                var typeResolver = this.Types;

                var @class = file.Classes.First();
                foreach (var field in model.Fields)
                {
                    var attributesToAdd = new Dictionary<string, Func<CSharpAttribute>>();
                    if (!typeResolver.Get(field.TypeReference).IsPrimitive &&
                        !field.TypeReference.IsNullable)
                    {
                        AddValidation(attributesToAdd, "IsRequired", () => new CSharpAttribute("Required").AddArgument($"ErrorMessage = \"{field.Name.ToSentenceCase()} is required.\""));
                    }

                    if (field.HasValidations())
                    {
                        if (field.GetValidations().NotEmpty())
                        {
                            AddValidation(attributesToAdd, "IsRequired", () => new CSharpAttribute("Required").AddArgument($"ErrorMessage = \"{field.Name.ToSentenceCase()} is required.\""));
                        }
                        if (field.GetValidations().MinLength() != null && field.GetValidations().MaxLength() != null)
                        {
                            AddValidation(attributesToAdd, "StringLength",
                                () => new CSharpAttribute("StringLength")
                                    .AddArgument($"{field.GetValidations().MaxLength()}, MinimumLength = {field.GetValidations().MinLength()}, ErrorMessage = \"{field.Name.ToSentenceCase()} must be between {field.GetValidations().MinLength()} and {field.GetValidations().MaxLength()} characters.\""));
                        }
                        else if (field.GetValidations().MinLength() != null)
                        {
                            AddValidation(attributesToAdd, "StringLength",
                                () => new CSharpAttribute("MinLength")
                                    .AddArgument($"{field.GetValidations().MinLength()}, ErrorMessage = \"{field.Name.ToSentenceCase()} must be {field.GetValidations().MinLength()} or more characters.\""));
                        }
                        else if (field.GetValidations().MaxLength() != null)
                        {
                            AddValidation(attributesToAdd, "StringLength",
                                () => new CSharpAttribute("MaxLength")
                                    .AddArgument($"{field.GetValidations().MaxLength()}, ErrorMessage = \"{field.Name.ToSentenceCase()} must be {field.GetValidations().MaxLength()} or less characters.\""));
                        }

                        if (field.GetValidations().Min() != null && field.GetValidations().Max() != null &&
                            int.TryParse(field.GetValidations().Min(), out var min) && int.TryParse(field.GetValidations().Max(), out var max))
                        {
                            AddValidation(attributesToAdd, "NumericRange",
                                () => new CSharpAttribute("Range")
                                    .AddArgument($"{min}, {max}, ErrorMessage = \"Value for {field.Name.ToSentenceCase()} must be between {min} and {max}.\""));
                        }
                        else if (!string.IsNullOrWhiteSpace(field.GetValidations().Min()) && int.TryParse(field.GetValidations().Min(), out var min2))
                        {
                            string limit = "double";
                            if (typeResolver.Get(field.TypeReference).Name == "int")
                            {
                                limit = "int";
                            }
                            AddValidation(attributesToAdd, "NumericRange",
                                () => new CSharpAttribute("Range")
                                    .AddArgument($"{min2}, {limit}.MaxValue, ErrorMessage = \"Value for {field.Name.ToSentenceCase()} must be more than {min2}.\""));
                        }
                        else if (!string.IsNullOrWhiteSpace(field.GetValidations().Max()) && int.TryParse(field.GetValidations().Max(), out var max2))
                        {
                            string limit = "double";
                            if (typeResolver.Get(field.TypeReference).Name == "int")
                            {
                                limit = "int";
                            }
                            AddValidation(attributesToAdd, "NumericRange",
                                () => new CSharpAttribute("Range")
                                    .AddArgument($"{limit}.MinValue, {max2}, ErrorMessage = \"Value for {field.Name.ToSentenceCase()} must be less than {max2}.\""));
                        }
                    }

                    if (!attributesToAdd.ContainsKey("StringLength") && TryGetMappedAttribute(field, out var attribute))
                    {
                        try
                        {
                            if (attribute.HasStereotype("Text Constraints") &&
                                attribute.GetStereotypeProperty<int?>("Text Constraints", "MaxLength") > 0 &&
                                field.GetValidations()?.MaxLength() == null)
                            {
                                AddValidation(attributesToAdd, "StringLength",
                                    () => new CSharpAttribute("MaxLength")
                                        .AddArgument($"{attribute.GetStereotypeProperty<int>("Text Constraints", "MaxLength")}, ErrorMessage = \"{field.Name.ToSentenceCase()} must be {attribute.GetStereotypeProperty<int>("Text Constraints", "MaxLength")} or less characters.\""));
                            }
                        }
                        catch (Exception e)
                        {
                            Logging.Log.Debug("Could not resolve [Text Constraints] stereotype for Domain attribute: " + e.Message);
                        }
                    }

                    if (attributesToAdd.Any())
                    {
                        file.AddUsing("System");
                        file.AddUsing("System.ComponentModel.DataAnnotations");
                        var property = @class.Properties.First(p => p.Name == field.Name.ToPascalCase());
                        foreach (var kvp in attributesToAdd)
                        {
                            property.AddAttribute(kvp.Value());
                        }
                    }
                }
            });
        }

        private void AddValidation(Dictionary<string, Func<CSharpAttribute>> attributesToAdd, string validationType, Func<CSharpAttribute> value)
        {
            if (!attributesToAdd.ContainsKey(validationType))
            {
                attributesToAdd.Add(validationType, value);
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
}
