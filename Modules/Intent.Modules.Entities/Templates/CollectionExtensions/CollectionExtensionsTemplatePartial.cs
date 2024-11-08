using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Entities.Templates.CollectionExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CollectionExtensionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Entities.CollectionExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CollectionExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddUsing("System");
            AddUsing("System.Linq");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("CollectionExtensions", (Action<CSharpClass>)(@class =>
                {
                    @class.Static();
                    @class.WithComments(@"/// <summary>
    /// Provides extension methods for collection objects.
    /// </summary>");

                    @class.AddMethod("ComparisonResult<TChanged, TOriginal>", "CompareCollections", method =>
                    {
                        method.WithComments(GetCompareCollectionsMethodDocs());
                        method.Static();

                        method.AddGenericParameter("TChanged")
                        .AddGenericParameter("TOriginal");

                        method.AddParameter(UseType("System.Collections.Generic.ICollection<TOriginal>"), "baseCollection", cfg => cfg.WithThisModifier())
                            .AddParameter(UseType("System.Collections.Generic.IEnumerable<TChanged>"), "changedCollection")
                            .AddParameter("Func<TOriginal, TChanged, bool>", "equalityCheck");

                        method.AddStatement("changedCollection ??= new List<TChanged>();", cfg => cfg.SeparatedFromNext());

                        method.AddObjectInitStatement("var toRemove", "baseCollection.Where(baseElement => changedCollection.All(changedElement => !equalityCheck(baseElement, changedElement))).ToList();");
                        method.AddObjectInitStatement("var toAdd", "changedCollection.Where(changedElement => baseCollection.All(baseElement => !equalityCheck(baseElement, changedElement))).ToList();", cfg => cfg.SeparatedFromNext());

                        method.AddObjectInitStatement("var possibleEdits", "new List<Match<TChanged, TOriginal>>();");
                        method.AddForEachStatement("changedElement", "changedCollection", @iteration =>
                        {
                            iteration.AddObjectInitStatement("var match", "baseCollection.FirstOrDefault(baseElement => equalityCheck(baseElement, changedElement));");
                            iteration.AddIfStatement("match is not null", @if =>
                            {
                                @if.AddInvocationStatement("possibleEdits.Add", invoc =>
                                {
                                    invoc.AddArgument("new Match<TChanged, TOriginal>(changedElement, match)");
                                });
                            });
                        });

                        method.AddReturn(new CSharpInvocationStatement("new ComparisonResult<TChanged, TOriginal>")
                            .AddArgument("toAdd")
                            .AddArgument("toRemove")
                            .AddArgument("possibleEdits")
                            .WithoutSemicolon());
                    });

                    AddNestedResultsClass(@class);

                    @class.AddNestedClass("Match", matchClass =>
                    {
                        matchClass.AddGenericParameter("TChanged")
                            .AddGenericParameter("TOriginal");

                        matchClass.WithComments(@"/// <summary>
        /// Represents a matched pair of changed and original elements.
        /// </summary>
        /// <typeparam name=""TChanged"">The type of the changed element.</typeparam>
        /// <typeparam name=""TOriginal"">The type of the original element.</typeparam>");

                        matchClass.AddConstructor(ctor =>
                        {
                            ctor.WithComments(@"/// <summary>
            /// Initializes a new instance of the <see cref=""Match{TChanged, TOriginal}""/> class.
            /// </summary>
            /// <param name=""changed"">The changed element.</param>
            /// <param name=""original"">The original element.</param>");

                            AddMatchConstructorParameters(ctor);
                        });
                    });

                }));
        }

        private static void AddNestedResultsClass(CSharpClass @class)
        {
            @class.AddNestedClass("ComparisonResult", resultClass =>
            {
                resultClass.AddGenericParameter("TChanged")
                    .AddGenericParameter("TOriginal");

                resultClass.WithComments(@"/// <summary>
        /// Represents the result of comparing two collections.
        /// </summary>
        /// <typeparam name=""TChanged"">The type of elements that have changed.</typeparam>
        /// <typeparam name=""TOriginal"">The type of original elements.</typeparam>");

                resultClass.AddConstructor(ctor =>
                {
                    ctor.WithComments(@"/// <summary>
            /// Initializes a new instance of the <see cref=""ComparisonResult{TChanged, TOriginal}""/> class.
            /// </summary>
            /// <param name=""toAdd"">A collection of elements to be added.</param>
            /// <param name=""toRemove"">A collection of elements to be removed.</param>
            /// <param name=""possibleEdits"">A collection of matched elements that might have edits.</param>");

                    AddResultConstructorParameters(ctor);

                    resultClass.AddMethod("bool", "HasChanges", method =>
                    {
                        method.WithComments(@"/// <summary>
            /// Determines whether there are any changes between the two collections.
            /// </summary>
            /// <returns><see langword=""true"" /> if there are changes; otherwise, <see langword=""false"" />.</returns>");

                        method.AddReturn("ToAdd.Count > 0 || ToRemove.Count > 0 || PossibleEdits.Count > 0");
                    });
                });
            });
        }

        private static void AddResultConstructorParameters(CSharpConstructor ctor)
        {
            ctor.AddParameter("ICollection<TChanged>", "toAdd", p =>
            {
                p.IntroduceProperty(pConf =>
                {
                    pConf.WithoutSetter();
                    pConf.WithComments(@"/// <summary>
            /// Gets the collection of elements to be added.
            /// </summary>");
                });
            })
                .AddParameter("ICollection<TOriginal>", "toRemove", p =>
                {
                    p.IntroduceProperty(pConf =>
                    {
                        pConf.WithoutSetter();
                        pConf.WithComments(@"/// <summary>
            /// Gets the collection of elements to be removed.
            /// </summary>");
                    });
                })
                .AddParameter("ICollection<Match<TChanged, TOriginal>>", "possibleEdits", p =>
                {
                    p.IntroduceProperty(pConf =>
                    {
                        pConf.WithoutSetter();
                        pConf.WithComments(@"/// <summary>
            /// Gets the collection of matched elements that might have edits.
            /// </summary>");
                    });
                });
        }

        private static void AddMatchConstructorParameters(CSharpConstructor ctor)
        {
            ctor.AddParameter("TChanged", "changed", p =>
            {
                p.IntroduceProperty(pConf =>
                {
                    pConf.PrivateSetter();
                    pConf.WithComments(@"/// <summary>
            /// Gets the changed element.
            /// </summary>");
                });
            })
                .AddParameter("TOriginal", "original", p =>
                {
                    p.IntroduceProperty(pConf =>
                    {
                        pConf.PrivateSetter();
                        pConf.WithComments(@"/// <summary>
            /// Gets the original element.
            /// </summary>");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"CollectionExtensions",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private string GetCompareCollectionsMethodDocs() =>
           @"/// <summary>
        /// Compares two collections and returns a result indicating the differences between them.
        /// </summary>
        /// <typeparam name=""TChanged"">The type of elements in the changed collection.</typeparam>
        /// <typeparam name=""TOriginal"">The type of elements in the base collection.</typeparam>
        /// <param name=""baseCollection"">The base collection to compare.</param>
        /// <param name=""changedCollection"">The changed collection to compare against the base collection.</param>
        /// <param name=""equalityCheck"">A predicate to determine if two elements are equal.</param>
        /// <returns>A <see cref=""ComparisonResult{TChanged, TOriginal}""/> object that describes the differences between the two collections.</returns>";
    }
}