using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Entities.Templates.DomainEntityInterface
{
    [IntentManaged(Mode.Ignore, Body = Mode.Merge)]
    public partial class DomainEntityInterfaceTemplate : CSharpTemplateBase<ClassModel>, ITemplate, ITemplatePostCreationHook, IDeclareUsings, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Entities.DomainEntityInterface";
        private readonly IMetadataManager _metadataManager;
        public const string Identifier = "Intent.Entities.DomainEntityInterface";
        public CSharpFile CSharpFile { get; set; }

        //private readonly IList<DomainEntityInterfaceDecoratorBase> _decorators = new List<DomainEntityInterfaceDecoratorBase>();

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public DomainEntityInterfaceTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            _metadataManager = ExecutionContext.MetadataManager;
            if (!ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
            {
                SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateICollection());
            }
            AddTypeSource(TemplateId);
            AddTypeSource(DomainEnumTemplate.TemplateId);
            AddTypeSource("Domain.ValueObject");
            AddTypeSource(TemplateRoles.Domain.DataContract);
        }

        public override void OnCreated()
        {
            if (Model.Operations.Any(x => x.IsAsync()))
            {
                AddUsing("System.Threading.Tasks");
            }

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"I{Model.Name}", @interface =>
                {
                    foreach (var genericType in Model.GenericTypes)
                    {
                        @interface.AddGenericParameter(genericType);
                    }

                    @interface.AddMetadata("model", Model);
                    @interface.RepresentsModel(Model);
                    @interface.WithMembersSeparated();

                    if (Model.ParentClass != null)
                    {
                        var baseType = this.GetDomainEntityInterfaceName(Model.ParentClass);
                        if (Model.ParentClassTypeReference.GenericTypeParameters.Any())
                        {
                            baseType = $"{baseType}<{string.Join(", ", Model.ParentClassTypeReference.GenericTypeParameters.Select(GetTypeName))}>";
                        }

                        @interface.ExtendsInterface(baseType);
                    }

                    foreach (var attribute in Model.Attributes)
                    {
                        @interface.AddProperty(GetTypeName(attribute), attribute.Name.ToPascalCase(), property =>
                        {
                            property.AddMetadata("model", attribute);
                            property.RepresentsModel(attribute);
                            if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                            {
                                property.ReadOnly();
                            }
                        });
                    }

                    foreach (var associationEnd in Model.AssociatedClasses.Where(x => x.IsNavigable))
                    {
                        @interface.AddProperty(GetTypeName(associationEnd), associationEnd.Name.ToPascalCase(), property =>
                        {
                            property.AddMetadata("model", associationEnd);
                            property.RepresentsModel(associationEnd);
                            //property.Virtual();
                            if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                            {
                                property.ReadOnly();
                            }
                        });
                        //if (associationEnd.IsCollection &&
                        //    associationEnd.Class != null &&
                        //    !ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                        //{
                        //    @interface.AddMethod("void", $"Add{associationEnd.Name.ToPascalCase().Singularize()}",
                        //        method =>
                        //        {
                        //            method.AddParameter(GetTypeName((IElement)associationEnd.Element), $"{associationEnd.Name.ToCamelCase().Singularize()}");
                        //        });
                        //    @interface.AddMethod("void", $"Remove{associationEnd.Name.ToPascalCase().Singularize()}",
                        //        method =>
                        //        {
                        //            method.AddParameter(GetTypeName((IElement)associationEnd.Element), $"{associationEnd.Name.ToCamelCase().Singularize()}");
                        //        });
                        //}
                    }

                    foreach (var operation in Model.Operations)
                    {
                        @interface.AddMethod(GetOperationReturnType(operation), operation.Name, method =>
                        {
                            foreach (var parameter in operation.Parameters)
                            {
                                method.AddParameter(GetOperationTypeName(parameter), parameter.Name, parm => parm.WithDefaultValue(parameter.Value));
                            }

                            if (operation.IsAsync())
                            {
                                method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", p => p.WithDefaultValue("default"));
                            }
                        });
                    }
                });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Ignore, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        //public void AddDecorator(DomainEntityInterfaceDecoratorBase decorator)
        //{
        //    _decorators.Add(decorator);
        //}

        //public IEnumerable<DomainEntityInterfaceDecoratorBase> GetDecorators()
        //{
        //    return _decorators.OrderBy(x => x.Priority);
        //}

        //public string GetInterfaces(ClassModel @class)
        //{
        //    var interfaces = GetDecorators().SelectMany(x => x.GetInterfaces(@class)).Distinct().ToList();
        //    if (Model.GetStereotypeProperty("Base Type", "Has Interface", false) && GetBaseTypeInterface() != null)
        //    {
        //        interfaces.Insert(0, GetBaseTypeInterface());
        //    }

        //    return string.Join(", ", interfaces);
        //}

        //public string InterfaceAnnotations(ClassModel @class)
        //{
        //    return GetDecorators().Aggregate(x => x.InterfaceAnnotations(@class));
        //}

        //public string BeforeProperties(ClassModel @class)
        //{
        //    return GetDecorators().Aggregate(x => x.BeforeProperties(@class));
        //}

        //public string PropertyBefore(AttributeModel attribute)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertyBefore(attribute));
        //}

        //public string PropertyAnnotations(AttributeModel attribute)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertyAnnotations(attribute));
        //}

        //public string PropertyBefore(AssociationEndModel associationEnd)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertyBefore(associationEnd));
        //}

        //public string PropertyAnnotations(AssociationEndModel associationEnd)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertyAnnotations(associationEnd));
        //}

        //public string AttributeAccessors(AttributeModel attribute)
        //{
        //    return GetDecorators().Select(x => x.AttributeAccessors(attribute)).FirstOrDefault(x => x != null) ?? "get; set;";
        //}

        //public string AssociationAccessors(AssociationEndModel associationEnd)
        //{
        //    return GetDecorators().Select(x => x.AssociationAccessors(associationEnd)).FirstOrDefault(x => x != null) ?? "get; set;";
        //}

        //public bool CanWriteDefaultAttribute(AttributeModel attribute)
        //{
        //    return GetDecorators().All(x => x.CanWriteDefaultAttribute(attribute));
        //}

        //public bool CanWriteDefaultAssociation(AssociationEndModel association)
        //{
        //    return GetDecorators().All(x => x.CanWriteDefaultAssociation(association));
        //}

        //public bool CanWriteDefaultOperation(OperationModel operation)
        //{
        //    return GetDecorators().All(x => x.CanWriteDefaultOperation(operation));
        //}


        public string GetOperationReturnType(OperationModel o)
        {
            if (o.TypeReference.Element == null)
            {
                return o.IsAsync() ? "Task" : "void";
            }
            return o.IsAsync() ? $"Task<{GetTypeName(o.TypeReference, "IEnumerable<{0}>")}>" : GetTypeName(o.TypeReference, "IEnumerable<{0}>");
        }

        public string GetOperationTypeName(IHasTypeReference hasTypeReference)
        {
            return GetOperationTypeName(hasTypeReference.TypeReference);
        }

        public string GetOperationTypeName(ITypeReference type)
        {
            return GetTypeName(type, "IEnumerable<{0}>"); // fall back on normal type resolution.
        }
    }
}
