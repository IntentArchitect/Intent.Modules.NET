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
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Templates.ServiceContract;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class ServiceContractTemplate : CSharpTemplateBase<ServiceModel, ServiceContractDecorator>,
    ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.Contracts.ServiceContract";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public ServiceContractTemplate(IOutputTarget outputTarget, ServiceModel model) : base(TemplateId, outputTarget,
        model)
    {
        AddTypeSource(DtoModelTemplate.TemplateId).WithCollectionFormatter(CSharpCollectionFormatter.CreateList());
        SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddInterface($"I{Model.Name.RemoveSuffix("RestController", "Controller", "Service")}Service", inter =>
            {
                inter.ImplementsInterfaces(new[] { UseType("System.IDisposable") });
                inter.TryAddXmlDocComments(Model.InternalElement);
                foreach (var operation in Model.Operations)
                {
                    inter.AddMethod(GetOperationReturnType(operation), operation.Name.ToPascalCase(),
                        method =>
                        {
                            method.TryAddXmlDocComments(operation.InternalElement);

                            foreach (var parameterModel in operation.Parameters)
                            {
                                method.AddParameter(GetTypeName(parameterModel.TypeReference), parameterModel.Name, p => p.WithXmlDocComment(parameterModel.InternalElement));
                            }

                            if (operation.IsAsync())
                            {
                                method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", p => p.WithDefaultValue("default"));
                            }
                        });
                }
            });
    }

    private string GetOperationReturnType(OperationModel o)
    {
        if (o.ReturnType == null)
        {
            return o.IsAsync() ? UseType("System.Threading.Tasks.Task") : "void";
        }

        return o.IsAsync() ? $"{UseType("System.Threading.Tasks.Task")}<{GetTypeName(o.ReturnType)}>" : GetTypeName(o.TypeReference);
    }

    [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }

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