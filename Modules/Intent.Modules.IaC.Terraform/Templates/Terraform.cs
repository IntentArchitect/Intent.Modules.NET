// ReSharper disable InconsistentNaming
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
    }
    
    public static class azurerm_servicebus_queue
    {
        public const string type = nameof(azurerm_servicebus_queue);
    }

    public static class azurerm_servicebus_subscription
    {
        public const string type = nameof(azurerm_servicebus_subscription);
    }
}