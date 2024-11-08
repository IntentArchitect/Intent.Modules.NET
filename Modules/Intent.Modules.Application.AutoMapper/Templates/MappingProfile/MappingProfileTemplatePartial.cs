using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.AutoMapper.Templates.MappingProfile
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class MappingProfileTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.AutoMapper.MappingProfile";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public MappingProfileTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddUsing("System");
            AddUsing("System.Linq");
            AddUsing("System.Reflection");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"MappingProfile", @class =>
                {
                    @class.WithBaseType(UseType("AutoMapper.Profile"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "exampleParam", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddInvocationStatement("ApplyMappingsFromAssembly", cfg =>
                        {
                            cfg.AddArgument(new CSharpInvocationStatement("Assembly.GetExecutingAssembly").WithoutSemicolon());
                        });
                    });

                    @class.AddMethod("void", "ApplyMappingsFromAssembly", method =>
                    {
                        method.Private();

                        method.AddParameter("Assembly", "assembly");

                        method.AddObjectInitStatement("var types", @"assembly.GetExportedTypes()
                .Where(t => Array.Exists(t.GetInterfaces(), i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();");

                        method.AddForEachStatement("type", "types", iteration =>
                        {
                            iteration.AddObjectInitStatement("var instance",
                                new CSharpInvocationStatement("Activator.CreateInstance")
                                    .AddArgument("type")
                                    .AddArgument("true"));

                            iteration.AddObjectInitStatement("var methodInfo", @"type.GetMethod(""Mapping"")
                    ?? type.GetInterface(""IMapFrom`1"")?.GetMethod(""Mapping"");");

                            var instance = outputTarget.GetProject().GetLanguageVersion().Major < 12 ? "new object[] { this }" : "[this]";
                            iteration.AddStatement($"methodInfo?.Invoke(instance, {instance});", cfg => cfg.SeparatedFromPrevious());
                        });

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
                className: $"MappingProfile",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

    }
}