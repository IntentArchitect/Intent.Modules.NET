using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.AutoMapper.Templates.MapFromInterface
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class MapFromInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.AutoMapper.MapFromInterface";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public MapFromInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"IMapFrom", @interface =>
                {
                    @interface.Internal();
                    @interface.AddGenericParameter("T");

                    @interface.AddMethod(CSharpTypeVoid.DefaultInstance, "Mapping", method =>
                    {
                        method.AddParameter(UseType("AutoMapper.Profile"), "profile");
                    });
                });
            AddNugetDependency(NugetPackages.AutoMapper(outputTarget));
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IMapFrom",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

    }
}