using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.Templates.Endpoint
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EndpointTemplate : CSharpTemplateBase<IEndpointModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.FastEndpoints.EndpointTemplate";

        private CSharpClass? _requestModelClass;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EndpointTemplate(IOutputTarget outputTarget, IEndpointModel model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.FastEndpoints(OutputTarget));

            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Application.Command);
            AddTypeSource(TemplateRoles.Application.Query);
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("FastEndpoints")
                .AddUsing("Mode = Intent.RoslynWeaver.Attributes.Mode")
                .OnBuild(file =>
                {
                    if (file.Template.ExecutionContext.FindTemplateInstances("Distribution.SwashbuckleConfiguration")?.Any() == true)
                    {
                        AddNugetDependency(NugetPackages.FastEndpointsSwaggerSwashbuckle(OutputTarget));
                    }
                });

            AddRequestModelIfApplicable();
            AddEndpointClass();
        }

        private void AddRequestModelIfApplicable()
        {
            if (!Model.Parameters.Any())
            {
                return;
            }

            _requestModelClass = new CSharpClass($"{Model.Name.RemoveSuffix("Endpoint")}RequestModel", CSharpFile);
            CSharpFile.OnBuild(file =>
            {
                file.TypeDeclarations.Add(_requestModelClass);
                foreach (var parameter in Model.Parameters)
                {
                    _requestModelClass.AddProperty(GetTypeName(parameter.TypeReference), parameter.Name.ToPropertyName(), prop =>
                    {
                        var attr = GetParameterBindingAttribute(parameter);
                        if (attr is not null)
                        {
                            prop.AddAttribute(attr);
                        }
                    });
                }
            }, 1);
        }

        private void AddEndpointClass()
        {
            CSharpFile.AddClass($"{Model.Name.RemoveSuffix("Endpoint")}Endpoint", @class =>
            {
                DefineEndpointBaseType(@class);

                @class.AddMethod("void", "Configure", method =>
                {
                    method.Override();
                    AddHttpVerbAndRoute(method);
                    AddDescriptionConfiguration(method);
                    method.AddStatement("AllowAnonymous();");
                });

                @class.AddMethod("Task", "HandleAsync", method =>
                {
                    method.Override().Async();
                    if (_requestModelClass is not null)
                    {
                        method.AddParameter(_requestModelClass.Name, "req");
                    }

                    method.AddParameter("CancellationToken", "ct");
                });
            });
        }

        private string? GetReturnType()
        {
            if (Model.ReturnType is not null &&
                GetTypeInfo(Model.ReturnType).IsPrimitive || Model.ReturnType.HasStringType())
            {
                return $"{this.GetJsonResponseTemplateName()}<{GetTypeName(Model)}>";
            }

            return Model.ReturnType is not null ? GetTypeName(Model.ReturnType) : null;
        }

        private void DefineEndpointBaseType(CSharpClass @class)
        {
            var requestType = _requestModelClass?.Name;
            var responseType = GetReturnType();
            string baseType = default!;

            if (requestType is not null &&
                responseType is not null)
            {
                baseType = $"Endpoint<{requestType}, {responseType}>";
            }
            else if (requestType is not null &&
                     responseType is null)
            {
                baseType = $"Endpoint<{requestType}>";
            }
            else if (requestType is null &&
                     responseType is not null)
            {
                baseType = $"EndpointWithoutRequest<{responseType}>";
            }
            else if (requestType is null &&
                     responseType is null)
            {
                baseType = "EndpointWithoutRequest";
            }

            @class.WithBaseType(baseType);
        }

        private void AddHttpVerbAndRoute(CSharpClassMethod method)
        {
            var verb = Model.Verb switch
            {
                HttpVerb.Get => "Get",
                HttpVerb.Post => "Post",
                HttpVerb.Delete => "Delete",
                HttpVerb.Put => "Put",
                HttpVerb.Patch => "Patch",
                _ => throw new NotSupportedException($"Verb {Model.Verb} is not supported.")
            };
            method.AddStatement(new CSharpInvocationStatement(verb)
                .AddArgument($@"""{Model.Route}"""));
        }

        private void AddDescriptionConfiguration(CSharpClassMethod method)
        {
            var operationId = (Model.InternalElement.GetStereotype("OpenAPI Settings")?.GetProperty<string>("OperationId") ?? string.Empty)
                .Replace("{ServiceName}", Model.Container.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper) ?? string.Empty)
                .Replace("{MethodName}", Model.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper));

            var lambda = new CSharpLambdaBlock("b");
            if (!string.IsNullOrWhiteSpace(operationId))
            {
                lambda.AddInvocationStatement("b.WithName", name => name.AddArgument($@"""{operationId}"""));
            }

            if (_requestModelClass is not null)
            {
                lambda.AddInvocationStatement($"b.Accepts<{_requestModelClass.Name}>");
            }

            var mediaTypeNamesApplicationJson = Model.MediaType == HttpMediaType.ApplicationJson || Model.ReturnType?.Element.IsDTOModel() == true
                ? "MediaTypeNames.Application.Json"
                : null;

            if (mediaTypeNamesApplicationJson is not null)
            {
                AddUsing("System.Net.Mime");
            }

            var producesReturnTypeDefinition = GetReturnType() is not null ? $"<{GetReturnType()}>" : "";

            switch (Model.Verb)
            {
                case HttpVerb.Get:
                case HttpVerb.Delete:
                    lambda.AddInvocationStatement($"b.Produces{producesReturnTypeDefinition}", i => i
                        .AddArgument($"StatusCodes.{Model.GetSuccessResponseCodeEnumValue("Status200OK")}")
                        .AddArgumentIfNotNull(mediaTypeNamesApplicationJson));
                    break;
                case HttpVerb.Post:
                    lambda.AddInvocationStatement($"b.Produces{producesReturnTypeDefinition}", i => i
                        .AddArgument($"StatusCodes.{Model.GetSuccessResponseCodeEnumValue("Status201Created")}")
                        .AddArgumentIfNotNull(mediaTypeNamesApplicationJson));
                    break;
                case HttpVerb.Put:
                case HttpVerb.Patch:
                    var defaultValue = Model.ReturnType != null ? "Status200OK" : "Status204NoContent";
                    lambda.AddInvocationStatement($"b.Produces{producesReturnTypeDefinition}", i => i
                        .AddArgument($"StatusCodes.{Model.GetSuccessResponseCodeEnumValue(defaultValue)}")
                        .AddArgumentIfNotNull(mediaTypeNamesApplicationJson));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown verb: {Model.Verb}");
            }

            AddUsing("Microsoft.AspNetCore.Http");
            if (Model.Parameters.Any())
            {
                lambda.AddInvocationStatement("b.ProducesProblemDetails");
            }

            lambda.AddInvocationStatement("b.ProducesProblemDetails", i => i.AddArgument("StatusCodes.Status500InternalServerError"));

            if (lambda.Statements.Count > 0)
            {
                method.AddInvocationStatement("Description", inv => inv.AddArgument(lambda));
            }
        }

        private CSharpAttribute? GetParameterBindingAttribute(IEndpointParameterModel parameter)
        {
            if (parameter.TypeReference.Element.IsDTOModel() &&
                parameter.Source is null or HttpInputSource.FromBody)
            {
                return new CSharpAttribute("FromBody");
            }

            if (parameter.Source is null or HttpInputSource.FromRoute &&
                Model.Route.Contains($"{{{parameter.Name}}}", StringComparison.OrdinalIgnoreCase))
            {
                return null; // FastEndpoints default to Route Parameters and doesn't have a FromRoute attribute
            }

            if (parameter.Source is HttpInputSource.FromHeader)
            {
                return new CSharpAttribute($"FromHeader").AddArgument($@"""{parameter.HeaderName}""");
            }

            if (parameter.Source is null or HttpInputSource.FromQuery)
            {
                var attr = new CSharpAttribute("FromQuery");
                if (!string.IsNullOrWhiteSpace(parameter.QueryStringName))
                {
                    attr.AddArgument($@"""{parameter.QueryStringName}""");
                }

                return attr;
            }

            return null;
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

static class EndpointUtilExtensions
{
    public static CSharpInvocationStatement AddArgumentIfNotNull(
        this CSharpInvocationStatement statement,
        CSharpStatement? argument,
        Action<CSharpStatement> configure = null)
    {
        if (argument is null)
        {
            return statement;
        }

        return statement.AddArgument(argument);
    }
}