using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Contracts.Clients.Shared.FileNamespaceProviders;
using Intent.Modules.Eventing.MassTransit.Templates.RequestResponse.MapperRequestMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.RequestResponse.MapperRequestMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MapperRequestMessageTemplate : CSharpTemplateBase<CommandQueryModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.RequestResponse.MapperRequestMessage";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MapperRequestMessageTemplate(IOutputTarget outputTarget, CommandQueryModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateRoles.Application.Command);
            AddTypeSource(TemplateRoles.Application.Query);
            
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass(model.Name, @class =>
                {
                    @class.ImplementsInterface(this.GetMapperRequestInterfaceName());
                    var urn = $"{_namespaceProvider.GetFileNamespace(this)}:{model.Name}";
                    @class.AddAttribute(UseType("MassTransit.MessageUrn"), attr => attr.AddArgument($@"""{urn}"""));
                    foreach (var property in Model.Properties)
                    {
                        @class.AddProperty(GetTypeName(property), property.Name);
                    }

                    @class.AddMethod("object", "CreateRequest", method =>
                    {
                        method.AddInvocationStatement($"return new {GetTypeName(model.InternalElement)}", stmt =>
                        {
                            foreach (var property in Model.Properties)
                            {
                                stmt.AddArgument(property.Name);
                            }
                        });
                    });
                });
        }

        private readonly SourcePackageFileNamespaceProvider _namespaceProvider = new();

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
}