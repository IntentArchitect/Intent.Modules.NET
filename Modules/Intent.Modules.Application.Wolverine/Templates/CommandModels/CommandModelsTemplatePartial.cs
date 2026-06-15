using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Wolverine.Templates.CommandInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.Templates.CommandModels
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CommandModelsTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.CommandModels";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CommandModelsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource("Domain.Enum");
            AddTypeSource("Application.Contract.Dto");
            AddTypeSource("Application.Contract.Enum");
            AddTypeSource("Application.Contracts.Client.Dto");
            AddTypeSource("Application.Contracts.Client.Enum");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddClass(Model.Name, @class =>
                {
                    @class.RepresentsModel(model);
                    @class.ImplementsInterface(this.GetCommandInterfaceName());

                    foreach (var property in Model.Properties)
                    {
                        @class.AddProperty(GetTypeName(property), property.Name.ToPropertyName(), prop =>
                        {
                            prop.RepresentsModel(property);
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
