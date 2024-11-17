using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Behaviours.Templates.AuthorizationBehaviour
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class AuthorizationBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.Behaviours.AuthorizationBehaviour";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public AuthorizationBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddUsing("System.Reflection");
            AddUsing("System.Linq");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AuthorizationBehaviour", @class =>
                {
                    @class.AddGenericParameter("TRequest", out var TRequest);
                    @class.AddGenericParameter("TResponse", out var TResponse);
                    @class.AddGenericTypeConstraint(TRequest, cfg =>
                    {
                        cfg.AddType("notnull");
                    });

                    @class.ImplementsInterface(UseType("MediatR.IPipelineBehavior<TRequest, TResponse>"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService", param => param.IntroduceReadonlyField());
                    });

                    @class.AddMethod(UseType($"System.Threading.Tasks.Task<{TResponse}>"), "Handle", method =>
                    {
                        method.Async();

                        method.AddParameter($"{TRequest}", "request");
                        method.AddParameter($"RequestHandlerDelegate<{TResponse}>", "next");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddObjectInitStatement("var authorizeAttributes",
                            new CSharpInvocationStatement(new CSharpInvocationStatement(new CSharpInvocationStatement("request", "GetType").WithoutSemicolon(), "GetCustomAttributes<AuthorizeAttribute>").WithoutSemicolon(), "ToList"));

                        method.AddIfStatement("authorizeAttributes.Count > 0", @if =>
                        {
                            @if.AddStatement("// Must be authenticated user");
                            @if.AddIfStatement("_currentUserService.UserId is null", userIf =>
                            {
                                userIf.AddStatement($"throw new {UseType("System.UnauthorizedAccessException")}();");
                                userIf.BeforeSeparator = CSharpCodeSeparatorType.None;
                            });

                            @if.AddStatement("");
                            @if.AddStatement("// Role-Based authorization");
                            @if.AddInvocationStatement("await RoleBasedAuthenticationAsync", invoc =>
                            {
                                invoc.AddArgument("authorizeAttributes");
                            });

                            @if.AddStatement("");
                            @if.AddStatement("// Policy-based authorization");
                            @if.AddInvocationStatement("await PolicyBasedAuthenticationAsync", invoc =>
                            {
                                invoc.AddArgument("authorizeAttributes");
                            });
                        });

                        method.AddStatement("");
                        method.AddStatement("// User is authorized / authorization not required");
                        method.AddReturn("await next()");
                    });

                    @class.AddMethod("Task", "RoleBasedAuthenticationAsync", method =>
                    {
                        method.Async();
                        method.AddParameter(UseType($"System.Collections.Generic.List<{GetTypeName("Intent.Application.Identity.AuthorizeAttribute")}>"), "authorizeAttributes");

                        var fluent = new CSharpStatement("!string.IsNullOrWhiteSpace(a.Roles)");
                        method.AddObjectInitStatement("var authorizeAttributesWithRoles", new CSharpInvocationStatement(new CSharpInvocationStatement("authorizeAttributes", "Where")
                            .AddArgument(new CSharpArgument(new CSharpLambdaBlock("a").WithExpressionBody(fluent))).WithoutSemicolon(), "ToList"));

                        method.AddIfStatement("authorizeAttributesWithRoles.Count > 0", @roleIf =>
                        {
                            @roleIf.AddForEachStatement("roles", "authorizeAttributesWithRoles.Select(a => a.Roles.Split(','))", @for =>
                            {
                                @for.AddObjectInitStatement("var authorized", "false;");
                                @for.AddForEachStatement("role", "roles", roleFor =>
                                {
                                    roleFor.BeforeSeparator = CSharpCodeSeparatorType.None;
                                    roleFor.AddObjectInitStatement("var isInRole", "await _currentUserService.IsInRoleAsync(role.Trim());");
                                    roleFor.AddIfStatement("isInRole", inRoleIf =>
                                    {
                                        inRoleIf.BeforeSeparator = CSharpCodeSeparatorType.None;
                                        inRoleIf.AddObjectInitStatement("authorized", "true;");
                                        inRoleIf.AddStatement("break;");
                                    });
                                });

                                @for.AddStatement("");
                                @for.AddStatement("// Must be a member of at least one role in roles");
                                @for.AddIfStatement("!authorized", notAuthIf =>
                                {
                                    notAuthIf.BeforeSeparator = CSharpCodeSeparatorType.None;
                                    notAuthIf.AddStatement($"throw new ForbiddenAccessException();");
                                });
                            });
                        });
                    });

                    @class.AddMethod("Task", "PolicyBasedAuthenticationAsync", method =>
                    {
                        method.Async();
                        method.AddParameter(UseType($"System.Collections.Generic.List<{GetTypeName("Intent.Application.Identity.AuthorizeAttribute")}>"), "authorizeAttributes");

                        var fluent = new CSharpStatement("!string.IsNullOrWhiteSpace(a.Policy)");
                        method.AddObjectInitStatement("var authorizeAttributesWithPolicies", new CSharpInvocationStatement(new CSharpInvocationStatement("authorizeAttributes", "Where")
                            .AddArgument(new CSharpArgument(new CSharpLambdaBlock("a").WithExpressionBody(fluent))).WithoutSemicolon(), "ToList"));

                        method.AddIfStatement("authorizeAttributesWithPolicies.Count > 0", @roleIf =>
                        {
                            @roleIf.AddForEachStatement("policy", "authorizeAttributesWithPolicies.Select(a => a.Policy)", @for =>
                            {
                                @for.AddObjectInitStatement("var authorized", "await _currentUserService.AuthorizeAsync(policy);");

                                @for.AddIfStatement("!authorized", notAuthIf =>
                                {
                                    notAuthIf.AddStatement($"throw new {GetTypeName("Intent.Application.Identity.ForbiddenAccessException")}();");
                                });
                            });
                        });
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"AuthorizationBehaviour",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
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
    }
}