using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.ReflectionHelper
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ReflectionHelperTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.ReflectionHelper";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ReflectionHelperTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Reflection")
                .AddClass($"ReflectionHelper", @class =>
                {
                    @class.Internal().Static();

                    @class.AddMethod("T", "CreateNewInstanceOf", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("T", out var t);

                        method.AddStatements(new[]
                        {
                            $"var constructorInfo = typeof({t}).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null)!;",
                            $"var instance = ({t})constructorInfo.Invoke({UseType("System.Array")}.Empty<object>());",
                            "return instance;"
                        });
                    });

                    @class.AddMethod("void", "ForceSetProperty", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("T", out var t);
                        method.AddParameter(t, "instance");
                        method.AddParameter("string", "propertyName");
                        method.AddParameter("object?", "value");

                        method.AddStatements(new[]
                        {
                            $"var propertyInfo = typeof({t}).GetProperty(propertyName)!;",
                            "propertyInfo = propertyInfo.DeclaringType!.GetProperty(propertyName)!;",
                            "propertyInfo.SetValue(instance, value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);"
                        });
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