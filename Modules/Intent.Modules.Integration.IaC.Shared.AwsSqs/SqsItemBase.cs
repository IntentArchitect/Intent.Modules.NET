using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Integration.IaC.Shared.AwsSqs;

internal abstract record SqsItemBase : IHasStereotypes, IHasName, IHasFolder, IElementWrapper
{
    public string ApplicationId { get; init; }
    public string ApplicationName { get; init; }
    public SqsMethodType MethodType { get; init; }
    public string QueueName { get; init; }
    public string QueueConfigurationName { get; init; }

    public abstract string GetModelTypeName(IntentTemplateBase template);
    public abstract string GetSubscriberTypeName<T>(IntentTemplateBase<T> template);
    public abstract IEnumerable<IStereotype> Stereotypes { get; }
    public abstract string Name { get; }
    public abstract FolderModel Folder { get; }
    public abstract IElement InternalElement { get; }
}
