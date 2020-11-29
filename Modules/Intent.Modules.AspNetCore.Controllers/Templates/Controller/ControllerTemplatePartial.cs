using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AspNetCore.Controllers.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Decorators;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using Enum = System.Enum;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ControllerTemplate : CSharpTemplateBase<ServiceModel, ControllerDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "AspNetCore.Controllers.Controller";

        public ControllerTemplate(IOutputTarget outputTarget, ServiceModel model) : base(TemplateId, outputTarget, model)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name.RemoveSuffix("Controller", "Service")}Controller",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public string OnEnterOperationBody(OperationModel o)
        {
            return GetDecorators().Aggregate(x => x.OnEnterOperationBody(o));
        }

        public string OnExitOperationBody(OperationModel o)
        {
            return GetDecorators().Aggregate(x => x.OnExitOperationBody(o));
        }

        private string GetControllerBase()
        {
            return "ApiControllerBase";
        }

        private string GetControllerAttributes()
        {
            var attributes = new List<string>();
            attributes.Add(Model.GetControllerSettings().Security().IsAuthorize() ? "[Authorize]" : "[AllowAnonymous]");
            return string.Join(@"
    ", attributes);
        }

        private string GetOperationAttributes(OperationModel o)
        {
            var attributes = new List<string>();
            attributes.Add(GetHttpVerbAndPath(o));
            if (!o.GetApiSettings().Security().IsInherit())
            {
                attributes.Add(o.GetApiSettings().Security().IsAuthorize() ? "[Authorize]" : "[AllowAnonymous]");
            }
            return string.Join(@"
        ", attributes);
        }

        private string GetHttpVerbAndPath(OperationModel o)
        {
            return $"[Http{GetHttpVerb(o).ToString().ToLower().ToPascalCase()}{(GetPath(o) != null ? $"(\"{GetPath(o)}\")" : "")}]";
        }

        //private string GetSecurityAttribute(OperationModel o)
        //{
        //    if (o.HasStereotype("Secured") || Model.HasStereotype("Secured"))
        //    {
        //        var roles = o.GetStereotypeProperty<string>("Secured", "Roles");
        //        return string.IsNullOrWhiteSpace(roles)
        //            ? "[Authorize]"
        //            : $"[Authorize(Roles = \"{roles}\")]";
        //    }
        //    return "[AllowAnonymous]";
        //}

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
                    if (operation.Parameters.Any(x => x.TypeReference.Element.SpecializationType == "DTO"))
                    {
                        Logging.Log.Warning($@"Intent.AspNetCore.Controllers: [{Model.Name}.{operation.Name}] Passing objects into HTTP {GetHttpVerb(operation)} operations is not well supported by this module.
    We recommend using a POST or PUT verb");
                        // Log warning
                    }
                    return operation.Parameters.Select(x => $"{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => x + ", " + y);
                default:
                    throw new NotSupportedException($"{verb} not supported");
            }
        }

        private string GetOperationArguments(OperationModel operation)
        {
            return string.Join(", ", operation.Parameters.Select(x => x.Name));
        }

        private string GetReturnType(OperationModel operation)
        {
            if (operation.ReturnType == null)
            {
                return "ActionResult";
            }
            return $"ActionResult<{GetTypeName(operation.TypeReference)}>";
        }



        private HttpVerb GetHttpVerb(OperationModel operation)
        {
            var verb = operation.GetApiSettings().HttpVerb();
            if (!verb.IsAUTO())
            {
                return Enum.TryParse(verb.Value, out HttpVerb verbEnum) ? verbEnum : HttpVerb.POST;
            }
            if (operation.ReturnType == null || operation.Parameters.Any(x => !Types.Get(x.TypeReference).IsPrimitive))
            {
                var hasIdParam = operation.Parameters.Any(x => x.Name.ToLower().EndsWith("id") && Types.Get(x.TypeReference).IsPrimitive);
                if (hasIdParam && new[] { "delete", "remove" }.Any(x => operation.Name.ToLower().Contains(x)))
                {
                    return HttpVerb.DELETE;
                }
                return hasIdParam ? HttpVerb.PUT : HttpVerb.POST;
            }
            return HttpVerb.GET;
        }

        private string GetPath(OperationModel operation)
        {
            var path = operation.GetApiSettings().HttpRoute();
            return !string.IsNullOrWhiteSpace(path) ? path : null;
        }

        private string GetParameterBindingAttribute(OperationModel operation, ParameterModel parameter)
        {
            const string ParameterBinding = "Parameter Binding";
            const string PropertyType = "Type";
            const string PropertyCustomType = "Custom Type";
            const string CustomValue = "Custom";

            if (parameter.HasStereotype(ParameterBinding))
            {
                var attributeName = parameter.GetStereotypeProperty<string>(ParameterBinding, PropertyType);
                if (string.Equals(attributeName, CustomValue, StringComparison.OrdinalIgnoreCase))
                {
                    var customAttributeValue = parameter.GetStereotypeProperty<string>(ParameterBinding, PropertyCustomType);
                    if (string.IsNullOrWhiteSpace(customAttributeValue))
                    {
                        throw new Exception("Parameter Binding was set to custom but no Custom attribute type was specified");
                    }
                    return $"[{customAttributeValue}]";
                }
                return $"[{attributeName}]";
            }

            if (operation.Parameters.Count(p => p.TypeReference.Element.SpecializationType == "DTO") == 1
                && parameter.TypeReference.Element.SpecializationType == "DTO")
            {
                return "[FromBody]";
            }
            return string.Empty;
        }

        internal enum HttpVerb
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}