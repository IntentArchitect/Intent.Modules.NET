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

namespace Intent.Modules.Application.Identity.Templates.AuthorizeAttribute
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class AuthorizeAttributeTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Identity.AuthorizeAttribute";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AuthorizeAttributeTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddUsing("System");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AuthorizeAttribute", @class =>
                {
                    @class.WithComments(@"/// <summary>
    /// Specifies the class this attribute is applied to requires authorization.
    /// </summary>");

                    @class.AddAttribute("AttributeUsage", att =>
                    {
                        att.AddArgument("AttributeTargets.Class");
                        att.AddArgument("AllowMultiple = true");
                        att.AddArgument("Inherited = true");
                    });


                    @class.WithBaseType("Attribute");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.WithComments(@"/// <summary>
        /// Initializes a new instance of the <see cref=""AuthorizeAttribute""/> class.
        /// </summary>");

                        ctor.AddObjectInitStatement("Roles", "null!;");
                        ctor.AddObjectInitStatement("Policy", "null!;");
                    });

                    @class.AddProperty("string", "Roles", prop =>
                    {
                        prop.WithComments(@"/// <summary>
        /// Gets or sets a comma delimited list of roles that are allowed to access the resource.
        /// </summary>");
                    });

                    @class.AddProperty("string", "Policy", prop =>
                    {
                        prop.WithComments(@"/// <summary>
        /// Gets or sets the policy name that determines access to the resource.
        /// </summary>");
                    });

                });
            FulfillsRole("Application.Identity.AuthorizeAttribute");
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"AuthorizeAttribute",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

    }
}