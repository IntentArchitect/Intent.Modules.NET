using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Eventing.Contracts.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Integration.IaC.Shared.AzureEventGrid;
using Intent.Modules.Integration.IaC.Shared.AzureServiceBus;

namespace Intent.Modules.IaC.Terraform.Templates;

// Need to hardcode stuff here for the time-being I'm afraid, until we get a more sustainable solution
// where other modules can expose themselves cross-app to this Terraform IaC module.
internal static class CompositeMessageBusHelper
{
    public const string DaprPubSub = "Intent.Dapr.AspNetCore.Pubsub";
    public const string AwsSqs = "Intent.Aws.Sqs";
    public const string AzureServiceBus = "Intent.Eventing.AzureServiceBus";
    public const string AzureQueueStorage = "Intent.Eventing.AzureQueueStorage";
    public const string AzureEventGrid = "Intent.Eventing.AzureEventGrid";
    public const string Kafka = "Intent.Eventing.Kafka";
    public const string MassTransit = "Intent.Eventing.MassTransit";
    public const string Solace = "Intent.Eventing.Solace";
    
    private static readonly HashSet<string> MessageBusModuleIds = 
    [
        DaprPubSub,
        AwsSqs,
        AzureServiceBus,
        AzureQueueStorage,
        AzureEventGrid,
        Kafka,
        MassTransit,
        Solace
    ];
    
    public static bool RequiresCompositeMessageBus(IApplicationConfig app)
    {
        return app.Modules.Count(x => MessageBusModuleIds.Contains(x.ModuleId)) > 1;
    }

    public static List<AzureEventGridMessage> FilterMessagesForApp(List<AzureEventGridMessage> messages, IApplicationConfig app)
    {
        var newList = new List<AzureEventGridMessage>();
        foreach (var message in messages)
        {
            if (HasMatchingStereotype(message, [Intent.Eventing.AzureEventGrid.Api.MessageModelStereotypeExtensions.AzureEventGrid.DefinitionId], app))
            {
                newList.Add(message);
            }
        }
        return newList;
    }
    
    public static List<AzureServiceBusItemBase> FilterMessagesForApp(List<AzureServiceBusItemBase> messages, IApplicationConfig app)
    {
        var newList = new List<AzureServiceBusItemBase>();
        foreach (var message in messages)
        {
            if (HasMatchingStereotype(message, [Intent.Eventing.AzureServiceBus.Api.MessageModelStereotypeExtensions.AzureServiceBus.DefinitionId], app))
            {
                newList.Add(message);
            }
        }
        return newList;
    }

