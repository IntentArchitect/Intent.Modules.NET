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

# Variables
variable "resource_group_name" {
  type = string
}

variable "resource_group_location" {
  type = string
}

# Resource group
resource "azurerm_resource_group" "main_rg" {
  name     = var.resource_group_name
  location = var.resource_group_location
}

# Workspace
resource "azurerm_log_analytics_workspace" "workspace" {
  name                = "log-analytics-workspace-name"
  location            = azurerm_resource_group.main_rg.location
  resource_group_name = azurerm_resource_group.main_rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

# Application Insights
resource "azurerm_application_insights" "app_insights" {
  name                = "app-insights"
  location            = azurerm_resource_group.main_rg.location
  resource_group_name = azurerm_resource_group.main_rg.name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.workspace.id
}
