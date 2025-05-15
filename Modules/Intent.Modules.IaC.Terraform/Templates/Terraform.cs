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
}