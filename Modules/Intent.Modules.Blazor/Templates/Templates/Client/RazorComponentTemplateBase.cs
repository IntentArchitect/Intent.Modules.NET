using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Api.Mappings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Blazor.Templates.Templates.Client;

public abstract class RazorComponentTemplateBase<TModel> : RazorTemplateBase<TModel>, IRazorComponentTemplate
    where TModel : IElementWrapper, IMetadataModel
{
    private readonly Queue<Action<ICSharpFileBuilderTemplate>> _onCodeBehindSetActions = [];
    private IBuildsCSharpMembers? _codeBehind;

    protected RazorComponentTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
    {
        BindingManager = new BindingManager(this, Model.InternalElement.Mappings.FirstOrDefault());
        ComponentBuilderProvider = DefaultRazorComponentBuilderProvider.Create(this);
    }

    public CSharpClassMappingManager CreateMappingManager()
    {
        var template = CodeBehindTemplate ?? (ICSharpTemplate)this;

        var mappingManager = new CSharpClassMappingManager(template);
        mappingManager.AddMappingResolver(new CallServiceOperationMappingResolver(template));
        mappingManager.AddMappingResolver(new PropertyCollectionMappingResolver(template));
        mappingManager.AddMappingResolver(new RazorBindingMappingResolver(template));
        mappingManager.SetFromReplacement(Model, null);
        mappingManager.SetToReplacement(Model, null);
        return mappingManager;
    }

    public override ICSharpCodeContext RootCodeContext => GetCodeBehind();

    public BindingManager BindingManager { get; }
    public abstract IRazorFile RazorFile { get; }
    public IRazorComponentBuilderProvider ComponentBuilderProvider { get; }

    public IBuildsCSharpMembers GetCodeBehind()
    {
        if (_codeBehind != null)
        {
            return _codeBehind;
        }

        if (CodeBehindTemplate != null)
        {

            _codeBehind = CodeBehindTemplate.CSharpFile.Classes.First();// IRazorComponentClass.CreateForCodeBehindFile(CodeBehindTemplate);
        }
        else
        {
            RazorFile.AddCodeBlock(x => _codeBehind = x);
        }

        return _codeBehind!;
    }

    /// <summary>
    /// The template instance which will generate the code behind file if the setting to do so is enabled.
    /// </summary>
    public ICSharpFileBuilderTemplate? CodeBehindTemplate { get; private set; }

    public override void AfterTemplateRegistration()
    {
        base.AfterTemplateRegistration();

        CodeBehindTemplate = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(CodeBehindTemplateId, Model);
        if (CodeBehindTemplate == null)
        {
            return;
        }

        while (_onCodeBehindSetActions.TryDequeue(out var action))
        {
            action(CodeBehindTemplate);
        }
    }

    public sealed override void SetDefaultTypeCollectionFormat(string collectionFormat)
    {
        OnCodeBehind(codeBehindTemplate => codeBehindTemplate.SetDefaultTypeCollectionFormat(collectionFormat));
        base.SetDefaultTypeCollectionFormat(collectionFormat);
    }

    public sealed override void SetDefaultCollectionFormatter(ICollectionFormatter collectionFormatter)
    {
        OnCodeBehind(codeBehindTemplate => codeBehindTemplate.SetDefaultCollectionFormatter(collectionFormatter));
        base.SetDefaultCollectionFormatter(collectionFormatter);
    }

    /// <remarks>
    /// This explicit implementation is because <see cref="CSharpTemplateBase{TModel}"/> has this as a "new" member
    /// for backwards compatibility.
    /// </remarks>>
    ClassTypeSource IIntentTemplate.AddTypeSource(string templateId)
    {
        return AddTypeSource(templateId);
    }

    public override ClassTypeSource AddTypeSource(string templateId)
    {
        OnCodeBehind(codeBehindTemplate => codeBehindTemplate.AddTypeSource(templateId));
        return base.AddTypeSource(templateId);
    }

    /// <remarks>
    /// This explicit implementation is because <see cref="CSharpTemplateBase{TModel}"/> has this as a "new" member
    /// for backwards compatibility.
    /// </remarks>>
    ClassTypeSource IIntentTemplate.AddTypeSource(string templateId, string collectionFormat)
    {
        OnCodeBehind(codeBehindTemplate => codeBehindTemplate.AddTypeSource(templateId, collectionFormat));
        return ((IntentTemplateBase<TModel>)this).AddTypeSource(templateId, collectionFormat);
    }

    public sealed override void AddTypeSource(string templateId, string collectionFormat)
    {
        OnCodeBehind(codeBehindTemplate => codeBehindTemplate.AddTypeSource(templateId, collectionFormat));
        base.AddTypeSource(templateId, collectionFormat);
    }

    protected abstract string CodeBehindTemplateId { get; }

    /// <summary>
    /// Allows actions to be performed against <see cref="CodeBehindTemplate"/> even from
    /// constructors by either executing the <paramref name="action"/> immediately when
    /// <see cref="CodeBehindTemplate"/> is not <see langword="null"/> or otherwise enqueues
    /// the <paramref name="action"/> to be performed once <see cref="CodeBehindTemplate"/> is
    /// set.
    /// </summary>
    private void OnCodeBehind(Action<ICSharpFileBuilderTemplate> action)
    {
        if (CodeBehindTemplate != null)
        {
            action(CodeBehindTemplate);
            return;
        }

        _onCodeBehindSetActions.Enqueue(action);
    }
}