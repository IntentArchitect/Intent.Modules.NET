using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace Intent.Modules.HashiCorp.Vault.Templates.HashiCorpVaultConfigurationSource
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HashiCorpVaultConfigurationSourceTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$"""
                     using System;

                     [assembly: DefaultIntentManaged(Mode.Fully)]

                     namespace {{Namespace}};
                     
                     public class {{ClassName}} : IConfigurationSource
                     {
                         private readonly HashiCorpVaultEntry _hashiCorpVaultEntry;
                     
                         public {{ClassName}}(HashiCorpVaultEntry hashiCorpVaultEntry)
                         {
                             _hashiCorpVaultEntry = hashiCorpVaultEntry;
                         }
                     
                         public IConfigurationProvider Build(IConfigurationBuilder builder) => new {{ClassNameBase}}Provider(_hashiCorpVaultEntry);
                     }
                     
                     public class {{ClassNameBase}}Provider : ConfigurationProvider
                     {
                         private readonly HashiCorpVaultEntry _hashiCorpVaultEntry;
                         private readonly IVaultClient _vaultClient;
                         private readonly Timer{{Nullable}} _reloadTimer;
                     
                         public {{ClassNameBase}}Provider(HashiCorpVaultEntry hashiCorpVaultEntry)
                         {
                             _hashiCorpVaultEntry = hashiCorpVaultEntry;
                     
                             var auth = GetAuthMethod(hashiCorpVaultEntry.AuthMethod);
                             _vaultClient = new VaultClient(new VaultClientSettings(hashiCorpVaultEntry.Url.ToString(), auth));
                     
                             if (hashiCorpVaultEntry.CacheTimeoutInSeconds != default)
                             {
                                 _reloadTimer = new Timer
                                 {
                                     Enabled = true,
                                     Interval = TimeSpan.FromSeconds(hashiCorpVaultEntry.CacheTimeoutInSeconds).TotalMilliseconds,
                                     AutoReset = true
                                 };
                     
                                 _reloadTimer.Elapsed += (_, _) => Load();
                             }
                         }
                     
                         private static IAuthMethodInfo GetAuthMethod(HashiCorpVaultAuthMethod authMethod)
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
                             var keys = _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: _hashiCorpVaultEntry.Path, mountPoint: _hashiCorpVaultEntry.MountPoint).GetAwaiter().GetResult();
                             Data.Clear();
                     
                             var flattenedKeys = keys.Data.Data.SelectMany(JsonConfigurationFileParser.Convert);
                             foreach (var entry in flattenedKeys)
                             {
                                 Data.Add(entry.Key, entry.Value);
                             }
                         }
                         
                         internal class JsonConfigurationFileParser
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
                     }
                     """;
        }
    }
}