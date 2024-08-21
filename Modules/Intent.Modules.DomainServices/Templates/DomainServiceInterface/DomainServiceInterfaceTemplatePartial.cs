using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.DomainServices.Templates.DomainServiceInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DomainServiceInterfaceTemplate : CSharpTemplateBase<DomainServiceModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.DomainServices.DomainServiceInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainServiceInterfaceTemplate(IOutputTarget outputTarget, DomainServiceModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateRoles.Domain.Entity.Interface);
            AddTypeSource(TemplateRoles.Domain.ValueObject);
            AddTypeSource(TemplateRoles.Domain.DataContract);
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Domain.DomainServices.Interface);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
                .AddInterface($"I{Model.Name}", @interface =>
                {
                    @interface.TryAddXmlDocComments(Model.InternalElement);
                    @interface.RepresentsModel(Model);
                    foreach (var operation in Model.Operations)
                    {
                        var isAsync = operation.Name.EndsWith("Async", System.StringComparison.OrdinalIgnoreCase);

                        @interface.AddMethod(operation, method =>
                        {
                            if (operation.GenericTypes.Any())
                            {
                                foreach (var genericType in operation.GenericTypes)
                                {
                                    method.AddGenericParameter(genericType);
                                }
                            }
                            method.TryAddXmlDocComments(operation.InternalElement);

                            if (isAsync)
                            {
                                method.Async();
                            }

                            foreach (var parameter in operation.Parameters)
                            {
                                method.AddParameter(GetTypeName(parameter), parameter.Name.ToParameterName(),
                                    param => param.WithDefaultValue(parameter.Value));
                            }

                            if (isAsync)
                            {
                                method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", p => p.WithDefaultValue("default"));
                            }
                        });
                    }
                });
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
}