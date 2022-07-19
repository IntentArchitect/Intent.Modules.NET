using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Decorators.ReturnTypes
{
    [IntentManaged(Mode.Merge)]
    public class AzureFunctionClassResourceLocationDecorator : AzureFunctionClassDecorator
    {
        [IntentManaged(Mode.Fully)] public const string DecoratorId = "Intent.AzureFunctions.ReturnTypes.AzureFunctionClassResourceLocationDecorator";

        [IntentManaged(Mode.Fully)] private readonly AzureFunctionClassTemplate _template;
        [IntentManaged(Mode.Fully)] private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public AzureFunctionClassResourceLocationDecorator(AzureFunctionClassTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override IEnumerable<string> GetRunMethodExitStatementList()
        {
            if (_template.Model.ReturnType == null
                || (_template.Model.ReturnType.Element.Id != TypeDefinitionIds.ResourceLocationVoidTypeDefId
                    && _template.Model.ReturnType.Element.Id != TypeDefinitionIds.ResourceLocationPayloadTypeDefId))
            {
                yield break;
            }

            yield return $"if (result?.HasLocation() == true)";
            yield return "{";

            switch (_template.Model.ReturnType.Element.Id)
            {
                case TypeDefinitionIds.ResourceLocationVoidTypeDefId:
                    yield return $@"    return new AcceptedResult(result.Location, null);";
                    break;
                case TypeDefinitionIds.ResourceLocationPayloadTypeDefId:
                    yield return $@"    return new AcceptedResult(result.Location, result.Payload);";
                    break;
            }

            yield return "}";
        }
    }
}