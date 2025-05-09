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

variable "function_app_id" {
  description = "The ID of the Function App"
  type        = string
}

variable "client_created_event_topic_id" {
  description = "The ID of the Client Created Event Topic"
  type        = string
}

variable "specific_topic_id" {
  description = "The ID of the Specific Topic"
  type        = string
}

variable "resource_group_name" {
  description = "The name of the resource group"
  type        = string
}

# Option 2: Use remote state (recommended for production)
# Uncomment this block if using remote state
# data "terraform_remote_state" "infrastructure" {
#   backend = "azurerm"
#   
#   config = {
#     resource_group_name  = "terraform-state-rg"
#     storage_account_name = "tfstateXXXXXXXX"
#     container_name       = "tfstate"
#     key                  = "infrastructure.tfstate"
#   }
# }
# 
# # Use these local variables if using remote state
# locals {
#   function_app_id              = data.terraform_remote_state.infrastructure.outputs.function_app_id
#   client_created_event_topic_id = data.terraform_remote_state.infrastructure.outputs.client_created_event_topic_id
#   specific_topic_id            = data.terraform_remote_state.infrastructure.outputs.specific_topic_id
#   resource_group_name          = data.terraform_remote_state.infrastructure.outputs.resource_group_name
# }

# Event Grid Subscriptions
resource "azurerm_eventgrid_event_subscription" "client_created_event_subscription" {
  name                 = "client-created-event-eg-sub"
  scope                = var.client_created_event_topic_id

  azure_function_endpoint {
    function_id                       = "${var.function_app_id}/functions/ClientCreatedEventConsumer"
    max_events_per_batch              = 1
    preferred_batch_size_in_kilobytes = 64
  }

  included_event_types = [ "AzureFunctions.AzureEventGrid.Eventing.Messages.ClientCreatedEvent" ]
}

resource "azurerm_eventgrid_event_subscription" "specific_topic_subscription" {
  name                 = "specific-topic-eg-sub"
  scope                = var.specific_topic_id

  azure_function_endpoint {
    function_id                       = "${var.function_app_id}/functions/SpecificTopicMessageConsumer"
    max_events_per_batch              = 1
    preferred_batch_size_in_kilobytes = 64
  }

  included_event_types = [ "AzureFunctions.AzureEventGrid.Eventing.Messages.SpecificTopicOneMessageEvent", "AzureFunctions.AzureEventGrid.Eventing.Messages.SpecificTopicTwoMessageEvent" ]
}
