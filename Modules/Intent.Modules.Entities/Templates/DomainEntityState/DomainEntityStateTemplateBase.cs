using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Modules.Entities.Templates.DomainEntityState;

public abstract class DomainEntityStateTemplateBase : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
{
    private DomainEntityInterfaceTemplate _interfaceTemplate;
    protected DomainEntityInterfaceTemplate InterfaceTemplate => _interfaceTemplate ?? GetTemplate<DomainEntityInterfaceTemplate>(DomainEntityInterfaceTemplate.TemplateId, Model);
    protected static string IsMerged => "is-merged";
    public CSharpFile CSharpFile { get; set; }

    [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
    protected DomainEntityStateTemplateBase(string templateId, IOutputTarget outputTarget, ClassModel model) : base(templateId, outputTarget, model)
    {
        SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateICollection());
        AddTypeSource(templateId);
        AddTypeSource(DomainEnumTemplate.TemplateId);
        AddTypeSource("Domain.ValueObject");
    }

    protected void AddProperties(CSharpClass @class)
    {
        foreach (var attribute in Model.Attributes)
        {
            AddProperty(@class, attribute.Name, attribute.TypeReference);

            if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters() &&
                !GetTypeName(attribute).Equals(InterfaceTemplate.GetTypeName(attribute)))
            {
                AddInterfaceQualifiedProperty(@class, attribute.Name, attribute.TypeReference);
            }
        }

        foreach (var associationEnd in Model.AssociatedClasses.Where(x => x.IsNavigable))
        {
            AddProperty(@class, associationEnd.Name, associationEnd);

            if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                !GetTypeName(associationEnd).Equals(InterfaceTemplate.GetTypeName(associationEnd)))
            {
                AddInterfaceQualifiedProperty(@class, associationEnd.Name, associationEnd);
            }
        }
    }

    protected void AddProperty(CSharpClass @class, string propertyName, ITypeReference typeReference)
    {
        @class.AddProperty(GetTypeName(typeReference), propertyName.ToPascalCase(), property =>
        {
            property.AddMetadata("model", typeReference);
            if (typeReference.Element.IsClassModel()) // not the most robust.
            {
                property.Virtual();
            }
            if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
            {
                property.PrivateSetter();
            }

            if (typeReference.IsCollection)
            {
                property.WithInitialValue($"new {UseType("System.Collections.Generic.List")}<{GetTypeName((IElement)typeReference.Element)}>()");
            }
        });
    }


    protected void AddInterfaceQualifiedProperty(CSharpClass @class, string propertyName, ITypeReference typeReference)
    {
        @class.AddProperty($"{InterfaceTemplate.GetTypeName(typeReference)}",
            $"{this.GetDomainEntityInterfaceName()}.{propertyName.ToPascalCase()}", property =>
            {
                if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                {
                    property.ReadOnly();
                }
                else
                {
                    property.Setter.WithExpressionImplementation($"{propertyName.ToPascalCase()} = {CastArgumentIfNecessary(typeReference, "value")}");
                }

                property.WithoutAccessModifier();
                property.Getter.WithExpressionImplementation(propertyName.ToPascalCase());
            });

        if (typeReference.IsCollection &&
            !ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
        {
            @class.AddMethod("void",
                $"{this.GetDomainEntityInterfaceName()}.Add{propertyName.ToPascalCase().Singularize()}",
                method =>
                {
                    if (@class.TryGetMetadata<bool>(IsMerged, out var isMerged) && isMerged)
                    {
                        method.AddAttribute($"IntentManaged(Mode.Fully)");
                    }

                    method.WithoutAccessModifier();
                    method.AddParameter(InterfaceTemplate.GetTypeName((IElement)typeReference.Element),
                        $"{propertyName.ToCamelCase().Singularize()}");
                    method.AddStatement(
                        $"{propertyName.ToPascalCase()}.Add(({GetTypeName((IElement)typeReference.Element)}){propertyName.ToCamelCase().Singularize()});");
                });
            @class.AddMethod("void",
                $"{this.GetDomainEntityInterfaceName()}.Remove{propertyName.ToPascalCase().Singularize()}",
                method =>
                {
                    if (@class.TryGetMetadata<bool>(IsMerged, out var isMerged) && isMerged)
                    {
                        method.AddAttribute($"IntentManaged(Mode.Fully)");
                    }
                    method.WithoutAccessModifier();
                    method.AddParameter(InterfaceTemplate.GetTypeName((IElement)typeReference.Element),
                        $"{propertyName.ToCamelCase().Singularize()}");
                    method.AddStatement(
                        $"{propertyName.ToPascalCase()}.Remove(({GetTypeName((IElement)typeReference.Element)}){propertyName.ToCamelCase().Singularize()});");
                });
        }
    }

    protected void AddInterfaceQualifiedMethod(CSharpClass @class, OperationModel operation)
    {
        @class.AddMethod(InterfaceTemplate.GetOperationReturnType(operation),
            $"{this.GetDomainEntityInterfaceName()}.{operation.Name}", method =>
            {
                if (@class.TryGetMetadata<bool>(IsMerged, out var isMerged) && isMerged)
                {
                    method.AddAttribute($"IntentManaged(Mode.Fully)");
                }
                method.WithoutAccessModifier();
                foreach (var parameter in operation.Parameters)
                {
                    method.AddParameter(InterfaceTemplate.GetOperationTypeName(parameter), parameter.Name);
                }

                method.AddStatement(
                    $"{(operation.ReturnType != null ? "return " : string.Empty)}{operation.Name}({string.Join(", ", operation.Parameters.Select(x => $"{CastArgumentIfNecessary(x.TypeReference, x.Name)}"))});");
            });
    }

    protected string GetOperationReturnType(OperationModel o)
    {
        if (o.TypeReference.Element == null)
        {
            return o.IsAsync() ? "Task" : "void";
        }
        return o.IsAsync() ? $"Task<{GetTypeName(o.TypeReference, "IEnumerable<{0}>")}>" : GetTypeName(o.TypeReference, "IEnumerable<{0}>");
    }

    protected string GetOperationTypeName(IHasTypeReference hasTypeReference)
    {
        return GetOperationTypeName(hasTypeReference.TypeReference);
    }

    protected string GetOperationTypeName(ITypeReference type)
    {
        return GetTypeName(type, "IEnumerable<{0}>"); // fall back on normal type resolution.
    }

    private string CastArgumentIfNecessary(ITypeReference typeReference, string argument)
    {
        var interfaceType = InterfaceTemplate.GetTypeInfo(typeReference);
        if (!interfaceType.Equals(GetTypeInfo(typeReference)))
        {
            if (interfaceType.IsCollection)
            {
                return $"{argument}.{UseType("System.Linq.Cast")}<{GetTypeName((IElement)typeReference.Element)}>().ToList()";
            }
            return $"({GetTypeName((IElement)typeReference.Element)}) {argument}";
        }

        return string.Empty;
    }
}