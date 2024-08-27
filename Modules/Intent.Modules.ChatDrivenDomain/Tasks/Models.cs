using System.Collections.Generic;

namespace Intent.Modules.ChatDrivenDomain.Tasks.Models;

public class SettingsResult
{
    public bool SettingsFileExists { get; set; }
    public SettingsData? Data { get; set; }
}

public class SettingsData
{
    public string ApiKey { get; set; }
    public string? Model { get; set; }
    public string? ApiUrl { get; set; }
}

public class InputModel
{
    public string Prompt { get; set; }
    public object Classes { get; set; }
}

public class ClassModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Comment { get; set; }
    public List<AssociationModel> Associations { get; set; }
    public List<AttributeModel> Attributes { get; set; }
}

public class AssociationModel
{
    public string Name { get; set; }
    public string Specialization { get; set; }
    public string SpecializationEndType { get; set; }
    public string ClassId { get; set; }
    public string Type { get; set; }
}

public class AttributeModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsNullable { get; set; }
    public bool IsCollection { get; set; }
    public string Comment { get; set; }
}