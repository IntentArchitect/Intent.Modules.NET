using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class VisualStudioSolutionModelStereotypeExtensions
    {
        public static VisualStudioSolutionOptions GetVisualStudioSolutionOptions(this VisualStudioSolutionModel model)
        {
            var stereotype = model.GetStereotype(VisualStudioSolutionOptions.DefinitionId);
            return stereotype != null ? new VisualStudioSolutionOptions(stereotype) : null;
        }


        public static bool HasVisualStudioSolutionOptions(this VisualStudioSolutionModel model)
        {
            return model.HasStereotype(VisualStudioSolutionOptions.DefinitionId);
        }

        public static bool TryGetVisualStudioSolutionOptions(this VisualStudioSolutionModel model, out VisualStudioSolutionOptions stereotype)
        {
            if (!HasVisualStudioSolutionOptions(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new VisualStudioSolutionOptions(model.GetStereotype(VisualStudioSolutionOptions.DefinitionId));
            return true;
        }

        public class VisualStudioSolutionOptions
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "90085088-b29b-4386-b87e-f7f62c76c5de";

            public VisualStudioSolutionOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string SolutionName()
            {
                return _stereotype.GetProperty<string>("Solution Name");
            }

            public string SolutionRelativeLocation()
            {
                return _stereotype.GetProperty<string>("Solution Relative Location");
            }

            public bool ManagePackageVersionsCentrally()
            {
                return _stereotype.GetProperty<bool>("Manage Package Versions Centrally");
            }

            public OutputLocationOptions OutputLocation()
            {
                return new OutputLocationOptions(_stereotype.GetProperty<string>("Output Location"));
            }

            public bool OnlyCheckCurrentGitRepository()
            {
                return _stereotype.GetProperty<bool>("Only check current Git repository");
            }

            public string RelativePath()
            {
                return _stereotype.GetProperty<string>("Relative Path");
            }

            public class OutputLocationOptions
            {
                public readonly string Value;

                public OutputLocationOptions(string value)
                {
                    Value = value;
                }

                public OutputLocationOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Same as sln file":
                            return OutputLocationOptionsEnum.SameAsSlnFile;
                        case "Check Parent Folders":
                            return OutputLocationOptionsEnum.CheckParentFolders;
                        case "Relative Path":
                            return OutputLocationOptionsEnum.RelativePath;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsSameAsSlnFile()
                {
                    return Value == "Same as sln file";
                }
                public bool IsCheckParentFolders()
                {
                    return Value == "Check Parent Folders";
                }
                public bool IsRelativePath()
                {
                    return Value == "Relative Path";
                }
            }

            public enum OutputLocationOptionsEnum
            {
                SameAsSlnFile,
                CheckParentFolders,
                RelativePath
            }

        }

    }
}