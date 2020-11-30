using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.AspNetCore.Templates.Program
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ProgramTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => ProgramTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget project)
        {
            return new ProgramTemplate(project);
        }

        //public void DoRegistration(ITemplateInstanceRegistry registery, IApplication application)
        //{
        //    var targetProjectIds = new List<string>
        //    {
        //        VisualStudioProjectTypeIds.CoreWebApp
        //    };

        //    var projects = application.Projects.Where(p => targetProjectIds.Contains(p.ProjectType.Id));

        //    foreach (var project in projects)
        //    {
        //        registery.Register(TemplateId, project, p => new CoreWebProgramTemplate(project));
        //    }
        //}
    }
}
