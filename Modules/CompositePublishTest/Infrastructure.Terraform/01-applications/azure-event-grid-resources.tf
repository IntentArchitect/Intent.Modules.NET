resource "azurerm_eventgrid_topic" "client_created_event" {
  name                = "client-created-event"
  location            = azurerm_resource_group.main_rg.location
  resource_group_name = azurerm_resource_group.main_rg.name
  input_schema        = "CloudEventSchemaV1_0"
}
