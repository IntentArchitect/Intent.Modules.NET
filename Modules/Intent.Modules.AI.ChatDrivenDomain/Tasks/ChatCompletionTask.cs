using System;
using System.Net.Http;
using System.Text.Json;
using Intent.Engine;
using Intent.Modules.AI.ChatDrivenDomain.Settings;
using Intent.Modules.AI.ChatDrivenDomain.Utils;
using Intent.Plugins;
using Intent.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OllamaSharp;

namespace Intent.Modules.AI.ChatDrivenDomain.Tasks;

public class ChatCompletionTask : IModuleTask
{
    private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ChatCompletionTask(IApplicationConfigurationProvider applicationConfigurationProvider)
    {
        _applicationConfigurationProvider = applicationConfigurationProvider;
    }

    public string TaskTypeId => "Intent.Modules.ChatDrivenDomain.Tasks.ChatCompletionTask";
    public string TaskTypeName => "Consulting the great LLAMA...";
    public int Order => 0;

    public string Execute(params string[] args)
    {
        Logging.Log.Info($"Args: {string.Join(",", args)}");

        try
        {
            var inputModel = JsonSerializer.Deserialize<InputModel>(args[0], SerializerOptions)!;
            var kernel = BuildSemanticKernel();

            var requestFunction = kernel.CreateFunctionFromPrompt(
                """
                # Domain Modeling Expert for Intent Architect
                
                You are a specialized domain modeling expert for Intent Architect. Your task is analyzing, optimizing, and modifying domain models using Domain-Driven Design principles. You MUST apply proper domain modeling constraints and best practices.
                
                ## Data Structure
                
                The domain model consists of Classes, Attributes, and Associations:
                
                ```
                Class {
                    id: string
                    name: string
                    comment: string
                    associations: Association*
                    attributes: Attribute*
                }
                
                Association {
                    id: string
                    name: string
                    associationEndType: string  // "Source End" or "Target End"
                    classId: string
                    relationship: string        // UML notation like "1 -> *"
                }
                
                Attribute {
                    id: string
                    name: string
                    type: string
                    isNullable: bool
                    isCollection: bool
                    comment: string
                }
                ```
                
                ## Primitive Types
                - string, int, long, decimal, datetime, bool, guid, object
                
                ## Relationship Rules - CRITICAL
                
                1. **Composite Relationships (1->1)**: 
                   - An entity can have only ONE composite owner
                   - INVALID: If ClassA and ClassB both have a 1->1 composite relationship to ClassC
                   - CORRECT: If an entity has a 1->1 relationship, it cannot be owned by multiple entities
                
                2. **One-to-Many (1->*)**: 
                   - Source has one reference to Target
                   - Target has a collection of Source entities
                   
                3. **Many-to-Many (*->*)**:
                   - Both sides have collections of each other
                
                4. **Navigability**:
                   - "Source End" is where the relationship originates
                   - "Target End" is where the relationship points to
                
                ## Example Interpretation
                
                ```json
                [
                  {
                    "id": "order-1",
                    "name": "Order",
                    "associations": [
                      {
                        "id": "assoc-1",
                        "name": "OrderLines",
                        "associationEndType": "Target End",
                        "classId": "line-1",
                        "relationship": "1 -> *"
                      }
                    ]
                  },
                  {
                    "id": "line-1",
                    "name": "OrderLine",
                    "associations": [
                      {
                        "id": "assoc-1", 
                        "name": "Order",
                        "associationEndType": "Source End",
                        "classId": "order-1",
                        "relationship": "1 -> *"
                      }
                    ]
                  }
                ]
                ```
                
                This means: 
                - An Order has many OrderLines (collection)
                - Each OrderLine belongs to exactly one Order
                - Order is the Source End of the relationship
                
                ## Domain To Analyze/Modify
                
                ```
                {{$domain}}
                ```
                
                ## User Instructions
                
                ```
                {{$prompt}}
                ```
                
                ## Output Requirements
                
                1. Provide ONLY a valid JSON domain model with NO explanations
                2. New IDs should follow format "Element-number"
                3. Associations MUST include "isNullable" and "isCollection" fields:
                   - "*" = isCollection: true
                   - "0..1" = isNullable: true
                4. ENSURE all relationship constraints are enforced (particularly composite relationships)
                5. Output JSON WITHOUT whitespace
                """, new OpenAIPromptExecutionSettings()
                {
                    MaxTokens = _applicationConfigurationProvider.GetSettings().GetChatDrivenDomainSettings().MaxTokens()
                });

            var result = requestFunction.InvokeAsync(kernel, new KernelArguments
            {
                ["domain"] = JsonSerializer.Serialize(inputModel.Classes),
                ["prompt"] = inputModel.Prompt
            }).Result;

            Logging.Log.Info($"GPT Result: {result}");

            return result.GetValue<string>()!.Replace("```json", "").Replace("```", "");
        }
        catch (Exception e)
        {
            Logging.Log.Failure(e);
            return Fail(e.GetBaseException().Message);
        }
    }

    private Kernel BuildSemanticKernel()
    {
        var settings = _applicationConfigurationProvider.GetSettings().GetChatDrivenDomainSettings();

        var model = string.IsNullOrWhiteSpace(settings.Model()) ? "gpt-4o" : settings.Model();

        var builder = Kernel.CreateBuilder();

        builder.Services.AddLogging(b => b.AddProvider(new SoftwareFactoryLoggingProvider()).SetMinimumLevel(LogLevel.Trace));

        var apiKey = settings.APIKey();

        switch (settings.Provider().AsEnum())
        {
            case ChatDrivenDomainSettings.ProviderOptionsEnum.OpenAi:
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                }

                builder.Services.AddOpenAIChatCompletion(
                    modelId: model,
                    apiKey: apiKey ?? throw new Exception("No API Key defined. Locate the ChatDrivenDomainSettings App Settings or set the OPENAI_API_KEY environment variable."));
                break;
            case ChatDrivenDomainSettings.ProviderOptionsEnum.AzureOpenAi:
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
                }

                builder.Services.AddAzureOpenAIChatCompletion(
                    deploymentName: settings.DeploymentName(),
                    endpoint: settings.APIUrl(),
                    apiKey: apiKey ?? throw new Exception("No API Key defined. Locate the ChatDrivenDomainSettings App Settings or set the AZURE_OPENAI_API_KEY environment variable."),
                    modelId: model);
                break;
            case ChatDrivenDomainSettings.ProviderOptionsEnum.Ollama:
#pragma warning disable SKEXP0070
                builder.Services.AddOllamaChatCompletion(
                    new OllamaApiClient(
                        new HttpClient
                        {
                            Timeout = TimeSpan.FromMinutes(10),
                            BaseAddress = new Uri(settings.APIUrl())
                        },
                        model)
                );
#pragma warning restore SKEXP0070
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var kernel = builder.Build();
        return kernel;
    }

    private string Fail(string reason)
    {
        Logging.Log.Failure(reason);
        var errorObject = new { errorMessage = reason };
        var json = JsonSerializer.Serialize(errorObject);
        return json;
    }
}