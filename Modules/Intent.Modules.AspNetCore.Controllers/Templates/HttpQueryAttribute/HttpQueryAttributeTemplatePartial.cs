using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.HttpQueryAttribute
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HttpQueryAttributeTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Controllers.HttpQueryAttribute";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpQueryAttributeTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // Stand-in until ASP.NET Core ships Microsoft.AspNetCore.Mvc.HttpQueryAttribute (none up to 11.0.0-rc.3).
            // See CONTEXT.md for when and how to retire it.
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Diagnostics.CodeAnalysis")
                .AddClass($"HttpQueryAttribute", @class =>
                {
                    // HttpMethods.Query exists from .NET 10, so older targets must not reference it, not even from a cref
                    var supportsHttpMethodsQuery = OutputTarget.GetMaxNetAppVersion().Major >= 10;
                    @class.WithComments(supportsHttpMethodsQuery
                        ?
                        [
                            "/// <summary>",
                            "/// Identifies an action that supports the HTTP QUERY method (RFC 10008). ASP.NET Core 10 ships",
                            "/// <see cref=\"Microsoft.AspNetCore.Http.HttpMethods.Query\"/> but no attribute for it, so this mirrors <see cref=\"Microsoft.AspNetCore.Mvc.HttpPostAttribute\"/>.",
                            "/// </summary>"
                        ]
                        :
                        [
                            "/// <summary>",
                            "/// Identifies an action that supports the HTTP QUERY method (RFC 10008). ASP.NET Core has no attribute for it,",
                            "/// so this mirrors <see cref=\"Microsoft.AspNetCore.Mvc.HttpPostAttribute\"/>.",
                            "/// </summary>"
                        ]);
                    @class.Sealed();
                    @class.AddAttribute("[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]");
                    @class.WithBaseType(UseType("Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute"));
                    // Qualified: HttpMethodAttribute has its own HttpMethods property.
                    @class.AddField("IEnumerable<string>", "SupportedMethods", field =>
                    {
                        field.Private().Static().ReadOnly();
                        field.WithAssignment(supportsHttpMethodsQuery
                            ? "[Microsoft.AspNetCore.Http.HttpMethods.Query]"
                            : "[\"QUERY\"]");
                    });
                    @class.AddConstructor(ctor =>
                    {
                        ctor.CallsBase(callBase => callBase.AddArgument("SupportedMethods"));
                    });
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "template", param => param.AddAttribute("[StringSyntax(\"Route\")]"));
                        ctor.CallsBase(callBase => callBase.AddArgument("SupportedMethods").AddArgument("template"));
                        ctor.AddStatement("ArgumentNullException.ThrowIfNull(template);");
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && HttpQueryHelper.NeedsHttpQueryInfrastructure(ExecutionContext.MetadataManager, ExecutionContext.GetApplicationConfig().Id);
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