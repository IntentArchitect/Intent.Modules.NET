using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;
using System.Timers;

namespace HashiCorpVault.Api.Configuration;

public static class HashicorpVaultConfiguration
{
    public static void ConfigureHashicorpVault(this IConfigurationBuilder builder, IConfiguration configuration)
    {
        var options = new HashicorpVaultOptions();
        configuration.GetSection("HashicorpVault").Bind(options);
        foreach (var vault in options.Vaults)
        {
            builder.Add(new HashicorpVaultConfigurationSource(vault));
        }
    }
}

public class HashicorpVaultConfigurationSource : IConfigurationSource
{
    private readonly HashicorpVaultEntry _hashicorpVaultEntry;

    public HashicorpVaultConfigurationSource(HashicorpVaultEntry hashicorpVaultEntry)
    {
        _hashicorpVaultEntry = hashicorpVaultEntry;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder) => new HashicorpVaultConfigurationProvider(_hashicorpVaultEntry);
}

public class HashicorpVaultConfigurationProvider : ConfigurationProvider
{
    private readonly HashicorpVaultEntry _hashicorpVaultEntry;
    private readonly IVaultClient _vaultClient;
    private readonly Timer _reloadTimer;

    public HashicorpVaultConfigurationProvider(HashicorpVaultEntry hashicorpVaultEntry)
    {
        _hashicorpVaultEntry = hashicorpVaultEntry;

        var auth = GetAuthMethod(hashicorpVaultEntry.AuthMethod);
        _vaultClient = new VaultClient(new VaultClientSettings(hashicorpVaultEntry.Url.ToString(), auth));

        if (hashicorpVaultEntry.CacheTimeoutInSeconds != default)
        {
            _reloadTimer = new Timer
            {
                Enabled = true,
                Interval = TimeSpan.FromSeconds(hashicorpVaultEntry.CacheTimeoutInSeconds).TotalMilliseconds,
                AutoReset = true
            };

            _reloadTimer.Elapsed += (_, _) => Load();
        }
    }

    private static IAuthMethodInfo GetAuthMethod(HashicorpVaultAuthMethod authMethod)
    {
        if (authMethod.AppRole is not null)
        {
            return new AppRoleAuthMethodInfo(authMethod.AppRole.RoleId, authMethod.AppRole.SecretId);
        }

        if (authMethod.UserPass is not null)
        {
            return new UserPassAuthMethodInfo(authMethod.UserPass.Username, authMethod.UserPass.Password);
        }

        if (authMethod.Token is not null)
        {
            return new TokenAuthMethodInfo(authMethod.Token.Token);
        }

        throw new InvalidOperationException("No Auth Method was specified");
    }

    public override void Load()
    {
        var keys = _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: _hashicorpVaultEntry.Path, mountPoint: _hashicorpVaultEntry.MountPoint).GetAwaiter().GetResult();
        Data.Clear();

        var flattenedKeys = keys.Data.Data.SelectMany(JsonConfigurationFileParser.Convert);
        foreach (var entry in flattenedKeys)
        {
            Data.Add(entry.Key, entry.Value);
        }
    }
}

public class JsonConfigurationFileParser
{
    private JsonConfigurationFileParser()
    {
    }

    private readonly Dictionary<string, string?> _data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<string> _paths = new Stack<string>();

    public static IDictionary<string, string?> Convert(KeyValuePair<string, object> pair) 
        => new JsonConfigurationFileParser().ConvertObject(pair);
    
    private Dictionary<string, string?> ConvertObject(KeyValuePair<string, object> pair)
    {
        if (pair.Value is not JsonElement jsonElement)
        {
            return _data;
        }

        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Object:
                VisitObjectElement(jsonElement, pair.Key);
                break;
            case JsonValueKind.Array:
                VisitArrayElement(jsonElement, pair.Key);
                break;
            case JsonValueKind.String:
            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
                _data.Add(pair.Key, pair.Value.ToString());
                break;
            case JsonValueKind.Null:
                _data.Add(pair.Key, null);
                break;
            case JsonValueKind.Undefined:
            default:
                throw new FormatException($"Unsupported JSON Token: {jsonElement.ValueKind}");
        }

        return _data;
    }
    
    private void VisitObjectElement(JsonElement element, string mainKey)
    {
        var isEmpty = true;

        foreach (JsonProperty property in element.EnumerateObject())
        {
            isEmpty = false;
            EnterContext(property.Name);
            VisitValue(property.Value, mainKey);
            ExitContext();
        }

        SetNullIfElementIsEmpty(isEmpty);
    }

    private void VisitArrayElement(JsonElement element, string mainKey)
    {
        int index = 0;

        foreach (JsonElement arrayElement in element.EnumerateArray())
        {
            EnterContext(index.ToString());
            VisitValue(arrayElement, mainKey);
            ExitContext();
            index++;
        }

        SetNullIfElementIsEmpty(isEmpty: index == 0);
    }

    private void SetNullIfElementIsEmpty(bool isEmpty)
    {
        if (isEmpty && _paths.Count > 0)
        {
            _data[_paths.Peek()] = null;
        }
    }

    private void VisitValue(JsonElement value, string mainKey)
    {
        Debug.Assert(_paths.Count > 0);

        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                VisitObjectElement(value, mainKey);
                break;

            case JsonValueKind.Array:
                VisitArrayElement(value, mainKey);
                break;

            case JsonValueKind.Number:
            case JsonValueKind.String:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                string key = $"{mainKey}:{_paths.Peek()}".Replace("__", ":");
                if (_data.ContainsKey(key))
                {
                    throw new FormatException($"Key is duplicated: {mainKey}");
                }

                _data[key] = value.ToString();
                break;

            case JsonValueKind.Undefined:
            default:
                throw new FormatException($"Unsupported JSON Token: {value.ValueKind}");
        }
    }

    private void EnterContext(string context) =>
        _paths.Push(_paths.Count > 0 ? _paths.Peek() + ConfigurationPath.KeyDelimiter + context : context);

    private void ExitContext() => _paths.Pop();
}