using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates.GrpcExceptionInterceptor
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GrpcExceptionInterceptorTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Grpc.GrpcExceptionInterceptor";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GrpcExceptionInterceptorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())

                .AddUsing("System")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Google.Protobuf.WellKnownTypes")
                .AddUsing("Google.Rpc")
                .AddUsing("Grpc.Core")
                .AddUsing("Grpc.Core.Interceptors")
                .AddUsing("Microsoft.AspNetCore.Hosting")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("Status = Google.Rpc.Status")

                .AddClass($"GrpcExceptionInterceptor", @class =>
                {
                    @class.WithBaseType("Interceptor");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IWebHostEnvironment", "webHostEnvironment", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    const string tResponse = "TResponse";
                    const string tRequest = "TRequest";
                    @class.AddMethod(tResponse, "UnaryServerHandler", method =>
                    {
                        method.Override().Async();
                        method.AddGenericParameter(tRequest);
                        method.AddGenericParameter(tResponse);

                        method.AddParameter(tRequest, "request");
                        method.AddParameter("ServerCallContext", "context");
                        method.AddParameter($"UnaryServerMethod<{tRequest}, {tResponse}>", "continuation");

                        method.AddTryBlock(block =>
                        {
                            block.AddReturn("await base.UnaryServerHandler(request, context, continuation)");
                        });
                        method.AddCatchBlock(@catch =>
                        {
                            @catch.WithExceptionType("Exception e");
                            @catch.AddSwitchStatement("e", @switch =>
                            {
                                if (ExecutionContext
                                    .FindTemplateInstances<IClassProvider>(TemplateDependency.OnTemplate("Application.Validation"))
                                    .Concat(ExecutionContext
                                        .FindTemplateInstances<IClassProvider>(TemplateDependency.OnTemplate("Application.Validation.Dto")))
                                    .Any())
                                {
                                    CSharpFile!.AddUsing("System.Linq");
                                    CSharpFile!.AddUsing("FluentValidation");

                                    @switch.AddCase("ValidationException exception", @case =>
                                    {
                                        @case.AddObjectInitializerBlock("throw new Status", initializer =>
                                        {
                                            initializer.AddInitStatement("Code", "(int)Code.InvalidArgument");
                                            initializer.AddInitStatement("Message", "\"Bad request\"");
                                            initializer.AddInitStatement("Details", new CSharpObjectInitializerBlock(null)
                                                .AddInvocationStatement("Any.Pack", invocation =>
                                                {
                                                    invocation.AddObjectInitializerBlock("new BadRequest", init =>
                                                    {
                                                        init.AddInitStatement("FieldViolations", new CSharpObjectInitializerBlock(null)
                                                                .AddStatement("exception.Errors.Select(x => new BadRequest.Types.FieldViolation { Field = x.PropertyName, Description = x.ErrorMessage })"));
                                                    });
                                                    invocation.WithoutSemicolon();
                                                }));
                                            initializer.AddInvocation("ToRpcException");
                                        });
                                    });
                                }

                                if (TryGetTypeName("Application.ForbiddenAccessException", out var forbiddenAccessException))
                                {
                                    @switch.AddCase(forbiddenAccessException, @case =>
                                    {
                                        @case.AddStatement("throw new Status { Code = (int)Code.PermissionDenied, Message = \"Permission denied\" }.ToRpcException();");
                                    });
                                }

                                @switch.AddCase("UnauthorizedAccessException", @case =>
                                {
                                    @case.AddStatement("throw new Status { Code = (int)Code.Unauthenticated, Message = \"Unauthenticated\" }.ToRpcException();");
                                });

                                if (TryGetTypeName("Domain.NotFoundException", out var domainNotFoundException))
                                {
                                    @switch.AddCase($"{domainNotFoundException} exception", @case =>
                                    {
                                        @case.AddStatement("throw new Status { Code = (int)Code.NotFound, Message = exception.Message }.ToRpcException();");
                                    });
                                }

                                @switch.AddDefault(@default =>
                                {
                                    @default.AddIfStatement("_webHostEnvironment.IsDevelopment()", @if =>
                                    {
                                        @if.AddObjectInitializerBlock("throw new Status", initializer =>
                                        {
                                            initializer.AddInitStatement("Code", "(int)Code.Internal");
                                            initializer.AddInitStatement("Message", "\"Internal error\"");
                                            initializer.AddInitStatement("Details", new CSharpObjectInitializerBlock(null)
                                                .AddStatement("Any.Pack(e.ToRpcDebugInfo())"));
                                            initializer.AddInvocation("ToRpcException");
                                        });
                                    });

                                    @default.AddStatement("throw;", s => s.SeparatedFromPrevious());
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