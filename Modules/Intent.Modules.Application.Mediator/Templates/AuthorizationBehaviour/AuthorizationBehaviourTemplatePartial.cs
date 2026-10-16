using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.Security.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.Templates.AuthorizationBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AuthorizationBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.AuthorizationBehaviour";

        private const string CurrentUserServiceInterfaceTemplateId = "Intent.Application.Identity.CurrentUserServiceInterface";
        private const string AuthorizeAttributeTemplateId = "Intent.Application.Identity.AuthorizeAttribute";
        private const string ForbiddenAccessExceptionTemplateId = "Intent.Application.Identity.ForbiddenAccessException";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AuthorizationBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq")
                .AddUsing("System.Reflection")
                .AddClass($"AuthorizationBehaviour", @class =>
                {
                    @class.AddGenericParameter("TRequest", out var tRequest);
                    @class.AddGenericParameter("TResponse", out var tResponse);
                    @class.ImplementsInterface(UseType($"Mediator.IPipelineBehavior<{tRequest}, {tResponse}>"));
                    @class.AddGenericTypeConstraint(tRequest, c => c.AddType(UseType("Mediator.IMessage")));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(GetTypeName(CurrentUserServiceInterfaceTemplateId), "currentUserService", param => param.IntroduceReadonlyField());
                    });

                    @class.AddMethod($"{UseType("System.Threading.Tasks.ValueTask")}<{tResponse}>", "Handle", method =>
                    {
                        method.Async(asValueTask: true);
                        method.AddParameter(tRequest, "request");
                        method.AddParameter(UseType($"Mediator.MessageHandlerDelegate<{tRequest}, {tResponse}>"), "next");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        var unauthorizedAccessException = UseType("System.UnauthorizedAccessException");
                        var forbiddenAccessException = GetTypeName(ForbiddenAccessExceptionTemplateId);

                        method.AddStatement($"var authorizeAttributes = request.GetType().GetCustomAttributes<{GetTypeName(AuthorizeAttributeTemplateId)}>();");
                        method.AddForEachStatement("authorizeAttribute", "authorizeAttributes", foreachAttribute =>
                        {
                            foreachAttribute.AddStatement("// Must be an authenticated user");
                            foreachAttribute.AddIfStatement("await _currentUserService.GetAsync() is null", @if =>
                            {
                                @if.SeparatedFromPrevious(false);
                                @if.AddStatement($"throw new {unauthorizedAccessException}();");
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
                                    @if.AddStatement($"throw new {forbiddenAccessException}();");
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
                                    @if.AddStatement($"throw new {forbiddenAccessException}();");
                                });
                            });
                        });

                        method.AddStatement("// User is authorized / authorization not required", s => s.SeparatedFromPrevious());
                        method.AddStatement("return await next(request, cancellationToken);");
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && IdentityIsInstalled() && AnyRequestIsSecured();
        }

        private bool IdentityIsInstalled()
        {
            return TryGetTypeName(CurrentUserServiceInterfaceTemplateId, out _) &&
                   TryGetTypeName(AuthorizeAttributeTemplateId, out _) &&
                   TryGetTypeName(ForbiddenAccessExceptionTemplateId, out _);
        }

        private bool AnyRequestIsSecured()
        {
            var services = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id);

            return services.GetCommandModels().Any(command => SecurityModelHelpers.GetSecurityModels(command.InternalElement).Any()) ||
                   services.GetQueryModels().Any(query => SecurityModelHelpers.GetSecurityModels(query.InternalElement).Any());
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
