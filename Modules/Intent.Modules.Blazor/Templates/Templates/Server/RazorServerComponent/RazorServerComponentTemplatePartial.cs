using System;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Templates.Templates.Server.RazorServerComponentCodeBehind;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using ComponentModel = Intent.Modelers.UI.Api.ComponentModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Server.RazorServerComponent
{
    using Intent.Modules.Blazor.Templates.Templates.Server;
    using Intent.Templates;

    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Ignore, Comments = Mode.Fully)]
    public partial class RazorServerComponentTemplate : ServerComponentRazorTemplateBase
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Server.RazorServerComponentTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="RazorServerComponentTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RazorServerComponentTemplate(IOutputTarget outputTarget, ComponentModel model) : base(TemplateId, outputTarget, model)
        {
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Ignore)]
        public sealed override IRazorFile RazorFile => BuiltRazorFile;

        /// <inheritdoc />
        [IntentManaged(Mode.Ignore)]
        protected override RazorFileConfig DefineRazorConfig() => DefineRazorConfigCore();

        /// <inheritdoc />
        [IntentManaged(Mode.Ignore)]
        public override string TransformText() => TransformTextCore();

        protected override string CodeBehindTemplateId => RazorServerComponentCodeBehindTemplate.TemplateId;
    }
}
