using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.Modules.AI.ChatDrivenDomain.Tasks.Models;
using Intent.Plugins;
using Intent.Utils;

namespace Intent.Modules.AI.ChatDrivenDomain.Tasks.Helpers;

public abstract class ModuleTaskSingleInputBase<TInputModel> : IModuleTask
    where TInputModel : class
{
    protected static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public abstract string TaskTypeId { get; }
    public abstract string TaskTypeName { get; }
    public virtual int Order => 0;

    public string Execute(params string[] args)
    {
        try
        {
            if (!ValidateRequest(args, out var inputModel, out var errorMessage))
            {
                return Fail(errorMessage!);
            }

            if (inputModel is null)
            {
                return Fail("Problem validating request model.");
            }
            
            Logging.Log.Info($"Executing: {TaskTypeId}; Input: {JsonSerializer.Serialize(inputModel, SerializerOptions)}");

            var executeResult = ExecuteModuleTask(inputModel);
            if (executeResult is null)
            {
                throw new InvalidOperationException("Execute result cannot be null.");
            }

            var result = GetResultString(executeResult);
            Logging.Log.Info($"Result: {result}");
            return result;
        }
        catch (Exception e)
        {
            Logging.Log.Failure(e);
            return Fail(e.GetBaseException().Message);
        }
    }

    protected abstract ValidationResult ValidateInputModel(TInputModel inputModel);
    protected abstract ExecuteResult ExecuteModuleTask(TInputModel inputModel);
    
    protected record ValidationResult
    {
        private ValidationResult(bool success, string? errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }
        
        public bool Success { get; }
        public string? ErrorMessage { get; }
        
        public static ValidationResult SuccessResult()
        {
            return new ValidationResult(true, null);
        }
        
        public static ValidationResult ErrorResult(string errorMessage)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);
            return new ValidationResult(false, errorMessage);
        }
    }

    private bool ValidateRequest(string[] args, out TInputModel? inputModel, out string? errorMessage)
    {
        inputModel = null;
        errorMessage = null;

        if (args.Length < 1)
        {
            errorMessage = "Expected 1 argument received 0";
            return false;
        }

        var settings = JsonSerializer.Deserialize<TInputModel>(args[0], SerializerOptions);
        if (settings == null)
        {
            errorMessage = $"Unable to deserialize : {args[0]}";
            return false;
        }

        inputModel = settings;

        var validationResult = ValidateInputModel(inputModel);
        if (validationResult is null)
        {
            throw new InvalidOperationException("Validation result cannot be null.");
        }
        if (!validationResult.Success)
        {
            errorMessage = validationResult.ErrorMessage;
            return false;
        }
        
        return true;
    }
    
    private static string GetResultString(ExecuteResult executeResult)
    {
        return JsonSerializer.Serialize(executeResult, SerializerOptions);
    }

    private string Fail(string reason)
    {
        var executionResult = new ExecuteResult();
        executionResult.Errors.Add($"""
                                    Failed to execute: {TaskTypeId}.
                                    {reason}
                                    """);
        return GetResultString(executionResult);
    }
}