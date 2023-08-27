using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Configuration.Templates.DefaultLocalConfigurationStore
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DefaultLocalConfigurationStoreTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return @$"apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: configuration-store
spec:
  type: configuration.redis
  version: v1
  metadata:
  - name: redisHost
    value: localhost:6379
  - name: redisPassword
    value: """"
  - name: actorStateStore
    value: ""true""";
        }
    }
}