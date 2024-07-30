using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace Intent.Modules.HashiCorp.Vault.Templates.HashiCorpVaultOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HashiCorpVaultOptionsTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$"""
                     using System;
                     using System.Collections.Generic;
                     using Microsoft.Extensions.Configuration;

                     [assembly: DefaultIntentManaged(Mode.Fully)]

                     namespace {{Namespace}};
                     
                     public class {{ClassName}}
                     {
                         public bool Enabled { get; set; }
                         public List<HashiCorpVaultEntry> Vaults { get; set; } = new List<HashiCorpVaultEntry>();
                     }
                     
                     public class HashiCorpVaultEntry
                     {
                         public HashiCorpVaultEntry()
                         {
                             Url = null!;
                             Path = null!;
                             MountPoint = null!;
                         }
                     
                         public string Name { get; set; }
                         public Uri Url { get; set; }
                         public HashiCorpVaultAuthMethod AuthMethod { get; set; } = new HashiCorpVaultAuthMethod();
                         public string Path { get; set; }
                         public string MountPoint { get; set; }
                         public int CacheTimeoutInSeconds { get; set; }
                         
                         public void ApplyShorthandConfig(IReadOnlyList<IConfigurationSection> config)
                         {
                             foreach (var section in config)
                             {
                                 var parts = section.Key.Split("_", StringSplitOptions.RemoveEmptyEntries);
                                 if (parts.Length != 2)
                                 {
                                     continue;
                                 }
                         
                                 if (string.IsNullOrWhiteSpace(section.Value))
                                 {
                                     continue;
                                 }
                                 var value = section.Value!;
                         
                                 var property = parts[1];
                                 switch (property)
                                 {
                                     case nameof(Url):
                                         Url = new Uri(value);
                                         break;
                                     case nameof(Path):
                                         Path = value;
                                         break;
                                     case nameof(MountPoint):
                                         MountPoint = value;
                                         break;
                                     case nameof(CacheTimeoutInSeconds):
                                         CacheTimeoutInSeconds = int.Parse(value);
                                         break;
                                     default:
                                         AuthMethod.ApplyShorthandConfig(property, value);
                                         break;
                                 }
                             }
                         }
                     }
                     
                     public class HashiCorpVaultAuthMethod
                     {
                         public HashiCorpVaultAuthMethodAppRole? AppRole { get; set; }
                         public HashiCorpVaultAuthMethodUserPass? UserPass { get; set; }
                         public HashiCorpVaultAuthMethodToken? Token { get; set; }
                         
                         public void ApplyShorthandConfig(string property, string value)
                         {
                             if (AppRole is not null)
                             {
                                 AppRole.ApplyShorthandConfig(property, value);
                             }
                             else if (UserPass is not null)
                             {
                                 UserPass.ApplyShorthandConfig(property, value);
                             }
                             else if (Token is not null)
                             {
                                 Token.ApplyShorthandConfig(property, value);
                             }
                         }
                     }
                     
                     public class HashiCorpVaultAuthMethodAppRole
                     {
                         public HashiCorpVaultAuthMethodAppRole()
                         {
                             RoleId = null!;
                             SecretId = null!;
                         }
                         
                         public string RoleId { get; set; }
                         public string SecretId { get; set; }
                         
                         public void ApplyShorthandConfig(string property, string value)
                         {
                             switch (property)
                             {
                                 case nameof(RoleId):
                                     RoleId = value;
                                     break;
                                 case nameof(SecretId):
                                     SecretId = value;
                                     break;
                             }
                         }
                     }
                     
                     public class HashiCorpVaultAuthMethodUserPass
                     {
                         public HashiCorpVaultAuthMethodUserPass()
                         {
                             Username = null!;
                             Password = null!;
                         }
                     
                         public string Username { get; set; }
                         public string Password { get; set; }
                         
                         public void ApplyShorthandConfig(string property, string value)
                         {
                             switch (property)
                             {
                                 case nameof(Username):
                                     Username = value;
                                     break;
                                 case nameof(Password):
                                     Password = value;
                                     break;
                             }
                         }
                     }
                     
                     public class HashiCorpVaultAuthMethodToken
                     {
                         public HashiCorpVaultAuthMethodToken()
                         {
                             Token = null!;
                         }
                     
                         public string Token { get; set; }
                         
                         public void ApplyShorthandConfig(string property, string value)
                         {
                             if (property == nameof(Token))
                             {
                                 Token = value;
                             }
                         }
                     }
                     """;
        }
    }
}