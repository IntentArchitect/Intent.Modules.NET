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

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Jwt.Templates.JwtClaimsMiddleware
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class JwtClaimsMiddlewareTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.Jwt.JwtClaimsMiddleware";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public JwtClaimsMiddlewareTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationJwtBearer(outputTarget));
            
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.IdentityModel.Tokens.Jwt")
                .AddUsing("System.Linq")
                .AddUsing("System.Net")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Security.Claims")
                .AddUsing("Microsoft.AspNetCore.Authorization")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.Azure.Functions.Worker")
                .AddUsing("Microsoft.Azure.Functions.Worker.Extensions.Http")
                .AddUsing("Microsoft.Azure.Functions.Worker.Middleware")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("Microsoft.Extensions.Primitives")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddClass("JwtClaimsMiddleware", cls =>
                {
                    cls.ImplementsInterface("IFunctionsWorkerMiddleware");

                    cls.AddField("JwtSecurityTokenHandler", "_tokenHandler", f => f.PrivateReadOnly());
                    cls.AddField("IHttpContextAccessor", "_httpContextAccessor", f => f.PrivateReadOnly());
                    cls.AddField("ILogger<JwtClaimsMiddleware>", "_logger", f => f.PrivateReadOnly());

                    cls.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("JwtSecurityTokenHandler", "tokenHandler");
                        ctor.AddParameter("IHttpContextAccessor", "httpContextAccessor");
                        ctor.AddParameter("ILogger<JwtClaimsMiddleware>", "logger");
                        ctor.AddStatement("_tokenHandler = tokenHandler;");
                        ctor.AddStatement("_httpContextAccessor = httpContextAccessor;");
                        ctor.AddStatement("_logger = logger;");
                    });

                    cls.AddMethod("Task", "Invoke", m =>
                    {
                        m.Async();
                        m.AddParameter("FunctionContext", "context");
                        m.AddParameter("FunctionExecutionDelegate", "next");

                        m.AddIfStatement("!IsHttpTrigger(context)", ifBlock =>
                        {
                            ifBlock.AddStatement("await next(context);");
                            ifBlock.AddStatement("return;");
                        });

                        m.AddStatement("var httpContext = context.GetHttpContext();");
                        m.AddIfStatement("httpContext == null", ifBlock =>
                        {
                            ifBlock.AddStatement("await next(context);");
                            ifBlock.AddStatement("return;");
                        });

                        m.AddAssignmentStatement("_httpContextAccessor.HttpContext", new CSharpStatement("httpContext"), s => s.WithSemicolon());

                        m.AddIfStatement("TryGetBearerToken(httpContext.Request, out var tokenValue)", ifBlock =>
                        {
                            ifBlock.AddIfStatement("TryCreatePrincipal(tokenValue, out var principal)", inner =>
                                {
                                    inner.AddStatement("AttachPrincipal(httpContext, tokenValue, principal);");
                                })
                                .AddElseStatement(inner =>
                                {
                                    inner.AddAssignmentStatement("httpContext.Response.StatusCode", new CSharpStatement("(int)HttpStatusCode.BadRequest"), s => s.WithSemicolon());
                                    inner.AddInvocationStatement("await httpContext.Response.WriteAsync", inv =>
                                    {
                                        inv.AddArgument("@\"Invalid authorization token.\"");
                                    });
                                    inner.AddStatement("return;");
                                });
                        });

                        m.AddStatement("await next(context);");
                    });

                    cls.AddMethod("void", "AttachPrincipal", m =>
                    {
                        m.Private();
                        m.AddParameter("HttpContext", "httpContext");
                        m.AddParameter("string", "tokenValue");
                        m.AddParameter("ClaimsPrincipal", "principal");

                        m.AddAssignmentStatement("httpContext.User", new CSharpStatement("principal"), s => s.WithSemicolon());
                        m.AddStatement("httpContext.Items[\"RawToken\"] = tokenValue;");
                    });

                    cls.AddMethod("bool", "TryGetBearerToken", m =>
                    {
                        m.Private().Static();
                        m.AddParameter("HttpRequest", "request");
                        m.AddParameter("out string", "token");

                        m.AddStatement("token = string.Empty;");
                        m.AddIfStatement("!request.Headers.TryGetValue(@\"Authorization\", out StringValues authorizationValues)", ifBlock =>
                        {
                            ifBlock.AddReturn("false");
                        });

                        m.AddStatement("var authorizationHeader = authorizationValues.FirstOrDefault();");
                        m.AddIfStatement(
                            "string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith(@\"Bearer \", StringComparison.OrdinalIgnoreCase)",
                            ifBlock =>
                            {
                                ifBlock.AddReturn("false");
                            });

                        m.AddAssignmentStatement("token", new CSharpStatement("authorizationHeader[@\"Bearer \".Length..].Trim()"), s => s.WithSemicolon());
                        m.AddReturn("!string.IsNullOrEmpty(token)");
                    });

                    cls.AddMethod("bool", "IsHttpTrigger", m =>
                    {
                        m.Private().Static();
                        m.AddParameter("FunctionContext", "context");

                        m.AddReturn(
                            "context.FunctionDefinition.InputBindings.Values.Any(binding => string.Equals(binding.Type, @\"httpTrigger\", StringComparison.OrdinalIgnoreCase))");
                    });

                    cls.AddMethod("bool", "TryCreatePrincipal", m =>
                    {
                        m.Private();
                        m.AddParameter("string", "tokenValue");
                        m.AddParameter("out ClaimsPrincipal", "principal");

                        m.AddStatement("principal = default!;");

                        m.AddTryBlock(tb =>
                            {
                                tb.AddStatement("var token = _tokenHandler.ReadJwtToken(tokenValue);");
                                tb.AddStatement("var identity = new ClaimsIdentity(token.Claims, @\"Bearer\", ClaimTypes.Name, ClaimTypes.Role);");
                                tb.AddAssignmentStatement("principal", new CSharpStatement("new ClaimsPrincipal(identity)"), s => s.WithSemicolon());
                                tb.AddReturn("true");
                            })
                            .AddCatchBlock("ArgumentException", "ex", cb =>
                            {
                                cb.AddInvocationStatement("_logger.LogWarning", inv =>
                                {
                                    inv.AddArgument("ex");
                                    inv.AddArgument("@\"Failed to parse JWT token due to invalid format.\"");
                                });
                                cb.AddReturn("false");
                            });
                    });
                })
                .AddClass("JwtClaimsMiddlewareExtensions", cls =>
                {
                    cls.Static();
                    cls.AddMethod("IFunctionsWorkerApplicationBuilder", "UseJwtClaimsMiddleware", m =>
                    {
                        m.Static();
                        m.AddParameter("this IFunctionsWorkerApplicationBuilder", "builder");
                        m.AddReturn("builder.UseMiddleware<JwtClaimsMiddleware>()");
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            ExecutionContext.EventDispatcher.Publish(ApplicationBuilderRegistrationRequest.ToRegister("UseJwtClaimsMiddleware")
                .HasDependency(this)
            );
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