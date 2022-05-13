using System;
using Intent.SdkEvolutionHelpers;
using Newtonsoft.Json.Linq;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;

public class AppSettingsEditor
{
    private readonly JObject _appSettings;

    internal AppSettingsEditor(JObject appSettings)
    {
        _appSettings = appSettings;
    }

    public dynamic Value => _appSettings;

    public bool PropertyExists(string key)
    {
        return _appSettings.TryGetFieldValue(key, out _);
    }

    public dynamic GetProperty(string key) => GetProperty<dynamic>(key);

    /// <summary>
    /// Obsolete. Use <see cref="GetProperty{T}"/> instead.
    /// </summary>
    [Obsolete(WillBeRemovedIn.Version4)]
    public T GetPropertyAs<T>(string key) => GetProperty<T>(key);

    public T GetProperty<T>(string key)
    {
        if (!_appSettings.TryGetFieldValue(key, out var value))
        {
            return default;
        }

        if (value is JObject jObject)
        {
            return jObject.ToObject<T>();
        }

        return (T)value;
    }

    public void AddPropertyIfNotExists(string key, object value)
    {
        _appSettings.SetFieldValue(key, value, allowReplacement: false);
    }

    public void SetProperty(string key, string value) => SetProperty(key, (object)value);

    public void SetProperty(string key, object value)
    {
        _appSettings.SetFieldValue(key, value, allowReplacement: true);
    }
}