using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.ODataQuery.ODataQuerySwaggerFilter", Version = "1.0")]

namespace MinimalHostingModel.Api.Filters
{
    public class ODataQueryFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasODataQueryOptions = context.MethodInfo.GetParameters().Any(MatchODataQueryOptions);

            if (!hasODataQueryOptions)
            {
                return;
            }

            int index = context.MethodInfo.GetParameters()
                                .Select((param, idx) => new { Param = param, Index = idx })
                                .FirstOrDefault(x => MatchODataQueryOptions(x.Param))?.Index ?? -1;
            var parameter = operation.Parameters[index];

            if (parameter == null)
            {
                return;
            }

            operation.Parameters.Remove(parameter);

            if (context.MethodInfo.ReturnType == typeof(Task<IActionResult>))
            {
                operation.Parameters.Add(OdataParameter("$select", "Selects which properties to include in the response. (e.g. $select=Name)"));
            }
            operation.Parameters.Add(OdataParameter("$top", "The max number of records. (e.g. $top=10)"));
            operation.Parameters.Add(OdataParameter("$skip", "The number of records to skip. (e.g. $skip=5)"));
            operation.Parameters.Add(OdataParameter("$filter", "A function that must evaluate to true for a record to be returned. (e.g. $filter=CustomerName eq 'bob')"));
            operation.Parameters.Add(OdataParameter("$orderby", "Determines what values are used to order a collection of records. (e.g. $orderby=Address1_Country,Address1_City desc)"));
        }

        private static bool MatchODataQueryOptions(ParameterInfo parameter)
        {
            return parameter.ParameterType.IsGenericType &&
                parameter.ParameterType.GetGenericTypeDefinition() == typeof(ODataQueryOptions<>);
        }

        private static OpenApiParameter OdataParameter(string name, string description)
        {
            return new()
            {
                Name = name,
                Description = description,
                Required = false,
                Schema = new OpenApiSchema { Type = "string" },
                In = ParameterLocation.Query
            };
        }
    }
}