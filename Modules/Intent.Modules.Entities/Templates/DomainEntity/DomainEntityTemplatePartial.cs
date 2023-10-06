using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Common.Types.Api;
using System;
using Intent.Utils;

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

            AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Interface);
            AddTypeSource(TemplateFulfillingRoles.Domain.DomainServices.Interface);
            AddTypeSource(TemplateFulfillingRoles.Domain.DataContract);
            AddTypeSource(DomainEnumTemplate.TemplateId);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddAssemblyAttribute(CSharpIntentTagModeAttribute.Implicit())
                .AddClass(Model.Name, (Action<CSharpClass>)(@class =>
                {
                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    @class.AddMetadata("model", Model);
                    @class.AddMetadata(IsMerged, true);
                    @class.WithPropertiesSeparated();
                    @class.TryAddXmlDocComments(Model.InternalElement);

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
                        // It's important we use the actual CSharpClass here from the other template
                        // and not a string because its metadata is checked by other templates and/or
                        // factory extensions.
                        var baseType = GetTemplate<ICSharpFileBuilderTemplate>(TemplateId, Model.ParentClass.Id).CSharpFile.Classes.First();

                        @class.ExtendsClass(
                            @class: baseType,
                            genericTypeParameters: Model.ParentClassTypeReference.GenericTypeParameters.Select(GetTypeName));
                    }

                    if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces())
                    {
                        var domainEntityInterfaceName = this.GetDomainEntityInterfaceName();
                        if (Model.GenericTypes.Any())
                        {
                            domainEntityInterfaceName = $"{domainEntityInterfaceName}<{string.Join(", ", Model.GenericTypes)}>";
                        }

                        @class.ImplementsInterface(domainEntityInterfaceName);
                    }

                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully())
                        .AddAttribute("DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)")
                        .AddAttribute("DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)");

                    if (!ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour())
                    {
                        AddProperties(@class);
                    }

                    foreach (var ctorModel in Model.Constructors)
                    {
                        @class.AddConstructor(ctor =>
                        {
                            ctor.AddMetadata("model", ctorModel);
                            ctor.TryAddXmlDocComments(ctorModel.InternalElement);
                            foreach (var parameter in ctorModel.Parameters)
                            {
                                ctor.AddParameter(GetOperationTypeName(parameter), parameter.Name.ToCamelCase(), parm => parm.WithDefaultValue(parameter.Value));
                                if (!parameter.InternalElement.IsMapped)
                                {
                                    continue;
                                }

                                var assignmentTarget = parameter.InternalElement.MappedElement.Element.Name.ToPascalCase();
                                if (!parameter.TypeReference.IsCollection)
                                {
                                    ctor.AddStatement($"{assignmentTarget} = {parameter.Name.ToCamelCase()};");
                                    continue;
                                }

                                if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                                {
                                    assignmentTarget = assignmentTarget.ToPrivateMemberName();
                                }

                                var mappedTypeAsList = GetTypeName(
                                        typeReference: parameter.InternalElement.MappedElement.Element.TypeReference,
                                        collectionFormat: UseType("System.Collections.Generic.List<{0}>"))
                                    .Replace("?", string.Empty);

                                ctor.AddStatement($"{assignmentTarget} = new {mappedTypeAsList}({parameter.Name.ToCamelCase()});");
                            }

                            if (ctor.Statements.Any())
                            {
                                ctor.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                            }
                        });
                    }
                    foreach (var operation in Model.Operations)
                    {
                        AddOperation(@class, operation, isOverride: false);
                    }
                    if (!Model.IsAbstract && Model.ParentClass != null)
                    {
                        GetTemplate<ICSharpFileBuilderTemplate>(TemplateId, Model.ParentClass.Id).CSharpFile.OnBuild(file => 
                        {
                            var baseType = file.Classes.First();
                            var abstractMethods = baseType.Methods.Where(m => m.IsAbstract);
                            if (abstractMethods.Any())
                            {
                                foreach (var abstractMethod in abstractMethods)
                                {
                                    var operation = abstractMethod.GetMetadata<OperationModel>("model");
                                    AddOperation(@class, operation, isOverride: true);
                                }
                            }
                        }, 100);
                    }
                }))
                .AfterBuild(file =>
                {
                    foreach (var method in file.Classes.First().Methods)
                    {
                        if (method.IsAbstract)
                            continue;
                        if (!method.Statements.Any())
                        {
                            method.AddStatement(@$"throw new {UseType("System.NotImplementedException")}(""Replace with your implementation..."");");
                        }
                        else
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                        }
                    }
                });
            if (Model.Operations.Any(x => x.IsAsync()))
            {
                AddUsing("System.Threading.Tasks");
            }
        }

        private void AddOperation(CSharpClass @class, OperationModel operation, bool isOverride)
        {
            @class.AddMethod(GetOperationReturnType(operation), operation.Name, method =>
            {
                method.AddMetadata("model", operation);
                method.TryAddXmlDocComments(operation.InternalElement);

                if (isOverride)
                {
                    method.Override();
                }
                else if (operation.IsAbstract)
                {                    
                    method.Abstract();
                    if (!@class.IsAbstract) { @class.Abstract(); }
                }

                var hasImplementation = false;

                foreach (var parameter in operation.Parameters)
                {
                    var parameterName = parameter.Name.ToCamelCase();
                    method.AddParameter(GetOperationTypeName(parameter), parameterName,
                        parm => parm.WithDefaultValue(parameter.Value));
                    if (!parameter.InternalElement.IsMapped)
                    {
                        continue;
                    }

                    if (method.IsAbstract)
                    {
                        Logging.Log.Warning($"Operation {operation.Name} - On {@class.Name} marked as abstract, ignoring mapping implementation.");
                        continue;
                    }

                    var assignmentTarget = parameter.InternalElement.MappedElement.Element.Name.ToPascalCase();
                    if (!parameter.TypeReference.IsCollection)
                    {
                        method.AddStatement($"{assignmentTarget} = {parameterName};");
                        hasImplementation = true;
                        continue;
                    }

                    if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                    {
                        assignmentTarget = assignmentTarget.ToPrivateMemberName();
                        method.AddStatement($"{assignmentTarget}.Clear();");
                        method.AddStatement($"{assignmentTarget}.AddRange({parameterName});");
                        hasImplementation = true;
                        continue;
                    }

                    method.AddStatement($"{assignmentTarget}.Clear();");

                    method.AddStatementBlock($"foreach (var item in {parameterName})", sb => sb
                        .AddStatement($"{assignmentTarget}.Add(item);")
                        .SeparatedFromPrevious());

                    hasImplementation = true;
                }

                if (operation.IsAsync())
                {
                    method.Async();
                    method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", p => p.WithDefaultValue("default"));
                }
            });

            if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                !ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour() &&
                (!InterfaceTemplate.GetOperationTypeName(operation).Equals(this.GetOperationTypeName(operation)) ||
                 !operation.Parameters.Select(InterfaceTemplate.GetOperationTypeName).SequenceEqual(operation.Parameters.Select(this.GetOperationTypeName))))
            {
                AddInterfaceQualifiedMethod(@class, operation);
            }
        }

        private static bool NeedsNullabilityAssignment(IResolvedTypeInfo typeInfo)
        {
            return !(typeInfo.IsPrimitive
                || typeInfo.IsNullable
                || typeInfo.IsCollection
                || (typeInfo.TypeReference != null && typeInfo.TypeReference.Element.IsEnumModel())
                || (typeInfo.TypeReference != null && typeInfo.TypeReference.Element.SpecializationType == "Generic Type"));
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
