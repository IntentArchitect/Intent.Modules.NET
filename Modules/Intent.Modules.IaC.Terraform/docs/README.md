# Intent.IaC.Terraform

## Overview

The Intent.IaC.Terraform module automatically generates Infrastructure as Code (IaC) through Terraform `.tf` files for your Intent Architect applications. This module scans all applications in your Intent Architect Solution to identify applications and generates the necessary Terraform configuration to deploy them to `Azure`.

## Features

- **Automatic Discovery**: Scans Intent Architect Solutions to identify Azure Function applications
- **Eventing Integration**: Detects and configures Azure Event Grid and Service Bus resources when `Intent.Eventing.AzureEventGrid` or `Intent.Eventing.AzureServiceBus` modules are present
- **Modular Structure**: Generates organized Terraform modules for applications and subscriptions
- **Deployment-Ready**: Creates subscription configurations that work with automated deployment pipelines

## Generated Structure

The module generates Terraform files organized into two main modules:

### 01-applications Module
Contains the core infrastructure resources:

- `azure-main.tf` - Common Azure provider configuration and resource group setup
- `azure-functions-{app-name}.tf` - Individual Azure Function app configurations
- `azure-event-grid-resources.tf` - Event Grid topics and related resources (when applicable)
- `azure-service-bus-resources.tf` - Service Bus queues, topics, and subscriptions (when applicable)

### 02-subscriptions Module
Contains Event Grid subscription configurations:

- `azure-main.tf` - Provider configuration for subscription deployment
- `azure-functions-{app-name}.tf` - Event Grid subscriptions for each function app

## Prerequisites

- Terraform CLI installed
  - [Installation](https://developer.hashicorp.com/terraform/install)
  - [Commands](https://developer.hashicorp.com/terraform/cli/commands)
- Azure CLI installed and authenticated
  - [Installation](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest)
  - [Getting started](https://learn.microsoft.com/en-us/cli/azure/get-started-with-azure-cli?view=azure-cli-latest)

## Configuration

The module automatically detects your applications inside your Intent Architect solution. Ensure your applications are properly configured with:

1. **Azure Function Applications**: By using `Intent.AzureFunctions` module
2. **Service Bus Configuration**: By using `Intent.Eventing.AzureServiceBus` module
3. **Event Grid Configuration**: By using `Intent.Eventing.AzureEventGrid` module

## Deployment Process

The generated Terraform modules are designed to work with a multi-stage deployment process:

### Stage 1: Deploy Applications
```bash
cd 01-applications
terraform init
terraform plan -var="resource_group_name=my-rg" -var="resource_group_location=East US"
terraform apply
```

### Stage 2: Deploy Application Code
Deploy your Azure Functions application code using your preferred method (Azure DevOps, GitHub Actions, etc.)

### Stage 3: Deploy Subscriptions
```bash
cd ../02-subscriptions
terraform init
terraform plan
terraform apply
```

> [!NOTE]
> The subscriptions module is deployed separately to ensure Azure Function endpoints are available before creating Event Grid subscriptions, as Event Grid requires valid endpoint URLs for subscription validation.

## Variables

The generated Terraform modules accept the following variables:

### 01-applications Module
- `resource_group_name` - Name of the Azure resource group to create/use
- `location` - Azure region for resource deployment

### 02-subscriptions Module
- `azure_functions_{app-name}_id` - Azure Function App Ids per application based on the output of module `01-applications`

## Example Output Structure

```
terraform/
├── 01-applications/
│   ├── azure-main.tf
│   ├── azure-functions-invoice.tf
│   ├── azure-functions-orders.tf
│   ├── azure-event-grid-resources.tf
│   └── azure-service-bus-resources.tf
└── 02-subscriptions/
    ├── azure-main.tf
    ├── azure-functions-orders.tf
    └── azure-functions-invoice.tf
```

## Troubleshooting

### Common Issues

**Event Grid Subscription Failures**
- Ensure Azure Functions are deployed and accessible before running the subscriptions module
- Verify function URLs are valid and return HTTP 200 responses

**Resource Group Conflicts**
- Check if resource group already exists and has conflicting resources
- Ensure proper permissions for resource group creation/modification

**Provider Authentication**
- Verify Azure CLI is authenticated: `az login`
- Check subscription context: `az account show`