using System;
using System.Text.Json;
using Intent.Modules.ChatDrivenDomain.Tasks.Models;
using Intent.Plugins;
using Intent.Utils;
using Microsoft.SemanticKernel;

namespace Intent.Modules.ChatDrivenDomain.Tasks;

public class ChatDrivenDomainTask : IModuleTask
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public string TaskTypeId => "Intent.Modules.ChatDrivenDomain.Tasks.ChatDrivenDomain";
    public string TaskTypeName => "Chat Driven Domain Processing";
    public int Order => 0;

    public string Execute(params string[] args)
    {
        Logging.Log.Info($"Args: {string.Join(",", args)}");
        
        try
        {
            var inputModel = JsonSerializer.Deserialize<InputModel>(args[0], SerializerOptions);
            
            var builder = Kernel.CreateBuilder();
            builder.Services.AddOpenAIChatCompletion("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);
            var kernel = builder.Build();

            var requestFunction = kernel.CreateFunctionFromPrompt(
                  """
                  You are an agent that is working alongside a product referred to as Intent Architect. You are an expert in designing business domains
                  for software development and know how Domain Driven Design works. You will receive a domain to work with and execute instructions to
                  provide a mutated version of that domain as a result in JSON notation.

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
                      name: string
                      specialization: string
                      specializationEndType: string
                      classId: string
                      type: string
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
                  Associations will have information on how Classes relate to one another. The ClassId will point to a Class in the collection you already have. 
                  The "Type" field will give a relationship description like "1 -> *" which will be like standard UML Class relationships. 
                  You **WILL** need to understand that relationship information **ALONGSIDE** the "SpecializationEndType" field.
                  So if you get the following:
                  
                  ```
                  [
                    {
                        "id": "0e8579d1-3a03-4248-941a-0eec8d05c154",
                        "name": "Order",
                        "associations": [
                            {
                            "name": "OrderLines",
                            "specialization": "Association Target End",
                            "associationEndType": "Target End",
                            "classId": "938ed54f-d622-4424-bfb9-02c486299b55",
                            "type": "1 -> *"
                            }
                        ]
                    },
                    {
                        "id": "938ed54f-d622-4424-bfb9-02c486299b55",
                        "name": "OrderLine",
                        "associations": [
                            {
                            "name": "Order",
                            "specialization": "Association Source End",
                            "associationEndType": "Source End",
                            "classId": "0e8579d1-3a03-4248-941a-0eec8d05c154",
                            "type": "1 -> *"
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
                  When you are generating new "Id" values, use a unique GUID.
                  Only output in JSON format and notation.
                  """);
            
            var result = requestFunction.InvokeAsync(kernel, new KernelArguments
            {
                ["domain"] = JsonSerializer.Serialize(inputModel.Classes),
                ["prompt"] = inputModel.Prompt
            }).Result;
            
            Logging.Log.Info($"GPT Result: {result}");

            return result.GetValue<string>();
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