    private static Dictionary<string, List<string>> GetMessageBusRegistry(IApplicationConfig app)
    {
        var registry = new Dictionary<string, List<string>>();
        
        if (app.Modules.Any(x => x.ModuleId == DaprPubSub))
        {
            const string DaprMessageBusId = "764c5213-7e84-4b10-9414-addde0c07b69";
            const string DaprStereotypeId = "ec96e452-9084-49bb-a883-aa42eb327fe7";
            registry.Add(DaprMessageBusId, [DaprStereotypeId]);
        }
        
        if (app.Modules.Any(x => x.ModuleId == AwsSqs))
        {
            const string AwsSqsMessageBusId = "cf57ada8-c5d7-4a86-8f78-b8ed74ca0123";
            const string AwsSqsStereotypeId = "74fbdee0-4098-4544-8ecf-f7c5787c78c3";
            registry.Add(AwsSqsMessageBusId, [AwsSqsStereotypeId]);
        }
        
        if (app.Modules.Any(x => x.ModuleId == AzureServiceBus))
        {
            const string AzureServiceBusMessageBusId = "caaac841-4a77-4147-b7d1-70273f0bd346";
            const string AzureServiceBusStereotypeId = "1f60bd15-005b-4184-8c12-c44c20158001";
            registry.Add(AzureServiceBusMessageBusId, [AzureServiceBusStereotypeId]);
        }

        if (app.Modules.Any(x => x.ModuleId == AzureQueueStorage))
        {
            const string AzureQueueStorageMessageBusId = "649d22c1-890a-4d65-a15c-bd3563993250";
            const string AzureQueueStorageStereotypeId = "7b57f640-600d-4b91-98a7-2a304c715f27";
            registry.Add(AzureQueueStorageMessageBusId, [AzureQueueStorageStereotypeId]);
        }

        if (app.Modules.Any(x => x.ModuleId == AzureEventGrid))
        {
            const string AzureEventGridMessageBusId = "48880079-8788-4c53-b7f3-0cbc7c4c8a88";
            const string AzureEventGridStereotypeId = "dca28d4b-c277-4fb3-afe0-17f35ea8b59b";
            const string AzureEventGridDomainStereotypeId = "b440c77b-3bde-4a96-bcb6-3289a23e5b1d";
            registry.Add(AzureEventGridMessageBusId, [AzureEventGridStereotypeId, AzureEventGridDomainStereotypeId]);
        }
        
        if (app.Modules.Any(x => x.ModuleId == Kafka))
        {
            const string KafkaMessageBusId = "c0f7166a-018a-453a-b9c0-48f690ec55fb";
            const string KafkaStereotypeId = "f18ed242-c439-4b46-834c-bc2947731486";
            registry.Add(KafkaMessageBusId, [KafkaStereotypeId]);
        }
        
        if (app.Modules.Any(x => x.ModuleId == MassTransit))
        {
            const string MassTransitMessageBusId = "c6185bdb-d69b-4db6-af0f-ef2fe07a783c";
            const string MessageTopologySettingsStereotypeId = "fc095295-eb25-470a-9ee5-19129919db2b";
            const string MassTransitMessageStereotypeId = "19664e24-b935-4822-ac61-26d47488be42";
            registry.Add(MassTransitMessageBusId, [MessageTopologySettingsStereotypeId, MassTransitMessageStereotypeId]);
        }
        
        if (app.Modules.Any(x => x.ModuleId == Solace))
        {
            const string SolaceMessageBusId = "8070fbd6-fc5e-444d-b0c8-fa47b4e87326";
            const string SolaceStereotypeId = "56e898f3-74db-486d-86f9-3e885e7509e6";
            registry.Add(SolaceMessageBusId, [SolaceStereotypeId]);
        }
        
        return registry;
    }
    
    private static bool HasMatchingStereotype<T>(T element, IReadOnlyList<string> brokerStereotypeNameOrIds, IApplicationConfig app)
        where T : IHasStereotypes
    {
        // Check element itself
        if (brokerStereotypeNameOrIds.Any(x => element.HasStereotype(x)))
        {
            return true;
        }

        var registry = GetMessageBusRegistry(app);
        
        // Check folder hierarchy
        if (element is IHasFolder hasFolder)
        {
            var currentFolder = hasFolder.Folder;
            while (currentFolder != null)
            {
                if (brokerStereotypeNameOrIds.Any(x => currentFolder.HasStereotype(x)) ||
                    currentFolder.GetMessageBus()?.Providers().Any(x => registry[x.Id].Any(y => brokerStereotypeNameOrIds.Any(z => z == y))) == true)
                {
                    return true;
                }

                currentFolder = currentFolder.Folder;
            }
        }

        // Check package level
        if (element is IElementWrapper wrapper)
        {
            var package = wrapper.InternalElement.Package;
            return brokerStereotypeNameOrIds.Any(x => package.HasStereotype(x)) ||
                   (package.IsEventingPackageModel() && new EventingPackageModel(package).GetMessageBus()?.Providers().Any(x => registry[x.Id].Any(y => brokerStereotypeNameOrIds.Any(z => z == y))) == true);
        }
        if (element is IElement el)
        {
            var package = el.Package;
            return brokerStereotypeNameOrIds.Any(x => package.HasStereotype(x)) ||
                   (package.IsEventingPackageModel() && new EventingPackageModel(package).GetMessageBus()?.Providers().Any(x => registry[x.Id].Any(y => brokerStereotypeNameOrIds.Any(z => z == y))) == true);
        }
        
        return false;
    }
}