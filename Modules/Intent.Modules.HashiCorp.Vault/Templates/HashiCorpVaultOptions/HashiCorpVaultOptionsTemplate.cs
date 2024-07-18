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
                     }
                     
                     public class HashiCorpVaultAuthMethod
                     {
                         public HashiCorpVaultAuthMethodToken{{Nullable}} Token { get; set; }
                         public HashiCorpVaultAuthMethodUserPass{{Nullable}} UserPass { get; set; }
                         public HashiCorpVaultAuthMethodAppRole{{Nullable}} AppRole { get; set; }
                     }
                     
                     public class HashiCorpVaultAuthMethodToken
                     {
                         public HashiCorpVaultAuthMethodToken()
                         {
                             Token = null!;
                         }
                     
                         public string Token { get; set; }
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
                     }
                     """;
        }
    }
}