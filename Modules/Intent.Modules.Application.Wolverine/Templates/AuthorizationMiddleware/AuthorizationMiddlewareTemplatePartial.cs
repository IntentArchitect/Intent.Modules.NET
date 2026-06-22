using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.Templates.AuthorizationMiddleware
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AuthorizationMiddlewareTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.AuthorizationMiddleware";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AuthorizationMiddlewareTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Reflection")
                .AddClass("AuthorizationMiddleware", @class =>
                {
                    var currentUserService = GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface", TemplateDiscoveryOptions.DoNotThrow);
                    var authorizeAttribute = GetTypeName("Intent.Application.Identity.AuthorizeAttribute", TemplateDiscoveryOptions.DoNotThrow);
                    var forbiddenException = GetTypeName("Intent.Application.Identity.ForbiddenAccessException", TemplateDiscoveryOptions.DoNotThrow);

                    if (string.IsNullOrEmpty(currentUserService))
                    {
                        return; // Identity module not installed — no-op middleware
                    }

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "BeforeAsync", method =>
                    {
                        method.Async();
                        method.AddParameter(UseType("Wolverine.Envelope"), "envelope");
                        method.AddParameter(currentUserService, "currentUserService");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");
                        method.AddStatement("await AuthorizeAsync(envelope.Message, currentUserService);");
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "AuthorizeAsync", method =>
                    {
                        method.Static().Async().Private();
                        method.AddParameter("object", "request");
                        method.AddParameter(currentUserService, "currentUserService");

                        method.AddStatement($"var authorizeAttributes = request.GetType().GetCustomAttributes<{authorizeAttribute}>();");

                        method.AddForEachStatement("authorizeAttribute", "authorizeAttributes", forEach =>
                        {
                            forEach.AddIfStatement("await currentUserService.GetAsync() is null", @if =>
                            {
                                @if.SeparatedFromPrevious(false);
                                @if.AddStatement("throw new UnauthorizedAccessException();");
                            });

                            forEach.AddIfStatement("!string.IsNullOrWhiteSpace(authorizeAttribute.Roles)", ifRoles =>
                            {
                                ifRoles.AddStatement("var authorized = false;");
                                ifRoles.AddStatement("var roles = authorizeAttribute.Roles.Split(',').Select(x => x.Trim());");
                                ifRoles.AddForEachStatement("role", "roles", forEachRole =>
                                {
                                    forEachRole.AddIfStatement("await currentUserService.IsInRoleAsync(role)", @if =>
                                    {
                                        @if.SeparatedFromPrevious(false);
                                        @if.AddStatement("authorized = true;");
                                        @if.AddStatement("break;");
                                    });
                                });
                                ifRoles.AddIfStatement("!authorized", @if =>
                                {
                                    @if.SeparatedFromPrevious(false);
                                    @if.AddStatement($"throw new {forbiddenException}();");
                                });
                            });

                            forEach.AddIfStatement("!string.IsNullOrWhiteSpace(authorizeAttribute.Policy)", ifPolicies =>
                            {
                                ifPolicies.AddStatement("var authorized = false;");
                                ifPolicies.AddStatement("var policies = authorizeAttribute.Policy.Split(',').Select(x => x.Trim());");
                                ifPolicies.AddForEachStatement("policy", "policies", forEachPolicy =>
                                {
                                    forEachPolicy.AddIfStatement("await currentUserService.AuthorizeAsync(policy)", @if =>
                                    {
                                        @if.SeparatedFromPrevious(false);
                                        @if.AddStatement("authorized = true;");
                                        @if.AddStatement("break;");
                                    });
                                });
                                ifPolicies.AddIfStatement("!authorized", @if =>
                                {
                                    @if.SeparatedFromPrevious(false);
                                    @if.AddStatement($"throw new {forbiddenException}();");
                                });
                            });
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