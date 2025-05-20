using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Intent.Engine;
using Intent.Modules.Application.Identity.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Behaviours.Templates.AuthorizationBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AuthorizationBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.Behaviours.AuthorizationBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AuthorizationBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("MediatR")
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Reflection")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"AuthorizationBehaviour", @class =>
                {
                    @class.AddGenericParameter("TRequest", out var tRequest);
                    @class.AddGenericParameter("TResponse", out var tResponse);
                    @class.ImplementsInterface($"IPipelineBehavior<{tRequest}, {tResponse}>");
                    @class.AddGenericTypeConstraint(tRequest, c => c.AddType("notnull"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetCurrentUserServiceInterface(), "currentUserService", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod(tResponse, "Handle", method =>
                    {
                        method.Async();
                        method.AddParameter(tRequest, "request");
                        method.AddParameter($"RequestHandlerDelegate<{tResponse}>", "next");
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddStatement($"var authorizeAttributes = request.GetType().GetCustomAttributes<{this.GetAuthorizationAttribute()}>();");

                        method.AddForEachStatement("authorizeAttribute", "authorizeAttributes", foreachAttribute =>
                        {
                            foreachAttribute.AddStatement("// Must be an authenticated user");
                            foreachAttribute.AddIfStatement("_currentUserService.UserId is null", @if =>
                            {
                                @if.SeparatedFromPrevious(false);
                                @if.AddStatement("throw new UnauthorizedAccessException();");
                            });

                            foreachAttribute.AddStatement("// Role-based authorization", s => s.SeparatedFromPrevious());
                            foreachAttribute.AddIfStatement("!string.IsNullOrWhiteSpace(authorizeAttribute.Roles)", ifRoles =>
                            {
                                ifRoles.SeparatedFromPrevious(false);
                                ifRoles.AddStatement("var authorized = false;");
                                ifRoles.AddStatement("var roles = authorizeAttribute.Roles.Split(\",\").Select(x => x.Trim());");

                                ifRoles.AddForEachStatement("role", "roles", @foreach =>
                                {
                                    @foreach.AddStatement("var isInRole = await _currentUserService.IsInRoleAsync(role);");
                                    @foreach.AddIfStatement("isInRole", @if =>
                                    {
                                        @if.SeparatedFromPrevious(false);
                                        @if.AddStatement("authorized = true;");
                                        @if.AddStatement("break;");
                                    });
                                });

                                ifRoles.AddStatement("// Must be a member of at least one role in roles", s => s.SeparatedFromPrevious());
                                ifRoles.AddIfStatement("!authorized", @if =>
                                {
                                    @if.SeparatedFromPrevious(false);
                                    @if.AddStatement($"throw new {this.GetForbiddenAccessException()}();");
                                });
                            });

                            foreachAttribute.AddStatement("// Policy-based authorization", s => s.SeparatedFromPrevious());
                            foreachAttribute.AddIfStatement("!string.IsNullOrWhiteSpace(authorizeAttribute.Policy)", ifPolicies =>
                            {
                                ifPolicies.SeparatedFromPrevious(false);
                                ifPolicies.AddStatement("var authorized = false;");
                                ifPolicies.AddStatement("var policies = authorizeAttribute.Policy.Split(\",\").Select(x => x.Trim());");

                                ifPolicies.AddForEachStatement("policy", "policies", @foreach =>
                                {
                                    @foreach.AddStatement("var isAuthorized = await _currentUserService.AuthorizeAsync(policy);");
                                    @foreach.AddIfStatement("isAuthorized", @if =>
                                    {
                                        @if.SeparatedFromPrevious(false);
                                        @if.AddStatement("authorized = true;");
                                        @if.AddStatement("break;");
                                    });
                                });

                                ifPolicies.AddStatement("// Must be authorized by at least one policy", s => s.SeparatedFromPrevious());
                                ifPolicies.AddIfStatement("!authorized", @if =>
                                {
                                    @if.SeparatedFromPrevious(false);
                                    @if.AddStatement($"throw new {this.GetForbiddenAccessException()}();");
                                });
                            });
                        });

                        method.AddStatement("// User is authorized / authorization not required", s => s.SeparatedFromPrevious());
                        var cancellationToken = Project.TryGetMaxNetAppVersion(out var version) &&
                                                version.Major is <= 2 or > 6
                            ? "cancellationToken"
                            : string.Empty;
                        method.AddStatement($"return await next({cancellationToken});");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({ClassName}<,>)")
                .ForInterface("typeof(IPipelineBehavior<,>)")
                .WithPriority(2)
                .ForConcern("MediatR")
                .RequiresUsingNamespaces("MediatR")
                .HasDependency(this));
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