using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using ModelHasFolderTemplateExtensions = Intent.Modules.Common.CSharp.Templates.ModelHasFolderTemplateExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
    public partial class ServiceImplementationTemplate : CSharpTemplateBase<ServiceModel, ServiceImplementationDecorator>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.ServiceImplementations.ServiceImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceImplementationTemplate(IOutputTarget outputTarget, ServiceModel model)
            : base(TemplateId, outputTarget, model)
        {
            SetDefaultTypeCollectionFormat("List<{0}>");

            AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");
            AddTypeSource(TemplateRoles.Application.Command);
            AddTypeSource(TemplateRoles.Application.Query);
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            CSharpFile = new CSharpFile(this.GetNamespace(), ModelHasFolderTemplateExtensions.GetFolderPath(this))
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddClass($"{Model.Name.RemoveSuffix("RestController", "Controller", "Service")}Service")
                .OnBuild(file =>
                {
                    var priClass = file.Classes.First();
                    priClass.AddMetadata("model", Model);
                    priClass.TryAddXmlDocComments(Model.InternalElement);
                    priClass.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    priClass.ImplementsInterface(GetServiceInterfaceName());
                    priClass.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });
                    foreach (var operation in Model.Operations)
                    {
                        priClass.AddMethod(GetOperationReturnType(operation), operation.Name.ToPascalCase(), method =>
                        {
                            foreach (var genericType in operation.GenericTypes)
                            {
                                @method.AddGenericParameter(genericType);
                            }

                            method.TryAddXmlDocComments(operation.InternalElement);
                            method.AddMetadata("model", operation);
                            method.RepresentsModel(operation);
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());

                            foreach (var parameter in operation.Parameters)
                            {
                                method.AddParameter(GetTypeName(parameter.TypeReference), parameter.Name, param =>
                                {
                                    // If you change anything here, please check also: WorkaroundForGetTypeNameIssue()
                                    param.AddMetadata("model", parameter);
                                    param.WithDefaultValue(parameter.Value)
                                        .WithXmlDocComment(parameter.InternalElement);
                                });
                            }

                            if (operation.IsAsync())
                            {
                                method
                                    .Async()
                                    .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", p => p.WithDefaultValue("default"));
                            }

                            method.AddStatement($"// TODO: Implement {method.Name} ({priClass.Name}) functionality");
                            method.AddStatement($@"throw new NotImplementedException(""Write your implementation for this service here..."");");
                        });
                    }
                    priClass.AddMethod("void", "Dispose");
                })
                .AfterBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var ctor = priClass.Constructors.First();
                    var dependencies = GetConstructorDependencies();
                    foreach (var dependency in dependencies)
                    {
                        ctor.AddParameter(dependency.ParameterType, dependency.ParameterName, parm => parm.IntroduceReadonlyField());
                    }

                    foreach (var method in priClass.Methods)
                    {
                        if (!method.TryGetMetadata<OperationModel>("model", out var operation))
                        {
                            continue;
                        }
                        var output = GetDecorators().Aggregate(x => x.GetDecoratedImplementation(operation));
                        if (!string.IsNullOrWhiteSpace(output))
                        {
                            method.Statements.Clear();
                            method.AddStatements(output);
                        }
                    }
                })
                .AfterBuild(WorkaroundForGetTypeNameIssue, 1000);
        }

        // Due to the nature of how GetTypeName resolves namespaces
        // there are cases where ambiguous references still exist
        // and causes compilation errors, this forces to re-evaluate
        // a lot of types in this template. For example when a service
        // is calling a proxy with the same Dto names on both sides.
        private void WorkaroundForGetTypeNameIssue(CSharpFile file)
        {
            var priClass = file.Classes.First();
            priClass.Interfaces[0] = GetServiceInterfaceName();
            foreach (var method in priClass.Methods)
            {
                var parameterTypesToReplace = method.Parameters
                    .Select((param, index) => new { Param = param, Index = index })
                    .Where(p => p.Param.HasMetadata("model"))
                    .ToArray();
                foreach (var entry in parameterTypesToReplace)
                {
                    var paramModel = entry.Param.GetMetadata<IElementWrapper>("model");
                    var param = new CSharpParameter(GetTypeName(paramModel.InternalElement.TypeReference), entry.Param.Name, method);
                    param.WithDefaultValue(entry.Param.DefaultValue);
                    param.WithXmlDocComment(entry.Param.XmlDocComment);
                    foreach (var attribute in entry.Param.Attributes)
                    {
                        param.Attributes.Add(attribute);
                    }
                    method.Parameters[entry.Index] = param;
                }
            }
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }


        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Application")
                .ForInterface(GetTemplate<IClassProvider>(ServiceContractTemplate.TemplateId, Model))
                .WithPriority(100));
        }

        private string GetOperationReturnType(OperationModel o)
        {
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? UseType("System.Threading.Tasks.Task") : "void";
            }

            return o.IsAsync() ? $"{UseType("System.Threading.Tasks.Task")}<{GetTypeName(o.ReturnType)}>" : GetTypeName(o.TypeReference);
        }

        public string GetServiceInterfaceName()
        {
            return GetTypeName(ServiceContractTemplate.TemplateId, Model.Id);
        }

        private IReadOnlyCollection<ConstructorParameter> GetConstructorDependencies()
        {
            var parameters = GetDecorators()
                .SelectMany(s => s.GetConstructorDependencies())
                .Distinct()
                .ToArray();
            return parameters;
        }
    }
}
