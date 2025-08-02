using System.Collections.Generic;
using System.Text.Json;

namespace Intent.Modules.AI.ChatDrivenDomain.Tasks.Models;

/// <summary>
/// Input model for AI chat completion task containing user prompt and current domain model state
/// </summary>
public class ChatCompletionModel
{
    public string Prompt { get; set; } = string.Empty;
    public List<ElementModel> Elements { get; set; } = [];
    public List<AssociationModel> Associations { get; set; } = [];
    
    // Backward compatibility - will be removed after migration
    public List<LegacyClassModel> Classes { get; set; } = [];
}

/// <summary>
/// Base element model that exactly mirrors Intent Architect's IElementReadOnlyApi structure
/// This is the foundation for all domain model elements (classes, attributes, associations)
/// </summary>
public class ElementModel
{
    public string Id { get; set; } = null!;
    public string Specialization { get; set; } = null!; // "Class", "Attribute", "Association Source End", etc.
    public string SpecializationId { get; set; } = null!; // Intent's specialization type IDs
    public string Name { get; set; } = null!;
    public string Comment { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsAbstract { get; set; }
    public bool IsStatic { get; set; }
    public int Order { get; set; }
    
    // TypeReference for attributes and associations
    public TypeReferenceModel? TypeReference { get; set; }
    
    // Hierarchical structure - contains attributes and association ends
    public List<ElementModel> Children { get; set; } = [];
    
    // Association-specific properties
    public string? OtherEndId { get; set; } // Soft-link to other association end
    public bool IsSourceEnd { get; set; }
    public bool IsTargetEnd { get; set; }
}

/// <summary>
/// Type reference model that exactly mirrors Intent Architect's ITypeReferenceData structure
/// Used for attributes and association type references
/// </summary>
public class TypeReferenceModel
{
    public string TypeId { get; set; } = null!; // Class ID or primitive type ID
    public bool IsNavigable { get; set; } // Always false for Source End, true for Target End
    public bool IsNullable { get; set; }
    public bool IsCollection { get; set; }
    public string? GenericTypeId { get; set; }
    public List<TypeReferenceModel>? GenericTypeParameters { get; set; }
    public string Display { get; set; } = null!; // Auto-generated: "Order[*]", "string", etc.
}

/// <summary>
/// Specialized model for Class elements with proper Intent Architect integration
/// </summary>
public class ClassModel : ElementModel
{
    public ClassModel()
    {
        Specialization = "Class";
        SpecializationId = IntentArchitectIds.ClassSpecializationId;
    }
}

/// <summary>
/// Specialized model for Attribute elements with proper Intent Architect integration
/// </summary>
public class AttributeModel : ElementModel
{
    public AttributeModel()
    {
        Specialization = "Attribute";
        SpecializationId = IntentArchitectIds.AttributeSpecializationId;
    }
}

/// <summary>
/// Specialized model for Association End elements with proper Intent Architect integration
/// </summary>
public class AssociationEndModel : ElementModel
{
    public AssociationEndModel(bool isSourceEnd)
    {
        IsSourceEnd = isSourceEnd;
        IsTargetEnd = !isSourceEnd;
        Specialization = isSourceEnd ? "Association Source End" : "Association Target End";
        SpecializationId = isSourceEnd 
            ? IntentArchitectIds.AssociationSourceEndId 
            : IntentArchitectIds.AssociationTargetEndId;
        TypeReference = new TypeReferenceModel { IsNavigable = !isSourceEnd };
    }
}

/// <summary>
/// Intent Architect constants for specialization IDs and primitive type IDs
/// These are the actual IDs used by Intent Architect for element types
/// </summary>
public static class IntentArchitectIds
{
    // Element Specialization IDs
    public const string ClassSpecializationId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10";
    public const string AttributeSpecializationId = "0090fb93-483e-41af-a11d-5ad2dc796adf";
    public const string AssociationSourceEndId = "8d9d2e5b-bd55-4f36-9ae4-2b9e84fd4e58";
    public const string AssociationTargetEndId = "0a66489f-30aa-417b-a75d-b945863366fd";
    
    // Primitive Type IDs (for TypeReference.TypeId)
    public const string StringTypeId = "d384db9c-a279-45e1-801e-e4e8099625f2";
    public const string IntTypeId = "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74";
    public const string LongTypeId = "33013006-E404-48C2-AC46-24EF5A5774FD";
    public const string BoolTypeId = "e6f92b09-b2c5-4536-8270-a4d9e5bbd930";
    public const string GuidTypeId = "6b649125-18ea-48fd-a6ba-0bfff0d8f488";
    public const string DateTimeTypeId = "a4107c29-7851-4121-9416-cf1236908f1e";
    public const string DecimalTypeId = "675c7b84-997a-44e0-82b9-cd724c07c9e6";
    public const string DoubleTypeId = "24A77F70-5B97-40DD-8F9A-4208AD5F9219";
}

/// <summary>
/// Standardized result structure for all AI tools with validation and ID mapping support
/// </summary>
public class ToolResult
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string> IdMappings { get; set; } = []; // AI_ID -> IA_ID
    public List<string> Errors { get; set; } = [];
    public Dictionary<string, object> Metadata { get; set; } = [];
    
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}

#region Legacy Models

/// <summary>
/// Legacy class model structure - kept for backward compatibility during migration
/// Will be removed once all tools are migrated to ElementModel
/// </summary>
public class LegacyClassModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public List<AssociationModel> Associations { get; set; } = [];
    public List<LegacyAttributeModel> Attributes { get; set; } = [];
}

/// <summary>
/// Legacy attribute model structure - kept for backward compatibility during migration
/// Will be removed once all tools are migrated to ElementModel
/// </summary>
public class LegacyAttributeModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsCollection { get; set; }
    public string Comment { get; set; } = string.Empty;
}

/// <summary>
/// Legacy association model structure - kept for backward compatibility during migration
/// Will be removed once all tools are migrated to ElementModel
/// </summary>
public class LegacyAssociationModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string SpecializationEndType { get; set; } = string.Empty;
    
    // Aliases for compatibility with different naming conventions
    public string Relationship => Type;
    public string AssociationEndType => SpecializationEndType;
    
    // Additional properties for clarity
    public bool IsNullable { get; set; }
    public bool IsCollection { get; set; }
}

/// <summary>
/// Association model representing relationships between domain elements
/// Replaces the flawed approach of treating association ends as child elements
/// </summary>
public class AssociationModel
{
    public string Id { get; set; } = null!;
    public string SourceElementId { get; set; } = null!;
    public string TargetElementId { get; set; } = null!;
    public string SourceEndName { get; set; } = null!;
    public string TargetEndName { get; set; } = null!;
    public string SourceCardinality { get; set; } = null!; // "1", "0..1", "*"
    public string TargetCardinality { get; set; } = null!; // "1", "0..1", "*"
    public string AssociationType { get; set; } = null!; // "Composite", "Aggregate"
}

#endregion
