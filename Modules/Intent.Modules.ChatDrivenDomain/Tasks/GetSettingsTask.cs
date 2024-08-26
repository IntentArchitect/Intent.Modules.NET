using System;
using System.IO;
using System.Text.Json;
using Intent.Modules.ChatDrivenDomain.Tasks.Models;
using Intent.Plugins;
using Intent.Utils;

namespace Intent.Modules.ChatDrivenDomain.Tasks;

public class GetSettingsTask : IModuleTask
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public string TaskTypeId => "Intent.Modules.ChatDrivenDomain.Tasks.GetSettingsTask";
    public string TaskTypeName => "Get Settings for AI";
    public int Order => 0;
    
    public string Execute(params string[] args)
    {
        try
        {
            var settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Intent Architect", "chatdrivendomain-settings.json");
            if (!Path.Exists(settingsFilePath))
            {
                return JsonSerializer.Serialize(new SettingsResult { SettingsFileExists = false }, SerializerOptions);
            }

            var settingsData = JsonSerializer.Deserialize<SettingsData>(File.ReadAllText(settingsFilePath), SerializerOptions);
            var result = JsonSerializer.Serialize(new SettingsResult { SettingsFileExists = true, Data = settingsData }, SerializerOptions);
            return result;
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