using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Aws.Lambda.Functions.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Templates.FunctionClass
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class FunctionClassTemplate : CSharpTemplateBase<ILambdaFunctionContainerModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.Lambda.Functions.FunctionClassTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FunctionClassTemplate(IOutputTarget outputTarget, ILambdaFunctionContainerModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AmazonLambdaCore(outputTarget));
            AddNugetDependency(NugetPackages.AmazonLambdaAPIGatewayEvents(outputTarget));
            AddNugetDependency(NugetPackages.AmazonLambdaSerializationSystemTextJson(outputTarget));
            AddNugetDependency(NugetPackages.AmazonLambdaAnnotations(outputTarget));
            AddNugetDependency(NugetPackages.AmazonLambdaLoggingAspNetCore(outputTarget));
            
            AddFrameworkDependency("Microsoft.AspNetCore.App");
            
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddAssemblyAttribute("LambdaSerializer", attr => attr.AddArgument("typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer)"))
                .AddUsing("Amazon.Lambda.Core")
                .AddUsing("Amazon.Lambda.Annotations")
                .AddUsing("Amazon.Lambda.Annotations.APIGateway")
                .AddUsing("Amazon.Lambda.APIGatewayEvents")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.AddConstructor(ctor =>
                    {
                    });
                    
                    foreach (var functionModel in Model.Endpoints)
                    {
                        @class.AddMethod($"Task<APIGatewayHttpApiV2ProxyResponse>", functionModel.Name.EnsureSuffixedWith("Async"), method =>
                        {
                            method.Async();
                            method.TryAddXmlDocComments(functionModel.InternalElement);
                            method.AddAttribute("LambdaFunction");
                            method.AddAttribute("HttpApi", attr => attr.AddArgument($"LambdaHttpMethod.{functionModel.Verb}").AddArgument($@"""{functionModel.Route}"""));
                            method.AddParameters(functionModel.Parameters, param =>
                            {
                                var paramModel = (IEndpointParameterModel)param.RepresentedModel;
                                switch (paramModel.Source)
                                {
                                    case HttpInputSource.FromRoute:
                                        break;
                                    case HttpInputSource.FromQuery:
                                        param.AddAttribute("FromQuery", attr => attr.AddArgument($@"Name = ""{paramModel.QueryStringName}"""));
                                        break;
                                    case HttpInputSource.FromBody:
                                        param.AddAttribute("FromBody");
                                        break;
                                    case HttpInputSource.FromForm:
                                        throw new ElementException(functionModel.InternalElement, $"Parameter {paramModel.Name} source is FromForm which is not supported in AWS Lambda functions.");
                                    case HttpInputSource.FromHeader:
                                        param.AddAttribute("FromHeader", attr => attr.AddArgument($@"Name = ""{paramModel.HeaderName}"""));
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException($"Source '{paramModel.Source}' is not supported.");
                                }
                            });

                            if (functionModel.ReturnType is null)
                            {
                                method.AddReturn(new CSharpObjectInitializerBlock("new APIGatewayHttpApiV2ProxyResponse")
                                    .AddInitStatement("StatusCode", "204"));
                                return;
                            }
                            
                            method.AddReturn(new CSharpObjectInitializerBlock("new APIGatewayHttpApiV2ProxyResponse")
                                .AddInitStatement("StatusCode", "200")
                                .AddInitStatement("Headers", new CSharpObjectInitializerBlock("new Dictionary<string, string>")
                                    .AddKeyAndValue("Content-Type", "application/json")
                                )
                                .AddInitStatement("Body", "result")
                            );
                        });
                    }
                    
                });
        }

        public override void BeforeTemplateExecution()
        {
            Project.GetProject().AddProperty("AWSProjectType", "Lambda");
            Project.GetProject().AddProperty("PublishReadyToRun", "true");
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