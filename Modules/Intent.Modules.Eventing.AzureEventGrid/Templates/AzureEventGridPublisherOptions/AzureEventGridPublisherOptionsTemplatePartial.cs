using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.IaC.Shared.AzureEventGrid;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridPublisherOptions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class AzureEventGridPublisherOptionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Eventing.AzureEventGrid.AzureEventGridPublisherOptions";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public AzureEventGridPublisherOptionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Collections.Generic");

        var publishEvents = IntegrationManager.Instance.GetPublishedAzureEventGridMessages(ExecutionContext.GetApplicationConfig().Id);
        var hasCustomTopics = publishEvents.Any(x => x.DomainName is null);
        var hasEventDomainTopics = publishEvents.Any(x => x.DomainName is not null);
        
        CSharpFile.AddClass("AzureEventGridPublisherOptions", @class =>
            {
                @class.AddField("List<AzureEventGridPublisherEntry>", "_entries", field => field.PrivateReadOnly().WithAssignment("[]"));
                @class.AddProperty("IReadOnlyList<AzureEventGridPublisherEntry>", "Entries", prop => prop.WithoutSetter().Getter.WithExpressionImplementation("_entries"));

                if (hasCustomTopics)
                {
                    @class.AddMethod("void", "AddTopic", method =>
                    {
                        method.AddGenericParameter("TMessage", out var tMessage)
                            .AddParameter("string", "credentialKey")
                            .AddParameter("string", "endpoint")
                            .AddParameter("string", "source");

                        method.AddStatement("ArgumentNullException.ThrowIfNull(credentialKey);");
                        method.AddStatement("ArgumentNullException.ThrowIfNull(endpoint);");
                        method.AddStatement("ArgumentNullException.ThrowIfNull(source);");
                        method.AddStatement($"_entries.Add(new AzureEventGridPublisherEntry(typeof({tMessage}), credentialKey, endpoint, source));");
                    });
                }

                if (hasEventDomainTopics)
                {
                    @class.AddMethod("void", "AddDomain", method =>
                    {
                        method.AddParameter("string", "credentialKey")
                            .AddParameter("string", "endpoint")
                            .AddParameter("Action<DomainOptions>?", "domainAction");

                        method.AddStatement("ArgumentNullException.ThrowIfNull(credentialKey);");
                        method.AddStatement("ArgumentNullException.ThrowIfNull(endpoint);");
                        method.AddStatement("var domainOptions = new DomainOptions(credentialKey, endpoint);", s => s.SeparatedFromPrevious());
                        method.AddStatement("domainAction?.Invoke(domainOptions);");
                        method.AddStatement("_entries.AddRange(domainOptions.ToEntries());", s => s.SeparatedFromPrevious());
                    });
                }
            })
            .AddRecord("AzureEventGridPublisherEntry", record =>
            {
                record.AddPrimaryConstructor(ctor =>
                {
                    ctor.AddParameter("Type", "MessageType")
                        .AddParameter("string", "CredentialKey")
                        .AddParameter("string", "Endpoint")
                        .AddParameter("string", "Source");
                });
            });

        if (hasEventDomainTopics)
        {
            CSharpFile.AddClass("DomainOptions", @class =>
            {
                @class.AddField("string", "_credentialKey", field => field.PrivateReadOnly());
                @class.AddField("string", "_endpoint", field => field.PrivateReadOnly());
                @class.AddField("List<AzureEventGridPublisherEntry>", "_entries", field => field.PrivateReadOnly().WithAssignment("[]"));

                @class.AddConstructor(ctor =>
                {
                    ctor.AddParameter("string", "credentialKey")
                        .AddParameter("string", "endpoint");
                    ctor.AddStatement("_credentialKey = credentialKey;");
                    ctor.AddStatement("_endpoint = endpoint;");
                });

                @class.AddMethod("void", "Add", method =>
                {
                    method.AddGenericParameter("TMessage")
                        .AddParameter("string", "source");

                    method.AddStatement("ArgumentNullException.ThrowIfNull(source);");
                    method.AddStatement("_entries.Add(new AzureEventGridPublisherEntry(typeof(TMessage), _credentialKey, _endpoint, source));");
                });

                @class.AddMethod("IEnumerable<AzureEventGridPublisherEntry>", "ToEntries", method => { method.AddStatement("return _entries;"); });
            });
        }
    }

    [IntentManaged(Mode.Fully)]
    public CSharpFile CSharpFile { get; }

    [IntentManaged(Mode.Fully, Body = Mode.Fully)]
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