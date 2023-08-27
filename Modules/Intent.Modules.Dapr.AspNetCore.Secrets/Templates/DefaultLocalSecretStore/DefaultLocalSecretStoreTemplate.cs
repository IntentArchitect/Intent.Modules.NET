using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Secrets.Templates.DefaultLocalSecretStore
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DefaultLocalSecretStoreTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return @$"apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: secret-store
spec:
  type: secretstores.local.env
  version: v1
  metadata:
";
        }
    }
}