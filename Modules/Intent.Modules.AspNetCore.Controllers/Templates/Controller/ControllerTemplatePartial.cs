using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.Security.Models;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Modules.Security.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class ControllerTemplate : CSharpTemplateBase<IControllerModel>, ICSharpFileBuilderTemplate, IControllerTemplate<IControllerModel>
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.AspNetCore.Controllers.Controller";
        private readonly bool _isIgnoredForApiExplorer;

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ControllerTemplate(IOutputTarget outputTarget, IControllerModel model = null) : base(TemplateId, outputTarget, model)
        {
            _isIgnoredForApiExplorer =
                (Model.InternalElement?.TryGetIsIgnoredForApiExplorer(out var isIgnoredForApiExplorer) == true && isIgnoredForApiExplorer) ||
                Model.Operations.All(x => x.InternalElement.TryGetIsIgnoredForApiExplorer(out var operationIsIgnored) && operationIsIgnored);

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
                                if (operation.Verb == HttpVerb.Get && parameter.TypeReference is { IsCollection: true } && parameter.TypeReference.Element.IsDTOModel())
                                {
                                    throw new ElementException(parameter.MappedPayloadProperty as IElement ?? operation.InternalElement, "GET Operations do not support collections on complex objects.");
                                }
                                if (isFileUpload && (parameter.Source == HttpInputSource.FromBody || FileTransferHelper.IsStreamType(parameter.TypeReference) || string.Equals("filename", parameter.Name, StringComparison.OrdinalIgnoreCase)))
                                {
                                    continue;
                                }

                                // If you change anything here, please check also: WorkaroundForGetTypeNameIssue()
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

                            if (this.TryGetMultiTenancyRoute(operation, out var _) && operation.Verb == HttpVerb.Post)
                            {
                                method.AddParameter(UseType("Finbuckle.MultiTenant.ITenantInfo"), "tenantInfo");
                            }

                            method.AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"));
                        });
                    }
                })
                .AfterBuild(WorkaroundForGetTypeNameIssue, 1000);
        }

        // Due to the nature of how GetTypeName resolves namespaces
        // there are cases where ambiguous references still exist
        // and causes compilation errors, this forces to re-evaluate
        // a lot of types in this template. For example when a service
        // is calling a proxy with the same Dto names on both sides.
        private void WorkaroundForGetTypeNameIssue(CSharpFile file)
        {
            var priClass = file.Classes.First();

            foreach (var method in priClass.Methods)
            {
                var parameterTypesToReplace = method.Parameters
                    .Select((param, index) => new { Param = param, Index = index })
                    .Where(p => p.Param.HasMetadata("model"))
                    .ToArray();
                foreach (var entry in parameterTypesToReplace)
                {
                    var paramModel = entry.Param.GetMetadata<IControllerParameterModel>("model");
                    var param = new CSharpParameter(GetTypeName(paramModel.TypeReference), entry.Param.Name, method);
                    param.AddMetadata("model", paramModel);
                    param.AddMetadata("modelId", paramModel.Id);
                    param.AddMetadata("mappedPayloadProperty", paramModel.MappedPayloadProperty);
                    param.WithDefaultValue(entry.Param.DefaultValue);
                    param.WithXmlDocComment(entry.Param.XmlDocComment);
                    foreach (var attribute in entry.Param.Attributes)
                    {
                        param.Attributes.Add(attribute);
                    }
                    method.Parameters[entry.Index] = param;
                }
            }
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

        private List<CSharpAttribute> GetControllerAttributes()
        {
            var attributes = new List<CSharpAttribute>();

            if (Model.RequiresAuthorization)
            {
                attributes.AddRange(GetAuthorizationAttributes(Model.SecurityModels));
            }
            else if (Model.AllowAnonymous)
            {
                attributes.Add(new CSharpAttribute("[AllowAnonymous]"));
            }

            if (_isIgnoredForApiExplorer)
            {
                AddUsing("Microsoft.AspNetCore.Mvc");
                attributes.Add(new CSharpAttribute("[ApiExplorerSettings(IgnoreApi = true)]"));
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
                    .Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
                    .Select(commentLine => $"/// {commentLine}"));
            }

            var returnType = FileTransferHelper.IsFileDownloadOperation(operation) ? "byte[]" : operation.ReturnType != null ? GetTypeName(operation.ReturnType) : "";
            lines.Add("/// </summary>");

            foreach (var param in operation.Parameters)
            {
                if (param.MappedPayloadProperty != null &&
                    param.MappedPayloadProperty.HasStereotype("OpenAPI Settings") &&
                    !string.IsNullOrWhiteSpace(param.MappedPayloadProperty.GetStereotype("OpenAPI Settings").GetProperty("Example Value")?.Value))
                {
                    lines.Add($"/// <param name=\"{param.Name.ToParameterName()}\" example=\"{param.MappedPayloadProperty.GetStereotype("OpenAPI Settings").GetProperty("Example Value")?.Value}\"></param>");
                }
            }

            switch (operation.Verb)
            {
                case HttpVerb.Get:
                    lines.Add($"/// <response code=\"{operation.GetSuccessResponseCode("200")}\">Returns the specified {returnType.Replace("<", "&lt;").Replace(">", "&gt;")}.</response>");
                    break;
                case HttpVerb.Post:
                    lines.Add($"/// <response code=\"{operation.GetSuccessResponseCode("201")}\">Successfully created.</response>");
                    break;
                case HttpVerb.Patch:
                case HttpVerb.Put:
                    lines.Add($"/// <response code=\"{(operation.GetSuccessResponseCode(operation.ReturnType != null ? "200" : "204"))}\">Successfully updated.</response>");
                    break;
                case HttpVerb.Delete:
                    lines.Add($"/// <response code=\"{operation.GetSuccessResponseCode("200")}\">Successfully deleted.</response>");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (operation.Parameters.Any())
            {
                lines.Add("/// <response code=\"400\">One or more validation errors have occurred.</response>");
            }

            if (operation.RequiresAuthorization)
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

        private List<CSharpAttribute> GetOperationAttributes(IControllerOperationModel operation)
        {
            var attributes = new List<CSharpAttribute>
            {
                GetHttpVerbAndPath(operation)
            };

            if (operation.RequiresAuthorization)
            {
                attributes.AddRange(GetAuthorizationAttributes(operation.SecurityModels));
            }
            else if (operation.AllowAnonymous)
            {
                attributes.Add(new CSharpAttribute("[AllowAnonymous]"));
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
                    attributes.Add(new CSharpAttribute($"[ProducesResponseType({apiResponse}StatusCodes.{operation.GetSuccessResponseCodeEnumValue("Status200OK")})]"));
                    break;
                case HttpVerb.Post:
                    attributes.Add(new CSharpAttribute($"[ProducesResponseType({apiResponse}StatusCodes.{operation.GetSuccessResponseCodeEnumValue("Status201Created")})]"));
                    break;
                case HttpVerb.Put:
                case HttpVerb.Patch:
                    var defaultValue = operation.ReturnType != null ? "Status200OK" : "Status204NoContent";
                    attributes.Add(new CSharpAttribute($"[ProducesResponseType({apiResponse}StatusCodes.{operation.GetSuccessResponseCodeEnumValue(defaultValue)})]"));
                    break;
                case HttpVerb.Delete:
                    attributes.Add(new CSharpAttribute($"[ProducesResponseType({apiResponse}StatusCodes.{operation.GetSuccessResponseCodeEnumValue("Status200OK")})]"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown verb: {operation.Verb}");
            }

            if (operation.Verb is not HttpVerb.Patch && operation.CanReturnNoContent())
            {
                attributes.Add(new CSharpAttribute($"[ProducesResponseType(StatusCodes.{operation.GetSuccessResponseCodeEnumValue("Status204NoContent")})]"));
            }

            if (operation.Parameters.Any())
            {
                attributes.Add(new CSharpAttribute("[ProducesResponseType(StatusCodes.Status400BadRequest)]"));
            }

            if (operation.RequiresAuthorization)
            {
                attributes.Add(new CSharpAttribute("[ProducesResponseType(StatusCodes.Status401Unauthorized)]"));
                attributes.Add(new CSharpAttribute("[ProducesResponseType(StatusCodes.Status403Forbidden)]"));
            }

            if (TryGetIsIgnoredForApiExplorer(operation, out var isIgnoredForApiExplorer))
            {
                AddUsing("Microsoft.AspNetCore.Mvc");
                attributes.Add(new CSharpAttribute($"[ApiExplorerSettings(IgnoreApi = {isIgnoredForApiExplorer.ToString().ToLowerInvariant()})]"));
            }

            if (operation.CanReturnNotFound())
            {
                attributes.Add(new CSharpAttribute("[ProducesResponseType(StatusCodes.Status404NotFound)]"));
            }

            attributes.Add(new CSharpAttribute("[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]"));
            return attributes;
        }

        private IEnumerable<CSharpAttribute> GetAuthorizationAttributes(IReadOnlyCollection<ISecurityModel> securityModels)
        {
            return securityModels
                .Select(model =>
                {
                    var attribute = new CSharpAttribute("Authorize");

                    if (model.Roles.Count > 0)
                    {
                        attribute.AddArgument($"Roles = {SecurityHelper.RolesToPermissionConstants(model.Roles, this)}");
                    }

                    if (model.Policies.Count > 0)
                    {
                        attribute.AddArgument($"Policy = {SecurityHelper.PoliciesToPermissionConstants(model.Policies, this)}");
                    }

                    return attribute;
                });
        }
        
        private static CSharpAttribute GetHttpVerbAndPath(IControllerOperationModel o)
        {
            var arguments = new List<string>();

            if (GetPath(o) != null)
            {
                arguments.Add($"\"{GetPath(o)}\"");
            }

            var operationId = (o.InternalElement.GetStereotype("OpenAPI Settings")?.GetProperty<string>("OperationId") ?? string.Empty)
                .Replace("{ServiceName}", o.Controller.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper))
                .Replace("{MethodName}", o.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper));
            if (!string.IsNullOrWhiteSpace(operationId))
            {
                arguments.Add($"Name = \"{operationId}\"");
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

        private bool TryGetIsIgnoredForApiExplorer(IControllerOperationModel operation, out bool isIgnoredForApiExplorer)
        {
            if (!operation.InternalElement.TryGetIsIgnoredForApiExplorer(out isIgnoredForApiExplorer))
            {
                return false;
            }

            if (_isIgnoredForApiExplorer == isIgnoredForApiExplorer)
            {
                isIgnoredForApiExplorer = default;
                return false;
            }

            return true;
        }
    }
}