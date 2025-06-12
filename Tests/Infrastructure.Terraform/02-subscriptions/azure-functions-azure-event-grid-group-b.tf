variable "azure_functions_azure_event_grid_group_b_id" {
  type = string
}

data "azurerm_eventgrid_topic" "client_created_event" {
  name                = "client-created-event"
  resource_group_name = var.resource_group_name
}

resource "azurerm_eventgrid_event_subscription" "azure_functions_azure_event_grid_group_b" {
  name                  = "azurefunctionsazureeventgridgroupb-clientcreatedevent-sub"
  scope                 = data.azurerm_eventgrid_topic.client_created_event.id
  event_delivery_schema = "CloudEventSchemaV1_0"

  azure_function_endpoint {
    function_id                       = "${var.azure_functions_azure_event_grid_group_b_id}/functions/ClientCreatedEventConsumer"
    max_events_per_batch              = 1
    preferred_batch_size_in_kilobytes = 64
  }

  included_event_types  = [ "AzureFunctions.AzureEventGrid.GroupA.Eventing.Messages.ClientCreatedEvent" ]
}
