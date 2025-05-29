resource "random_string" "azure-functions-net6" {
  length  = 3
  special = false
  upper   = false
}

locals {
  azure-functions-net6_app_name     = "azure-functions-net6"
  azure_functions_net6_storage_name = "azurefunction${random_string.azure-functions-net6.result}storage"
}

# Hosting Plan (App Service Plan)
resource "azurerm_service_plan" "azure-functions-net6_function_plan" {
  name                = "asp-${local.azure-functions-net6_app_name}"
  location            = azurerm_resource_group.main_rg.location
  resource_group_name = azurerm_resource_group.main_rg.name
  os_type             = "Windows"
  sku_name            = "Y1"
}

# Storage Account
resource "azurerm_storage_account" "azure_functions_net6_storage_name" {
  name                     = local.azure_functions_net6_storage_name
  location                 = azurerm_resource_group.main_rg.location
  resource_group_name      = azurerm_resource_group.main_rg.name
  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "StorageV2"
}

# Function App
resource "azurerm_windows_function_app" "azure-functions-net6_function_app" {
  name                       = local.azure-functions-net6_app_name
  location                   = azurerm_resource_group.main_rg.location
  resource_group_name        = azurerm_resource_group.main_rg.name
  service_plan_id            = azurerm_service_plan.azure-functions-net6_function_plan.id
  storage_account_name       = azurerm_storage_account.azure_functions_net6_storage_name.name
  storage_account_access_key = azurerm_storage_account.azure_functions_net6_storage_name.primary_access_key

  site_config {
    application_stack {
      dotnet_version = "v6.0"
    }
    cors {
      allowed_origins = [ "https://portal.azure.com" ]
    }
  }

  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.app_insights.instrumentation_key
    "AzureWebJobsStorage"            = "DefaultEndpointsProtocol=https;AccountName=${azurerm_storage_account.azure_functions_net6_storage_name.name};AccountKey=${azurerm_storage_account.azure_functions_net6_storage_name.primary_access_key};EndpointSuffix=core.windows.net"
    "FUNCTIONS_WORKER_RUNTIME"       = "dotnet"
  }
}

# Output variables
output "azure-functions-net6_function_app_id" {
  value = azurerm_windows_function_app.azure-functions-net6_function_app.id
}
