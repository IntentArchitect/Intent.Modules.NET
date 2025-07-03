resource "azurerm_eventgrid_domain" "main_domain" {
  name                = "main-domain"
  location            = azurerm_resource_group.main_rg.location
  resource_group_name = azurerm_resource_group.main_rg.name
  input_schema        = "CloudEventSchemaV1_0"
}

resource "azurerm_eventgrid_domain_topic" "order_created_event" {
  name                = "order-created-event"
  resource_group_name = azurerm_resource_group.main_rg.name
  domain_name         = azurerm_eventgrid_domain.main_domain.name
}

resource "azurerm_eventgrid_topic" "client_created_event" {
  name                = "client-created-event"
  location            = azurerm_resource_group.main_rg.location
  resource_group_name = azurerm_resource_group.main_rg.name
  input_schema        = "CloudEventSchemaV1_0"
}

resource "azurerm_eventgrid_topic" "specific_topic" {
  name                = "specific-topic"
  location            = azurerm_resource_group.main_rg.location
  resource_group_name = azurerm_resource_group.main_rg.name
  input_schema        = "CloudEventSchemaV1_0"
}
