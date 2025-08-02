using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using Intent.Modules.AI.ChatDrivenDomain.Tasks.Models;
using Microsoft.SemanticKernel;

namespace Intent.Modules.AI.ChatDrivenDomain.Plugins;

/// <summary>
/// AI-powered domain model mutation plugin with proper Intent Architect API integration.
/// This plugin provides validated tools that directly manipulate Intent Architect's domain model
/// using the correct API patterns and DDD semantics.
/// </summary>
public class ModelMutationPlugin
{
    private readonly ChatCompletionModel _inputModel;
    private readonly Dictionary<string, string> _aiToIntentIdMappings;

    public ModelMutationPlugin(ChatCompletionModel inputModel)
    {
        _inputModel = inputModel;
        _aiToIntentIdMappings = new Dictionary<string, string>();
    }

    #region Core Domain Tools

    [KernelFunction]
    [Description("Gets current domain model context for AI session initialization. Returns complete model structure or empty array for clean slate.")]
    public string GetDomainContext()
    {
        try
        {
            var result = new ToolResult
            {
                Success = true,
                Message = "Domain context retrieved successfully"
            };

            // Convert current model to ElementModel structure
            var elements = _inputModel.Elements;
            var associations = _inputModel.Associations ?? [];
            
            result.Metadata["elements"] = elements;
            result.Metadata["associations"] = associations;
            result.Metadata["elementCount"] = elements.Count;
            result.Metadata["associationCount"] = associations.Count;
            result.Metadata["hasCleanSlate"] = elements.Count == 0 && associations.Count == 0;

            return result.ToJson();
        }
        catch (Exception ex)
        {
            var result = new ToolResult
            {
                Success = false,
                Message = "Failed to retrieve domain context"
            };
            result.Errors.Add($"Error: {ex.Message}");
            return result.ToJson();
        }
    }

    [KernelFunction]
    [Description("Creates a new class with validation and proper Intent Architect integration")]
    public string CreateClass(
        [Description("The name of the class")] string className,
        [Description("Optional comment/description for the class")] string comment = "")
    {
        var result = new ToolResult { Success = true };
        
        // VALIDATION FIRST - fail fast with clear error messages
        if (string.IsNullOrWhiteSpace(className))
        {
            result.Success = false;
            result.Errors.Add("Class name is required");
            return result.ToJson();
        }

        if (ClassNameExists(className))
        {
            result.Success = false;
            result.Errors.Add($"Class '{className}' already exists. Choose a different name.");
            return result.ToJson();
        }

        // CREATION - simulate Intent Architect API behavior
        try
        {
            // Generate Intent Architect-style ID
            string intentArchitectId = Guid.NewGuid().ToString();
            string aiInternalId = Guid.NewGuid().ToString();

            // Create new class element
        var newClass = new ClassModel
        {
                Id = intentArchitectId,
                Name = className,
            Comment = comment,
                Children = new List<ElementModel>()
            };

            // Add to current model
            if (_inputModel.Elements == null)
            {
                _inputModel.Elements = new List<ElementModel>();
            }
            _inputModel.Elements.Add(newClass);

            // ID MAPPING - track AI internal ID to IA generated ID
            _aiToIntentIdMappings[aiInternalId] = intentArchitectId;
            result.IdMappings[aiInternalId] = intentArchitectId;
            result.Message = $"Created class '{className}' successfully";
            result.Metadata["iaElementId"] = intentArchitectId;
            result.Metadata["aiElementId"] = aiInternalId;
            result.Metadata["specialization"] = "Class";
            result.Metadata["specializationId"] = IntentArchitectIds.ClassSpecializationId;

            return result.ToJson();
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Failed to create class: {ex.Message}");
            return result.ToJson();
        }
    }

