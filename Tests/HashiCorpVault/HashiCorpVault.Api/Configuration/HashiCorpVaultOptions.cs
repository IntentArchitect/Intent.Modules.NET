using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HashiCorp.Vault.HashiCorpVaultOptions", Version = "1.0")]

namespace HashiCorpVault.Api.Configuration;

public class HashiCorpVaultOptions
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
    public HashiCorpVaultAuthMethodToken? Token { get; set; }
    public HashiCorpVaultAuthMethodUserPass? UserPass { get; set; }
    public HashiCorpVaultAuthMethodAppRole? AppRole { get; set; }
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