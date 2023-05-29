using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.ReturnTypes.ResourceLocationClass", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application
{
    public class ResourceLocation
    {
        public ResourceLocation(string location)
        {
            Location = location;
        }

        public string Location { get; }

        public bool HasLocation()
        {
            return !string.IsNullOrEmpty(Location);
        }

        public static ResourceLocation ToLocation(string location)
        {
            return new ResourceLocation(location);
        }

        public static ResourceLocation<TPayload> ToLocation<TPayload>(string location, TPayload payload)
        {
            return new ResourceLocation<TPayload>(location, payload);
        }
    }

    public class ResourceLocation<TPayload> : ResourceLocation
    {
        public ResourceLocation(string location, TPayload payload)
            : base(location)
        {
            Payload = payload;
        }

        public TPayload Payload { get; }
    }
}