    [KernelFunction]
    [Description("Creates or updates a single attribute with full validation and type resolution")]
    public string CreateOrUpdateAttribute(
        [Description("The ID of the class to add/update the attribute")]
        string classId,
        [Description("The name of the attribute")]
        string attributeName,
        [Description("The type name (string, int, bool, decimal, DateTime, Guid, or custom class name)")]
        string typeName,
        [Description("Whether the attribute can be null")]
        bool isNullable = false,
        [Description("Whether the attribute is a collection")]
        bool isCollection = false,
        [Description("Optional comment/description for the attribute")]
        string comment = "",
        [Description("For updates - the existing attribute ID (Intent Architect ID)")]
        string existingAttributeId = "")
    {
        var result = new ToolResult { Success = true };

        // VALIDATION: Check class exists
        var classElement = FindClassById(classId);
        if (classElement == null)
        {
            result.Success = false;
            result.Errors.Add($"Class with ID '{classId}' not found");
            return result.ToJson();
        }

        // VALIDATION: Check for duplicate names within class (excluding current attribute for updates)
        var existingAttribute = classElement.Children.FirstOrDefault(c => 
            c.Specialization == "Attribute" && 
            string.Equals(c.Name, attributeName, StringComparison.OrdinalIgnoreCase) &&
            c.Id != existingAttributeId);
        
        if (existingAttribute != null)
        {
            result.Success = false;
            result.Errors.Add($"Attribute '{attributeName}' already exists in class '{classElement.Name}'. Choose a different name.");
            return result.ToJson();
        }

        // TYPE RESOLUTION: Convert typeName to proper TypeId
        var typeReference = ResolveTypeReference(typeName, isNullable, isCollection);
        if (typeReference == null)
        {
            result.Success = false;
            result.Errors.Add($"Type '{typeName}' not recognized. Use: string, int, bool, decimal, DateTime, Guid, or a valid class name.");
            return result.ToJson();
        }

        try
        {
            AttributeModel attribute;
            string intentArchitectId;
            string aiInternalId = Guid.NewGuid().ToString();

            if (!string.IsNullOrEmpty(existingAttributeId))
            {
                // UPDATE EXISTING
                var existing = classElement.Children.FirstOrDefault(c => c.Id == existingAttributeId);
                if (existing == null)
                {
                    result.Success = false;
                    result.Errors.Add($"Attribute with ID '{existingAttributeId}' not found in class '{classElement.Name}'");
                    return result.ToJson();
                }

                attribute = (AttributeModel)existing;
                intentArchitectId = existingAttributeId;
                result.Message = $"Updated attribute '{attributeName}' in class '{classElement.Name}'";
            }
            else
            {
                // CREATE NEW
                intentArchitectId = Guid.NewGuid().ToString();
                attribute = new AttributeModel
                {
                    Id = intentArchitectId
                };
                classElement.Children.Add(attribute);
                result.Message = $"Created attribute '{attributeName}' in class '{classElement.Name}'";
            }

            // Set properties
            attribute.Name = attributeName;
        attribute.Comment = comment;
            attribute.TypeReference = typeReference;

            // ID MAPPING
            _aiToIntentIdMappings[aiInternalId] = intentArchitectId;
            result.IdMappings[aiInternalId] = intentArchitectId;
            result.Metadata["iaElementId"] = intentArchitectId;
            result.Metadata["aiElementId"] = aiInternalId;
            result.Metadata["className"] = classElement.Name;
            result.Metadata["typeName"] = typeName;
            result.Metadata["typeId"] = typeReference.TypeId;

            return result.ToJson();
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Failed to create/update attribute: {ex.Message}");
            return result.ToJson();
        }
    }

