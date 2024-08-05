using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.ODataQuery.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.ODataQuery.Templates.ODataQuerySwaggerFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ODataQuerySwaggerFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.ODataQuery.ODataQuerySwaggerFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ODataQuerySwaggerFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreOData(OutputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"ODataQueryFilter", @class =>
                {
                    AddUsing("System.Linq");
                    AddUsing("System.Collections");
                    AddUsing("System.Reflection");
                    AddUsing("System.Threading.Tasks");
                    AddUsing("Microsoft.AspNetCore.Mvc");
                    AddUsing("Microsoft.OpenApi.Models");
                    AddUsing("Microsoft.AspNetCore.OData.Query");
                    AddUsing("Swashbuckle.AspNetCore.SwaggerGen");
                    @class.ImplementsInterface(UseType("Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter"));

                    @class.AddMethod("void", "Apply", method =>
                    {
                        method
                            .AddParameter("OpenApiOperation", "operation")
                            .AddParameter("OperationFilterContext", "context");

                        method.AddStatement("var hasODataQueryOptions = context.MethodInfo.GetParameters().Any(MatchODataQueryOptions);");
                        method.AddIfStatement("!hasODataQueryOptions", c => c.AddStatement("return;"));

                        method.AddStatement(@"int index = context.MethodInfo.GetParameters()
                                .Select((param, idx) => new { Param = param, Index = idx })
                                .FirstOrDefault(x => MatchODataQueryOptions(x.Param))?.Index ?? -1;", s => s.SeparatedFromPrevious());
                        method.AddStatement("var parameter = operation.Parameters[index];");

                        method.AddIfStatement("parameter == null", c =>
                        {
                            c.SeparatedFromPrevious();
                            c.AddStatement("return;");
                        });
                        method.AddStatement("operation.Parameters.Remove(parameter);", s => s.SeparatedFromPrevious());
                        var settings = ExecutionContext.Settings.GetODataQuerySettings();

                        if (settings.AllowSelectOption())
                        {
                            method.AddIfStatement($"context.MethodInfo.ReturnType == typeof({UseType("Task<IActionResult>")})", c =>
                            {
                                c.AddStatement("operation.Parameters.Add(OdataParameter(\"$select\", \"Selects which properties to include in the response. (e.g. $select=Name)\"));", s => s.SeparatedFromPrevious());
                            });
                        }
                        if (settings.AllowExpandOption())
                        {
                            method.AddStatement("operation.Parameters.Add(OdataParameter(\"$expand\", \"Expands related entities inline.\"));");
                        }
                        method.AddStatement("operation.Parameters.Add(OdataParameter(\"$top\", \"The max number of records. (e.g. $top=10)\"));");

                        method.AddStatement("operation.Parameters.Add(OdataParameter(\"$skip\", \"The number of records to skip. (e.g. $skip=5)\"));");
                        if (settings.AllowFilterOption())
                        {
                            method.AddStatement("operation.Parameters.Add(OdataParameter(\"$filter\", \"A function that must evaluate to true for a record to be returned. (e.g. $filter=CustomerName eq 'bob')\"));");
                        }
                        if (settings.AllowOrderByOption())
                        {
                            method.AddStatement("operation.Parameters.Add(OdataParameter(\"$orderby\", \"Determines what values are used to order a collection of records. (e.g. $orderby=Address1_Country,Address1_City desc)\"));");
                        }
                    });

                    @class.AddMethod("bool", "MatchODataQueryOptions", method =>
                    {
                        method
                            .Private()
                            .Static()
                            .AddParameter("ParameterInfo", "parameter");
                        method.AddStatement(@"return parameter.ParameterType.IsGenericType && 
                parameter.ParameterType.GetGenericTypeDefinition() == typeof(ODataQueryOptions<>);");
                    });
                    @class.AddMethod("OpenApiParameter", "OdataParameter", method =>
                    {
                        method
                            .Private()
                            .Static()
                            .AddParameter("string", "name")
                            .AddParameter("string", "description");
                        method.AddStatement(@"return new() 
        {
            Name = name,
            Description = description,
            Required = false,
            Schema = new OpenApiSchema { Type = ""string"" },
            In = ParameterLocation.Query
        };");
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration")) != null;
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