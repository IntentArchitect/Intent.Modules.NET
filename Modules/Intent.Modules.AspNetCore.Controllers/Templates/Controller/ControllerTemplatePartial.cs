using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class ControllerTemplate : CSharpTemplateBase<IControllerModel, ControllerDecorator>, ICSharpFileBuilderTemplate, IControllerTemplate<IControllerModel>
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.AspNetCore.Controllers.Controller";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ControllerTemplate(IOutputTarget outputTarget, IControllerModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource("Domain.Enum");
            AddTypeSource("Application.Contract.Dto");
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Authorization")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
                .AddClass($"{Model.Name.RemoveSuffix("Controller", "Service")}Controller", @class =>
                {
                    @class.AddAttribute("[ApiController]");
                    @class.WithBaseType("ControllerBase");
                    @class.AddConstructor();
                    @class.AddMetadata("model", Model);
                    @class.AddMetadata("modelId", Model.Id);
                    foreach (var attribute in GetControllerAttributes())
                    {
                        @class.AddAttribute(attribute);
                    }
                    foreach (var operation in Model.Operations)
                    {
                        var isFileUpload = FileTransferHelper.IsFileUploadOperation(operation);
                        @class.AddMethod($"Task<{GetReturnType(operation)}>", operation.Name.ToPascalCase(), method =>
                        {
                            method.AddMetadata("model", operation);
                            method.AddMetadata("modelId", operation.Id);
                            method.AddMetadata("route", operation.Route);
                            method.Async();
                            method.WithComments(GetOperationComments(operation));
                            if (isFileUpload)
                            {
                                method.AddAttribute(this.GetBinaryContentAttributeName().RemoveSuffix("Attribute"));
                            }
                            foreach (var attribute in GetOperationAttributes(operation))
                            {
                                method.AddAttribute(attribute);
                            }
                            foreach (var parameter in operation.Parameters)
                            {
                                if (isFileUpload && parameter.Source == HttpInputSource.FromBody || FileTransferHelper.IsStreamType( parameter.TypeReference))
                                {
                                    continue;
                                }

                                method.AddParameter(GetTypeName(parameter), parameter.Name.ToParameterName(), param =>
                                {
                                    param.AddMetadata("model", parameter);
                                    param.AddMetadata("modelId", parameter.Id);
                                    param.AddMetadata("mappedPayloadProperty", parameter.MappedPayloadProperty);

                                    param.WithDefaultValue(parameter.Value);
                                    var attr = GetParameterBindingAttribute(parameter);
                                    if (!string.IsNullOrWhiteSpace(attr))
                                    {
                                        param.AddAttribute(attr);
                                    }
                                });
                            }

                            method.AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"));
                        });
                    }
                });
        }


        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name.RemoveSuffix("Controller", "Service")}Controller",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: this.GetFolderPath());
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private IEnumerable<CSharpAttribute> GetControllerAttributes()
        {
            var attributes = new List<CSharpAttribute>();

            if (IsControllerSecured())
            {
                attributes.AddRange(GetAuthorizationAttributes(Model.AuthorizationModel));
            }
            else if (Model.AllowAnonymous)
            {
                attributes.Add(new CSharpAttribute("[AllowAnonymous]"));
            }

            if (Model.Route != null)
            {
                attributes.Add(new CSharpAttribute($"[Route(\"{Model.Route}\")]"));
            }

            return attributes;
        }

        private IEnumerable<string> GetOperationComments(IControllerOperationModel operation)
        {
            var lines = new List<string>
            {
                "/// <summary>"
            };

            if (!string.IsNullOrWhiteSpace(operation.Comment))
            {
                lines.AddRange(operation.Comment
                    .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(commentLine => $"/// {commentLine}"));
            }

            string returnType = FileTransferHelper.IsFileDownloadOperation(operation) ? "byte[]" : operation.ReturnType != null ? GetTypeName(operation.ReturnType) : "";
            lines.Add("/// </summary>");
            switch (operation.Verb)
            {
                case HttpVerb.Get:
                    lines.Add($"/// <response code=\"200\">Returns the specified {returnType.Replace("<", "&lt;").Replace(">", "&gt;")}.</response>");
                    break;
                case HttpVerb.Post:
                    lines.Add("/// <response code=\"201\">Successfully created.</response>");
                    break;
                case HttpVerb.Patch:
                case HttpVerb.Put:
                    lines.Add($"/// <response code=\"{(operation.ReturnType != null ? "200" : "204")}\">Successfully updated.</response>");
                    break;
                case HttpVerb.Delete:
                    lines.Add("/// <response code=\"200\">Successfully deleted.</response>");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (operation.Parameters.Any())
            {
                lines.Add("/// <response code=\"400\">One or more validation errors have occurred.</response>");
            }

            if (IsOperationSecured(operation))
            {
                lines.Add("/// <response code=\"401\">Unauthorized request.</response>");
                lines.Add("/// <response code=\"403\">Forbidden request.</response>");
            }

            if (operation.CanReturnNotFound())
            {
                var responseText = operation.ReturnType == null ||
                                   operation.ReturnType.HasStringType() ||
                                   GetTypeInfo(operation.ReturnType).IsPrimitive
                    ? "One or more entities could not be found with the provided parameters."
                    : $"No {returnType.Replace("<", "&lt;").Replace(">", "&gt;")} could be found with the provided parameters.";

                lines.Add($"/// <response code=\"404\">{responseText}</response>");
            }

            return lines;
        }

        public CSharpStatement GetReturnStatement(IControllerOperationModel operationModel)
        {
            return Utils.GetReturnStatement(this, operationModel);
        }


        private IEnumerable<CSharpAttribute> GetOperationAttributes(IControllerOperationModel operation)
        {
            var attributes = new List<CSharpAttribute>
            {
                GetHttpVerbAndPath(operation)
            };

            if (operation.RequiresAuthorization || operation.AllowAnonymous)
            {
                if ((!IsControllerSecured() && IsOperationSecured(operation)) 
                    || !string.IsNullOrWhiteSpace(operation.AuthorizationModel?.RolesExpression)
                    || !string.IsNullOrWhiteSpace(operation.AuthorizationModel?.Policy))
                {
                    attributes.AddRange(GetAuthorizationAttributes(operation.AuthorizationModel));
                }
                else if (IsControllerSecured() &&
                         !IsOperationSecured(operation) &&
                         operation.AllowAnonymous)
                {
                    attributes.Add(new CSharpAttribute("[AllowAnonymous]"));
                }
            }

            var apiResponse = operation.ReturnType != null ? $"typeof({UseType(GetTypeInfo(operation).WithIsNullable(false))}), " : string.Empty;
            if (FileTransferHelper.IsFileDownloadOperation(operation))
            {
                apiResponse = "typeof(byte[]), ";
            }
            if (operation.MediaType == HttpMediaType.ApplicationJson && operation.ReturnType != null)
            {
                // Need this because adding contentType to ProducesResponseType doesn't work - ongoing issue with Swashbuckle:
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2320
                attributes.Add(new CSharpAttribute($"[Produces({UseType("System.Net.Mime.MediaTypeNames")}.Application.Json)]"));
                if (GetTypeInfo(operation.ReturnType).IsPrimitive || operation.ReturnType.HasStringType())
                {
                    apiResponse = $"typeof({this.GetJsonResponseName()}<{GetTypeName(operation)}>), ";
                }
            }

            switch (operation.Verb)
            {
                case HttpVerb.Get:
                    attributes.Add(new CSharpAttribute($"[ProducesResponseType({apiResponse}StatusCodes.Status200OK)]"));
                    break;
                case HttpVerb.Post:
                    attributes.Add(new CSharpAttribute($"[ProducesResponseType({apiResponse}StatusCodes.Status201Created)]"));
                    break;
                case HttpVerb.Put:
                case HttpVerb.Patch:
                    attributes.Add(new CSharpAttribute(operation.ReturnType != null
                        ? $"[ProducesResponseType({apiResponse}StatusCodes.Status200OK)]"
                        : "[ProducesResponseType(StatusCodes.Status204NoContent)]"));
                    break;
                case HttpVerb.Delete:
                    attributes.Add(new CSharpAttribute($"[ProducesResponseType({apiResponse}StatusCodes.Status200OK)]"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (operation.Parameters.Any())
            {
                attributes.Add(new CSharpAttribute("[ProducesResponseType(StatusCodes.Status400BadRequest)]"));
            }

            if (IsOperationSecured(operation))
            {
                attributes.Add(new CSharpAttribute("[ProducesResponseType(StatusCodes.Status401Unauthorized)]"));
                attributes.Add(new CSharpAttribute("[ProducesResponseType(StatusCodes.Status403Forbidden)]"));
            }

            if (operation.CanReturnNotFound())
            {
                attributes.Add(new CSharpAttribute("[ProducesResponseType(StatusCodes.Status404NotFound)]"));
            }

            attributes.Add(new CSharpAttribute("[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]"));
            return attributes;
        }

        private static IEnumerable<CSharpAttribute> GetAuthorizationAttributes(IAuthorizationModel authorizationModel)
        {
            var result = new List<CSharpAttribute>();
            var attribute = new CSharpAttribute("Authorize");

            if (!string.IsNullOrWhiteSpace(authorizationModel?.RolesExpression))
            {
                if (authorizationModel.RolesExpression.Contains("+"))
                {
                    var roles = authorizationModel.RolesExpression.Split('+');

                    foreach (var roleGroup in roles)
                    {
                        attribute = new CSharpAttribute("Authorize");
                        attribute.AddArgument($"Roles = \"{roleGroup}\"");
                        result.Add(attribute);
                    }
                    return result;
                }
                attribute.AddArgument($"Roles = \"{authorizationModel.RolesExpression}\"");
            }

            if (!string.IsNullOrWhiteSpace(authorizationModel?.Policy))
            {
                attribute.AddArgument($"Policy = {authorizationModel.Policy}");
            }

#pragma warning disable CS0618 // Type or member is obsolete
            if (!string.IsNullOrWhiteSpace(authorizationModel?.AuthenticationSchemesExpression))
            {
                attribute.AddArgument($"AuthenticationSchemes = {authorizationModel.AuthenticationSchemesExpression}");
            }
#pragma warning restore CS0618 // Type or member is obsolete

            result.Add(attribute);
            return result;
        }

        private bool IsControllerSecured()
        {
            return ExecutionContext.Settings.GetAPISettings().DefaultAPISecurity().AsEnum() switch
            {
                APISettings.DefaultAPISecurityOptionsEnum.Secured => Model.RequiresAuthorization || !Model.AllowAnonymous,
                APISettings.DefaultAPISecurityOptionsEnum.Unsecured => Model.RequiresAuthorization,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool IsOperationSecured(IControllerOperationModel operation)
        {
            if (!operation.RequiresAuthorization && !operation.AllowAnonymous)
            {
                return IsControllerSecured();
            }

            return IsControllerSecured()
                ? operation.RequiresAuthorization || !operation.AllowAnonymous
                : operation.RequiresAuthorization;
        }

        private CSharpAttribute GetHttpVerbAndPath(IControllerOperationModel o)
        {
            var arguments = new List<string>();

            if (GetPath(o) != null)
            {
                arguments.Add($"\"{GetPath(o)}\"");
            }

            var openApiSettings = o.InternalElement.GetStereotype("OpenAPI Settings");
            if (openApiSettings != null)
            {
                var operationId = openApiSettings.GetProperty<string>("OperationId");
                arguments.Add(string.IsNullOrWhiteSpace(operationId)
                    ? $"Name = \"{o.Name}\""
                    : $"Name = \"{operationId}\"");
            }

            var joinedArguments = arguments.Any()
                ? $"({string.Join(", ", arguments)})"
                : string.Empty;

            return new CSharpAttribute(
                $"[Http{o.Verb.ToString().ToLower().ToPascalCase()}{joinedArguments}]");
        }

        private string GetReturnType(IControllerOperationModel operation)
        {

            if (FileTransferHelper.IsFileDownloadOperation(operation))
            {
                return "ActionResult<byte[]>";
            }

            if (operation.ReturnType == null)
            {
                return "ActionResult";
            }

            var returnTypeName = GetTypeName(operation.TypeReference);
            if (this.ShouldBeJsonResponseWrapped(operation))
            {
                returnTypeName = $"{this.GetJsonResponseName()}<{returnTypeName}>";
            }

            return $"ActionResult<{returnTypeName}>";
        }

        private static string GetPath(IControllerOperationModel operation)
        {
            var path = operation.Route;
            return !string.IsNullOrWhiteSpace(path) ? path : null;
        }

        private static string GetParameterBindingAttribute(IControllerParameterModel parameter)
        {
            return parameter.Source switch
            {
                HttpInputSource.FromBody => "[FromBody]",
                HttpInputSource.FromForm => "[FromForm]",
                HttpInputSource.FromHeader => $"[FromHeader(Name = \"{parameter.HeaderName ?? parameter.Name}\")]",
                HttpInputSource.FromQuery => $"[FromQuery{(!string.IsNullOrWhiteSpace(parameter.QueryStringName) ? $"(Name = \"{parameter.QueryStringName}\")" : string.Empty)}]",
                HttpInputSource.FromRoute => "[FromRoute]",
                _ => string.Empty
            };
        }
    }
}