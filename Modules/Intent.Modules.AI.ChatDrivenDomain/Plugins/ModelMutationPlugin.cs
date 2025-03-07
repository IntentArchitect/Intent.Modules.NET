using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using Intent.Modules.AI.ChatDrivenDomain.Tasks;
using Microsoft.SemanticKernel;

namespace Intent.Modules.AI.ChatDrivenDomain.Plugins;

public class ModelMutationPlugin
{
    private readonly InputModel _inputModel;
    private readonly List<ClassModel> _classes;

    public ModelMutationPlugin(InputModel inputModel)
    {
        _inputModel = inputModel;
        _classes = new List<ClassModel>(inputModel.Classes);
    }

    [KernelFunction]
    [Description("Creates a new class in the domain model")]
    public string CreateClass(
        [Description("The name of the class")] string name,
        [Description("Optional comment/description for the class")]
        string comment = "")
    {
        var id = Guid.NewGuid().ToString();
        var newClass = new ClassModel
        {
            Id = id,
            Name = name,
            Comment = comment,
            Associations = [],
            Attributes = []
        };

        _classes.Add(newClass);
        return id;
    }

    [KernelFunction]
    [Description("Creates a new attribute for an existing class")]
    public string CreateAttribute(
        [Description("The ID of the class to add the attribute to")]
        string classId,
        [Description("The name of the attribute")]
        string name,
        [Description("The type of the attribute (string, int, long, decimal, datetime, bool, guid, object)")]
        string type,
        [Description("Whether the attribute can be null")]
        bool isNullable = false,
        [Description("Whether the attribute is a collection")]
        bool isCollection = false,
        [Description("Optional comment/description for the attribute")]
        string comment = "")
    {
        var classModel = FindClass(classId);
        if (classModel == null)
        {
            return $"Error: Class with ID '{classId}' not found";
        }

        var id = Guid.NewGuid().ToString();
        var attribute = new AttributeModel
        {
            Id = id,
            Name = name,
            Type = type,
            IsNullable = isNullable,
            IsCollection = isCollection,
            Comment = comment
        };

        classModel.Attributes.Add(attribute);
        return id;
    }

    [KernelFunction]
    [Description("Updates an existing attribute in a class")]
    public string UpdateAttribute(
        [Description("The ID of the class containing the attribute")]
        string classId,
        [Description("The ID of the attribute to update")]
        string attributeId,
        [Description("The new name of the attribute")]
        string name,
        [Description("The new type of the attribute (string, int, long, decimal, datetime, bool, guid, object)")]
        string type,
        [Description("Whether the attribute can be null")]
        bool isNullable = false,
        [Description("Whether the attribute is a collection")]
        bool isCollection = false,
        [Description("Optional comment/description for the attribute")]
        string comment = "")
    {
        var classModel = FindClass(classId);
        if (classModel == null)
        {
            return $"Error: Class with ID '{classId}' not found";
        }

        var attribute = classModel.Attributes.FirstOrDefault(a => a.Id == attributeId);
        if (attribute == null)
        {
            return $"Error: Attribute with ID '{attributeId}' not found in class '{classModel.Name}'";
        }

        attribute.Name = name;
        attribute.Type = type;
        attribute.IsNullable = isNullable;
        attribute.IsCollection = isCollection;
        attribute.Comment = comment;
        
        return attributeId;
    }

    [KernelFunction]
    [Description("Removes an attribute from an existing class")]
    public string RemoveAttribute(
        [Description("The ID of the class to remove the attribute from")]
        string classId,
        [Description("The ID of the attribute to remove")]
        string attributeId)
    {
        var classModel = FindClass(classId);
        if (classModel == null)
        {
            return $"Error: Class with ID '{classId}' not found";
        }

        var attribute = classModel.Attributes.FirstOrDefault(a => a.Id == attributeId);
        if (attribute == null)
        {
            return $"Error: Attribute with ID '{attributeId}' not found in class '{classModel.Name}'";
        }

        classModel.Attributes.Remove(attribute);
        return $"Removed attribute '{attribute.Name}' from class '{classModel.Name}'";
    }

    [KernelFunction]
    [Description("Removes an association between two classes")]
    public string RemoveAssociation(
        [Description("The ID of the source class")]
        string sourceClassId,
        [Description("The ID of the target class")]
        string targetClassId)
    {
        var sourceClass = FindClass(sourceClassId);
        if (sourceClass == null)
        {
            return $"Error: Source class with ID '{sourceClassId}' not found";
        }

        var targetClass = FindClass(targetClassId);
        if (targetClass == null)
        {
            return $"Error: Target class with ID '{targetClassId}' not found";
        }

        sourceClass.Associations.RemoveAll(a => a.ClassId == targetClass.Id);
        targetClass.Associations.RemoveAll(a => a.ClassId == sourceClass.Id);

        return $"Removed association between '{sourceClass.Name}' and '{targetClass.Name}'";
    }

