using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.FluentValidation.Shared;

public interface IFluentValidationTemplate : ICSharpFileBuilderTemplate
{
    string ToValidateTemplateId { get; }
    string DtoTemplateId { get; }
    string ValidatorProviderTemplateId { get; }
}