using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
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

        private readonly IList<DomainEntityDecoratorBase> _decorators = new List<DomainEntityDecoratorBase>();
        public CSharpFile CSharpFile { get; set; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEntityTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateICollection());
            AddTypeSource(TemplateId);
            AddTypeSource(DomainEntityInterfaceTemplate.Identifier);
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
                        @class.AddMethod(GetTypeName(operation), operation.Name, method =>
                        {
                            foreach (var parameter in operation.Parameters)
                            {
                                method.AddParameter(GetTypeName(parameter), parameter.Name);
                            }
                            method.AddStatement(@"throw new NotImplementedException(""Replace with your implementation..."");");
                        });
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
    }
}
