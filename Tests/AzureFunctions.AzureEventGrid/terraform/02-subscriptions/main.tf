terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
}

variable "resource_group_name" {
  description = "The name of the resource group"
  type        = string
}

variable "function_app_id" {
  description = "The ID of the Function App"
  type        = string
}

variable "event_grid_topic_client_created_event_id" {
  description = "The ID of Client created event"
  type        = string
}

variable "event_grid_topic_specific_topic_id" {
  description = "The ID of Specific topic"
  type        = string
}

# Event Grid Subscriptions
resource "azurerm_eventgrid_event_subscription" "event_grid_topic_client_created_event_subscription" {
  name                 = "event_grid_topic_client_created_event_sub"
  scope                = var.event_grid_topic_client_created_event_id

  azure_function_endpoint {
    function_id                       = "${var.function_app_id}/functions/ClientCreatedEventConsumer"
    max_events_per_batch              = 1
    preferred_batch_size_in_kilobytes = 64
  }

  included_event_types = [ "AzureFunctions.AzureEventGrid.Eventing.Messages.ClientCreatedEvent" ]
}

resource "azurerm_eventgrid_event_subscription" "event_grid_topic_specific_topic_subscription" {
  name                 = "event_grid_topic_specific_topic_sub"
  scope                = var.event_grid_topic_specific_topic_id

  azure_function_endpoint {
    function_id                       = "${var.function_app_id}/functions/SpecificTopicMessageConsumer"
    max_events_per_batch              = 1
    preferred_batch_size_in_kilobytes = 64
  }

  included_event_types = [
    "AzureFunctions.AzureEventGrid.Eventing.Messages.SpecificTopicOneMessageEvent",
    "AzureFunctions.AzureEventGrid.Eventing.Messages.SpecificTopicTwoMessageEvent"
  ]
}
