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
using Intent.Modules.Constants;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Entities.Templates.DomainEntity
{
    [IntentManaged(Mode.Ignore, Body = Mode.Merge)]
    public partial class DomainEntityTemplate : DomainEntityStateTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Entities.DomainEntity";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEntityTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            if (!ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour())
            {
                FulfillsRole(TemplateFulfillingRoles.Domain.Entity.Primary);
                if (!ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces())
                {
                    FulfillsRole(TemplateFulfillingRoles.Domain.Entity.Interface);
                }
            }

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass(Model.Name, @class =>
                {
                    @class.AddMetadata("model", Model);
                    @class.AddMetadata(IsMerged, true);

                    if (Model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                    if (ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour())
                    {
                        @class.Partial();
                    }

                    if (Model.ParentClass != null)
                    {
                        @class.ExtendsClass(GetTemplate<ICSharpFileBuilderTemplate>(TemplateId, Model.ParentClass.Id).CSharpFile.Classes.First());
                    }
                    if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces())
                    {
                        @class.ImplementsInterface(this.GetDomainEntityInterfaceName());
                    }

                    @class.AddAttribute("IntentManaged(Mode.Merge, Signature = Mode.Fully)")
                        .AddAttribute("DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)")
                        .AddAttribute("DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)");

                    if (!ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour())
                    {
                        AddProperties(@class);
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
                        @class.AddMethod(GetOperationReturnType(operation), operation.Name, method =>
                        {
                            foreach (var parameter in operation.Parameters)
                            {
                                method.AddParameter(GetOperationTypeName(parameter), parameter.Name);
                            }
                            method.AddStatement(@"throw new NotImplementedException(""Replace with your implementation..."");");
                        });

                        if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                            !ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour() &&
                            (!InterfaceTemplate.GetOperationTypeName(operation).Equals(this.GetOperationTypeName(operation)) ||
                             !operation.Parameters.Select(InterfaceTemplate.GetOperationTypeName).SequenceEqual(operation.Parameters.Select(this.GetOperationTypeName))))
                        {
                            AddInterfaceQualifiedMethod(@class, operation);
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

        public string GetOperationReturnType(OperationModel o)
        {
            if (o.TypeReference.Element == null)
            {
                return o.IsAsync() ? "Task" : "void";
            }
            return o.IsAsync() ? $"Task<{GetTypeName(o.TypeReference, "IEnumerable<{0}>")}>" : GetTypeName(o.TypeReference, "IEnumerable<{0}>");
        }

        private string GetOperationTypeName(IHasTypeReference hasTypeReference)
        {
            return GetOperationTypeName(hasTypeReference.TypeReference);
        }

        private string GetOperationTypeName(ITypeReference type)
        {
            return GetTypeName(type, "IEnumerable<{0}>"); // fall back on normal type resolution.
        }

        private DomainEntityInterfaceTemplate _interfaceTemplate;
        private DomainEntityInterfaceTemplate InterfaceTemplate => _interfaceTemplate ?? GetTemplate<DomainEntityInterfaceTemplate>(DomainEntityInterfaceTemplate.TemplateId, Model);


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
}
