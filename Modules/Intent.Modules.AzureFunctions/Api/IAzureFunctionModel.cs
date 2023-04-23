using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;
using JetBrains.Annotations;

namespace Intent.AzureFunctions.Api;

public interface IAzureFunctionModel : IMetadataModel, IHasName, IHasTypeReference, IHasFolder<IFolder>
{
    TriggerType TriggerType { get; }
    string AuthorizationLevel { get;}
    IList<IAzureFunctionParameterModel> Parameters { get;}
    [CanBeNull] public string QueueName { get; }
    [CanBeNull] public string Connection { get; }
    [CanBeNull] string ScheduleExpression { get; }
    [CanBeNull] string EventHubName { get; }
    ITypeReference ReturnType { get; }
    bool IsMapped { get; }
    IElementMapping Mapping { get; }
    IElement InternalElement { get; }
}

public interface IAzureFunctionParameterModel : IMetadataModel, IHasName, IHasTypeReference
{
    HttpInputSource? InputSource { get; }
    bool IsMapped { get; }
    string MappedPath { get; }
}