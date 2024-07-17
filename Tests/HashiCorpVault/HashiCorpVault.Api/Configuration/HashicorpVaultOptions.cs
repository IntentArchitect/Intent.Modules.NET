using System;
using System.Collections.Generic;

namespace HashiCorpVault.Api.Configuration;

public class HashicorpVaultOptions
{
    public bool Enabled { get; set; }
    public List<HashicorpVaultEntry> Vaults { get; set; }
}

public class HashicorpVaultEntry
{
    public string Name { get; set; }
    public Uri Url { get; set; }
    public HashicorpVaultAuthMethod AuthMethod { get; set; }
    public string Path { get; set; }
    public string MountPoint { get; set; }
    public int CacheTimeoutInSeconds { get; set; }
}

public class HashicorpVaultAuthMethod
{
    public HashicorpVaultAuthMethodToken? Token { get; set; }
    public HashicorpVaultAuthMethodUserPass? UserPass { get; set; }
    public HashicorpVaultAuthMethodAppRole? AppRole { get; set; }
}

public class HashicorpVaultAuthMethodToken
{
    public string Token { get; set; }
}

public class HashicorpVaultAuthMethodUserPass
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class HashicorpVaultAuthMethodAppRole
{
    public string RoleId { get; set; }
    public string SecretId { get; set; }
}