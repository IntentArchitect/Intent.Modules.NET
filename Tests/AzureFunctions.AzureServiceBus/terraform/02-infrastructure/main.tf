terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "random_string" "unique" {
  length  = 8
  special = false
  upper   = false
}

locals {
  function_app_name          = "azure-functions-azure-service-bus-${random_string.unique.result}"
  storage_name               = "storage${random_string.unique.result}"
  service_bus_namespace_name = "service-bus-${random_string.unique.result}"
}

# Variables
variable "resource_group_name" {
  type = string
}

variable "resource_group_location" {
  type = string
}

variable "app_insights_name" {
  type = string
}

# Hosting Plan (App Service Plan)
resource "azurerm_service_plan" "function_plan" {
  name                = "asp-${local.function_app_name}"
  location            = var.resource_group_location
  resource_group_name = var.resource_group_name
  os_type             = "Windows"
  sku_name            = "Y1"
}

# Storage Account
resource "azurerm_storage_account" "storage" {
  name                     = local.storage_name
  location                 = var.resource_group_location
  resource_group_name      = var.resource_group_name
  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "StorageV2"
}

resource "azurerm_servicebus_namespace" "service_bus" {
  name                = local.service_bus_namespace_name
  location            = var.resource_group_location
  resource_group_name = var.resource_group_name
  sku                 = "Standard"
}

resource "azurerm_servicebus_queue" "create_org_queue" {
  name         = "create-org"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_servicebus_topic" "specific_topic_topic" {
  name         = "specific-topic"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_servicebus_queue" "specific_queue_queue" {
  name         = "specific-queue"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_servicebus_topic" "client_created_topic" {
  name         = "client-created"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_servicebus_topic" "client_created_topic" {
  name         = "client-created"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_servicebus_subscription" "client_created_subscription" {
  name               = "client-created"
  topic_id           = azurerm_servicebus_topic.client_created_topic.id
  max_delivery_count = 3
}

resource "azurerm_servicebus_topic" "specific_topic_topic" {
  name         = "specific-topic"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_servicebus_subscription" "specific_topic_subscription" {
  name               = "specific-topic"
  topic_id           = azurerm_servicebus_topic.specific_topic_topic.id
  max_delivery_count = 3
}

# Application Insights
data "azurerm_application_insights" "app_insights" {
  name                = var.app_insights_name
  resource_group_name = var.resource_group_name
}

# Function App
resource "azurerm_windows_function_app" "function_app" {
  name                       = local.function_app_name
  location                   = var.resource_group_location
  resource_group_name        = var.resource_group_name
  service_plan_id            = azurerm_service_plan.function_plan.id
  storage_account_name       = azurerm_storage_account.storage.name
  storage_account_access_key = azurerm_storage_account.storage.primary_access_key

  site_config {
    application_stack {
      dotnet_version = "v8.0"
    }
    cors {
      allowed_origins = [ "https://portal.azure.com" ]
    }
  }

  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY"            = data.azurerm_application_insights.app_insights.instrumentation_key
    "AzureWebJobsStorage"                       = "DefaultEndpointsProtocol=https;AccountName=${azurerm_storage_account.storage.name};AccountKey=${azurerm_storage_account.storage.primary_access_key};EndpointSuffix=core.windows.net"
    "AzureServiceBus:ClientCreated"             = azurerm_servicebus_topic.client_created_topic.id
    "AzureServiceBus:ClientCreated"             = azurerm_servicebus_topic.client_created_topic.id
    "AzureServiceBus:ClientCreatedSubscription" = azurerm_servicebus_subscription.client_created_subscription.id
    "AzureServiceBus:ConnectionString"          = azurerm_servicebus_namespace.service_bus.default_primary_connection_string
    "AzureServiceBus:CreateOrg"                 = azurerm_servicebus_queue.create_org_queue.id
    "AzureServiceBus:SpecificQueue"             = azurerm_servicebus_queue.specific_queue_queue.id
    "AzureServiceBus:SpecificTopic"             = azurerm_servicebus_topic.specific_topic_topic.id
    "AzureServiceBus:SpecificTopic"             = azurerm_servicebus_topic.specific_topic_topic.id
    "AzureServiceBus:SpecificTopicSubscription" = azurerm_servicebus_subscription.specific_topic_subscription.id
    "FUNCTIONS_WORKER_RUNTIME"                  = "dotnet-isolated"
  }
}

# Output values needed for the second deployment
output "resource_group_name" {
  value = var.resource_group_name
}

output "function_app_id" {
  value = azurerm_windows_function_app.function_app.id
}
