using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Aws.Lambda.Functions.Api;
using Intent.Modules.Aws.Lambda.Functions.Templates.LambdaFunctionClass;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Aws.Lambda.Functions.Dispatch.Wolverine.Templates.LambdaFunction;

[IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
internal class CqrsLambdaFunctionClassTemplateRegistration : FilePerModelTemplateRegistration<ILambdaFunctionContainerModel>
{
    private readonly IMetadataManager _metadataManager;

    public CqrsLambdaFunctionClassTemplateRegistration(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    public override string TemplateId => LambdaFunctionClassTemplate.TemplateId;

    [IntentManaged(Mode.Fully)]
    public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ILambdaFunctionContainerModel model)
    {
        return new LambdaFunctionClassTemplate(outputTarget, model);
    }

    [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
    public override IEnumerable<ILambdaFunctionContainerModel> GetModels(IApplication application)
    {
        var elementsGroupedByParent = Enumerable.Empty<IElement>()
            .Concat(_metadataManager.Services(application).GetCommandModels()
                .Where(x => x.HasHttpSettings())
                .Select(x => x.InternalElement))
            .Concat(_metadataManager.Services(application).GetQueryModels()
                .Where(x => x.HasHttpSettings())
                .Select(x => x.InternalElement))
            .GroupBy(x => x.ParentElement);

        return elementsGroupedByParent
            .Select(grouping => new CqrsLambdaFunctionContainerModel(
                parentElement: grouping.Key,
                elements: grouping,
                context: application))
            .ToArray();
    }
}
