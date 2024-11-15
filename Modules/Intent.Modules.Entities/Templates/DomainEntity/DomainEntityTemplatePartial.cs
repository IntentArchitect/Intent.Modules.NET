using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

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
                FulfillsRole(TemplateRoles.Domain.Entity.Primary);
                if (!ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces())
                {
                    FulfillsRole(TemplateRoles.Domain.Entity.Interface);
                }
            }
            else
            {
                FulfillsRole(TemplateRoles.Domain.Entity.Behaviour);
            }

            if (Model.Operations.Any(x => x.IsAsync()))
            {
                AddUsing("System.Threading.Tasks");
            }

            AddTypeSource(TemplateRoles.Domain.ValueObject);
            AddTypeSource(TemplateRoles.Domain.Entity.Interface);
            AddTypeSource(TemplateRoles.Domain.DomainServices.Interface);
            AddTypeSource(TemplateRoles.Domain.DataContract);
            AddTypeSource(DomainEnumTemplate.TemplateId);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
                .AddClass(Model.Name, @class =>
                {
                    @class.RepresentsModel(Model);
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
                        .AddAttribute("DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Merge, AccessModifiers = AccessModifiers.Public)");

                    if (!ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour())
                    {
                        AddProperties(@class);
                    }

                    foreach (var ctorModel in Model.Constructors)
                    {
                        @class.AddConstructor(ctor =>
                        {
                            ctor.AddMetadata("model", ctorModel);
                            ctor.RepresentsModel(ctorModel);
                            ctor.TryAddXmlDocComments(ctorModel.InternalElement);

                            foreach (var parameter in ctorModel.Parameters)
                            {
                                ctor.AddParameter(GetOperationTypeName(parameter), parameter.Name.ToCamelCase(), param =>
                                {
                                    param.WithDefaultValue(parameter.Value).RepresentsModel(parameter);
                                });
                            }
                            if (ctorModel.InternalElement.Mappings.Any())
                            {
                                ctor.AddStatements(GetMappingImplementation(ctorModel.InternalElement.Mappings.Single(), ctor));
                            }
                            else
                            {
                                // LEGACY MAPPING:
                                foreach (var parameter in ctorModel.Parameters.Where(x => x.InternalElement.IsMapped))
                                {
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
                            var abstractMethods = baseType.Methods.Where(m => m.IsAbstract).ToArray();
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
                })
                .AfterBuild(file =>
                {
                    foreach (var method in file.Classes.First().Methods)
                    {
                        if (method.IsAbstract)
                            continue;

                        if (!method.Statements.Any())
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                            method.AddStatement("// [IntentFully]");
                            method.AddStatement($"// TODO: Implement {method.Name} ({file.Classes.First().Name}) functionality");
                            method.AddStatement(@$"throw new {UseType("System.NotImplementedException")}(""Replace with your implementation..."");");
                        }
                    }
                });

            if (ExecutionContext.Settings.GetDomainSettings().UseImplicitTagModeForEntities())
            {
                CSharpFile.IntentTagModeImplicit();
            }
        }

        private void AddOperation(CSharpClass @class, OperationModel operation, bool isOverride)
        {
            @class.AddMethod(GetOperationReturnType(operation), operation, method =>
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
                else if (operation.IsStatic)
                {
                    method.Static();
                }

                foreach (var parameter in operation.Parameters)
                {
                    method.AddParameter(GetOperationTypeName(parameter), parameter,
                        param => param.WithDefaultValue(parameter.Value));
                }

                if (operation.InternalElement.Mappings.Any())
                {
                    if (method.IsAbstract)
                    {
                        Logging.Log.Warning($"Operation {operation.Name} - On {@class.Name} marked as abstract, ignoring mapping implementation.");
                    }
                    method.AddStatements(GetMappingImplementation(operation.InternalElement.Mappings.Single(), method));
                }
                else
                {
                    // LEGACY MAPPING:
                    foreach (var parameter in operation.Parameters.Where(x => x.InternalElement.IsMapped))
                    {
                        var parameterName = parameter.Name.ToParameterName();

                        if (method.IsAbstract)
                        {
                            Logging.Log.Warning($"Operation {operation.Name} - On {@class.Name} marked as abstract, ignoring mapping implementation.");
                            continue;
                        }

                        var assignmentTarget = parameter.InternalElement.MappedElement.Element.Name.ToPascalCase();
                        if (!parameter.TypeReference.IsCollection)
                        {
                            method.AddStatement($"{assignmentTarget} = {method.GetReferenceForModel(parameter).Name};");
                            continue;
                        }

                        if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                        {
                            assignmentTarget = assignmentTarget.ToPrivateMemberName();
                            method.AddStatement($"{assignmentTarget}.Clear();");
                            method.AddStatement($"{assignmentTarget}.AddRange({parameterName});");
                            continue;
                        }

                        method.AddStatement($"{assignmentTarget}.Clear();");

                        method.AddStatementBlock($"foreach (var item in {parameterName})", sb => sb
                            .AddStatement($"{assignmentTarget}.Add(item);")
                            .SeparatedFromPrevious());

                    }
                }

                if (operation.IsAsync())
                {
                    method.Async();
                    method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", p => p.WithDefaultValue("default"));
                }
            });

            if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                !ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour() &&
                (!InterfaceTemplate.GetOperationTypeName(operation).Equals(GetOperationTypeName(operation)) ||
                 !operation.Parameters.Select(InterfaceTemplate.GetOperationTypeName).SequenceEqual(operation.Parameters.Select(GetOperationTypeName))))
            {
                AddInterfaceQualifiedMethod(@class, operation);
            }
        }

        private IEnumerable<CSharpStatement> GetMappingImplementation(IElementToElementMapping mapping, ICSharpCodeContext codeContext)
        {
            var mappingManager = new CSharpClassMappingManager(this);
            mappingManager.SetFromReplacement(mapping.SourceElement, null);
            mappingManager.SetToReplacement(mapping.TargetElement, null);
            var generalization = mapping.TargetElement.AsClassModel()?.Generalizations().SingleOrDefault();
            if (generalization != null)
            {
                mappingManager.SetFromReplacement(generalization, null);
                mappingManager.SetToReplacement(generalization, null);
            }
            mappingManager.AddMappingResolver(new CollectionPropertyResolver(this));

            return mappingManager.GenerateUpdateStatements(mapping, codeContext)
                .Select(s =>
                {
                    if (s is CSharpAssignmentStatement assignment)
                    {
                        assignment.WithSemicolon();
                    }
                    return s;
                })
                .ToList();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
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

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(
                new TemplateMetadata(Id, "2.0"),
                new V00ToV02Migration(),
                new V01ToV02Migration());
        }

        private class V00ToV02Migration : ITemplateMigration
        {
            public string Execute(string currentText)
            {
                return currentText
                    .ReplaceLineEndings()
                    .Replace($"[assembly: IntentTagModeImplicit]{Environment.NewLine}", string.Empty);
            }

            public TemplateMigrationCriteria Criteria => TemplateMigrationCriteria.UnversionedUpgrade(2);
        }

        private class V01ToV02Migration : ITemplateMigration
        {
            public string Execute(string currentText)
            {
                return currentText
                    .ReplaceLineEndings()
                    .Replace($"[assembly: IntentTagModeImplicit]{Environment.NewLine}", string.Empty);
            }

            public TemplateMigrationCriteria Criteria => TemplateMigrationCriteria.Upgrade(1, 2);
        }
    }

    internal class CollectionPropertyResolver : IMappingTypeResolver
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public CollectionPropertyResolver(ICSharpFileBuilderTemplate template)
        {
            _template = template;
        }

        public ICSharpMapping ResolveMappings(MappingModel mappingModel)
        {
            if (mappingModel.Model.IsAssociationEndModel() && mappingModel.Model.TypeReference.IsCollection)
            {
                return new CollectionPropertyMapper(mappingModel, _template);
            }
            return null;
        }
    }

    internal class CollectionPropertyMapper : CSharpMappingBase
    {
        public CollectionPropertyMapper(MappingModel mappingModel, ICSharpFileBuilderTemplate template) : base(mappingModel, template) { }

        public override IEnumerable<CSharpStatement> GetMappingStatements()
        {

            if (Template.ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
            {
                yield return new CSharpInvocationStatement(GetTargetStatement(), "Clear");
                yield return new CSharpInvocationStatement(GetTargetStatement(), "AddRange").AddArgument(GetSourceStatement());
                yield break;
            }

            yield return new CSharpInvocationStatement(GetTargetStatement(), "Clear");
            yield return new CSharpForEachStatement("item", GetSourceStatement().ToString())
                .AddStatement(new CSharpInvocationStatement(GetTargetStatement(), "Add").AddArgument("item"))
                .SeparatedFromPrevious();
        }

        public override CSharpStatement GetTargetStatement()
        {
            if (Template.ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
            {
                return GetTargetPathText().ToPrivateMemberName();
            }

            return base.GetTargetStatement();
        }
    }
}
