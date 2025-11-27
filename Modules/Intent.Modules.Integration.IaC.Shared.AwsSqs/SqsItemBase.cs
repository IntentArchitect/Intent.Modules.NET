using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Integration.IaC.Shared.AwsSqs;

internal abstract record SqsItemBase : IHasStereotypes
{
    public string ApplicationId { get; init; }
    public string ApplicationName { get; init; }
    public SqsMethodType MethodType { get; init; }
    public string QueueName { get; init; }
    public string QueueConfigurationName { get; init; }

    public abstract string GetModelTypeName(IntentTemplateBase template);
    public abstract string GetSubscriberTypeName<T>(IntentTemplateBase<T> template);
    public abstract IEnumerable<IStereotype> Stereotypes { get; }
}
