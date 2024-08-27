using System;
using System.IO;
using System.Text.Json;
using Intent.Modules.ChatDrivenDomain.Tasks.Models;
using Intent.Plugins;
using Intent.Utils;

namespace Intent.Modules.ChatDrivenDomain.Tasks;

public class UpdateSettingsTask : IModuleTask
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public string TaskTypeId => "Intent.Modules.ChatDrivenDomain.Tasks.UpdateSettingsTask";
    public string TaskTypeName => "Update Settings for AI";
    public int Order => 0;
    
    public string Execute(params string[] args)
    {
        Logging.Log.Info($"Args: {string.Join(",", args)}");
        
        try
        {
            var settings = JsonSerializer.Deserialize<SettingsData>(args[0], SerializerOptions);
            
            var settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Intent Architect", "chatdrivendomain-settings.json");
            
            File.WriteAllText(settingsFilePath, args[0]);
            return JsonSerializer.Serialize(new { Success = true }, SerializerOptions);
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