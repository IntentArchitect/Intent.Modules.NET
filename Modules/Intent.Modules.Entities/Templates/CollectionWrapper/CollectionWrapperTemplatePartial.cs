using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Entities.Templates.CollectionWrapper
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CollectionWrapperTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Entities.CollectionWrapper";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CollectionWrapperTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddClass(@"CollectionWrapper", @class =>
                {
                    @class.AddGenericParameter("TInterface", out var TInterface);
                    @class.AddGenericParameter("TImplementation", out var TImplementation);
                    @class.AddGenericTypeConstraint(TImplementation, type => type.AddType(TInterface));
                    @class.WithComments($$"""
                                          /// <summary>
                                          /// Provides a wrapper over an <see cref="ICollection{{{TImplementation}}}"/> to make it behave as an <see cref="ICollection{{{TInterface}}}"/>.  
                                          /// </summary>
                                          /// <typeparam name="{{TInterface}}">The interface type the collection should be exposed as.</typeparam>
                                          /// <typeparam name="{{TImplementation}}">The actual type of the items in the collection. Must implement <typeparamref name="{{TInterface}}"/>.</typeparam>
                                          """);
                    @class.ImplementsInterface($"ICollection<{TInterface}>");

                    @class.AddField($"ICollection<{TImplementation}>", "_wrappedCollection", field =>
                    {
                        field.PrivateReadOnly();
                    });

                    @class.AddConstructor(ctor =>
                    {
                        ctor.WithComments($$"""
                                          /// <summary>
                                          /// Initializes a new instance of the <see cref="CollectionWrapper{{{TInterface}}, {{TImplementation}}}"/> class.
                                          /// </summary>
                                          /// <param name="wrappedCollection">The collection to be wrapped.</param>
                                          """);
                        ctor.AddParameter($"ICollection<{TImplementation}>", "wrappedCollection");
                        ctor.AddStatement("_wrappedCollection = wrappedCollection;");
                    });

                    @class.AddMethod($"IEnumerator<{TInterface}>", "GetEnumerator", method =>
                    {
                        method.WithComments("/// <inheritdoc />");
                        method.AddStatement(@"return _wrappedCollection.Cast<TInterface>().GetEnumerator();");
                    });

                    @class.AddMethod("IEnumerator", "GetEnumerator", method =>
                    {
                        method.WithComments("/// <inheritdoc />");
                        method.WithExpressionBody("GetEnumerator()");
                        method.AddMetadata("explicit-interface", "IEnumerable");
                    });

                    @class.AddMethod("void", "Add", method =>
                    {
                        method.WithComments("/// <inheritdoc />");
                        method.AddParameter("TInterface", "item");
                        method.AddStatement(@"_wrappedCollection.Add((TImplementation)item!);");
                    });

                    @class.AddMethod("void", "Clear", method =>
                    {
                        method.WithComments("/// <inheritdoc />");
                        method.AddStatement(@"_wrappedCollection.Clear();");
                    });

                    @class.AddMethod("bool", "Contains", method =>
                    {
                        method.WithComments("/// <inheritdoc />");
                        method.AddParameter("TInterface", "item");
                        method.AddStatement(@"return _wrappedCollection.Contains((TImplementation)item!);");
                    });

                    @class.AddMethod("void", "CopyTo", method =>
                    {
                        method.WithComments("/// <inheritdoc />");
                        method.AddParameter("TInterface[]", "array");
                        method.AddParameter("int", "arrayIndex");
                        method.AddStatement(@"_wrappedCollection.Cast<TInterface>().ToArray().CopyTo(array, arrayIndex);");
                    });

                    @class.AddMethod("bool", "Remove", method =>
                    {
                        method.WithComments("/// <inheritdoc />");
                        method.AddParameter("TInterface", "item");
                        method.AddStatement(@"return _wrappedCollection.Remove((TImplementation)item!);");
                    });

                    @class.AddProperty("int", "Count", property =>
                    {
                        property.WithComments("/// <inheritdoc />");
                        property.WithoutSetter();
                        property.Getter.WithExpressionImplementation("_wrappedCollection.Count");
                    });

                    @class.AddProperty("bool", "IsReadOnly", property =>
                    {
                        property.WithComments("/// <inheritdoc />");
                        property.WithoutSetter();
                        property.Getter.WithExpressionImplementation("_wrappedCollection.IsReadOnly");
                    });
                })
                .AddClass(@"CollectionWrapperExtensions", @class =>
                {
                    @class.Static();
                    @class.WithComments("""
                                        /// <summary>
                                        /// Provides extension methods for <see cref="ICollection{T}"/>.
                                        /// </summary>
                                        """);

                    @class.AddMethod("ICollection<TInterface>", "CreateWrapper", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("TInterface", out var TInterface);
                        method.AddGenericParameter("TImplementation", out var TImplementation);
                        method.AddGenericTypeConstraint(TImplementation, type => type.AddType(TInterface));
                        method.WithComments($$"""
                                            /// <summary>
                                            /// Creates a wrapper for a collection to expose it as a different interface.
                                            /// </summary>
                                            /// <typeparam name="{{TInterface}}">The interface type the collection should be exposed as.</typeparam>
                                            /// <typeparam name="{{TImplementation}}">The actual type of the items in the collection. Must implement <typeparamref name="{{TInterface}}"/>.</typeparam>
                                            /// <param name="collection">The collection to be wrapped.</param>
                                            /// <returns>An <see cref="ICollection{{{TInterface}}}"/> that wraps the provided collection.</returns>
                                            """);
                        method.AddParameter("ICollection<TImplementation>", "collection", param => param.WithThisModifier());
                        method.AddStatement(@"return new CollectionWrapper<TInterface, TImplementation>(collection);");
                    });
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

        public override bool CanRunTemplate()
        {
            return ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                   !ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters();
        }
    }
}