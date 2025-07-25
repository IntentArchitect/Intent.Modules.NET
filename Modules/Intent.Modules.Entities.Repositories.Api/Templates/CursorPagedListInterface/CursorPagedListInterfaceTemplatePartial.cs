using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Entities.Repositories.Api.Templates.CursorPagedListInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CursorPagedListInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Entities.Repositories.Api.CursorPagedListInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CursorPagedListInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Repository.Interface.CursorPagedList);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"ICursorPagedList", @interface =>
                {
                    AddUsing("System.Collections.Generic");

                    @interface.ImplementsInterfaces("IList<T>");
                    @interface.AddGenericParameter("T");

                    @interface.AddProperty("int", "PageSize", prop =>
                    {
                        prop.WithoutSetter();
                    });

                    @interface.AddProperty("string?", "CursorToken", prop =>
                    {
                        prop.WithoutSetter();
                    });
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