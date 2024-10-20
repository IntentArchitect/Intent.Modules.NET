using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.DotNet.Cli.Sln.Internal;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.SharedKernel.Consumer.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SolutionPatcherExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.SharedKernel.Consumer.SolutionPatcherExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            base.OnAfterCommitChanges(application);
            var sharedKernel = TemplateHelper.GetSharedKernel();
            AddProjectReferences(application, sharedKernel);
        }

        private void AddProjectReferences(IApplication application, SharedKernel sharedKernel)
        {
            //Application
            {
                var appTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Application.DependencyInjection);
                if (appTemplate is not null)
                {
                    var kernelConfig = application.GetApplicationConfig(sharedKernel.ApplicationId);
                    ((CSharpTemplateBase<object>)appTemplate).AddAssemblyReference(new ProjectReference($"{kernelConfig.OutputLocation}\\{sharedKernel.ApplicationName}.Application\\{sharedKernel.ApplicationName}.Application.csproj"));
                }
            }

            //Infrastructure
            {
                var infraTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Infrastructure.DependencyInjection.DependencyInjection");
                if (infraTemplate is not null)
                {
                    var kernelConfig = application.GetApplicationConfig(sharedKernel.ApplicationId);
                    ((CSharpTemplateBase<object>)infraTemplate).AddAssemblyReference(new ProjectReference($"{kernelConfig.OutputLocation}\\{sharedKernel.ApplicationName}.Infrastructure\\{sharedKernel.ApplicationName}.Infrastructure.csproj"));
                }
            }

            //Domain
            {
                var domainTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.DomainEvents.DomainEventBase");
                if (domainTemplate is not null)
                {
                    var kernelConfig = application.GetApplicationConfig(sharedKernel.ApplicationId);
                    ((CSharpTemplateBase<object>)domainTemplate).AddAssemblyReference(new ProjectReference($"{kernelConfig.OutputLocation}\\{sharedKernel.ApplicationName}.Domain\\{sharedKernel.ApplicationName}.Domain.csproj"));
                }
            }

        }

        protected override void OnAfterCommitChanges(IApplication application)
        {
            base.OnAfterCommitChanges(application);
            var sharedKernel = TemplateHelper.GetSharedKernel();
            var rawfiles = Directory.GetFiles(Path.GetFullPath(application.OutputRootDirectory).Replace(@"\", "/"), "*.*");

            var slnFileNames = rawfiles.Where(f => f.EndsWith(".sln"));
            if (slnFileNames.Count() != 1)
            {
                return;
            }

            string targetFile = slnFileNames.First();
            if (!File.Exists(targetFile))
            {
                return;
            }
            var slnFile = SlnFile.Read(targetFile, File.ReadAllText(targetFile));
            bool changes = false;

            changes |= SynchroniseSharedProjects(application, slnFile, sharedKernel);

            if (changes)
            {
                File.WriteAllText(targetFile, slnFile.Generate());
            }
        }

        private bool SynchroniseSharedProjects(IApplication application, SlnFile slnFile, SharedKernel sharedKernel)
        {
            var sharedFolder = slnFile.GetOrCreateFolder("2150E333-8FDC-42A3-9474-1A3956D46DE8", "0 - Shared Kernel");           
            //Only doing this once off if they not there add them
            var solutionItemsProject = slnFile.Projects.FirstOrDefault(p => p.Name == $"{sharedKernel.ApplicationName}.Domain");
            if (solutionItemsProject != null)
            {
                return false;
            }
            var kernelConfig = application.GetApplicationConfig(sharedKernel.ApplicationId);

            CreateProject(slnFile, "BE69BA22-8613-4F7E-A3F5-EB737D31A3BA", $"{sharedKernel.ApplicationName}.Application", "9A19103F-16F7-4668-BE54-9A1E7A4F7556", $"{kernelConfig.OutputLocation.Replace(@"..\..\", @"..\")}\\{sharedKernel.ApplicationName}.Application\\{sharedKernel.ApplicationName}.Application.csproj", sharedFolder);
            CreateProject(slnFile, "5E293F1C-FFA1-49E1-B583-4CD13E39ABE3", $"{sharedKernel.ApplicationName}.Domain", "9A19103F-16F7-4668-BE54-9A1E7A4F7556", $"{kernelConfig.OutputLocation.Replace(@"..\..\", @"..\")}\\{sharedKernel.ApplicationName}.Domain\\{sharedKernel.ApplicationName}.Domain.csproj", sharedFolder);
            CreateProject(slnFile, "5901A0A9-37DB-4119-B149-C1F4E165883C", $"{sharedKernel.ApplicationName}.Infrastructure", "9A19103F-16F7-4668-BE54-9A1E7A4F7556", $"{kernelConfig.OutputLocation.Replace(@"..\..\", @"..\")}\\{sharedKernel.ApplicationName}.Infrastructure\\{sharedKernel.ApplicationName}.Infrastructure.csproj", sharedFolder);

            return true;
        }

        private static void CreateProject(
            SlnFile slnFile,
            string id,
            string name,
            string typeGuid,
            string filePath,
            SlnProject parent)
        {
            var project = slnFile.Projects.CreateProject(id, name, typeGuid, filePath, parent);
            foreach (var configuration in slnFile.SolutionConfigurationsSection.Values)
            {
                var propertySet = slnFile.ProjectConfigurationsSection.GetOrCreatePropertySet(project.Id);
                propertySet.TryAdd($"{configuration}.ActiveCfg", configuration);
                propertySet.TryAdd($"{configuration}.Build.0", configuration);
            }
        }

    }
}