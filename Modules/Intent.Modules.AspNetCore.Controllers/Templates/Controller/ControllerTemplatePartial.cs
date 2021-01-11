using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AspNetCore.Controllers.Api;
using Intent.Engine;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using Enum = System.Enum;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ControllerTemplate : CSharpTemplateBase<ServiceModel, ControllerDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Controllers.Controller";

        public ControllerTemplate(IOutputTarget outputTarget, ServiceModel model) : base(TemplateId, outputTarget, model)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name.RemoveSuffix("Controller", "Service")}Controller",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public string GetEnterClass()
        {
            return GetDecorators().Aggregate(x => x.EnterClass());
        }

        public string GetExitClass()
        {
            return GetDecorators().Aggregate(x => x.ExitClass());
        }

        public string GetEnterOperationBody(OperationModel o)
        {
            return GetDecorators().Aggregate(x => x.EnterOperationBody(o));
        }

        public string GetMidOperationBody(OperationModel o)
        {
            return GetDecorators().Aggregate(x => x.MidOperationBody(o));
        }

        public string GetExitOperationBody(OperationModel o)
        {
            return GetDecorators().Aggregate(x => x.ExitOperationBody(o));
        }

        private string GetControllerBase()
        {
            return GetDecorators().Select(x => x.BaseClass()).SingleOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? "ControllerBase";
        }

        private string GetControllerAttributes()
        {
            var attributes = new List<string>();
            if (Model.HasSecured())
            {
                attributes.Add("[Authorize]");
            }
            attributes.Add($@"[Route(""{(string.IsNullOrWhiteSpace(Model.GetHttpServiceSettings().Route()) ? "api/[controller]" : Model.GetHttpServiceSettings().Route())}"")]");
            return string.Join(@"
    ", attributes);
        }

        private string GetOperationAttributes(OperationModel o)
        {
            var attributes = new List<string>();
            attributes.Add(GetHttpVerbAndPath(o));
            if (o.ParentService.HasSecured() != o.HasSecured())
            {
                attributes.Add(o.HasSecured() ? "[Authorize]" : "[AllowAnonymous]");
            }
            return string.Join(@"
        ", attributes);
        }

        private string GetHttpVerbAndPath(OperationModel o)
        {
            return $"[Http{GetHttpVerb(o).ToString().ToLower().ToPascalCase()}{(GetPath(o) != null ? $"(\"{GetPath(o)}\")" : "")}]";
        }

        private string GetOperationParameters(OperationModel operation)
        {
            if (!operation.Parameters.Any())
            {
                return string.Empty;
            }
            var verb = GetHttpVerb(operation);
            switch (verb)
            {
                case HttpVerb.POST:
                case HttpVerb.PUT:
                    return operation.Parameters.Select(x => $"{GetParameterBindingAttribute(operation, x)}{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => $"{x}, {y}");
                case HttpVerb.GET:
                case HttpVerb.DELETE:
                    if (operation.Parameters.Any(x => x.TypeReference.Element.SpecializationTypeId != TypeDefinitionModel.SpecializationTypeId &&
                                                      x.GetParameterSettings().Source().IsDefault()))
                    {
                        Logging.Log.Warning($@"Intent.AspNetCore.Controllers: [{Model.Name}.{operation.Name}] Passing objects into HTTP {GetHttpVerb(operation)} operations is not well supported by this module.
    We recommend using a POST or PUT verb");
                        // Log warning
                    }
                    return operation.Parameters.Select(x => $"{GetParameterBindingAttribute(operation, x)}{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => x + ", " + y);
                default:
                    throw new NotSupportedException($"{verb} not supported");
            }
        }

        private string GetReturnType(OperationModel operation)
        {
            if (operation.ReturnType == null)
            {
                return "ActionResult";
            }
            return $"ActionResult<{GetTypeName(operation.TypeReference)}>";
        }

        private string GetPath(OperationModel operation)
        {
            var path = operation.GetHttpSettings().Route();
            return !string.IsNullOrWhiteSpace(path) ? path : null;
        }

        private string GetParameterBindingAttribute(OperationModel operation, ParameterModel parameter)
        {
            if (parameter.GetParameterSettings().Source().IsDefault())
            {
                return "";
            }

            if (parameter.GetParameterSettings().Source().IsFromBody())
            {
                return "[FromBody]";
            }
            if (parameter.GetParameterSettings().Source().IsFromHeader())
            {
                return "[FromHeader]";
            }
            if (parameter.GetParameterSettings().Source().IsFromQuery())
            {
                return "[FromQuery]";
            }
            if (parameter.GetParameterSettings().Source().IsFromRoute())
            {
                return "[FromRoute]";
            }

            return "";
        }

        private string GetConstructorParameters()
        {
            return string.Join(@",
            ", GetDecorators().SelectMany(x => x.ConstructorParameters()));
        }

        private string GetConstructorImplementation()
        {
            return GetDecorators().Aggregate(x => x.ConstructorImplementation());
        }

        private HttpVerb GetHttpVerb(OperationModel operation)
        {
            var verb = operation.GetHttpSettings().Verb();
            return Enum.TryParse(verb.Value, out HttpVerb verbEnum) ? verbEnum : HttpVerb.POST;
            //if (!verb.IsAUTO())
            //{
            //    return Enum.TryParse(verb.Value, out HttpVerb verbEnum) ? verbEnum : HttpVerb.POST;
            //}
            //if (operation.ReturnType == null || operation.Parameters.Any(x => !Types.Get(x.TypeReference).IsPrimitive))
            //{
            //    var hasIdParam = operation.Parameters.Any(x => x.Name.ToLower().EndsWith("id") && Types.Get(x.TypeReference).IsPrimitive);
            //    if (hasIdParam && new[] { "delete", "remove" }.Any(x => operation.Name.ToLower().Contains(x)))
            //    {
            //        return HttpVerb.DELETE;
            //    }
            //    return hasIdParam ? HttpVerb.PUT : HttpVerb.POST;
            //}
            //return HttpVerb.GET;
        }

        public enum HttpVerb
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}