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

namespace Intent.Modules.Entities.Templates.UpdateHelper
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class UpdateHelperTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Entities.UpdateHelper";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UpdateHelperTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddClass($"UpdateHelper", @class =>
                {
                    @class.Static();
                    @class.WithComments("""
                                        /// <summary>
                                        /// Provides utility methods for updating collections.
                                        /// </summary>
                                        """);

                    @class.AddMethod("ICollection<TOriginal>", "CreateOrUpdateCollection", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("TChanged", out var TChanged);
                        method.AddGenericParameter("TOriginal", out var TOriginal);
                        method.WithComments($$"""
                                            /// <summary>
                                            /// Performs mutations to synchronize the baseCollection to end up the same as the changedCollection.
                                            /// </summary>
                                            /// <typeparam name="{{TChanged}}">The type of items in the changed collection.</typeparam>
                                            /// <typeparam name="{{TOriginal}}">The type of items in the base collection.</typeparam>
                                            /// <param name="baseCollection">The base collection to be updated.</param>
                                            /// <param name="changedCollection">The collection containing the changes.</param>
                                            /// <param name="equalityCheck">A predicate that determines if an item from the base collection matches an item from the changed collection.</param>
                                            /// <param name="assignmentAction">A delegate that defines how to update an item from the base collection using an item from the changed collection.</param>
                                            /// <returns>The updated base collection.</returns>
                                            /// <remarks>
                                            /// If the changed collection is <see langword="null" />, an empty list of type <typeparamref name="{{TOriginal}}"/> will be returned.
                                            /// If the base collection is <see langword="null" />, a new list of type <typeparamref name="{{TOriginal}}"/> will be created and used.
                                            /// </remarks>
                                            """);
                        method.AddParameter($"ICollection<{TOriginal}>", "baseCollection");
                        method.AddParameter($"IEnumerable<{TChanged}>?", "changedCollection");
                        method.AddParameter($"Func<{TOriginal}, {TChanged}, bool>", "equalityCheck");
                        method.AddParameter($"Func<{TOriginal}?, {TChanged}, {TOriginal}>", "assignmentAction");
                        method.AddStatement($$"""
                                            if (changedCollection == null)
                                            {
                                                return new List<{{TOriginal}}>();
                                            }
                                            """);
                        method.AddStatement($"baseCollection ??= new List<{TOriginal}>()!;", st => st.SeparatedFromPrevious());
                        method.AddStatement("var result = baseCollection.CompareCollections(changedCollection, equalityCheck);", st => st.SeparatedFromPrevious());
                        method.AddStatement("""
                                            foreach (var elementToAdd in result.ToAdd)
                                            {
                                                var newEntity = assignmentAction(default, elementToAdd);
                                                baseCollection.Add(newEntity);
                                            }
                                            """);
                        method.AddStatement("""
                                            foreach (var elementToRemove in result.ToRemove)
                                            {
                                                baseCollection.Remove(elementToRemove);
                                            }
                                            """, st => st.SeparatedFromPrevious());
                        method.AddStatement("""
                                            foreach (var elementToEdit in result.PossibleEdits)
                                            {
                                                assignmentAction(elementToEdit.Original, elementToEdit.Changed);
                                            }
                                            """, st => st.SeparatedFromPrevious());
                        method.AddReturn("baseCollection", st => st.SeparatedFromPrevious());
                    });

                    @class.AddMethod("ICollection<TEntity>", "SynchronizeCollection", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("TEntity", out var TEntity);
                        method.WithComments($$"""
                                            /// <summary>
                                            /// Synchronizes a collection by adding and removing items to match the changed collection.
                                            /// This is useful for many-to-many relationships where entities already exist.
                                            /// </summary>
                                            /// <typeparam name="{{TEntity}}">The type of items in the base and changed collection.</typeparam>
                                            /// <param name="baseCollection">The base collection to be synchronized.</param>
                                            /// <param name="changedCollection">The collection to synchronize to.</param>
                                            /// <param name="equalityCheck">A predicate that determines if an item from the base collection matches an item from the changed collection.</param>
                                            /// <returns>The synchronized base collection.</returns>
                                            /// <remarks>
                                            /// This method does not create new entities or update existing ones. It only adds and removes items.
                                            /// Typically used for many-to-many relationships in EF Core where both sides already exist.
                                            /// If the changed collection is <see langword="null" />, the base collection will be cleared.
                                            /// </remarks>
                                            """);
                        method.AddParameter($"ICollection<{TEntity}>", "baseCollection");
                        method.AddParameter($"IEnumerable<{TEntity}>?", "changedCollection");
                        method.AddParameter($"Func<{TEntity}, {TEntity}, bool>", "equalityCheck");
                        method.AddStatement($$"""
                                            if (changedCollection == null)
                                            {
                                                foreach (var entity in baseCollection)
                                                {
                                                    baseCollection.Remove(entity);
                                                }
                                                return baseCollection;
                                            }
                                            """);
                        method.AddStatement("var result = baseCollection.CompareCollections(changedCollection, equalityCheck);", st => st.SeparatedFromPrevious());
                        method.AddStatement("""
                                            foreach (var elementToRemove in result.ToRemove)
                                            {
                                                baseCollection.Remove(elementToRemove);
                                            }
                                            """, st => st.SeparatedFromPrevious());
                        method.AddStatement("""
                                            foreach (var elementToAdd in result.ToAdd)
                                            {
                                                baseCollection.Add(elementToAdd);
                                            }
                                            """, st => st.SeparatedFromPrevious());
                        method.AddReturn("baseCollection", st => st.SeparatedFromPrevious());
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
    }
}