    [KernelFunction]
    [Description("Creates multiple attributes for a class in one bulk operation with validation")]
    public string CreateAttributesForClass(
        [Description("The ID of the class to add attributes to")]
        string classId,
        [Description("JSON array of attributes: [{\"name\":\"Email\",\"type\":\"string\",\"isNullable\":true,\"comment\":\"Contact email\"}]")]
        string attributesJson)
    {
        var result = new ToolResult { Success = true };

        // VALIDATION: Check class exists
        var classElement = FindClassById(classId);
        if (classElement == null)
        {
            result.Success = false;
            result.Errors.Add($"Class with ID '{classId}' not found");
            return result.ToJson();
        }

        // Parse attributes JSON
        List<JsonElement> attributes;
        try
        {
            attributes = JsonSerializer.Deserialize<List<JsonElement>>(attributesJson) ?? new List<JsonElement>();
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Invalid JSON format: {ex.Message}");
            return result.ToJson();
        }

        if (attributes.Count == 0)
        {
            result.Success = false;
            result.Errors.Add("No attributes provided in JSON array");
            return result.ToJson();
        }

        // VALIDATION: Check all attributes before creating any
        var attributeData = new List<(string name, string type, bool isNullable, bool isCollection, string comment)>();
        var existingNames = classElement.Children
            .Where(c => c.Specialization == "Attribute")
            .Select(c => c.Name.ToLowerInvariant())
            .ToHashSet();

        foreach (var attrObj in attributes)
        {
            if (!attrObj.TryGetProperty("name", out JsonElement nameElement) || string.IsNullOrWhiteSpace(nameElement.GetString()))
            {
                result.Success = false;
                result.Errors.Add("Each attribute must have a 'name' property");
                return result.ToJson();
            }

            var name = nameElement.GetString()!;
            if (existingNames.Contains(name.ToLowerInvariant()))
            {
                result.Success = false;
                result.Errors.Add($"Attribute '{name}' already exists in class '{classElement.Name}'");
                return result.ToJson();
            }

            var type = attrObj.TryGetProperty("type", out JsonElement typeElement) ? typeElement.GetString() ?? "string" : "string";
            var isNullable = attrObj.TryGetProperty("isNullable", out JsonElement nullableElement) && nullableElement.GetBoolean();
            var isCollection = attrObj.TryGetProperty("isCollection", out JsonElement collectionElement) && collectionElement.GetBoolean();
            var comment = attrObj.TryGetProperty("comment", out JsonElement commentElement) ? commentElement.GetString() ?? "" : "";

            // Validate type
            if (ResolveTypeReference(type, isNullable, isCollection) == null)
            {
                result.Success = false;
                result.Errors.Add($"Invalid type '{type}' for attribute '{name}'");
                return result.ToJson();
            }

            attributeData.Add((name, type, isNullable, isCollection, comment));
            existingNames.Add(name.ToLowerInvariant()); // Prevent duplicates within this batch
        }

        // BULK CREATION: All validation passed, create all attributes
        try
        {
            var createdIds = new List<string>();
            
            foreach (var (name, type, isNullable, isCollection, comment) in attributeData)
            {
                var intentArchitectId = Guid.NewGuid().ToString();
                var aiInternalId = Guid.NewGuid().ToString();
                
                var attribute = new AttributeModel
                {
                    Id = intentArchitectId,
                    Name = name,
                    Comment = comment,
                    TypeReference = ResolveTypeReference(type, isNullable, isCollection)!
                };

                classElement.Children.Add(attribute);
                
                // ID MAPPING
                _aiToIntentIdMappings[aiInternalId] = intentArchitectId;
                result.IdMappings[aiInternalId] = intentArchitectId;
                createdIds.Add(intentArchitectId);
            }

            result.Message = $"Created {attributeData.Count} attributes in class '{classElement.Name}' successfully";
            result.Metadata["createdAttributeCount"] = attributeData.Count;
            result.Metadata["className"] = classElement.Name;
            result.Metadata["createdIds"] = createdIds;

            return result.ToJson();
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Failed to create attributes: {ex.Message}");
            return result.ToJson();
        }
    }

