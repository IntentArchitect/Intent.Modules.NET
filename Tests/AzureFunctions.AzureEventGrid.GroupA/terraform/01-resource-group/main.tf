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

variable "resource_group_name" {
  type = string
}

variable "resource_group_location" {
  type = string
}

resource "random_string" "unique" {
  length  = 8
  special = false
  upper   = false
}

locals {
  app_insights_name = "app-insights-${random_string.unique.result}"
  location          = azurerm_resource_group.rg.location
}

# Resource Group
resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.resource_group_location
}

# Application Insights
resource "azurerm_application_insights" "app_insights" {
  name                = local.app_insights_name
  location            = local.location
  resource_group_name = var.resource_group_name
  application_type    = "web"
}

output "resource_group_name" {
  value = var.resource_group_name
}

output "resource_group_location" {
  value = var.resource_group_location
}

output "app_insights_name" {
  value = azurerm_application_insights.app_insights.name
}
