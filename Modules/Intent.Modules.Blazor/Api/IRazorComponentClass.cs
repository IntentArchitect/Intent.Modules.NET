using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Api;

public interface IRazorComponentClass : IBuildsCSharpMembers
{
    IRazorComponentClass AddInjectedProperty(string fullyQualifiedTypeName, string? propertyName = null);

    static IRazorComponentClass CreateForCodeBehindFile(ICSharpFileBuilderTemplate template)
    {
        var @class = template.CSharpFile.Classes[0];

        return new WrapperImplementation(
            buildsCSharpMembers: @class,
            addInjectedProperty: (fullyQualifiedTypeName, propertyName) =>
            {
                var type = template.UseType(fullyQualifiedTypeName);
                if (@class.Properties.Any(x => x.Type == type))
                {
                    return;
                }
                @class.AddProperty(
                    type: type,
                    name: propertyName ?? type,
                    configure: property =>
                    {
                        property.AddAttribute(template.UseType("Microsoft.AspNetCore.Components.Inject"));
                        property.WithInitialValue("default!");
                    });
            });
    }

    static IRazorComponentClass CreateForCodeBlock(IRazorCodeBlock razorCodeBlock)
    {
        return new WrapperImplementation(
            buildsCSharpMembers: razorCodeBlock,
            addInjectedProperty: (fullyQualifiedTypeName, propertyName) =>
            {
                razorCodeBlock.RazorFile.AddInjectDirective(fullyQualifiedTypeName, propertyName);
            });
    }

    private class WrapperImplementation(IBuildsCSharpMembers buildsCSharpMembers, Action<string, string?> addInjectedProperty) : IRazorComponentClass
    {
        public IRazorComponentClass AddInjectedProperty(string fullyQualifiedTypeName, string? propertyName = null)
        {
            addInjectedProperty(fullyQualifiedTypeName, propertyName);
            return this;
        }

        #region ICSharpCodeContext implementation

        void ICSharpCodeContext.RepresentsModel(IMetadataModel model) => buildsCSharpMembers.RepresentsModel(model);
        ICSharpCodeContext ICSharpCodeContext.AddMetadata(string key, object value) => buildsCSharpMembers.AddMetadata(key, value);
        bool ICSharpCodeContext.HasMetadata(string key) => buildsCSharpMembers.HasMetadata(key);
        T ICSharpCodeContext.GetMetadata<T>(string key) => buildsCSharpMembers.GetMetadata<T>(key);
        object ICSharpCodeContext.GetMetadata(string key) => buildsCSharpMembers.GetMetadata(key);
        bool ICSharpCodeContext.TryGetMetadata<T>(string key, out T value) => buildsCSharpMembers.TryGetMetadata(key, out value);
        bool ICSharpCodeContext.TryGetMetadata(string key, out object value) => buildsCSharpMembers.TryGetMetadata(key, out value);
        IHasCSharpName ICSharpCodeContext.GetReferenceForModel(string modelId) => buildsCSharpMembers.GetReferenceForModel(modelId);
        IHasCSharpName ICSharpCodeContext.GetReferenceForModel(IMetadataModel model) => buildsCSharpMembers.GetReferenceForModel(model);
        bool ICSharpCodeContext.TryGetReferenceForModel(string modelId, out IHasCSharpName reference) => buildsCSharpMembers.TryGetReferenceForModel(modelId, out reference);
        bool ICSharpCodeContext.TryGetReferenceForModel(IMetadataModel model, out IHasCSharpName reference) => buildsCSharpMembers.TryGetReferenceForModel(model, out reference);
        void ICSharpCodeContext.RegisterReferenceable(string modelId, ICSharpReferenceable cSharpReferenceable) => buildsCSharpMembers.RegisterReferenceable(modelId, cSharpReferenceable);
        ICSharpFile ICSharpCodeContext.File => buildsCSharpMembers.File;
        ICSharpCodeContext ICSharpCodeContext.Parent => buildsCSharpMembers.Parent;

        #endregion

        #region IBuildsCSharpMembers implementation

        IList<ICodeBlock> IBuildsCSharpMembers.Declarations => buildsCSharpMembers.Declarations;
        IBuildsCSharpMembers IBuildsCSharpMembers.InsertField(int index, string type, string name, Action<ICSharpField> configure) => buildsCSharpMembers.InsertField(index, type, name, configure);
        IBuildsCSharpMembers IBuildsCSharpMembers.AddField(string type, string name, Action<ICSharpField> configure) => buildsCSharpMembers.AddField(type, name, configure);
        IBuildsCSharpMembers IBuildsCSharpMembers.InsertProperty(int index, string type, string name, Action<ICSharpProperty> configure) => buildsCSharpMembers.InsertProperty(index, type, name, configure);
        IBuildsCSharpMembers IBuildsCSharpMembers.AddProperty(string type, string name, Action<ICSharpProperty> configure) => buildsCSharpMembers.AddProperty(type, name, configure);
        IBuildsCSharpMembers IBuildsCSharpMembers.InsertMethod(int index, string returnType, string name, Action<ICSharpClassMethod> configure) => buildsCSharpMembers.InsertMethod(index, returnType, name, configure);
        IBuildsCSharpMembers IBuildsCSharpMembers.AddMethod(string returnType, string name, Action<ICSharpClassMethod> configure) => buildsCSharpMembers.AddMethod(returnType, name, configure);
        IBuildsCSharpMembers IBuildsCSharpMembers.AddClass(string name, Action<ICSharpClass> configure) => buildsCSharpMembers.AddClass(name, configure);
        int IBuildsCSharpMembers.IndexOf(ICodeBlock codeBlock) => buildsCSharpMembers.IndexOf(codeBlock);
        ICSharpTemplate IBuildsCSharpMembers.Template => buildsCSharpMembers.Template;

        #endregion
    }
}