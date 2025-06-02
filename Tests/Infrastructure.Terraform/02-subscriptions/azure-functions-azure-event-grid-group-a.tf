variable "azure_functions_azure_event_grid_group_a_id" {
  type = string
}

data "azurerm_eventgrid_topic" "specific_topic" {
  name                = "specific-topic"
  resource_group_name = var.resource_group_name
}

resource "azurerm_eventgrid_event_subscription" "azure_functions_azure_event_grid_group_a" {
  name                 = "specific-topic-sub"
  scope                = data.azurerm_eventgrid_topic.specific_topic.id

  event_delivery_schema = "CloudEventSchemaV1_0"

  azure_function_endpoint {
    function_id                       = "${var.azure_functions_azure_event_grid_group_a_id}/functions/SpecificTopicMessageConsumer"
    max_events_per_batch              = 1
    preferred_batch_size_in_kilobytes = 64
  }

  included_event_types = [
    "AzureFunctions.AzureEventGrid.GroupB.Eventing.Messages.SpecificTopicOneMessage",
    "AzureFunctions.AzureEventGrid.GroupB.Eventing.Messages.SpecificTopicTwoMessage"
  ]
}
