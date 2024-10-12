using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Google.CloudStorage.Templates.BulkCloudObjectItem
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class BulkCloudObjectItemTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Google.CloudStorage.BulkCloudObjectItem";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BulkCloudObjectItemTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddRecord("BulkCloudObjectItem", @record =>
                {
                    @record.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "Name",
                            prm => prm.IntroduceProperty(p => p.Init()));

                        ctor.AddParameter(UseType($"System.IO.Stream"), "DataStream",
                            prm => prm.IntroduceProperty(p => p.Init()));

                        ctor.AddParameter(UseType($"string?"), "ContentType",
                            prm =>
                            {
                                prm.IntroduceProperty(p => p.Init());
                                prm.WithDefaultValue("null");
                            });

                        ctor.WithComments(
                            [
                            "/// <summary>",
                            "/// Constructor for the object which represents a single item used for bulk uploads to object storage",
                            "/// </summary>",
                            "/// <param name=\"Name\">The name of the object.</param>",
                            "/// <param name=\"DataStream\">The stream of data to upload.</param>",
                            "/// <param name=\"ContentType\">The content type of the object. This should be a MIME type. Can be null.</param>",
                        ]);
                    });

                    @record.WithComments(
               [
                        "/// <summary>",
                        "/// Represents a single item used for bulk uploads to object storage.",
                        "/// </summary>"
                    ]);

                });
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