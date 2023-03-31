using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.WebApi.Api;
using Intent.Modules.AspNetCore.Controllers.Settings;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class ControllerTemplate : CSharpTemplateBase<IControllerModel, ControllerDecorator>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.AspNetCore.Controllers.Controller";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ControllerTemplate(IOutputTarget outputTarget, IControllerModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource("Domain.Enum");
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Authorization")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .AddClass($"{Model.Name.RemoveSuffix("Controller", "Service")}Controller", @class =>
                {
                    @class.AddAttribute("[ApiController]");
                    @class.WithBaseType("ControllerBase");
                    @class.AddConstructor();
                    foreach (var attribute in GetControllerAttributes())
                    {
                        @class.AddAttribute(attribute);
                    }
                    foreach (var operation in Model.Operations)
                    {
                        @class.AddMethod($"Task<{GetReturnType(operation)}>", operation.Name.ToPascalCase(), method =>
                        {
                            method.AddMetadata("model", operation);
                            method.Async();
                            method.WithComments(GetOperationComments(operation));
                            foreach (var attribute in GetOperationAttributes(operation))
                            {
                                method.AddAttribute(attribute);
                            }
                            foreach (var parameter in operation.Parameters)
                            {
                                method.AddParameter(GetTypeName(parameter), parameter.Name.ToCamelCase(), param =>
                                {
                                    var attr = GetParameterBindingAttribute(operation, parameter);
                                    if (!string.IsNullOrWhiteSpace(attr))
                                    {
                                        param.AddAttribute(attr);
                                    }
                                });
                            }

                            method.AddParameter("CancellationToken", "cancellationToken");
                        });
                    }
                });
        }
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
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

        //public HttpVerb GetHttpVerb(IControllerOperationModel operation)
        //{
        //    var verb = operation.GetHttpSettings().Verb();

        //    return Enum.TryParse(verb.Value, ignoreCase: true, out HttpVerb verbEnum) ? verbEnum : HttpVerb.Post;
        //}

        private IEnumerable<string> GetControllerAttributes()
        {
            var attributes = new List<string>();
            if (IsControllerSecured())
            {
                attributes.Add(GetAuthorizationAttribute(Model.AuthorizationModel));
            }
            else if (Model.AllowAnonymous)
            {
                attributes.Add("[AllowAnonymous]");
            }

            if (Model.Route != null)
            {
                attributes.Add($@"[Route(""{Model.Route}"")]");
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

            lines.Add("/// </summary>");
            switch (operation.Verb)
            {
                case HttpVerb.Get:
                    lines.Add($"/// <response code=\"200\">Returns the specified {GetTypeName(operation.ReturnType).Replace("<", "&lt;").Replace(">", "&gt;")}.</response>");
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

            if (operation.Verb == HttpVerb.Get && operation.ReturnType?.IsCollection == false)
            {
                lines.Add($"/// <response code=\"404\">Can't find an {GetTypeName(operation.ReturnType).Replace("<", "&lt;").Replace(">", "&gt;")} with the parameters provided.</response>");
            }

            return lines;
        }

        private IEnumerable<string> GetOperationAttributes(IControllerOperationModel operation)
        {
            var attributes = new List<string> { GetHttpVerbAndPath(operation) };
            if (operation.RequiresAuthorization || operation.AllowAnonymous)
            {
                if ((!IsControllerSecured() && IsOperationSecured(operation)) ||
                    operation.AuthorizationModel != null)
                {
                    attributes.Add(GetAuthorizationAttribute(operation.AuthorizationModel));
                }
                else if (IsControllerSecured() &&
                         !IsOperationSecured(operation) &&
                         operation.AllowAnonymous)
                {
                    attributes.Add("[AllowAnonymous]");
                }
            }

            var apiResponse = operation.ReturnType != null ? $"typeof({GetTypeName(operation)}), " : string.Empty;
				operation.ReturnType != null)
            if (operation.MediaType == MediaTypeOptions.ApplicationJson && operation.ReturnType != null)
            {
                // Need this because adding contentType to ProducesResponseType doesn't work - ongoing issue with Swashbuckle:
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2320
                attributes.Add($@"[Produces({UseType("System.Net.Mime.MediaTypeNames")}.Application.Json)]");
                if (GetTypeInfo(operation.ReturnType).IsPrimitive || operation.ReturnType.HasStringType())
                {
                    apiResponse = $"typeof({this.GetJsonResponseName()}<{GetTypeName(operation)}>), ";
                }
            }

            switch (operation.Verb)
            {
                case HttpVerb.Get:
                    attributes.Add($@"[ProducesResponseType({apiResponse}StatusCodes.Status200OK)]");
                    break;
                case HttpVerb.Post:
                    attributes.Add($@"[ProducesResponseType({apiResponse}StatusCodes.Status201Created)]");
                    break;
                case HttpVerb.Put:
                case HttpVerb.Patch:
                    attributes.Add(operation.ReturnType != null
                        ? $@"[ProducesResponseType({apiResponse}StatusCodes.Status200OK)]"
                        : @"[ProducesResponseType(StatusCodes.Status204NoContent)]");
                    break;
                case HttpVerb.Delete:
                    attributes.Add($@"[ProducesResponseType({apiResponse}StatusCodes.Status200OK)]");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (operation.Parameters.Any())
            {
                attributes.Add(@"[ProducesResponseType(StatusCodes.Status400BadRequest)]");
            }

            if (IsOperationSecured(operation))
            {
                attributes.Add(@"[ProducesResponseType(StatusCodes.Status401Unauthorized)]");
                attributes.Add(@"[ProducesResponseType(StatusCodes.Status403Forbidden)]");
            }

            if (operation.Verb == HttpVerb.Get && operation.ReturnType?.IsCollection == false)
            {
                attributes.Add(@"[ProducesResponseType(StatusCodes.Status404NotFound)]");
            }

            attributes.Add(@"[ProducesResponseType(StatusCodes.Status500InternalServerError)]");
            return attributes;
        }

        private static string GetAuthorizationAttribute(IAuthorizationModel authorizationModel)
        {
            var fieldExpressions = new List<string>();

            if (!string.IsNullOrWhiteSpace(authorizationModel?.RolesExpression))
            {
                fieldExpressions.Add($"Roles = {authorizationModel.RolesExpression}");
            }

            if (!string.IsNullOrWhiteSpace(authorizationModel?.Policy))
            {
                fieldExpressions.Add($"Policy = {authorizationModel.Policy}");
            }

            if (!string.IsNullOrWhiteSpace(authorizationModel?.AuthenticationSchemesExpression))
            {
                fieldExpressions.Add($"AuthenticationSchemes = {authorizationModel.AuthenticationSchemesExpression}");
            }

            if (fieldExpressions.Any())
            {
                return $"[Authorize ({string.Join(", ", fieldExpressions)})]";
            }

            return "[Authorize]";
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

        private string GetHttpVerbAndPath(IControllerOperationModel o)
        {
            return
                $"[Http{o.Verb.ToString().ToLower().ToPascalCase()}{(GetPath(o) != null ? $"(\"{GetPath(o)}\")" : "")}]";
        }

        private string GetReturnType(IControllerOperationModel operation)
        {
            return operation.ReturnType == null
                ? "ActionResult"
                : $"ActionResult<{GetTypeName(operation.TypeReference)}>";
        }

        private static string GetPath(IControllerOperationModel operation)
        {
            var path = operation.Route;
            return !string.IsNullOrWhiteSpace(path) ? path : null;
        }

        private static string GetParameterBindingAttribute(IControllerOperationModel operation, IControllerParameterModel parameter)
        {
            if (parameter.Source == SourceOptionsEnum.Default)
            {
                if (parameter.TypeReference.Element.IsTypeDefinitionModel() &&
                    operation.Route?.Contains($"{{{parameter.Name}}}") == true)
                {
                    return "[FromRoute]";
                }

                if ((operation.Verb == HttpVerb.Get || operation.Verb == HttpVerb.Delete) &&
                    !parameter.TypeReference.Element.IsTypeDefinitionModel())
                {
                    return "[FromQuery]";
                }

                if ((operation.Verb == HttpVerb.Post || operation.Verb == HttpVerb.Put) &&
                    !parameter.TypeReference.Element.IsTypeDefinitionModel())
                {
                    return "[FromBody]";
                }

                return string.Empty;
            }

            return parameter.Source switch
            {
                SourceOptionsEnum.FromBody => "[FromBody]",
                SourceOptionsEnum.FromForm => "[FromForm]",
                SourceOptionsEnum.FromHeader => $@"[FromHeader(Name = ""{parameter.HeaderName ?? parameter.Name}"")]",
                SourceOptionsEnum.FromQuery => "[FromQuery]",
                SourceOptionsEnum.FromRoute => "[FromRoute]",
                _ => string.Empty
            };
        }

    }
}