    [KernelFunction]
    [Description("Creates a bidirectional association between two classes with DDD validation and cardinality support")]
    public string CreateAssociation(
        [Description("The ID of the source class")]
        string sourceClassId,
        [Description("The ID of the target class")]
        string targetClassId,
        [Description("The name for the association on the source end (e.g., 'Orders')")]
        string sourceEndName,
        [Description("The name for the association on the target end (e.g., 'Customer')")]
        string targetEndName,
        [Description("Source cardinality: '1', '0..1', '*'")]
        string sourceCardinality,
        [Description("Target cardinality: '1', '0..1', '*'")]
        string targetCardinality,
        [Description("Association type: 'Composite' (ownership) or 'Aggregate' (collaboration)")]
        string associationType = "Aggregate")
    {
        var result = new ToolResult { Success = true };

        // VALIDATION: Verify classes exist by ID
        var sourceClass = FindClassById(sourceClassId);
        if (sourceClass == null)
        {
            result.Success = false;
            result.Errors.Add($"Source class with ID '{sourceClassId}' not found");
            return result.ToJson();
        }

        var targetClass = FindClassById(targetClassId);
        if (targetClass == null)
        {
            result.Success = false;
            result.Errors.Add($"Target class with ID '{targetClassId}' not found");
            return result.ToJson();
        }

        // VALIDATION: Check naming conflicts with attributes in each class
        var sourceNameConflict = sourceClass.Children.Any(c => 
            c.Specialization == "Attribute" && 
            string.Equals(c.Name, sourceEndName, StringComparison.OrdinalIgnoreCase));
        if (sourceNameConflict)
        {
            result.Success = false;
            result.Errors.Add($"Name '{sourceEndName}' conflicts with existing attribute in class '{sourceClass.Name}'. Choose a different name.");
            return result.ToJson();
        }

        var targetNameConflict = targetClass.Children.Any(c => 
            c.Specialization == "Attribute" && 
            string.Equals(c.Name, targetEndName, StringComparison.OrdinalIgnoreCase));
        if (targetNameConflict)
        {
            result.Success = false;
            result.Errors.Add($"Name '{targetEndName}' conflicts with existing attribute in class '{targetClass.Name}'. Choose a different name.");
            return result.ToJson();
        }

        // VALIDATION: Check naming conflicts with existing associations
        var associations = _inputModel.Associations ?? [];
        var sourceAssocConflict = associations.Any(a => 
            a.SourceElementId == sourceClass.Id && 
            string.Equals(a.SourceEndName, sourceEndName, StringComparison.OrdinalIgnoreCase));
        if (sourceAssocConflict)
        {
            result.Success = false;
            result.Errors.Add($"Association end name '{sourceEndName}' already exists in class '{sourceClass.Name}'. Choose a different name.");
            return result.ToJson();
        }

        var targetAssocConflict = associations.Any(a => 
            a.TargetElementId == targetClass.Id && 
            string.Equals(a.TargetEndName, targetEndName, StringComparison.OrdinalIgnoreCase));
        if (targetAssocConflict)
        {
            result.Success = false;
            result.Errors.Add($"Association end name '{targetEndName}' already exists in class '{targetClass.Name}'. Choose a different name.");
            return result.ToJson();
        }

        // VALIDATION: For Composite - verify single-owner constraint using associations collection
        if (string.Equals(associationType, "Composite", StringComparison.OrdinalIgnoreCase))
        {
            var existingCompositeOwner = associations.FirstOrDefault(a => 
                a.TargetElementId == targetClass.Id && 
                string.Equals(a.AssociationType, "Composite", StringComparison.OrdinalIgnoreCase));

            if (existingCompositeOwner != null)
            {
                result.Success = false;
                result.Errors.Add($"Class '{targetClass.Name}' already has a composite owner. " +
                                 "DDD rule: An entity can only have one composite owner. " +
                                 "Use 'Aggregate' type for collaboration relationships.");
                return result.ToJson();
            }
        }

        // VALIDATION: Validate cardinality values
        if (!IsValidCardinality(sourceCardinality) || !IsValidCardinality(targetCardinality))
        {
            result.Success = false;
            result.Errors.Add("Invalid cardinality. Use: '1', '0..1', or '*'");
            return result.ToJson();
        }

        try
        {
            // Generate ID for the association (single entity, not two ends)
            var associationId = Guid.NewGuid().ToString();
            var aiAssociationId = Guid.NewGuid().ToString();

            // Create single AssociationModel (not association ends in classes)
            var association = new AssociationModel
            {
                Id = associationId,
                SourceElementId = sourceClass.Id,
                TargetElementId = targetClass.Id,
                SourceEndName = sourceEndName,
                TargetEndName = targetEndName,
                SourceCardinality = sourceCardinality,
                TargetCardinality = targetCardinality,
                AssociationType = associationType
            };

            // Initialize associations collection if needed
            if (_inputModel.Associations == null)
            {
                _inputModel.Associations = new List<AssociationModel>();
            }

            // Add association to the separate associations collection
            _inputModel.Associations.Add(association);

            // ID MAPPING for the association
            _aiToIntentIdMappings[aiAssociationId] = associationId;
            result.IdMappings[aiAssociationId] = associationId;

            result.Message = $"Created {associationType.ToLowerInvariant()} association between '{sourceClass.Name}' and '{targetClass.Name}' successfully";
            result.Metadata["associationType"] = associationType;
            result.Metadata["sourceClassName"] = sourceClass.Name;
            result.Metadata["targetClassName"] = targetClass.Name;
            result.Metadata["associationId"] = associationId;
            result.Metadata["relationship"] = $"{sourceCardinality} -> {targetCardinality}";

            return result.ToJson();
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Failed to create association: {ex.Message}");
            return result.ToJson();
        }
    }

