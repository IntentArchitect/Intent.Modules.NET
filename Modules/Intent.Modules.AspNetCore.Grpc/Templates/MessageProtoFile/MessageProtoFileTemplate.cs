using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates.MessageProtoFile
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessageProtoFileTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var sb = new StringBuilder();

            sb.AppendLine("""syntax = "proto3";""");
            sb.AppendLine();

            if (Imports.Count > 0)
            {
                foreach (var import in Imports)
                {
                    sb.AppendLine($"""import "{import}";""");
                }

                sb.AppendLine();
            }

            sb.AppendLine($"""option csharp_namespace = "{CSharpNamespace}";""");
            sb.AppendLine();
            sb.AppendLine($"package {Package};");

            sb.AppendJoin(null, _types.Values);

            return sb.ToString();
        }
    }
}