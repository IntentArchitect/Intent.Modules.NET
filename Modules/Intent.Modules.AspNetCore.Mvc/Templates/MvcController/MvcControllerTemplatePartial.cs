using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AspNetCore.Mvc.Api;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Metadata.Security.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.Security.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.AspNetCore.Mvc.Api.OperationModelStereotypeExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Mvc.Templates.MvcController
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MvcControllerTemplate : CSharpTemplateBase<ServiceModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Mvc.MvcController";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MvcControllerTemplate(IOutputTarget outputTarget, ServiceModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Application.Command);
            AddTypeSource(TemplateRoles.Application.Query);
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            var securityModels = SecurityModelHelpers.GetSecurityModels(Model.InternalElement).ToArray();
            var securedByDefault = !Model.HasUnsecured() && securityModels.Length > 0;

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .AddClass($"{Model.Name.RemoveSuffix("Controller", "Service")}Controller", @class =>
                {
                    @class.WithBaseType("Controller");
                    @class.AddConstructor();
                    @class.AddMetadata("model", Model);

                    if (Model.HasUnsecured())
                    {
                        @class.AddAttribute("[AllowAnonymous]");
                    }
                    else
                    {
                        foreach (var attribute in GetAuthorizationAttributes(securityModels))
                        {
                            @class.AddAttribute(attribute);
                        }
                    }

                    if (Model.TryGetMVCSettings(out var serviceMvcSettings) &&
                        !string.IsNullOrWhiteSpace(serviceMvcSettings.Route()))
                    {
                        @class.AddAttribute($"[Route(\"{serviceMvcSettings.Route()}\")]");
                    }

                    foreach (var operation in Model.Operations.Where(x => x.HasMVCSettings()))
                    {
                        var mvcSettings = operation.GetMVCSettings();

                        var resultType = mvcSettings.ReturnType().IsOperationReturnType()
                            ? $"ActionResult<{GetTypeName(operation)}>"
                            : "ActionResult";

                        @class.AddMethod(resultType, operation.Name.ToPascalCase(), method =>
                        {
                            method.AddMetadata("model", operation);
                            method.Async();
                            method.TryAddXmlDocComments(operation.InternalElement);

                            foreach (var attribute in GetOperationAttributes(operation, mvcSettings, securedByDefault))
                            {
                                method.AddAttribute(attribute);
                            }

                            foreach (var parameter in operation.Parameters)
                            {
                                if (mvcSettings.Verb().IsGET() && parameter.TypeReference is { IsCollection: true } && parameter.TypeReference.Element.IsDTOModel())
                                {
                                    throw new ElementException(operation.InternalElement, "GET Operations do not support collections on complex objects.");
                                }

                                // If you change anything here, please check also: WorkaroundForGetTypeNameIssue()
                                method.AddParameter(parameter, param =>
                                {
                                    param.AddMetadata("model", parameter);
                                    param.WithDefaultValue(parameter.Value);
                                });
                            }

                            if (mvcSettings.Verb().AsEnum() == MVCSettings.VerbOptionsEnum.POST && IsRouteMultiTenancyConfigured(mvcSettings.Route()))
                            {
                                method.AddParameter(UseType("Finbuckle.MultiTenant.ITenantInfo"), "tenantInfo");
                            }

                            method.AddOptionalCancellationTokenParameter();
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
                    var paramModel = entry.Param.GetMetadata<ParameterModel>("model");
                    var param = new CSharpParameter(GetTypeName(paramModel.TypeReference), entry.Param.Name, method);
                    param.AddMetadata("model", paramModel);
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

        private CSharpAttribute GetHttpVerbAndPath(OperationModel operation, MVCSettings mvcSettings)
        {
            var arguments = new List<string>();

            if (!string.IsNullOrWhiteSpace(mvcSettings.Route()))
            {
                arguments.Add($"\"{mvcSettings.Route()}\"");
            }

            if (!string.IsNullOrWhiteSpace(mvcSettings.Name()))
            {
                arguments.Add($"Name = \"{mvcSettings.Name()}\"");
            }

            var joinedArguments = arguments.Any()
                ? $"({string.Join(", ", arguments)})"
                : string.Empty;

            return new CSharpAttribute(
                $"[Http{mvcSettings.Verb().Value.ToLower().ToPascalCase()}{joinedArguments}]");
        }

        private IEnumerable<CSharpAttribute> GetAuthorizationAttributes(IReadOnlyCollection<ISecurityModel> securityModels)
        {
            return securityModels
                .Select(model =>
                {
                    var attribute = new CSharpAttribute(UseType("Microsoft.AspNetCore.Authorization.Authorize"));

                    if (model.Roles.Count > 0)
                    {
                        attribute.AddArgument($"Roles = \"{string.Join(",", model.Roles)}\"");
                    }

                    if (model.Policies.Count > 0)
                    {
                        attribute.AddArgument($"Policy = \"{string.Join(",", model.Policies)}\"");
                    }

                    return attribute;
                });
        }

        private List<CSharpAttribute> GetOperationAttributes(OperationModel operation, MVCSettings mvcSettings, bool securedByDefault)
        {
            var attributes = new List<CSharpAttribute>
            {
                GetHttpVerbAndPath(operation, mvcSettings)
            };

            var securityModels = SecurityModelHelpers.GetSecurityModels(operation.InternalElement, checkParents: false).ToArray();
            attributes.AddRange(GetAuthorizationAttributes(securityModels));

            if (operation.HasUnsecured())
            {
                attributes.Add(new CSharpAttribute("[AllowAnonymous]"));
            }

            return attributes;
        }

        private bool IsRouteMultiTenancyConfigured(string operationRoute)
        {
            // only apply if the strategy is route strategy
            var tenancyStrategy = ExecutionContext.GetSettings().GetSetting("41ae5a02-3eb2-42a6-ade2-322b3c1f1115", "e15fe0fb-be28-4cc5-8b85-37a07b7ca160");
            if (tenancyStrategy is null || tenancyStrategy.Value != "route")
            {
                return false;
            }

            // get the route parameter
            var routeParamValue = ExecutionContext.GetSettings().GetSetting("41ae5a02-3eb2-42a6-ade2-322b3c1f1115", "c8ff4af6-68b6-4e31-a291-43ada6a0008a");

            // if the method is not using the route parameter in the URL path
            if (!operationRoute.Contains($"{{{routeParamValue?.Value ?? string.Empty}}}"))
            {
                return false;
            }

            return true;
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