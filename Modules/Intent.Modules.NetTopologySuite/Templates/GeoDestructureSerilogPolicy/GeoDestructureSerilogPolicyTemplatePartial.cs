using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.NetTopologySuite.Templates.GeoDestructureSerilogPolicy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GeoDestructureSerilogPolicyTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.NetTopologySuite.GeoDestructureSerilogPolicy";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GeoDestructureSerilogPolicyTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.NetTopologySuite);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("NetTopologySuite.Geometries")
                .AddUsing("Serilog.Core")
                .AddUsing("Serilog.Events")
                .AddClass($"GeoDestructureSerilogPolicy", @class =>
                {
                    @class.WithComments("""
                                        /// <summary>
                                        /// NetTopologySuite.Geometries by default when serialized will cause circular referencing which results in infinite logging.
                                        /// This Destructure solves this problem by overriding the serializing behaviour to write out simpler and easy to read representations.
                                        /// </summary>
                                        """);
                    @class.ImplementsInterface("IDestructuringPolicy");
                    @class.AddMethod("bool", "TryDestructure", method =>
                    {
                        method.AddParameter("object", "value")
                            .AddParameter("ILogEventPropertyValueFactory", "propertyValueFactory")
                            .AddParameter($"LogEventPropertyValue{Nullable}", "result", param => param.WithOutParameterModifier());
                        method.AddStatements(
                            """
                            result = value switch
                            {
                                Point point => new ScalarValue($"Point({point.X}, {point.Y})"),
                                _ => null
                            };
                            return result is not null;
                            """);
                    });
                });
        }

        private string Nullable => OutputTarget.GetProject().NullableEnabled ? "?" : string.Empty;

        public override bool CanRunTemplate()
        {
            return ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Modules.AspNetCore.Logging.Serilog");
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