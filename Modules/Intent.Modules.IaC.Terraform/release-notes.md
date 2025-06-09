### Version 1.0.0

- New Feature: Generate Terraform files to build Azure resources for Azure Functions.
- New Feature: If Azure Service Bus is installed, it will generate Terraform code to install in Azure.
- New Feature: If Azure Event Grid is installed, it will generate Terraform code to install in Azure.

> [!NOTE]
> 
> Event Grid Topics will be using `Cloud Event Schema` Version 1.0. This is the [recommended schema](https://learn.microsoft.com/en-us/azure/event-grid/event-schema) by Microsoft.