resource "azurerm_eventgrid_topic" "client_created_event" {
  name                = "client-created-event"
  location            = azurerm_resource_group.main_rg.location
  resource_group_name = azurerm_resource_group.main_rg.name
}

resource "azurerm_eventgrid_topic" "specific_topic" {
  name                = "specific-topic"
  location            = azurerm_resource_group.main_rg.location
  resource_group_name = azurerm_resource_group.main_rg.name
}
