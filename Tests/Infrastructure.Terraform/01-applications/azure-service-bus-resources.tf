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

resource "azurerm_servicebus_queue" "create_org" {
  name         = "create-org"
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

resource "azurerm_servicebus_subscription" "azure_functions_azure_service_bus_group_a_specific_topic" {
  name               = "azurefunctions-azureservicebus-groupa"
  topic_id           = azurerm_servicebus_topic.specific_topic.id
  max_delivery_count = 3
}

resource "azurerm_servicebus_topic" "publish_and_consume" {
  name         = "publish-and-consume"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_servicebus_subscription" "azure_functions_azure_service_bus_group_b_client_created" {
  name               = "azurefunctions-azureservicebus-groupb"
  topic_id           = azurerm_servicebus_topic.client_created.id
  max_delivery_count = 3
}

resource "azurerm_servicebus_subscription" "azure_functions_azure_service_bus_group_b_publish_and_consume" {
  name               = "azurefunctions-azureservicebus-groupb"
  topic_id           = azurerm_servicebus_topic.publish_and_consume.id
  max_delivery_count = 3
}

resource "azurerm_servicebus_subscription" "asp_net_core_azure_service_bus_group_b_client_created" {
  name               = "aspnetcore-azureservicebus-groupb"
  topic_id           = azurerm_servicebus_topic.client_created.id
  max_delivery_count = 3
}

resource "azurerm_servicebus_subscription" "asp_net_core_azure_service_bus_group_b_publish_and_consume" {
  name               = "aspnetcore-azureservicebus-groupb"
  topic_id           = azurerm_servicebus_topic.publish_and_consume.id
  max_delivery_count = 3
}
