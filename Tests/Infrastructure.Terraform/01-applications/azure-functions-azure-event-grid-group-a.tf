resource "random_string" "azure-functions-azure-event-grid-group-a" {
  length  = 3
  special = false
  upper   = false
}

locals {
  azure-functions-azure-event-grid-group-a_app_name     = "azure-functions-azure-event-grid-group-a"
  azure_functions_azure_event_grid_group_a_storage_name = "azurefunction${random_string.azure-functions-azure-event-grid-group-a.result}storage"
}

# Hosting Plan (App Service Plan)
resource "azurerm_service_plan" "azure-functions-azure-event-grid-group-a_function_plan" {
  name                = "asp-${local.azure-functions-azure-event-grid-group-a_app_name}"
  location            = azurerm_resource_group.main_rg.location
  resource_group_name = azurerm_resource_group.main_rg.name
  os_type             = "Windows"
  sku_name            = "Y1"
}

# Storage Account
resource "azurerm_storage_account" "azure_functions_azure_event_grid_group_a_storage_name" {
  name                     = local.azure_functions_azure_event_grid_group_a_storage_name
  location                 = azurerm_resource_group.main_rg.location
  resource_group_name      = azurerm_resource_group.main_rg.name
  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "StorageV2"
}

# Function App
resource "azurerm_windows_function_app" "azure-functions-azure-event-grid-group-a_function_app" {
  name                       = local.azure-functions-azure-event-grid-group-a_app_name
  location                   = azurerm_resource_group.main_rg.location
  resource_group_name        = azurerm_resource_group.main_rg.name
  service_plan_id            = azurerm_service_plan.azure-functions-azure-event-grid-group-a_function_plan.id
  storage_account_name       = azurerm_storage_account.azure_functions_azure_event_grid_group_a_storage_name.name
  storage_account_access_key = azurerm_storage_account.azure_functions_azure_event_grid_group_a_storage_name.primary_access_key

  site_config {
    application_stack {
      dotnet_version = "v8.0"
    }
    cors {
      allowed_origins = [ "https://portal.azure.com" ]
    }
  }

  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY"               = azurerm_application_insights.app_insights.instrumentation_key
    "AzureWebJobsStorage"                          = "DefaultEndpointsProtocol=https;AccountName=${azurerm_storage_account.azure_functions_azure_event_grid_group_a_storage_name.name};AccountKey=${azurerm_storage_account.azure_functions_azure_event_grid_group_a_storage_name.primary_access_key};EndpointSuffix=core.windows.net"
    "FUNCTIONS_WORKER_RUNTIME"                     = "dotnet-isolated"
    "EventGrid:Topics:OrderCreatedEvent:Source"    = "order-created-event"
    "EventGrid:Domains:MainDomain:Key"             = azurerm_eventgrid_domain.main_domain.primary_access_key
    "EventGrid:Domains:MainDomain:Endpoint"        = azurerm_eventgrid_domain.main_domain.endpoint
    "EventGrid:Topics:ClientCreatedEvent:Source"   = "client-created-event"
    "EventGrid:Topics:ClientCreatedEvent:Key"      = azurerm_eventgrid_topic.client_created_event.primary_access_key
    "EventGrid:Topics:ClientCreatedEvent:Endpoint" = azurerm_eventgrid_topic.client_created_event.endpoint
  }
}

# Output variables
output "azure-functions-azure-event-grid-group-a_function_app_id" {
  value = azurerm_windows_function_app.azure-functions-azure-event-grid-group-a_function_app.id
}