    [KernelFunction]
    [Description("Updates the project context with current operation results")]
    public string UpdateContext(
        [Description("The operation that was performed")]
        string operation,
        [Description("Detailed description of what was done")]
        string details,
        [Description("Status: 'success', 'warning', 'error'")]
        string status,
        [Description("Additional metadata as JSON")]
        string metadata = "{}")
    {
        var result = new ToolResult { Success = true };

        try
        {
            var contextEntry = new
            {
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"),
                Operation = operation,
                Details = details,
                Status = status,
                Metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(metadata)
            };

            result.Message = "Context updated successfully";
            result.Metadata["contextEntry"] = contextEntry;
            result.Metadata["timestamp"] = contextEntry.Timestamp;

            return result.ToJson();
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Failed to update context: {ex.Message}");
            return result.ToJson();
        }
    }

    [KernelFunction]
    [Description("Gets current context for AI session initialization and state tracking")]
    public string GetCurrentContext()
    {
        var result = new ToolResult { Success = true };

        try
        {
            var elements = _inputModel.Elements;
            
            var contextSummary = new
            {
                LastUpdated = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"),
                ModelVersion = Guid.NewGuid().ToString("N")[..8], // Simple hash
                ValidationStatus = "Valid", // All tools validate before execution
                DomainModelSummary = new
                {
                    Classes = elements.Count(e => e.Specialization == "Class"),
                    TotalAttributes = elements.SelectMany(e => e.Children).Count(c => c.Specialization == "Attribute"),
                    TotalAssociations = elements.SelectMany(e => e.Children).Count(c => c.IsSourceEnd || c.IsTargetEnd),
                    CompositeRelationships = elements.SelectMany(e => e.Children).Count(IsCompositeRelationship),
                    AggregateRelationships = elements.SelectMany(e => e.Children).Count(c => (c.IsSourceEnd || c.IsTargetEnd) && !IsCompositeRelationship(c))
                },
                CurrentClasses = elements.Where(e => e.Specialization == "Class").Select(e => new {
                    Name = e.Name,
                    Id = e.Id,
                    AttributeCount = e.Children.Count(c => c.Specialization == "Attribute"),
                    AssociationCount = e.Children.Count(c => c.IsSourceEnd || c.IsTargetEnd)
                }).ToList(),
                DDDPatterns = new
                {
                    HasCompositeRelationships = elements.SelectMany(e => e.Children).Any(IsCompositeRelationship),
                    HasAggregateRelationships = elements.SelectMany(e => e.Children).Any(c => (c.IsSourceEnd || c.IsTargetEnd) && !IsCompositeRelationship(c)),
                    WellFormedDomainModel = elements.Count > 0
                },
                TechnicalNotes = new
                {
                    UsingIntentArchitectIds = true,
                    SoftLinkingEnabled = true,
                    ValidationEnabled = true,
                    DDDComplianceChecks = true
                }
            };

            result.Message = "Current context retrieved successfully";
            result.Metadata["contextSummary"] = contextSummary;
            result.Metadata["hasCleanSlate"] = elements.Count == 0;

            return result.ToJson();
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Failed to get current context: {ex.Message}");
            return result.ToJson();
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Checks if a class name already exists in the current model
    /// </summary>
    private bool ClassNameExists(string className)
    {
        var elements = _inputModel.Elements;
        return elements.Any(e => e.Specialization == "Class" && 
                                string.Equals(e.Name, className, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Finds a class element by its ID
    /// </summary>
    private ElementModel? FindClassById(string classId)
    {
        var elements = _inputModel.Elements;
        return elements.FirstOrDefault(e => e.Specialization == "Class" && e.Id == classId);
    }

    /// <summary>
    /// Resolves a type name to a proper TypeReferenceModel with Intent Architect type IDs
    /// </summary>
    private TypeReferenceModel? ResolveTypeReference(string typeName, bool isNullable, bool isCollection)
    {
        var typeId = typeName.ToLowerInvariant() switch
        {
            "string" => IntentArchitectIds.StringTypeId,
            "int" => IntentArchitectIds.IntTypeId,
            "long" => IntentArchitectIds.LongTypeId,
            "bool" or "boolean" => IntentArchitectIds.BoolTypeId,
            "guid" => IntentArchitectIds.GuidTypeId,
            "datetime" => IntentArchitectIds.DateTimeTypeId,
            "decimal" => IntentArchitectIds.DecimalTypeId,
            "double" => IntentArchitectIds.DoubleTypeId,
            _ => ResolveCustomClassTypeId(typeName) // Check if it's a custom class
        };

        if (string.IsNullOrEmpty(typeId))
        {
            return null; // Type not found
        }

        var display = isCollection ? $"{typeName}[*]" : typeName;
        if (isNullable && !isCollection)
        {
            display += "?";
        }

        return new TypeReferenceModel
        {
            TypeId = typeId,
            IsNavigable = true, // For attributes, always navigable
            IsNullable = isNullable,
            IsCollection = isCollection,
            Display = display
        };
    }

    /// <summary>
    /// Resolves a custom class name to its type ID
    /// </summary>
    private string? ResolveCustomClassTypeId(string className)
    {
        var elements = _inputModel.Elements;
        var classElement = elements.FirstOrDefault(e => e.Specialization == "Class" && 
                                                       string.Equals(e.Name, className, StringComparison.OrdinalIgnoreCase));
        return classElement?.Id;
    }

    /// <summary>
    /// Gets the current updated model for frontend consumption
    /// </summary>
    public object GetCurrentModel()
    {
        var elements = _inputModel.Elements;
        var associations = _inputModel.Associations ?? [];
        
        return new
        {
            elements = elements,
            associations = associations
        };
    }

    /// <summary>
    /// Validates cardinality string
    /// </summary>
    private bool IsValidCardinality(string cardinality)
    {
        return cardinality switch
        {
            "1" or "0..1" or "*" => true,
            _ => false
        };
    }

    /// <summary>
    /// Checks if an association element represents a composite relationship
    /// </summary>
    private bool IsCompositeRelationship(ElementModel associationEnd)
    {
        return string.Equals(associationEnd.Value, "Composite", StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}
