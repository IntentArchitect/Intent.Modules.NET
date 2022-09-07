using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Entities.Templates.DomainEntity
{
    [IntentManaged(Mode.Ignore, Body = Mode.Merge)]
    public partial class DomainEntityTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate, IHasDecorators<DomainEntityDecoratorBase>, ITemplatePostCreationHook, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Entities.DomainEntity";

        private const string InterfaceTypeContext = "interface";

        private readonly IList<DomainEntityDecoratorBase> _decorators = new List<DomainEntityDecoratorBase>();
        public CSharpFile CSharpFile { get; set; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEntityTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateICollection());
            AddTypeSource(TemplateId);
            AddTypeSource(DomainEnumTemplate.TemplateId);
            AddTypeSource("Domain.ValueObject");

            if (!ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour())
            {
                FulfillsRole("Domain.Entity");
                if (!ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces())
                {
                    FulfillsRole("Domain.Entity.Interface");
                }
            }

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass(Model.Name, @class =>
                {
                    Types.AddTypeSource(CSharpTypeSource.Create(ExecutionContext, DomainEntityInterfaceTemplate.TemplateId, "IEnumerable<{0}>"), contextName: InterfaceTypeContext);

                    if (Model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                    if (ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour())
                    {
                        @class.Partial();
                    }

                    @class.AddAttribute("IntentManaged(Mode.Merge, Signature = Mode.Fully)")
                        .AddAttribute("DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)")
                        .AddAttribute("DefaultIntentManaged(Mode.Fully, Body = Mode.Ignore, Targets = Targets.Methods, AccessModifiers = AccessModifiers.Public)");

                    if (!ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour())
                    {
                        if (Model.ParentClass != null)
                        {
                            @class.ExtendsClass(GetTemplate<ICSharpFileBuilderTemplate>(TemplateId, Model.ParentClass.Id).CSharpFile.Classes.First());
                        }
                        if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces())
                        {
                            @class.ImplementsInterface(this.GetDomainEntityInterfaceName());
                        }

                        foreach (var attribute in Model.Attributes)
                        {
                            @class.AddProperty(GetTypeName(attribute), attribute.Name.ToPascalCase(), property =>
                            {
                                property.AddMetadata("model", attribute);
                                if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                                {
                                    property.PrivateSetter();
                                }
                            });

                            if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                                ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters() &&
                                attribute.TypeReference.IsCollection)
                            {
                                @class.AddProperty($"{GetTypeName(attribute, "IEnumerable<{0}>")}", $"{this.GetDomainEntityInterfaceName()}.{attribute.Name.ToPascalCase()}", property =>
                                {
                                    property.ReadOnly();
                                    property.WithoutAccessModifier();
                                    property.Getter.WithExpressionImplementation(attribute.Name.ToPascalCase());
                                });
                            }
                        }

                        foreach (var associationEnd in Model.AssociatedClasses.Where(x => x.IsNavigable))
                        {
                            @class.AddProperty(GetTypeName(associationEnd), associationEnd.Name.ToPascalCase(), property =>
                            {
                                property.AddMetadata("model", associationEnd);
                                property.Virtual();
                                if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                                {
                                    property.PrivateSetter();
                                }
                            });

                            if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                                ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters() &&
                                !GetTypeName(associationEnd).Equals(TryGetInterfaceTypeName(associationEnd)))
                            {
                                @class.AddProperty($"{TryGetInterfaceTypeName(associationEnd)}", $"{this.GetDomainEntityInterfaceName()}.{associationEnd.Name.ToPascalCase()}", property =>
                                {
                                    property.ReadOnly();
                                    property.WithoutAccessModifier();
                                    property.Getter.WithExpressionImplementation(associationEnd.Name.ToPascalCase());
                                });
                            }
                        }
                    }

                    foreach (var operation in Model.Constructors)
                    {
                        @class.AddConstructor(ctor =>
                        {
                            foreach (var parameter in operation.Parameters)
                            {
                                ctor.AddParameter(GetTypeName(parameter), parameter.Name.ToCamelCase());
                                if (parameter.InternalElement.IsMapped)
                                {
                                    ctor.AddStatement($"{parameter.InternalElement.MappedElement.Element.Name} = {parameter.Name.ToCamelCase()};");
                                }
                            }
                        });
                    }
                    foreach (var operation in Model.Operations)
                    {
                        @class.AddMethod(GetOperationTypeName(operation), operation.Name, method =>
                        {
                            foreach (var parameter in operation.Parameters)
                            {
                                method.AddParameter(GetOperationTypeName(parameter), parameter.Name);
                            }
                            method.AddStatement(@"throw new NotImplementedException(""Replace with your implementation..."");");
                        });

                        if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                            ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters() &&
                            (!TryGetInterfaceTypeName(operation).Equals(GetOperationTypeName(operation)) ||
                             !Enumerable.SequenceEqual(operation.Parameters.Select(TryGetInterfaceTypeName), operation.Parameters.Select(GetOperationTypeName))))
                        {
                            @class.AddMethod(TryGetInterfaceTypeName(operation), $"{this.GetDomainEntityInterfaceName()}.{operation.Name}", method =>
                            {
                                method.AddAttribute("IntentManaged(Mode.Fully)");
                                method.WithoutAccessModifier();
                                foreach (var parameter in operation.Parameters)
                                {
                                    method.AddParameter(TryGetInterfaceTypeName(parameter), parameter.Name);
                                }

                                method.AddStatement($"{(operation.ReturnType != null ? "return " : string.Empty)}{operation.Name}({string.Join(", ", operation.Parameters.Select(x => $"{CastArgumentIfNecessary(x)}"))});");
                            });
                        }
                    }
                });
            if (Model.Operations.Any(x => x.IsAsync()))
            {
                AddUsing("System.Threading.Tasks");
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Ignore, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public void AddDecorator(DomainEntityDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<DomainEntityDecoratorBase> GetDecorators()
        {
            return _decorators;
        }

        public string Constructors(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.Constructors(@class));
        }

        public string GetParametersDefinition(OperationModel operation)
        {
            return string.Join(", ", operation.Parameters.Select(x => $"{GetTypeName(x.Type)} {x.Name.ToCamelCase()}"));
        }

        public string EmitOperationReturnType(OperationModel o)
        {
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? "async Task" : "void";
            }
            return o.IsAsync() ? $"async Task<{GetTypeName(o.ReturnType)}>" : GetTypeName(o.ReturnType);
        }

        private string GetOperationTypeName(IHasTypeReference hasTypeReference)
        {
            return GetOperationTypeName(hasTypeReference.TypeReference);
        }

        private string GetOperationTypeName(ITypeReference type)
        {
            return GetTypeName(type, "IEnumerable<{0}>"); // fall back on normal type resolution.
        }

        private string TryGetInterfaceTypeName(IHasTypeReference hasTypeReference)
        {
            return TryGetInterfaceTypeName(hasTypeReference.TypeReference);
        }

        private string TryGetInterfaceTypeName(ITypeReference type)
        {
            var interfaceType = Types.InContext(InterfaceTypeContext).Get(type);
            if (interfaceType.Template != null || interfaceType.GenericTypeParameters.Any(x => x.Template != null)) // found interface type
            {
                return UseType(interfaceType);
            }

            return GetOperationTypeName(type); // fall back on normal type resolution.
        }

        private string CastArgumentIfNecessary(ParameterModel parameter)
        {
            var interfaceType = Types.InContext(InterfaceTypeContext).Get(parameter.TypeReference);
            if (interfaceType.Template != null || interfaceType.GenericTypeParameters.Any(x => x.Template != null)) // found interface type
            {
                if (interfaceType.Name.Equals("IEnumerable"))
                {
                    return $"{parameter.Name}.Cast<{GetTypeName((IElement)parameter.TypeReference.Element)}>()";
                }
                return $"({GetTypeName((IElement)parameter.TypeReference.Element)}) {parameter.Name}";
            }

            return string.Empty;
        }
    }
}
