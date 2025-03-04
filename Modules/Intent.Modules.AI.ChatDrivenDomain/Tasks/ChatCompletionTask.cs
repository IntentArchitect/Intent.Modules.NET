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
                You are an agent that is working alongside a product referred to as Intent Architect. You are an expert in designing business domains
                for software development and know how Domain Driven Design works. YOU WILL NOT BE LAZY! GO the EXTRA MILE for the user!

                There will be an input model supplied that will have the following structure:

                ```
                Class
                {
                    id: string
                    name: string
                    comment: string
                    associations: Association*
                    attributes: Attribute*
                }

                Association
                {
                    id: string
                    name: string
                    associationEndType: string
                    classId: string
                    relationship: string
                }

                Attribute
                {
                    id: string
                    name: string
                    type: string
                    isNullable: bool
                    isCollection: bool
                    comment: string
                }
                ```

                The primitive types that are available are:

                - string
                - int
                - long
                - decimal
                - datetime
                - bool
                - guid
                - object

                The * denotes a collection of notation.
                You will receive a JSON payload that conforms to the structure above.
                Classes are the root elements which will have Attributes which make up what information the Class can hold such as a "Name", "PhoneNumber", etc.
                Associations will have information on how Classes relate to one another.
                The Id for the Association on both classes will be the same. 
                The ClassId will point to a Class in the collection you already have. 
                The "relationship" field will give a relationship description like "1 -> *" which will be like standard UML Class relationships. 
                You **WILL** need to understand that relationship information **ALONGSIDE** the "associationEndType" field.
                So if you get the following:

                ```
                [
                  {
                      "id": "0e8579d1-3a03-4248-941a-0eec8d05c154",
                      "name": "Order",
                      "associations": [
                          {
                          "id": "4d7b42ee-105a-4685-ae14-68d4932000c0",
                          "name": "OrderLines",
                          "associationEndType": "Target End",
                          "classId": "938ed54f-d622-4424-bfb9-02c486299b55",
                          "relationship": "1 -> *"
                          }
                      ]
                  },
                  {
                      "id": "938ed54f-d622-4424-bfb9-02c486299b55",
                      "name": "OrderLine",
                      "associations": [
                          {
                          "id": "4d7b42ee-105a-4685-ae14-68d4932000c0",
                          "name": "Order",
                          "associationEndType": "Source End",
                          "classId": "0e8579d1-3a03-4248-941a-0eec8d05c154",
                          "relationship": "1 -> *"
                          }
                      ]
                  }
                ] 
                ```

                You can deduce that Class "Order" has Id "0e8579d1-3a03-4248-941a-0eec8d05c154" and has
                an association with another Class "OrderLine" of Id "938ed54f-d622-4424-bfb9-02c486299b55".
                The relationship is `1 to many` where "Order" is the Source End and "OrderLine" is the Target End.

                Here is the domain that has been supplied to you to scrutinize:

                ```
                {{$domain}}
                ```

                Follow the user prompt and provide an output following the same structure as above in JSON notation.

                ```
                {{$prompt}}
                ```

                Output instructions:
                When you mutate the above given domain based on the above prompt, use the structure provided initially.
                Any new IDs that are assigned should be in the form "Element-number".
                Additionally the Associations should have "isNullable" and "isCollection" fields that reflect
                the "relationship" field. * = collection, 0..1 = nullable.
                ONLY output the structure in JSON format WITHOUT WHITESPACE! 
                """, new OpenAIPromptExecutionSettings()
                {
                    MaxTokens = 4096
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
                    apiKey: apiKey ?? throw new Exception("No API Key defined"));
                break;
            case ChatDrivenDomainSettings.ProviderOptionsEnum.AzureOpenAi:
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
                }

                builder.Services.AddAzureOpenAIChatCompletion(
                    deploymentName: settings.DeploymentName(),
                    endpoint: settings.APIUrl(),
                    apiKey: apiKey ?? throw new Exception("No API Key defined"),
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