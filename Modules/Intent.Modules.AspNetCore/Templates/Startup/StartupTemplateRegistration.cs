using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.Templates.Program;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Constants;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.AspNetCore.Templates.Startup
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class StartupTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => StartupTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project)
        {
            return new StartupTemplate(project, null);
        }

        //public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        //{
        //    var targetProjectIds = new List<string>
        //    {
        //        VisualStudioProjectTypeIds.CoreWebApp
        //    };

        //    var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

        //    foreach (var project in projects)
        //    {
        //        registry.Register(TemplateId, project, p => new CoreWebStartupTemplate(project, application.EventDispatcher));
        //    }
        //}
    }
}
