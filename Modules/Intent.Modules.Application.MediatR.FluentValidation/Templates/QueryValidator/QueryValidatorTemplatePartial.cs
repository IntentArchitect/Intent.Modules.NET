using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.FluentValidation.Shared.Templates.DtoValidator;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates.QueryValidator
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class QueryValidatorTemplate : DtoValidatorTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.FluentValidation.QueryValidator";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public QueryValidatorTemplate(IOutputTarget outputTarget, QueryModel model)
            : base(
                templateId: TemplateId,
                outputTarget: outputTarget,
                model: new DTOModel(model.InternalElement),
                dtoTemplateId: QueryModelsTemplate.TemplateId,
                modelParameterName: "command",
                model.GetConceptName())
        {
        }
    }
}