using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.FastEndpoints.Templates.Endpoint.Models;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.Templates.Endpoint
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EndpointTemplate : CSharpTemplateBase<IEndpointModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.FastEndpoints.EndpointTemplate";

        //private CSharpClass? _requestModelClass; // For handling inputs without body payloads
        //private IElement? _requestPayload; // Request Model + Body Payload

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

            AddKnownType("FastEndpoints.IEventBus");

            Versions = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id).GetApiVersionModels().FirstOrDefault();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("FastEndpoints")
                .AddUsing("Mode = Intent.RoslynWeaver.Attributes.Mode")
                .AddClass($"{Model.Name.RemoveSuffix("Endpoint")}Endpoint", @class =>
                    {
                        var payloadModel = TryGetRequestTemplate(Model);

                        TryResolveRequestModelClassName(payloadModel, out var requestModelClassName);

                        DefineEndpointBaseType(@class, requestModelClassName);

                        @class.AddConstructor();
                        @class.AddMethod("void", "Configure", method =>
                        {
                            method.Override();
                            AddHttpVerbAndRoute(method);
                            AddDescriptionConfiguration(method, requestModelClassName);
                            AddValidator(method, payloadModel);
                            AddSecurity(method);
                            AddVersioning(method);
                        });

                        @class.AddMethod("Task", "HandleAsync", method =>
                        {
                            method.Override().Async();

                            if (requestModelClassName is not null)
                            {
                                method.AddParameter(requestModelClassName, "req");
                            }

                            method.AddParameter("CancellationToken", "ct");
                            method.AddMetadata("handle", true);
                        });
                    });
        }

        private bool TryResolveRequestModelClassName(IElement? payloadModel, out string? requestModelClassName)
        {
            var requestTemplate = GetTypeInfo(payloadModel).Template as ICSharpFileBuilderTemplate;
            if (requestTemplate != null)
            {
                AddUsing(requestTemplate.Namespace);
                AddHttpAttributeInputsToRequestTemplate(requestTemplate);
            }

            requestModelClassName = requestTemplate != null
                ? GetTypeName(requestTemplate)
                : Model.Parameters.Count > 0 ? CreateInlineRequestModelClass().Name : null;

            return requestModelClassName != null;
        }

        private void AddHttpAttributeInputsToRequestTemplate(ICSharpFileBuilderTemplate requestTemplate)
        {
            requestTemplate.CSharpFile.OnBuild(file =>
            {
                foreach (var parameter in Model.Parameters)
                {
                    if (file.Classes.First().TryGetReferenceForModel(parameter, out var reference)
                        && reference is CSharpProperty property)
                    {
                        AddHttpInputAttributesToProperty(property, parameter);
                    }
                }
            });
        }

        private ApiVersionModel? Versions { get; set; }

        public override void AfterTemplateRegistration()
        {
            if (HasSwashbuckleInstalled())
            {
                AddNugetDependency(NugetPackages.FastEndpointsSwaggerSwashbuckle(OutputTarget));
            }
        }

        public CSharpStatement? GetReturnStatement()
        {
            return this.GetReturnStatement(Model);
        }

        private IElement? TryGetRequestTemplate(IEndpointModel model)
        {
            if ((model.InternalElement.SpecializationType == "Command" ||
                 model.InternalElement.SpecializationType == "Query") &&
                model.Parameters.Any())
            {
                return model.InternalElement;
            }
            else
            {
                var payloadParameters = model.Parameters
                    .Where(p => p.TypeReference.Element.IsDTOModel())
                    .Select(s => (IElement)s.TypeReference.Element)
                    .ToArray();
                if (payloadParameters.Length > 1)
                {
                    throw new ElementException(model.InternalElement, "This service cannot have more than one DTO payload.");
                }
                return payloadParameters.FirstOrDefault();
            }
        }

        private CSharpClass CreateInlineRequestModelClass()
        {
            var @class = new CSharpClass($"{Model.Name.RemoveSuffix("Endpoint")}RequestModel", CSharpFile);
            CSharpFile.OnBuild(file =>
            {
                foreach (var parameter in Model.Parameters)
                {
                    @class.AddProperty(GetTypeName(parameter.TypeReference), parameter.Name.ToPropertyName(), prop =>
                    {
                        AddHttpInputAttributesToProperty(prop, parameter);
                    });
                }
                file.TypeDeclarations.Add(@class);
            }, 1);
            return @class;
        }

        private string? GetReturnType()
        {
            if (this.ShouldBeJsonResponseWrapped(Model))
            {
                return $"{this.GetJsonResponseTemplateName()}<{GetTypeName(Model)}>";
            }

            return Model.ReturnType is not null ? GetTypeName(Model.ReturnType) : null;
        }

        private void DefineEndpointBaseType(CSharpClass @class, string? requestType)
        {
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
                .AddArgument($@"""{Model.Route}""".Replace("{version}", "v{version:apiVersion}")));
        }

        private void AddDescriptionConfiguration(CSharpClassMethod method, string? requestModelName)
        {
            var operationId = (Model.InternalElement.GetStereotype("OpenAPI Settings")?.GetProperty<string>("OperationId") ?? string.Empty)
                .Replace("{ServiceName}", Model.Container.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper) ?? string.Empty)
                .Replace("{MethodName}", Model.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper));

            AddUsing("Microsoft.AspNetCore.Builder");

            var lambda = new CSharpLambdaBlock("b");
            if (!string.IsNullOrWhiteSpace(operationId))
            {
                lambda.AddInvocationStatement("b.WithName", name => name.AddArgument($@"""{operationId}"""));
            }

            lambda.AddInvocationStatement("b.WithTags", i => i.AddArgument($@"""{Model.Container.Name}"""));

            if (!string.IsNullOrWhiteSpace(Model.Comment))
            {
                lambda.AddInvocationStatement("b.WithSummary", i => i.AddArgument($@"@""{Model.Comment}"""));
            }

            if (requestModelName is not null)
            {
                lambda.AddInvocationStatement($"b.Accepts<{requestModelName}>", i =>
                {
                    if (Model.Parameters.Any(x => x.Source is null or HttpInputSource.FromBody
                                                  && Model.Route?.Contains($"{{{x.Name}}}", StringComparison.OrdinalIgnoreCase) != true))
                    {
                        AddUsing("System.Net.Mime");
                        i.AddArgument("MediaTypeNames.Application.Json");
                    }
                });
            }

            var mediaTypeNamesApplicationJson = Model.MediaType == HttpMediaType.ApplicationJson || Model.ReturnType?.Element.IsDTOModel() == true
                ? "contentType: MediaTypeNames.Application.Json"
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

        private void AddValidator(CSharpClassMethod method, IElement? payloadModel)
        {
            if (payloadModel is null)
            {
                return;
            }

            var validatorTemplate = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>("Application.Validation.Dto", payloadModel);
            if (validatorTemplate is null)
            {
                return;
            }

            method.AddStatement($"Validator<{GetTypeName(validatorTemplate)}>();");
        }

        private void AddSecurity(CSharpClassMethod method)
        {
            if (!IsEndpointSecured())
            {
                method.AddStatement("AllowAnonymous();");
            }

            if (!string.IsNullOrWhiteSpace(Model.Authorization?.RolesExpression) &&
                Model.Authorization.RolesExpression.Contains('+'))
            {
                var roles = Model.Authorization.RolesExpression.Split('+');

                foreach (var roleGroup in roles)
                {
                    var individualRoles = roleGroup.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    method.AddStatement($@"Roles({string.Join(", ", individualRoles.Select(s => $@"""{s}"""))});");
                }
            }

            if (!string.IsNullOrWhiteSpace(Model.Authorization?.Policy))
            {
                method.AddStatement($@"Policies(""{Model.Authorization.Policy}"");");
            }
        }

        private void AddVersioning(CSharpClassMethod method)
        {
            if (Versions is null)
            {
                return;
            }

            AddUsing("FastEndpoints.AspVersioning");
            AddUsing("Asp.Versioning");
            AddNugetDependency(NugetPackages.FastEndpointsAspVersioning(OutputTarget));

            CSharpStatement versionSet = new CSharpInvocationStatement("x.WithVersionSet")
                .AddArgument(@""">>Api Version<<""")
                .WithoutSemicolon();

            var versions = Model.ApplicableVersions.Any() ? Model.ApplicableVersions : [new EndpointApiVersionModel("", "V1.0", false)];

            foreach (var versionModel in versions)
            {
                versionSet = versionSet.AddInvocation($"MapToApiVersion", inv => inv.AddArgument($@"new ApiVersion({versionModel.Version.Replace("V", "")})").WithoutSemicolon());
            }

            method.AddInvocationStatement("Options",
                inv => inv.AddArgument(new CSharpLambdaBlock("x").WithExpressionBody(versionSet)));
        }

        private void AddHttpInputAttributesToProperty(CSharpProperty property, IEndpointParameterModel parameter)
        {
            if (parameter.Source is HttpInputSource.FromHeader)
            {
                property.File.Template.AddNugetDependency(NugetPackages.FastEndpointsAttributes(property.File.Template.OutputTarget));
                property.AddAttribute(property.File.Template.UseType($"FastEndpoints.FromHeader"), attr => attr.AddArgument($@"""{parameter.HeaderName}"""));
            }

            if (parameter.Source is null or HttpInputSource.FromQuery)
            {
                property.File.Template.AddNugetDependency(NugetPackages.FastEndpointsAttributes(property.File.Template.OutputTarget));
                property.AddAttribute(property.File.Template.UseType("FastEndpoints.FromQueryParams"), attr =>
                {
                    if (!string.IsNullOrWhiteSpace(parameter.QueryStringName))
                    {
                        attr.AddArgument($@"""{parameter.QueryStringName}""");
                    }
                });
            }
        }

        private bool HasSwashbuckleInstalled()
        {
            return ExecutionContext.FindTemplateInstances("Distribution.SwashbuckleConfiguration")?.Any() == true;
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

        private bool IsContainerSecured()
        {
            // Still to be applied
            // return ExecutionContext.Settings.GetAPISettings().DefaultAPISecurity().AsEnum() switch
            // {
            //     APISettings.DefaultAPISecurityOptionsEnum.Secured => Model.RequiresAuthorization || !Model.AllowAnonymous,
            //     APISettings.DefaultAPISecurityOptionsEnum.Unsecured => Model.RequiresAuthorization,
            //     _ => throw new ArgumentOutOfRangeException()
            // };
            return Model.Container.RequiresAuthorization || !Model.Container.AllowAnonymous;
        }

        private bool IsEndpointSecured()
        {
            if (!Model.RequiresAuthorization && !Model.AllowAnonymous)
            {
                return IsContainerSecured();
            }

            return IsContainerSecured()
                ? Model.RequiresAuthorization || !Model.AllowAnonymous
                : Model.RequiresAuthorization;
        }
    }
}

internal static class EndpointUtilExtensions
{
    public static CSharpInvocationStatement AddArgumentIfNotNull(
        this CSharpInvocationStatement statement,
        CSharpStatement? argument,
        Action<CSharpStatement>? configure = null)
    {
        if (argument is null)
        {
            return statement;
        }

        return statement.AddArgument(argument);
    }
}