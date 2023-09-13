using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.FluentValidation.Shared.Templates.DtoValidator;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates.CommandValidator
{
    [IntentManaged(Mode.Ignore)]
    public class CommandValidatorTemplate : DtoValidatorTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.FluentValidation.CommandValidator";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CommandValidatorTemplate(IOutputTarget outputTarget, CommandModel model)
            : base(
                templateId: TemplateId,
                outputTarget: outputTarget,
                model: new DTOModel(model.InternalElement),
                dtoTemplateId: CommandModelsTemplate.TemplateId,
                modelParameterName: "command",
                model.GetConceptName())
        {
        }
    }
}