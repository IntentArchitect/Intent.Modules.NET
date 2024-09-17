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
        var isBaseClass = Model.ChildClasses.Any();

        foreach (var attribute in Model.Attributes)
        {
            AddProperty(
                @class: @class,
                propertyName: attribute.Name,
                typeReference: attribute.TypeReference,
                model: attribute,
                element: attribute.InternalElement,
                isBaseClass: isBaseClass);

            if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters() &&
                !GetTypeName(attribute).Equals(InterfaceTemplate.GetTypeName(attribute)))
            {
                AddInterfaceQualifiedProperty(
                    @class: @class,
                    propertyName: attribute.Name,
                    typeReference: attribute.TypeReference,
                    model: attribute);
            }
        }

        foreach (var associationEnd in Model.AssociatedClasses.Where(x => x.IsNavigable))
        {
            AddProperty(
                @class: @class,
                propertyName: associationEnd.Name,
                typeReference: associationEnd,
                model: associationEnd,
                element: associationEnd.InternalAssociationEnd,
                isBaseClass: isBaseClass);

            if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                !GetTypeName(associationEnd).Equals(InterfaceTemplate.GetTypeName(associationEnd)))
            {
                AddInterfaceQualifiedProperty(
                    @class: @class,
                    propertyName: associationEnd.Name,
                    typeReference: associationEnd,
                    model: associationEnd);
            }
        }
    }

    protected void AddProperty(CSharpClass @class, string propertyName, ITypeReference typeReference, IMetadataModel model, IElement element, bool isBaseClass)
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
            property.RepresentsModel(model);
            /*
            if (typeReference.Element.IsClassModel()) // not the most robust. Needed for lazy loading proxies (so should move to EF).
            {
                property.Virtual();
            }*/

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
                if (isBaseClass)
                {
                    property.ProtectedSetter();
                }
                else
                {
                    property.PrivateSetter();
                }
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

    protected void AddInterfaceQualifiedProperty(CSharpClass @class, string propertyName, ITypeReference typeReference, IMetadataModel model)
    {
        @class.AddProperty($"{InterfaceTemplate.GetTypeName(typeReference)}", $"{propertyName.ToPascalCase()}", property =>
        {
            property.ExplicitlyImplements(this.GetDomainEntityInterfaceName());
            property.AddMetadata("model", model);

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

                var join = string.Join(", ", operation.Parameters.Select(x => $"{CastArgumentIfNecessary(x.TypeReference, x.Name)}"));

                method.AddStatement(
                    $"{(operation.ReturnType != null ? "return " : string.Empty)}{operation.Name}({join});");
            });
    }

    protected string GetOperationReturnType(OperationModel o)
    {
        if (o.TypeReference.Element == null)
        {
            return o.IsAsync() ? "Task" : "void";
        }
        return o.IsAsync() ? $"Task<{GetTypeName(o.TypeReference, "System.Collections.Generic.IEnumerable<{0}>")}>" : GetTypeName(o.TypeReference, "System.Collections.Generic.IEnumerable<{0}>");
    }

    protected string GetOperationTypeName(IHasTypeReference hasTypeReference)
    {
        return GetOperationTypeName(hasTypeReference.TypeReference);
    }

    protected string GetOperationTypeName(ITypeReference type)
    {
        return GetTypeName(type, "System.Collections.Generic.IEnumerable<{0}>"); // fall back on normal type resolution.
    }

    private string UseStaticMethod(string templateIdOrRole, string methodName)
    {
        AddUsing(GetTemplate<ICSharpTemplate>(templateIdOrRole).Namespace);
        return methodName;
    }

    private string CastArgumentIfNecessary(ITypeReference typeReference, string argumentName)
    {
        var interfaceType = InterfaceTemplate.GetTypeInfo(typeReference);
        if (!string.Equals(interfaceType.ToString(), GetTypeInfo(typeReference).ToString()))
        {
            if (interfaceType.IsCollection)
            {
                return $"{argumentName}.{UseType("System.Linq.Cast")}<{GetTypeName(typeReference, collectionFormat: "{0}")}>().ToList()";
            }

            return $"({GetTypeName(typeReference)}) {argumentName}";
        }

        return argumentName;
    }
}