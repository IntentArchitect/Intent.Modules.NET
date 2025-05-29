resource "azurerm_servicebus_namespace" "service_bus" {
  name                = "azure-service-bus"
  location            = azurerm_resource_group.main_rg.location
  resource_group_name = azurerm_resource_group.main_rg.name
  sku                 = "Standard"
}

resource "azurerm_servicebus_topic" "client_created" {
  name         = "client-created"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_servicebus_topic" "create_org_integration_command" {
  name         = "create-org-integration-command"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_servicebus_queue" "specific_queue" {
  name         = "specific-queue"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_servicebus_topic" "specific_topic" {
  name         = "specific-topic"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_servicebus_subscription" "specific_topic" {
  name               = "specific-topic"
  topic_id           = azurerm_servicebus_topic.specific_topic.id
  max_delivery_count = 3
}

resource "azurerm_servicebus_subscription" "client_created" {
  name               = "client-created"
  topic_id           = azurerm_servicebus_topic.client_created.id
  max_delivery_count = 3
}

resource "azurerm_servicebus_subscription" "create_org_integration_command" {
  name               = "create-org-integration-command"
  topic_id           = azurerm_servicebus_topic.create_org_integration_command.id
  max_delivery_count = 3
}