    [KernelFunction]
    [Description("Creates an association between two classes")]
    public string CreateAssociation(
        [Description("The ID of the source class")]
        string sourceClassId,
        [Description("The ID of the target class")]
        string targetClassId,
        [Description("The relationship type (e.g., '1 -> *', '1 -> 1', '* -> *', '1 -> 0..1')")]
        string relationship)
    {
        var sourceClass = FindClass(sourceClassId);
        if (sourceClass == null)
        {
            return $"Error: Source class with ID '{sourceClassId}' not found";
        }

        var targetClass = FindClass(targetClassId);
        if (targetClass == null)
        {
            return $"Error: Target class with ID '{targetClassId}' not found";
        }

        // Parse relationship to determine isNullable and isCollection
        var sourceIsCollection = relationship.Contains('*') && 
                                 relationship.IndexOf('*') < relationship.IndexOf("->", StringComparison.Ordinal);
        var sourceIsNullable = relationship.Contains("0..1") && 
                               relationship.IndexOf("0..1", StringComparison.Ordinal) < relationship.IndexOf("->", StringComparison.Ordinal);
        var targetIsCollection = relationship.Contains('*') && 
                                 relationship.IndexOf('*') > relationship.IndexOf("->", StringComparison.Ordinal);
        var targetIsNullable = relationship.Contains("0..1") && 
                               relationship.IndexOf("0..1", StringComparison.Ordinal) > relationship.IndexOf("->", StringComparison.Ordinal);

        // Generate a unique ID for the association
        var associationId = Guid.NewGuid().ToString();

        // Create source-to-target association
        var sourceAssociation = new AssociationModel
        {
            Id = associationId,
            Name = targetClass.Name,
            ClassId = targetClass.Id,
            Type = relationship,
            SpecializationEndType = "Target End",
            IsCollection = targetIsCollection,
            IsNullable = targetIsNullable
        };
        sourceClass.Associations.Add(sourceAssociation);

        // Create target-to-source association
        var targetAssociation = new AssociationModel
        {
            Id = associationId, // Same ID for both ends of the association
            Name = sourceClass.Name,
            ClassId = sourceClass.Id,
            Type = relationship,
            SpecializationEndType = "Source End",
            IsCollection = sourceIsCollection,
            IsNullable = sourceIsNullable
        };
        targetClass.Associations.Add(targetAssociation);

        return associationId;
    }

    [KernelFunction]
    [Description("Updates an existing class")]
    public string UpdateClass(
        [Description("The ID of the class to update")]
        string classId,
        [Description("The new name for the class")]
        string name,
        [Description("The new comment for the class")]
        string comment = "")
    {
        var classModel = FindClass(classId);
        if (classModel == null)
        {
            return $"Error: Class with ID '{classId}' not found";
        }

        classModel.Name = name;
        classModel.Comment = comment;

        return $"{classId}|{name}";
    }

    [KernelFunction]
    [Description("Removes a class from the domain model")]
    public string RemoveClass(
        [Description("The ID of the class to remove")]
        string classId)
    {
        var classModel = FindClass(classId);
        if (classModel == null)
        {
            return $"Error: Class with ID '{classId}' not found";
        }

        // Remove associations pointing to this class
        foreach (var cls in _classes)
        {
            cls.Associations.RemoveAll(a => a.ClassId == classModel.Id);
        }

        // Remove the class
        _classes.Remove(classModel);
        return $"Removed class '{classModel.Name}' and all associations to it";
    }

    [KernelFunction]
    [Description("Lists all classes in the domain model")]
    public string ListClasses()
    {
        if (_classes.Count == 0)
        {
            return "No classes found in the domain model.";
        }

        var result = "Classes in the domain model:\n";
        foreach (var cls in _classes)
        {
            result += $"- {cls.Name} (ID: {cls.Id})\n";
        }

        return result;
    }

    [KernelFunction]
    [Description("Gets detailed information about a specific class")]
    public string GetClassDetails(
        [Description("The name or ID of the class")]
        string classNameOrId)
    {
        var classModel = FindClass(classNameOrId);
        if (classModel == null)
        {
            return $"Error: Class '{classNameOrId}' not found";
        }

        var result = $"Class: {classModel.Name} (ID: {classModel.Id})\n";

        if (!string.IsNullOrEmpty(classModel.Comment))
        {
            result += $"Comment: {classModel.Comment}\n";
        }

        result += "\nAttributes:\n";
        if (classModel.Attributes.Count == 0)
        {
            result += "- None\n";
        }
        else
        {
            foreach (var attr in classModel.Attributes)
            {
                var typeInfo = attr.Type;
                if (attr.IsCollection)
                {
                    typeInfo += "[]";
                }

                if (attr.IsNullable)
                {
                    typeInfo += "?";
                }

                result += $"- {attr.Name}: {typeInfo}";
                if (!string.IsNullOrEmpty(attr.Comment))
                {
                    result += $" // {attr.Comment}";
                }

                result += "\n";
            }
        }

        result += "\nAssociations:\n";
        if (classModel.Associations.Count == 0)
        {
            result += "- None\n";
        }
        else
        {
            foreach (var assoc in classModel.Associations)
            {
                var targetClass = _classes.FirstOrDefault(c => c.Id == assoc.ClassId);
                result += $"- {assoc.Name} -> {targetClass?.Name ?? "Unknown"} ({assoc.Type})\n";
            }
        }

        return result;
    }

    private ClassModel? FindClass(string nameOrId)
    {
        return _classes.FirstOrDefault(c =>
            string.Equals(c.Name, nameOrId, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.Id, nameOrId, StringComparison.OrdinalIgnoreCase));
    }

    public List<ClassModel> GetClasses()
    {
        return _classes;
    }
}
