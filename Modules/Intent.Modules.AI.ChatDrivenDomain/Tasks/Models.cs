using System.Collections.Generic;

namespace Intent.Modules.AI.ChatDrivenDomain.Tasks;

public class InputModel
{
    public string Prompt { get; set; }
    public List<ClassModel> Classes { get; set; }
}

public class ClassModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Comment { get; set; }
    public List<AssociationModel> Associations { get; set; } = new List<AssociationModel>();
    public List<AttributeModel> Attributes { get; set; } = new List<AttributeModel>();
}

public class AssociationModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ClassId { get; set; }
    public string Type { get; set; }
    public string SpecializationEndType { get; set; }
    
    // Aliases for compatibility with different naming conventions
    public string Relationship => Type;
    public string AssociationEndType => SpecializationEndType;
    
    // Additional properties for clarity
    public bool IsNullable { get; set; }
    public bool IsCollection { get; set; }
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
