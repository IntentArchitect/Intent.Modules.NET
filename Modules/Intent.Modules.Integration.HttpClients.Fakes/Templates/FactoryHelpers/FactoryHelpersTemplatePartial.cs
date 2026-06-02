using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Fakes.Templates.FactoryHelpers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FactoryHelpersTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Integration.HttpClients.Fakes.FactoryHelpers";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FactoryHelpersTemplate(IOutputTarget outputTarget, object? model = null)
            : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
                .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
                .IntentManagedFully()
                .AddClass("FactoryHelpers", @class =>
                {
                    @class.Internal().Static();

                    @class.AddMethod("T", "Configure", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("T");
                        method.AddParameter("T", "instance");
                        method.AddParameter(UseType("System.Action<T>"), "configure");
                        method.AddStatement($"{UseType("System.ArgumentNullException")}.ThrowIfNull(configure);");
                        method.AddStatement("configure(instance);");
                        method.AddReturn("instance");
                    });

                    @class.AddMethod(UseType("System.Collections.Generic.List<T>"), "List", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("T");
                        method.AddParameter(UseType("System.Func<T>"), "create");
                        method.AddParameter("int", "count");
                        method.AddParameter($"{UseType("System.Action<T, int>")}?", "configure", parameter => parameter.WithDefaultValue("null"));
                        method.AddStatement($"{UseType("System.ArgumentNullException")}.ThrowIfNull(create);");
                        method.AddStatement($"var list = new {UseType("System.Collections.Generic.List<T>")}(count);");
                        method.AddStatement("var index = 0;");
                        method.AddWhileStatement("index < count", loop =>
                        {
                            loop.AddStatement("var dto = create();");
                            loop.AddStatement("configure?.Invoke(dto, index);");
                            loop.AddStatement("list.Add(dto);");
                            loop.AddStatement("index++;");
                        });
                        method.AddReturn("list");
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
