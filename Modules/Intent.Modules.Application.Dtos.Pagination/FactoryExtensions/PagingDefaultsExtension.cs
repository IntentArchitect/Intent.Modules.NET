using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Dtos.Pagination.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Pagination.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PagingDefaultsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Dtos.Pagination.PagingDefaultsExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        private static readonly string[] _pageNumberOptions = ["page", "pageno", "pagenum", "pagenumber"];
        private static readonly string[] _pageSizeOptions = ["size", "pagesize"];

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            // if neither value have been set, then skip over
            if (string.IsNullOrWhiteSpace(application.Settings.GetPaginationSettings().PageSizeDefault()) &&
                string.IsNullOrWhiteSpace(application.Settings.GetPaginationSettings().OrderByDefault()))
            {
                return;
            }

            const string pagedResultTypeDefinitionId = "9204e067-bdc8-45e7-8970-8a833fdc5253";

            // set the default values
            // All have to be set as cannot have methods where later params have default value but earlier params do not
            var pageNumberDefault = 1;
            var pageSizeDefault = !string.IsNullOrWhiteSpace(application.Settings.GetPaginationSettings().PageSizeDefault()) ?
                application.Settings.GetPaginationSettings().PageSizeDefault() :
                "20";
            var orderByDefault = !string.IsNullOrWhiteSpace(application.Settings.GetPaginationSettings().OrderByDefault()) ?
                application.Settings.GetPaginationSettings().OrderByDefault() :
                "null";

            // Get the queries which have a return type of PagedResult
            var queryModels = application.MetadataManager.Services(application).GetQueryModels()
                .Where(qm => qm.TypeReference?.ElementId == pagedResultTypeDefinitionId)
                .ToArray();

            foreach (var queryModel in queryModels)
            {
                UpdateController(application, pageNumberDefault, pageSizeDefault, orderByDefault, queryModel.InternalElement);
                UpdateQueryModel(application, pageNumberDefault, pageSizeDefault, orderByDefault, queryModel.InternalElement);
            }

            // Get the queries which have a return type of PagedResult
            var serviceOperations = application.MetadataManager.Services(application).GetServiceModels()
                .SelectMany(sm => sm.Operations)
                .Where(op => op.TypeReference?.ElementId == pagedResultTypeDefinitionId)
                .ToArray();

            foreach (var operation in serviceOperations)
            {
                UpdateController(application, pageNumberDefault, pageSizeDefault, orderByDefault, operation.InternalElement);
                UpdateServiceOperation(application, pageNumberDefault, pageSizeDefault, orderByDefault, operation.InternalElement);
            }
        }

        private static void UpdateController(IApplication application, int pageNumberDefault, string pageSizeDefault, string orderByDefault, IElement queryModel)
        {
            var controllerTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.Controllers.Controller", queryModel.ParentElement.Id);
            controllerTemplate?.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var method = @class.FindMethod(m => m.HasMetadata("modelId") && m.GetMetadata<string>("modelId") == queryModel.Id);

                if (method is null)
                {
                    return;
                }

                if (TryGetInvalidParametersAfterPaging(method.Parameters, out var invalidParameters))
                {
                    Logging.Log.Warning($"Could not apply default paging values to controller '{controllerTemplate.ClassName}', operation '{method.Name}'.");
                    Logging.Log.Warning($"The following parameters without default values appear after parameters that have default values: {string.Join(',', invalidParameters)}");

                    return;
                }

                UpdateMethodParameters(application, pageNumberDefault, pageSizeDefault, orderByDefault, method);
            });
        }

        private static void UpdateServiceOperation(IApplication application, int pageNumberDefault, string pageSizeDefault, string orderByDefault, IElement operationModel)
        {
            var operationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.ServiceImplementations.ServiceImplementation", operationModel.ParentElement.Id);
            operationTemplate?.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var method = @class.FindMethod(m => m.HasMetadata("model") && m.GetMetadata<OperationModel>("model")?.Id == operationModel.Id);

                if (method is null)
                {
                    return;
                }

                if (TryGetInvalidParametersAfterPaging(method.Parameters, out var invalidParameters))
                {
                    Logging.Log.Warning($"Could not apply default paging values to service '{operationTemplate.ClassName}', operation '{method.Name}'.");
                    Logging.Log.Warning($"The following parameters without default values appear after parameters that have default values: {string.Join(',', invalidParameters)}");

                    return;
                }

                UpdateMethodParameters(application, pageNumberDefault, pageSizeDefault, orderByDefault, method);
            });
        }

        private static void UpdateQueryModel(IApplication application, int pageNumberDefault, string pageSizeDefault, string orderByDefault, IElement queryModel)
        {
            var queryTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.MediatR.QueryModels", queryModel.Id);
            queryTemplate?.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.FirstOrDefault();

                if (ctor is null)
                {
                    return;
                }

                if (TryGetInvalidParametersAfterPaging(ctor.Parameters, out var invalidParameters))
                {
                    Logging.Log.Warning($"Could not apply default paging values to the query '{queryTemplate.ClassName}'.");
                    Logging.Log.Warning($"The following parameters without default values appear after parameters that have default values: {string.Join(',', invalidParameters)}");

                    return;
                }

                foreach (var parameter in ctor.Parameters)
                {
                    if (parameter.Type.StartsWith("int") && _pageNumberOptions.Contains(parameter.Name.ToLower()) && string.IsNullOrWhiteSpace(parameter.DefaultValue))
                    {
                        parameter.WithDefaultValue($"{pageNumberDefault}");
                    }

                    if (parameter.Type.StartsWith("int") && parameter.Name.Equals("pageindex", System.StringComparison.CurrentCultureIgnoreCase) && string.IsNullOrWhiteSpace(parameter.DefaultValue))
                    {
                        parameter.WithDefaultValue($"{pageNumberDefault - 1}");
                    }

                    if (parameter.Type.StartsWith("int") && _pageSizeOptions.Contains(parameter.Name.ToLower()) && string.IsNullOrWhiteSpace(parameter.DefaultValue))
                    {
                        parameter.WithDefaultValue($"{pageSizeDefault}");
                    }

                    if (parameter.Type.StartsWith("string") && parameter.Name.Equals("orderby", System.StringComparison.CurrentCultureIgnoreCase) && string.IsNullOrWhiteSpace(parameter.DefaultValue))
                    {
                        // if its null or is already quoted, then use as is
                        if (orderByDefault.Equals("null") || orderByDefault.StartsWith('"'))
                        {
                            parameter.WithDefaultValue(orderByDefault);
                            continue;
                        }

                        parameter.WithDefaultValue($"\"{orderByDefault}\"");
                    }
                }
            });
        }

        private static void UpdateMethodParameters(IApplication application, int pageNumberDefault, string pageSizeDefault, string orderByDefault, CSharpClassMethod method)
        {
            foreach (var parameter in method.Parameters)
            {
                if (parameter.Type.StartsWith("int") && _pageNumberOptions.Contains(parameter.Name.ToLower()) && string.IsNullOrWhiteSpace(parameter.DefaultValue))
                {
                    parameter.WithDefaultValue($"{pageNumberDefault}");
                }

                if (parameter.Type.StartsWith("int") && parameter.Name.Equals("pageindex", System.StringComparison.CurrentCultureIgnoreCase) && string.IsNullOrWhiteSpace(parameter.DefaultValue))
                {
                    parameter.WithDefaultValue($"{pageNumberDefault - 1}");
                }

                if (parameter.Type.StartsWith("int") && _pageSizeOptions.Contains(parameter.Name.ToLower()) && string.IsNullOrWhiteSpace(parameter.DefaultValue))
                {
                    parameter.WithDefaultValue($"{pageSizeDefault}");
                }

                if (parameter.Type.StartsWith("string") && parameter.Name.Equals("orderby", System.StringComparison.CurrentCultureIgnoreCase) && string.IsNullOrWhiteSpace(parameter.DefaultValue))
                {
                    // if its null or is already quoted, then use as is
                    if (orderByDefault.Equals("null") || orderByDefault.StartsWith('"'))
                    {
                        parameter.WithDefaultValue(orderByDefault);
                        continue;
                    }

                    parameter.WithDefaultValue($"\"{orderByDefault}\"");
                }
            }
        }

        public static bool TryGetInvalidParametersAfterPaging(IEnumerable<ICSharpParameter> parameterNames, out IReadOnlyList<string> invalidParameters)
        {
            ArgumentNullException.ThrowIfNull(parameterNames);

            var validPagingNames = new HashSet<string>(
                _pageNumberOptions.Concat(_pageSizeOptions).Concat(["pageIndex", "orderBy"]),
                StringComparer.OrdinalIgnoreCase);

            var invalid = new List<string>();
            var seenPagingParam = false;

            foreach (var param in parameterNames)
            {
                var name = param.Name?.Trim() ?? string.Empty;

                if (validPagingNames.Contains(name))
                {
                    seenPagingParam = true;
                    continue;
                }

                // only if the param doesn't have a default value
                if (seenPagingParam && string.IsNullOrWhiteSpace(param.DefaultValue))
                {
                    // We've already seen a paging param, and this one isn't valid
                    invalid.Add(name);
                }
            }

            invalidParameters = invalid;

            return invalid.Count > 0;
        }
    }
}
