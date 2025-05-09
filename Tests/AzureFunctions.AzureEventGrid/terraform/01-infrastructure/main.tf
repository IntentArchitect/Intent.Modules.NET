terraform {
  # Terraform configuration for example project

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
  function_app_name   = "azure-functions-azure-event-grid-${random_string.unique.result}"
  app_insights_name   = "app-insights-${random_string.unique.result}"
  storage_name        = "storage${random_string.unique.result}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
}

variable "input_resource_group_name" {
  type    = string
}

variable "input_resource_group_location" {
  type    = string
}

# Resource Group
resource "azurerm_resource_group" "rg" {
  name     = var.input_resource_group_name
  location = var.input_resource_group_location
}

# Application Insights
resource "azurerm_application_insights" "app_insights" {
  name                = local.app_insights_name
  location            = local.location
  resource_group_name = local.resource_group_name
  application_type    = "web"
}

# Hosting Plan (App Service Plan)
resource "azurerm_service_plan" "function_plan" {
  name                = "asp-${local.function_app_name}"
  location            = local.location
  resource_group_name = local.resource_group_name
  os_type             = "Windows"
  sku_name            = "Y1"
}

# Storage Account
resource "azurerm_storage_account" "storage" {
  name                     = local.storage_name
  location                 = local.location
  resource_group_name      = local.resource_group_name
  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "StorageV2"
}

resource "azurerm_eventgrid_topic" "event_grid_topic_specific_topic" {
  name                = "specific-topic"
  location            = local.location
  resource_group_name = local.resource_group_name
}

resource "azurerm_eventgrid_topic" "event_grid_topic_client_created_event" {
  name                = "client-created-event"
  location            = local.location
  resource_group_name = local.resource_group_name
}

# Function App
resource "azurerm_windows_function_app" "function_app" {
  name                       = local.function_app_name
  location                   = local.location
  resource_group_name        = local.resource_group_name
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
    "APPINSIGHTS_INSTRUMENTATIONKEY"               = azurerm_application_insights.app_insights.instrumentation_key
    "AzureWebJobsStorage"                          = "DefaultEndpointsProtocol=https;AccountName=${azurerm_storage_account.storage.name};AccountKey=${azurerm_storage_account.storage.primary_access_key};EndpointSuffix=core.windows.net"
    "EventGrid:Topics:ClientCreatedEvent:Endpoint" = azurerm_eventgrid_topic.event_grid_topic_client_created_event.endpoint
    "EventGrid:Topics:ClientCreatedEvent:Key"      = azurerm_eventgrid_topic.event_grid_topic_client_created_event.id
    "EventGrid:Topics:SpecificTopic:Endpoint"      = azurerm_eventgrid_topic.event_grid_topic_specific_topic.endpoint
    "EventGrid:Topics:SpecificTopic:Key"           = azurerm_eventgrid_topic.event_grid_topic_specific_topic.id
    "FUNCTIONS_WORKER_RUNTIME"                     = "dotnet-isolated"
  }
}

# Output values needed for the second deployment
output "resource_group_name" {
  value = azurerm_resource_group.rg.name
}

output "function_app_id" {
  value = azurerm_windows_function_app.function_app.id
}

output "event_grid_topic_specific_topic_id" {
  value = azurerm_eventgrid_topic.event_grid_topic_specific_topic.id
}

output "event_grid_topic_client_created_event_id" {
  value = azurerm_eventgrid_topic.event_grid_topic_client_created_event.id
}
