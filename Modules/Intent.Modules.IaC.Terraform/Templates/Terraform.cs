// ReSharper disable InconsistentNaming

using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.IaC.Shared.AzureEventGrid;
using Intent.Modules.Integration.IaC.Shared.AzureServiceBus;

namespace Intent.Modules.IaC.Terraform.Templates;

internal static class Terraform
{
    public static class azurerm_resource_group
    {
        public const string type = nameof(azurerm_resource_group);
        public static class main_rg
        {
            public const string refname = nameof(main_rg);
            
            private const string expression = $"{type}.{refname}";
            public const string name = $"{expression}.{nameof(name)}";
            public const string location = $"{expression}.{nameof(location)}";
        }
    }

    public static class azurerm_application_insights
    {
        public const string type = nameof(azurerm_application_insights);

        public static class app_insights
        {
            public const string refname = nameof(app_insights);
            
            private const string expression = $"{type}.{refname}";
            public const string instrumentation_key = $"{expression}.{nameof(instrumentation_key)}";
        }
    }

    public static class azurerm_servicebus_namespace
    {
        public const string type = nameof(azurerm_servicebus_namespace);

        public static class service_bus
        {
            public const string refname = nameof(service_bus);
            
            private const string expression = $"{type}.{refname}";
            public const string id = $"{expression}.{nameof(id)}";
            public const string default_primary_connection_string = $"{expression}.{nameof(default_primary_connection_string)}";
        }
    }

    public static class azurerm_servicebus_topic
    {
        public const string type = nameof(azurerm_servicebus_topic);

        public static ReferenceData topic(AzureServiceBusItemBase message)
        {
            var refname = message.QueueOrTopicName.ToSnakeCase();
            var expression = $"{type}.{refname}";
            return new (refname, $"{expression}.name", $"{expression}.id");
        }
    }
    
    public static class azurerm_servicebus_queue
    {
        public const string type = nameof(azurerm_servicebus_queue);
        
        public static ReferenceData queue(AzureServiceBusItemBase message)
        {
            var refname = message.QueueOrTopicName.ToSnakeCase();
            var expression = $"{type}.{refname}";
            return new (refname, $"{expression}.name", $"{expression}.id");
        }
    }

    public static class azurerm_servicebus_subscription
    {
        public const string type = nameof(azurerm_servicebus_subscription);

        public static ReferenceData subscription(AzureServiceBusItemBase message)
        {
            var refname = $"{message.ApplicationName.Replace(".", "_").ToSnakeCase()}_{message.QueueOrTopicName.ToSnakeCase()}";
            var expression = $"{type}.{refname}";
            return new (refname, $"{expression}.name", $"{expression}.id");
        }
    }

    public static class azurerm_eventgrid_topic
    {
        public const string type = nameof(azurerm_eventgrid_topic);
        
        public static EventGridTopicData topic(AzureEventGridMessage message)
        {
            var refname = message.TopicName.ToSnakeCase();
            var expression = $"{type}.{refname}";
            return new (refname, $"{expression}.id", $"{expression}.primary_access_key", $"{expression}.endpoint");
        }

        public static EventGridTopicData topicData(AzureEventGridMessage message)
        {
            var refname = message.TopicName.ToSnakeCase();
            var expression = $"data.{type}.{refname}";
            return new (refname, $"{expression}.id", $"{expression}.primary_access_key", $"{expression}.endpoint");
        }

        public record EventGridTopicData(string refname, string id, string primary_access_key, string endpoint);
    }

    public static class azurerm_eventgrid_domain
    {
        public const string type = nameof(azurerm_eventgrid_domain);
        
        public static EventGridDomainData domain(string domainName)
        {
            var refname = domainName.ToSnakeCase();
            var expression = $"{type}.{refname}";
            return new (refname, $"{expression}.id", $"{expression}.primary_access_key", $"{expression}.endpoint");
        }

        public static EventGridDomainData domainData(string domainName)
        {
            var refname = domainName.ToSnakeCase();
            var expression = $"data.{type}.{refname}";
            return new (refname, $"{expression}.id", $"{expression}.primary_access_key", $"{expression}.endpoint");
        }

        public record EventGridDomainData(string refname, string id, string primary_access_key, string endpoint);
    }

    public static class azurerm_eventgrid_domain_topic
    {
        public const string type = nameof(azurerm_eventgrid_domain_topic);
        
        public static EventGridDomainTopicData domainTopic(AzureEventGridMessage message)
        {
            var refname = message.TopicName.ToSnakeCase();
            var expression = $"{type}.{refname}";
            return new (refname, $"{expression}.id");
        }

        public static EventGridDomainTopicData domainTopicData(AzureEventGridMessage message)
        {
            var refname = message.TopicName.ToSnakeCase();
            var expression = $"data.{type}.{refname}";
            return new (refname, $"{expression}.id");
        }

        public record EventGridDomainTopicData(string refname, string id);
    }

    public static class azurerm_eventgrid_event_subscription
    {
        public const string type = nameof(azurerm_eventgrid_event_subscription);
    }

    public record ReferenceData(string refname, string name, string id);
}