using Intent.Configuration;
using Intent.Engine;
using Intent.IArchitect.CrossPlatform.IO;
using Intent.Modelers.UI.Api;
using Intent.Modules.AI.Blazor.Tasks.Config;
using Intent.Modules.AI.Blazor.Tasks.Helpers;
using Intent.Modules.Common.AI;
using Intent.Plugins;
using Intent.Registrations;
using Intent.Utils;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AI.Blazor.Tasks
{
    public class GetPromptTemplatesTask : IModuleTask
    {
        private readonly IApplicationConfigurationProvider _applicationConfigurationProvider;
        private readonly ISolutionConfig _solution;

        public GetPromptTemplatesTask(
            IApplicationConfigurationProvider applicationConfigurationProvider,
            ISolutionConfig solution)
        {
            _applicationConfigurationProvider = applicationConfigurationProvider;
            _solution = solution;
        }

        public string TaskTypeId => "Intent.Modules.AI.Blazor.GetPromptTemplates";
        public string TaskTypeName => "Get Blazor AI Prompt Templates";
        public int Order => 0;

        public string Execute(params string[] args)
        {
            string pageName = args.Length > 0 ? args[0] :  "";
            var result = new List<TemplateResult>();
            string promptTemplateLocation = Path.Combine(_solution.SolutionRootLocation, "AI.Prompt.Templates", "Intent.Blazor.AI");
            var config = PromptConfig.TryLoad(_solution.SolutionRootLocation);
            if (config != null)
            {
                var guess = TemplateGuesser.Guess(config, pageName);
                result.AddRange(config.Templates.Select(t => new TemplateResult 
                { 
                    Id = t.Id, 
                    Description = t.Name, 
                    AdditionalInfo = t.Description, 
                    DefaultUserPrompt = t.DefaultUserPrompt,
                    RecommenedDefault = guess is null ? false : t.Id == guess.TemplateId 
                }));
            }
            return JsonConvert.SerializeObject(result, JsonNet.Settings);
        }
    }

    public class TemplateResult
    { 
        public string Id { get; set; }
        public string Description { get; set; }
        public string AdditionalInfo { get; set; }
        public bool RecommenedDefault { get; set; }
        public string DefaultUserPrompt { get; set; }
    }

}
