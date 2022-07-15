using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Templates.ValidationBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ValidationBehaviourTemplate : CSharpTemplateBase<object, ValidationBehaviourContract>
    {
        public const string TemplateId = "Intent.Application.FluentValidation.ValidationBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValidationBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.FluentValidation);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ValidationBehaviour",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetGenericTypeParameters()
        {
            var genericTypes = new List<string>();

            genericTypes.Add("TRequest");
            genericTypes.AddRange(GetDecorators().SelectMany(s => s.GetGenericTypeParameters()));

            return genericTypes.Any() ? $"<{string.Join(", ", genericTypes)}>" : string.Empty;
        }

        private string GetInheritanceDeclarations()
        {
            var inheritanceTypes = new List<string>();

            inheritanceTypes.AddRange(GetDecorators().SelectMany(s => s.GetInheritanceDeclarations()));

            return inheritanceTypes.Any() ? $" : {string.Join(", ", inheritanceTypes)}" : string.Empty;
        }

        private string GetGenericTypeConstraints()
        {
            var genericTypeConstraints = new List<string>();

            genericTypeConstraints.AddRange(GetDecorators().SelectMany(s => s.GetGenericTypeConstraints()));

            const string newLine = @"
        ";
            return newLine + string.Join(newLine, genericTypeConstraints);
        }

        private string GetHandleReturnType()
        {
            var returnTypes = GetDecorators()
                .Where(p => p.GetHandleReturnType() != null)
                .Select(s => s.GetHandleReturnType())
                .ToArray();
            if (returnTypes.Length > 1)
            {
                throw new Exception("More than one Decorator is trying to set the Handle Return Type");
            }

            var returnType = returnTypes.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(returnType))
            {
                return $"Task<{returnType}>";
            }

            return "Task";
        }

        private string GetHandleParameterList()
        {
            var paramList = new List<string>();

            paramList.Add("TRequest request");
            paramList.Add("CancellationToken cancellationToken");
            paramList.AddRange(GetDecorators().SelectMany(s => s.GetHandleParameterList()));

            return string.Join(", ", paramList);
        }

        private string GetValidationException()
        {
            return "ValidationException";
        }

        private string GetHandleExitStatementList()
        {
            var statementList = new List<string>();

            statementList.AddRange(GetDecorators().SelectMany(s => s.GetHandleExitStatementList()));

            const string newLine = @"
            ";
            return string.Join(newLine, statementList);
        }
    }
}