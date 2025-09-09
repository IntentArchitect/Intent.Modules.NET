using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.CosmosDB.Helpers;
using Intent.Modules.CosmosDB.Settings;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryBase;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryOptions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var authMethods = ExecutionContext.Settings.GetCosmosDBSettings().AuthenticationMethods();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"CosmosConfiguration", @class =>
                {
                    @class.Static();

                    // add this method first, as the other methods use its presence as a check
                    // only add if we have more than one auth method, and we'll extract the building to a method
                    if (authMethods.Length > 1)
                    {
                        // get the statements to be added
                        var statements = ConfigurationHelper.GetConfigurationStatements(ExecutionContext.MetadataManager.Domain(ExecutionContext.GetApplicationConfig().Id), this, true);

                        if (statements.Count != 0)
                        {
                            @class.AddMethod("void", "BuildContainer", mth =>
                            {
                                mth.Private();
                                mth.Static();
                                mth.AddParameter(UseType("Microsoft.Azure.CosmosRepository.Options.RepositoryOptions"), "options");
                                // not pretty, but if any of the statements need CosmosRepositoryOptions instance
                                // then add as a parameter
                                if (statements.Any(s => s.Text.Contains("cosmosOptions.")))
                                {
                                    mth.AddParameter("CosmosRepositoryOptions", "cosmosOptions");
                                }

                                mth.AddStatements(statements);
                            });
                        }
                    }

                    @class.AddMethod(UseType("Microsoft.Extensions.DependencyInjection.IServiceCollection"), "ConfigureCosmosRepository", mth =>
                    {
                        mth.Static();

                        mth.AddParameter("IServiceCollection", "services", config =>
                        {
                            config.WithThisModifier();
                        });
                        mth.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");

                        var optionsTemplate = GetTemplate<CSharpTemplateBase<object>>(CosmosDBRepositoryOptionsTemplate.TemplateId);
                        if (optionsTemplate is not null)
                        {
                            AddUsing(optionsTemplate.Namespace);
                        }

                        mth.AddInvocationStatement($"services.Configure<CosmosRepositoryOptions>", config =>
                        {
                            var invoc = new CSharpInvocationStatement("configuration.GetSection").AddArgument("\"RepositoryOptions\"").WithoutSemicolon();

                            config.AddArgument(invoc);
                        });

                        var getSectionInvoc = new CSharpInvocationStatement("configuration.GetSection")
                            .AddArgument("\"RepositoryOptions\"")
                            .AddInvocation("Get<CosmosRepositoryOptions>");
                        mth.AddObjectInitStatement("var cosmosOptions", getSectionInvoc);

                        // if more than one auth method and one is managed identity
                        if (authMethods.Length > 1 && authMethods.Any(a => a.IsManagedIdentity()))
                        {
                            mth.AddIfStatement("cosmosOptions?.AuthenticationMethod?.ToLower() == \"managedidentity\"", @if =>
                            {
                                @if.AddStatements(GetManagedIdentityConfiguration());
                                @if.AddStatements(GetManagedIdentityAddStatements());

                                @if.AddReturn("services");
                            });
                        }
                        else if (authMethods.Any(a => a.IsManagedIdentity()))
                        {
                            mth.AddStatement("");
                            mth.AddStatements(GetManagedIdentityConfiguration());
                            mth.AddStatements(GetManagedIdentityAddStatements());
                        }

                        // add the key based statements at the end as a fall back
                        if (authMethods.Any(a => a.IsKeyBased()))
                        {
                            mth.AddStatement("");
                            mth.AddStatements(GetKeyBasedAddStatements());
                        }

                        mth.AddReturn("services", ret => ret.SeparatedFromPrevious());
                    });


                });
        }

        public override bool CanRunTemplate()
        {
            var authenticationMethods = ExecutionContext.Settings.GetCosmosDBSettings().AuthenticationMethods();
            var outputTemplate = authenticationMethods != null && authenticationMethods.Any(a => !a.IsKeyBased());

            return base.CanRunTemplate() && outputTemplate;
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

        private List<CSharpStatement> GetManagedIdentityConfiguration()
        {
            var clientIdStatement = new CSharpObjectInitStatement("var managedIdentityClientId", "cosmosOptions?.ManagedIdentityClientId;");
            var conditionalStatement = new CSharpStatement($@"string.IsNullOrWhiteSpace(managedIdentityClientId)
                ? new {UseType("Azure.Identity.DefaultAzureCredential")}()
                : new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {{
                    ManagedIdentityClientId = managedIdentityClientId
                }});");
            var credentialStatement = new CSharpObjectInitStatement("var credential", conditionalStatement);

            return [clientIdStatement, credentialStatement, ""];
        }
        private List<CSharpStatement> GetManagedIdentityAddStatements()
        {
            var invocation = new CSharpInvocationStatement("services.AddCosmosRepository");

            // this will add the statements and the lambda block to the invocation. Adds the "options =>" (if required) and the statements
            ConfigurationHelper.BuildConfigurationLambda(ExecutionContext.MetadataManager.Domain(ExecutionContext.GetApplicationConfig().Id),
                    this, invocation);

            // if there was no need initially for the options, then add it now
            var optionsLambda = new CSharpLambdaBlock("options");
            if (!invocation.Statements.Any(s => s.Text == "options"))
            {
                invocation.AddArgument(optionsLambda);
            }
            else
            {
                optionsLambda = invocation.Statements.First(s => s.Text == "options") as CSharpLambdaBlock;
                optionsLambda.AddStatement("");
            }

            optionsLambda.AddObjectInitStatement("options.CosmosConnectionString", "null;");
            optionsLambda.AddObjectInitStatement("options.TokenCredential", "credential;");

            return [invocation];
        }

        private List<CSharpStatement> GetKeyBasedAddStatements()
        {
            var invocation = new CSharpInvocationStatement("services.AddCosmosRepository");

            ConfigurationHelper.BuildConfigurationLambda(ExecutionContext.MetadataManager.Domain(ExecutionContext.GetApplicationConfig().Id),
                    this, invocation);

            var optionsLambda = new CSharpLambdaBlock("options");
            if (!invocation.Statements.Any(s => s.Text == "options"))
            {
                invocation.AddArgument(optionsLambda);
            }
            else
            {
                optionsLambda = invocation.Statements.First(s => s.Text == "options") as CSharpLambdaBlock;
                optionsLambda.AddStatement("");
            }

            optionsLambda.AddObjectInitStatement("options.AccountEndpoint", "null;");
            optionsLambda.AddObjectInitStatement("options.TokenCredential", "null;");

            return [invocation];
        }
    }
}