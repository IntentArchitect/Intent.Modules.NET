using System;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.DocumentDB.Shared;

namespace Intent.Modules.MongoDb.FactoryExtensions;

public class MongoDbPrimaryKeyInitStrategy : IPrimaryKeyInitStrategy
{
    public bool ShouldInsertPkInitializationCode(ClassModel targetClass)
    {
        return !targetClass.IsAggregateRoot();
    }

    public string GetGetterInitExpression(ICSharpTemplate template, ClassModel targetClass, string fieldName, string fieldTypeName)
    {
        return fieldTypeName switch
        {
            "string" => $"{fieldName} ??= {template.UseType("System.Guid")}.NewGuid().ToString()",
            "guid" => $"{fieldName} ??= {template.UseType("System.Guid")}.NewGuid()",
            "int" or "long" => $"{fieldName} ?? throw new {template.UseType("System.NullReferenceException")}(\"{fieldName} has not been set\")",
            _ => fieldName
        };
    }
}