using System;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.OpenApi.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OpenApiOperationAttributeExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AzureFunctions.OpenApi.OpenApiOperationAttributeExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var azureFunctionClassTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.AzureFunctions.AzureFunctionClass");
            if (azureFunctionClassTemplates is null || azureFunctionClassTemplates.Count() < 1)
            {
                return;
            }

            foreach (var template in azureFunctionClassTemplates)
            {
                if (!template.TryGetModel<IAzureFunctionModel>(out var templateModel) ||
                    !templateModel.HasStereotype("OpenAPI Operation"))
                {
                    continue;
                }

                template.CSharpFile.AfterBuild(file =>
                {
                    var runMethod = file.Classes.FirstOrDefault().FindMethod("Run");
                    var openApiOpeationAttribute = runMethod.Attributes.FirstOrDefault(att => att.Name == "OpenApiOperation");

                    var openApiOperationAttributeStereotype = templateModel.GetStereotype("OpenAPI Operation");

                    var summary = openApiOperationAttributeStereotype.GetProperty("Summary").Value;
                    var description = openApiOperationAttributeStereotype.GetProperty("Description").Value;
                    var tags = openApiOperationAttributeStereotype.GetProperty("Tags").Value;
                    var deprecated = Convert.ToBoolean(openApiOperationAttributeStereotype.GetProperty("Deprecated").Value);

                    if (!string.IsNullOrEmpty(summary))
                    {
                        var summaryStatement = openApiOpeationAttribute.FindStatement(s => s.Text.Contains("Summary"));
                        if (summaryStatement is null)
                        {
                            openApiOpeationAttribute.AddStatement($"""Summary = "{summary}" """);
                        }
                        else
                        {
                            summaryStatement.Text = $"""Summary = "{summary}" """;
                        }
                    }

                    if (!string.IsNullOrEmpty(description))
                    {
                        var descriptionStatement = openApiOpeationAttribute.FindStatement(s => s.Text.Contains("Description"));
                        if (descriptionStatement is null)
                        {
                            openApiOpeationAttribute.AddStatement($"""Description = "{description}" """);
                        }
                        else
                        {
                            descriptionStatement.Text = $"""Description = "{description}" """;
                        }
                    }

                    if (!string.IsNullOrEmpty(tags))
                    {
                        var tagStatement = openApiOpeationAttribute.FindStatement(s => s.Text.Contains("tags"));
                        if (tagStatement is null)
                        {
                            openApiOpeationAttribute.AddStatement($"tags: new[] {{ {string.Join(", ", tags.Split(" ").Select(t => $"\"{t}\""))} }}");
                        }
                        else
                        {
                            tagStatement.Text = $"tags: new[] {{ {string.Join(", ", tags.Split(" ").Select(t => $"\"{t}\""))} }}";
                        }
                    }

                    if (deprecated)
                    {
                        var deprecatedStatement = openApiOpeationAttribute.FindStatement(s => s.Text.Contains("Deprecated"));
                        if (deprecatedStatement is null)
                        {
                            openApiOpeationAttribute.AddStatement($"Deprecated = true");
                        }
                        else
                        {
                            deprecatedStatement.Text = $"Deprecated = true";
                        }
                    }
                });

            }
        }
    }
}