using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Templates.ServiceContract;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class ServiceContractTemplate : CSharpTemplateBase<ServiceModel, ServiceContractDecorator>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.Contracts.ServiceContract";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public ServiceContractTemplate(IOutputTarget outputTarget, ServiceModel model) : base(TemplateId, outputTarget, model)
    {
        SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());

        AddTypeSource(TemplateRoles.Application.Contracts.Enum);
        AddTypeSource(DtoModelTemplate.TemplateId).WithCollectionFormatter(CSharpCollectionFormatter.CreateList());
        AddTypeSource(TemplateRoles.Domain.Enum);
        AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
        AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
            .AddInterface($"I{Model.Name.RemoveSuffix("RestController", "Controller", "Service")}Service", @interface =>
            {
                @interface.RepresentsModel(Model);
                @interface.ImplementsInterfaces([UseType("System.IDisposable")]);
                @interface.TryAddXmlDocComments(Model.InternalElement);
                foreach (var operation in Model.Operations)
                {
                    @interface.AddMethod(operation, method =>
                    {
                        foreach (var genericType in operation.GenericTypes)
                        {
                            @method.AddGenericParameter(genericType);
                        }

                        method.TryAddXmlDocComments(operation.InternalElement);

                        foreach (var parameterModel in operation.Parameters)
                        {
                            method.AddParameter(GetTypeName(parameterModel.TypeReference), parameterModel.Name, param =>
                            {
                                // If you change anything here, please check also: WorkaroundForGetTypeNameIssue()
                                param.AddMetadata("model", parameterModel);
                                param.WithXmlDocComment(parameterModel.InternalElement);
                            });
                        }

                        if (operation.IsAsync())
                        {
                            method.Async();
                            method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", p => p.WithDefaultValue("default"));
                        }
                    });
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
        var priInterface = file.Interfaces.First();
        foreach (var method in priInterface.Methods)
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
}