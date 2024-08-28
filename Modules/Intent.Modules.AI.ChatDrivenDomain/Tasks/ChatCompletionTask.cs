using System;
using System.IO;
using System.Text.Json;
using Intent.Modules.ChatDrivenDomain.Tasks.Models;
using Intent.Plugins;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Intent.Modules.ChatDrivenDomain.Tasks;

public class ChatCompletionTask : IModuleTask
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public string TaskTypeId => "Intent.Modules.ChatDrivenDomain.Tasks.ChatCompletionTask";
    public string TaskTypeName => "Consulting the great LLAMA...";
    public int Order => 0;

    public string Execute(params string[] args)
    {
        Logging.Log.Info($"Args: {string.Join(",", args)}");
        
        try
        {
            var inputModel = JsonSerializer.Deserialize<InputModel>(args[0], SerializerOptions);
            
            var settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Intent Architect", "chatdrivendomain-settings.json");
            if (!Path.Exists(settingsFilePath))
            {
                throw new Exception("No AI settings configured. Please open AI Settings first.");
            }

            var settingsData = JsonSerializer.Deserialize<SettingsData>(File.ReadAllText(settingsFilePath), SerializerOptions);
            var model = string.IsNullOrWhiteSpace(settingsData?.Model) ? "gpt-4o" : settingsData.Model; 
            
            var builder = Kernel.CreateBuilder();

            if (!string.IsNullOrWhiteSpace(settingsData?.ApiUrl))
            {
#pragma warning disable SKEXP0010
                builder.Services.AddOpenAIChatCompletion(model, new Uri(settingsData.ApiUrl));
#pragma warning enable SKEXP0010
            }
            else if (!string.IsNullOrWhiteSpace(settingsData?.ApiKey))
            {
                builder.Services.AddOpenAIChatCompletion(model, settingsData.ApiKey);
            }
            else
            {
                throw new Exception($"Invalid API Settings. Please review AI Settings.");
            }
            
            var kernel = builder.Build();

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

    private string Fail(string reason)
    {
        Logging.Log.Failure(reason);
        var errorObject = new { errorMessage = reason };
        var json = JsonSerializer.Serialize(errorObject);
        return json;
    }
}