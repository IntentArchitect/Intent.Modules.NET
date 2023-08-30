using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ConfigurationTemplates.PubSubOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PubSubOptionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.GoogleCloud.PubSub.ConfigurationTemplates.PubSubOptions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PubSubOptionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"PubSubOptions")
                .OnBuild(file =>
                {
                    file.AddUsing("Google.Api.Gax");

                    var priClass = file.Classes.First();
                    priClass.AddProperty("bool", "UseMetadataServer");
                    priClass.AddProperty("bool", "ShouldSetupCloudResources");
                    priClass.AddProperty("string", "ProjectId");
                    priClass.AddProperty("bool", "UsePubSubEmulator");
                    priClass.AddProperty("bool", "ShouldAuthorizePushNotification");
                    priClass.AddProperty("string", "VerificationToken");
                    priClass.AddMethod("EmulatorDetection", "GetEmulatorDetectionMode", method =>
                    {
                        method.AddStatement($"return UsePubSubEmulator ? EmulatorDetection.EmulatorOnly : EmulatorDetection.None;");
                    });
                    priClass.AddMethod("bool", "HasVerificationToken", method =>
                    {
                        method.AddStatement($"return !string.IsNullOrWhiteSpace(VerificationToken);");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            this.ApplyAppSetting("GoogleCloudPubSub:UseMetadataServer", false);
            this.ApplyAppSetting("GoogleCloudPubSub:ProjectId", $"x-object-xyz");
            this.ApplyAppSetting("GoogleCloudPubSub:ShouldSetupCloudResources", true);
            this.ApplyAppSetting("GoogleCloudPubSub:UsePubSubEmulator", false);
            this.ApplyAppSetting("GoogleCloudPubSub:ShouldAuthorizePushNotification", true);
            this.ApplyAppSetting("GoogleCloudPubSub:VerificationToken", "your-own-secret-string");

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister(extensionMethodName: "AddOptions")
                .RequiresUsingNamespaces("Microsoft.Extensions.DependencyInjection")
                .WithPriority(-1100));
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}