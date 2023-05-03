using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Entities.Templates.CollectionWrapper;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Modules.Entities.Templates;

public abstract class DomainEntityStateTemplateBase : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
{
    private DomainEntityInterfaceTemplate _interfaceTemplate;
    protected DomainEntityInterfaceTemplate InterfaceTemplate => _interfaceTemplate ??= GetTemplate<DomainEntityInterfaceTemplate>(DomainEntityInterfaceTemplate.TemplateId, Model);
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
            AddProperty(@class, attribute.Name, attribute.TypeReference, attribute, attribute.InternalElement);

            if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters() &&
                !GetTypeName(attribute).Equals(InterfaceTemplate.GetTypeName(attribute)))
            {
                AddInterfaceQualifiedProperty(@class, attribute.Name, attribute.TypeReference);
            }
        }

        foreach (var associationEnd in Model.AssociatedClasses.Where(x => x.IsNavigable))
        {
            AddProperty(@class, associationEnd.Name, associationEnd, associationEnd, associationEnd.InternalAssociationEnd);

            if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                !GetTypeName(associationEnd).Equals(InterfaceTemplate.GetTypeName(associationEnd)))
            {
                AddInterfaceQualifiedProperty(@class, associationEnd.Name, associationEnd);
            }
        }
    }

    protected void AddProperty(CSharpClass @class, string propertyName, ITypeReference typeReference, IMetadataModel model, IElement element)
    {
        var isPrivateSetterCollection = typeReference.IsCollection &&
                                            ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters();
        var propertyType = isPrivateSetterCollection
            ? GetTypeName(typeReference, UseType("System.Collections.Generic.IReadOnlyCollection<{0}>"))
            : GetTypeName(typeReference);

        @class.AddProperty(propertyType, propertyName.ToPascalCase(), property =>
        {
            property.TryAddXmlDocComments(element);
            property.AddMetadata("model", model);
            if (typeReference.Element.IsClassModel()) // not the most robust. Needed for lazy loading proxies (so should move to EF).
            {
                property.Virtual();
            }

            if (isPrivateSetterCollection)
            {
                var fieldName = propertyName.ToPrivateMemberName();
                @class.AddField(AsListType(), fieldName, field =>
                {
                    field.AddMetadata("model", model);
                    field.WithAssignment($"new {AsListType()}()");
                });

                property.Getter
                    .WithExpressionImplementation($"{fieldName}.AsReadOnly()");
                
                property.Setter
                    .WithExpressionImplementation($"{fieldName} = new {AsListType()}(value)")
                    .Private()
                    ;

                return;
            }

            if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
            {
                property.PrivateSetter();
            }

            if (model is AttributeModel attribute && !string.IsNullOrWhiteSpace(attribute.Value))
            {
                property.WithInitialValue(attribute.Value);
            }
            else if (typeReference.IsCollection)
            {
                property.WithInitialValue($"new {AsListType()}()");
            }

            string AsListType()
            {
                return GetTypeName(typeReference, UseType("System.Collections.Generic.List<{0}>")).Replace("?", "");
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
                property.Getter.WithExpressionImplementation(typeReference.IsCollection && !ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters()
                    ? $"{propertyName.ToPascalCase()}.{UseStaticMethod(CollectionWrapperTemplate.TemplateId, "CreateWrapper")}<{InterfaceTemplate.GetTypeName((IElement)typeReference.Element)}, {GetTypeName((IElement)typeReference.Element)}>()"
                    : $"{propertyName.ToPascalCase()}");
            });

        //if (typeReference.IsCollection &&
        //    !ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
        //{
        //    @class.AddMethod("void",
        //        $"{this.GetDomainEntityInterfaceName()}.Add{propertyName.ToPascalCase().Singularize()}",
        //        method =>
        //        {
        //            if (@class.TryGetMetadata<bool>(IsMerged, out var isMerged) && isMerged)
        //            {
        //                method.AddAttribute($"IntentManaged(Mode.Fully)");
        //            }

        //            method.WithoutAccessModifier();
        //            method.AddParameter(InterfaceTemplate.GetTypeName((IElement)typeReference.Element),
        //                $"{propertyName.ToCamelCase().Singularize()}");
        //            method.AddStatement(
        //                $"{propertyName.ToPascalCase()}.Add(({GetTypeName((IElement)typeReference.Element)}){propertyName.ToCamelCase().Singularize()});");
        //        });
        //    @class.AddMethod("void",
        //        $"{this.GetDomainEntityInterfaceName()}.Remove{propertyName.ToPascalCase().Singularize()}",
        //        method =>
        //        {
        //            if (@class.TryGetMetadata<bool>(IsMerged, out var isMerged) && isMerged)
        //            {
        //                method.AddAttribute($"IntentManaged(Mode.Fully)");
        //            }
        //            method.WithoutAccessModifier();
        //            method.AddParameter(InterfaceTemplate.GetTypeName((IElement)typeReference.Element),
        //                $"{propertyName.ToCamelCase().Singularize()}");
        //            method.AddStatement(
        //                $"{propertyName.ToPascalCase()}.Remove(({GetTypeName((IElement)typeReference.Element)}){propertyName.ToCamelCase().Singularize()});");
        //        });
        //}
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
                    method.AddParameter(InterfaceTemplate.GetOperationTypeName(parameter), parameter.Name,
                        parm => parm.WithDefaultValue(parameter.Value));
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

    private string UseStaticMethod(string templateIdOrRole, string methodName)
    {
        AddUsing(GetTemplate<ICSharpTemplate>(templateIdOrRole).Namespace);
        return methodName;
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